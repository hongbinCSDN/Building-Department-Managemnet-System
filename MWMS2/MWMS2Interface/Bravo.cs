using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MWMS2Interface.Entity;
using System.IO;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Advanced;
using System.Data.Entity;
using System.Drawing;
using Ghostscript.NET.Rasterizer;
using Ghostscript.NET;
using System.Reflection;
using System.Drawing.Imaging;
using Renci.SshNet;
using MWMS2Interface.Constant;

namespace MWMS2Interface
{
    class Bravo
    {
        static void Main(string[] args)
        {
            //   PdfDocument doc222 = PdfReader.Open(@"C:\Users\user\Desktop\Document\MWMS2\111.pdf");

       ///     SendFileToServer.Send(@"C:\Users\user\Desktop\Document\MWMS2\111.jpg", "BRAVO");
         ///   SendFileToServer.Send(@"C:\Users\user\Desktop\Document\MWMS2\222.jpg", "BRAVO");
            //    PdfToImage(@"C:\Users\user\Desktop\Document\MWMS2\", "", @"C:\Users\user\Desktop\Document\MWMS2\111.pdf", (int)doc222.Pages[0].Height.Value, (int)doc222.Pages[0].Width.Value);
            using (EntitiesProcessing db = new EntitiesProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    var queryMWScannedDocumentPlanning = db.P_MW_SCANNED_DOCUMENT
                                                            .Where(x => x.RRM_SYN_STATUS == "PENDING_LOAD_DETAILS" 
                                                                         && x.FOLDER_TYPE == "Public");
                            foreach (var item in queryMWScannedDocumentPlanning)
                        {
                            FileInfo file = new FileInfo(item.FILE_PATH);
                            if (string.IsNullOrWhiteSpace(item.FILE_PATH) || !file.Exists)
                            {
                                item.RRM_SYN_STATUS = "READY";
                            }
                            else
                            {
                                PdfDocument doc = PdfReader.Open(item.FILE_PATH);
                                item.PAGE_COUNT = doc.PageCount;
                                item.FILE_SIZE_CODE = getPaperSize((float)doc.Pages[0].Height.Value, (float)doc.Pages[0].Width.Value);
                                string JpegFilePath = item.FILE_PATH.Replace(".pdf", ".jpg");
                                PdfToImage(JpegFilePath, "", item.FILE_PATH, 1200, 1600);
                            }



                            item.RRM_SYN_STATUS = "READY";
                     
                    }

                    db.SaveChanges();



                    var queryMWAddress = db.P_MW_ADDRESS.Where(x => x.RRM_SYN_STATUS == "READY");
                    var queryMWRecord = db.P_MW_RECORD.Where(x => x.RRM_SYN_STATUS == "READY");
                    var queryMWRecordAddressInfo = db.P_MW_RECORD_ADDRESS_INFO.Where(x => x.RRM_SYN_STATUS == "READY");
                    var queryMWRecordItem = db.P_MW_RECORD_ITEM.Where(x => x.RRM_SYN_STATUS == "READY");
                    var queryMWScannedDocument = db.P_MW_SCANNED_DOCUMENT.Where(x => x.RRM_SYN_STATUS == "READY");

                    //NEW
                    var queryMWFile = db.P_MW_FILEREF.Where(x => x.RRM_SYN_STATUS == "READY");

                    String sql =
                         "	UUID, FLAT, FLOOR, BLOCK, BUILDING_NAME, " +
                         "	STREET_NO, STREE_NAME, DISTRICT, REGION, LOCATION, " +
                         "	ADDRESS_TYPE, PERSON_CONTACT_ID, " +
                         "	BLOCK_ID, UNIT_ID, STREET_LOCATION_TABLE_ID, " +
                         "	ENGLISH_STREET_NAME, ENGLISH_STREET_TYPE, ENGLISH_STREET_DIRECTION, " +
                         "	ENGLISH_ST_TYPE_PRE_INDICATOR, ENGLISH_ST_LOCATION_NAME_1, ENGLISH_ST_LOCATION_NAME_2, " +
                         "	ENGLISH_ST_LOCATION_NAME_3, CHINESE_STREET_NAME, CHINESE_STREET_TYPE, " +
                         "	CHINESE_STREET_DIRECTION, CHINESE_ST_TYPE_PRE_INDICATOR, CHINESE_ST_LOCATION_NAME_1, " +
                         "	CHINESE_ST_LOCATION_NAME_2, CHINESE_ST_LOCATION_NAME_3, BUILDING_NO_NUMERIC, " +
                         "	BUILDING_NO_ALPHA, BUILDING_NO_EXTENSION, BLOCK_ID_NUMERIC, BLOCK_ID_ALPHA, " +
                         "	BLOCK_ID_ALPHA_PRE_INDICATOR, ENGLISH_BLOCK_DESCRIPTION, CHINESE_BLOCK_DESCRIPTION, " +
                         "	BLOCK_DESC_PRECEDE_INDICATOR, ENGLISH_BUILDING_NAME_LINE_1, ENGLISH_BUILDING_NAME_LINE_2, " +
                         "	ENGLISH_BUILDING_NAME_LINE_3, CHINESE_BUILDING_NAME_LINE_1, CHINESE_BUILDING_NAME_LINE_2, " +
                         "	CHINESE_BUILDING_NAME_LINE_3, ENGLISH_BLOCK_ADDRESS, ENGLISH_BLOCK_ADDRESS_2, " +
                         "	CHINESE_BLOCK_ADDRESS, ENGLISH_DEVELOPMENT_NAME, CHINESE_DEVELOPMENT_NAME, " +
                         "	FLOOR_CODE, ENGLISH_FLOOR_DESCRIPTION, CHINESE_FLOOR_DESCRIPTION, " +
                         "	ENGLISH_UNIT_NO, CHINESE_UNIT_NO, ASSESSMENT_NO, ENGLISH_TENEMENT_DESCRIPTION, " +
                         "	CHINESE_TENEMENT_DESCRIPTION, ENGLISH_RRM_ADDRESS, ENGLISH_RRM_BUILDING, " +
                         "	CHINESE_RRM_ADDRESS, CHINESE_RRM_BUILDING, DISPLAY_STREET, DISPLAY_STREET_NO, " +
                         "	DISPLAY_BUILDINGNAME, DISPLAY_FLOOR, DISPLAY_FLAT, DISPLAY_DISTRICT, " +
                         "	DISPLAY_REGION, ENGLISH_DISPLAY, CHINESE_DISPLAY, DISPLAY_STREET_CODE, " +
                         "	DISPLAY_BLOCK_ID, CREATED_BY, CREATED_DATE ";

                    String mwRecordsql =
                        "	UUID, REFERENCE_NUMBER,	LOCATION_ADDRESS_ID,	COMMECEMENT_DATE, " +
                        "	COMPLETION_DATE,	COMMECEMENT_SUBMISSION_DATE,	STATUS_CODE, " +
                        "	PLAN_NO, CLASS_CODE,	MW_PROGRESS_STATUS_CODE,	IS_DATA_ENTRY,	 " +
                        "	COMPLETION_SUBMISSION_DATE,	LOCATION_OF_MINOR_WORK,	FILE_REFERENCE,	 " +
                        "	LANGUAGE_CODE,	SIGNBOARD_PERFROMER_ID,	S_FORM_TYPE_CODE,	OWNER_ID,	 " +
                        "	COMMENCEMENT_ACKNOWLEDGE_DATE,	COMMENCEMENT_CONDINTIONAL_DATE,	 " +
                        "	COMMENCEMENT_REFUSE_DATE,	COMPLETION_ACKNOWLEDGE_DATE,	 " +
                        "	COMPLETION_CONDINTIONAL_DATE,	COMPLETION_REFUSE_DATE,	SUBMIT_TYPE,	 " +
                        "	FIRST_RECEIVED_DATE,	LAST_RECEIVED_DATE,	VERIFICATION_SPO, " +
                        "	CREATED_BY,	CREATED_DATE, FILEREF_FOUR , FILEREF_TWO ";

                    String mwRecordsql2 =
                       "	UUID, (select REFERENCE_NO from  P_MW_REFERENCE_NO where UUID=REFERENCE_NUMBER) AS REFERENCE_NUMBER,	LOCATION_ADDRESS_ID,	COMMENCEMENT_DATE, " +
                       "	COMPLETION_DATE,	COMMENCEMENT_SUBMISSION_DATE,	STATUS_CODE, " +
                       "	PLAN_NO, CLASS_CODE,	MW_PROGRESS_STATUS_CODE,	IS_DATA_ENTRY,	 " +
                       "	COMPLETION_SUBMISSION_DATE,	LOCATION_OF_MINOR_WORK,	FILE_REFERENCE,	 " +
                       "	LANGUAGE_CODE,	SIGNBOARD_PERFROMER_ID,	S_FORM_TYPE_CODE,	OWNER_ID,	 " +
                       "	COMMENCEMENT_ACKNOWLEDGE_DATE,	COMMENCEMENT_CONDINTIONAL_DATE,	 " +
                       "	COMMENCEMENT_REFUSE_DATE,	COMPLETION_ACKNOWLEDGE_DATE,	 " +
                       "	COMPLETION_CONDINTIONAL_DATE,	COMPLETION_REFUSE_DATE,	SUBMIT_TYPE,	 " +
                       "	FIRST_RECEIVED_DATE,	LAST_RECEIVED_DATE,	VERIFICATION_SPO, " +
                       "	CREATED_BY,	CREATED_DATE, FILEREF_FOUR , FILEREF_TWO ";

                    String RecordAddresssql = "UUID,	MW_RECORD_ID,	MW_ADDRESS_ID,	CREATED_BY,	CREATED_DATE ";


                    String mwRecordItemsql =
                    "	UUID,	MW_RECORD_ID,	MW_ITEM_CODE,	LOCATION_DESCRIPTION,	COMMENCEMENT_DATE, " +
                    "	COMPLETION_DATE,	RELEVANT_REFERENCE,	REVISION,	CLASS_CODE,	ORDERING, " +
                    "	STATUS_CODE,	CREATED_BY,	CREATED_DATE ";

                    String sdsql =
                        "	UUID,	MW_RECORD_ID,	DSN_SUB,	FILE_PATH,	PAGE_COUNT, " +
                        "	DOCUMENT_TYPE,	SCAN_DATE,	FILE_SIZE_CODE,	DOC_TITLE,	RELATIVE_FILE_PATH, " +
                        "	SUBMIT_TYPE,	FORM_TYPE,	STATUS_CODE,	PREVIEW_FILE_PATH,	CREATED_BY,	CREATED_DATE	 ";
                    string PEM_FILE_PATH = "E:\\pem_scan";
                    string RRM_FILE_PATH = "D:\\RRM\\pem_scan";
                    String sdsql2 =
                   "	UUID,	DSN_ID,	DSN_SUB,	REPLACE(FILE_PATH,'"+ PEM_FILE_PATH+"','" + RRM_FILE_PATH+"')" +" AS FILE_PATH ,	PAGE_COUNT, " +
                   "	DOCUMENT_TYPE,	SCAN_DATE,	FILE_SIZE_CODE,	DOC_TITLE,	RELATIVE_FILE_PATH, " +
                   "	SUBMIT_TYPE,	FORM_TYPE,	STATUS_CODE,	REPLACE(FILE_PATH,'" + PEM_FILE_PATH + "','" + RRM_FILE_PATH + "')" +" AS REALATIVE_FILE_PATH,	CREATED_BY,	CREATED_DATE	 ";

                    string mwRefSql = "UUID, MW_RECORD_ID, FILEREF_FOUR, FILEREF_TWO, BLK_ID, UNIT_ID, CREATED_BY, CREATED_DATE, MODIFIED_BY," +
                        " MODIFIED_DATE";




                    db.Database.ExecuteSqlCommand("Insert into MW_ADDRESS@Bravodblink (" + sql + " )  (select "+sql +"  from P_MW_ADDRESS where RRM_SYN_STATUS ='READY') ");
                    db.Database.ExecuteSqlCommand("Insert into MW_RECORD@Bravodblink ("+ mwRecordsql + ") select "+ mwRecordsql2 + " from P_MW_RECORD where RRM_SYN_STATUS ='READY' ");
                    db.Database.ExecuteSqlCommand("Insert into MW_RECORD_ADDRESS_INFO@Bravodblink (" + RecordAddresssql + ") select " + RecordAddresssql + "from P_MW_RECORD_ADDRESS_INFO where RRM_SYN_STATUS ='READY' ");
                    db.Database.ExecuteSqlCommand("Insert into MW_RECORD_ITEM@Bravodblink (" + mwRecordItemsql + " )  select" + mwRecordItemsql + "  from P_MW_RECORD_ITEM where RRM_SYN_STATUS ='READY' ");
                    db.Database.ExecuteSqlCommand("Insert into MW_SCANNED_DOCUMENT@Bravodblink (" + sdsql + " )  select" + sdsql2 + "from  P_MW_SCANNED_DOCUMENT where RRM_SYN_STATUS ='READY' ");


                    //to be add new table !!!
                    db.Database.ExecuteSqlCommand("Insert into MW_FILEREF@Bravodblink (" + mwRefSql + " )  select " + mwRefSql + " from  P_MW_FILEREF where RRM_SYN_STATUS ='READY' ");




                    queryMWScannedDocument.ToList().ForEach(x => x.RRM_SYN_STATUS = "PENDING_FILE_SYN");
                    queryMWAddress.ToList().ForEach(x => x.RRM_SYN_STATUS = "Complete");
                    queryMWRecord.ToList().ForEach(x => x.RRM_SYN_STATUS = "Complete");
                    queryMWRecordAddressInfo.ToList().ForEach(x => x.RRM_SYN_STATUS = "Complete");
                    queryMWRecordItem.ToList().ForEach(x => x.RRM_SYN_STATUS = "Complete");
                    queryMWFile.ToList().ForEach(x => x.RRM_SYN_STATUS = "Complete");
                    db.SaveChanges();

           
                 
                        var UploadToBravoQuery = db.P_MW_SCANNED_DOCUMENT.Where(x => x.RRM_SYN_STATUS == "PENDING_FILE_SYN");
                    bool dirChanged = false;
                    #region Copy photo to bravo
                    using (SftpClient sftpClient = new SftpClient("192.168.88.200", 22, "admin", "Asladm01"))
                    {
                        Console.WriteLine("Connect to server");
                        sftpClient.Connect();
                        foreach (var item in UploadToBravoQuery)
                        {
                        
                            string JpegFilePath = item.FILE_PATH.Replace(".pdf", ".jpg");

              
                            FileInfo file = new FileInfo(JpegFilePath);
                            Console.WriteLine(JpegFilePath);
                            if (string.IsNullOrWhiteSpace(JpegFilePath) || !file.Exists || JpegFilePath.Contains("61"))
                            {
                                Console.WriteLine("No Need");
                            }
                            else
                            {


                                Console.WriteLine("Send File");

                        
                                if (!dirChanged)
                                {
                                    sftpClient.ChangeDirectory("BRAVO");
                                    dirChanged = true;
                                }
                           
                                Console.WriteLine("Creating FileStream object to stream a file");
                                using (FileStream fs = new FileStream(JpegFilePath, FileMode.Open))
                                {
                                    sftpClient.BufferSize = 1024;
                                    sftpClient.UploadFile(fs, Path.GetFileName(JpegFilePath));
                                }
                             
                            }


                            //using (FileStream fs = new FileStream(JpegFilePath, FileMode.Open))
                            //    {
                            //    using (SftpClient sftpClient = new SftpClient("192.168.88.200", 22, "admin", "Asladm01"))
                            //    {
                            //        sftpClient.Connect();
                            //        sftpClient.ChangeDirectory("BRAVO");
                            //        sftpClient.BufferSize = 1024;
                            //        sftpClient.UploadFile(fs, Path.GetFileName(JpegFilePath));
                            //        sftpClient.Dispose();
                            //        Console.WriteLine("Send File Finished");
                            //    }
                            //}

                        }
                        sftpClient.Dispose();



                    }
                    #endregion
                    UploadToBravoQuery.ToList().ForEach(x => x.RRM_SYN_STATUS = "Complete");
                    db.SaveChanges();
                    tran.Commit();
                   }
            }

        }




