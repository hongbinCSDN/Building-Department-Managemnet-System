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
using OfficeOpenXml;
using System.Reflection;
using System.Data.Entity.Core.EntityClient;

namespace MWMS2.Services
{
    static class DATA_KEY
    {
        public const string RESULT = "RESULT";
        public const string SESSION = "SESSION";
        public const string MESSAGE = "MESSAGE";
        public const string DSMS_AUTH = "DSMS_AUTH";
    }
    static class RESULT
    {
        public const string SUCCESS = "SUCCESS";
        public const string FAILURE = "FAILURE";
        public const string CONNECTION_ERROR = "CONNECTION_ERROR";
        public const string REQ_CHANGE_INFO = "REQ_CHANGE_INFO";
        public const string REQ_LOGIN = "REQ_LOGIN";
    }

    public class DocBLService
    {/*
        private static volatile AtcpDAOService _DAO;
        private static readonly object locker = new object();
        private static AtcpDAOService DAO { get { if (_DAO == null) lock (locker) if (_DAO == null) _DAO = new AtcpDAOService(); return _DAO; } }
        */
     //private readonly static string RESULT_SUCCESS = "RESULT_SUCCESS";
     //private readonly static string RESULT_REQ_CHANGE_INFO = "RESULT_REQ_CHANGE_INFO";


        private static volatile ProcessingIncomingBLService _processingIncomingBL;
        private static readonly object locker = new object();
        private static ProcessingIncomingBLService ProcessingIncomingBL { get { if (_processingIncomingBL == null) lock (locker) if (_processingIncomingBL == null) _processingIncomingBL = new ProcessingIncomingBLService(); return _processingIncomingBL; } }



