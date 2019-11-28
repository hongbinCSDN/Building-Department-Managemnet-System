using MWMS2.Areas.Admin.Models;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Utility;
using System.Data.Entity;
using MWMS2.Controllers;
using MWMS2.Constant;
using System.Xml;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Configuration;
using System.Transactions;
using System.IO;
using System.Text;
using System.Collections.Specialized;
using Ghostscript.NET.Rasterizer;
using Ghostscript.NET;
using System.Drawing.Imaging;
using System.Drawing;
using System.Reflection;

namespace MWMS2.Services
{
    static class S_DATA_KEY
    {
        public const string RESULT = "RESULT";
        public const string SESSION = "SESSION";
        public const string MESSAGE = "MESSAGE";
    }
    static class S_RESULT
    {
        public const string SUCCESS = "SUCCESS";
        public const string FAILURE = "FAILURE";
        public const string CONNECTION_ERROR = "CONNECTION_ERROR";
        public const string REQ_CHANGE_INFO = "REQ_CHANGE_INFO";
        public const string REQ_LOGIN = "REQ_LOGIN";
    }

    public class S_DocBLService
    {/*
        private static volatile AtcpDAOService _DAO;
        private static readonly object locker = new object();
        private static AtcpDAOService DAO { get { if (_DAO == null) lock (locker) if (_DAO == null) _DAO = new AtcpDAOService(); return _DAO; } }
        */
        //private readonly static string RESULT_SUCCESS = "RESULT_SUCCESS";
        //private readonly static string RESULT_REQ_CHANGE_INFO = "RESULT_REQ_CHANGE_INFO";