        private static GhostscriptVersionInfo GhostScriptVersion => GetGhostscriptVersionInfo();

        private const int DesiredXDpi = 1;
        private const int DesiredYDpi = 1;

        private static string GetUniqueFileName(string extenType, string filePath)
        {
            var fileName = Guid.NewGuid().ToString("N") + extenType;

            while (File.Exists(Path.Combine(filePath, fileName)))
            {
                fileName = Guid.NewGuid().ToString("N") + extenType;
            }

            return fileName;
        }
        private static GhostscriptVersionInfo GetGhostscriptVersionInfo()
        {
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var vesion = new GhostscriptVersionInfo(new Version(0, 0, 0), path + @"\gsdll32.dll", string.Empty,
                GhostscriptLicense.GPL);

            return vesion;
        }
       
        private static void PdfToImage(string fileName, string outPdfImagePath, string file,int w, int h )
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
                        var test = img.GetThumbnailImage(1200, 1600,null,IntPtr.Zero);
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
        static public string getPaperSize(float paperHeight, float paperWidth)
        {
            float checkHeight = paperHeight - 10;
            float checkWidth = paperWidth - 10;
            if (checkHeight < checkWidth)
            {
                float orgCheckWidth = checkWidth;
                checkWidth = checkHeight;
                checkHeight = orgCheckWidth;
            }
            String paperSize = "A4";
            XSize A5 = PageSizeConverter.ToSize(PdfSharp.PageSize.A5);
            XSize A4 = PageSizeConverter.ToSize(PdfSharp.PageSize.A4);
            XSize A3 = PageSizeConverter.ToSize(PdfSharp.PageSize.A3);
            XSize A2 = PageSizeConverter.ToSize(PdfSharp.PageSize.A2);
            XSize A1 = PageSizeConverter.ToSize(PdfSharp.PageSize.A1);
            XSize A0 = PageSizeConverter.ToSize(PdfSharp.PageSize.A0);
            if ((checkHeight < A5.Height) &&
                (checkWidth < A5.Width))
            {
                paperSize = "A5";
            }
            else if ((checkHeight < A4.Height) &&
                      (checkWidth < A4.Width))
            {
                paperSize = "A4";
            }
            else if ((checkHeight < A3.Height) &&
                     (checkWidth < A3.Width))
            {
                paperSize = "A3";
            }
            else if ((checkHeight < A2.Height) &&
                     (checkWidth < A2.Width))
            {
                paperSize = "A2";
            }
            else if ((checkHeight < A1.Height) &&
                     (checkWidth < A1.Width))
            {
                paperSize = "A1";
            }
            else if ((checkHeight < A0.Height) &&
                     (checkWidth < A0.Width))
            {
                paperSize = "A0";
            }
            return paperSize;
        }


