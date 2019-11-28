using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwainDotNet;

namespace PScan
{
    class MainService
    {
        public string HIST_DocumentType { get; set; } = null;
        public DateTime? HIST_LastSubmission { get; set; } = null;
        private static MainService _instance;
        private static readonly object locker = new object();



        public MainWindow MainWindow { get; set; }
        public WDoc WindowDoc { get; set; } = null;




        private HtmlRequester htmlRequester;
        private bool _isDP = false;
        private string _dpUrl = "https://dp2.bd.hksarg";
        private string _url = "https://mwms2.bd.hksarg";
        //private string _url = "https://10.5.132.187";
        //private string _url = "http://localhost:50194";
        private string _localPath = "C:\\ScanBackup\\";
        public string TempPath { get { return "C:\\ScanTemp\\"; } }


        public string SaveTempFile(string filename, MemoryStream ms)
        {
            if (!Directory.Exists(TempPath)) Directory.CreateDirectory(TempPath);
            DirectoryInfo di = new DirectoryInfo(TempPath);
            FileStream file = new FileStream(TempPath + filename, FileMode.Create, FileAccess.Write);
            ms.WriteTo(file);
            file.Close();
            ms.Close();
            return TempPath + filename;
        }


        public string _loginPostCode { get; set; }

        public bool Alive { get; set; } = false;
        public List<string> _groups { get; set; } = new List<string>();


        private string AppPATH { get { return _url + "/MWMS2"; } }
        private string ServicePATH { get { return (_isDP ? _dpUrl : _url) + "/MWMS2/Doc/"; } }

        private MainService()
        {
            htmlRequester = HtmlRequester.Create();
        }
        public static MainService Instance { get { if (_instance == null) lock (locker) if (_instance == null) _instance = new MainService(); return _instance; } }
        public string Save2Local(DRMS_DOCUMENT_META_DATA data)
        {
            string saveFilePath = _localPath + data.DSN + ".xlsx";
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add("Uploaded Document Data");

                Type t = typeof(DRMS_DOCUMENT_META_DATA);
                for (int i = 0; i < t.GetProperties().Length; i++)
                {
                    ws.Cells[1, i + 1].Value = t.GetProperties()[i].Name;
                }
                for (int i = 0; i < t.GetProperties().Length; i++)
                {
                    object value = data.GetType().GetProperty(t.GetProperties()[i].Name).GetValue(data, null);
                    ws.Cells[2, i + 1].Value = value == null ? "" : value;
                }
                ws.Cells.AutoFitColumns();
                byte[] bin = package.GetAsByteArray();
                if(!Directory.Exists(_localPath)) Directory.CreateDirectory(_localPath);
                if (File.Exists(saveFilePath))
                {
                    File.Move(saveFilePath, _localPath + data.DSN + "." + Util.NewUuid() + ".xlsx");
                }
                File.WriteAllBytes(saveFilePath, bin);
            }
            return saveFilePath;
        }
        public void Upload(List<string> files, NameValueCollection parameters)
        {
            if (!AskLogin()) throw DocServerException.Create(null);
            string rs = htmlRequester.PostFile(ServicePATH + "Upload", files.ToArray(), parameters);
            JObject jObject = JObject.Parse(rs);
            if (jObject == null || !jObject.ContainsKey(DATA_KEY.RESULT) || jObject[DATA_KEY.RESULT].ToString() != RESULT.SUCCESS)
            {
                throw DocServerException.ConnectionProblem;
            }
        }







        public void BatchUpdate(string filePath, string sheet)
        {
            if (!AskLogin()) throw DocServerException.Create(null);
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("sheet", sheet);
            string rs = htmlRequester.PostFile(ServicePATH + "BatchUpdate", new string[] { filePath }, parameters);
            JObject jObject = JObject.Parse(rs);
            if (jObject == null || !jObject.ContainsKey(DATA_KEY.RESULT))
            {
                throw DocServerException.ConnectionProblem;
            }
        }


        public WDoc OpenWDoc()
        {
            if (WindowDoc == null)
            {
                WindowDoc = new WDoc();
                WindowDoc.Closed += _wDoc_Closed;
                WindowDoc.Show();
            }
            WindowDoc.Focus();
            return WindowDoc;
        }

        private void _wDoc_Closed(object sender, EventArgs e)
        {
            WindowDoc = null;
        }


        public List<P_MW_SCANNED_DOCUMENT> LoadSubDocs(string dsn)
        {
            if (!AskLogin()) throw DocServerException.Create(null);
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("dsn", dsn);

            JObject jObject = htmlRequester.ReqJson(ServicePATH + "LoadSubDocs", parameters);
            if (jObject == null || !jObject.ContainsKey(DATA_KEY.RESULT) || jObject[DATA_KEY.RESULT].ToString() != RESULT.SUCCESS)
            {
                throw DocServerException.Create(jObject);
            }
            List<P_MW_SCANNED_DOCUMENT> rs = JsonConvert.DeserializeObject<List<P_MW_SCANNED_DOCUMENT>>(jObject["data"].ToString());
            return rs;
        }

        public P_MW_DSN GenerateNewDsn()
        {
            if (!AskLogin()) throw DocServerException.Create(null);
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            
            JObject jObject = htmlRequester.ReqJson(ServicePATH + "GenerateNewDsn", parameters);
            try
            {
                P_MW_DSN rs = JsonConvert.DeserializeObject<P_MW_DSN>(jObject["Data"]["Data"].ToString());
                return rs;
            } catch
            {
                throw DocServerException.Create(jObject);
            }
        }

