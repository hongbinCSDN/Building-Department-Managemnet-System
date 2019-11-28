using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace MWMS2ScanTools
{
    static class DATA_KEY
    {
        public const string RESULT = "RESULT";
        public const string SESSION = "SESSION";
        public const string MESSAGE = "MESSAGE";
    }
    static class RESULT
    {
        public const string SUCCESS = "SUCCESS";
        public const string FAILURE = "FAILURE";
        public const string CONNECTION_ERROR = "CONNECTION_ERROR";
        public const string REQ_CHANGE_INFO = "REQ_CHANGE_INFO";
        public const string REQ_LOGIN = "REQ_LOGIN";
    }

    class DocServerKernal
    {
        private readonly string url = ConfigurationManager.AppSettings["url"] + "/Doc/"; //"http://localhost:50194/MWMS2/Doc/";
        public HtmlRequester htmlRequester;
        private string Session { get; set; }
        public bool alive = false;

        public DocServerKernal()
        {
            htmlRequester = new HtmlRequester();
        }

        public void GetSession()
        {
            JObject jObject;
            try
            {
                jObject = htmlRequester.ReqJson(url + "GetSession", null);
            }
            catch(Exception ex)
            {
                Program.log("DocServerException.ConnectionProblem");
                throw DocServerException.ConnectionProblem;
            }
            if (jObject.ContainsKey(DATA_KEY.RESULT) &&
                    jObject[DATA_KEY.RESULT].ToString() == RESULT.SUCCESS &&
                    jObject.ContainsKey(DATA_KEY.SESSION))
            {
                Session = jObject[DATA_KEY.SESSION].ToString();
                Program.log("GetSession Success, Session : " + Session);
            }
            else
            {
                Program.log("DocServerException.Create(jObject)");
                throw DocServerException.Create(jObject);
            }
        }

        public void ChangeInfo(string username, string password)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("username", username);
            parameters.Add("password", password);
            try
            {
                JObject jObject = htmlRequester.ReqJson(url + "ChangeInfo", parameters);
                if (jObject == null || !jObject.ContainsKey(DATA_KEY.RESULT))
                {
                    Program.log("ChangeInfo() DocServerException.ConnectionProblem");
                    throw DocServerException.ConnectionProblem;

                }

                Program.log("ChangeInfo() Success");
            } catch
            {
                Program.log("ChangeInfo() DocServerException.ConnectionProblem");
                throw DocServerException.ConnectionProblem;
            }
        }

        public void Login(string username, string password)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("username", username);
            parameters.Add("password", password);
            try
            {
                JObject jObject = htmlRequester.ReqJson(url + "Login", parameters);
                if (jObject == null || !jObject.ContainsKey(DATA_KEY.RESULT) || jObject[DATA_KEY.RESULT].ToString() != RESULT.SUCCESS)
                {
                    Program.log("Login() DocServerException.ConnectionProblem");
                    throw DocServerException.ConnectionProblem;

                }
                alive = true;
                Program.log("Login() Success");
            }
            catch(Exception ex)
            {
                Program.log("Login() DocServerException.ConnectionProblem");
                throw DocServerException.ConnectionProblem;
            }
        }

        public JObject SearchDoc(string keyword, string stage = null, string dsn = null, DateTime? createdDateFrom = null, DateTime? createdDateTo = null)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("keyword", keyword);
            parameters.Add("stage", stage);
            parameters.Add("dsn", dsn);
            parameters.Add("createdDateFrom", createdDateFrom == null ? "" : createdDateFrom.Value.Ticks.ToString());
            parameters.Add("createdDateTo", createdDateFrom == null ? "" : createdDateTo.Value.Ticks.ToString());
            try
            {
                JObject jObject = htmlRequester.ReqJson(url + "SearchDoc", parameters);
                if (jObject == null || !jObject.ContainsKey(DATA_KEY.RESULT))
                {
                    Program.log("SearchDoc() DocServerException.ConnectionProblem");
                    throw DocServerException.ConnectionProblem;
                }
                return jObject;
            }
            catch
            {
                Program.log("SearchDoc() DocServerException.ConnectionProblem");
                throw DocServerException.ConnectionProblem;
            }
        }

        public JObject LoadDocInfo(string dsn)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("dsn", dsn);
            try
            {
                JObject jObject = htmlRequester.ReqJson(url + "LoadDocInfo", parameters);
                    if (jObject == null || !jObject.ContainsKey(DATA_KEY.RESULT))
                {
                    Program.log("LoadDocInfo() DocServerException.ConnectionProblem");
                    throw DocServerException.ConnectionProblem;
                }
                return jObject;
            }
            catch
            {
                Program.log("LoadDocInfo() DocServerException.ConnectionProblem");
                throw DocServerException.ConnectionProblem;
            }
        }

        public void Upload(string[] files, NameValueCollection parameters)
        {
            try
            {
                string rs = htmlRequester.PostFile(url + "Upload", files, parameters);
                JObject jObject = JObject.Parse(rs);
                if (jObject == null || !jObject.ContainsKey(DATA_KEY.RESULT))
                {
                    Program.log("Upload() DocServerException.ConnectionProblem");
                    throw DocServerException.ConnectionProblem;

                }
                Program.log("Upload() Success");
            }
            catch(Exception ex)
            {

                Program.log("Upload() DocServerException.ConnectionProblem");
                throw DocServerException.ConnectionProblem;
            }
        }
    }


    class HtmlRequester
    {
        //private readonly CookieContainer _CookieContainer;
        //public readonly CookieContainer _CookieContainer;
        private static volatile CookieContainer _CookieContainer;
        private static readonly object locker = new object();
        public static CookieContainer CookieContainer { set { _CookieContainer = value; } get { if (_CookieContainer == null) lock (locker) if (_CookieContainer == null) _CookieContainer = new CookieContainer(); return _CookieContainer; } }


        public HtmlRequester()
        {
        }

        public string ReqText(string url, Dictionary<string, string> parameters)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.CookieContainer = CookieContainer;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                NameValueCollection postParams = HttpUtility.ParseQueryString(string.Empty);
                if (parameters != null) foreach (KeyValuePair<string, string> kv in parameters) postParams.Add(kv.Key, kv.Value);
                byte[] byteArray = Encoding.UTF8.GetBytes(postParams.ToString());
                using (Stream reqStream = request.GetRequestStream()) reqStream.Write(byteArray, 0, byteArray.Length);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string data = null;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream = null;
                    if (response.CharacterSet == null) readStream = new StreamReader(receiveStream);
                    else readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    data = readStream.ReadToEnd();
                    response.Close();
                    readStream.Close();
                }
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public JObject ReqJson(string url, Dictionary<string, string> parameters)
        {
            return JObject.Parse(ReqText(url, parameters));
        }

        public string PostFile(string url, string[] files, NameValueCollection formFields = null)
        {
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = CookieContainer;
            request.ContentType = "multipart/form-data; boundary=" +boundary;
            request.Method = "POST";
            request.KeepAlive = true;

            Stream memStream = new System.IO.MemoryStream();
            var boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            var endBoundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--");

            string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";

            if (formFields != null)
            {
                foreach (string key in formFields.Keys)
                {
                    string formitem = string.Format(formdataTemplate, key, formFields[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    memStream.Write(formitembytes, 0, formitembytes.Length);
                }
            }
            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" + "Content-Type: application/octet-stream\r\n\r\n";

            for (int i = 0; i < files.Length; i++)
            {
                memStream.Write(boundarybytes, 0, boundarybytes.Length);
                var header = string.Format(headerTemplate, "uplTheFile", files[i]);
                var headerbytes = System.Text.Encoding.UTF8.GetBytes(header);

                memStream.Write(headerbytes, 0, headerbytes.Length);

                using (var fileStream = new FileStream(files[i], FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[1024];
                    var bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        memStream.Write(buffer, 0, bytesRead);
                    }
                }
            }

            memStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
            request.ContentLength = memStream.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                memStream.Position = 0;
                byte[] tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();
                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            }

            using (var response = request.GetResponse())
            {
                Stream stream2 = response.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                return reader2.ReadToEnd();
            }
        }

    }









}