        private dynamic GetVDSession(SYS_POST sysPost)
        {
            if (sysPost == null) return null;
            if (string.IsNullOrWhiteSpace(sysPost.DSMS_PW)) return null;
            if (string.IsNullOrWhiteSpace(sysPost.DSMS_USERNAME)) return null;
            XmlDocument result = CallWebService("SystemLogin", new Dictionary<string, string>()
            {
                ["userName"] = sysPost.DSMS_USERNAME,
                ["password"] = sysPost.DSMS_PW,
                ["language"] = "0" });
            if (result.GetElementsByTagName("SessionID").Count != 0)
            {
                string session =  result.GetElementsByTagName("SessionID")[0].InnerText;
                //return result.GetElementsByTagName("SessionID")[0].InnerText;
                if (result.GetElementsByTagName("FullName").Count != 0)
                {
                    string grps = "";
                    for(int i=  0;i < result.GetElementsByTagName("FullName").Count; i++)
                    {
                        if (i > 0) grps = grps + ";";
                        grps = grps + result.GetElementsByTagName("FullName")[i].InnerText;
                    }
                    return new { SessionID = session, GroupIDs = grps };
                    //return result.GetElementsByTagName("SessionID")[0].InnerText;
                }
                else
                {
                    return new { SessionID = session };
                }
            }
            return null;
        }   
        public dynamic ChangeInfo(DocModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Username) && !string.IsNullOrWhiteSpace(model.Password))
            {
                XmlDocument result = CallWebService("SystemLogin", new Dictionary<string, string>() { ["userName"] = model.Username, ["password"] = model.Password, ["language"] = "0" });
                if (result.GetElementsByTagName("SessionID").Count != 0)
                {
                    using (EntitiesAuth db = new EntitiesAuth())
                    {
                        //SYS_POST sysPost =  new SYS_POST() { UUID = SessionUtil.LoginPost.UUID };
                        SYS_POST sysPost = db.SYS_POST.Where(o => o.UUID == SessionUtil.LoginPost.UUID).FirstOrDefault();
                        sysPost.DSMS_USERNAME = model.Username;
                        sysPost.DSMS_PW = model.Password;
                        
                        db.SaveChanges();
                    }
                    return new { RESULT = RESULT.SUCCESS };
                }
                else if(true)
                {

                    if (result.GetElementsByTagName("Success")[0].InnerText == "false")
                    {
                        if (result.GetElementsByTagName("Description").Count == 0) return new { RESULT = RESULT.FAILURE, MESSAGE = "VitalDoc error without error message, please login again." };
                        string message = result.GetElementsByTagName("Description")[0].InnerText;
                        if (string.IsNullOrWhiteSpace(message)) return new { RESULT = RESULT.FAILURE, MESSAGE = "VitalDoc error with blank error message, please login again." };


                        int x = 1;
                        while (true)
                        {
                            if (result.GetElementsByTagName("Parameter" + x).Count > 0)
                            {
                                message = message.Replace("[%" + x + "]", result.GetElementsByTagName("Parameter" + x)[0].InnerText);
                                x++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        return new { RESULT = RESULT.FAILURE, MESSAGE = message };
                    }
                    return new { RESULT = RESULT.FAILURE, MESSAGE = "VitalDoc user not exists." };
                }
                else
                {
                    SYS_POST sysPost = SessionUtil.LoginPost;
                    if(sysPost == null) return new { RESULT = RESULT.FAILURE, MESSAGE = "Session timeout, please login again."};
                    //create user
                    XmlDocument loginXml = CallWebService("SystemLogin", new Dictionary<string, string>() { ["userName"] = "administrator", ["password"] = "P@ssw0rd01", ["language"] = "0" });
                    if(loginXml.GetElementsByTagName("SessionID").Count == 0)
                    {
                        return new { RESULT = RESULT.FAILURE };
                    }
                    string sesionId = loginXml.GetElementsByTagName("SessionID")[0].InnerText;
                    XmlDocument crerateRes = CallWebService("UserCreateByData",
                        new Dictionary<string, string>()
                        {
                            ["sessionIDIn"] = sesionId,
                            ["userLoginName"] = sesionId,
                            ["userFullName"] = sysPost.CODE,
                            ["userEmail"] = sysPost.EMAIL,
                            ["userPwd"] = model.Password,
                            ["userFieldContent"] = "",
                            ["userGroups"] = "MWUsers",
                            ["userPrivileges"] = ""
                        });
                    Console.WriteLine(crerateRes);

                    if (crerateRes.GetElementsByTagName("Success")[0].InnerText == "false") 
                    {
                        if(crerateRes.GetElementsByTagName("Description").Count == 0) return new { RESULT = RESULT.FAILURE, MESSAGE = "VitalDoc error without error message, please login again." };
                        string message = crerateRes.GetElementsByTagName("Description")[0].InnerText;
                        if (string.IsNullOrWhiteSpace(message)) return new { RESULT = RESULT.FAILURE, MESSAGE = "VitalDoc error with blank error message, please login again." };


                        int x = 1;
                        while (true)
                        {
                            if (crerateRes.GetElementsByTagName("Parameter" + x).Count > 0) {
                                message = message.Replace("[%" + x +"]", crerateRes.GetElementsByTagName("Parameter" + x)[0].InnerText);
                                x++;
                            } else
                            {
                                break;
                            }
                        }

                        return new { RESULT = RESULT.FAILURE, MESSAGE = message };
                    }
                    using (EntitiesAuth db = new EntitiesAuth())
                    {
                        //SYS_POST sysPost =  new SYS_POST() { UUID = SessionUtil.LoginPost.UUID };
                        SYS_POST sysPostdb = db.SYS_POST.Where(o => o.UUID == SessionUtil.LoginPost.UUID).FirstOrDefault();
                        sysPostdb.DSMS_USERNAME = model.Username;
                        sysPostdb.DSMS_PW = model.Password;
                        db.SaveChanges();
                    }
                    return new { RESULT = RESULT.SUCCESS};
                }
            }
            return null;
        }


        public dynamic Login(DocModel model)
        {
            SYS_POST sysPost;
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                sysPost = SessionUtil.LoginPost;
            }
            else
            {
                string findPassword = model.Password == null ? "" : EncryptDecryptUtil.getEncrypt(model.Password);
                using (EntitiesAuth db = new EntitiesAuth())
                {
                    sysPost = db.SYS_POST
                        .Where(o => o.BD_PORTAL_LOGIN == model.Username && o.PW == findPassword)
                       .FirstOrDefault();
                    SessionUtil.LoginPost = sysPost;
                }
            }
            if (SessionUtil.LoginPost == null) return new { RESULT = RESULT.FAILURE };
            dynamic tempSession = GetVDSession(sysPost);
            if(tempSession == null)
                return new { RESULT = RESULT.SUCCESS };
            else
                return new { RESULT = RESULT.SUCCESS, DSMS_AUTH = tempSession };
        }