        public dynamic GetSession(SMMDocModel model)
        {
            if(SessionUtil.LoginPost == null)
            {
                //portal invalid
                return new { RESULT = RESULT.REQ_LOGIN };
            }
            string DSMS_USERNAME = SessionUtil.LoginPost.DSMS_USERNAME;
            string DSMS_PW = SessionUtil.LoginPost.DSMS_PW;
            string CODE = SessionUtil.LoginPost.CODE;
            string BD_PORTAL_LOGIN = SessionUtil.LoginPost.BD_PORTAL_LOGIN;
            if (!string.IsNullOrWhiteSpace(DSMS_USERNAME) && !string.IsNullOrWhiteSpace(DSMS_PW)) 
            {
                XmlDocument result = CallWebService("SystemLogin", new Dictionary<string, string>() { ["userName"] = DSMS_USERNAME, ["password"] = DSMS_PW ,["language"] = "0"});
                if(result.GetElementsByTagName("SessionID").Count != 0)
                {
                    return new { RESULT = RESULT.SUCCESS, DSMS_USERNAME, CODE, BD_PORTAL_LOGIN, SESSION = result.GetElementsByTagName("SessionID")[0].InnerText };
                }
            }
            return new { RESULT = RESULT.REQ_CHANGE_INFO, DSMS_USERNAME, CODE, BD_PORTAL_LOGIN };
        }
        public dynamic ChangeInfo(SMMDocModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Username) && !string.IsNullOrWhiteSpace(model.Password))
            {
                XmlDocument result = CallWebService("SystemLogin", new Dictionary<string, string>() { ["userName"] = model.Username, ["password"] = model.Password, ["language"] = "0" });
                if (result.GetElementsByTagName("SessionID").Count != 0)
                {
                    using (EntitiesAuth db = new EntitiesAuth())
                    {
                        SYS_POST sysPost = new SYS_POST() { UUID = SessionUtil.LoginPost.UUID };
                        sysPost.DSMS_USERNAME = model.Username;
                        sysPost.DSMS_PW = model.Password;
                        db.SYS_POST.Attach(sysPost);
                        db.SaveChanges();
                    }
                    return new { RESULT = RESULT.SUCCESS, SESSION = result.GetElementsByTagName("SessionID")[0].InnerText };
                }
                else
                {
                    //create user
                    XmlDocument loginXml = CallWebService("SystemLogin", new Dictionary<string, string>() { ["userName"] = "BdUser", ["password"] = "password", ["language"] = "0" });
                    string sesionId = loginXml.GetElementsByTagName("SessionID")[0].InnerText;
                    XmlDocument crerateRes = CallWebService("UserCreate",
                        new Dictionary<string, string>()
                        {
                            ["sessionIDIn"] = sesionId
                            ,
                            ["userInfo"] = "<Fullname>" + model.Username + "</Fullname><Password>" + model.Password + "</Password >"
                        });
                    Console.WriteLine(crerateRes);
                }
            }
            return null;
        }


        public dynamic Login(SMMDocModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Username) && !string.IsNullOrWhiteSpace(model.Password))
            {

                string findPassword = model.Password == null ? "" : EncryptDecryptUtil.getEncrypt(model.Password);
                using (EntitiesAuth db = new EntitiesAuth())
                {
                    SYS_POST SYS_POST = db.SYS_POST
                        .Where(o => o.BD_PORTAL_LOGIN == model.Username && o.PW == findPassword)
                       .FirstOrDefault();
                    SessionUtil.LoginPost = SYS_POST;
                }
                if(SessionUtil.LoginPost == null)
                    return new { RESULT = RESULT.FAILURE };
                return new { RESULT = RESULT.SUCCESS };




                //XmlDocument result = CallWebService("Login", new Dictionary<string, string>() { ["userName"] = model.Username, ["password"] = model.Password, ["language"] = "0" });
                //if (result.GetElementsByTagName("SessionID").Count != 0)
                //{
                //    using (EntitiesAuth db = new EntitiesAuth())
                //    {
                //        SYS_POST sysPost = new SYS_POST() { UUID = SessionUtil.LoginPost.UUID };
                //        sysPost.DSMS_USERNAME = model.Username;
                //        sysPost.DSMS_PW = model.Password;
                //        db.SYS_POST.Attach(sysPost);
                //        db.SaveChanges();
                //    }
                //    return new { RESULT = RESULT.SUCCESS, SESSION = result.GetElementsByTagName("SessionID")[0].InnerText };
                //}
                //else
                //{
                //    //create user
                //    XmlDocument loginXml = CallWebService("SystemLogin", new Dictionary<string, string>() { ["userName"] = "BdUser", ["password"] = "password", ["language"] = "0" });
                //    string sesionId = loginXml.GetElementsByTagName("SessionID")[0].InnerText;
                //    XmlDocument crerateRes = CallWebService("UserCreate",
                //        new Dictionary<string, string>()
                //        {
                //            ["sessionIDIn"] = sesionId
                //            ,
                //            ["userInfo"] = "<Fullname>" + model.Username + "</Fullname><Password>" + model.Password + "</Password >"
                //        });
                //    Console.WriteLine(crerateRes);
                //}
            }
            return null;
        }


        public dynamic LoadDir(SMMDocModel model)
        {//sessionIDIn

            string sessionIDIn = model.Session;
            if (!string.IsNullOrWhiteSpace(sessionIDIn))
            {
                XmlDocument result = CallWebService("DirectoryQueryBatch", new Dictionary<string, string>() { ["sessionIDIn"] = sessionIDIn});
                if (result != null)
                {
                }
            }
           

            return null;
        }
        public dynamic LoadDocInfo(SMMDocModel model)
        {//sessionIDIn
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                if (model == null) return new { RESULT = false };
                if (model.Dsn == null) return new { RESULT = false };
                B_SV_SCANNED_DOCUMENT dsn = db.B_SV_SCANNED_DOCUMENT.Where(o => o.DSN_NUMBER == model.Dsn)
                   .FirstOrDefault();
                if (dsn == null) return new { RESULT = false };
                return new { RESULT = true, data = dsn };
            }
        }
        public dynamic SearchDoc(SMMDocModel model)
        {//sessionIDIn
            using (EntitiesDRMS db = new EntitiesDRMS())
            {
                dynamic data;
                if(model.Keyword != null)
                {
                    data = db.DRMS_DOCUMENT_META_DATA
                    .Where(o => o.SUBMISSION_LETTER_DATE != null)
                    .Where(o=> o.DSN.StartsWith(model.Keyword)
                    || o.EFSS_SUBMISSION_REF_NO.StartsWith(model.Keyword)
                    || o.BD_FILE_REF.StartsWith(model.Keyword))
                    .OrderBy(o => o.SUBMISSION_LETTER_DATE)
                    //.Select(o => new { o.DSN, YEAR = o.SUBMISSION_LETTER_DATE.Substring(0, 4), o.STATUS, DATE = o.SUBMISSION_LETTER_DATE })
                    .ToList();
                } else
                {
                    data = db.DRMS_DOCUMENT_META_DATA
                    .Where(o => o.SUBMISSION_LETTER_DATE != null)
                    .OrderBy(o => o.SUBMISSION_LETTER_DATE)
                    //.Select(o => new { o.DSN, YEAR = o.SUBMISSION_LETTER_DATE.Substring(0, 4), o.STATUS, DATE = o.SUBMISSION_LETTER_DATE })
                    .ToList();
                }
                return new { RESULT = true, data };
            }
        }

        

        public ActionResult DeleteDoc(SMMDocModel model)
        {
            return null;
        }

        public dynamic Download(string fileType, string fileUuid)
        {
            if (fileType == null) return null;

            if ("PEM" == fileType)
            {
                using(EntitiesMWProcessing db = new EntitiesMWProcessing())
                {
                    P_MW_SCANNED_DOCUMENT doc = db.P_MW_SCANNED_DOCUMENT.Where(o => o.FILE_PATH == fileUuid).FirstOrDefault();
                    if(doc != null)
                    {
                        string PEMFilePath = ApplicationConstant.PEMFilePath;
                        if (!Directory.Exists(PEMFilePath)) Directory.CreateDirectory(PEMFilePath);
                        if (File.Exists(PEMFilePath + doc.FILE_PATH))
                        {
                            byte[] fileBytes = System.IO.File.ReadAllBytes(PEMFilePath + doc.FILE_PATH);
                            string fileName = doc.DSN_SUB;
                            return new { data = fileBytes, type = System.Net.Mime.MediaTypeNames.Application.Pdf, name = fileName+".pdf" };

                        }
                    }
                }
            }
            else if ("SMM" == fileType)
            {

            }
            return null;
        }



        private static GhostscriptVersionInfo GhostScriptVersion => GetGhostscriptVersionInfo();
        private static GhostscriptVersionInfo GetGhostscriptVersionInfo()
        {
            //var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var vesion = new GhostscriptVersionInfo(new Version(0, 0, 0), System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "bin") + @"\gsdll32.dll", string.Empty,
                GhostscriptLicense.GPL);

            return vesion;
        }
        private const int DesiredXDpi = 1;
        private const int DesiredYDpi = 1;



        private static void PdfToImage(string fileName, string outPdfImagePath, string file, int w, int h)
        {
            var tempFiles = new List<string>();

            using (var rasterizer = new GhostscriptRasterizer())
            {
                rasterizer.Open(file, GhostScriptVersion, false);

                for (var i = 1; i <= 1; i++)
                {
                    // var fileName = Path.Combine(outPdfTempImagePath, GetUniqueFileName(".jpg", outPdfTempImagePath));
                    tempFiles.Add(fileName);

                    var img = rasterizer.GetPage(DesiredXDpi, DesiredYDpi, i);

                    if (i == 1)
                    {
                        var test = img.GetThumbnailImage(1200, 1600, null, IntPtr.Zero);
                        test.Save(fileName, ImageFormat.Jpeg);

                    }
                    else
                    {
                        img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        img.Save(fileName, ImageFormat.Png);
                    }
                }
                rasterizer.Close();
            }

            // outImageFileName = GetUniqueFileName(".png", outPdfImagePath);
            // MergeImages(tempFiles).Save(Path.Combine(outPdfImagePath, outImageFileName));
        }






        public dynamic Upload(SMMDocModel model, HttpFileCollectionBase files)
        {
            if (SessionUtil.LoginPost == null)
            {
                //portal invalid
                return new { RESULT = RESULT.REQ_LOGIN };
            }
            string SMMFilePath = ApplicationConstant.SMMSCANFilePath;
            if (!System.IO.Directory.Exists(SMMFilePath)) Directory.CreateDirectory(SMMFilePath);



            using (var scope = new TransactionScope())
            {
                //List< B_SV_SCANNED_DOCUMENT> docs;
                B_SV_SCANNED_DOCUMENT dsn;
                string fileName = CommonUtil.NewUuid();
                using (EntitiesSignboard db = new EntitiesSignboard())
                {
                    //using (DbContextTransaction transaction = db.Database.BeginTransaction())
                    //{

                    DateTime datetimeNow = DateTime.Now;

                    string data = model.Data;
                    data = Encoding.UTF8.GetString(Convert.FromBase64String(data));



                    NameValueCollection nv = new NameValueCollection();
                    string[] lines = data.Replace("\r\n", "\n").Split('\n');
                    //string[] lines = File.ReadAllLines(fi[i].FullName);
                    for (int j = 0; j < lines.Length; j++)
                    {
                        if (lines[j] == "") break;
                        nv.Add(lines[j], lines[++j]);
                    }
                    // return null;

                    //DSN = nv["doc.DSN"],
                    //        DOCUMENT_TYPE = nv["doc.DOCUMENT_TYPE"],
                    //        FOLDER_TYPE = nv["doc.FOLDER_TYPE"],
                    //        FILE_DESCRIPTION = nv["doc.FILE_DESCRIPTION"]

                    string tempDSN = nv["doc.DSN"].ToString();
                    dsn = db.B_SV_SCANNED_DOCUMENT.Where(o => o.DSN_NUMBER == tempDSN).FirstOrDefault();


                    for (int i= 0;i < files.Count; i++)
                    {
                        if(files[i].ContentLength !=0 &&  nv["_SUB" + i] != null)
                        {
                            JObject subDocData = JObject.Parse(nv["_SUB" + i]);

                            // B_SV_SCANNED_DOCUMENT doc = new B_SV_SCANNED_DOCUMENT();

                            string suffixPath = DateTime.Now.Year.ToString() + ApplicationConstant.FileSeparator.ToString() + DateTime.Now.Month.ToString() + ApplicationConstant.FileSeparator.ToString() + nv["doc.DSN"].ToString(); ;
                            string filePath = SMMFilePath + suffixPath;
                            string file_Name = ApplicationConstant.FileSeparator.ToString() + nv["doc.DSN"].ToString() + ".pdf";
                            System.IO.Directory.CreateDirectory(filePath);
                           string  fileFullPath =filePath+ file_Name;


                            if (File.Exists(fileFullPath))
                            {
                                File.Move(fileFullPath, filePath + CommonUtil.NewUuid() + ".pdf");
                            }
                            else
                            {
                               
                            }

                            files[i].SaveAs(fileFullPath);

                            string JpegFilePath = fileFullPath.Replace(".pdf", ".jpg");
                            PdfToImage(JpegFilePath, "", fileFullPath, 1200, 1600);
                            // doc.D = nv["doc.DSN"].ToString() + subDocData["fDSN_SUB"].ToString() ;


                            dsn.FILE_PATH = fileFullPath;

                            
                            dsn.RELATIVE_FILE_PATH = ApplicationConstant.FileSeparator.ToString() + suffixPath + file_Name;
                            // doc.PAGE_COUNT = model.PageCount;
                            dsn.DOCUMENT_TYPE = nv["doc.DOCUMENT_TYPE"].ToString();
                            dsn.SCAN_DATE = datetimeNow;
                            dsn.FILE_SIZE_CODE = "A4";
                            dsn.DOC_DESCRIPTION = nv["doc.FILE_DESCRIPTION"].ToString();
                            dsn.FOLDER_TYPE = nv["doc.FOLDER_TYPE"].ToString();
                            //db.B_SV_SCANNED_DOCUMENT.Add(doc);
                            db.SaveChanges();

                        }
                        break;
                    }
                
                    
                }

                scope.Complete();
            }






         
            return new { RESULT = RESULT.SUCCESS };
        }



















        /*


                POST /vdws/VD_WS_Server.asmx HTTP/1.1
        Host: 202.83.247.87
        Content-Type: application/soap+xml; charset=utf-8
        Content-Length: length

            */


        //webservice
        private static readonly string vitalDocURL = "http://202.83.247.87:8082/vdws/VD_WS_Server.asmx";
        private static readonly string reqStr1 = "<soap12:Envelope" +
            " xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"" +
            " xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"" +
            " xmlns:soap12=\"http://www.w3.org/2003/05/soap-envelope\"><soap12:Body>";
        private static readonly string reqStr2 = "</soap12:Body></soap12:Envelope>";
        public static XmlDocument CallWebService(string func, Dictionary<string, string> paramaters)
        {
            HttpWebRequest webRequest = CreateWebRequest(vitalDocURL, vitalDocURL + "?op=" + func);
            webRequest.ContentType = "application/soap+xml; charset=utf-8";
            XmlDocument soapEnvelopeDocument = new XmlDocument();
            string reqBody = "<" + func + " xmlns=\"http://tempuri.org/\">";
            if (paramaters != null) foreach (KeyValuePair<string, string> kv in paramaters) reqBody += "<" + kv.Key + ">" + kv.Value + "</" + kv.Key + ">";
            reqBody += "</" + func + ">";
            soapEnvelopeDocument.LoadXml(reqStr1 + reqBody + reqStr2);
            using (Stream stream = webRequest.GetRequestStream()) { soapEnvelopeDocument.Save(stream); }


            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);
            asyncResult.AsyncWaitHandle.WaitOne();
            string soapResult;
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            {
                using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
                {
                    soapResult = rd.ReadToEnd();
                }
                Console.Write(soapResult);
            }


            XmlDocument doc = new XmlDocument();
            doc.LoadXml(soapResult);
            //JObject jObject = JObject.Parse(JsonConvert.SerializeXmlNode(doc.GetElementsByTagName("soap:Body")[0].ChildNodes[0]));
            return doc;
        }

        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
    }
}