        public P_MW_DSN LoadDocInfo(string dsn)
        {
            if (!AskLogin()) throw DocServerException.Create(null);
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("dsn", dsn);

            JObject jObject = htmlRequester.ReqJson(ServicePATH + "LoadDocInfo", parameters);
            if (jObject == null || !jObject.ContainsKey(DATA_KEY.RESULT) || jObject[DATA_KEY.RESULT].ToString() != RESULT.SUCCESS)
            {
                return null;
                //throw DocServerException.Create(jObject);
            }
            P_MW_DSN rs = JsonConvert.DeserializeObject<P_MW_DSN>(jObject["data"].ToString());
            return rs;
        }

        public P_MW_ACK_LETTER LoadDocRelated(string dsn)
        {
            if (!AskLogin()) throw DocServerException.Create(null);
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("dsn", dsn);

            JObject jObject = htmlRequester.ReqJson(ServicePATH + "LoadDocRelated", parameters);
            if (jObject == null || !jObject.ContainsKey(DATA_KEY.RESULT) || jObject[DATA_KEY.RESULT].ToString() != RESULT.SUCCESS)
            {
                throw DocServerException.Create(jObject);
            }
            if (jObject["data"] == null) return null;
            P_MW_ACK_LETTER rs = JsonConvert.DeserializeObject<P_MW_ACK_LETTER>(jObject["data"].ToString());
            return rs;
        }

        public List<DRMS_DOCUMENT_META_DATA> SearchDoc(string keyword, string stage = null, string dsn = null, DateTime? createdDateFrom = null, DateTime? createdDateTo = null)
        {
            if (!AskLogin()) throw DocServerException.Create(null);
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            parameters.Add("key0.word", keyword);
            parameters.Add("stage", stage);
            parameters.Add("dsn", dsn);
            parameters.Add("createdDateFrom", createdDateFrom == null ? "" : createdDateFrom.Value.Ticks.ToString());
            parameters.Add("createdDateTo", createdDateFrom == null ? "" : createdDateTo.Value.Ticks.ToString());

            JObject jObject = htmlRequester.ReqJson(ServicePATH + "SearchDoc", parameters);
            if (jObject == null || !jObject.ContainsKey(DATA_KEY.RESULT) || jObject[DATA_KEY.RESULT].ToString() != RESULT.SUCCESS)
            {
                return null;
                //throw DocServerException.Create(jObject);
            };

            List<DRMS_DOCUMENT_META_DATA> rs = JsonConvert.DeserializeObject<List<DRMS_DOCUMENT_META_DATA>>(jObject["data"].ToString());
            return rs;
        }
        private bool AskLogin()
        {
            if(!Alive)
            {
                WLogin windowLogin = new WLogin();
                windowLogin.ShowDialog();
            }
            return Alive;
        }


        public void LoginPortal(string username, string password)
        {
            string req =  htmlRequester.ReqText(_dpUrl, null);
            string findstr = "name=\"goto\" value=\"";
            int first = req.IndexOf(findstr) + findstr.Length;
            int len = req.IndexOf("\"", first) - first;
            string key = req.Substring(first, len);

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("IDToken1", username);
            parameters.Add("IDToken2", password);
            parameters.Add("IDButton", "Log In");
            parameters.Add("goto", key);
            parameters.Add("gotoOnFail", "");
            parameters.Add("SunQueryParamsString", "");
            parameters.Add("encoded", "true");
            parameters.Add("gx_charset", "UTF-8");

            string req2 = htmlRequester.ReqText(_dpUrl + ":8443/sso/UI/Login", parameters);
            bool sucesslogin = req2.IndexOf("Please Wait While Redirecting to console") > 0;
            if (sucesslogin)
            {
                string req3 = htmlRequester.ReqText(_dpUrl + "/MWMS2", parameters);
                bool sucesslogin2 = req3.IndexOf("Buildings Department - Minor Works Management System 2.0") > 0;
                if (sucesslogin2)
                {
                    //Program.docServerKernal.Login("", "");
                    Alive = true;
                    _isDP = true;
                    return;
                }
            }
            throw DocServerException.Create(null);
        }
        public void Login(string username, string password)
        {
            _groups.Clear();
            Alive = false;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("username", username);
            parameters.Add("password", password);

            JObject jObject = htmlRequester.ReqJson(ServicePATH + "Login", parameters);
            if (jObject == null || !jObject.ContainsKey(DATA_KEY.RESULT) || jObject[DATA_KEY.RESULT].ToString() != RESULT.SUCCESS)
            {
                throw DocServerException.Create(jObject);
            }

            if (!jObject.ContainsKey(DATA_KEY.DSMS_AUTH))
            {
                throw DocServerException.Create(jObject);
            }

            jObject = htmlRequester.ReqJson(ServicePATH + "Login", parameters);
            string Session = jObject[DATA_KEY.DSMS_AUTH]["SessionID"].Value<string>();
            string groupIDs = jObject[DATA_KEY.DSMS_AUTH]["GroupIDs"].Value<string>();
            string[] ga = groupIDs.Split(';');
            _groups.AddRange(ga);
            Alive = true;
            _loginPostCode = username;
        }
    }
}
