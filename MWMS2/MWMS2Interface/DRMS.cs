
using MWMS2Interface.Constant;
//using MWMS2Interface.EntityDRMS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Xml;

namespace MWMS2Interface
{
    class DRMS
    {

        string adminUsername = ApplicationConstant.DRMS_ACCOUNT;
        string adminPassword = ApplicationConstant.DRMS_PW;
       //string adminUsername = "administrator";
       //string adminPassword = "P@ssw0rd01";
        static void Main(string[] args)
        {
            string PEMFilePath = @"E:\PEM\";
            string workingKey = CommonUtil.NewUuid();


            //using (EntitiesDRMS db = new EntitiesDRMS())
            //{
            //    db.Database.ExecuteSqlCommand("UPDATE DRMS_DOCUMENT_META_DATA SET STATUS = '" + workingKey + "' WHERE STATUS = 'local'");
            //}

            //using (TransactionScope scope = new TransactionScope())
            //{
            //    using (EntitiesDRMS db = new EntitiesDRMS())
            //    {
            //        List<DRMS_DOCUMENT_META_DATA> localRecords = db.DRMS_DOCUMENT_META_DATA.Where(o => o.STATUS == workingKey).ToList();


            //        for (int i = 0; i < localRecords.Count; i++)
            //        {
            //            DRMS_DOCUMENT_META_DATA localRecord = localRecords[i];//D0001121154
            //            P_MW_SCANNED_DOCUMENT mwScannedDoc = db.P_MW_SCANNED_DOCUMENT.Where(o => o.DSN_SUB.Contains(localRecord.DSN)).FirstOrDefault();
            //            if (mwScannedDoc == null)
            //            {
            //                localRecord.STATUS = "P_MW_SCANNED_DOCUMENT not exist.";
            //                continue;
            //            }
            //            if(!File.Exists(PEMFilePath + mwScannedDoc.FILE_PATH))
            //            {
            //                localRecord.STATUS = "File not exist.";
            //                continue;
            //            }
            //            string postCode = localRecord.CREATED_BY;
            //            if (string.IsNullOrWhiteSpace(postCode))
            //            {
            //                localRecord.STATUS = "PostCode is blank.";
            //                continue;
            //            }
            //            SYS_POST sysPost = db.SYS_POST.Where(o => o.CODE == postCode).FirstOrDefault();
            //            if (sysPost == null)
            //            {
            //                localRecord.STATUS = "PostCode not exists.";
            //                continue;
            //            }

            //            string DSMS_USERNAME = sysPost.DSMS_USERNAME;
            //            string DSMS_PW = sysPost.DSMS_PW;
            //            if(string.IsNullOrWhiteSpace(DSMS_USERNAME) || string.IsNullOrWhiteSpace(DSMS_PW))
            //            {
            //                localRecord.STATUS = "DSMS_ACCOUNT_INVALID";
            //            }

            //            XmlDocument result = CallWebService("SystemLogin", new Dictionary<string, string>() { ["userName"] = DSMS_USERNAME, ["password"] = DSMS_PW, ["language"] = "0" });
            //            if (result.GetElementsByTagName("SessionID").Count == 0)
            //            {
            //                localRecord.STATUS = "Invalid document system user/password.";
            //                continue;
            //            }
            //            string sessionID = result.GetElementsByTagName("SessionID")[0].InnerText;

                        

            //            byte[] bytes = File.ReadAllBytes(PEMFilePath + mwScannedDoc.FILE_PATH);
            //            string file = Convert.ToBase64String(bytes);


            //            result = CallWebService("AttachmentUploadFile", new Dictionary<string, string>() {
            //                ["sessionIDIn"] = sessionID
            //                , ["fileName"] = mwScannedDoc.DSN_SUB + ".PDF"
            //                , ["data"] = file
            //            });
            //            if (result.GetElementsByTagName("Success").Count != 0)
            //            {
            //                if (result.GetElementsByTagName("Success")[0].InnerText.ToLower() == "false")
            //                {
            //                    localRecord.STATUS = "upload error.";
            //                    continue;
            //                }
            //            }
            //                Console.WriteLine(result);
            //        }
            //        db.SaveChanges();
            //    }

            //}
        }



        /*
        POST /vdws/VD_WS_Server.asmx HTTP/1.1
        Host: 202.83.247.87
        Content-Type: application/soap+xml; charset=utf-8
        Content-Length: length
        */

        /*
        <AttachmentUploadFile xmlns="http://tempuri.org/">
      <sessionIDIn>string</sessionIDIn>
      <fileName>string</fileName>
      <data>base64Binary</data>
    </AttachmentUploadFile>

            */


        //DR http://10.5.182.204/WebVD/
        //webservice
        //private static readonly string vitalDocURL = "http://202.83.247.87:8082/vdws/VD_WS_Server.asmx";
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