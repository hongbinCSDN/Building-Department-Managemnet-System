using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PScan
{

    class HtmlRequester
    {
        public string LoginSession { get; set; }
        private static volatile CookieContainer _CookieContainer;
        private static readonly object locker = new object();
        public static CookieContainer CookieContainer { set { _CookieContainer = value; } get { if (_CookieContainer == null) lock (locker) if (_CookieContainer == null) _CookieContainer = new CookieContainer(); return _CookieContainer; } }

        private HtmlRequester() { }
        public static HtmlRequester Create() { return new HtmlRequester(); }






        public string ReqText(string url, Dictionary<string, string> parameters)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = CookieContainer;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            //request.ClientCertificates.Add(new System.Security.Cryptography.X509Certificates.X509Certificate());
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
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


        public JObject ReqJson(string url, Dictionary<string, string> parameters)
        {
            return JObject.Parse(ReqText(url, parameters));
        }

        public string PostFile(string url, string[] files, NameValueCollection formFields = null)
        {
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = CookieContainer;
            request.ContentType = "multipart/form-data; boundary=" + boundary;
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
