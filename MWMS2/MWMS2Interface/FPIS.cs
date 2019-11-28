
using MWMS2Interface.Constant;
using MWMS2Interface.Entity;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWMS2Interface
{
    class FPIS
    {
        static private List<List<object>> getData()
        {
            List<List<object>> data = new List<List<object>>();
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();

            string queryStr = "" +
                         "\r\n" + "\t" + " SELECT DISTINCT ITEM.RELEVANT_REFERENCE AS \"Order No.\", REF_NO.REFERENCE_NO AS \"MW Submission No.\",  " +
                         "\r\n" + "\t" + " R.COMMENCEMENT_DATE AS \"MW Submission Date\", " +
                         "\r\n" + "\t" + " CASE WHEN  R.LANGUAGE_CODE = 'EN' THEN  CAST(A.ENGLISH_DISPLAY AS VARCHAR2(1000))" +
                         "\r\n" + "\t" + " ELSE CAST(A.CHINESE_DISPLAY AS VARCHAR2(1000)) END AS \"RVD Address\", " +
                         "\r\n" + "\t" + " R.LOCATION_OF_MINOR_WORK AS \"Work Location\" " +
                         "\r\n" + "\t" + " FROM P_MW_RECORD R," +
                         "\r\n" + "\t" + "P_MW_REFERENCE_NO REF_NO, P_MW_ADDRESS A, P_MW_RECORD_ITEM ITEM " +
                         "\r\n" + "\t" + " WHERE R.REFERENCE_NUMBER =REF_NO.UUID AND R.LOCATION_ADDRESS_ID=A.UUID" +
                         "\r\n" + "\t" + " AND R.UUID =ITEM.MW_RECORD_ID AND ITEM.RELEVANT_REFERENCE IS NOT NULL" +
                         "\r\n" + "\t" + " AND R.IS_DATA_ENTRY  = 'N' AND ITEM.STATUS_CODE = 'FINAL' AND ITEM.RELEVANT_REFERENCE  IS NOT NULL " +
                         "\r\n" + "\t" + " AND REF_NO.REFERENCE_NO  IS NOT NULL " +
                         "\r\n" + "\t" + " AND  R.COMMENCEMENT_DATE  IS NOT NULL" +
                         "\r\n" + "\t" + " AND CASE WHEN  R.LANGUAGE_CODE = 'EN' THEN  CAST(A.ENGLISH_DISPLAY AS VARCHAR2(1000)) " +
                         "\r\n" + "\t" + " ELSE CAST(A.CHINESE_DISPLAY AS VARCHAR2(1000)) END IS NOT NULL ORDER BY REF_NO.REFERENCE_NO";

            using (EntitiesProcessing db = new EntitiesProcessing())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryStr, queryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }

            return data;
        }
     
        static void Main(string[] args)
        {
            List<List<object>> data = new List<List<object>>();

            data = getData();
            List<string> headerList = new List<string>() {
                 "Order No.", "MW Submission No.", "MW Submission Date", "RVD Address", "Work Location"
                    };
            Services.Common c = new Services.Common();
            c.exportCSVFile("MWMS_FPIS" + DateTime.Now.ToString("yyyyMMdd"), "txt",  headerList, data);
            SendFileToServer.Send("MWMS_FPIS" + DateTime.Now.ToString("yyyyMMdd") + ".zip", "Bravo");

        }

        public static class SendFileToServer
        {
            private static string host = ApplicationConstant.FPIS_IP;

            // Enter your sftp username here
            private static string username = ApplicationConstant.FPIS_ACCOUNT;

            // Enter your sftp password here
            private static string password = ApplicationConstant.FPIS_PW;
            public static int Send(string fileName, string path)
            {
                // var connectionInfo = new ConnectionInfo(host, "sftp", new PasswordAuthenticationMethod(username, password));

                using (SftpClient sftpClient = new SftpClient(host, 22, username, password))
                {
                    Console.WriteLine("Connect to server");
                    sftpClient.Connect();

                    sftpClient.ChangeDirectory(path);
                    Console.WriteLine("Creating FileStream object to stream a file");
                    using (FileStream fs = new FileStream(fileName, FileMode.Open))
                    {
                        sftpClient.BufferSize = 1024;
                        sftpClient.UploadFile(fs, Path.GetFileName(fileName));
                    }
                    sftpClient.Dispose();
                }

                
                return 0;
            }
        }

    }
}
