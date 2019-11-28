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

namespace PScan
{
    static class DATA_KEY
    {
        public const string RESULT = "RESULT";
        public const string DSMS_AUTH = "DSMS_AUTH";
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
        public bool IsDP = false;
        private  string _url = ConfigurationManager.AppSettings["url"] + "/Doc/"; 
        private  string _dpUrl = ConfigurationManager.AppSettings["dpPath"] + "/MWMS2/Doc/";
        private  string URL {
            get
            {
                return IsDP ? _dpUrl : _url;
            }
        }
        public HtmlRequester htmlRequester;
        public string Session { get; set; }
        public string LoginPostCode { get; set; }
        public bool alive { get; set; } = false;
        public List<string> groups { get; set; } = new List<string>();
       
        public DocServerKernal()
        {
            htmlRequester = HtmlRequester.Create();
        }

        public void GetSession()
        {
            JObject jObject;
            try
            {
                jObject = htmlRequester.ReqJson(URL + "GetSession", null);
            }
            catch(Exception ex)
            {
                Util.log("DocServerException.ConnectionProblem");
                throw DocServerException.ConnectionProblem;
            }
            if (jObject.ContainsKey(DATA_KEY.RESULT) &&
                    jObject[DATA_KEY.RESULT].ToString() == RESULT.SUCCESS &&
                    jObject.ContainsKey(DATA_KEY.SESSION))
            {
                Session = jObject[DATA_KEY.SESSION].ToString();
                Util.log("GetSession Success, Session : " + Session);
            }
            else
            {
                Util.log("DocServerException.Create(jObject)");
                throw DocServerException.Create(jObject);
            }
        }

        public void ChangeInfo(string username, string password)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("username", username);
            parameters.Add("password", password);
           
                JObject jObject = htmlRequester.ReqJson(URL + "ChangeInfo", parameters);
                if (jObject == null || !jObject.ContainsKey(DATA_KEY.RESULT)|| jObject[DATA_KEY.RESULT].ToString() != RESULT.SUCCESS)
                {
                Util.log("ChangeInfo() DocServerException.ConnectionProblem");
                    throw jObject.ContainsKey(DATA_KEY.MESSAGE) ? new Exception(jObject[DATA_KEY.MESSAGE].ToString()) : DocServerException.ConnectionProblem;

                }

            Util.log("ChangeInfo() Success");
        }

        public int Login(string username, string password)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("username", username);
            parameters.Add("password", password);

            JObject jObject = htmlRequester.ReqJson(URL + "Login", parameters);
            if (jObject == null || !jObject.ContainsKey(DATA_KEY.RESULT) || jObject[DATA_KEY.RESULT].ToString() != RESULT.SUCCESS)
            {
                Util.log("Login() DocServerException.ConnectionProblem");
                throw DocServerException.ConnectionProblem;
            }

            if (!jObject.ContainsKey(DATA_KEY.DSMS_AUTH))
            // if (((Boolean)jObject[DATA_KEY.DSMS_AUTH]) != false)
            {
                //FormResetDRMS resetForm = new FormResetDRMS();
                //resetForm.ShowDialog();
                //if (resetForm.Success == 0)
               // {
              //      return 0;
              //  }
            }

             jObject = htmlRequester.ReqJson(URL + "Login", parameters);
            string Session = jObject[DATA_KEY.DSMS_AUTH]["SessionID"].Value<string>();
            string groupIDs = jObject[DATA_KEY.DSMS_AUTH]["GroupIDs"].Value<string>();
            string[] ga = groupIDs.Split(';');
            groups.AddRange(ga);
            alive = true;
            Util.log("Login() Success");
            this.LoginPostCode = username;
            return 1;
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
                JObject jObject = htmlRequester.ReqJson(URL + "SearchDoc", parameters);
                if (jObject == null || !jObject.ContainsKey(DATA_KEY.RESULT))
                {
                    Util.log("SearchDoc() DocServerException.ConnectionProblem");
                    throw DocServerException.ConnectionProblem;
                }
                return jObject;
            }
            catch
            {
                Util.log("SearchDoc() DocServerException.ConnectionProblem");
                throw DocServerException.ConnectionProblem;
            }
        }
        public JObject LoadSubDocs(string dsn)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("dsn", dsn);
            try
            {
                JObject jObject = htmlRequester.ReqJson(URL + "LoadSubDocs", parameters);
                if (jObject == null || !jObject.ContainsKey(DATA_KEY.RESULT))
                {
                    Util.log("LoadSubDocs() DocServerException.ConnectionProblem");
                    throw DocServerException.ConnectionProblem;
                }
                return jObject;
            }
            catch(Exception ex)
            {
                Util.log("LoadSubDocs() DocServerException.ConnectionProblem");
                throw DocServerException.ConnectionProblem;
            }
        }
        public JObject LoadDocInfo(string dsn)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("dsn", dsn);
            try
            {
                JObject jObject = htmlRequester.ReqJson(URL + "LoadDocInfo", parameters);
                    if (jObject == null || !jObject.ContainsKey(DATA_KEY.RESULT))
                {
                    Util.log("LoadDocInfo() DocServerException.ConnectionProblem");
                    throw DocServerException.ConnectionProblem;
                }
                return jObject;
            }
            catch
            {
                Util.log("LoadDocInfo() DocServerException.ConnectionProblem");
                throw DocServerException.ConnectionProblem;
            }
        }

        public void Upload(string[] files, NameValueCollection parameters)
        {
            try
            {
                string rs = htmlRequester.PostFile(URL + "Upload", files, parameters);
                JObject jObject = JObject.Parse(rs);
                if (jObject == null || !jObject.ContainsKey(DATA_KEY.RESULT))
                {
                    Util.log("Upload() DocServerException.ConnectionProblem");
                    throw DocServerException.ConnectionProblem;

                }
                Util.log("Upload() Success");
            }
            catch(Exception ex)
            {

                Util.log("Upload() DocServerException.ConnectionProblem");
                throw DocServerException.ConnectionProblem;
            }
        }
        /*public void BatchUpdate(byte[] filePath, string sheet)
        {
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("sheet", sheet);
            try
            {
                string rs = htmlRequester.PostFile(url + "BatchUpdate", new string[] { filePath }, parameters);
                JObject jObject = JObject.Parse(rs);
                if (jObject == null || !jObject.ContainsKey(DATA_KEY.RESULT))
                {
                    Program.log("BatchUpdate() DocServerException.ConnectionProblem");
                    throw DocServerException.ConnectionProblem;

                }
                Program.log("BatchUpdate() Success");
            }
            catch (Exception ex)
            {

                Program.log("BatchUpdate() DocServerException.ConnectionProblem");
                throw DocServerException.ConnectionProblem;
            }
        }
        */
    }










}