        public dynamic LoadDir(DocModel model)
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
        public dynamic LoadSubDocs(DocModel model)
        {//sessionIDIn
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                db.Configuration.ProxyCreationEnabled = false;
                if (model == null) return new { RESULT = false };
                if (model.Dsn == null) return new { RESULT = false };
                List< P_MW_SCANNED_DOCUMENT> P_MW_SCANNED_DOCUMENTs = db.P_MW_SCANNED_DOCUMENT.Where(o => o.P_MW_DSN.DSN == model.Dsn).ToList();
               // P_MW_DSN dsn = db.P_MW_DSN.Where(o => o.DSN == model.Dsn).ToList()
              //      .Select(o => new P_MW_DSN() {
              //          DSN_SUB = o.

                        /*
                        doc.DSN_SUB = nv["doc.DSN"].ToString() + subDocData["fDSN_SUB"].ToString() ;
                doc.P_MW_DSN = dsn;
                doc.FILE_PATH = nv["doc.DSN"].ToString() + subDocData["fDSN_SUB"].ToString() + ".pdf";
                //doc.PAGE_COUNT = model.PageCount;
                doc.DOCUMENT_TYPE = subDocData["fDOCUMENT_TYPE"].ToString();
                doc.SCAN_DATE = datetimeNow;
                doc.FILE_SIZE_CODE = "A4";
                doc.DOC_TITLE = subDocData["fDOC_DESC"].ToString();
                doc.FOLDER_TYPE = subDocData["fFOLDER_TYPE"].ToString();

                */
                for(int i= 0; i < P_MW_SCANNED_DOCUMENTs.Count; i++)
                {
                    byte[] bs;
                    if (File.Exists(ApplicationConstant.PEMFilePath + P_MW_SCANNED_DOCUMENTs[i].FILE_PATH))
                    bs =File.ReadAllBytes(ApplicationConstant.PEMFilePath + P_MW_SCANNED_DOCUMENTs[i].FILE_PATH);
                    else bs = new byte[0];
                    P_MW_SCANNED_DOCUMENTs[i].FILE_PATH =Convert.ToBase64String(bs);
                }
                //FORM_CODE = o.FORM_CODE, RECORD_ID = o.RECORD_ID,  }).FirstOrDefault();
                //if (dsn == null) return new { RESULT = false };
                return new { RESULT = RESULT.SUCCESS, data = P_MW_SCANNED_DOCUMENTs };
            }
        }
        public dynamic LoadDocInfo(DocModel model)
        {//sessionIDIn
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                if (model == null) return new { RESULT = false };
                if (model.Dsn == null) return new { RESULT = false };
                P_MW_DSN dsn = db.P_MW_DSN.Where(o => o.DSN == model.Dsn).ToList()
                    .Select(o => new P_MW_DSN() { DSN = o.DSN, FORM_CODE = o.FORM_CODE, RECORD_ID = o.RECORD_ID }).FirstOrDefault();
                if (dsn == null) return new { RESULT = false };
                bool existsDoc = false;
                using (EntitiesDRMS db2 = new EntitiesDRMS())
                {
                    existsDoc = db2.DRMS_DOCUMENT_META_DATA.Where(o => o.DSN == dsn.DSN).ToList().Count > 0;
                }
                return new { RESULT = RESULT.SUCCESS, data = dsn, existsDoc = existsDoc ? "Y" : "N" };
            }
        }
        public dynamic LoadDocRelated(DocModel model)
        {//sessionIDIn
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                if (model == null) return new { RESULT = false };
                if (model.Dsn == null) return new { RESULT = false };
                P_MW_ACK_LETTER letter = db.P_MW_ACK_LETTER.Where(o => o.DSN == model.Dsn).ToList()
                    .Select(o => new P_MW_ACK_LETTER() { DSN = o.DSN, AUDIT_RELATED = o.AUDIT_RELATED, ORDER_RELATED=o.ORDER_RELATED , SIGNBOARD_RELATED = o.SIGNBOARD_RELATED, SSP = o.SSP}).FirstOrDefault();
                if (letter == null) return new { RESULT = RESULT.SUCCESS };
                return new { RESULT = RESULT.SUCCESS, data = letter };
            }
        }
        


        public dynamic GenerateNewDsn()
        {
            ServiceResult serviceResult = new ServiceResult();
            JsonResult rs = ProcessingIncomingBL.GenerateNewDsn();
            return rs;
        }











        public dynamic SearchDoc(DocModel model)
        {//sessionIDIn
            using (EntitiesDRMS db = new EntitiesDRMS())
            {
                dynamic data2;
                if (model.Keyword == null) model.Keyword = "";
                DisplayGrid dg = new DisplayGrid();


                string q = ""
                + "\r\n\t" + " SELECT                        "
                + "\r\n\t" + "  DATA.* "
                + "\r\n\t" + " FROM DRMS_DOCUMENT_META_DATA DATA,                       "
                + "\r\n\t" + " (                                                                     "
                + "\r\n\t" + " SELECT DSN, MAX(LPAD(DSN_VERSION_NUMBER,8,'0')) AS VER FROM                       "
                + "\r\n\t" + " DRMS_DOCUMENT_META_DATA                                               "
                + "\r\n\t" + " GROUP BY DSN                                                          "
                + "\r\n\t" + " ) BASE                                                                ";
                //+ "\r\n\t" + " WHERE DATA.DSN = BASE.DSN AND DATA.DSN_VERSION_NUMBER = BASE.VER      ";
                dg.Query = q;
                string whereQ = " WHERE DATA.DSN = BASE.DSN AND LPAD(DATA.DSN_VERSION_NUMBER,8,'0') = BASE.VER ";
                int pCnt = 0;
                List<object> paras = new List<object>();
                if(model.Dsn != null)
                {
                    whereQ = whereQ + "\r\n\t" + " AND DATA.DSN = :dsn ";
                    dg.QueryParameters.Add("dsn", model.Dsn);
                }
                if (model.Stage != null)
                {
                    whereQ = whereQ + "\r\n\t" + " AND DATA.DOCUMENT_CONTROL_STAGE = :stage ";
                    dg.QueryParameters.Add("stage", model.Stage);
                }
                if (model.DateFrom != null)
                {
                    whereQ = whereQ + "\r\n\t" + " AND DATA.DOC_DATE >=:dateFrom ";
                    dg.QueryParameters.Add("dateFrom", model.DateFrom);
                }
                if (model.DateTo != null)
                {
                    whereQ = whereQ + "\r\n\t" + " AND DATA.DOC_DATE <=:dateTo ";
                    dg.QueryParameters.Add("dateTo", model.DateTo);
                }
                dg.Rpp = -1;
                dg.QueryWhere = whereQ;
                dg.Search();


                //if (model.Keyword != null)
                //{
                data2 = db.DRMS_DOCUMENT_META_DATA
                    //.Where(o => o.SUBMISSION_LETTER_DATE != null)
                    .Where(o =>
                        (model.Dsn == null && (model.Keyword == "" ||
                        o.DSN.StartsWith(model.Keyword) ||
                        o.EFSS_SUBMISSION_REF_NO.StartsWith(model.Keyword) ||
                        o.BD_FILE_REF.StartsWith(model.Keyword)))||
                        (model.Dsn != null  && o.DSN == model.Dsn)
                    )
                    .OrderBy(o => o.SUBMISSION_LETTER_DATE)
                    //.Select(o => new { o.DSN, YEAR = o.SUBMISSION_LETTER_DATE.Substring(0, 4), o.STATUS, DATE = o.SUBMISSION_LETTER_DATE })
                    .ToList();
               /* } else
                {
                    data = db.DRMS_DOCUMENT_META_DATA
                    .Where(o => o.SUBMISSION_LETTER_DATE != null)
                    .OrderBy(o => o.SUBMISSION_LETTER_DATE)
                    //.Select(o => new { o.DSN, YEAR = o.SUBMISSION_LETTER_DATE.Substring(0, 4), o.STATUS, DATE = o.SUBMISSION_LETTER_DATE })
                    .ToList();
                }*/
                return new { RESULT = RESULT.SUCCESS, data= dg.Data };
            }
        }

        

        public ActionResult DeleteDoc(DocModel model)
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
        public dynamic BatchUpdate(DocModel model, HttpFileCollectionBase files)
        {
            if (files == null || files.Count == 0) return null;
            if (model == null || string.IsNullOrWhiteSpace(model.sheet)) return null;


            List<DRMS_DOCUMENT_META_DATA> updateData = new List<DRMS_DOCUMENT_META_DATA>();

            int idx = 2;
            while (true)
            {
                DRMS_DOCUMENT_META_DATA DRMS_DOCUMENT_META_DATA = Excel2data(files[0], idx++);
                if (DRMS_DOCUMENT_META_DATA == null) break;
                updateData.Add(DRMS_DOCUMENT_META_DATA);
            }

            using (EntitiesDRMS db = new EntitiesDRMS())
            {
                for (int i = 0; i < updateData.Count; i++)
                {
                    DRMS_DOCUMENT_META_DATA data = updateData[i];
                    dynamic rs = SearchDoc(new DocModel() { Dsn = data.DSN });
                    List<Dictionary<string, object>> dbData = rs.data;
                    if (dbData == null || dbData.Count == 0) continue;
                    // DRMS_DOCUMENT_META_DATA dbData = db.DRMS_DOCUMENT_META_DATA.Where(o => o.DSN == data.DSN).FirstOrDefault();
                    string ver = dbData[0]["DSN_VERSION_NUMBER"].ToString();
                    /*   Type t = data.GetType();
                   for (int j = 0; j < t.GetProperties().Length; i++)
                   {
                       if (t.GetProperties()[j].CanWrite && t.GetProperties()[j].CanRead)
                       {
                           PropertyInfo pi = t.GetProperties()[j];
                           pi.SetValue(dbData, pi.GetValue(data));
                       }
                   }*/
                    data.MODIFIED_BY = null;
                    data.MODIFIED_DATE = null;
                    data.CREATED_BY = null;
                    data.CREATED_DATE = null;
                    data.UUID = null;
                    if (string.IsNullOrWhiteSpace(ver))
                    {
                        data.DSN_VERSION_NUMBER = "2";
                    } else
                    {
                        data.DSN_VERSION_NUMBER = int.Parse(ver) + 1 + "";
                    }
                    db.DRMS_DOCUMENT_META_DATA.Add(data);
                }
                db.SaveChanges();
            }

            return new { RESULT = RESULT.SUCCESS };
        }

        private DRMS_DOCUMENT_META_DATA Excel2data(HttpPostedFileBase postedFile, int? rowNo)
        {
            if (rowNo == null) rowNo = 2;
             DRMS_DOCUMENT_META_DATA data = new DRMS_DOCUMENT_META_DATA();
            using (ExcelPackage ep = new ExcelPackage(postedFile.InputStream))
            {
                if (ep.Workbook.Worksheets.Count == 0) return null;
                ExcelWorksheet sheet = ep.Workbook.Worksheets[1];
                if (sheet == null) return null;

                int colIdx = 1;
                while (true)
                {
                    string header = sheet.Cells[1, colIdx].Text;
                    string value = sheet.Cells[rowNo.Value, colIdx].Text;
                    if (string.IsNullOrWhiteSpace(header)) break;
                    colIdx++;
                    PropertyInfo prop = data.GetType().GetProperty(header, BindingFlags.Public | BindingFlags.Instance);
                    if (null != prop && prop.CanWrite)
                    {
                        try
                        {//SUBMISSION_LETTER_DATE
                            if (header == "DSN")
                            {
                                if (string.IsNullOrWhiteSpace(value)) { return null; }
                                prop.SetValue(data, value, null);
                            }
                            if (header == "UPLOAD_DATE")
                            {
                                prop.SetValue(data, DateTime.Now, null);
                            }
                            else if (header == "DOC_DATE")
                            {
                                prop.SetValue(data, DateTime.Now, null);
                            } else if (header == "SUBMISSION_LETTER_DATE")
                            {
                                if (!string.IsNullOrWhiteSpace(value))
                                {
                                    prop.SetValue(data, (new DateTime(long.Parse(value))).ToString("yyyy-MM-dd"), null);
                                }
                            }
                            //else if (header == "MW_ITEM_NUMBER")
                            //{
                            //    //prop.SetValue(data, DateTime.Now, null);
                            //}
                            else
                            {
                                prop.SetValue(data, value, null);
                            }
                        }catch(Exception ex)
                        {

                        }
                    }
                }

            }
            return data;
        }
        public dynamic Upload(DocModel modelX, HttpFileCollectionBase files)
        {
            
            if(files.Count < 2)
            {
                return new { RESULT = RESULT.FAILURE };
            }
            if (SessionUtil.LoginPost == null)
            {
                //portal invalid
                return new { RESULT = RESULT.REQ_LOGIN };
            }
            string PEMFilePath = ApplicationConstant.PEMFilePath;
            if (!System.IO.Directory.Exists(PEMFilePath)) Directory.CreateDirectory(PEMFilePath);



            using (var scope = new TransactionScope())
            {
                //List< P_MW_SCANNED_DOCUMENT> docs;
                P_MW_DSN dsn;
                string fileName = CommonUtil.NewUuid();
                using (EntitiesMWProcessing db = new EntitiesMWProcessing())
                {
                    //using (DbContextTransaction transaction = db.Database.BeginTransaction())
                    //{

                    DateTime datetimeNow = DateTime.Now;


                    DRMS_DOCUMENT_META_DATA DRMS_DOCUMENT_META_DATA = Excel2data(files[files.Count - 1], 2);



                    using (EntitiesDRMS docDb = new EntitiesDRMS())
                    {
                        string dsnno = DRMS_DOCUMENT_META_DATA.DSN;

                        List<DRMS_DOCUMENT_META_DATA> oldVersionDatas = docDb.DRMS_DOCUMENT_META_DATA.Where(o => o.DSN == dsnno).ToList();
                        if (oldVersionDatas.Count == 0)
                        {
                            DRMS_DOCUMENT_META_DATA.DSN_VERSION_NUMBER = "1";
                            
                        } else
                        {
                            int maxVersion = 0;
                            for (int i = 0; i < oldVersionDatas.Count; i++)
                            {
                                try
                                {
                                    int currentVersion = int.Parse(oldVersionDatas[i].DSN_VERSION_NUMBER);
                                    maxVersion = Math.Max(maxVersion, currentVersion);
                                }
                                catch
                                {
                                }
                            }
                            DRMS_DOCUMENT_META_DATA.DSN_VERSION_NUMBER = (maxVersion + 1) + "";
                        }
                        DRMS_DOCUMENT_META_DATA.REC_TYPE = "MWU";
                        if (string.IsNullOrWhiteSpace(DRMS_DOCUMENT_META_DATA.DOCUMENT_CONTROL_STAGE)) DRMS_DOCUMENT_META_DATA.DOCUMENT_CONTROL_STAGE = "1";
                        docDb.DRMS_DOCUMENT_META_DATA.Add(DRMS_DOCUMENT_META_DATA); 
                        docDb.SaveChanges();
                    }
                    dsn = db.P_MW_DSN.Where(o => o.DSN == DRMS_DOCUMENT_META_DATA.DSN).FirstOrDefault();
                    if (dsn == null)
                    {
                        dsn = new P_MW_DSN();
                        dsn.DSN = DRMS_DOCUMENT_META_DATA.DSN;
                        db.P_MW_DSN.Add(dsn);
                        db.SaveChanges();
                    }





                    List< P_MW_SCANNED_DOCUMENT> oldSubDoc =  db.P_MW_SCANNED_DOCUMENT.Where(o => o.P_MW_DSN.DSN == DRMS_DOCUMENT_META_DATA.DSN).ToList();

                    for(int i = 0; i < oldSubDoc.Count; i++)
                    {
                        string filePath = PEMFilePath + oldSubDoc[i].FILE_PATH;
                        File.Move(filePath, filePath + "." + oldSubDoc[i].UUID + ".pdf");
                    }
                    db.P_MW_SCANNED_DOCUMENT.RemoveRange(oldSubDoc);

                    string sub = modelX.SUB;
                    JArray subObj = JArray.Parse(sub);
                    List<P_MW_SCANNED_DOCUMENT> subDocs =subObj.ToObject<List<P_MW_SCANNED_DOCUMENT>>();
                   // List <P_MW_SCANNED_DOCUMENT> subDocs = modelX.SUB;
                    //string[] SubDocList = DRMS_DOCUMENT_META_DATA.SubDocList.Split(';');

                    for (int i= 0;i < files.Count -1; i++)
                    {
                        if(files[i].ContentLength !=0 )
                        {
                            //JObject subDocData = JObject.Parse(SubDocList[i]);
                            P_MW_SCANNED_DOCUMENT passDoc = subDocs[i];
                            P_MW_SCANNED_DOCUMENT doc = new P_MW_SCANNED_DOCUMENT();
                            string filePath = PEMFilePath + DRMS_DOCUMENT_META_DATA.DSN + passDoc.DSN_SUB + ".pdf";
                            if (File.Exists(filePath))
                            {
                                File.Move(filePath, filePath + CommonUtil.NewUuid() + ".pdf");
                            }
                            files[i].SaveAs(filePath);
                            if (passDoc.PAGE_COUNT != null) doc.PAGE_COUNT = passDoc.PAGE_COUNT;
                            doc.DSN_SUB = DRMS_DOCUMENT_META_DATA.DSN + passDoc.DSN_SUB;
                            doc.P_MW_DSN = dsn;
                            doc.FILE_PATH = DRMS_DOCUMENT_META_DATA.DSN + passDoc.DSN_SUB + ".pdf";
                            //doc.PAGE_COUNT = model.PageCount;
                            doc.DOCUMENT_TYPE = passDoc.DOCUMENT_TYPE;
                            doc.SCAN_DATE = datetimeNow;
                            doc.FILE_SIZE_CODE = "A4";
                            doc.DOC_TITLE = passDoc.DOC_TITLE;
                            doc.FOLDER_TYPE = passDoc.FOLDER_TYPE;
                            db.P_MW_SCANNED_DOCUMENT.Add(doc);
                            db.SaveChanges();
                        }
                    }
                }

                scope.Complete();
            }
            
            
            /*

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                //using (DbContextTransaction transaction = db.Database.BeginTransaction())
                //{
                    DateTime datetimeNow =DateTime.Now;
                    //UUID, DSN_ID, DSN_SUB, FILE_PATH, PAGE_COUNT, DOCUMENT_TYPE, SCAN_DATE, FILE_SIZE_CODE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, SUBMIT_TYPE, FORM_TYPE, DOC_TITLE, RELATIVE_FILE_PATH, STATUS_CODE, FOLDER_TYPE, RRM_SYN_STATUS
                    P_MW_DSN dsn = db.P_MW_DSN.Where(o => o.DSN == model.Doc.DSN).FirstOrDefault();
                    P_MW_SCANNED_DOCUMENT doc = new P_MW_SCANNED_DOCUMENT();

                    doc.DSN_SUB = model.Doc.DSN;
                    doc.P_MW_DSN = dsn;
                    doc.FILE_PATH = "";
                    //doc.PAGE_COUNT = model.PageCount;
                    doc.DOCUMENT_TYPE = model.Doc.DOCUMENT_TYPE;
                    doc.SCAN_DATE = datetimeNow;
                    doc.FILE_SIZE_CODE = "A4";
                    doc.DOC_TITLE = model.Doc.MW_ITEM_NUMBER;
                    doc.FOLDER_TYPE = "Private";
                    db.P_MW_SCANNED_DOCUMENT.Add(doc);
                    db.SaveChanges();
                    EntitiesDRMS docDb = new EntitiesDRMS(db.Database.Connection, false);
                    //docDb.Database.UseTransaction(transaction.UnderlyingTransaction);
                    DRMS_DOCUMENT_META_DATA DRMS_DOCUMENT_META_DATA = new DRMS_DOCUMENT_META_DATA();
                    DRMS_DOCUMENT_META_DATA.REC_TYPE = "PEM";
                    docDb.DRMS_DOCUMENT_META_DATA.Add(DRMS_DOCUMENT_META_DATA);
                    docDb.SaveChanges();
                    file.SaveAs(ImagePath + "/" + DRMS_DOCUMENT_META_DATA.UUID);
                }
            }*/
            return new { RESULT = RESULT.SUCCESS };
        }



















        /*


                POST /vdws/VD_WS_Server.asmx HTTP/1.1
        Host: 202.83.247.87
        Content-Type: application/soap+xml; charset=utf-8
        Content-Length: length

            */


        //webservice
        private static readonly string vitalDocURL = "http://10.5.182.204/VDWS/VD_WS_Server.asmx";
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