        public static class SendFileToServer
        {
            private static string host = ApplicationConstant.BRAVO_IP;

            // Enter your sftp username here
            private static string username = ApplicationConstant.BRAVO_ACCOUNT;

            // Enter your sftp password here
            private static string password = ApplicationConstant.BRAVO_PW;
            public static int Send(string fileName,string path)
            {
               // var connectionInfo = new ConnectionInfo(host, "sftp", new PasswordAuthenticationMethod(username, password));

                using (SftpClient sftpClient = new SftpClient(host,22, username, password))
                {
                    Console.WriteLine("Connect to server");
                    sftpClient.Connect();
                    
                    sftpClient.ChangeDirectory( path);
                    Console.WriteLine("Creating FileStream object to stream a file");
                    using (FileStream fs = new FileStream(fileName, FileMode.Open))
                    {
                        sftpClient.BufferSize = 1024;
                        sftpClient.UploadFile(fs, Path.GetFileName(fileName));
                    }
                    sftpClient.Dispose();
                }
                     
                //// Upload File
                //using (var sftp = new SftpClient(connectionInfo))
                //{

                //    sftp.Connect();
                //    sftp.ChangeDirectory("/" + path);
                //    //sftp.ChangeDirectory("/MyFolder");
                //    using (var uplfileStream = System.IO.File.OpenRead(fileName))
                //    {
                //        sftp.UploadFile(uplfileStream, fileName, true);
                //    }
                //    sftp.Disconnect();
                //}
                return 0;
            }
        }



    }
}
