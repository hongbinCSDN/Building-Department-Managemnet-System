using MWMS2.Areas.Registration.Models;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using MWMS2.Utility;
using MWMS2.Constant;
using System.Text;
using System.IO;
using OfficeOpenXml;
using System.IO.Compression;

namespace MWMS2.Services
{
    public class RegistrationDataExportService : RegistrationCommonService
    {
        private static log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string QP_SQL =
            "		SELECT  " +
            //"		UUID, "+
            "		CASE WHEN ENGLISH_NAME IS NULL OR ENGLISH_NAME='' THEN CHINESE_NAME ELSE ENGLISH_NAME END AS ENGLISH_NAME,                                         " +
            "		CASE WHEN CHINESE_NAME IS NULL OR CHINESE_NAME='' THEN                                                                                             " +
            "		 ENGLISH_NAME ELSE CHINESE_NAME ||  ' ' || ENGLISH_NAME END AS CHINESE_NAME,                                                                       " +
            "		CASE WHEN ENGLISH_COMPANY_NAME IS NULL OR ENGLISH_COMPANY_NAME='' THEN CHINESE_COMPANY_NAME ELSE ENGLISH_COMPANY_NAME END AS ENGLISH_COMPANY_NAME, " +
            "		CASE WHEN CHINESE_COMPANY_NAME IS NULL OR CHINESE_COMPANY_NAME='' THEN ENGLISH_COMPANY_NAME ELSE                                                   " +
            "		  CHINESE_COMPANY_NAME ||  ' ' || ENGLISH_COMPANY_NAME end AS CHINESE_COMPANY_NAME,                                                                " +
            "		REGISTRATION_NUMBER,                                                                                                                               " +
            "		REGISTRATION_TYPE,                                                                                                                                 " +
            "		CASE WHEN DISTRICT_ENGLISH IS NULL OR DISTRICT_ENGLISH='' THEN '-' ELSE DISTRICT_ENGLISH END AS DISTRICT_ENGLISH,                                  " +
            "		CASE WHEN DISTRICT_CHINESE IS NULL OR DISTRICT_CHINESE='' THEN '-' ELSE DISTRICT_CHINESE END AS DISTRICT_CHINESE,                                  " +
            "		CASE WHEN ITEMS IS NULL OR ITEMS='' THEN '-' ELSE ITEMS END AS ITEMS,                                                                              " +
            "		FLAG,  " +
            "		CASE WHEN SERVICE_IN_BUILDING_SAFETY IS NULL OR SERVICE_IN_BUILDING_SAFETY='' THEN '-' ELSE                                                        " +
            "			 SERVICE_IN_BUILDING_SAFETY END AS SERVICE_IN_BUILDING_SAFETY,                                                                                 " +
            "	   	CASE WHEN TELEPHONE_NO IS NULL OR TELEPHONE_NO='' THEN '-' ELSE TELEPHONE_NO END AS TELEPHONE_NO,                                                  " +
            "	  	CASE WHEN EMAIL IS NULL OR EMAIL='' THEN '-' ELSE EMAIL END AS EMAIL,                                                                              " +
            "	  	CASE WHEN FAX IS NULL OR FAX='' THEN '-' ELSE FAX END AS FAX,                                                                                      " +
            "		CASE WHEN AS_NAME_ENGLISH IS NULL OR AS_NAME_ENGLISH='' THEN '#' ELSE AS_NAME_ENGLISH END AS AS_NAME_ENGLISH,                                      " +
            "		CASE WHEN AS_NAME_CHINESE IS NULL OR AS_NAME_CHINESE='' THEN '#' ELSE AS_NAME_CHINESE END AS AS_NAME_CHINESE,                                      " +
            "		COMPANY_TYPE_ONE,                                                                                                                                  " +
            "		COMPANY_TYPE_TWO, " +
            "		COMPANY_TYPE_THREE, " +
            "		AS_TYPE_ONE, " +
            "		AS_TYPE_TWO, " +
            "		AS_TYPE_THREE, " +
            "		SEARCH_TYPE, " +
            "		ITEM_KEY, " +
            "		CASE WHEN BS_KEY IS NULL OR BS_KEY='' THEN '' ELSE BS_KEY END AS BS_KEY, " +
            "		TYPE_KEY, " +
            "		CASE WHEN WILLINGNESS_QP='Y' THEN '*' ELSE '' END AS WILLINGNESS_QP, " +
            "		INTERESTED_FSS, " +
            "		MBIS_RI, " +
            "		EXPIRY_DATE, " +
            "		FLAG                                                                                                                                    " +
            //--"	 --	SERVICE_IN_MWIS, "+
            //  "		COUNT(*) OVER () as TotalCount   "+
            "		FROM c_SEARCH_REGISTRATION_QP  " +
            "		WHERE     (SEARCH_TYPE = 'QP' )  " +
            "		ORDER BY REGISTRATION_TYPE, ENGLISH_COMPANY_NAME,CHINESE_COMPANY_NAME , ENGLISH_NAME,CHINESE_NAME   ";

        private String registerSQL =

             " select registration_number, registration_type, english_name, chinese_name, english_company_name, chinese_company_name, " +
             " case when company_type_one is not null and company_type_two is not null and company_type_three is not null then 'I,II,III<br>II,III<br>III' " +
             " when company_type_one is not null and company_type_two is not null and company_type_three is null then 'I,II,III<br>II,III' " +
             " when company_type_one is not null and company_type_two is null and company_type_three is null then 'I,II,III' " +
             " when company_type_one is null and company_type_two is not null and company_type_three is not null then 'II,III<br>III' " +
             " when company_type_one is null and company_type_two is not null and company_type_three is null then 'II,III' " +
             " when company_type_one is null and company_type_two is null and company_type_three is not null then 'III' " +
             " end comp_class, " +
             "    case when company_type_one is not null and company_type_two is not null and company_type_three is not null then  company_type_one || '<br>' || company_type_two || '<br>' || company_type_three " +
             " when company_type_one is not null and company_type_two is not null and company_type_three is null then company_type_one||'<br>'||company_type_two " +
             " when company_type_one is not null and company_type_two is null and company_type_three is null then company_type_one " +
             " when company_type_one is null and company_type_two is not null and company_type_three is not null then company_type_two||'<br>'||company_type_three " +
             " when company_type_one is null and company_type_two is not null and company_type_three is null then company_type_two " +
             " when company_type_one is null and company_type_two is null and company_type_three is not null then company_type_three " +
            " end comp_type, " +
            " as_name_english, as_name_chinese, " +
            " case when as_type_one is not null and as_type_two is not null and as_type_three is not null then 'I,II,III<br>II,III<br>III' " +
            " when as_type_one is not null and as_type_two is not null and as_type_three is null then 'I,II,III<br>II,III' " +
            " when as_type_one is not null and as_type_two is null and as_type_three is null then 'I,II,III' " +
            " when as_type_one is null and as_type_two is not null and as_type_three is not null then 'II,III<br>III' " +
            " when as_type_one is null and as_type_two is not null and as_type_three is null then 'II,III' " +
            " when as_type_one is null and as_type_two is null and as_type_three is not null then 'III' " +
            "  end as_class, " +
            " case when as_type_one is not null and as_type_two is not null and as_type_three is not null then as_type_one||'<br>'||as_type_two||'<br>'||as_type_three " +
            "    when as_type_one is not null and as_type_two is not null and as_type_three is null then as_type_one||'<br>'||as_type_two " +
            "   when as_type_one is not null and as_type_two is null and as_type_three is null then as_type_one " +
            "   when as_type_one is null and as_type_two is not null and as_type_three is not null then as_type_two||'<br>'||as_type_three " +
            "  when as_type_one is null and as_type_two is not null and as_type_three is null then as_type_two " +
            "  when as_type_one is null and as_type_two is null and as_type_three is not null then as_type_three " +
            " end as_type, " +
            " to_char(expiry_date, 'DD/MM/YYYY') expiry_date, flag, items, " +
            " service_in_building_safety, interested_fss, mbis_ri, " +
            " district_english, district_chinese, telephone_no, fax, email " +
            " from C_Search_registration_qp where search_type = 'REG'  " +
            " order by registration_type, english_name, english_company_name ";

        //private String Query_CancelMeeting =""+
        //     "\r\n" + "\t" +" SELECT sv.ENGLISH_DESCRIPTION, ca.SURNAME, ca.given_name_on_id , cis.meeting_NUMBER, meeting.year, meeting.meeting_no " +
        //     "\r\n" + "\t" +" , cis.interview_date, ccm.FAX_NO1, ccm.fax_no2 " +
        //     "\r\n" + "\t" +" ,(select (ca2.surname || ' ' || ca2.given_name_on_id) from C_MEETING_MEMBER mm2 " +
        //     "\r\n" + "\t" +" inner join C_COMMITTEE_MEMBER ccm2 on ccm2.UUID = mm2.MEMBER_ID "+ 
        //     "\r\n" + "\t" +" inner join C_APPLICANT ca2 on ca2.UUID = ccm2.APPLICANT_ID "+
        //     "\r\n" + "\t" +" inner join C_MEETING meeting2 on meeting2.UUID = mm2.meeting_ID "+
        //     "\r\n" + "\t" +" inner join C_S_SYSTEM_VALUE sv2 on sv2.UUID = mm2.committee_role_id "+
        //     "\r\n" + "\t" +" where meeting2.UUID = '8a8591a22590a121012591576d4800aa' "+
        //     "\r\n" + "\t" +" and sv2.code = '2') as secretary " +
        //     "\r\n" + "\t" +" from C_MEETING_MEMBER mm  " +
        //     "\r\n" + "\t" +" inner join C_COMMITTEE_MEMBER ccm on ccm.UUID = mm.MEMBER_ID  " +
        //     "\r\n" + "\t" +" inner join C_APPLICANT ca on ca.UUID = ccm.APPLICANT_ID  " +
        //     "\r\n" + "\t" +" inner join C_S_SYSTEM_VALUE sv on sv.UUID = ca.TITLE_ID  " +
        //     "\r\n" + "\t" +" inner join C_INTERVIEW_SCHEDULE cis on cis.MEETING_ID = mm.MEETING_ID  " +
        //     "\r\n" + "\t" +" inner join C_MEETING meeting on meeting.UUID = cis.MEETING_ID  " +
        //     "\r\n" + "\t" + " where cis.meeting_number  = '2010-GBC-B-01' ";

        private String Query_ExportCRCMeeting =
             " SELECT cis.meeting_NUMBER, meeting.year, meeting.meeting_group " +
             " ,cis.interview_date, sv.ENGLISH_DESCRIPTION, (ca.Surname || ' ' || ca.given_name_on_id) as fullName " +
             " ,(select (ca2.surname || ' ' || ca2.given_name_on_id) from C_MEETING_MEMBER mm2 " +
             " inner join C_COMMITTEE_MEMBER ccm2 on ccm2.UUID = mm2.MEMBER_ID " +
             " inner join C_APPLICANT ca2 on ca2.UUID = ccm2.APPLICANT_ID " +
             " inner join C_MEETING meeting2 on meeting2.UUID = mm2.meeting_ID " +
             " inner join C_S_SYSTEM_VALUE sv2 on sv2.UUID = mm2.committee_role_id " +
             " where meeting2.UUID = '8a8591a22590a121012591576d4800aa' " +
             " and sv2.code = '2') as secretary " +
             " from C_MEETING_MEMBER mm  " +
             " inner join C_COMMITTEE_MEMBER ccm on ccm.UUID = mm.MEMBER_ID  " +
             " inner join C_APPLICANT ca on ca.UUID = ccm.APPLICANT_ID  " +
             " inner join C_S_SYSTEM_VALUE sv on sv.UUID = ca.TITLE_ID  " +
             " inner join C_INTERVIEW_SCHEDULE cis on cis.MEETING_ID = mm.MEETING_ID  " +
             " inner join C_MEETING meeting on meeting.UUID = cis.MEETING_ID  " +
             " where cis.meeting_number  = '2010-GBC-B-01' ";

        //private String Query_CRCMeetingMBR =
        //     " SELECT sv.ENGLISH_DESCRIPTION, ca.SURNAME, ca.given_name_on_id , ccm.english_care_of " +
        //     " , addr.address_line1, addr.address_line2, addr.address_line3, addr.address_line4, addr.address_line5 " +
        //     " , addr2.address_line1, addr2.address_line2, addr2.address_line3, addr2.address_line4, addr2.address_line5, cis.interview_date " +
        //     " ,(select (ca2.surname || ' ' || ca2.given_name_on_id) from C_MEETING_MEMBER mm2 " +
        //     " inner join C_COMMITTEE_MEMBER ccm2 on ccm2.UUID = mm2.MEMBER_ID " +
        //     " inner join C_APPLICANT ca2 on ca2.UUID = ccm2.APPLICANT_ID " +
        //     " inner join C_MEETING meeting2 on meeting2.UUID = mm2.meeting_ID " +
        //     " inner join C_S_SYSTEM_VALUE sv2 on sv2.UUID = mm2.committee_role_id " +
        //     " where meeting2.UUID = '8a8591a22590a121012591576d4800aa' " +
        //     " and sv2.code = '2') as fullName " +
        //     " from C_MEETING_MEMBER mm  " +
        //     " inner join C_COMMITTEE_MEMBER ccm on ccm.UUID = mm.MEMBER_ID  " +
        //     " inner join C_APPLICANT ca on ca.UUID = ccm.APPLICANT_ID  " +
        //     " inner join C_S_SYSTEM_VALUE sv on sv.UUID = ca.TITLE_ID  " +
        //     " inner join C_INTERVIEW_SCHEDULE cis on cis.MEETING_ID = mm.MEETING_ID  " +
        //     " inner join C_MEETING meeting on meeting.UUID = cis.MEETING_ID  " +
        //     " inner join C_ADDRESS addr on addr.UUID = ccm.english_address_id  " +
        //     " inner join C_ADDRESS addr2 on addr2.UUID = ccm.chinese_address_id  " +
        //     " where cis.meeting_number  = '2010-GBC-B-01' ";


        public string addValueLine(string line, string appendStr)
        {
            line += string.IsNullOrEmpty(line) ? appendDoubleQuote(appendStr) : "," + appendDoubleQuote(appendStr);
            return line;
        }

        public FileStreamResult CreateZipFileFileStreamResult<T>(IEnumerable<T> objectList, string fileName)
        {

            using (FileStream zipToOpen = new FileStream(@"c:\MWMS2\release.zip", FileMode.Create))
            {
                using (ZipArchive test = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry readmeEntry = test.CreateEntry("Readme.txt", System.IO.Compression.CompressionLevel.Optimal);
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        writer.WriteLine("Information about this package.");
                        writer.WriteLine("========================");
                    }
                }
            }

            var ms = new MemoryStream();
            var contentType = System.Net.Mime.MediaTypeNames.Application.Zip;
            ExcelPackage excelPackage = null;
            ZipArchive archive = null;
            try
            {
                excelPackage = new ExcelPackage(ms);

                var workSheet1 = excelPackage.Workbook.Worksheets.Add("Sheet1");

                workSheet1.Cells["A1"].LoadFromCollection<T>(objectList, true);

                excelPackage.SaveAs(ms);

                ms.Seek(0, SeekOrigin.Begin);


                var memoryStream = new MemoryStream();

                FileStream zipToOpen = new FileStream(@"c:\MWMS2\release.zip", FileMode.Create);


                using (ZipArchive test = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry readmeEntry = test.CreateEntry("Readme.txt", System.IO.Compression.CompressionLevel.Optimal);
                    using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                    {
                        writer.WriteLine("Information about this package.");
                        writer.WriteLine("========================");
                    }
                }




                var archiveZip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true);




                ZipArchiveEntry demoFile = archiveZip.CreateEntry("test.xls");
                var entryStream = demoFile.Open();

                var fsr = new FileStreamResult(new FileStream(@"c:\MWMS2\release.zip", FileMode.Open), contentType);
                fsr.FileDownloadName = fileName + ".zip";

                return fsr;
            }
            catch (Exception ex)
            {
                if (archive != null)
                    archive.Dispose();

                if (excelPackage != null)
                    excelPackage.Dispose();

                if (ms != null)
                    ms.Dispose();

                throw;
            }
        }



        public FileStreamResult exportALLRegistrationData()
        {

            String resultZipFilePath =
                ApplicationConstant.getTempPath("RegisterExport");

            resultZipFilePath = resultZipFilePath + "register.zip";


            List<FileStreamResult> fileList = new List<FileStreamResult>();

            fileList.Add(exportExcelFile("test.",
                new List<string> { "asdfasdf", "asdfa" }, null));
            // return fileList[0];
            fileList.Add(exportExcelFile("sdfasd",
                new List<string> { "asdfasdf", "asdfa" }, null));


            /*******************************************************************/
            string path = @"C:\MWMS2\ZIP\" + "register.zip";

            FileStream zipToOpen = new FileStream(resultZipFilePath, FileMode.Create);
            using (ZipArchive test = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
            {
                for (int i = 0; i < fileList.Count; i++)
                {
                    FileStreamResult eachFile = fileList[i];
                    ZipArchiveEntry readmeEntry = test.CreateEntry(eachFile.FileDownloadName,
                        System.IO.Compression.CompressionLevel.Optimal);
                    using (var zipStream = readmeEntry.Open())
                        eachFile.FileStream.CopyTo(zipStream);
                }
            }
            FileStreamResult result = new FileStreamResult(
                 new FileStream(resultZipFilePath, FileMode.Open), "application/zip");
            result.FileDownloadName = "register.ZIP";
            return result;
        }

        public FileStreamResult exportExcelRegistrationData()
        {
            List<string> headerList = new List<string>() {
                "REGISTRATION_NUMBER",
                "REGISTRATION_TYPE",
                "ENGLISH_NAME",
                "CHINESE_NAME",
                "ENGLISH_COMPANY_NAME",
                "CHINESE_COMPANY_NAME",
                "COMP_CLASS",
                "COMP_TYPE",
                "AS_NAME_ENGLISH",
                "AS_NAME_CHINESE",
                "AS_CLASS",
                "AS_TYPE",
                "EXPIRY_DATE",
                "FLAG",
                "ITEMS",
                "SERVICE_IN_BUILDING_SAFETY",
                "INTERESTED_FSS",
                "MBIS_RI",
                "DISTRICT_ENGLISH",
                "DISTRICT_CHINESE",
                "TELEPHONE_NO",
                "FAX",
                "EMAIL"};
            List<List<object>> data = null;

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    String sql = this.registerSQL;
                    DbDataReader dr = CommonUtil.GetDataReader(conn, sql, null);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return this.exportCSVFile("registers_full_list", headerList, data);

        }
        public FileStreamResult exportCSVRegistrationData_OLD()
        {
            List<string> headerList = new List<string>() {
                "REGISTRATION_NUMBER",
                "REGISTRATION_TYPE",
                "ENGLISH_NAME",
                "CHINESE_NAME",
                "ENGLISH_COMPANY_NAME",
                "CHINESE_COMPANY_NAME",
                "COMP_CLASS",
                "COMP_TYPE",
                "AS_NAME_ENGLISH",
                "AS_NAME_CHINESE",
                "AS_CLASS",
                "AS_TYPE",
                "EXPIRY_DATE",
                "FLAG",
                "ITEMS",
                "SERVICE_IN_BUILDING_SAFETY",
                "INTERESTED_FSS",
                "MBIS_RI",
                "DISTRICT_ENGLISH",
                "DISTRICT_CHINESE",
                "TELEPHONE_NO",
                "FAX",
                "EMAIL"};

            var sb = new StringBuilder();

            String headerLine = "";
            foreach (String header in headerList)
            {
                headerLine = appendCertContent(headerLine, header);
            }
            headerLine += Environment.NewLine;

            sb.Append(headerLine);

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    String sql = this.registerSQL;

                    DbDataReader dr = CommonUtil.GetDataReader(conn, sql, null);

                    //List<Dictionary<string, object>> datas = new List<Dictionary<string, object>>();
                    while (dr.Read())
                    {
                        String eachLine = "";
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            String dataObejct = dr.GetValue(i).ToString();
                            if (dr.GetValue(i) is DateTime)
                            {
                                dataObejct = ((DateTime)dr.GetValue(i)).ToString(ApplicationConstant.DISPLAY_DATE_FORMAT);
                            }
                            if (i == 0)
                            {
                                eachLine = appendDoubleQuote(dataObejct);
                            }
                            else
                            {
                                eachLine = addValueLine(eachLine, dataObejct);
                            }
                        }
                        eachLine += Environment.NewLine;
                        sb.Append(eachLine);

                    }
                    conn.Close();
                }
            }




            var byteArray = Encoding.UTF8.GetBytes(sb.ToString());
            var stream = new MemoryStream(byteArray);
            var mimeType = "text/plain";
            FileStreamResult result = new FileStreamResult(stream, mimeType);
            result.FileDownloadName = "registers_full_list.csv";
            return result;


        }
        private List<List<object>> getIndivGazetteMemoData(DateTime? dataOfGazette, string regType, string acting, string authId)
        {
            string queryStr =
                "\r\n" + "\t" + "SELECT DISTINCT C_RPT_INDIV_GAZETTE( :dateOfGazette, :registrationType ) as FILE_REF_STRING, " +
                "\r\n" + "\t" + " CASE WHEN :acting1 = 1 then S_AUTH.ENGLISH_ACTION_NAME ELSE S_AUTH.ENGLISH_NAME END AS AUTH_ENAME, " +
                "\r\n" + "\t" + " CASE WHEN :acting2 = 1 then S_AUTH.CHINESE_ACTION_NAME ELSE S_AUTH.CHINESE_NAME END AS AUTH_CNAME, " +
                "\r\n" + "\t" + " CASE WHEN :acting3 = 1 then S_AUTH.ENGLISH_ACTION_RANK ELSE S_AUTH.ENGLISH_RANK END AS AUTH_ERANK, " +
                "\r\n" + "\t" + " CASE WHEN :acting4 = 1 then S_AUTH.CHINESE_ACTION_RANK ELSE S_AUTH.CHINESE_RANK END AS AUTH_CRANK, " +
                "\r\n" + "\t" + " S_AUTH.TELEPHONE_NO AS AUTH_TEL," +
                "\r\n" + "\t" + " S_AUTH.FAX_NO AS AUTH_FAX " +
                "\r\n" + "\t" + " FROM " +
                "\r\n" + "\t" + " C_S_AUTHORITY S_AUTH ";
            if (authId != null && !"".Equals(authId.Trim()))
            {
                queryStr += "\r\n" + "\t" + " WHERE" + " S_AUTH.UUID = :authId";
            }
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            queryParameters.Add("dateOfGazette", dataOfGazette.ToString());
            queryParameters.Add("registrationType", regType);
            queryParameters.Add("acting1", acting);
            queryParameters.Add("acting2", acting);
            queryParameters.Add("acting3", acting);
            queryParameters.Add("acting4", acting);
            if (authId != null && !"".Equals(authId.Trim()))
            {
                queryParameters.Add("authId", authId);
            }
            List<List<object>> data = new List<List<object>>();
            using (EntitiesRegistration db = new EntitiesRegistration())
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
        private List<List<object>> getCompGazetteMemoData(DateTime? dataOfGazette, string regType, string acting, string authId)
        {
            string queryStr =
                "\r\n" + "\t" + "SELECT DISTINCT C_RPT_COMP_GAZETTE( :dateOfGazette, :registrationType ) as FILE_REF_STRING, " +
                "\r\n" + "\t" + "\r\n" + "\t" + " CASE WHEN :acting1 = 1 then S_AUTH.ENGLISH_ACTION_NAME ELSE S_AUTH.ENGLISH_NAME END AS AUTH_ENAME, " +
                "\r\n" + "\t" + " CASE WHEN :acting2 = 1 then S_AUTH.CHINESE_ACTION_NAME ELSE S_AUTH.CHINESE_NAME END AS AUTH_CNAME, " +
                "\r\n" + "\t" + " CASE WHEN :acting3 = 1 then S_AUTH.ENGLISH_ACTION_RANK ELSE S_AUTH.ENGLISH_RANK END AS AUTH_ERANK, " +
                "\r\n" + "\t" + " CASE WHEN :acting4 = 1 then S_AUTH.CHINESE_ACTION_RANK ELSE S_AUTH.CHINESE_RANK END AS AUTH_CRANK, " +
                "\r\n" + "\t" + " S_AUTH.TELEPHONE_NO AS AUTH_TEL," +
                "\r\n" + "\t" + " S_AUTH.FAX_NO AS AUTH_FAX " +
                "\r\n" + "\t" + " FROM " +
                "\r\n" + "\t" + " C_S_AUTHORITY S_AUTH ";
            if (authId != null && !"".Equals(authId.Trim()))
            {
                queryStr += "\r\n" + "\t" + " WHERE" + " S_AUTH.UUID = :authId";
            }
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            queryParameters.Add("dateOfGazette", dataOfGazette.ToString());
            queryParameters.Add("registrationType", regType);
            queryParameters.Add("acting1", acting);
            queryParameters.Add("acting2", acting);
            queryParameters.Add("acting3", acting);
            queryParameters.Add("acting4", acting);
            if (authId != null && !"".Equals(authId.Trim()))
            {
                queryParameters.Add("authId", authId);
            }

            List<List<object>> data = new List<List<object>>();
            using (EntitiesRegistration db = new EntitiesRegistration())
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
        private List<List<object>> getWeeklyNotice(DateTime? dataOfGazette, string regType, string acting, string authId)
        {
            List<List<object>> data = new List<List<object>>();
            string queryStr =
                "\r\n" + "\t" + "SELECT DISTINCT C_RPT_COMP_APP_WEEKLY( :dateOfGazette, :registrationType ) as FILE_REF_STRING, " +
                "\r\n" + "\t" + " CASE WHEN :acting1 = 1 then S_AUTH.ENGLISH_ACTION_NAME ELSE S_AUTH.ENGLISH_NAME END AS AUTH_ENAME, " +
                "\r\n" + "\t" + " CASE WHEN :acting2 = 1 then S_AUTH.CHINESE_ACTION_NAME ELSE S_AUTH.CHINESE_NAME END AS AUTH_CNAME, " +
                "\r\n" + "\t" + " CASE WHEN :acting3 = 1 then S_AUTH.ENGLISH_ACTION_RANK ELSE S_AUTH.ENGLISH_RANK END AS AUTH_ERANK, " +
                "\r\n" + "\t" + " CASE WHEN :acting4 = 1 then S_AUTH.CHINESE_ACTION_RANK ELSE S_AUTH.CHINESE_RANK END AS AUTH_CRANK, " +
                "\r\n" + "\t" + " S_AUTH.TELEPHONE_NO AS AUTH_TEL," +
                "\r\n" + "\t" + " S_AUTH.FAX_NO AS AUTH_FAX " +
                "\r\n" + "\t" + " FROM " +
                "\r\n" + "\t" + " C_S_AUTHORITY S_AUTH ";
            if (authId != null && !authId.Trim().Equals(""))
            {
                queryStr += " WHERE" + " S_AUTH.UUID = :authId";
            }
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            queryParameters.Add("dateOfGazette", dataOfGazette.ToString());
            queryParameters.Add("registrationType", regType);
            queryParameters.Add("acting1", acting);
            queryParameters.Add("acting2", acting);
            queryParameters.Add("acting3", acting);
            queryParameters.Add("acting4", acting);
            if (authId != null && !"".Equals(authId.Trim()))
            {
                queryParameters.Add("authId", authId);
            }
            //for (int i = 0; i < list.size(); i++)
            //{
            //    Object[] element = (Object[])list.get(i);
            //    String[] newElement = new String[element.length + 1];

            //    for (int j = 0; j < element.length; j++)
            //    {
            //        newElement[j] = (String)element[j];
            //    }
            //    if (acting.equals("1"))
            //    {
            //        newElement[element.length] = "True";
            //    }
            //    else
            //    {
            //        newElement[element.length] = "False";
            //    }
            //    list.remove(i);
            //    list.add(i, newElement);
            //}
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryStr, queryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            //for (int i = 0; i < data.Count(); i++)
            //{
            //    object[] element = data.ElementAt(i);
            //    string[] newElement = new string[element.Length + 1];

            //    for (int j = 0; j < element.Length; j++)
            //    {
            //        newElement[j] = (string)element[j];
            //    }
            //    if (acting.Equals("1"))
            //    {
            //        newElement[element.Length] = "True";
            //    }
            //    else
            //    {
            //        newElement[element.Length] = "False";
            //    }
            //    data.Remove(i);
            //    data.Add(i, newElement);
            //}
            return data;
        }
        private List<List<object>> getCGCLabelTwo(
            string regtype, string txtFileRef,
            DateTime? gaz_fr_date, DateTime? gaz_to_date,
            DateTime? expiry_fr_date, DateTime? expiry_to_date,
            string ddPNRCUUID, string ddCtrUUID, string companyName,
            string chkExpiryDate)
        {
            List<List<object>> data = new List<List<object>>();
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            string queryStr =
                "\r\n" + "\t" + " SELECT " +
                "\r\n" + "\t" + " C_APPL.FILE_REFERENCE_NO AS FILE_REF, " +
                "\r\n" + "\t" + " C_APPL.ENGLISH_COMPANY_NAME AS C_ENAME, " +
                "\r\n" + "\t" + " ADDR.ADDRESS_LINE1 AS ADDR1, " +
                "\r\n" + "\t" + " ADDR.ADDRESS_LINE2 AS ADDR2, " +
                "\r\n" + "\t" + " ADDR.ADDRESS_LINE3 AS ADDR3, " +
                "\r\n" + "\t" + " ADDR.ADDRESS_LINE4 AS ADDR4, " +
                "\r\n" + "\t" + " ADDR.ADDRESS_LINE5 AS ADDR5, C_APPL.ENGLISH_CARE_OF  " +
                "\r\n" + "\t" + " FROM " +
                "\r\n" + "\t" + " C_COMP_APPLICATION C_APPL , C_ADDRESS ADDR where ADDR.UUID = C_APPL.ENGLISH_ADDRESS_ID " +
                "\r\n" + "\t" + " and C_APPL.REGISTRATION_TYPE =  :regtype ";

            queryParameters.Add("regtype", regtype);
            if (!string.IsNullOrWhiteSpace(txtFileRef))
            {
                queryStr += "\r\n" + "\t" + " AND upper(C_APPL.file_reference_no) LIKE  :txtFileRef ";
                queryParameters.Add("txtFileRef", "%" + txtFileRef.Trim().ToUpper() + "%");
            }
            if (gaz_fr_date != null)
            {
                queryStr += "\r\n" + "\t" + " AND C_APPL.gazette_date >= :gaz_fr_date ";
                queryParameters.Add("gaz_fr_date", gaz_fr_date);
            }
            if (gaz_to_date != null)
            {
                queryStr += "\r\n" + "\t" + " AND C_APPL.gazette_date <= :gaz_to_date ";
                queryParameters.Add("gaz_to_date", gaz_to_date);
            }
            if (expiry_fr_date != null)
            {
                queryStr += "\r\n" + "\t" + " AND C_APPL.expiry_date >= :expiry_fr_date ";
                queryParameters.Add("expiry_fr_date", expiry_fr_date);
            }
            if (expiry_to_date != null)
            {
                queryStr += "\r\n" + "\t" + " AND C_APPL.expiry_date <= :expiry_to_date ";
                queryParameters.Add("expiry_to_date", expiry_to_date);
            }
            if (!string.IsNullOrWhiteSpace(ddPNRCUUID))
            {
                queryStr += "\r\n" + "\t" + " AND C_APPL.practice_notes_id = :ddPNRCUUID ";
                queryParameters.Add("ddPNRCUUID", ddPNRCUUID);
            }
            if (!string.IsNullOrWhiteSpace(ddCtrUUID))
            {
                queryStr += "\r\n" + "\t" + " AND C_APPL.category_id = :ddCtrUUID ";
                queryParameters.Add("ddCtrUUID", ddCtrUUID);
            }
            if (!string.IsNullOrWhiteSpace(companyName))
            {
                queryStr += " AND upper(C_APPL.english_company_name) LIKE  :companyName ";
                queryParameters.Add("companyName", "%" + companyName.Trim().ToUpper() + "%");
            }
            if ("on".Equals(chkExpiryDate))
            {
                queryStr +=
                "\r\n" + "\t" + " AND C_APPL.CERTIFICATION_NO IS NOT NULL   " +
                "\r\n" + "\t" + " AND  ( (C_APPL.EXPIRY_DATE IS NOT NULL and  C_APPL.EXPIRY_DATE >= CURRENT_DATE) or " +
                "\r\n" + "\t" + " ( (C_APPL.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and (C_APPL.EXPIRY_DATE < CURRENT_DATE) ) )" +
                "\r\n" + "\t" + " ) " +
                "\r\n" + "\t" + " AND ((C_APPL.REMOVAL_DATE IS NULL) OR  " +
                "\r\n" + "\t" + "      (C_APPL.REMOVAL_DATE > CURRENT_DATE) " +
                "\r\n" + "\t" + " ) ";
            }
            queryStr += "\r\n" + "\t" + " ORDER BY C_APPL.FILE_REFERENCE_NO ";

            using (EntitiesRegistration db = new EntitiesRegistration())
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

        private List<List<object>> getLabelInterviewers(string meetingGroupUUID)
        {
            string queryStr =
                            " SELECT " +
                            " '' as FILE_REF, " +
                            " S_TITLE.CODE AS TITLE, " +
                            " APPLN.SURNAME || ' ' || APPLN.GIVEN_NAME_ON_ID AS APPLN_NAME, " +
                            " '' AS C_ENAME, " +
                            " ADDR.ADDRESS_LINE1 AS ADDR1, " +
                            " ADDR.ADDRESS_LINE2 AS ADDR2, " +
                            " ADDR.ADDRESS_LINE3 AS ADDR3, " +
                            " ADDR.ADDRESS_LINE4 AS ADDR4, " +
                            " ADDR.ADDRESS_LINE5 AS ADDR5 " +
                            " FROM " +
                            " C_MEETING M " +
                            " INNER JOIN C_MEETING_MEMBER M_MEM ON M.UUID = M_MEM.MEETING_ID " +
                            " INNER JOIN C_COMMITTEE_MEMBER C_MEM ON M_MEM.MEMBER_ID = C_MEM.UUID " +
                            " INNER JOIN C_APPLICANT APPLN ON C_MEM.APPLICANT_ID = APPLN.UUID " +
                            " INNER JOIN C_ADDRESS ADDR ON C_MEM.ENGLISH_ADDRESS_ID = ADDR.UUID " +
                            " LEFT JOIN C_S_SYSTEM_VALUE S_TITLE ON S_TITLE.UUID = APPLN.TITLE_ID " +
                            " WHERE " +
                            " M.UUID= :meetingGroupUUID ";

            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            queryParameters.Add("meetingGroupUUID", meetingGroupUUID);

            List<List<object>> data = new List<List<object>>();
            using (EntitiesRegistration db = new EntitiesRegistration())
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


        private List<List<object>> getCGCLabelInterviewees(string regType, string meetingGroupUUID)
        {
            string queryStr =
                            " SELECT " +
                            " C_APPL.FILE_REFERENCE_NO AS FILE_REF, " +
                            " S_TITLE.CODE AS TITLE, " +
                            " APPLN.SURNAME || ' ' || APPLN.GIVEN_NAME_ON_ID AS APPLN_NAME, " +
                            " C_APPL.ENGLISH_COMPANY_NAME AS C_ENAME, " +
                            " ADDR.ADDRESS_LINE1 AS ADDR1, " +
                            " ADDR.ADDRESS_LINE2 AS ADDR2, " +
                            " ADDR.ADDRESS_LINE3 AS ADDR3, " +
                            " ADDR.ADDRESS_LINE4 AS ADDR4, " +
                            " ADDR.ADDRESS_LINE5 AS ADDR5 " +
                            " FROM " +
                            " C_ADDRESS ADDR INNER JOIN C_COMP_APPLICATION C_APPL ON ADDR.UUID = C_APPL.ENGLISH_ADDRESS_ID " +
                            " INNER JOIN C_COMP_APPLICANT_INFO C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID " +
                            " INNER JOIN C_INTERVIEW_CANDIDATES INTRV_CAN ON C_APPLN.CANDIDATE_NUMBER = INTRV_CAN.CANDIDATE_NUMBER " +
                            " INNER JOIN C_APPLICANT APPLN ON C_APPLN.APPLICANT_ID = APPLN.UUID " +
                            " INNER JOIN C_INTERVIEW_SCHEDULE INTRV_SCH ON INTRV_CAN.INTERVIEW_SCHEDULE_ID = INTRV_SCH.UUID " +
                            " LEFT JOIN C_S_SYSTEM_VALUE S_TITLE ON S_TITLE.UUID = APPLN.TITLE_ID " +
                            " WHERE " +
                            " C_APPL.REGISTRATION_TYPE = :regtype  " +
                            " AND INTRV_SCH.MEETING_ID = :meetingGroupUUID ";

            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            queryParameters.Add("meetingGroupUUID", meetingGroupUUID);
            queryParameters.Add("regtype", regType);

            List<List<object>> data = new List<List<object>>();
            using (EntitiesRegistration db = new EntitiesRegistration())
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
        private String[] getApplicantNameForExport(EntitiesRegistration dbss, string applicationUUID, string roleCode)
        {

            string sql =
               "SELECT " +
               "A.SURNAME, A.GIVEN_NAME_ON_ID, A.CHINESE_NAME " +
               "FROM C_COMP_APPLICANT_INFO T, C_APPLICANT A, C_S_SYSTEM_VALUE R , C_S_SYSTEM_VALUE status " +
               "WHERE T.APPLICANT_ID = A.UUID AND T.APPLICANT_ROLE_ID = R.UUID  " +
               "AND T.APPLICANT_STATUS_ID = status.UUID  " +
               "AND T.MASTER_ID = :C_COMP_APPLICATION_UUID  " +
               "AND r.CODE = :ROLE_CODE  " +
                "AND status.CODE = :STATUS_CODE  " +
                "AND(T.REMOVAL_DATE IS NULL OR TO_DATE(T.REMOVAL_DATE) > TO_DATE(SYSDATE))  ";

            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            queryParameters.Add("C_COMP_APPLICATION_UUID", applicationUUID);
            queryParameters.Add("ROLE_CODE", roleCode);
            queryParameters.Add("STATUS_CODE", RegistrationConstant.STATUS_ACTIVE);


            string eng_names = "";
            string chi_names = "";

            using (EntitiesRegistration db = new EntitiesRegistration())
            {

                using (DbConnection conn = db.Database.Connection)
                {

                    DbDataReader dr = CommonUtil.GetDataReader(conn, sql, queryParameters);


                    //List<Dictionary<string, object>> datas = new List<Dictionary<string, object>>();
                    int j = 0;
                    while (dr.Read())
                    {
                        string SURNAME = dr.GetValue(0).ToString();
                        string GIVEN_NAME_ON_ID = dr.GetValue(1).ToString();
                        string CHINESE_NAME = dr.GetValue(2).ToString();

                        if (j > 0) eng_names += " | ";
                        if (j > 0) chi_names += " | ";
                        if (string.IsNullOrEmpty(SURNAME) && string.IsNullOrEmpty(GIVEN_NAME_ON_ID))
                        {
                            if (string.IsNullOrEmpty(CHINESE_NAME))
                                eng_names += "";
                            else
                                eng_names += CHINESE_NAME;
                        }
                        else
                        {
                            eng_names += SURNAME + " " + GIVEN_NAME_ON_ID;
                        }
                        if (string.IsNullOrEmpty(CHINESE_NAME))
                        {
                            if (string.IsNullOrEmpty(SURNAME) && string.IsNullOrEmpty(GIVEN_NAME_ON_ID))
                                chi_names += "";
                            else
                                chi_names += SURNAME + " " + GIVEN_NAME_ON_ID;
                        }
                        else
                        {
                            chi_names += CHINESE_NAME;
                        }
                        j++;
                    }
                    conn.Close();
                }
            }


            return new string[] { eng_names, chi_names };

        }


        private String getQualificationDetails(List<C_IND_QUALIFICATION_DETAIL> bs)
        {
            String result = "";
            for (int i = 0; i < bs.Count; i++)
            {
                if ("".Equals(result))
                {
                    result = result + bs[i].C_S_CATEGORY_CODE_DETAIL.CODE;
                }
                else
                {
                    result = result + "," + bs[i].C_S_CATEGORY_CODE_DETAIL.CODE;
                }
            }
            return result;
        }

        private String getBSItem(List<C_S_SYSTEM_VALUE> bs)
        {
            String result = "";
            for (int i = 0; i < bs.Count; i++)
            {
                if ("".Equals(result))
                {
                    result = result + bs[i].CODE;
                }
                else
                {
                    result = result + "," + bs[i].CODE;
                }
            }
            return result;
        }

        public FileStreamResult exportIndApplicationData(CRMDataExportModel dataExport)
        {

            List<string> headerList = new List<string>();
            List<List<object>> data = new List<List<object>>();

            String[] exportCloumnCommon =
                    { "file_ref", "reg_no", "category",
                        "surname", "given_name", "ch_name", "expiry_dt"};
            String[] exportCloumnTelNo =
              { "tel_no1", "tel_no2", "tel_no3", "email", "fax_no1", "fax_no2"};
            String[] exportCloumnEmergency =
              { "emrg_no1", "emrg_no2", "emrg_no3"};
            String[] exportCloumnAddress =
              { "c_o", "pnrc", "addr1", "addr2", "addr3", "addr4", "addr5", "addr_cn1", "addr_cn2", "addr_cn3", "addr_cn4", "addr_cn5"};
            String[] exportCloumnBS =
              { "bs_addr1", "bs_addr2", "bs_addr3", "bs_addr4",
                        "bs_addr5", "bs_tel_no1", "bs_fax_no1", "bs_codes"};
            String[] exportCloumnPRB =
              { "prb_code", "prb_reg_no", "prb_expiry_dt", "prb_ctr_code", "prb_details"};


            bool isTelNo = dataExport.exportOfficeInfo;
            bool isEmergency = dataExport.exportEmergencyNo;
            bool isAddress = dataExport.exportCorrAddress;
            bool isBS = dataExport.exportBuildingSafety;
            bool isPRB = dataExport.exportPRBQualification;


            List<string> selectedCatList = null;


            if (dataExport.SelectedCategoryList != null && dataExport.SelectedCategoryList.Count > 0)
            {
                selectedCatList = new List<string>();
                for (int i = 0; i < dataExport.SelectedCategoryList.Count; i++)
                {
                    selectedCatList.Add(dataExport.SelectedCategoryList[i]);
                    if (dataExport.SelectedCategoryList[i].Equals(RegistrationConstant.S_CATEGORY_CODE_AP_A))
                    {
                        selectedCatList.Add(RegistrationConstant.S_CATEGORY_CODE_AP_I);
                    }
                    if (dataExport.SelectedCategoryList[i].Equals(RegistrationConstant.S_CATEGORY_CODE_AP_E))
                    {
                        selectedCatList.Add(RegistrationConstant.S_CATEGORY_CODE_AP_II);
                    }
                    if (dataExport.SelectedCategoryList[i].Equals(RegistrationConstant.S_CATEGORY_CODE_AP_S))
                    {
                        selectedCatList.Add(RegistrationConstant.S_CATEGORY_CODE_AP_III);
                    }
                }
            }


            headerList.AddRange(exportCloumnCommon);

            if (isTelNo)
            {
                headerList.AddRange(exportCloumnTelNo);
            }
            if (isEmergency)
            {
                headerList.AddRange(exportCloumnEmergency);
            }

            if (isAddress)
            {
                headerList.AddRange(exportCloumnAddress);
            }

            if (isBS)
            {
                headerList.AddRange(exportCloumnBS);
            }

            if (isPRB)
            {
                headerList.AddRange(exportCloumnPRB);
            }


            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    String sql =
                        " SELECT  " +
                        " A.UUID AS IND_APPLICATION_UUID, " +
                        " A.FILE_REFERENCE_NO, " +
                        " C.CERTIFICATION_NO, " +
                        " CAT_CODE.CODE AS CAT_CODE, " +
                        " APP.SURNAME, " +
                        " APP.GIVEN_NAME_ON_ID, " +
                        " APP.CHINESE_NAME, " +
                        " C.EXPIRY_DATE, " +
                        " A.TELEPHONE_NO1, " +
                        " A.TELEPHONE_NO2, " +
                        " A.TELEPHONE_NO3, " +
                        " A.EMAIL, " +
                        " A.FAX_NO1, " +
                        " A.FAX_NO2, " +
                        " A.EMERGENCY_NO1, " +
                        " A.EMERGENCY_NO2, " +
                        " A.EMERGENCY_NO3, " +
                        " A.POST_TO, " +
                        " A.ENGLISH_CARE_OF, " +
                        " PNR.ENGLISH_DESCRIPTION AS PNR_DESCRIPTION, " +
                        " E_ADD.ADDRESS_LINE1 AS H_E_ADDRESS_LINE1, " +
                        " E_ADD.ADDRESS_LINE2 AS H_E_ADDRESS_LINE2, " +
                        " E_ADD.ADDRESS_LINE3 AS H_E_ADDRESS_LINE3, " +
                        " E_ADD.ADDRESS_LINE4 AS H_E_ADDRESS_LINE4, " +
                        " E_ADD.ADDRESS_LINE5 AS H_E_ADDRESS_LINE5, " +
                        " C_ADD.ADDRESS_LINE1 AS H_C_ADDRESS_LINE1, " +
                        " C_ADD.ADDRESS_LINE2 AS H_C_ADDRESS_LINE2, " +
                        " C_ADD.ADDRESS_LINE3 AS H_C_ADDRESS_LINE3, " +
                        " C_ADD.ADDRESS_LINE4 AS H_C_ADDRESS_LINE4, " +
                        " C_ADD.ADDRESS_LINE5 AS H_C_ADDRESS_LINE5, " +
                        " E_O_ADD.ADDRESS_LINE1 AS O_E_ADDRESS_LINE1, " +
                        " E_O_ADD.ADDRESS_LINE2 AS O_E_ADDRESS_LINE2, " +
                        " E_O_ADD.ADDRESS_LINE3 AS O_E_ADDRESS_LINE3, " +
                        " E_O_ADD.ADDRESS_LINE4 AS O_E_ADDRESS_LINE4, " +
                        " E_O_ADD.ADDRESS_LINE5 AS O_E_ADDRESS_LINE5, " +
                        " C_O_ADD.ADDRESS_LINE1 AS O_C_ADDRESS_LINE1, " +
                        " C_O_ADD.ADDRESS_LINE2 AS O_C_ADDRESS_LINE2, " +
                        " C_O_ADD.ADDRESS_LINE3 AS O_C_ADDRESS_LINE3, " +
                        " C_O_ADD.ADDRESS_LINE4 AS O_C_ADDRESS_LINE4, " +
                        " C_O_ADD.ADDRESS_LINE5 AS O_C_ADDRESS_LINE5, " +
                        " BS_ADD.ADDRESS_LINE1 AS BS_ADDRESS_LINE1, " +
                        " BS_ADD.ADDRESS_LINE2 AS BS_ADDRESS_LINE2, " +
                        " BS_ADD.ADDRESS_LINE3 AS BS_ADDRESS_LINE3, " +
                        " BS_ADD.ADDRESS_LINE4 AS BS_ADDRESS_LINE4, " +
                        " BS_ADD.ADDRESS_LINE5 AS BS_ADDRESS_LINE5, " +
                        " A.BS_TELEPHONE_NO1, " +
                        " A.BS_FAX_NO1, " +
                        " A.UUID AS BS_KEY_UUID, " +
                        " Q_C.CODE AS Q_CODE, " +
                        " Q.REGISTRATION_NUMBER AS Q_REG_NO, " +
                        " Q.EXPIRY_DATE AS Q_EXPIRY_DATE, " +
                        " Q_CAT.CODE AS Q_CAT_CODE, " +
                        " Q.UUID AS Q_KEY_UUID " +
                        " FROM C_IND_APPLICATION A " +
                        " INNER JOIN C_IND_CERTIFICATE C ON A.UUID=C.MASTER_ID " +
                        " LEFT OUTER JOIN C_APPLICANT APP ON A.APPLICANT_ID = APP.UUID " +
                        " LEFT OUTER JOIN C_S_CATEGORY_CODE CAT_CODE ON C.CATEGORY_ID = CAT_CODE.UUID " +
                        " LEFT OUTER JOIN C_IND_QUALIFICATION Q ON Q.MASTER_ID = A.UUID " +
                        " LEFT OUTER JOIN C_S_SYSTEM_VALUE Q_C ON Q.PRB_ID = Q_C.UUID " +
                        " LEFT OUTER JOIN C_S_CATEGORY_CODE Q_CAT ON Q.CATEGORY_ID=Q_CAT.UUID " +
                        " LEFT OUTER JOIN C_S_SYSTEM_VALUE PNR ON  A.PRACTICE_NOTES_ID = PNR.UUID  " +
                        " LEFT OUTER JOIN C_ADDRESS E_ADD ON A.ENGLISH_HOME_ADDRESS_ID =  E_ADD.UUID " +
                        " LEFT OUTER JOIN C_ADDRESS C_ADD ON A.CHINESE_HOME_ADDRESS_ID =  C_ADD.UUID " +
                        " LEFT OUTER JOIN C_ADDRESS E_O_ADD ON A.ENGLISH_OFFICE_ADDRESS_ID =  E_O_ADD.UUID " +
                        " LEFT OUTER JOIN C_ADDRESS C_O_ADD ON A.CHINESE_OFFICE_ADDRESS_ID =  C_O_ADD.UUID " +
                        " LEFT OUTER JOIN C_ADDRESS BS_ADD ON A.ENGLISH_BS_ADDRESS_ID =  BS_ADD.UUID " +
                        " WHERE C.CERTIFICATION_NO IS NOT NULL " +
                        " AND C.EXPIRY_DATE IS NOT NULL " +
                        " AND C.EXPIRY_DATE > CURRENT_DATE " +
                        " AND ((C.REMOVAL_DATE IS NULL) OR (C.REMOVAL_DATE > CURRENT_DATE)) ";

                    Dictionary<string, object> queryParameters = new Dictionary<string, object>();
                    String whereQ = " and A.REGISTRATION_TYPE = :RegType";
                    queryParameters.Add("RegType", dataExport.RegType);
                    if (selectedCatList != null)
                    {
                        whereQ += " and CAT_CODE.CODE IN ( :categoryUUID ) ";
                        queryParameters.Add("categoryUUID", selectedCatList);
                    }

                    if (!string.IsNullOrWhiteSpace(dataExport.PNRC))
                    {
                        whereQ += " and  A.PRACTICE_NOTES_ID  = :PRACTICE_NOTES_ID ";
                        queryParameters.Add("PRACTICE_NOTES_ID", dataExport.PNRC);
                    }
                    DbDataReader dr = CommonUtil.GetDataReader(conn, sql + whereQ + " ORDER  BY A.FILE_REFERENCE_NO", queryParameters);

                    List<object> rowData = new List<object>();
                    //List<Dictionary<string, object>> datas = new List<Dictionary<string, object>>();
                    while (dr.Read())
                    {

                        rowData = new List<object>();
                        //String eachLine = "";
                        int i = 0;
                        String IND_APPLICATION_UUID = dr.GetValue(i++).ToString();

                        // { "file_ref", "reg_no", "category", "surname", "given_name", "ch_name", "expiry_dt"};

                        rowData.Add(dr.GetValue(i++).ToString());
                        rowData.Add(dr.GetValue(i++).ToString());
                        rowData.Add(dr.GetValue(i++).ToString());
                        rowData.Add(dr.GetValue(i++).ToString());
                        rowData.Add(dr.GetValue(i++).ToString());
                        rowData.Add(dr.GetValue(i++).ToString());
                        object expiryDt = dr.GetValue(i++);
                        if (string.IsNullOrEmpty(expiryDt.ToString()))
                        {
                            rowData.Add("");
                        }
                        else
                        {
                            rowData.Add(((DateTime)expiryDt).ToString(ApplicationConstant.DISPLAY_DATE_FORMAT));
                        }
                        //  { "tel_no1", "tel_no2", "tel_no3", "email", "fax_no1", "fax_no2"};

                        if (isTelNo)
                        {
                            for (int a = 0; a < 6; a++)
                            {
                                rowData.Add(dr.GetValue(i++).ToString());
                            }
                        }
                        else
                        {
                            i = i + 6;

                        }

                        //{ "emrg_no1", "emrg_no2", "emrg_no3"};
                        if (isEmergency)
                        {
                            for (int a = 0; a < 3; a++)
                            {
                                rowData.Add(dr.GetValue(i++).ToString());
                            }
                        }
                        else
                        {
                            i = i + 3;

                        }
                        //{ "c_o", "pnrc", "addr1", "addr2", "addr3", "addr4", "addr5", "addr_cn1", "addr_cn2", "addr_cn3", "addr_cn4", "addr_cn5"};
                        //{ "bs_addr1", "bs_addr2", "bs_addr3", "bs_addr4", "bs_addr5", "bs_tel_no1", "bs_fax_no1", "bs_codes"};
                        //{ "prb_code", "prb_reg_no", "prb_expiry_dt", "prb_ctr_code", "prb_details"};

                        if (isAddress)
                        {
                            String POST_TO = dr.GetValue(i++).ToString();
                            rowData.Add(dr.GetValue(i++).ToString());
                            rowData.Add(dr.GetValue(i++).ToString());
                            if ("H".Equals(POST_TO))
                            {
                            }
                            else
                            {
                                i = i + 10;
                            }
                            for (int a = 0; a < 10; a++)
                            {
                                rowData.Add(dr.GetValue(i++).ToString());
                            }
                            if ("H".Equals(POST_TO))
                            {
                                i = i + 10;
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            i = i + 23;
                        }

                        if (isBS)
                        {
                            for (int a = 0; a < 7; a++)
                            {
                                rowData.Add(dr.GetValue(i++).ToString());
                            }

                            String BS_KEY_UUID = dr.GetValue(i++).ToString();
                            // DbDataReader dr = CommonUtil.GetDataReader(conn, sql + whereQ, queryParameters);
                            List<C_S_SYSTEM_VALUE> bs = (from t1 in db.C_S_SYSTEM_VALUE
                                                         join t2 in db.C_BUILDING_SAFETY_INFO on t1 equals t2.C_S_SYSTEM_VALUE
                                                         where (t2.MASTER_ID == BS_KEY_UUID) && (t2.REGISTRATION_TYPE == dataExport.RegType)
                                                         select t1).ToList();
                            rowData.Add(this.getBSItem(bs));
                            //rowData.Add(dr.GetValue(i++).ToString());
                        }
                        else
                        {
                            i = i + 8;

                        }

                        if (isPRB)
                        {
                            for (int a = 0; a < 4; a++)
                            {
                                rowData.Add(dr.GetValue(i++).ToString());
                            }

                            String Q_KEY_UUID = dr.GetValue(i++).ToString();
                            List<C_IND_QUALIFICATION_DETAIL> qualifcationDetails =
                                (from t1 in db.C_IND_QUALIFICATION_DETAIL
                                 where (t1.IND_QUALIFICATION_ID == Q_KEY_UUID)
                                 select t1).Include(o => o.C_S_CATEGORY_CODE_DETAIL).ToList();
                            rowData.Add(this.getQualificationDetails(qualifcationDetails));
                        }
                        else
                        {
                            i = i + 5;

                        }

                        data.Add(rowData);

                        //printWriter.println();
                    }

                    conn.Close();
                }
            }
            return this.exportExcelFile("registration", headerList, data);
        }


        public FileStreamResult exportCompApplicationData(CRMDataExportModel dataExport)
        {

            List<string> headerList = new List<string>();
            List<List<object>> data = new List<List<object>>();

            String[] exportCloumnCommon =
                  { "file_ref", "reg_no", "category", "en_name", "ch_name", "expiry_dt"};

            String[] exportCloumnAddress =
              { "c_o", "addr1", "addr2", "addr3", "addr4", "addr5", "addr_cn1", "addr_cn2", "addr_cn3", "addr_cn4", "addr_cn5", "pnrc"};

            String[] exportCloumnOfficeOne =
              { "tel_no1", "tel_no2", "tel_no3", "email"};

            String[] exportCloumnBS =
              { "bs_addr1", "bs_addr2", "bs_addr3", "bs_addr4",
                        "bs_addr5", "bs_tel_no1", "bs_fax_no1", "bs_codes"};
            String[] exportCloumnOfficeTwo =
              { "fax_no1", "fax_no2", "AS", "AS_chn", "TD", "TD_chn"};


            headerList.AddRange(exportCloumnCommon);

            bool isOfficeOne = dataExport.exportOfficeInfo;
            bool isOfficeTwo = dataExport.exportOfficeInfo;
            bool isAddress = dataExport.exportCorrAddress;
            bool isBS = dataExport.exportBuildingSafety;


            if (isAddress)
            {
                headerList.AddRange(exportCloumnAddress);
            }

            if (isOfficeOne)
            {
                headerList.AddRange(exportCloumnOfficeOne);
            }

            if (isBS)
            {
                headerList.AddRange(exportCloumnBS);
            }

            if (isOfficeTwo)
            {
                headerList.AddRange(exportCloumnOfficeTwo);
            }

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {

                    String sql = "SELECT " +
                               " distinct " +
                               "N.UUID," +
                               "N.FILE_REFERENCE_NO," +
                               "N.CERTIFICATION_NO," +
                               "CAT.ENGLISH_DESCRIPTION AS CATEGORY," +
                               "N.ENGLISH_COMPANY_NAME," +
                               "N.CHINESE_COMPANY_NAME," +
                               "N.EXPIRY_DATE," +
                               "N.ENGLISH_CARE_OF," +
                               "E_ADD.ADDRESS_LINE1," +
                               "E_ADD.ADDRESS_LINE2," +
                               "E_ADD.ADDRESS_LINE3," +
                               "E_ADD.ADDRESS_LINE4," +
                               "E_ADD.ADDRESS_LINE5," +
                               "C_ADD.ADDRESS_LINE1 AS C_ADDRESS_LINE1," +
                               "C_ADD.ADDRESS_LINE2 AS C_ADDRESS_LINE2," +
                               "C_ADD.ADDRESS_LINE3 AS C_ADDRESS_LINE3," +
                               "C_ADD.ADDRESS_LINE4 AS C_ADDRESS_LINE4," +
                               "C_ADD.ADDRESS_LINE5 AS C_ADDRESS_LINE5," +
                               "C_PRAC.ENGLISH_DESCRIPTION AS PRAC_DESCRIPTION," +
                               "N.TELEPHONE_NO1," +
                               "N.TELEPHONE_NO2," +
                               "N.TELEPHONE_NO3," +
                               "N.EMAIL_ADDRESS, " +
                               "E_BS_ADD.ADDRESS_LINE1 AS BS_E_ADDRESS_LINE1," +
                               "E_BS_ADD.ADDRESS_LINE2 AS BS_E_ADDRESS_LINE2," +
                               "E_BS_ADD.ADDRESS_LINE3 AS BS_E_ADDRESS_LINE3," +
                               "E_BS_ADD.ADDRESS_LINE4 AS BS_E_ADDRESS_LINE4," +
                               "E_BS_ADD.ADDRESS_LINE5 AS BS_E_ADDRESS_LINE5," +
                               "N.BS_TELEPHONE_NO1," +
                               "N.BS_FAX_NO1," +
                               "N.FAX_NO1," +
                               "N.FAX_NO2 " +
                               "FROM C_COMP_APPLICATION N " +
                               "INNER JOIN C_COMP_APPLICANT_INFO I ON N.UUID = I.MASTER_ID " +
                               "LEFT JOIN C_S_CATEGORY_CODE CAT ON CAT.UUID = N.CATEGORY_ID " +
                               "LEFT JOIN C_ADDRESS C_ADD ON C_ADD.UUID = N.CHINESE_ADDRESS_ID " +
                               "LEFT JOIN C_ADDRESS E_ADD ON E_ADD.UUID = N.ENGLISH_ADDRESS_ID " +
                               "LEFT JOIN C_ADDRESS C_BS_ADD ON C_BS_ADD.UUID = N.CHINESE_BS_ADDRESS_ID " +
                               "LEFT JOIN C_ADDRESS E_BS_ADD ON E_BS_ADD.UUID = N.ENGLISH_BS_ADDRESS_ID " +
                               "LEFT JOIN C_S_SYSTEM_VALUE C_PRAC ON N.PRACTICE_NOTES_ID = C_PRAC.UUID ";

                    Dictionary<string, object> queryParameters = new Dictionary<string, object>();
                    String whereQ = " where 1 =1 and N.REGISTRATION_TYPE = :RegType";
                    queryParameters.Add("RegType", dataExport.RegType);

                    if (!string.IsNullOrWhiteSpace(dataExport.Category))
                    {
                        whereQ += " and N.CATEGORY_ID = :categoryUUID ";
                        queryParameters.Add("categoryUUID", dataExport.Category);
                    }

                    if (!string.IsNullOrWhiteSpace(dataExport.ApplicantRole))
                    {
                        whereQ += " and I.APPLICANT_ROLE_ID = :APPLICANT_ROLE_ID ";
                        queryParameters.Add("APPLICANT_ROLE_ID", dataExport.ApplicantRole);
                    }

                    if (!string.IsNullOrWhiteSpace(dataExport.PNRC))
                    {
                        whereQ += " and N.PRACTICE_NOTES_ID = :PRACTICE_NOTES_ID ";
                        queryParameters.Add("PRACTICE_NOTES_ID", dataExport.PNRC);
                    }
                    DbDataReader dr = CommonUtil.GetDataReader(conn, sql + whereQ, queryParameters);

                    List<object> rowData = new List<object>();
                    //List<Dictionary<string, object>> datas = new List<Dictionary<string, object>>();
                    while (dr.Read())
                    {
                        rowData = new List<object>();
                        //String eachLine = "";
                        int i = 0;
                        String C_COMP_APPLICATION_UUID = dr.GetValue(i++).ToString();
                        // { "file_ref", "reg_no", "category", "en_name", "ch_name", "expiry_dt"};
                        rowData.Add(dr.GetValue(i++).ToString());
                        rowData.Add(dr.GetValue(i++).ToString());
                        rowData.Add(dr.GetValue(i++).ToString());
                        rowData.Add(dr.GetValue(i++).ToString());
                        rowData.Add(dr.GetValue(i++).ToString());
                        object expiryDt = dr.GetValue(i++);
                        if (string.IsNullOrEmpty(expiryDt.ToString()))
                        {
                            rowData.Add("");
                        }
                        else
                        {
                            String expiryStr = ((DateTime)expiryDt).ToString(ApplicationConstant.DISPLAY_DATE_FORMAT);
                            rowData.Add(expiryStr);
                        }
                        // { "c_o", "addr1", "addr2", "addr3", "addr4", "addr5", 
                        // "addr_cn1", "addr_cn2", "addr_cn3", "addr_cn4", "addr_cn5", "pnrc"};
                        if (isAddress)
                        {
                            for (int a = 0; a < 12; a++)
                            {
                                rowData.Add(dr.GetValue(i++).ToString());
                            }
                        }
                        else
                        {
                            i = i + 12;
                        }
                        //{ "tel_no1", "tel_no2", "tel_no3", "email"};
                        if (isOfficeOne)
                        {
                            for (int a = 0; a < 4; a++)
                            {
                                rowData.Add(dr.GetValue(i++).ToString());
                            }
                        }
                        else
                        {
                            i = i + 4;
                        }
                        //{ "bs_addr1", "bs_addr2", "bs_addr3", "bs_addr4",
                        //   "bs_addr5", "bs_tel_no1", "bs_fax_no1", "bs_codes"};
                        if (isBS)
                        {
                            for (int a = 0; a < 7; a++)
                            {
                                rowData.Add(dr.GetValue(i++).ToString());
                            }
                            List<C_S_SYSTEM_VALUE> bs = (from t1 in db.C_S_SYSTEM_VALUE
                                                         join t2 in db.C_BUILDING_SAFETY_INFO on t1 equals t2.C_S_SYSTEM_VALUE
                                                         where (t2.MASTER_ID == C_COMP_APPLICATION_UUID) && (t2.REGISTRATION_TYPE == dataExport.RegType)
                                                         select t1).ToList();
                            rowData.Add(this.getBSItem(bs));
                        }
                        else
                        {
                            i = i + 7;
                        }

                        if (isOfficeTwo)
                        {
                            rowData.Add(dr.GetValue(i++).ToString());
                            rowData.Add(dr.GetValue(i++).ToString());
                            string[] tdInfo = getApplicantNameForExport(db, C_COMP_APPLICATION_UUID, "AS");
                            rowData.Add(tdInfo[0]);
                            rowData.Add(tdInfo[1]);
                            tdInfo = getApplicantNameForExport(db, C_COMP_APPLICATION_UUID, "TD");
                            rowData.Add(tdInfo[0]);
                            rowData.Add(tdInfo[1]);
                        }
                        else
                        {
                            i = i + 6;
                        }

                        data.Add(rowData);

                    }

                    conn.Close();
                }
            }
            return this.exportExcelFile("registration", headerList, data);
        }

        public FileStreamResult exportApplicationData_OLD(CRMDataExportModel dataExport)
        {

            List<string> headerList = new List<string>();
            List<List<object>> data = new List<List<object>>();


            String[] exportCloumnCommon =
                  { "file_ref", "reg_no", "category", "en_name", "ch_name", "expiry_dt"};

            String[] exportCloumnAddress =
              { "c_o", "addr1", "addr2", "addr3", "addr4", "addr5", "addr_cn1", "addr_cn2", "addr_cn3", "addr_cn4", "addr_cn5", "pnrc"};

            String[] exportCloumnOfficeOne =
              { "tel_no1", "tel_no2", "tel_no3", "email"};

            String[] exportCloumnBS =
              { "bs_addr1", "bs_addr2", "bs_addr3", "bs_addr4",
                        "bs_addr5", "bs_tel_no1", "bs_fax_no1", "bs_codes"};
            String[] exportCloumnOfficeTwo =
              { "fax_no1", "fax_no2", "AS", "AS_chn", "TD", "TD_chn"};

            var sb = new StringBuilder();

            String headerLine = "";

            headerList.AddRange(exportCloumnCommon);


            for (int i = 0; i < exportCloumnCommon.Length; i++)
            {
                headerLine = appendCertContent(headerLine, exportCloumnCommon[i]);
            }

            bool isOfficeOne = dataExport.exportOfficeInfo;
            bool isOfficeTwo = dataExport.exportOfficeInfo;
            bool isAddress = dataExport.exportCorrAddress;
            bool isBS = dataExport.exportBuildingSafety;


            if (isAddress)
            {
                for (int i = 0; i < exportCloumnAddress.Length; i++)
                {
                    headerLine = appendCertContent(headerLine, exportCloumnAddress[i]);
                }
            }

            if (isOfficeOne)
            {
                for (int i = 0; i < exportCloumnOfficeOne.Length; i++)
                {
                    headerLine = appendCertContent(headerLine, exportCloumnOfficeOne[i]);
                }
            }

            if (isBS)
            {
                for (int i = 0; i < exportCloumnBS.Length; i++)
                {
                    headerLine = appendCertContent(headerLine, exportCloumnBS[i]);
                }
            }

            if (isOfficeTwo)
            {
                for (int i = 0; i < exportCloumnOfficeTwo.Length; i++)
                {
                    headerLine = appendCertContent(headerLine, exportCloumnOfficeTwo[i]);
                }
            }

            headerLine += Environment.NewLine;
            sb.Append(headerLine);

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {

                    String sql = "SELECT " +
                               " distinct " +
                               "N.UUID," +
                               "N.FILE_REFERENCE_NO," +
                               "N.CERTIFICATION_NO," +
                               "CAT.ENGLISH_DESCRIPTION AS CATEGORY," +
                               "N.ENGLISH_COMPANY_NAME," +
                               "N.CHINESE_COMPANY_NAME," +
                               "N.EXPIRY_DATE," +
                               "N.ENGLISH_CARE_OF," +
                               "E_ADD.ADDRESS_LINE1," +
                               "E_ADD.ADDRESS_LINE2," +
                               "E_ADD.ADDRESS_LINE3," +
                               "E_ADD.ADDRESS_LINE4," +
                               "E_ADD.ADDRESS_LINE5," +
                               "C_ADD.ADDRESS_LINE1 AS C_ADDRESS_LINE1," +
                               "C_ADD.ADDRESS_LINE2 AS C_ADDRESS_LINE2," +
                               "C_ADD.ADDRESS_LINE3 AS C_ADDRESS_LINE3," +
                               "C_ADD.ADDRESS_LINE4 AS C_ADDRESS_LINE4," +
                               "C_ADD.ADDRESS_LINE5 AS C_ADDRESS_LINE5," +
                               "C_PRAC.ENGLISH_DESCRIPTION AS PRAC_DESCRIPTION," +
                               "N.TELEPHONE_NO1," +
                               "N.TELEPHONE_NO2," +
                               "N.TELEPHONE_NO3," +
                               "N.EMAIL_ADDRESS, " +
                               "E_BS_ADD.ADDRESS_LINE1 AS BS_E_ADDRESS_LINE1," +
                               "E_BS_ADD.ADDRESS_LINE2 AS BS_E_ADDRESS_LINE2," +
                               "E_BS_ADD.ADDRESS_LINE3 AS BS_E_ADDRESS_LINE3," +
                               "E_BS_ADD.ADDRESS_LINE4 AS BS_E_ADDRESS_LINE4," +
                               "E_BS_ADD.ADDRESS_LINE5 AS BS_E_ADDRESS_LINE5," +
                               "N.BS_TELEPHONE_NO1," +
                               "N.BS_FAX_NO1," +
                               "N.FAX_NO1," +
                               "N.FAX_NO2 " +
                               "FROM C_COMP_APPLICATION N " +
                               "INNER JOIN C_COMP_APPLICANT_INFO I ON N.UUID = I.MASTER_ID " +
                               "LEFT JOIN C_S_CATEGORY_CODE CAT ON CAT.UUID = N.CATEGORY_ID " +
                               "LEFT JOIN C_ADDRESS C_ADD ON C_ADD.UUID = N.CHINESE_ADDRESS_ID " +
                               "LEFT JOIN C_ADDRESS E_ADD ON E_ADD.UUID = N.ENGLISH_ADDRESS_ID " +
                               "LEFT JOIN C_ADDRESS C_BS_ADD ON C_BS_ADD.UUID = N.CHINESE_BS_ADDRESS_ID " +
                               "LEFT JOIN C_ADDRESS E_BS_ADD ON E_BS_ADD.UUID = N.ENGLISH_BS_ADDRESS_ID " +
                               "LEFT JOIN C_S_SYSTEM_VALUE C_PRAC ON N.PRACTICE_NOTES_ID = C_PRAC.UUID ";

                    Dictionary<string, object> queryParameters = new Dictionary<string, object>();
                    String whereQ = " where 1 =1 and N.REGISTRATION_TYPE = :RegType";
                    queryParameters.Add("RegType", dataExport.RegType);

                    if (!string.IsNullOrWhiteSpace(dataExport.Category))
                    {
                        whereQ += " and N.CATEGORY_ID = :categoryUUID ";
                        queryParameters.Add("categoryUUID", dataExport.Category);
                    }

                    if (!string.IsNullOrWhiteSpace(dataExport.ApplicantRole))
                    {
                        whereQ += " and I.APPLICANT_ROLE_ID = :APPLICANT_ROLE_ID ";
                        queryParameters.Add("APPLICANT_ROLE_ID", dataExport.ApplicantRole);
                    }

                    if (!string.IsNullOrWhiteSpace(dataExport.PNRC))
                    {
                        whereQ += " and N.PRACTICE_NOTES_ID = :PRACTICE_NOTES_ID ";
                        queryParameters.Add("PRACTICE_NOTES_ID", dataExport.PNRC);
                    }
                    DbDataReader dr = CommonUtil.GetDataReader(conn, sql + whereQ, queryParameters);

                    //List<Dictionary<string, object>> datas = new List<Dictionary<string, object>>();
                    while (dr.Read())
                    {
                        String eachLine = "";
                        int i = 0;
                        String C_COMP_APPLICATION_UUID = dr.GetValue(i++).ToString();
                        // { "file_ref", "reg_no", "category", "en_name", "ch_name", "expiry_dt"};
                        eachLine = appendDoubleQuote(dr.GetValue(i++).ToString());
                        eachLine = addValueLine(eachLine, dr.GetValue(i++).ToString());
                        eachLine = addValueLine(eachLine, dr.GetValue(i++).ToString());
                        eachLine = addValueLine(eachLine, dr.GetValue(i++).ToString());
                        eachLine = addValueLine(eachLine, dr.GetValue(i++).ToString());

                        object expiryDt = dr.GetValue(i++);

                        if (string.IsNullOrEmpty(expiryDt.ToString()))
                        {
                            eachLine = addValueLine(eachLine, "");

                        }
                        else
                        {
                            eachLine = addValueLine(eachLine,
                          ((DateTime)expiryDt).ToString(ApplicationConstant.DISPLAY_DATE_FORMAT));

                        }
                        // { "c_o", "addr1", "addr2", "addr3", "addr4", "addr5", 
                        // "addr_cn1", "addr_cn2", "addr_cn3", "addr_cn4", "addr_cn5", "pnrc"};
                        if (isAddress)
                        {
                            for (int a = 0; a < 12; a++)
                            {
                                eachLine = addValueLine(eachLine, dr.GetValue(i++).ToString());
                            }
                        }
                        //{ "tel_no1", "tel_no2", "tel_no3", "email"};
                        if (isOfficeOne)
                        {
                            for (int a = 0; a < 4; a++)
                            {
                                eachLine = addValueLine(eachLine, dr.GetValue(i++).ToString());
                            }
                        }
                        //{ "bs_addr1", "bs_addr2", "bs_addr3", "bs_addr4",
                        //   "bs_addr5", "bs_tel_no1", "bs_fax_no1", "bs_codes"};

                        if (isBS)
                        {
                            for (int a = 0; a < 7; a++)
                            {
                                eachLine = addValueLine(eachLine, dr.GetValue(i++).ToString());
                            }
                        }
                        // DbDataReader dr = CommonUtil.GetDataReader(conn, sql + whereQ, queryParameters);
                        List<C_S_SYSTEM_VALUE> bs = (from t1 in db.C_S_SYSTEM_VALUE
                                                     join t2 in db.C_BUILDING_SAFETY_INFO on t1 equals t2.C_S_SYSTEM_VALUE
                                                     where (t2.MASTER_ID == C_COMP_APPLICATION_UUID) && (t2.REGISTRATION_TYPE == dataExport.RegType)
                                                     select t1).ToList();
                        eachLine = addValueLine(eachLine, this.getBSItem(bs));

                        if (isOfficeTwo)
                        {
                            eachLine = addValueLine(eachLine, dr.GetValue(i++).ToString());
                            eachLine = addValueLine(eachLine, dr.GetValue(i++).ToString());


                            string[] tdInfo = getApplicantNameForExport(db, C_COMP_APPLICATION_UUID,
                                "AS");
                            eachLine = addValueLine(eachLine, tdInfo[0]);
                            eachLine = addValueLine(eachLine, tdInfo[1]);

                            tdInfo = getApplicantNameForExport(db, C_COMP_APPLICATION_UUID,
                               "TD");
                            eachLine = addValueLine(eachLine, tdInfo[0]);
                            eachLine = addValueLine(eachLine, tdInfo[1]);
                        }


                        eachLine += Environment.NewLine;
                        sb.Append(eachLine);
                    }

                    conn.Close();
                }
            }




            var byteArray = Encoding.UTF8.GetBytes(sb.ToString());
            var stream = new MemoryStream(byteArray);
            var mimeType = "text/plain";
            FileStreamResult result = new FileStreamResult(stream, mimeType);
            result.FileDownloadName = "registration.csv";
            return result;


        }

        public FileStreamResult exportQPExcelData()
        {
            List<string> headerList = new List<string>() {

                "ENGLISH_NAME", "CHINESE_NAME", "ENGLISH_COMPANY_NAME", "CHINESE_COMPANY_NAME",
                "REGISTRATION_NUMBER", "REGISTRATION_TYPE", "DISTRICT_ENGLISH", "DISTRICT_CHINESE",
                "ITEMS", "FLAG", "SERVICE_IN_BUILDING_SAFETY", "TELEPHONE_NO", "EMAIL",
                "FAX", "AS_NAME_ENGLISH", "AS_NAME_CHINESE", "COMPANY_TYPE_ONE", "COMPANY_TYPE_TWO",
                "COMPANY_TYPE_THREE", "AS_TYPE_ONE", "AS_TYPE_TWO", "AS_TYPE_THREE", "SEARCH_TYPE",
                "ITEM_KEY", "BS_KEY", "TYPE_KEY", "WILLINGNESS_QP", "INTERESTED_FSS", "MBIS_RI",
                "EXPIRY_DATE", "FLAG" };
            List<List<object>> data = null;

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    String sql = this.QP_SQL;
                    DbDataReader dr = CommonUtil.GetDataReader(conn, sql, null);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return this.exportExcelFile("QP_LIST", headerList, data);

        }
        public FileStreamResult exportQPCSVData()
        {
            List<string> headerList = new List<string>() {

                "ENGLISH_NAME", "CHINESE_NAME", "ENGLISH_COMPANY_NAME", "CHINESE_COMPANY_NAME",
                "REGISTRATION_NUMBER", "REGISTRATION_TYPE", "DISTRICT_ENGLISH", "DISTRICT_CHINESE",
                "ITEMS", "FLAG", "SERVICE_IN_BUILDING_SAFETY", "TELEPHONE_NO", "EMAIL",
                "FAX", "AS_NAME_ENGLISH", "AS_NAME_CHINESE", "COMPANY_TYPE_ONE", "COMPANY_TYPE_TWO",
                "COMPANY_TYPE_THREE", "AS_TYPE_ONE", "AS_TYPE_TWO", "AS_TYPE_THREE", "SEARCH_TYPE",
                "ITEM_KEY", "BS_KEY", "TYPE_KEY", "WILLINGNESS_QP", "INTERESTED_FSS", "MBIS_RI",
                "EXPIRY_DATE", "FLAG" };
            List<List<object>> data = null;

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    String sql = this.QP_SQL;
                    DbDataReader dr = CommonUtil.GetDataReader(conn, sql, null);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return this.exportCSVFile("QP_LIST", headerList, data);

        }

        public FileStreamResult ExportMRACancelMeeting(string uuid, string Meeting_number)
        {
            List<string> headerList = new List<string>() {

                "title", "given_name", "surname", "meeting_no",
                "meeting_year", "meeting_group", "interv_date", "fax_no1",
                "fax_no2", "secretary"
            };

            List<List<object>> data = getMRACancelMeetinga(uuid, Meeting_number);
            return this.exportCSVFile("CancellationMeeting", headerList, data);

        }
        private List<List<object>> getMRACancelMeetinga(string uuid, string Meeting_number)
        {
            string queryStr =
                "\r\n" + "\t" + " SELECT sv.ENGLISH_DESCRIPTION, ca.SURNAME, ca.given_name_on_id 		 " +
                "\r\n" + "\t" + " , cis.meeting_NUMBER, meeting.year, meeting.meeting_no                  " +
                "\r\n" + "\t" + " , cis.interview_date, ccm.FAX_NO1, ccm.fax_no2                          " +
                "\r\n" + "\t" + " ,                                                                       " +
                "\r\n" + "\t" + " (                                                                       " +
                "\r\n" + "\t" + " select (Appl.surname || ' ' || Appl.given_name_on_id)                   " +
                "\r\n" + "\t" + " from C_INTERVIEW_SCHEDULE inter                                         " +
                "\r\n" + "\t" + " inner join C_Meeting m on m.uuid = inter.MEETING_ID                     " +
                "\r\n" + "\t" + " inner join C_Meeting_Member mm on mm.MEETING_ID = m.uuid                " +
                "\r\n" + "\t" + " inner join C_COMMITTEE_MEMBER ccm on ccm.UUID = mm.MEMBER_ID            " +
                "\r\n" + "\t" + " inner join C_APPLICANT Appl on Appl.UUID = ccm.APPLICANT_ID             " +
                "\r\n" + "\t" + " inner join C_S_SYSTEM_VALUE sv2 on sv2.UUID = mm.committee_role_id      " +
                "\r\n" + "\t" + " where 1=1                                                               " +
                "\r\n" + "\t" + " and inter.uuid = :uuid                                                  " +
                "\r\n" + "\t" + " and sv2.code ='2'                                                       " +
                "\r\n" + "\t" + " ) as secretary                                                          " +
                "\r\n" + "\t" + " from C_MEETING_MEMBER mm                                                " +
                "\r\n" + "\t" + " inner join C_COMMITTEE_MEMBER ccm on ccm.UUID = mm.MEMBER_ID            " +
                "\r\n" + "\t" + " inner join C_APPLICANT ca on ca.UUID = ccm.APPLICANT_ID                 " +
                "\r\n" + "\t" + " inner join C_S_SYSTEM_VALUE sv on sv.UUID = ca.TITLE_ID                 " +
                "\r\n" + "\t" + " inner join C_INTERVIEW_SCHEDULE cis on cis.MEETING_ID = mm.MEETING_ID   " +
                "\r\n" + "\t" + " inner join C_MEETING meeting on meeting.UUID = cis.MEETING_ID           " +
                "\r\n" + "\t" + " where cis.meeting_number  = :Meeting_number                             ";

            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            queryParameters.Add("uuid", uuid);
            queryParameters.Add("Meeting_number", Meeting_number);

            List<List<object>> data = new List<List<object>>();
            using (EntitiesRegistration db = new EntitiesRegistration())
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


        public FileStreamResult ExportCRCMeeting(string uuid, string Meeting_number)
        {
            List<string> headerList = new List<string>() {

               "meeting_no", "meeting_year", "meeting_group", "interv_date", "title_1", "mbr_fullname_1",
               "title_2", "mbr_fullname_2", "title_3", "mbr_fullname_3", "title_4", "mbr_fullname_4" ,"title_5",
               "mbr_fullname_5", "title_6", "mbr_fullname_6", "title_7", "mbr_fullname_7", "title_8", "mbr_fullname_8",
               "title_9", "mbr_fullname_9", "title_10", "mbr_fullname_10", "title_11", "mbr_fullname_11", "title_12",
               "mbr_fullname_12", "title_13", "mbr_fullname_13", "title_14", "mbr_fullname_14", "title_15", "mbr_fullname_15",
               "title_16", "mbr_fullname_16", "title_17", "mbr_fullname_17", "title_18", "mbr_fullname_18", "title_19",
               "mbr_fullname_19", "title_20", "mbr_fullname_20", "secretary"
            };

            List<List<object>> data = getCRCMeeting(uuid, Meeting_number);
            return this.exportCSVFile("ExportCRCMeeting", headerList, data);

        }
        private List<List<object>> getCRCMeeting(string uuid, string Meeting_number)
        {
            string queryStr = "" +
                "\r\n" + "\t" + " SELECT                                                                                            " +
                "\r\n" + "\t" + " cis.meeting_NUMBER, meeting.year, meeting.meeting_group                                            " +
                "\r\n" + "\t" + " ,cis.interview_date, sv.ENGLISH_DESCRIPTION, (ca.Surname || ' ' || ca.given_name_on_id) as fullName" +
                "\r\n" + "\t" + " ,(                                                                                                 " +
                "\r\n" + "\t" + " select (Appl.surname || ' ' || Appl.given_name_on_id)                                              " +
                "\r\n" + "\t" + " from C_INTERVIEW_SCHEDULE inter                                                                    " +
                "\r\n" + "\t" + " inner join C_Meeting m on m.uuid = inter.MEETING_ID                                                " +
                "\r\n" + "\t" + " inner join C_Meeting_Member mm on mm.MEETING_ID = m.uuid                                           " +
                "\r\n" + "\t" + " inner join C_COMMITTEE_MEMBER ccm on ccm.UUID = mm.MEMBER_ID                                       " +
                "\r\n" + "\t" + " inner join C_APPLICANT Appl on Appl.UUID = ccm.APPLICANT_ID                                        " +
                "\r\n" + "\t" + " inner join C_S_SYSTEM_VALUE sv2 on sv2.UUID = mm.committee_role_id                                 " +
                "\r\n" + "\t" + " where 1=1                                                                                          " +
                "\r\n" + "\t" + " and inter.uuid = :uuid                                                                             " +
                "\r\n" + "\t" + " and sv2.code ='2'                                                                                  " +
                "\r\n" + "\t" + " ) as secretary                                                                                     " +
                "\r\n" + "\t" + " from C_MEETING_MEMBER mm                                                                           " +
                "\r\n" + "\t" + " inner join C_COMMITTEE_MEMBER ccm on ccm.UUID = mm.MEMBER_ID                                       " +
                "\r\n" + "\t" + " inner join C_APPLICANT ca on ca.UUID = ccm.APPLICANT_ID                                            " +
                "\r\n" + "\t" + " inner join C_S_SYSTEM_VALUE sv on sv.UUID = ca.TITLE_ID                                            " +
                "\r\n" + "\t" + " inner join C_INTERVIEW_SCHEDULE cis on cis.MEETING_ID = mm.MEETING_ID                              " +
                "\r\n" + "\t" + " inner join C_MEETING meeting on meeting.UUID = cis.MEETING_ID    									 " +
                "\r\n" + "\t" + " where cis.meeting_number  = :Meeting_number                                                        ";

            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            queryParameters.Add("uuid", uuid);
            queryParameters.Add("Meeting_number", Meeting_number);

            List<List<object>> data = new List<List<object>>();
            using (EntitiesRegistration db = new EntitiesRegistration())
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
        public FileStreamResult ExportCRCMeetingMBR(string uuid, string Meeting_number)
        {
            List<string> headerList = new List<string>() {
                "title","surname","given_name","company_name","addr1","addr2","addr3","addr4","addr5",
                "caddr1","caddr2","caddr3","caddr4","caddr5","interv_date","secretary"

            };

            List<List<object>> data = getCRCMeetingMBR(uuid, Meeting_number);
            return this.exportCSVFile("ExportCRCMeeting", headerList, data);
        }
        private List<List<object>> getCRCMeetingMBR(string uuid, string Meeting_number)
        {
            string queryStr = "" +
                "\r\n" + "\t" + " SELECT                                                                                                                       " +
                "\r\n" + "\t" + " sv.ENGLISH_DESCRIPTION, ca.SURNAME, ca.given_name_on_id , ccm.english_care_of                                                " +
                "\r\n" + "\t" + " , addr.address_line1, addr.address_line2, addr.address_line3, addr.address_line4, addr.address_line5                         " +
                "\r\n" + "\t" + " , addr2.address_line1, addr2.address_line2, addr2.address_line3, addr2.address_line4, addr2.address_line5, cis.interview_date" +
                "\r\n" + "\t" + " ,(                                                                                                                           " +
                "\r\n" + "\t" + "    select (Appl.surname || ' ' || Appl.given_name_on_id)                                                                     " +
                "\r\n" + "\t" + "    from C_INTERVIEW_SCHEDULE inter                                                                                           " +
                "\r\n" + "\t" + "    inner join C_Meeting m on m.uuid = inter.MEETING_ID                                                                       " +
                "\r\n" + "\t" + "    inner join C_Meeting_Member mm on mm.MEETING_ID = m.uuid                                                                  " +
                "\r\n" + "\t" + "    inner join C_COMMITTEE_MEMBER ccm on ccm.UUID = mm.MEMBER_ID                                                              " +
                "\r\n" + "\t" + "    inner join C_APPLICANT Appl on Appl.UUID = ccm.APPLICANT_ID                                                               " +
                "\r\n" + "\t" + "    inner join C_S_SYSTEM_VALUE sv2 on sv2.UUID = mm.committee_role_id                                                        " +
                "\r\n" + "\t" + "    where 1=1                                                                                                                 " +
                "\r\n" + "\t" + "    and inter.uuid = :UUID                                                                                                    " +
                "\r\n" + "\t" + "    and sv2.code ='2'                                                                                                         " +
                "\r\n" + "\t" + " ) as fullName                                                                                                                " +
                "\r\n" + "\t" + " from C_MEETING_MEMBER mm                                                                                                     " +
                "\r\n" + "\t" + " inner join C_COMMITTEE_MEMBER ccm on ccm.UUID = mm.MEMBER_ID                                                                 " +
                "\r\n" + "\t" + " inner join C_APPLICANT ca on ca.UUID = ccm.APPLICANT_ID                                                                      " +
                "\r\n" + "\t" + " inner join C_S_SYSTEM_VALUE sv on sv.UUID = ca.TITLE_ID                                                                      " +
                "\r\n" + "\t" + " inner join C_INTERVIEW_SCHEDULE cis on cis.MEETING_ID = mm.MEETING_ID                                                        " +
                "\r\n" + "\t" + " inner join C_MEETING meeting on meeting.UUID = cis.MEETING_ID                                                                " +
                "\r\n" + "\t" + " inner join C_ADDRESS addr on addr.UUID = ccm.english_address_id                                                              " +
                "\r\n" + "\t" + " inner join C_ADDRESS addr2 on addr2.UUID = ccm.chinese_address_id                                                            " +
                "\r\n" + "\t" + " where cis.meeting_number  = :Meeting_number                                                                                  ";

            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            queryParameters.Add("uuid", uuid);
            queryParameters.Add("Meeting_number", Meeting_number);

            List<List<object>> data = new List<List<object>>();
            using (EntitiesRegistration db = new EntitiesRegistration())
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
        public FileStreamResult ExportPnlInfoExportConvictionCover(string CompName)
        {
            List<string> headerList = new List<string>() {
                "name"
            };
            //object DataObject = model.InfoSheetCompanyName;
            List<List<object>> nameList = new List<List<object>>();
            List<object> nameInput;
            if (CompName == null)
            {
                nameInput = new List<object>() { " " };
            }
            else
            {
                nameInput = new List<object>() { CompName };
            }
            nameList.Add(nameInput);
            return this.exportCSVFile("ConvictionCover", "txt", headerList, nameList);

        }
        public FileStreamResult ExportIPConvictionCover(CRMReportModel model)
        {
            List<string> headerList = new List<string>() {

                "name"
            };
            //object DataObject = model.InfoSheetCompanyName;
            List<List<object>> nameList = new List<List<object>>();
            List<object> nameInput = new List<object>();
            if (model.InfoSurname == null && model.InfoGivenName != null)
            {
                nameInput.Add(" ");
                nameInput.Add(model.InfoGivenName);
            }
            else if (model.InfoSurname != null && model.InfoGivenName == null)
            {
                //nameInput.Add(" ");
                nameInput.Add(model.InfoSurname);
            }
            else
            {
                nameInput = new List<object>() { model.InfoSurname + " " + model.InfoGivenName };
            }
            nameList.Add(nameInput);
            return this.exportCSVFile("ConvictionCover", "txt", headerList, nameList);
        }
        public FileStreamResult ExportIndivGazettememoData(DateTime? dataOfGazette, string regType, string acting, string authId)
        {
            List<string> headerList = new List<string>() {
                "file_ref_1", "file_ref_2",
                "file_ref_3", "file_ref_4", "file_ref_5", "file_ref_6",
                "file_ref_7", "file_ref_8", "file_ref_9", "file_ref_10",
                "file_ref_11", "file_ref_12", "file_ref_13", "file_ref_14",
                "file_ref_15", "gazet_dt", "category", "auth_name",
                "auth_cname", "auth_rank", "auth_crank", "auth_tel",
                "auth_fax", "acting"
            };
            List<List<object>> data = new List<List<object>>();
            data = getIndivGazetteMemoData(dataOfGazette, regType, acting, authId);
            for (int i = 0; i < data.Count(); i++)
            {
                data[i][0] += appendDoubleQuote(dataOfGazette.ToString().Trim()) + "," + appendDoubleQuote(regType.ToString().Trim());
            }
            return this.exportCSVFile("IndivGazettememoData", "txt", headerList, data);

        }
        public FileStreamResult ExportCompGazettememoData(DateTime? dataOfGazette, string regType, string acting, string authId)
        {
            List<string> headerList = new List<string>() {
                "file_ref_1", "file_ref_2",
                "file_ref_3", "file_ref_4", "file_ref_5", "file_ref_6",
                "file_ref_7", "file_ref_8", "file_ref_9", "file_ref_10",
                "file_ref_11", "file_ref_12", "file_ref_13", "file_ref_14",
                "file_ref_15", "gazet_dt", "category", "auth_name",
                "auth_cname", "auth_rank", "auth_crank", "auth_tel",
                "auth_fax", "acting"
            };
            List<List<object>> data = new List<List<object>>();
            data = getCompGazetteMemoData(dataOfGazette, regType, acting, authId);
            for (int i = 0; i < data.Count(); i++)
            {
                data[i][0] += appendDoubleQuote(dataOfGazette.ToString().Trim()) + "," + appendDoubleQuote(regType.ToString().Trim());
            }
            return this.exportCSVFile("IndivGazettememoData", "txt", headerList, data);

        }
        public FileStreamResult ExportMwIndivWeeklyNotice(DateTime? dataOfGazette, string regType, string acting, string authId)
        {

            string DOUBLEQUOTE = "\"";
            string TAB = "\t";
            string SEPARATOR = ",";
            List<List<object>> data = new List<List<object>>();
            List<string> headerList = new List<string>();
            List<object> dataList = new List<object>();
            try
            {
                headerList = new List<string>() {
                        "file_ref_1",// "reg_no_1",
					    "title_1", "surname_1", "given_name_1", "given_name_id_1",
                        "cname_1", "cert_cname_1", "letter_file_ref_1", "reg_dt_1",
                        "cat_code_1", "expiry_dt", "item_line_1", "item_line_2",
                        "item_line_3", "item_line_4", "item_line_5", "item_line_6",
                        "egazet_dt", "cgazet_dt", "auth_name", "auth_cname",
                        "auth_rank", "auth_crank", "auth_tel", "auth_fax", "acting"
                };

                //for (int i = 0; i < headerList.Count(); i++)
                //{
                //    if (i != (headerList.Count() - 1))
                //        headerList.Add(headerList[i].ToString());
                //    else
                //        headerList.Add(headerList[i].ToString());

                //}
                List<List<object>> fileRefs = new List<List<object>>();
                List<List<object>> authInfo = new List<List<object>>();
                fileRefs = getMwIndivWeeklyNoticeFileRef(dataOfGazette, regType);
                authInfo = getMwIndivWeeklyNoticeAuth(acting, authId);
                for (int i = 0; i < fileRefs.Count(); i++)
                {
                    List<object> o = fileRefs[i];
                    for (int j = 0; j < o.Count() - 1; j++)
                    {
                        headerList.Add(o[j].ToString());
                    }
                    string indApp = o[o.Count() - 1].ToString();
                    C_IND_APPLICATION app = getIndApplicationByUUID(indApp);
                    List<C_IND_APPLICATION_MW_ITEM> indApplicationMwItemList = getIndApplicationMwItemByIndApplication(app);
                    List<string> mwItems = new List<string>();
                    if (indApplicationMwItemList != null
                            && indApplicationMwItemList.Count() > 0)
                    {
                        for (int mwi = 0; mwi < indApplicationMwItemList.Count(); mwi++)
                        {
                            C_IND_APPLICATION_MW_ITEM indApplicationMwItem = indApplicationMwItemList[mwi];
                            mwItems.Add(indApplicationMwItem.C_S_SYSTEM_VALUE.CODE.Substring(5));
                        }
                    }

                    int MAX_SIZE = 42;
                    int MAX_LINE = 6;
                    int MAX_ITEM_IN_LINE = 7;


                    // Add empty items
                    while (mwItems.Count() < MAX_SIZE)
                    {
                        mwItems.Add("");
                    }
                    // Print the line
                    for (int k = 0; k < MAX_LINE; k++)
                    {
                        int start = k * MAX_ITEM_IN_LINE;
                        string mwItems1 = mwItems[start];
                        string mwItems2 = mwItems[start + 1];
                        string mwItems3 = mwItems[start + 2];
                        string mwItems4 = mwItems[start + 3];
                        string mwItems5 = mwItems[start + 4];
                        string mwItems6 = mwItems[start + 5];
                        string mwItems7 = mwItems[start + 6];
                        dataList.Add(DOUBLEQUOTE);
                        dataList.Add(mwItems1 + TAB);
                        dataList.Add(mwItems2 + TAB);
                        dataList.Add(mwItems3 + TAB);
                        dataList.Add(mwItems4 + TAB);
                        dataList.Add(mwItems5 + TAB);
                        dataList.Add(mwItems6 + TAB);
                        dataList.Add(mwItems7);
                        dataList.Add(DOUBLEQUOTE + SEPARATOR);
                    }
                    headerList.Add(dataOfGazette.ToString());
                    if (authInfo.Count() > 0)
                    {
                        string authDetail = authInfo[0].ToString();
                        for (int j = 0; j < authDetail.Count(); j++)
                        {
                            if (j == authDetail.Count() - 1)
                                dataList.Add(authDetail[j].ToString());
                            else
                                dataList.Add(authDetail[j].ToString());

                        }
                    }
                    else
                    {
                        for (int l = 0; l < 7; l++)
                        {
                            if (l == 6)
                            {
                                dataList.Add(DOUBLEQUOTE + " " + DOUBLEQUOTE);
                            }
                            else
                                dataList.Add(DOUBLEQUOTE + " " + DOUBLEQUOTE
                                        + SEPARATOR);
                        }
                    }
                }
                // add for merging format
                int emptyElement = (fileRefs.Count() % 3 == 0) ? 0 : (3 - (fileRefs
                        .Count() % 3));
                for (int i = 0; i < emptyElement; i++)
                {
                    for (int j = 0; j < 17; j++)
                    {
                        dataList.Add(DOUBLEQUOTE + DOUBLEQUOTE + SEPARATOR);
                    }
                    headerList.Add(dataOfGazette.ToString());
                    //doPrint(printWriter, DateUtil.getChineseFormatDate(DateUtil
                    //        .getDisplayDateToDBDate(parameterMap.get("gaz_date")
                    //                .toString())));
                    if (authInfo.Count() > 0)
                    {
                        string authDetail = authInfo[0].ToString();
                        for (int j = 0; j < authDetail.Count(); j++)
                        {
                            if (j == authDetail.Count() - 1)
                                dataList.Add(authDetail[j].ToString());
                            else
                                dataList.Add(authDetail[j].ToString());

                        }
                    }
                    else
                    {
                        for (int l = 0; l < 7; l++)
                        {
                            if (l == 6)
                            {
                                dataList.Add(DOUBLEQUOTE + " " + DOUBLEQUOTE);
                            }
                            else
                                dataList.Add(DOUBLEQUOTE + " " + DOUBLEQUOTE
                                        + SEPARATOR);
                        }
                    }
                }
                data.Add(dataList);
            }
            catch (Exception e)
            {
                Log.Fatal("Output error ", e);
            }
            return this.exportCSVFile("AP_RSE_Weeky_Gazette", "txt", headerList, data);
        }
        public FileStreamResult ExportWeeklyNotice(DateTime dataOfGazette, string regType, string acting, string authId)
        {
            List<string> headerList = new List<string>() {
                    "file_ref_1", "reg_no_1",
                    "ename_1", "cname_1", "cert_cname_1", "cat_code_1",
                    "expiry_dt_1", "file_ref_2", "reg_no_2", "ename_2",
                    "cname_2", "cert_cname_2", "cat_code_2", "expiry_dt_2",
                    "file_ref_3", "reg_no_3", "ename_3", "cname_3",
                    "cert_cname_3", "cat_code_3", "expiry_dt_3", "file_ref_4",
                    "reg_no_4", "ename_4", "cname_4", "cert_cname_4",
                    "cat_code_4", "expiry_dt_4", "file_ref_5", "reg_no_5",
                    "ename_5", "cname_5", "cert_cname_5", "cat_code_5",
                    "expiry_dt_5", "file_ref_6", "reg_no_6", "ename_6",
                    "cname_6", "cert_cname_6", "cat_code_6", "expiry_dt_6",
                    "file_ref_7", "reg_no_7", "ename_7", "cname_7",
                    "cert_cname_7", "cat_code_7", "expiry_dt_7", "file_ref_8",
                    "reg_no_8", "ename_8", "cname_8", "cert_cname_8",
                    "cat_code_8", "expiry_dt_8", "file_ref_9", "reg_no_9",
                    "ename_9", "cname_9", "cert_cname_9", "cat_code_9",
                    "expiry_dt_9", "file_ref_10", "reg_no_10", "ename_10",
                    "cname_10", "cert_cname_10", "cat_code_10", "expiry_dt_10",
                    "file_ref_11", "reg_no_11", "ename_11", "cname_11",
                    "cert_cname_11", "cat_code_11", "expiry_dt_11",
                    "file_ref_12", "reg_no_12", "ename_12", "cname_12",
                    "cert_cname_12", "cat_code_12", "expiry_dt_12",
                    "file_ref_13", "reg_no_13", "ename_13", "cname_13",
                    "cert_cname_13", "cat_code_13", "expiry_dt_13",
                    "file_ref_14", "reg_no_14", "ename_14", "cname_14",
                    "cert_cname_14", "cat_code_14", "expiry_dt_14",
                    "file_ref_15", "reg_no_15", "ename_15", "cname_15",
                    "cert_cname_15", "cat_code_15", "expiry_dt_15",
                    "egazet_dt", "cgazet_dt", "auth_name", "auth_cname",
                    "auth_rank", "auth_crank", "auth_tel", "auth_fax", "acting"
            };
            List<List<object>> data = new List<List<object>>();
            data = getWeeklyNotice(dataOfGazette, regType, acting, authId);
            for (int i = 0; i < data.Count(); i++)
            {
                data[i][0] += appendDoubleQuote(dataOfGazette.ToString("dd MMMM yyyy")) + "," + appendDoubleQuote(getChineseFormatDate(dataOfGazette));
            }
            return this.exportCSVFile("MwWeeklyNotice", "txt", headerList, data);

        }
        public FileStreamResult ExportIndivWeeklyNotice(DateTime? dataOfGazette, string regType, string acting, string authId, string cat_code)
        {
            List<List<object>> data = new List<List<object>>();
            List<string> headerList = new List<string>();

            if (cat_code == null || cat_code.Equals(""))
            {
                headerList = new List<string>{
                    "file_ref_1", "reg_no_1",
                        "title_1", "surname_1", "given_name_1",
                        "given_name_id_1", "cname_1", "cert_cname_1",
                        "letter_file_ref_1", "reg_dt_1", "cat_code_1",
                        "file_ref_2", "reg_no_2", "title_2", "surname_2",
                        "given_name_2", "given_name_id_2", "cname_2",
                        "cert_cname_2", "letter_file_ref_2", "reg_dt_2",
                        "cat_code_2", "file_ref_3", "reg_no_3", "title_3",
                        "surname_3", "given_name_3", "given_name_id_3",
                        "cname_3", "cert_cname_3", "letter_file_ref_3",
                        "reg_dt_3", "cat_code_3", "file_ref_4", "reg_no_4",
                        "title_4", "surname_4", "given_name_4",
                        "given_name_id_4", "cname_4", "cert_cname_4",
                        "letter_file_ref_4", "reg_dt_4", "cat_code_4",
                        "file_ref_5", "reg_no_5", "title_5", "surname_5",
                        "given_name_5", "given_name_id_5", "cname_5",
                        "cert_cname_5", "letter_file_ref_5", "reg_dt_5",
                        "cat_code_5", "file_ref_6", "reg_no_6", "title_6",
                        "surname_6", "given_name_6", "given_name_id_6",
                        "cname_6", "cert_cname_6", "letter_file_ref_6",
                        "reg_dt_6", "cat_code_6", "file_ref_7", "reg_no_7",
                        "title_7", "surname_7", "given_name_7",
                        "given_name_id_7", "cname_7", "cert_cname_7",
                        "letter_file_ref_7", "reg_dt_7", "cat_code_7",
                        "file_ref_8", "reg_no_8", "title_8", "surname_8",
                        "given_name_8", "given_name_id_8", "cname_8",
                        "cert_cname_8", "letter_file_ref_8", "reg_dt_8",
                        "cat_code_8", "file_ref_9", "reg_no_9", "title_9",
                        "surname_9", "given_name_9", "given_name_id_9",
                        "cname_9", "cert_cname_9", "letter_file_ref_9",
                        "reg_dt_9", "cat_code_9", "file_ref_10", "reg_no_10",
                        "title_10", "surname_10", "given_name_10",
                        "given_name_id_10", "cname_10", "cert_cname_10",
                        "letter_file_ref_10", "reg_dt_10", "cat_code_10",
                        "file_ref_11", "reg_no_11", "title_11", "surname_11",
                        "given_name_11", "given_name_id_11", "cname_11",
                        "cert_cname_11", "letter_file_ref_11", "reg_dt_11",
                        "cat_code_11", "file_ref_12", "reg_no_12", "title_12",
                        "surname_12", "given_name_12", "given_name_id_12",
                        "cname_12", "cert_cname_12", "letter_file_ref_12",
                        "reg_dt_12", "cat_code_12", "file_ref_13", "reg_no_13",
                        "title_13", "surname_13", "given_name_13",
                        "given_name_id_13", "cname_13", "cert_cname_13",
                        "letter_file_ref_13", "reg_dt_13", "cat_code_13",
                        "file_ref_14", "reg_no_14", "title_14", "surname_14",
                        "given_name_14", "given_name_id_14", "cname_14",
                        "cert_cname_14", "letter_file_ref_14", "reg_dt_14",
                        "cat_code_14", "file_ref_15", "reg_no_15", "title_15",
                        "surname_15", "given_name_15", "given_name_id_15",
                        "cname_15", "cert_cname_15", "letter_file_ref_15",
                        "reg_dt_15", "cat_code_15", "egazet_dt", "cgazet_dt",
                        "auth_name", "auth_cname", "auth_rank", "auth_crank",
                        "auth_tel", "auth_fax", "acting"
                };
                data = getIndivWeeklyNotices(dataOfGazette, regType, acting, authId);
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i][0] += appendDoubleQuote(dataOfGazette.Value.ToString("dd MMMM yyyy")) + "," + appendDoubleQuote(getChineseFormatDate(dataOfGazette));
                }
            }
            else
            {
                headerList = new List<string>
                {
                    "file_ref_1", "reg_no_1",
                    "title_1", "surname_1", "given_name_1",
                    "given_name_id_1", "cname_1", "cert_cname_1",
                    "letter_file_ref_1", "cat_code_1", "reg_dt_1",
                    "auth_name", "auth_cname", "auth_rank", "auth_crank",
                    "auth_tel", "auth_fax", "acting", "egazet_dt",
                    "cgazet_dt"
                };
                data = getIndivWeeklyNotice(dataOfGazette, regType, acting, authId, cat_code);
                for (int i = 0; i < data.Count(); i++)
                {
                    data[i][0] += appendDoubleQuote(dataOfGazette.Value.ToString("dd MMMM yyyy")) + "," + appendDoubleQuote(getChineseFormatDate(dataOfGazette));
                }
            }
            return this.exportCSVFile("MwWeeklyNotice", "txt", headerList, data);
        }

        public FileStreamResult ExportProLabelTwoORThreeInterview(CRMReportModel model, string regType, string reportName)
        {
            List<List<object>> data = new List<List<object>>();
            int emptyLine = 0;
            try
            {
                if (reportName.Equals("IPLabelTwoInterview"))
                {
                    emptyLine = 2 * (int.Parse(model.dd2RowsInt) - 1)
                            + (int.Parse(model.dd2ColsInt) - 1);
                }
                else
                {
                    emptyLine = 3 * (int.Parse(model.dd3RowsInt) - 1)
                            + (int.Parse(model.dd3ColsInt) - 1);
                }
            }
            catch (Exception e)
            {

            }
            if (model.exportType.Equals("1"))
            {
                data = getLabelInterviewers(model.MeetingGroup);

            }
            else if (model.exportType.Equals("2"))
            {
                data = getProfLabelInterviewees(model.MeetingGroup, regType);
            }
            else
            {
                data = getLabelInterviewers(model.MeetingGroup);
                List<List<object>> data2 = getProfLabelInterviewees(model.MeetingGroup, regType);
                data.AddRange(data2);
            }
            //data = getProLabelTwo(model, regType);
            List<string> headerList = new List<string>() {
                "file_ref", "title", "appln_name",
                "c_ename", "addr1", "addr2", "addr3", "addr4", "addr5"
            };
            //data = getWeeklyNoticeAuth(dataOfGazette, regType, acting, authId);
            return this.exportCSVFile("label", "txt", headerList, data);
        }

        private List<List<object>> getProfLabelInterviewees(string meetingGroupUUID, string regType)
        {
            List<List<object>> data = new List<List<object>>();
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            string queryStr = "" +
                             "\r\n" + "\t" + " SELECT " +
                             "\r\n" + "\t" + " DISTINCT C_APPL.FILE_REFERENCE_NO AS FILE_REF, " +
                             "\r\n" + "\t" + " S_TITLE.CODE AS TITLE, " +
                             "\r\n" + "\t" + " APPLN.SURNAME || ' ' || APPLN.GIVEN_NAME_ON_ID AS APPLN_NAME, " +
                             "\r\n" + "\t" + " '' AS C_ENAME, " +
                             "\r\n" + "\t" + " ADDR.ADDRESS_LINE1 AS ADDR1, " +
                             "\r\n" + "\t" + " ADDR.ADDRESS_LINE2 AS ADDR2, " +
                             "\r\n" + "\t" + " ADDR.ADDRESS_LINE3 AS ADDR3, " +
                             "\r\n" + "\t" + " ADDR.ADDRESS_LINE4 AS ADDR4, " +
                             "\r\n" + "\t" + " ADDR.ADDRESS_LINE5 AS ADDR5 " +
                             "\r\n" + "\t" + " FROM " +
                             "\r\n" + "\t" + " C_ADDRESS ADDR INNER JOIN C_IND_APPLICATION C_APPL ON ADDR.UUID = C_APPL.ENGLISH_OFFICE_ADDRESS_ID " +
                             "\r\n" + "\t" + " INNER JOIN C_IND_CERTIFICATE C_APPLN ON C_APPL.UUID = C_APPLN.MASTER_ID " +
                             "\r\n" + "\t" + " INNER JOIN C_INTERVIEW_CANDIDATES INTRV_CAN ON C_APPLN.CANDIDATE_NUMBER = INTRV_CAN.CANDIDATE_NUMBER " +
                             "\r\n" + "\t" + " INNER JOIN C_APPLICANT APPLN ON C_APPL.APPLICANT_ID = APPLN.UUID " +
                             "\r\n" + "\t" + " INNER JOIN C_INTERVIEW_SCHEDULE INTRV_SCH ON INTRV_CAN.INTERVIEW_SCHEDULE_ID = INTRV_SCH.UUID " +
                             "\r\n" + "\t" + " LEFT JOIN C_S_SYSTEM_VALUE S_TITLE ON S_TITLE.UUID = APPLN.TITLE_ID " +
                             "\r\n" + "\t" + " WHERE " +
                             "\r\n" + "\t" + " C_APPL.REGISTRATION_TYPE = :regtype  " +
                             "\r\n" + "\t" + " AND INTRV_SCH.MEETING_ID = :meetingGroupUUID ";
            queryParameters.Add("regtype", regType);
            queryParameters.Add("meetingGroupUUID", meetingGroupUUID);

            using (EntitiesRegistration db = new EntitiesRegistration())
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
        public FileStreamResult ExportProLabelTwoORThree(CRMReportModel model, string regType, string reportName)
        {
            List<List<object>> data = new List<List<object>>();
            int emptyLine = 0;
            try
            {
                if (reportName.Equals("LabelTwo"))
                {
                    emptyLine = 2 * (int.Parse(model.dd2Rows) - 1)
                            + (int.Parse(model.dd2Cols) - 1);
                }
                else
                {
                    emptyLine = 3 * (int.Parse(model.dd3Rows) - 1)
                            + (int.Parse(model.dd3Cols) - 1);
                }
            }
            catch (Exception e)
            {

            }
            data = getProLabelTwo(model, regType);
            List<string> headerList = new List<string>() {
                "file_ref", "name", "addr1", "addr2",
                "addr3", "addr4", "addr5", "c_o"
            };
            //data = getWeeklyNoticeAuth(dataOfGazette, regType, acting, authId);
            return this.exportCSVFile("label", "txt", headerList, data);
        }
        private List<List<object>> getProLabelTwo(CRMReportModel model, string regType)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();
            string queryStr = "" +
                "\r\n" + "\t" + " SELECT DISTINCT " +
                "\r\n" + "\t" + " IND.FILE_REFERENCE_NO AS FILE_REF, " +
                "\r\n" + "\t" + " APP.SURNAME, APP.GIVEN_NAME_ON_ID, APP.CHINESE_NAME, " +
                "\r\n" + "\t" + " ADDR.ADDRESS_LINE1 AS ADDR1, " +
                "\r\n" + "\t" + " ADDR.ADDRESS_LINE2 AS ADDR2, " +
                "\r\n" + "\t" + " ADDR.ADDRESS_LINE3 AS ADDR3, " +
                "\r\n" + "\t" + " ADDR.ADDRESS_LINE4 AS ADDR4, " +
                "\r\n" + "\t" + " ADDR.ADDRESS_LINE5 AS ADDR5, " +
                "\r\n" + "\t" + " IND.POST_TO AS POST_TO, " +
                "\r\n" + "\t" + " IND.correspondence_lang_id AS correspondence_lang_id, " +
                "\r\n" + "\t" + " IND.ENGLISH_CARE_OF AS ENGLISH_CARE_OF , " +
                "\r\n" + "\t" + " IND.chinese_care_of AS chinese_care_of " +
                "\r\n" + "\t" + " FROM " +
                "\r\n" + "\t" + " C_IND_APPLICATION IND, C_IND_CERTIFICATE CERT, C_APPLICANT APP, C_ADDRESS ADDR " +
                "\r\n" + "\t" + " where " +
                "\r\n" + "\t" + " ADDR.UUID = CASE WHEN IND.correspondence_lang_id='C' THEN " +
                "\r\n" + "\t" + " CASE WHEN IND.POST_TO ='H' THEN IND.CHINESE_HOME_ADDRESS_ID ELSE IND.CHINESE_OFFICE_ADDRESS_ID END " +
                "\r\n" + "\t" + " ELSE CASE WHEN IND.POST_TO ='H' THEN IND.ENGLISH_HOME_ADDRESS_ID ELSE IND.ENGLISH_OFFICE_ADDRESS_ID END " +
                "\r\n" + "\t" + " END " +
                "\r\n" + "\t" + " and IND.UUID=CERT.MASTER_ID " +
                "\r\n" + "\t" + " and IND.APPLICANT_ID=APP.UUID " +
                "\r\n" + "\t" + " and IND.REGISTRATION_TYPE =  :regtype ";
            queryParameters.Add("regtype", regType);
            if (!string.IsNullOrWhiteSpace(model.txtFileRef))
            {
                queryStr += " AND upper(IND.file_reference_no) LIKE  :txtFileRef ";
                queryParameters.Add("txtFileRef", "%" + model.txtFileRef.Trim().ToUpper() + "%");
            }
            if (model.LabelGazDateFrom != null)
            {
                queryStr += " AND CERT.GAZETTE_DATE >= to_date( :gaz_fr_date , 'dd/MM/yyyy') ";
                queryParameters.Add("gaz_fr_date", model.LabelGazDateFrom.ToString());
            }
            if (model.LabelGazDateTo != null)
            {
                queryStr += " AND CERT.GAZETTE_DATE <= to_date( :gaz_to_date , 'dd/MM/yyyy') ";
                queryParameters.Add("gaz_to_date", model.LabelGazDateTo.ToString());
            }
            if (model.LabelExpiryDateFrom != null)
            {
                queryStr += " AND CERT.EXPIRY_DATE >= to_date( :expiry_fr_date , 'dd/MM/yyyy') ";
                queryParameters.Add("expiry_fr_date", model.LabelExpiryDateFrom.ToString());
            }
            if (model.LabelExpiryDateTo != null)
            {
                queryStr += " AND CERT.EXPIRY_DATE <= to_date( :expiry_to_date , 'dd/MM/yyyy') ";
                queryParameters.Add("expiry_to_date", model.LabelExpiryDateTo.ToString());
            }
            if (!string.IsNullOrWhiteSpace(model.PN))
            {
                queryStr += " AND IND.practice_notes_id = :ddPNRCUUID ";
                queryParameters.Add("ddPNRCUUID", model.PN);
            }
            if (!string.IsNullOrWhiteSpace(model.CategoryCode))
            {
                queryStr += " AND CERT.category_id = :ddCtrUUID ";
                queryParameters.Add("ddCtrUUID", model.CategoryCode);
            }
            if (!string.IsNullOrWhiteSpace(model.Surname))
            {
                queryStr += " AND upper(APP.SURNAME) LIKE  :surname ";
                queryParameters.Add("surname", "%" + model.Surname.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.GivenName))
            {
                queryStr += " AND upper(APP.GIVEN_NAME_ON_ID) LIKE  :givename ";
                queryParameters.Add("givename", "%" + model.GivenName.Trim().ToUpper() + "%");
            }
            if (model.chkExpiryDate == true)
            {
                queryStr +=
                " and CERT.expiry_date is not null  " +
                " and CERT.expiry_date > current_date " +
                " and ((CERT.removal_Date is null) or (CERT.removal_Date > current_date)) ";
            }
            using (EntitiesRegistration db = new EntitiesRegistration())
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
        private List<List<object>> getIndivWeeklyNotices(DateTime? dataOfGazette, string regType, string acting, string authId)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();
            string queryStr =
                "\r\n" + "\t" + "SELECT DISTINCT C_RPT_INDIV_APP_WEEKLY( :dateOfGazette, :registrationType ) as FILE_REF_STRING, " +
                "\r\n" + "\t" + " CASE WHEN :acting1 = 1 then S_AUTH.ENGLISH_ACTION_NAME ELSE S_AUTH.ENGLISH_NAME END AS AUTH_ENAME, " +
                "\r\n" + "\t" + " CASE WHEN :acting2 = 1 then S_AUTH.CHINESE_ACTION_NAME ELSE S_AUTH.CHINESE_NAME END AS AUTH_CNAME, " +
                "\r\n" + "\t" + " CASE WHEN :acting3 = 1 then S_AUTH.ENGLISH_ACTION_RANK ELSE S_AUTH.ENGLISH_RANK END AS AUTH_ERANK, " +
                "\r\n" + "\t" + " CASE WHEN :acting4 = 1 then S_AUTH.CHINESE_ACTION_RANK ELSE S_AUTH.CHINESE_RANK END AS AUTH_CRANK, " +
                "\r\n" + "\t" + " S_AUTH.TELEPHONE_NO AS AUTH_TEL," +
                "\r\n" + "\t" + " S_AUTH.FAX_NO AS AUTH_FAX " +
                "\r\n" + "\t" + " FROM " +
                "\r\n" + "\t" + " C_S_AUTHORITY S_AUTH ";
            if (authId != null && !authId.Trim().Equals(""))
            {
                queryStr += " WHERE" + " S_AUTH.UUID = :authId";
            }
            queryParameters.Add("dateOfGazette", dataOfGazette.ToString());
            queryParameters.Add("registrationType", regType);
            queryParameters.Add("acting1", acting);
            queryParameters.Add("acting2", acting);
            queryParameters.Add("acting3", acting);
            queryParameters.Add("acting4", acting);
            if (authId != null && !authId.Trim().Equals(""))
            {
                queryParameters.Add("authId", authId);
            }
            //for (int i = 0; i < list.size(); i++)
            //{
            //    Object[] element = (Object[])list.get(i);
            //    String[] newElement = new String[element.length + 1];

            //    for (int j = 0; j < element.length; j++)
            //    {
            //        newElement[j] = (String)element[j];
            //    }
            //    if (acting.equals("1"))
            //    {
            //        newElement[element.length] = "True";
            //    }
            //    else
            //    {
            //        newElement[element.length] = "False";
            //    }
            //    list.remove(i);
            //    list.add(i, newElement);
            //}
            using (EntitiesRegistration db = new EntitiesRegistration())
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

        private List<List<object>> getMwIndivWeeklyNoticeFileRef(DateTime? dataOfGazette, string regType)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();

            string queryStr = "" +
                     "\r\n" + "\t" + " I_APPL.FILE_REFERENCE_NO AS FILE_REF," +
                     "\r\n" + "\t" + " S_TITLE.ENGLISH_DESCRIPTION,  " +
                     "\r\n" + "\t" + " upper(APPLN.SURNAME),  " +
                     "\r\n" + "\t" + " c_propercase( APPLN.GIVEN_NAME_ON_ID), " +
                     "\r\n" + "\t" + " c_propercase( APPLN.GIVEN_NAME_ON_ID), " +
                     "\r\n" + "\t" + " CASE WHEN APPLN.CHINESE_NAME IS NULL THEN ( upper(APPLN.SURNAME) || ' ' || c_propercase( APPLN.GIVEN_NAME_ON_ID)) ELSE CAST (APPLN.CHINESE_NAME AS VARCHAR2 (300)) END,  " +
                     "\r\n" + "\t" + " CASE WHEN APPLN.CHINESE_NAME IS NULL THEN ( upper(APPLN.SURNAME) || ' ' || c_propercase( APPLN.GIVEN_NAME_ON_ID)) ELSE CAST (APPLN.CHINESE_NAME AS VARCHAR2 (300)) END, " +
                     "\r\n" + "\t" + " CASE WHEN S_CAT.CODE = 'RSE' THEN I_APPL.FILE_REFERENCE_NO ||'(SE)'  " +
                     "\r\n" + "\t" + " WHEN S_CAT.CODE = 'RGE' THEN I_APPL.FILE_REFERENCE_NO ||'(GE)' ELSE I_APPL.FILE_REFERENCE_NO  " +
                     "\r\n" + "\t" + " END,  " +
                     "\r\n" + "\t" + " to_char (I_CERT.REGISTRATION_DATE, 'dd.mm.yyyy')," +
                     "\r\n" + "\t" + " S_CAT.CODE AS CAT_CODE, " +
                     "\r\n" + "\t" + " CASE WHEN I_CERT.EXTENDED_DATE IS NULL THEN to_char (I_CERT.EXPIRY_DATE, 'dd.mm.yyyy') " +
                     "\r\n" + "\t" + " ELSE to_char (I_CERT.EXTENDED_DATE, 'dd.mm.yyyy') END," +
                     "\r\n" + "\t" + " I_APPL.UUID " +
                     "\r\n" + "\t" + " FROM   " +
                     "\r\n" + "\t" + " C_APPLICANT APPLN INNER JOIN C_IND_APPLICATION I_APPL ON APPLN.UUID = I_APPL.APPLICANT_ID" +
                     "\r\n" + "\t" + " INNER JOIN C_IND_CERTIFICATE I_CERT ON I_APPL.UUID = I_CERT.MASTER_ID " +
                     "\r\n" + "\t" + " INNER JOIN C_S_CATEGORY_CODE S_CAT ON I_CERT.CATEGORY_ID = S_CAT.UUID  " +
                     "\r\n" + "\t" + " LEFT JOIN C_S_SYSTEM_VALUE S_TITLE ON APPLN.TITLE_ID = S_TITLE.UUID ";

            int whereClauseCount = 0;
            if (regType != null && !regType.Trim().Equals(""))
            {
                queryStr += " WHERE" + " S_CAT.REGISTRATION_TYPE = :registrationType  ";
                whereClauseCount++;
            }
            if (dataOfGazette != null)
            {
                if (whereClauseCount > 0)
                    queryStr += " and ";
                else queryStr += " WHERE ";
                queryStr += " I_CERT.GAZETTE_DATE = TO_DATE (:dateOfGazette,'DD/MM/YYYY')  ";
            }
            queryStr += " order by APPLN.SURNAME || ' ' || APPLN.GIVEN_NAME_ON_ID, S_CAT.CODE ";

            queryParameters.Add("gaz_date", dataOfGazette.ToString());
            queryParameters.Add("reg_type", regType);


            using (EntitiesRegistration db = new EntitiesRegistration())
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

        private List<List<object>> getMwIndivWeeklyNoticeAuth(string acting, string authId)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();

            string queryStr = "" +
                    "\r\n" + "\t" + " SELECT CASE WHEN :acting1=1 then S_AUTH.ENGLISH_ACTION_NAME ELSE S_AUTH.ENGLISH_NAME END AS AUTH_ENAME," +
                    "\r\n" + "\t" + " CASE WHEN :acting2=1 then S_AUTH.CHINESE_ACTION_NAME ELSE S_AUTH.CHINESE_NAME END AS AUTH_CNAME," +
                    "\r\n" + "\t" + " CASE WHEN :acting3=1 then S_AUTH.ENGLISH_ACTION_RANK ELSE S_AUTH.ENGLISH_RANK END AS AUTH_ERANK," +
                    "\r\n" + "\t" + " CASE WHEN :acting4=1 then S_AUTH.CHINESE_ACTION_RANK ELSE S_AUTH.CHINESE_RANK END AS AUTH_CRANK," +
                    "\r\n" + "\t" + " S_AUTH.TELEPHONE_NO AS AUTH_TEL," +
                    "\r\n" + "\t" + " S_AUTH.FAX_NO AS AUTH_FAX" +
                    "\r\n" + "\t" + " FROM " +
                    "\r\n" + "\t" + " C_S_AUTHORITY S_AUTH ";

            queryStr += "\r\n" + "\t" + " WHERE" + " S_AUTH.UUID = :authId";

            queryParameters.Add("acting1", acting);
            queryParameters.Add("acting2", acting);
            queryParameters.Add("acting3", acting);
            queryParameters.Add("acting4", acting);
            queryParameters.Add("authId", authId);

            //for (int i = 0; i < list.size(); i++)
            //{
            //    Object[] element = (Object[])list.get(i);
            //    String[] newElement = new String[element.length + 1];

            //    for (int j = 0; j < element.length; j++)
            //    {
            //        newElement[j] = (String)element[j];
            //    }
            //    if (acting.equals("1"))
            //    {
            //        newElement[element.length] = "True";
            //    }
            //    else
            //    {
            //        newElement[element.length] = "False";
            //    }
            //    list.remove(i);
            //    list.add(i, newElement);
            //}

            using (EntitiesRegistration db = new EntitiesRegistration())
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
        private List<List<object>> getIndivWeeklyNotice(DateTime? dataOfGazette, string regType, string acting, string authId, string cat_code)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();

            string queryStr = "" +
                 "\r\n" + "\t" + " SELECT " +
                 "\r\n" + "\t" + " I_APPL.FILE_REFERENCE_NO AS FILE_REF, " +
                 "\r\n" + "\t" + " I_CERT.CERTIFICATION_NO AS reg_NO, " +
                 "\r\n" + "\t" + " S_TITLE.ENGLISH_DESCRIPTION, " +
                 "\r\n" + "\t" + " upper(APPLN.SURNAME), " +
                 "\r\n" + "\t" + " c_propercase( APPLN.GIVEN_NAME_ON_ID), " +
                 "\r\n" + "\t" + " c_propercase( APPLN.GIVEN_NAME_ON_ID), " +
                 "\r\n" + "\t" + " CASE WHEN APPLN.CHINESE_NAME IS NULL THEN ( upper(APPLN.SURNAME) || ' ' || c_propercase( APPLN.GIVEN_NAME_ON_ID)) ELSE CAST (APPLN.CHINESE_NAME AS VARCHAR2 (300)) END, " +
                 "\r\n" + "\t" + " CASE WHEN APPLN.CHINESE_NAME IS NULL THEN ( upper(APPLN.SURNAME) || ' ' || c_propercase( APPLN.GIVEN_NAME_ON_ID)) ELSE CAST (APPLN.CHINESE_NAME AS VARCHAR2 (300)) END, " +
                 "\r\n" + "\t" + " CASE WHEN S_CAT.CODE = 'RSE' THEN I_APPL.FILE_REFERENCE_NO ||'(SE)' " +
                 "\r\n" + "\t" + " WHEN S_CAT.CODE = 'RGE' THEN I_APPL.FILE_REFERENCE_NO ||'(GE)' ELSE I_APPL.FILE_REFERENCE_NO " +
                 "\r\n" + "\t" + " END, " +
                 "\r\n" + "\t" + " S_CAT.CODE AS CAT_CODE, " +
                 "\r\n" + "\t" + " to_char (I_CERT.REGISTRATION_DATE, 'dd.mm.yyyy'), " +
                 "\r\n" + "\t" + " CASE WHEN :acting1=1 then S_AUTH.ENGLISH_ACTION_NAME ELSE S_AUTH.ENGLISH_NAME END AS AUTH_ENAME, " +
                 "\r\n" + "\t" + " CASE WHEN :acting2=1 then S_AUTH.CHINESE_ACTION_NAME ELSE S_AUTH.CHINESE_NAME END AS AUTH_CNAME, " +
                 "\r\n" + "\t" + " CASE WHEN :acting3=1 then S_AUTH.ENGLISH_ACTION_RANK ELSE S_AUTH.ENGLISH_RANK END AS AUTH_ERANK, " +
                 "\r\n" + "\t" + " CASE WHEN :acting4=1 then S_AUTH.CHINESE_ACTION_RANK ELSE S_AUTH.CHINESE_RANK END AS AUTH_CRANK, " +
                 "\r\n" + "\t" + " S_AUTH.TELEPHONE_NO AS AUTH_TEL, " +
                 "\r\n" + "\t" + " S_AUTH.FAX_NO AS AUTH_FAX, " +
                 "\r\n" + "\t" + " CASE WHEN :acting5=1 then 'True' ELSE 'False' END AS acting " +
                 "\r\n" + "\t" + " FROM " +
                 "\r\n" + "\t" + " C_APPLICANT APPLN INNER JOIN C_IND_APPLICATION I_APPL ON APPLN.UUID = I_APPL.APPLICANT_ID " +
                 "\r\n" + "\t" + " INNER JOIN C_IND_CERTIFICATE I_CERT ON I_APPL.UUID = I_CERT.MASTER_ID " +
                 "\r\n" + "\t" + " INNER JOIN C_S_CATEGORY_CODE S_CAT ON I_CERT.CATEGORY_ID = S_CAT.UUID " +
                 "\r\n" + "\t" + " LEFT JOIN C_S_SYSTEM_VALUE S_TITLE ON APPLN.TITLE_ID = S_TITLE.UUID " +
                 "\r\n" + "\t" + " LEFT JOIN " +
                 "\r\n" + "\t" + " (SELECT * FROM C_S_AUTHORITY S_AUTH WHERE S_AUTH.UUID = :authorityId AND ROWNUM <=1) S_AUTH ON 1=1 " +
                 "\r\n" + "\t" + " WHERE " +
                 "\r\n" + "\t" + " S_CAT.REGISTRATION_TYPE = :reg_type " +
                 "\r\n" + "\t" + " and I_CERT.GAZETTE_DATE = TO_DATE(:gaz_date,'DD/MM/YYYY') " +
                 "\r\n" + "\t" + (cat_code != null && !cat_code.Equals("") ? " and S_CAT.CODE = :cat_code " : "") +
                 "\r\n" + "\t" + "ORDER BY S_CAT.CODE, APPLN.SURNAME, APPLN.GIVEN_NAME_ON_ID ";

            queryParameters.Add("gaz_date", dataOfGazette.ToString());
            queryParameters.Add("reg_type", regType);
            queryParameters.Add("authorityId", authId);
            queryParameters.Add("acting1", acting);
            queryParameters.Add("acting2", acting);
            queryParameters.Add("acting3", acting);
            queryParameters.Add("acting4", acting);
            queryParameters.Add("acting5", acting);

            if (cat_code != null && !"".Equals(cat_code))
            {
                queryParameters.Add("cat_code", cat_code);
            }

            using (EntitiesRegistration db = new EntitiesRegistration())
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
        public FileStreamResult ExportCGCLabelTwoORThree(
            string ReportName, string txtFileRef,
            string RegType, DateTime? GazDateFro, DateTime? GazDateTo,
            DateTime? ExpDateFro, DateTime? ExpDateTo,
            string ddPNRCUUID, string ddCtrUUID, string textCompName,
            string chkExpiryDate, string dd2Cols, string dd2Rows,
            string dd3Cols, string dd3Rows)
        {
            List<string> headerList = new List<string>() {
                "file_ref", "name", "addr1", "addr2",
                "addr3", "addr4", "addr5", "c_o"
            };
            int emptyLine = 0;
            try
            {
                if ("LabelTwo".Equals(ReportName))
                {
                    emptyLine = 2 * (int.Parse(dd2Rows) - 1)
                            + (int.Parse(dd2Cols) - 1);
                }
                else
                {
                    emptyLine = 3 * (int.Parse(dd3Rows) - 1)
                            + (int.Parse(dd3Cols) - 1);
                }
            }
            catch (Exception e)
            {
            }
            List<List<object>> data = new List<List<object>>();

            data = getCGCLabelTwo(RegType, txtFileRef, GazDateFro,
                GazDateTo, ExpDateFro, ExpDateTo, ddPNRCUUID, ddCtrUUID, textCompName, chkExpiryDate);

            List<string> finalHeaderList = new List<string>();

            for (int i = 0; i < headerList.Count(); i++)
            {
                finalHeaderList.Add(headerList.ElementAt(i));
            }
            for (int i = 0; i < emptyLine; i++)
            {
                for (int j = 0; j < headerList.Count(); j++)
                {
                    finalHeaderList.Add("");
                }
            }
            return this.exportCSVFile("label", "txt", finalHeaderList, data);
        }
        public FileStreamResult ExportCGCLabelTwoORThreeInterview(
            string ReportName, string regType,
            string exportTypeInterV, string meetingGroup,
            string dd2ColsIntV, string dd2RowsIntV,
            string dd3ColsIntV, string dd3RowsIntV)
        {
            List<List<object>> data = new List<List<object>>();
            int emptyLine = 0;

            try
            {
                if ("LabelTwoInterview".Equals(ReportName))
                {
                    emptyLine = 2 * (int.Parse(dd2RowsIntV) - 1)
                            + (int.Parse(dd2ColsIntV) - 1);
                }
                else
                {
                    emptyLine = 3 * (int.Parse(dd3RowsIntV) - 1)
                            + (int.Parse(dd3ColsIntV) - 1);
                }
            }
            catch (Exception e)
            {

            }
            if ("1".Equals(exportTypeInterV))
            {
                data = getLabelInterviewers(meetingGroup);
            }
            else if ("2".Equals(exportTypeInterV))
            {
                data = getCGCLabelInterviewees(regType, meetingGroup);
            }
            else
            {
                data = getLabelInterviewers(meetingGroup);
                List<List<object>> data2 = getCGCLabelInterviewees(regType, meetingGroup);
                data.AddRange(data2);
            }

            List<string> headerList = new List<string>() {
                "file_ref", "title", "appln_name",
                "c_ename", "addr1", "addr2", "addr3", "addr4", "addr5"
            };
            List<string> finalHeaderList = new List<string>();
            for (int i = 0; i < headerList.Count(); i++)
            {
                finalHeaderList.Add(headerList[i]);
            }
            for (int i = 0; i < emptyLine; i++)
            {
                for (int j = 0; j < headerList.Count(); j++)
                {
                    finalHeaderList.Add("");
                }
            }
            return this.exportCSVFile("label", "txt", finalHeaderList, data);

        }

        public FileStreamResult ExportAsTdOoExpInfoReport(CRMReportModel model, string registrationType)
        {
            List<string> headerList = new List<string>() {
                    "category_code", "file_ref",
                    "e_comp_name", "c_comp_name", "date_of_expiry",
                    "role", "title", "surname", "given_name", "ch_name",
                    "HKID / Passport No.",
                    "e_residential_addr1", "e_residential_addr2", "e_residential_addr3", "e_residential_addr4", "e_residential_addr5",
                    "c_residential_addr1", "c_residential_addr2", "c_residential_addr3", "c_residential_addr4", "c_residential_addr5",
                    "office_tel", "moblie_tel", "residential_tel",
                    "email1", "email2", "status",
                    "date_of_acceptance", "date_of_removal",
                    "date_of_withdrawal", "date_of_refusal",
                    "issue_date_of_QP_Card", "expiry_date_of_QP_Card",
                    "serial_no_of_QP_Card", "return_date_of_QP_Card",
                    "Remarks"
            };
            List<List<object>> data = new List<List<object>>();
            data = retrieveASTDOOInformation(model, registrationType);

            return this.exportExcelFile("appln_info" + registrationType, headerList, data);
        }
        public FileStreamResult ExportMWCReport(CRMReportModel model, string registrationType)
        {

            List<string> headerList = new List<string>() {
                    "file_ref", "reg_no",
                    "e_comp_name", "c_comp_name", "e_office_address",
                    "c_office_address", "c_o", "chn_c_o", "tel1", "tel2",
                    "tel3", "fax1", "fax2", "email", "pnrc", "category_code",
                    "expiry_date", "form_used", "period_of_validity",
                    "app_status", "date_of_registration", "date_of_gazette",
                    "date_of_approval", "retention_application_submitted",
                    "retention_commenced", "restoration_application_submitted",
                    "restoration_commenced", "removed_from_register",
                    "extended_date_of_expiry", "eng_authority_name",
                    "chn_authority_name", "eng_authority_title",
                    "chn_authority_title", "letter_file_ref", "Highest_Class",
                    "Interested_in_Providing_Services_of_QP",
                    "Interested_in_Providing_Services_in_Fire_Safety"
            };
            List<List<object>> data = new List<List<object>>();
            data = retrieveMWCInformation(model, registrationType);

            return this.exportExcelFile("appln_info_" + registrationType, headerList, data);
        }
        private List<List<object>> retrieveMWCInformation(CRMReportModel model, string registrationType)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();

            string queryStr = ""
                    + "\r\n" + "\t" + "SELECT DISTINCT "
                    + "\r\n" + "\t" + "C_APPL.FILE_REFERENCE_NO AS FILE_REF, "
                    + "\r\n" + "\t" + "C_APPL.CERTIFICATION_NO AS REG_NO, "
                    + "\r\n" + "\t" + "C_APPL.ENGLISH_COMPANY_NAME AS E_COMP_NAME, "
                    + "\r\n" + "\t" + "C_APPL.CHINESE_COMPANY_NAME AS C_COMP_NAME, "
                    + "\r\n" + "\t" + "TRIM(E_ADDR.ADDRESS_LINE1)||' '||TRIM(E_ADDR.ADDRESS_LINE2)||' '||TRIM(E_ADDR.ADDRESS_LINE3)||' '||TRIM(E_ADDR.ADDRESS_LINE4)||' '||TRIM(E_ADDR.ADDRESS_LINE5) AS E_OFFICE_ADDRESS, "
                    + "\r\n" + "\t" + "TRIM(C_ADDR.ADDRESS_LINE1)||TRIM(C_ADDR.ADDRESS_LINE2)||TRIM(C_ADDR.ADDRESS_LINE3)||TRIM(C_ADDR.ADDRESS_LINE4)||TRIM(C_ADDR.ADDRESS_LINE5) AS C_OFFICE_ADDRESS, "
                    + "\r\n" + "\t" + "C_APPL.ENGLISH_CARE_OF AS C_O, "
                    + "\r\n" + "\t" + "C_APPL.CHINESE_CARE_OF AS CHN_C_O, "
                    + "\r\n" + "\t" + "C_APPL.TELEPHONE_NO1 AS TEL1, "
                    + "\r\n" + "\t" + "C_APPL.TELEPHONE_NO2 AS TEL2, "
                    + "\r\n" + "\t" + "C_APPL.TELEPHONE_NO3 AS TEL3, "
                    + "\r\n" + "\t" + "C_APPL.FAX_NO1 AS FAX1, "
                    + "\r\n" + "\t" + "C_APPL.FAX_NO2 AS FAX2, "
                    + "\r\n" + "\t" + "C_APPL.EMAIL_ADDRESS AS EMAIL, "
                    + "\r\n" + "\t" + "S_PNRC.ENGLISH_DESCRIPTION AS PNRC, "
                    + "\r\n" + "\t" + "S_CAT.CODE AS CATEGORY_CODE, "
                    + "\r\n" + "\t" + "to_char(C_APPL.EXPIRY_DATE, 'dd/mm/yyyy') AS EXPIRY_DATE, "
                    + "\r\n" + "\t" + " S_APP_FORM.CODE AS FORM_USED, "
                    + "\r\n" + "\t" + "S_POV.CODE AS PERIOD_OF_VALIDITY, "
                    + "\r\n" + "\t" + "S_STATUS.ENGLISH_DESCRIPTION AS APP_STATUS, "
                    + "\r\n" + "\t" + "to_char(C_APPL.REGISTRATION_DATE, 'dd/mm/yyyy') AS DATE_OF_FIRST_REGISTRATION, "
                    + "\r\n" + "\t" + "to_char(C_APPL.GAZETTE_DATE, 'dd/mm/yyyy') AS DATE_OF_GAZETTE, "
                    + "\r\n" + "\t" + "to_char(C_APPL.APPROVAL_DATE, 'dd/mm/yyyy') AS DATE_OF_APPROVAL, "
                    + "\r\n" + "\t" + "to_char(C_APPL.RETENTION_APPLICATION_DATE, 'dd/mm/yyyy') AS RET_APPL_SUB, "
                    + "\r\n" + "\t" + "to_char(C_APPL.RETENTION_DATE, 'dd/mm/yyyy') AS RETENTION_COMMENCED, "
                    + "\r\n" + "\t" + "to_char(C_APPL.RESTORATION_APPLICATION_DATE, 'dd/mm/yyyy') AS REST_APPL_SUB, "
                    + "\r\n" + "\t" + "to_char(C_APPL.RESTORE_DATE, 'dd/mm/yyyy') AS RESTORATION_COMMENCED, "
                    + "\r\n" + "\t" + "to_char(C_APPL.REMOVAL_DATE, 'dd/mm/yyyy') AS REMOVED_FROM_REGISTER, "
                    + "\r\n" + "\t" + "to_char(C_APPL.EXTEND_DATE, 'dd/mm/yyyy') AS EXTEND_DATE_OF_EXPIRY, "
                    + "\r\n" + "\t" + "AUTH.ENGLISH_NAME AS AUTH_NAME, "
                    + "\r\n" + "\t" + "AUTH.CHINESE_NAME AS AUTH_CNAME, "
                    + "\r\n" + "\t" + "AUTH.ENGLISH_RANK AS AUTH_RANK, "
                    + "\r\n" + "\t" + "AUTH.CHINESE_RANK AS AUTH_CRANK, "
                    + "\r\n" + "\t" + "C_APPL.FILE_REFERENCE_NO AS LETTER_FILE_REF, "
                    + "\r\n" + "\t" + "(SELECT CASE WHEN min(sv1.CODE)='Class 1' THEN 'I' "
                    + "\r\n" + "\t" + "WHEN  min(sv1.CODE)='Class 2' THEN 'II' "
                    + "\r\n" + "\t" + "ELSE 'III' END AS ITEM FROM C_COMP_APPLICANT_MW_ITEM a "
                    + "\r\n" + "\t" + "INNER JOIN C_S_SYSTEM_VALUE sv1 ON sv1.UUID = a.ITEM_CLASS_ID "
                    + "\r\n" + "\t" + "INNER JOIN C_COMP_APPLICANT_INFO b ON a.COMPANY_APPLICANTS_ID = b.UUID "
                    + "\r\n" + "\t" + "INNER JOIN C_S_SYSTEM_VALUE sv2 ON sv2.UUID = b.APPLICANT_STATUS_ID "
                    + "\r\n" + "\t" + "INNER JOIN C_S_SYSTEM_VALUE sv3 ON sv3.UUID = b.APPLICANT_ROLE_ID "
                    + "\r\n" + "\t" + "INNER JOIN C_COMP_APPLICATION c ON b.MASTER_ID = c.UUID "
                    + "\r\n" + "\t" + "WHERE c.UUID = C_APPL.UUID "
                    + "\r\n" + "\t" + "AND sv2.CODE = '1' AND sv3.CODE='AS') AS HIGHER_CLASS, "
                    + "\r\n" + "\t" + "CASE WHEN C_APPL.WILLINGNESS_QP='Y' THEN 'Yes' "
                    + "\r\n" + "\t" + "WHEN C_APPL.WILLINGNESS_QP='N' THEN 'No' "
                    + "\r\n" + "\t" + "ELSE 'No Indication' END AS IS_INTERESTED_QP, "
                    + "\r\n" + "\t" + "SVALUE_FSS.ENGLISH_DESCRIPTION AS INTERESTED_FSS "
                    + "\r\n" + "\t" + "FROM C_COMP_APPLICATION C_APPL "
                    + "\r\n" + "\t" + "LEFT JOIN C_COMP_APPLICANT_INFO CAI ON C_APPL.UUID = CAI.MASTER_ID "
                    + "\r\n" + "\t" + "LEFT JOIN C_ADDRESS E_ADDR ON E_ADDR.UUID = C_APPL.ENGLISH_ADDRESS_ID "
                    + "\r\n" + "\t" + "LEFT JOIN C_ADDRESS C_ADDR ON C_ADDR.UUID = C_APPL.CHINESE_ADDRESS_ID "
                    + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE S_PNRC ON C_APPL.PRACTICE_NOTES_ID = S_PNRC.UUID "
                    + "\r\n" + "\t" + "INNER JOIN C_S_CATEGORY_CODE S_CAT ON S_CAT.UUID = C_APPL.CATEGORY_ID "
                    + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE S_POV ON C_APPL.PERIOD_OF_VALIDITY_ID = S_POV.UUID "
                    + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE S_APP_FORM ON C_APPL.APPLICATION_FORM_ID = S_APP_FORM.UUID "
                    + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE S_STATUS ON C_APPL.APPLICATION_STATUS_ID = S_STATUS.UUID "
                    + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_TYPE STYPE_FSS ON STYPE_FSS.TYPE = '" + RegistrationConstant.SYSTEM_TYPE_FSS_DROPDOWN + "' "
                    + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE SVALUE_FSS ON  STYPE_FSS.UUID = SVALUE_FSS.SYSTEM_TYPE_ID "
                    + "\r\n" + "\t" + "AND NVL(C_APPL.INTERESTED_FSS, 'I') = SVALUE_FSS.CODE ";

            if (model.AuthorityName != null && !"".Equals(model.AuthorityName.Trim()))
            {
                model.AuthorityName = "'" + model.AuthorityName + "'";
                queryStr += " LEFT JOIN C_S_AUTHORITY AUTH ON AUTH.UUID = "
                        + model.AuthorityName + " ";
            }
            queryStr += " WHERE 1=1 AND ";

            if (registrationType != null && !"".Equals(registrationType.Trim()))
            {
                registrationType = "'" + registrationType + "'";
                queryStr += "\r\n" + "\t" + " S_CAT.REGISTRATION_TYPE = " + registrationType
                             + " AND S_APP_FORM.REGISTRATION_TYPE = "
                             + registrationType + " AND ";
            }
            string whereClause = "";

            if (!string.IsNullOrWhiteSpace(model.txtExpFileRef))
            {
                whereClause +=
                        "\r\n" + "\t" + "and UPPER(C_APPL.FILE_REFERENCE_NO) like '" + model.txtExpFileRef + "%' ";
            }
            if (!string.IsNullOrWhiteSpace(model.txtExpNameOfContractor))
            {
                whereClause += ""
                        + "\r\n" + "\t" + "and (UPPER(C_APPL.ENGLISH_COMPANY_NAME) like '"
                        + "\r\n" + "\t" + model.txtExpNameOfContractor
                        + "\r\n" + "\t" + "%' "
                        + "\r\n" + "\t" + "or UPPER(C_APPL.CHINESE_COMPANY_NAME) like '"
                        + "\r\n" + "\t" + model.txtExpNameOfContractor
                        + "\r\n" + "\t" + "%' "
                        + "\r\n" + "\t" + "or C_APPL.uuid in (select cai.master_id from comp_applicant_info cai "
                        + "\r\n" + "\t" + "inner join C_applicant apt on apt.uuid = cai.applicant_id "
                        + "\r\n" + "\t" + "where upper(apt.chinese_name) like '"
                        + "\r\n" + "\t" + model.txtExpNameOfContractor
                        + "\r\n" + "\t" + "%' or upper(apt.surname)||' '||upper(apt.given_name_on_id) like '"
                        + "\r\n" + "\t" + model.txtExpNameOfContractor + "%')) ";
            }

            if (!string.IsNullOrWhiteSpace(model.StatusId))
            {
                whereClause += "and C_APPL.APPLICATION_STATUS_ID = '" + model.StatusId + "' ";
            }

            if (!string.IsNullOrWhiteSpace(model.HighClass))
            {
                whereClause += ""
                        + "\r\n" + "\t" + " and (SELECT min(sv1.CODE) FROM C_COMP_APPLICANT_MW_ITEM a "
                        + "\r\n" + "\t" + " INNER JOIN C_S_SYSTEM_VALUE sv1 ON sv1.UUID = a.ITEM_CLASS_ID "
                        + "\r\n" + "\t" + " INNER JOIN C_COMP_APPLICANT_INFO b ON a.COMPANY_APPLICANTS_ID = b.UUID "
                        + "\r\n" + "\t" + " INNER JOIN C_S_SYSTEM_VALUE sv2 ON sv2.UUID = b.APPLICANT_STATUS_ID "
                        + "\r\n" + "\t" + " INNER JOIN C_S_SYSTEM_VALUE sv3 ON sv3.UUID = b.APPLICANT_ROLE_ID "
                        + "\r\n" + "\t" + " INNER JOIN C_COMP_APPLICATION c ON b.MASTER_ID = c.UUID "
                        + "\r\n" + "\t" + " WHERE c.UUID = C_APPL.UUID "
                        + "\r\n" + "\t" + " AND sv2.CODE = '1' AND sv3.CODE='AS') = '" + model.HighClass + "' ";
            }

            if (model.txtExpApprovalDtFm != null)
            {
                whereClause += ""
                        + "\r\n" + "\t" + "and to_char(C_APPL.APPROVAL_DATE, 'yyyymmdd') >= '"
                        + model.txtExpApprovalDtFm.ToString() + "' ";
            }
            if (model.txtExpApprovalDtTo != null)
            {
                whereClause += ""
                        + "\r\n" + "\t" + "and to_char(C_APPL.APPROVAL_DATE, 'yyyymmdd') <= '"
                        + model.txtExpApprovalDtTo.ToString() + "' ";
            }

            if (!string.IsNullOrWhiteSpace(model.PeriodOfValidity))
            {
                whereClause += ""
                        + "\r\n" + "\t" + "and C_APPL.PERIOD_OF_VALIDITY_ID = '"
                        + model.PeriodOfValidity + "' ";
            }

            if (model.txtExpRemovalDtFm != null)
            {
                whereClause += ""
                        + "\r\n" + "\t" + "and to_char(C_APPL.REMOVAL_DATE, 'yyyymmdd') >= '"
                        + model.txtExpRemovalDtFm.ToString() + "' ";
            }
            if (model.txtExpRemovalDtTo != null)
            {
                whereClause += ""
                        + "\r\n" + "\t" + "and to_char(C_APPL.REMOVAL_DATE, 'yyyymmdd') <= '"
                        + model.txtExpRemovalDtTo.ToString() + "' ";
            }

            if (!string.IsNullOrWhiteSpace(model.willingnessQp))
            {
                whereClause += "\r\n" + "\t" + "and C_APPL.WILLINGNESS_QP = '" + model.willingnessQp + "' ";
            }

            if (!string.IsNullOrWhiteSpace(model.interestedFSS))
            {
                whereClause += "\r\n" + "\t" + "and (C_APPL.INTERESTED_FSS = '"
                        + model.interestedFSS + "' ";
                if ("I".Equals(model.interestedFSS))
                {
                    whereClause += "or C_APPL.INTERESTED_FSS IS NULL ";
                }
                whereClause += ") ";
            }

            if (model.txtExpRegDtFm != null)
            {
                whereClause += ""
                        + "\r\n" + "\t" + "and to_char(C_APPL.REGISTRATION_DATE, 'yyyymmdd') >= '"
                        + model.txtExpRegDtFm.ToString() + "' ";
            }

            if (model.txtExpRegDtTo != null)
            {
                whereClause += ""
                        + "\r\n" + "\t" + "and to_char(C_APPL.REGISTRATION_DATE, 'yyyymmdd') <= '"
                        + model.txtExpRegDtTo.ToString() + "' ";
            }
            if (model.txtExpRetDtFm != null)
            {
                whereClause += ""
                        + "\r\n" + "\t" + "and to_char(C_APPL.RETENTION_APPLICATION_DATE, 'yyyymmdd') >= '"
                        + model.txtExpRetDtFm.ToString() + "' ";
            }
            if (model.txtExpRetDtTo != null)
            {
                whereClause += ""
                        + "\r\n" + "\t" + "and to_char(C_APPL.RETENTION_APPLICATION_DATE, 'yyyymmdd') <= '"
                        + model.txtExpRetDtTo.ToString() + "' ";
            }
            if (model.txtExpRestDtFm != null)
            {
                whereClause += ""
                        + "\r\n" + "\t" + "and to_char(C_APPL.RESTORATION_APPLICATION_DATE, 'yyyymmdd') >= '"
                        + model.txtExpRestDtFm.ToString() + "' ";
            }
            if (model.txtExpRestDtTo != null)
            {
                whereClause += ""
                        + "\r\n" + "\t" + "and to_char(C_APPL.RESTORATION_APPLICATION_DATE, 'yyyymmdd') <= '"
                        + model.txtExpRestDtTo.ToString() + "' ";
            }
            if (model.txtExpGazDtFm != null)
            {
                whereClause += ""
                        + "\r\n" + "\t" + "and to_char(C_APPL.GAZETTE_DATE, 'yyyymmdd') >= '"
                        + model.txtExpGazDtFm.ToString() + "' ";
            }
            if (model.txtExpGazDtTo != null)
            {
                whereClause += ""
                        + "\r\n" + "\t" + "and to_char(C_APPL.GAZETTE_DATE, 'yyyymmdd') <= '"
                        + model.txtExpGazDtTo.ToString() + "' ";
            }
            if (model.txtExpExpiryDtFm != null)
            {
                whereClause += ""
                        + "\r\n" + "\t" + "and to_char(C_APPL.EXPIRY_DATE, 'yyyymmdd') >= '"
                        + model.txtExpExpiryDtFm.ToString() + "' ";
            }
            if (model.txtExpExpiryDtTo != null)
            {
                whereClause += ""
                        + "\r\n" + "\t" + "and to_char(C_APPL.EXPIRY_DATE, 'yyyymmdd') <= '"
                        + model.txtExpExpiryDtTo.ToString() + "' ";
            }
            if (!"ALL".Equals(model.ddlExpPNRC))
            {
                whereClause += ""
                        + "\r\n" + "\t" + "and C_APPL.PRACTICE_NOTES_ID = '" + model.ddlExpPNRC + "' ";
            }
            if (!"ALL".Equals(model.ddlExpCatCode))
            {
                whereClause += "" + "\r\n" + "\t" + "and C_APPL.CATEGORY_ID = '" + model.ddlExpCatCode + "' ";
            }

            if (model.chkExpInfo == true)
            {
                whereClause += ""
                        + "\r\n" + "\t" + " and ((C_APPL.EXPIRY_DATE is not null and to_char( C_APPL.EXPIRY_DATE, 'yyyymmdd') >= to_char(sysdate, 'yyyymmdd')) "
                        + "\r\n" + "\t" + "or (to_char(C_APPL.RETENTION_APPLICATION_DATE, 'yyyymmdd') > '20040831' and to_char( C_APPL.EXPIRY_DATE, 'yyyymmdd') < to_char(sysdate, 'yyyymmdd'))) ";
            }

            queryStr += "\r\n" + "\t" + "S_CAT.CODE <> 'RVC' and S_CAT.CODE <>'SC' AND S_CAT.CODE<>'RC' " + whereClause;


            queryStr += "\r\n" + "\t" + " ORDER BY C_APPL.FILE_REFERENCE_NO, C_APPL.CERTIFICATION_NO";
            using (EntitiesRegistration db = new EntitiesRegistration())
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
        public FileStreamResult ExportGBCSCReport(CRMReportModel model, string registrationType)
        {

            List<string> headerList = new List<string>() {
                    "file_ref", "reg_no",
                    "e_comp_name", "c_comp_name", "e_office_address",
                    "c_office_address", "c_o", "chn_c_o", "tel1", "tel2",
                    "tel3", "fax1", "fax2", "email", "pnrc", "category_code",
                    "expiry_date", "form_used", "period_of_validity",
                    "app_status",
                    //UR
                    "pooling file ref.",
                    "date_of_registration", "date_of_gazette",
                    "date_of_approval", "retention_application_submitted",
                    "retention_commenced", "restoration_application_submitted",
                    "restoration_commenced", "removed_from_register",
                    "extended_date_of_expiry", "eng_authority_name",
                    "chn_authority_name", "eng_authority_title",
                    "chn_authority_title", "letter_file_ref",
                    "Interested_in_Providing_Services_of_QP",
                    "Interested_in_Providing_Services_in_Fire_Safety"
            };
            List<List<object>> data = new List<List<object>>();
            data = retrieveGBCSCInformation(model, registrationType);

            return this.exportExcelFile("appln_info_" + registrationType, headerList, data);
        }

        private List<List<object>> retrieveGBCSCInformation(CRMReportModel model, string registrationType)
        {

            List<List<object>> data = new List<List<object>>();
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            string whereClause = "";
            string queryStr = ""
                        + "\r\n" + "\t" + "SELECT DISTINCT "
                        + "\r\n" + "\t" + "C_APPL.FILE_REFERENCE_NO AS FILE_REF, "
                        + "\r\n" + "\t" + "C_APPL.CERTIFICATION_NO AS REG_NO, "
                        + "\r\n" + "\t" + "C_APPL.ENGLISH_COMPANY_NAME AS E_COMP_NAME, "
                        + "\r\n" + "\t" + "C_APPL.CHINESE_COMPANY_NAME AS C_COMP_NAME, "
                        + "\r\n" + "\t" + "TRIM(E_ADDR.ADDRESS_LINE1)||' '||TRIM(E_ADDR.ADDRESS_LINE2)||' '||TRIM(E_ADDR.ADDRESS_LINE3)||' '||TRIM(E_ADDR.ADDRESS_LINE4)||' '||TRIM(E_ADDR.ADDRESS_LINE5) AS E_OFFICE_ADDRESS, "
                        + "\r\n" + "\t" + "TRIM(C_ADDR.ADDRESS_LINE1)||TRIM(C_ADDR.ADDRESS_LINE2)||TRIM(C_ADDR.ADDRESS_LINE3)||TRIM(C_ADDR.ADDRESS_LINE4)||TRIM(C_ADDR.ADDRESS_LINE5) AS C_OFFICE_ADDRESS, "
                        + "\r\n" + "\t" + "C_APPL.ENGLISH_CARE_OF AS C_O, "
                        + "\r\n" + "\t" + "C_APPL.CHINESE_CARE_OF AS CHN_C_O, "
                        + "\r\n" + "\t" + "C_APPL.TELEPHONE_NO1 AS TEL1, "
                        + "\r\n" + "\t" + "C_APPL.TELEPHONE_NO2 AS TEL2, "
                        + "\r\n" + "\t" + "C_APPL.TELEPHONE_NO3 AS TEL3, "
                        + "\r\n" + "\t" + "C_APPL.FAX_NO1 AS FAX1, "
                        + "\r\n" + "\t" + "C_APPL.FAX_NO2 AS FAX2, "
                        + "\r\n" + "\t" + "C_APPL.EMAIL_ADDRESS AS EMAIL, "
                        + "\r\n" + "\t" + "S_PNRC.ENGLISH_DESCRIPTION AS PNRC, "
                        + "\r\n" + "\t" + "S_CAT.CODE AS CATEGORY_CODE, "
                        + "\r\n" + "\t" + "to_char(C_APPL.EXPIRY_DATE, 'dd/mm/yyyy') AS EXPIRY_DATE, "
                        + "\r\n" + "\t" + " S_APP_FORM.CODE AS FORM_USED, "
                        + "\r\n" + "\t" + "S_POV.CODE AS PERIOD_OF_VALIDITY, "
                        + "\r\n" + "\t" + "S_STATUS.ENGLISH_DESCRIPTION AS APP_STATUS, "
                        //UR//UR
                        + "\r\n" + "\t" + "POOL.POOL_NO AS POOL_NO, "

                        + "\r\n" + "\t" + "to_char(C_APPL.REGISTRATION_DATE, 'dd/mm/yyyy') AS DATE_OF_FIRST_REGISTRATION, "
                        + "\r\n" + "\t" + "to_char(C_APPL.GAZETTE_DATE, 'dd/mm/yyyy') AS DATE_OF_GAZETTE, "
                        + "\r\n" + "\t" + "to_char(C_APPL.APPROVAL_DATE, 'dd/mm/yyyy') AS DATE_OF_APPROVAL, "
                        + "\r\n" + "\t" + "to_char(C_APPL.RETENTION_APPLICATION_DATE, 'dd/mm/yyyy') AS RET_APPL_SUB, "
                        + "\r\n" + "\t" + "to_char(C_APPL.RETENTION_DATE, 'dd/mm/yyyy') AS RETENTION_COMMENCED, "
                        + "\r\n" + "\t" + "to_char(C_APPL.RESTORATION_APPLICATION_DATE, 'dd/mm/yyyy') AS REST_APPL_SUB, "
                        + "\r\n" + "\t" + "to_char(C_APPL.RESTORE_DATE, 'dd/mm/yyyy') AS RESTORATION_COMMENCED, "
                        + "\r\n" + "\t" + "to_char(C_APPL.REMOVAL_DATE, 'dd/mm/yyyy') AS REMOVED_FROM_REGISTER, "
                        + "\r\n" + "\t" + "to_char(C_APPL.EXTEND_DATE, 'dd/mm/yyyy') AS EXTEND_DATE_OF_EXPIRY, "
                        + "\r\n" + "\t" + "AUTH.ENGLISH_NAME AS AUTH_NAME, "
                        + "\r\n" + "\t" + "AUTH.CHINESE_NAME AS AUTH_CNAME, "
                        + "\r\n" + "\t" + "AUTH.ENGLISH_RANK AS AUTH_RANK, "
                        + "\r\n" + "\t" + "AUTH.CHINESE_RANK AS AUTH_CRANK, "
                        + "\r\n" + "\t" + "C_APPL.FILE_REFERENCE_NO AS LETTER_FILE_REF, "
                        + "\r\n" + "\t" + "CASE WHEN C_APPL.WILLINGNESS_QP='Y' THEN 'Yes' "
                        + "\r\n" + "\t" + "WHEN C_APPL.WILLINGNESS_QP='N' THEN 'No' "
                        + "\r\n" + "\t" + "ELSE 'No Indication' END AS IS_INTERESTED_QP, "
                        + "\r\n" + "\t" + "CASE WHEN S_CAT.CODE IN ('SC','SC(D)','SC(F)','SC(GI)','SC(SF)','SC(V)') THEN 'N/A' "
                        + "\r\n" + "\t" + " WHEN INTERESTED_FSS='Y' THEN 'Yes' "
                        + "\r\n" + "\t" + "WHEN INTERESTED_FSS='N' THEN 'No' "
                        + "\r\n" + "\t" + "ELSE 'No Indication' END AS INTERESTED_FSS "
                        + "\r\n" + "\t" + "FROM C_COMP_APPLICATION C_APPL "
                        + "\r\n" + "\t" + "LEFT JOIN C_COMP_APPLICANT_INFO CAI ON C_APPL.UUID = CAI.MASTER_ID "
                        + "\r\n" + "\t" + "LEFT JOIN C_ADDRESS E_ADDR ON E_ADDR.UUID = C_APPL.ENGLISH_ADDRESS_ID "
                        + "\r\n" + "\t" + "LEFT JOIN C_ADDRESS C_ADDR ON C_ADDR.UUID = C_APPL.CHINESE_ADDRESS_ID "
                        + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE S_PNRC ON C_APPL.PRACTICE_NOTES_ID = S_PNRC.UUID "
                        + "\r\n" + "\t" + "INNER JOIN C_S_CATEGORY_CODE S_CAT ON S_CAT.UUID = C_APPL.CATEGORY_ID "
                        + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE S_POV ON C_APPL.PERIOD_OF_VALIDITY_ID = S_POV.UUID "
                        + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE S_APP_FORM ON C_APPL.APPLICATION_FORM_ID = S_APP_FORM.UUID "
                        + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE S_STATUS ON C_APPL.APPLICATION_STATUS_ID = S_STATUS.UUID "
                        + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_TYPE STYPE_FSS ON STYPE_FSS.TYPE = '" + RegistrationConstant.SYSTEM_TYPE_FSS_DROPDOWN + "' "
                        + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE SVALUE_FSS ON  STYPE_FSS.UUID = SVALUE_FSS.SYSTEM_TYPE_ID  AND NVL(C_APPL.INTERESTED_FSS, 'I') = SVALUE_FSS.CODE "
                        //UR
                        + "\r\n" + "\t" + "LEFT JOIN C_POOLING POOL on C_APPL.UUID =  POOL.master_id";

            if (!string.IsNullOrWhiteSpace(model.Pooling))
            {
                if (model.Pooling == "1")
                {
                    whereClause += "\r\n" + "\t" + "and POOL.POOL_NO is not null ";
                }
                else
                {
                    whereClause += "\r\n" + "\t" + "and POOL.POOL_NO is null ";
                }

            }
            if (!string.IsNullOrWhiteSpace(model.txtExpFileRef))
            {
                //" AND upper(C_APPL.file_reference_no) LIKE  :txtFileRef ";
                //queryParameters.Add("txtFileRef", "%" + model.txtExpFileRef.Trim().ToUpper() + "%");
                whereClause += "\r\n" + "\t" + "and UPPER(C_APPL.FILE_REFERENCE_NO) like '%" + model.txtExpFileRef.Trim().ToUpper() + "%' ";
            }

            if (!string.IsNullOrWhiteSpace(model.txtExpNameOfContractor))
            {
                whereClause += ""
                        + "\r\n" + "\t" + "and (UPPER(C_APPL.ENGLISH_COMPANY_NAME) like '"
                        + model.txtExpNameOfContractor
                        + "%' "
                        + "\r\n" + "\t" + "or UPPER(C_APPL.CHINESE_COMPANY_NAME) like '"
                        + model.txtExpNameOfContractor
                        + "%' "
                        + "\r\n" + "\t" + "or C_APPL.uuid in (select cai.master_id from comp_applicant_info cai "
                        + "\r\n" + "\t" + "inner join applicant apt on apt.uuid = cai.applicant_id "
                        + "\r\n" + "\t" + "where upper(apt.chinese_name) like '"
                        + model.txtExpNameOfContractor
                        + "%' or upper(apt.surname)||' '||upper(apt.given_name_on_id) like '"
                        + model.txtExpNameOfContractor + "%')) ";
            }

            if (!string.IsNullOrWhiteSpace(model.StatusId))
            {
                whereClause += "\r\n" + "\t" + "and C_APPL.APPLICATION_STATUS_ID = '" + model.StatusId
                            + "' ";
            }

            if (model.txtExpApprovalDtFm != null)
            {
                whereClause += "\r\n" + "\t" + "and to_char(C_APPL.APPROVAL_DATE, 'yyyymmdd') >= '"
                        + model.txtExpApprovalDtFm.ToString() + "' ";
            }
            if (model.txtExpApprovalDtTo != null)
            {
                whereClause += "\r\n" + "\t" + "and to_char(C_APPL.APPROVAL_DATE, 'yyyymmdd') <= '"
                        + model.txtExpApprovalDtTo.ToString() + "' ";
            }

            if (!string.IsNullOrWhiteSpace(model.PeriodOfValidity))
            {
                whereClause += "\r\n" + "\t" + "and C_APPL.PERIOD_OF_VALIDITY_ID = '"
                        + model.PeriodOfValidity + "' ";
            }

            if (model.txtExpRemovalDtFm != null)
            {
                whereClause += "\r\n" + "\t" + "and to_char(C_APPL.REMOVAL_DATE, 'yyyymmdd') >= '"
                        + model.txtExpRemovalDtFm.ToString() + "' ";
            }
            if (model.txtExpRemovalDtTo != null)
            {
                whereClause += "\r\n" + "\t" + "and to_char(C_APPL.REMOVAL_DATE, 'yyyymmdd') <= '"
                        + model.txtExpRemovalDtTo.ToString() + "' ";
            }

            if (!string.IsNullOrWhiteSpace(model.willingnessQp))
            {
                whereClause += "\r\n" + "\t" + "and C_APPL.WILLINGNESS_QP = '"
                        + model.willingnessQp.ToString() + "' ";
            }

            if (!string.IsNullOrWhiteSpace(model.interestedFSS))
            {
                whereClause += "\r\n" + "\t" + "and (C_APPL.INTERESTED_FSS = '"
                        + model.interestedFSS + "' ";
                if ("I".Equals(model.interestedFSS))
                {
                    whereClause += "or C_APPL.INTERESTED_FSS IS NULL ";
                }
                whereClause += ") ";
            }

            if (model.txtExpRegDtFm != null)
            {
                whereClause += "\r\n" + "\t" + "and to_char(C_APPL.REGISTRATION_DATE, 'yyyymmdd') >= '"
                        + model.txtExpRegDtFm.ToString() + "' ";
            }
            if (model.txtExpRegDtTo != null)
            {
                whereClause += "\r\n" + "\t" + "and to_char(C_APPL.REGISTRATION_DATE, 'yyyymmdd') <= '"
                        + model.txtExpRegDtTo.ToString() + "' ";
            }
            if (model.txtExpRetDtFm != null)
            {
                whereClause += "\r\n" + "\t" + "and to_char(C_APPL.RETENTION_APPLICATION_DATE, 'yyyymmdd') >= '"
                        + model.txtExpRetDtFm.ToString() + "' ";
            }
            if (model.txtExpRetDtTo != null)
            {
                whereClause += "\r\n" + "\t" + "and to_char(C_APPL.RETENTION_APPLICATION_DATE, 'yyyymmdd') <= '"
                        + model.txtExpRetDtTo.ToString() + "' ";
            }
            if (model.txtExpRestDtFm != null)
            {
                whereClause += "\r\n" + "\t" + "and to_char(C_APPL.RESTORATION_APPLICATION_DATE, 'yyyymmdd') >= '"
                        + model.txtExpRestDtFm.ToString() + "' ";
            }
            if (model.txtExpRestDtTo != null)
            {
                whereClause += "\r\n" + "\t" + "and to_char(C_APPL.RESTORATION_APPLICATION_DATE, 'yyyymmdd') <= '"
                        + model.txtExpRestDtTo.ToString() + "' ";
            }
            if (model.txtExpGazDtFm != null)
            {
                whereClause += "\r\n" + "\t" + "and to_char(C_APPL.GAZETTE_DATE, 'yyyymmdd') >= '" + model.txtExpGazDtFm.ToString() + "' ";
            }
            if (model.txtExpGazDtTo != null)
            {
                whereClause += "\r\n" + "\t" + "and to_char(C_APPL.GAZETTE_DATE, 'yyyymmdd') <= '" + model.txtExpGazDtTo.ToString() + "' ";
            }
            if (model.txtExpExpiryDtFm != null)
            {
                whereClause += "\r\n" + "\t" + "and to_char(C_APPL.EXPIRY_DATE, 'yyyymmdd') >= '" + model.txtExpExpiryDtFm.ToString() + "' ";
            }
            if (model.txtExpExpiryDtTo != null)
            {
                whereClause += "\r\n" + "\t" + "and to_char(C_APPL.EXPIRY_DATE, 'yyyymmdd') <= '" + model.txtExpExpiryDtTo.ToString() + "' ";
            }
            if (!"ALL".Equals(model.ddlExpPNRC))
            {
                whereClause += "\r\n" + "\t" + "and C_APPL.PRACTICE_NOTES_ID = '" + model.ddlExpPNRC + "' ";
            }
            if (!"ALL".Equals(model.ddlExpCatCode))
            {
                whereClause += "\r\n" + "\t" + "and C_APPL.CATEGORY_ID = '"
                        + model.ddlExpCatCode + "' ";
            }
            if (!"false".Equals(model.chkExpInfo))
            {
                whereClause += "\r\n" + "\t" + " and ((C_APPL.EXPIRY_DATE is not null and to_char( C_APPL.EXPIRY_DATE, 'yyyymmdd') >= to_char(sysdate, 'yyyymmdd')) "
                        + "or (to_char(C_APPL.RETENTION_APPLICATION_DATE, 'yyyymmdd') > '20040831' and to_char( C_APPL.EXPIRY_DATE, 'yyyymmdd') < to_char(sysdate, 'yyyymmdd'))) ";
            }

            if (model.AuthorityName != null && !"".Equals(model.AuthorityName.Trim()))
            {
                model.AuthorityName = "'" + model.AuthorityName + "'";
                queryStr += "\r\n" + "\t" + " LEFT JOIN C_S_AUTHORITY AUTH ON AUTH.UUID = "
                        + model.AuthorityName + " ";
            }
            queryStr += "\r\n" + "\t" + " WHERE 1=1 AND";

            if (registrationType != null && !"".Equals(registrationType.Trim()))
            {
                registrationType = "'" + registrationType + "'";
                queryStr += "\r\n" + "\t" + " S_CAT.REGISTRATION_TYPE = " + registrationType
                        + " AND S_APP_FORM.REGISTRATION_TYPE = "
                        + registrationType + " AND ";
            }
            queryStr += "\r\n" + "\t" + "S_CAT.CODE <> 'RVC' and S_CAT.CODE <>'SC' AND S_CAT.CODE<>'RC' " + whereClause;

            queryStr += "\r\n" + "\t" + " ORDER BY C_APPL.FILE_REFERENCE_NO, C_APPL.CERTIFICATION_NO";

            using (EntitiesRegistration db = new EntitiesRegistration())
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

        private List<List<object>> retrieveAPRSERGEInformation(CRMReportModel model, string registrationType)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();
            string MWIScode = "";
            if (registrationType == RegistrationConstant.REGISTRATION_TYPE_MWIA)
            {
                MWIScode += "\r\n" + "\t" + "CASE WHEN APPLN.HKID IS NOT NULL AND   APPLN.PASSPORT_NO  IS NOT NULL"
                            + "\r\n" + "\t" + " THEN "
                            + "\r\n" + "\t" + EncryptDecryptUtil.getDecryptSQL("APPLN.HKID") + " ||'/'|| "
                            + "\r\n" + "\t" + EncryptDecryptUtil.getDecryptSQL("APPLN.PASSPORT_NO")
                            + "\r\n" + "\t" + " ELSE "
                            + "\r\n" + "\t" + EncryptDecryptUtil.getDecryptSQL("APPLN.HKID") + "||"
                            + "\r\n" + "\t" + EncryptDecryptUtil.getDecryptSQL("APPLN.PASSPORT_NO")
                            + "\r\n" + "\t" + " END AS HKID_PASSPORT_NO,"
                            // Add Qualidation/Education
                            + "\r\n\t CASE UPPER(MW_I_CAPA_FINAL.SUPPORT_BY) "
                            + "\r\n\t WHEN 'E' THEN 'Experience Only' "
                            + "\r\n\t WHEN 'Q' THEN 'Qualification Only' "
                            + "\r\n\t WHEN 'A' THEN 'Qualification + Experience Only' "
                            + "\r\n\t WHEN 'EQA' THEN 'Exp./Quali./Quali + Exp.' "
                            + "\r\n\t WHEN 'EQ' THEN 'Exp./Quali.' "
                            + "\r\n\t WHEN 'EA' THEN 'Exp./Quali.+ Exp.' "
                            + "\r\n\t WHEN 'QA' THEN 'Quali./Quali.+ Exp.' "
                            + "\r\n\t END AS Capa_supby, "
                            // Add Trade
                            + " \r\n\t S_Mw_I_Capa_Main.DESCRIPTION AS Capa_descr, "
                            ;
            }


            string Disposal = "";
            if (registrationType == RegistrationConstant.REGISTRATION_TYPE_MWIA)
            {
                Disposal += "\r\n" + "\t" + " to_char(I_APPL.DATE_OF_DISPOSAL, 'dd/mm/yyyy') AS DATE_OF_DISPOSAL, ";
            }
            string queryStr = ""
                    + "\r\n" + "\t" + " SELECT DISTINCT "
                    + "\r\n" + "\t" + " I_APPL.FILE_REFERENCE_NO AS FILE_REF, "
                    + "\r\n" + "\t" + " I_CERT.CERTIFICATION_NO AS CERT_NO, "
                    + "\r\n" + "\t" + " S_TITLE.ENGLISH_DESCRIPTION AS TITLE, "
                    + "\r\n" + "\t" + " APPLN.SURNAME AS SURNAME, "
                    + "\r\n" + "\t" + " APPLN.GIVEN_NAME_ON_ID AS GIVEN_NAME, "
                    + "\r\n" + "\t" + " APPLN.CHINESE_NAME AS CNAME, "
                    + MWIScode
                    //+"\r\n" + "\t"+ " H_ADDR.ADDRESS_LINE1 AS H_ADDR1, "
                    //+"\r\n" + "\t"+ " H_ADDR.ADDRESS_LINE2 AS H_ADDR2, "
                    //+"\r\n" + "\t"+ " H_ADDR.ADDRESS_LINE3 AS H_ADDR3, "
                    //+"\r\n" + "\t"+ " H_ADDR.ADDRESS_LINE4 AS H_ADDR4, "
                    //+"\r\n" + "\t"+ " H_ADDR.ADDRESS_LINE5 AS H_ADDR5, "
                    //+"\r\n" + "\t"+ " H_CADDR.ADDRESS_LINE1 AS H_CADDR1, "
                    //+"\r\n" + "\t"+ " H_CADDR.ADDRESS_LINE2 AS H_CADDR2, "
                    //+"\r\n" + "\t"+ " H_CADDR.ADDRESS_LINE3 AS H_CADDR3, "
                    //+"\r\n" + "\t"+ " H_CADDR.ADDRESS_LINE4 AS H_CADDR4, "
                    //+"\r\n" + "\t"+ " H_CADDR.ADDRESS_LINE5 AS H_CADDR5, "
                    + "\r\n" + "\t" + " I_APPL.ENGLISH_CARE_OF AS C_O, "
                    + "\r\n" + "\t" + " I_APPL.CHINESE_CARE_OF AS C_C_O, "
                    //+"\r\n" + "\t"+ " O_ADDR.ADDRESS_LINE1 AS O_ADDR1, "
                    //+"\r\n" + "\t"+ " O_ADDR.ADDRESS_LINE2 AS O_ADDR2, "
                    //+"\r\n" + "\t"+ " O_ADDR.ADDRESS_LINE3 AS O_ADDR3, "
                    //+"\r\n" + "\t"+ " O_ADDR.ADDRESS_LINE4 AS O_ADDR4, "
                    //+"\r\n" + "\t"+ " O_ADDR.ADDRESS_LINE5 AS O_ADDR5, "
                    //+"\r\n" + "\t"+ " O_CADDR.ADDRESS_LINE1 AS O_CADDR1, "
                    //+"\r\n" + "\t"+ " O_CADDR.ADDRESS_LINE2 AS O_CADDR2, "
                    //+"\r\n" + "\t"+ " O_CADDR.ADDRESS_LINE3 AS O_CADDR3, "
                    //+"\r\n" + "\t"+ " O_CADDR.ADDRESS_LINE4 AS O_CADDR4, "
                    //+"\r\n" + "\t"+ " O_CADDR.ADDRESS_LINE5 AS O_CADDR5, "
                    //+"\r\n" + "\t"+ " I_APPL.EMERGENCY_NO1 AS EMRG_NO1, "
                    //+"\r\n" + "\t"+ " I_APPL.EMERGENCY_NO2 AS EMRG_NO2, "
                    //+"\r\n" + "\t"+ " I_APPL.EMERGENCY_NO3 AS EMRG_NO3, "
                    //+"\r\n" + "\t"+ " I_APPL.TELEPHONE_NO1 AS TEL_NO1, "
                    //+"\r\n" + "\t"+ " I_APPL.TELEPHONE_NO2 AS TEL_NO2, "
                    //+"\r\n" + "\t"+ " I_APPL.TELEPHONE_NO3 AS TEL_NO3, "
                    //+"\r\n" + "\t"+ " I_APPL.FAX_NO1 AS FAX_NO1, "
                    //+"\r\n" + "\t"+ " I_APPL.FAX_NO2 AS FAX_NO2, "
                    //+"\r\n" + "\t"+ " I_APPL.EMAIL AS EMAIL, "
                    + "\r\n" + "\t" + " S_PNRC.ENGLISH_DESCRIPTION AS PNAP, "
                    + "\r\n" + "\t" + " S_PRB.ENGLISH_DESCRIPTION AS PRB_DESC, "
                    + "\r\n" + "\t" + " S_CAT.CODE AS CAT_CODE, "
                    + "\r\n" + "\t" + " to_char(I_CERT.EXPIRY_DATE, 'dd/mm/yyyy') AS EXP_D, "
                    + "\r\n" + "\t" + " S_APP_FORM.CODE AS FORM_USED, "
                    + "\r\n" + "\t" + " S_POV.CODE AS POV_CODE, "
                    + "\r\n" + "\t" + " S_STATUS.ENGLISH_DESCRIPTION AS APP_STATUS, "

                    + "\r\n" + "\t" + " to_char(I_CERT.REGISTRATION_DATE, 'dd/mm/yyyy') AS REG_D, "
                    + "\r\n" + "\t" + " to_char(I_CERT.GAZETTE_DATE, 'dd/mm/yyyy') AS GAZETTE_D, "
                    + "\r\n" + "\t" + " to_char(I_CERT.APPROVAL_DATE, 'dd/mm/yyyy') AS APPROVAL_D, "
                    + "\r\n" + "\t" + " to_char(I_CERT.RETENTION_APPLICATION_DATE, 'dd/mm/yyyy') AS RETENTION_APPL_D, "
                    + "\r\n" + "\t" + " to_char(I_CERT.RETENTION_DATE, 'dd/mm/yyyy') AS RETENTION_COMMENCED, "
                    + "\r\n" + "\t" + " to_char(I_CERT.RESTORATION_APPLICATION_DATE, 'dd/mm/yyyy') AS RESTORATION_APPL_D, "
                    + "\r\n" + "\t" + " to_char(I_CERT.RESTORE_DATE, 'dd/mm/yyyy') AS RESTORATION_COMMENCED, "
                    + "\r\n" + "\t" + " to_char(I_CERT.REMOVAL_DATE, 'dd/mm/yyyy') AS REMOVED_FROM_REGISTER, "
                    + "\r\n" + "\t" + " to_char(I_CERT.EXTENDED_DATE, 'dd/mm/yyyy') AS EXTENDED_D, "
                    //UR
                    + Disposal
                    + "\r\n" + "\t" + " AUTH.ENGLISH_NAME AS AUTH_NAME, "
                    + "\r\n" + "\t" + " AUTH.CHINESE_NAME AS AUTH_CNAME, "
                    + "\r\n" + "\t" + " AUTH.ENGLISH_RANK AS AUTH_RANK, "
                    + "\r\n" + "\t" + " AUTH.CHINESE_RANK AS AUTH_CRANK, "

                    + "\r\n" + "\t" + " CASE WHEN S_CAT.CODE = 'RSE' THEN I_APPL.FILE_REFERENCE_NO ||'(SE)'  "
                    + "\r\n" + "\t" + " WHEN S_CAT.CODE = 'RGE' THEN I_APPL.FILE_REFERENCE_NO ||'(GE)' ELSE I_APPL.FILE_REFERENCE_NO "
                    + "\r\n" + "\t" + " END AS LETTER_FILE_REF, "
                    + "\r\n" + "\t" + " CASE WHEN I_APPL.WILLINGNESS_QP='Y' THEN 'Yes' "
                    + "\r\n" + "\t" + " WHEN I_APPL.WILLINGNESS_QP='N' THEN 'No' "
                    + "\r\n" + "\t" + " ELSE 'No Indication' END AS IS_INTERESTED_QP, "
                    + "\r\n" + "\t" + " SVALUE_FSS.ENGLISH_DESCRIPTION AS INTERESTED_FSS, "
                    + "\r\n" + "\t" + " to_char(I_CERT.CARD_ISSUE_DATE, 'dd/mm/yyyy') AS CARD_ISSUE_DATE, "
                    + "\r\n" + "\t" + " to_char(I_CERT.CARD_EXPIRY_DATE, 'dd/mm/yyyy') AS CARD_EXPIRY_DATE, "

                    + "\r\n" + "\t" + " I_CERT.CARD_SERIAL_NO AS CARD_SERIAL_NO, "
                    + "\r\n" + "\t" + " to_char(I_CERT.CARD_RETURN_DATE, 'dd/mm/yyyy') AS CARD_RETURN_DATE ";

            // add all option fields at the end
            if (model.checkboxAddr)
            {
                queryStr +=
                        "\r\n" + "\t" + " , H_ADDR.ADDRESS_LINE1 AS H_ADDR1, "
                    + "\r\n" + "\t" + " H_ADDR.ADDRESS_LINE2 AS H_ADDR2, "
                    + "\r\n" + "\t" + " H_ADDR.ADDRESS_LINE3 AS H_ADDR3, "
                    + "\r\n" + "\t" + " H_ADDR.ADDRESS_LINE4 AS H_ADDR4, "
                    + "\r\n" + "\t" + " H_ADDR.ADDRESS_LINE5 AS H_ADDR5, "
                    + "\r\n" + "\t" + " H_CADDR.ADDRESS_LINE1 AS H_CADDR1, "
                    + "\r\n" + "\t" + " H_CADDR.ADDRESS_LINE2 AS H_CADDR2, "
                    + "\r\n" + "\t" + " H_CADDR.ADDRESS_LINE3 AS H_CADDR3, "
                    + "\r\n" + "\t" + " H_CADDR.ADDRESS_LINE4 AS H_CADDR4, "
                    + "\r\n" + "\t" + " H_CADDR.ADDRESS_LINE5 AS H_CADDR5, "
                    + "\r\n" + "\t" + " O_ADDR.ADDRESS_LINE1 AS O_ADDR1, "
                    + "\r\n" + "\t" + " O_ADDR.ADDRESS_LINE2 AS O_ADDR2, "
                    + "\r\n" + "\t" + " O_ADDR.ADDRESS_LINE3 AS O_ADDR3, "
                    + "\r\n" + "\t" + " O_ADDR.ADDRESS_LINE4 AS O_ADDR4, "
                    + "\r\n" + "\t" + " O_ADDR.ADDRESS_LINE5 AS O_ADDR5, "
                    + "\r\n" + "\t" + " O_CADDR.ADDRESS_LINE1 AS O_CADDR1, "
                    + "\r\n" + "\t" + " O_CADDR.ADDRESS_LINE2 AS O_CADDR2, "
                    + "\r\n" + "\t" + " O_CADDR.ADDRESS_LINE3 AS O_CADDR3, "
                    + "\r\n" + "\t" + " O_CADDR.ADDRESS_LINE4 AS O_CADDR4, "
                    + "\r\n" + "\t" + " O_CADDR.ADDRESS_LINE5 AS O_CADDR5 ";

            }
            if (model.checkboxTel)
            {
                queryStr +=
                      "\r\n" + "\t" + " , I_APPL.TELEPHONE_NO1 AS TEL_NO1 "
                    + "\r\n" + "\t" + " , I_APPL.TELEPHONE_NO2 AS TEL_NO2 "
                    + "\r\n" + "\t" + " , I_APPL.TELEPHONE_NO3 AS TEL_NO3 ";

            }
            if (model.checkboxEmail)
            {
                queryStr += "\r\n" + "\t" + " , I_APPL.EMAIL AS EMAIL ";
            }
            if (model.checkboxFax)
            {
                queryStr +=
                     "\r\n" + "\t" + " , I_APPL.FAX_NO1 AS FAX_NO1 "
                    + "\r\n" + "\t" + " , I_APPL.FAX_NO2 AS FAX_NO2 ";

            }
            if (model.checkboxEmergency)
            {
                queryStr +=
                    "\r\n" + "\t" + " , I_APPL.EMERGENCY_NO1 AS EMRG_NO1 "
                    + "\r\n" + "\t" + " , I_APPL.EMERGENCY_NO2 AS EMRG_NO2 "
                    + "\r\n" + "\t" + " , I_APPL.EMERGENCY_NO3 AS EMRG_NO3 ";

            }

            queryStr += "\r\n" + "\t" + " FROM "
                    + "\r\n" + "\t" + " C_IND_CERTIFICATE I_CERT "
                    + "\r\n" + "\t" + " INNER JOIN C_IND_APPLICATION I_APPL ON I_CERT.MASTER_ID = I_APPL.UUID "
                    + "\r\n" + "\t" + " INNER JOIN C_APPLICANT APPLN ON I_APPL.APPLICANT_ID = APPLN.UUID "
                    + "\r\n" + "\t" + " INNER JOIN C_S_CATEGORY_CODE S_CAT ON I_CERT.CATEGORY_ID = S_CAT.UUID "
                    + "\r\n" + "\t" + " INNER JOIN C_S_SYSTEM_VALUE S_CAT_GP ON S_CAT.CATEGORY_GROUP_ID = S_CAT_GP.UUID "
                    + "\r\n" + "\t" + " LEFT OUTER JOIN C_IND_QUALIFICATION QUALI ON I_APPL.UUID = QUALI.MASTER_ID AND I_CERT.CATEGORY_ID = QUALI.CATEGORY_ID "
                    + "\r\n" + "\t" + " LEFT JOIN C_S_SYSTEM_TYPE STYPE_FSS ON STYPE_FSS.TYPE = '" + RegistrationConstant.SYSTEM_TYPE_FSS_DROPDOWN + "' "
                    + "\r\n" + "\t" + " LEFT JOIN C_S_SYSTEM_VALUE SVALUE_FSS ON  STYPE_FSS.UUID = SVALUE_FSS.SYSTEM_TYPE_ID "
                    + "\r\n\t left JOIN C_MW_IND_CAPA_FINAL MW_I_CAPA_FINAL on MW_I_CAPA_FINAL.MASTER_ID = I_APPL.UUID "
                    + "\r\n\t left JOIN C_S_Mw_Ind_Capa_Main S_Mw_I_Capa_Main on S_Mw_I_Capa_Main.S_MW_IND_CAPA_ID = MW_I_CAPA_FINAL.MW_IND_CAPA_DISPLAY_ID"

                    //+ "\r\n\t LEFT JOIN C_MW_IND_CAPA_DETAIL CAPA_DETAIL ON 
                    //+ "\r\n\t LEFT JOIN C_MW_IND_CAPA_DETAIL_ITEM CAPA_DETAIL_ITEM ON CAPA_DETAIL.UUID = CAPA_DETAIL_ITEM.MW_IND_CAPA_DETAIL_ID "
                    //+ "\r\n\t LEFT JOIN C_S_MW_IND_CAPA_MAIN S_CAPA ON S_CAPA.CHECKBOX_CODE = CAPA_DETAIL_ITEM.CODE "
                    + "\r\n" + "\t" + " AND NVL(I_APPL.INTERESTED_FSS, 'I') = SVALUE_FSS.CODE ";
            //For UR 

            if (model.AuthorityName != null && !"".Equals(model.AuthorityName.Trim()))
            {
                model.AuthorityName = "'" + model.AuthorityName + "'";
                queryStr += " LEFT JOIN C_S_AUTHORITY AUTH ON AUTH.UUID = "
                        + model.AuthorityName + " ";
            }

            queryStr += ""
                        + "\r\n" + "\t" + " LEFT JOIN C_ADDRESS H_ADDR ON I_APPL.ENGLISH_HOME_ADDRESS_ID = H_ADDR.UUID "
                        + "\r\n" + "\t" + " LEFT JOIN C_ADDRESS H_CADDR ON I_APPL.CHINESE_HOME_ADDRESS_ID = H_CADDR.UUID "
                        + "\r\n" + "\t" + " LEFT JOIN C_ADDRESS O_ADDR ON I_APPL.ENGLISH_OFFICE_ADDRESS_ID = O_ADDR.UUID "
                        + "\r\n" + "\t" + " LEFT JOIN C_ADDRESS O_CADDR ON I_APPL.CHINESE_OFFICE_ADDRESS_ID = O_CADDR.UUID "
                        + "\r\n" + "\t"
                        + "\r\n" + "\t" + " LEFT JOIN C_S_SYSTEM_VALUE S_PNRC ON I_APPL.PRACTICE_NOTES_ID = S_PNRC.UUID "
                        + "\r\n" + "\t" + " LEFT JOIN C_S_SYSTEM_VALUE S_PRB ON QUALI.PRB_ID = S_PRB.UUID "
                        + "\r\n" + "\t" + " LEFT JOIN C_S_SYSTEM_VALUE S_APP_FORM ON I_CERT.APPLICATION_FORM_ID = S_APP_FORM.UUID "
                        + "\r\n" + "\t" + " LEFT JOIN C_S_SYSTEM_VALUE S_STATUS ON I_CERT.APPLICATION_STATUS_ID = S_STATUS.UUID "
                        + "\r\n" + "\t" + " LEFT JOIN C_S_SYSTEM_VALUE S_POV ON I_CERT.PERIOD_OF_VALIDITY_ID = S_POV.UUID "
                        + "\r\n" + "\t" + " LEFT JOIN C_S_SYSTEM_VALUE S_TITLE ON APPLN.TITLE_ID = S_TITLE.UUID "
                        + "\r\n" + "\t" + " WHERE ";

            string whereClause = "";
            if (!string.IsNullOrWhiteSpace(model.trade))
            {
                whereClause += " \r\n\t AND S_Mw_I_Capa_Main.UUID = '" + model.trade + "'";
            }
            if (!string.IsNullOrWhiteSpace(model.qualification))
            {
                whereClause += " \r\n\t AND MW_I_CAPA_FINAL.SUPPORT_BY = '" + model.qualification.ToUpper() + "' ";
            }
            if (!string.IsNullOrWhiteSpace(model.ageFrom) && !string.IsNullOrWhiteSpace(model.ageTo))
            {
                whereClause += " \r\n\t AND TRUNC((SYSDATE - APPLN.DOB)/365.25) >= cast('" + model.ageFrom + "' AS numeric)";
                whereClause += " \r\n\t AND TRUNC((SYSDATE - APPLN.DOB)/365.25) <= cast('" + model.ageTo + "' AS numeric) ";
            }
            if (!string.IsNullOrWhiteSpace(model.txtExpFileRef))
            {
                whereClause +=
                        "\r\n" + "\t" + "and UPPER (I_APPL.FILE_REFERENCE_NO) like '"
                        + model.txtExpFileRef + "%' ";
            }
            if (model.txtExpRegDtFm != null)
            {
                whereClause +=
                        "\r\n" + "\t" + "and to_char(I_CERT.REGISTRATION_DATE, 'yyyymmdd') >= '"
                        + model.txtExpRegDtFm.ToString() + "' ";
            }
            if (model.txtExpRegDtTo != null)
            {
                whereClause +=
                        "\r\n" + "\t" + "and to_char(I_CERT.REGISTRATION_DATE, 'yyyymmdd') <= '"
                        + model.txtExpRegDtTo.ToString() + "' ";
            }
            if (model.txtExpRetDtFm != null)
            {
                whereClause +=
                        "\r\n" + "\t" + "and to_char(I_CERT.RETENTION_APPLICATION_DATE, 'yyyymmdd') >= '"
                        + model.txtExpRetDtFm.ToString() + "' ";
            }
            if (model.txtExpRetDtTo != null)
            {
                whereClause +=
                        "\r\n" + "\t" + "and to_char(I_CERT.RETENTION_APPLICATION_DATE, 'yyyymmdd') <= '"
                        + model.txtExpRetDtTo.ToString() + "' ";
            }
            if (model.txtExpRestDtFm != null)
            {
                whereClause +=
                        "\r\n" + "\t" + "and to_char(I_CERT.RESTORATION_APPLICATION_DATE, 'yyyymmdd') >= '"
                        + model.txtExpRestDtFm.ToString() + "' ";
            }
            if (model.txtExpRestDtTo != null)
            {
                whereClause +=
                        "\r\n" + "\t" + "and to_char(I_CERT.RESTORATION_APPLICATION_DATE, 'yyyymmdd') <= '"
                        + model.txtExpRestDtTo.ToString() + "' ";
            }
            if (model.txtExpGazDtFm != null)
            {
                whereClause +=
                        "\r\n" + "\t" + "and to_char(I_CERT.GAZETTE_DATE, 'yyyymmdd') >= '"
                        + model.txtExpGazDtFm.ToString() + "' ";
            }
            if (model.txtExpGazDtTo != null)
            {
                whereClause +=
                        "\r\n" + "\t" + "and to_char(I_CERT.GAZETTE_DATE, 'yyyymmdd') <= '"
                        + model.txtExpGazDtTo.ToString() + "' ";
            }
            if (model.txtExpExpiryDtFm != null)
            {
                whereClause +=
                        "\r\n" + "\t" + "\r\n" + "\t" + "and to_char(I_CERT.EXPIRY_DATE, 'yyyymmdd') >= '"
                        + model.ToString() + "' ";
            }
            if (model.txtExpExpiryDtTo != null)
            {
                whereClause +=
                        "\r\n" + "\t" + "and to_char(I_CERT.EXPIRY_DATE, 'yyyymmdd') <= '"
                        + model.txtExpExpiryDtTo.ToString() + "' ";
            }
            if (!"ALL".Equals(model.ddlExpPNRC))
            {
                whereClause += "\r\n" + "\t" + "and I_APPL.PRACTICE_NOTES_ID = '" + model.ddlExpPNRC + "' ";
            }
            //if (!"ALL".Equals(model.ddlExpCatCode))
            //{
            //    whereClause += "\r\n" + "\t" + "and I_CERT.CATEGORY_ID = '" + model.ddlExpCatCode + "' ";
            //}
            //UR
            if (registrationType == RegistrationConstant.REGISTRATION_TYPE_IP)
            {
                string result = "";
                if (model.ddlExpCatCode is null)
                {
                    whereClause += "";
                }
                else if (model.ddlExpCatCode.ElementAt(0) == "ALL")
                {
                    result = "'8a85932a25bfae310125bfae31620000','8a85932a25bfae310125bfae31620001','8a85932a25bfae310125bfae31620002'," +
                        "'8a85932a25bfae310125bfae31720009','8a85932a25bfae310125bfae31720008','8a85934932fb8d6c0132fb927ec20002'" +
                        "'8a85934932fb8d6c0132fb9382860003','8a85934932fb8d6c0132fb940cd40004' ";
                    whereClause += "\r\n" + "\t" + "and I_CERT.CATEGORY_ID in(" + result + ") ";
                }
                else if (model.ddlExpCatCode.Count >= 1 && model.ddlExpCatCode.ElementAt(0) != "ALL")
                {
                    foreach (string smi in model.ddlExpCatCode)
                    {
                        if (string.IsNullOrEmpty(result))
                        {
                            result += "'" + smi + "'";
                        }
                        else
                        {
                            result += "," + "'" + smi + "'";
                        }
                    }
                    whereClause += "\r\n" + "\t" + "and I_CERT.CATEGORY_ID in(" + result + ") ";
                }
            }
            if (!string.IsNullOrWhiteSpace(model.txtExpSurname))
            {
                //whereClause = whereClause
                //        + "and UPPER (APPLN.SURNAME) like '"
                //        + StringUtil.getReportParam(model.txtExpSurname) + "%' ";
                whereClause += "\r\n" + "\t" + "and UPPER (APPLN.SURNAME) like '%" + model.txtExpSurname.ToUpper() + "%'";
            }
            if (!string.IsNullOrWhiteSpace(model.txtExpGName))
            {
                //whereClause = whereClause
                //        + "and UPPER (APPLN.GIVEN_NAME_ON_ID) like '"
                //        + StringUtil.getReportParam(model.txtExpGName) + "%' ";
                whereClause += "\r\n" + "\t" + "and UPPER (APPLN.GIVEN_NAME_ON_ID) like '%" + model.txtExpSurname.ToUpper() + "%'";
            }
            if (model.chkExpInfo == true)
            {
                whereClause +=
                        "\r\n" + "\t" + " and ((I_CERT.EXPIRY_DATE is not null and to_char( I_CERT.EXPIRY_DATE, 'yyyymmdd') >= to_char(sysdate, 'yyyymmdd')) "
                        + "or (to_char(I_CERT.RETENTION_APPLICATION_DATE, 'yyyymmdd') > '20040831' and to_char( I_CERT.EXPIRY_DATE, 'yyyymmdd') < to_char(sysdate, 'yyyymmdd'))) ";
            }

            if (!string.IsNullOrWhiteSpace(model.StatusId))
            {
                whereClause += "\r\n" + "\t" + "and I_CERT.APPLICATION_STATUS_ID = '" + model.StatusId + "' ";
            }

            if (model.txtExpApprovalDtFm != null)
            {
                whereClause += "\r\n" + "\t" + "and to_char(I_CERT.APPROVAL_DATE, 'yyyymmdd') >= '"
                        + model.txtExpApprovalDtFm.ToString() + "' ";
            }
            if (model.txtExpApprovalDtTo != null)
            {
                whereClause += "\r\n" + "\t" + "and to_char(I_CERT.APPROVAL_DATE, 'yyyymmdd') <= '"
                        + model.txtExpApprovalDtTo.ToString() + "' ";
            }

            if (!string.IsNullOrWhiteSpace(model.PeriodOfValidity))
            {
                whereClause +=
                        "\r\n" + "\t" + "and I_CERT.PERIOD_OF_VALIDITY_ID = '"
                        + model.PeriodOfValidity + "' ";
            }

            if (model.txtExpRemovalDtFm != null)
            {
                whereClause +=
                        "\r\n" + "\t" + "and to_char(I_CERT.REMOVAL_DATE, 'yyyymmdd') >= '"
                        + model.txtExpRemovalDtFm.ToString() + "' ";
            }
            if (model.txtExpRemovalDtTo != null)
            {
                whereClause +=
                        "\r\n" + "\t" + "and to_char(I_CERT.REMOVAL_DATE, 'yyyymmdd') <= '"
                        + model.txtExpRemovalDtTo.ToString() + "' ";
            }

            if (!string.IsNullOrWhiteSpace(model.willingnessQp))
            {
                whereClause += "\r\n" + "\t" + "and I_APPL.WILLINGNESS_QP = '" + model.willingnessQp + "' ";
            }

            if (!string.IsNullOrWhiteSpace(model.interestedFSS))
            {
                whereClause += "\r\n" + "\t" + "and (I_APPL.INTERESTED_FSS = '" + model.interestedFSS + "' ";
                if ("I".Equals(model.interestedFSS))
                {
                    whereClause += "or I_APPL.INTERESTED_FSS IS NULL ";
                }
                whereClause += ") ";
            }
            //qp card
            if (model.QpIssueFrDate != null)
            {
                whereClause +=
                 "\r\n" + "\t" + "AND to_char(I_CERT.CARD_ISSUE_DATE, 'yyyymmdd') >= '"
                + model.QpIssueFrDate.ToString() + "' ";
            }
            if (model.QpIssueToDate != null)
            {
                whereClause +=
                 "\r\n" + "\t" + "AND to_char(I_CERT.CARD_ISSUE_DATE, 'yyyymmdd') <= '"
                + model.QpIssueToDate.ToString() + "' ";
            }

            if (model.QpExpiryFrDate != null)
            {
                whereClause +=
                 "\r\n" + "\t" + "AND to_char(I_CERT.CARD_EXPIRY_DATE, 'yyyymmdd') >= '"
                + model.QpExpiryFrDate.ToString() + "' ";
            }
            if (model.QpExpiryToDate != null)
            {
                whereClause +=
                 "\r\n" + "\t" + "AND to_char(I_CERT.CARD_EXPIRY_DATE, 'yyyymmdd') <= '"
                + model.QpExpiryToDate.ToString() + "' ";
            }
            if (model.qp_serial_no != null)
            {
                whereClause +=
                 "\r\n" + "\t" + "AND UPPER(I_CERT.CARD_SERIAL_NO) like '%" + model.qp_serial_no.ToUpper() + "%' ";
            }

            if (model.QpReturnFrDate != null)
            {
                whereClause +=
                 "\r\n" + "\t" + "AND to_char(I_CERT.CARD_RETURN_DATE, 'yyyymmdd') >= '"
                + model.QpReturnFrDate.ToString() + "' ";
            }
            if (model.QpReturnToDate != null)
            {
                whereClause +=
                 "\r\n" + "\t" + "AND to_char(I_CERT.CARD_RETURN_DATE, 'yyyymmdd') <= '"
                + model.QpReturnToDate.ToString() + "' ";
            }
            //UR
            if (registrationType == RegistrationConstant.REGISTRATION_TYPE_MWIA)
            {
                if (model.Disposal != null)
                {
                    whereClause += "\r\n" + "\t" + "AND to_char(I_APPL.DATE_OF_DISPOSAL, 'yyyymmdd') = '" + model.Disposal.ToString() + "' ";
                }
            }
            if (registrationType != null && !"".Equals(registrationType.Trim()))
            {
                registrationType = "'" + registrationType + "'";
                queryStr += "\r\n" + "\t" + " S_CAT.REGISTRATION_TYPE = " + registrationType
                            + "\r\n" + "\t" + " AND S_APP_FORM.REGISTRATION_TYPE = "
                            + registrationType + "  " + whereClause;
            }

            //UR
            //if (!string.IsNullOrWhiteSpace(case_clause))
            //{
            //    queryStr += case_clause;
            //}
            queryStr += " ORDER BY FILE_REF, CERT_NO ";
            using (EntitiesRegistration db = new EntitiesRegistration())
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
        private void printHeader(string c1, string c2, List<List<string>> header)
        {
            if (string.IsNullOrWhiteSpace(c1) || c1.Equals(""))
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(c2))
            {
                c2 = "";
            }
            List<string> column = null;
            List<string> case_clause = null;
            List<string> select_clause = null;
            List<string> allAnd_clause = null;

            if (header.Count > 0)
            {
                column = header.ElementAt(0);
                case_clause = header.ElementAt(1);
                select_clause = header.ElementAt(2);
                allAnd_clause = header.ElementAt(3);
            }
            else
            {
                column = new List<string>();
                case_clause = new List<string>();
                select_clause = new List<string>();
                allAnd_clause = new List<string>();
            }



            if (c1.Equals("AP"))
            {
                c2 = c2.Replace("(", "AP(");
                string[] itemList = c2.Split('\\', '|');
                string andOr = itemList[0];
                if (c2.IndexOf("AP") != -1)
                {
                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string colStr = item.Replace(")", "").Replace("(", "_");
                        column.Add(item);
                        case_clause.Add("CASE WHEN category = '" + item + "' THEN file_reference_no END AS " + colStr);
                        select_clause.Add("Max(" + colStr + ")");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(" + colStr + ") IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(" + colStr + ") IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    column.Add("AP(A)");
                    column.Add("AP(E)");
                    column.Add("AP(S)");
                    case_clause.Add("CASE WHEN category = 'AP(A)' THEN file_reference_no END AS AP_A");
                    case_clause.Add("CASE WHEN category = 'AP(E)' THEN file_reference_no END AS AP_E");
                    case_clause.Add("CASE WHEN category = 'AP(S)' THEN file_reference_no END AS AP_S");
                    select_clause.Add("Max(AP_A)");
                    select_clause.Add("Max(AP_E)");
                    select_clause.Add("Max(AP_S)");

                    allAnd_clause.Add(" (Max(AP_A) IS NOT NULL " + andOr + " Max(AP_E) IS NOT NULL " + andOr + " Max(AP_S) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("RSE"))
            {
                column.Add("RSE");
                case_clause.Add("CASE WHEN category = 'RSE' THEN file_reference_no END AS RSE");
                select_clause.Add("Max(RSE)");
                allAnd_clause.Add(" Max(RSE) IS NOT NULL ");
            }
            else if (c1.Equals("RGE"))
            {
                column.Add("RGE");
                case_clause.Add("CASE WHEN category = 'RGE' THEN file_reference_no END AS RGE");
                select_clause.Add("Max(RGE)");
                allAnd_clause.Add(" Max(RGE) IS NOT NULL ");
            }
            else if (c1.Equals("RI"))
            {
                c2 = c2.Replace("(", "RI(");
                string[] itemList = c2.Split('\\', '|');
                string andOr = itemList[0];
                if (c2.IndexOf("RI") != -1)
                {
                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string colStr = item.Replace(")", "").Replace("(", "_");
                        column.Add(item);
                        //from UR
                        column.Add("Status");
                        column.Add("Disciplines/Divisions");
                        case_clause.Add("CASE WHEN category = '" + item + "' THEN file_reference_no END AS " + colStr + ", STATUS, DISCIPLINES ");
                        select_clause.Add("Max(" + colStr + ")");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(" + colStr + ") IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(" + colStr + ") IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    column.Add("RI(A)");
                    column.Add("RI(E)");
                    column.Add("RI(S)");
                    case_clause.Add("CASE WHEN category = 'RI(A)' THEN file_reference_no END AS RI_A");
                    case_clause.Add("CASE WHEN category = 'RI(E)' THEN file_reference_no END AS RI_E");
                    case_clause.Add("CASE WHEN category = 'RI(S)' THEN file_reference_no END AS RI_S");
                    select_clause.Add("Max(RI_A)");
                    select_clause.Add("Max(RI_E)");
                    select_clause.Add("Max(RI_S)");

                    allAnd_clause.Add(" (Max(RI_A) IS NOT NULL " + andOr + " Max(RI_E) IS NOT NULL " + andOr + " Max(RI_S) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("RGBC - AS"))
            {
                column.Add("GBC - AS");
                case_clause.Add("CASE WHEN category = 'GBC' and role = 'AS' THEN file_reference_no END AS GBC_AS");
                select_clause.Add("Max(GBC_AS)");
                allAnd_clause.Add(" Max(GBC_AS) IS NOT NULL ");
            }
            else if (c1.Equals("RGBC - TD"))
            {
                column.Add("GBC - TD");
                case_clause.Add("CASE WHEN category = 'GBC' and role = 'TD' THEN file_reference_no END AS GBC_TD");
                select_clause.Add("Max(GBC_TD)");
                allAnd_clause.Add(" Max(GBC_TD) IS NOT NULL ");
            }
            else if (c1.Equals("SC - AS"))
            {
                c2 = c2.Replace("(", "SC(");
                string[] itemList = c2.Split('\\', '|');
                string andOr = itemList[0];
                if (c2.IndexOf("SC") != -1)
                {
                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string colStr = item.Replace(")", "").Replace("(", "");
                        column.Add(item + " - AS");
                        case_clause.Add("CASE WHEN category = '" + item + "' and role = 'AS' THEN file_reference_no END AS " + colStr + "_AS");
                        select_clause.Add("Max(" + colStr + "_AS)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(" + colStr + "_AS) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(" + colStr + "_AS) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    column.Add("SC(D) - AS");
                    column.Add("SC(F) - AS");
                    column.Add("SC(GI) - AS");
                    column.Add("SC(SF) - AS");
                    column.Add("SC(V) - AS");
                    case_clause.Add("CASE WHEN category = 'SC(D)' and role = 'AS' THEN file_reference_no END AS SCD_AS");
                    case_clause.Add("CASE WHEN category = 'SC(F)' and role = 'AS' THEN file_reference_no END AS SCF_AS");
                    case_clause.Add("CASE WHEN category = 'SC(GI)' and role = 'AS' THEN file_reference_no END AS SCGI_AS");
                    case_clause.Add("CASE WHEN category = 'SC(SF)' and role = 'AS' THEN file_reference_no END AS SCSF_AS");
                    case_clause.Add("CASE WHEN category = 'SC(V)' and role = 'AS' THEN file_reference_no END AS SCV_AS");
                    select_clause.Add("Max(SCD_AS)");
                    select_clause.Add("Max(SCF_AS)");
                    select_clause.Add("Max(SCGI_AS)");
                    select_clause.Add("Max(SCSF_AS)");
                    select_clause.Add("Max(SCV_AS)");

                    allAnd_clause.Add(" (Max(SCD_AS) IS NOT NULL " + andOr + " Max(SCF_AS) IS NOT NULL " + andOr + " Max(SCGI_AS) IS NOT NULL " + andOr + " Max(SCSF_AS) IS NOT NULL " + andOr + " Max(SCV_AS) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("SC - TD"))
            {
                c2 = c2.Replace("(", "SC(");
                string[] itemList = c2.Split('\\', '|');
                string andOr = itemList[0];
                if (c2.IndexOf("SC") != -1)
                {
                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string colStr = item.Replace(")", "").Replace("(", "");
                        column.Add(item + " - TD");
                        case_clause.Add("CASE WHEN category = '" + item + "' and role = 'TD' THEN file_reference_no END AS " + colStr + "_TD");
                        select_clause.Add("Max(" + colStr + "_TD)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(" + colStr + "_TD) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(" + colStr + "_TD) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    column.Add("SC(D) - TD");
                    column.Add("SC(F) - TD");
                    column.Add("SC(GI) - TD");
                    column.Add("SC(SF) - TD");
                    column.Add("SC(V) - TD");
                    case_clause.Add("CASE WHEN category = 'SC(D)' and role = 'TD' THEN file_reference_no END AS SCD_TD");
                    case_clause.Add("CASE WHEN category = 'SC(F)' and role = 'TD' THEN file_reference_no END AS SCF_TD");
                    case_clause.Add("CASE WHEN category = 'SC(GI)' and role = 'TD' THEN file_reference_no END AS SCGI_TD");
                    case_clause.Add("CASE WHEN category = 'SC(SF)' and role = 'TD' THEN file_reference_no END AS SCSF_TD");
                    case_clause.Add("CASE WHEN category = 'SC(V)' and role = 'TD' THEN file_reference_no END AS SCV_TD");
                    select_clause.Add("Max(SCD_TD)");
                    select_clause.Add("Max(SCF_TD)");
                    select_clause.Add("Max(SCGI_TD)");
                    select_clause.Add("Max(SCSF_TD)");
                    select_clause.Add("Max(SCV_TD)");

                    allAnd_clause.Add(" (Max(SCD_TD) IS NOT NULL " + andOr + " Max(SCF_TD) IS NOT NULL " + andOr + " Max(SCGI_TD) IS NOT NULL " + andOr + " Max(SCSF_TD) IS NOT NULL " + andOr + " Max(SCV_TD) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC(P) - AS - Class I, II, III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        column.Add("MWC(P) - Class I, II, III - " + item + " - AS");
                        case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 1' and mtype = '" + stype + "' THEN file_reference_no END AS MWCP_I_" + colStr + "_AS");
                        select_clause.Add("Max(MWCP_I_" + colStr + "_AS)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWCP_I_" + colStr + "_AS) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWCP_I_" + colStr + "_AS) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    column.Add("MWC(P) - Class I - (A) - AS");
                    column.Add("MWC(P) - Class I - (B) - AS");
                    column.Add("MWC(P) - Class I - (C) - AS");
                    column.Add("MWC(P) - Class I - (D) - AS");
                    column.Add("MWC(P) - Class I - (E) - AS");
                    column.Add("MWC(P) - Class I - (F) - AS");
                    column.Add("MWC(P) - Class I - (G) - AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 1' and mtype = 'Type A' THEN file_reference_no END AS MWCP_I_A_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 1' and mtype = 'Type B' THEN file_reference_no END AS MWCP_I_B_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 1' and mtype = 'Type C' THEN file_reference_no END AS MWCP_I_C_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 1' and mtype = 'Type D' THEN file_reference_no END AS MWCP_I_D_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 1' and mtype = 'Type E' THEN file_reference_no END AS MWCP_I_E_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 1' and mtype = 'Type F' THEN file_reference_no END AS MWCP_I_F_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 1' and mtype = 'Type G' THEN file_reference_no END AS MWCP_I_G_AS");
                    select_clause.Add("Max(MWCP_I_A_AS)");
                    select_clause.Add("Max(MWCP_I_B_AS)");
                    select_clause.Add("Max(MWCP_I_C_AS)");
                    select_clause.Add("Max(MWCP_I_D_AS)");
                    select_clause.Add("Max(MWCP_I_E_AS)");
                    select_clause.Add("Max(MWCP_I_F_AS)");
                    select_clause.Add("Max(MWCP_I_G_AS)");

                    allAnd_clause.Add(" (Max(MWCP_I_A_AS) IS NOT NULL " + andOr + " Max(MWCP_I_B_AS) IS NOT NULL " + andOr + " Max(MWCP_I_C_AS) IS NOT NULL " + andOr + " Max(MWCP_I_D_AS) IS NOT NULL " + andOr + " Max(MWCP_I_E_AS) IS NOT NULL " + andOr + " Max(MWCP_I_F_AS) IS NOT NULL " + andOr + " Max(MWCP_I_G_AS) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC(P) - AS - Class II, III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        column.Add("MWC(P) - Class II, III - " + item + " - AS");
                        case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 2' and mtype = '" + stype + "' THEN file_reference_no END AS MWCP_II_" + colStr + "_AS");
                        select_clause.Add("Max(MWCP_II_" + colStr + "_AS)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWCP_II_" + colStr + "_AS) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWCP_II_" + colStr + "_AS) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    column.Add("MWC(P) - Class II - (A) - AS");
                    column.Add("MWC(P) - Class II - (B) - AS");
                    column.Add("MWC(P) - Class II - (C) - AS");
                    column.Add("MWC(P) - Class II - (D) - AS");
                    column.Add("MWC(P) - Class II - (E) - AS");
                    column.Add("MWC(P) - Class II - (F) - AS");
                    column.Add("MWC(P) - Class II - (G) - AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 2' and mtype = 'Type A' THEN file_reference_no END AS MWCP_II_A_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 2' and mtype = 'Type B' THEN file_reference_no END AS MWCP_II_B_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 2' and mtype = 'Type C' THEN file_reference_no END AS MWCP_II_C_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 2' and mtype = 'Type D' THEN file_reference_no END AS MWCP_II_D_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 2' and mtype = 'Type E' THEN file_reference_no END AS MWCP_II_E_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 2' and mtype = 'Type F' THEN file_reference_no END AS MWCP_II_F_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 2' and mtype = 'Type G' THEN file_reference_no END AS MWCP_II_G_AS");
                    select_clause.Add("Max(MWCP_II_A_AS)");
                    select_clause.Add("Max(MWCP_II_B_AS)");
                    select_clause.Add("Max(MWCP_II_C_AS)");
                    select_clause.Add("Max(MWCP_II_D_AS)");
                    select_clause.Add("Max(MWCP_II_E_AS)");
                    select_clause.Add("Max(MWCP_II_F_AS)");
                    select_clause.Add("Max(MWCP_II_G_AS)");

                    allAnd_clause.Add(" (Max(MWCP_II_A_AS) IS NOT NULL " + andOr + " Max(MWCP_II_B_AS) IS NOT NULL " + andOr + " Max(MWCP_II_C_AS) IS NOT NULL " + andOr + " Max(MWCP_II_D_AS) IS NOT NULL " + andOr + " Max(MWCP_II_E_AS) IS NOT NULL " + andOr + " Max(MWCP_II_F_AS) IS NOT NULL " + andOr + " Max(MWCP_II_G_AS) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC(P) - AS - Class III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        column.Add("MWC(P) - Class III - " + item + " - AS");
                        case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 3' and mtype = '" + stype + "' THEN file_reference_no END AS MWCP_III_" + colStr + "_AS");
                        select_clause.Add("Max(MWCP_III_" + colStr + "_AS)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWCP_III_" + colStr + "_AS) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWCP_III_" + colStr + "_AS) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    column.Add("MWC(P) - Class III - (A) - AS");
                    column.Add("MWC(P) - Class III - (B) - AS");
                    column.Add("MWC(P) - Class III - (C) - AS");
                    column.Add("MWC(P) - Class III - (D) - AS");
                    column.Add("MWC(P) - Class III - (E) - AS");
                    column.Add("MWC(P) - Class III - (F) - AS");
                    column.Add("MWC(P) - Class III - (G) - AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 3' and mtype = 'Type A' THEN file_reference_no END AS MWCP_III_A_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 3' and mtype = 'Type B' THEN file_reference_no END AS MWCP_III_B_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 3' and mtype = 'Type C' THEN file_reference_no END AS MWCP_III_C_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 3' and mtype = 'Type D' THEN file_reference_no END AS MWCP_III_D_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 3' and mtype = 'Type E' THEN file_reference_no END AS MWCP_III_E_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 3' and mtype = 'Type F' THEN file_reference_no END AS MWCP_III_F_AS");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'AS' and Class = 'Class 3' and mtype = 'Type G' THEN file_reference_no END AS MWCP_III_G_AS");
                    select_clause.Add("Max(MWCP_III_A_AS)");
                    select_clause.Add("Max(MWCP_III_B_AS)");
                    select_clause.Add("Max(MWCP_III_C_AS)");
                    select_clause.Add("Max(MWCP_III_D_AS)");
                    select_clause.Add("Max(MWCP_III_E_AS)");
                    select_clause.Add("Max(MWCP_III_F_AS)");
                    select_clause.Add("Max(MWCP_III_G_AS)");

                    allAnd_clause.Add(" (Max(MWCP_III_A_AS) IS NOT NULL " + andOr + " Max(MWCP_III_B_AS) IS NOT NULL " + andOr + " Max(MWCP_III_C_AS) IS NOT NULL " + andOr + " Max(MWCP_III_D_AS) IS NOT NULL " + andOr + " Max(MWCP_III_E_AS) IS NOT NULL " + andOr + " Max(MWCP_III_F_AS) IS NOT NULL " + andOr + " Max(MWCP_III_G_AS) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC(P) - TD - Class I, II, III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        column.Add("MWC(P) - Class I, II, III - " + item + " - TD");
                        case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 1' and mtype = '" + stype + "' THEN file_reference_no END AS MWCP_I_" + colStr + "_TD");
                        select_clause.Add("Max(MWCP_I_" + colStr + "_TD)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWCP_I_" + colStr + "_TD) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWCP_I_" + colStr + "_TD) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    column.Add("MWC(P) - Class I, II, III - (A) - TD");
                    column.Add("MWC(P) - Class I, II, III - (B) - TD");
                    column.Add("MWC(P) - Class I, II, III - (C) - TD");
                    column.Add("MWC(P) - Class I, II, III - (D) - TD");
                    column.Add("MWC(P) - Class I, II, III - (E) - TD");
                    column.Add("MWC(P) - Class I, II, III - (F) - TD");
                    column.Add("MWC(P) - Class I, II, III - (G) - TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 1' and mtype = 'Type A' THEN file_reference_no END AS MWCP_I_A_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 1' and mtype = 'Type B' THEN file_reference_no END AS MWCP_I_B_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 1' and mtype = 'Type C' THEN file_reference_no END AS MWCP_I_C_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 1' and mtype = 'Type D' THEN file_reference_no END AS MWCP_I_D_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 1' and mtype = 'Type E' THEN file_reference_no END AS MWCP_I_E_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 1' and mtype = 'Type F' THEN file_reference_no END AS MWCP_I_F_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 1' and mtype = 'Type G' THEN file_reference_no END AS MWCP_I_G_TD");
                    select_clause.Add("Max(MWCP_I_A_TD)");
                    select_clause.Add("Max(MWCP_I_B_TD)");
                    select_clause.Add("Max(MWCP_I_C_TD)");
                    select_clause.Add("Max(MWCP_I_D_TD)");
                    select_clause.Add("Max(MWCP_I_E_TD)");
                    select_clause.Add("Max(MWCP_I_F_TD)");
                    select_clause.Add("Max(MWCP_I_G_TD)");

                    allAnd_clause.Add(" (Max(MWCP_I_A_TD) IS NOT NULL " + andOr + " Max(MWCP_I_B_TD) IS NOT NULL " + andOr + " Max(MWCP_I_C_TD) IS NOT NULL " + andOr + " Max(MWCP_I_D_TD) IS NOT NULL " + andOr + " Max(MWCP_I_E_TD) IS NOT NULL " + andOr + " Max(MWCP_I_F_TD) IS NOT NULL " + andOr + " Max(MWCP_I_G_TD) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC(P) - TD - Class II, III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        column.Add("MWC(P) - Class II, III - " + item + " - TD");
                        case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 2' and mtype = '" + stype + "' THEN file_reference_no END AS MWCP_II_" + colStr + "_TD");
                        select_clause.Add("Max(MWCP_II_" + colStr + "_TD)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWCP_II_" + colStr + "_TD) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWCP_II_" + colStr + "_TD) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    column.Add("MWC(P) - Class II, III - (A) - TD");
                    column.Add("MWC(P) - Class II, III - (B) - TD");
                    column.Add("MWC(P) - Class II, III - (C) - TD");
                    column.Add("MWC(P) - Class II, III - (D) - TD");
                    column.Add("MWC(P) - Class II, III - (E) - TD");
                    column.Add("MWC(P) - Class II, III - (F) - TD");
                    column.Add("MWC(P) - Class II, III - (G) - TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 2' and mtype = 'Type A' THEN file_reference_no END AS MWCP_II_A_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 2' and mtype = 'Type B' THEN file_reference_no END AS MWCP_II_B_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 2' and mtype = 'Type C' THEN file_reference_no END AS MWCP_II_C_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 2' and mtype = 'Type D' THEN file_reference_no END AS MWCP_II_D_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 2' and mtype = 'Type E' THEN file_reference_no END AS MWCP_II_E_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 2' and mtype = 'Type F' THEN file_reference_no END AS MWCP_II_F_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 2' and mtype = 'Type G' THEN file_reference_no END AS MWCP_II_G_TD");
                    select_clause.Add("Max(MWCP_II_A_TD)");
                    select_clause.Add("Max(MWCP_II_B_TD)");
                    select_clause.Add("Max(MWCP_II_C_TD)");
                    select_clause.Add("Max(MWCP_II_D_TD)");
                    select_clause.Add("Max(MWCP_II_E_TD)");
                    select_clause.Add("Max(MWCP_II_F_TD)");
                    select_clause.Add("Max(MWCP_II_G_TD)");

                    allAnd_clause.Add(" (Max(MWCP_II_A_TD) IS NOT NULL " + andOr + " Max(MWCP_II_B_TD) IS NOT NULL " + andOr + " Max(MWCP_II_C_TD) IS NOT NULL " + andOr + " Max(MWCP_II_D_TD) IS NOT NULL " + andOr + " Max(MWCP_II_E_TD) IS NOT NULL " + andOr + " Max(MWCP_II_F_TD) IS NOT NULL " + andOr + " Max(MWCP_II_G_TD) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC(P) - TD - Class III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        column.Add("MWC(P) - Class III - " + item + " - TD");
                        case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 3' and mtype = '" + stype + "' THEN file_reference_no END AS MWCP_III_" + colStr + "_TD");
                        select_clause.Add("Max(MWCP_III_" + colStr + "_TD)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWCP_III_" + colStr + "_TD) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWCP_III_" + colStr + "_TD) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    column.Add("MWC(P) - Class III - (A) - TD");
                    column.Add("MWC(P) - Class III - (B) - TD");
                    column.Add("MWC(P) - Class III - (C) - TD");
                    column.Add("MWC(P) - Class III - (D) - TD");
                    column.Add("MWC(P) - Class III - (E) - TD");
                    column.Add("MWC(P) - Class III - (F) - TD");
                    column.Add("MWC(P) - Class III - (G) - TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 3' and mtype = 'Type A' THEN file_reference_no END AS MWCP_III_A_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 3' and mtype = 'Type B' THEN file_reference_no END AS MWCP_III_B_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 3' and mtype = 'Type C' THEN file_reference_no END AS MWCP_III_C_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 3' and mtype = 'Type D' THEN file_reference_no END AS MWCP_III_D_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 3' and mtype = 'Type E' THEN file_reference_no END AS MWCP_III_E_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 3' and mtype = 'Type F' THEN file_reference_no END AS MWCP_III_F_TD");
                    case_clause.Add("CASE WHEN category = 'MWC(P)' and role = 'TD' and Class = 'Class 3' and mtype = 'Type G' THEN file_reference_no END AS MWCP_III_G_TD");
                    select_clause.Add("Max(MWCP_III_A_TD)");
                    select_clause.Add("Max(MWCP_III_B_TD)");
                    select_clause.Add("Max(MWCP_III_C_TD)");
                    select_clause.Add("Max(MWCP_III_D_TD)");
                    select_clause.Add("Max(MWCP_III_E_TD)");
                    select_clause.Add("Max(MWCP_III_F_TD)");
                    select_clause.Add("Max(MWCP_III_G_TD)");

                    allAnd_clause.Add(" (Max(MWCP_III_A_TD) IS NOT NULL " + andOr + " Max(MWCP_III_B_TD) IS NOT NULL " + andOr + " Max(MWCP_III_C_TD) IS NOT NULL " + andOr + " Max(MWCP_III_D_TD) IS NOT NULL " + andOr + " Max(MWCP_III_E_TD) IS NOT NULL " + andOr + " Max(MWCP_III_F_TD) IS NOT NULL " + andOr + " Max(MWCP_III_G_TD) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC - AS - Class I, II, III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        column.Add("MWC - Class I, II, III - " + item + " - AS");
                        case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 1' and mtype = '" + stype + "' THEN file_reference_no END AS MWC_I_" + colStr + "_AS");
                        select_clause.Add("Max(MWC_I_" + colStr + "_AS)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWC_I_" + colStr + "_AS) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWC_I_" + colStr + "_AS) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    column.Add("MWC - Class I, II, III - (A) - AS");
                    column.Add("MWC - Class I, II, III - (B) - AS");
                    column.Add("MWC - Class I, II, III - (C) - AS");
                    column.Add("MWC - Class I, II, III - (D) - AS");
                    column.Add("MWC - Class I, II, III - (E) - AS");
                    column.Add("MWC - Class I, II, III - (F) - AS");
                    column.Add("MWC - Class I, II, III - (G) - AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 1' and mtype = 'Type A' THEN file_reference_no END AS MWC_I_A_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 1' and mtype = 'Type B' THEN file_reference_no END AS MWC_I_B_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 1' and mtype = 'Type C' THEN file_reference_no END AS MWC_I_C_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 1' and mtype = 'Type D' THEN file_reference_no END AS MWC_I_D_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 1' and mtype = 'Type E' THEN file_reference_no END AS MWC_I_E_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 1' and mtype = 'Type F' THEN file_reference_no END AS MWC_I_F_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 1' and mtype = 'Type G' THEN file_reference_no END AS MWC_I_G_AS");
                    select_clause.Add("Max(MWC_I_A_AS)");
                    select_clause.Add("Max(MWC_I_B_AS)");
                    select_clause.Add("Max(MWC_I_C_AS)");
                    select_clause.Add("Max(MWC_I_D_AS)");
                    select_clause.Add("Max(MWC_I_E_AS)");
                    select_clause.Add("Max(MWC_I_F_AS)");
                    select_clause.Add("Max(MWC_I_G_AS)");

                    allAnd_clause.Add(" (Max(MWC_I_A_AS) IS NOT NULL " + andOr + " Max(MWC_I_B_AS) IS NOT NULL " + andOr + " Max(MWC_I_C_AS) IS NOT NULL " + andOr + " Max(MWC_I_D_AS) IS NOT NULL " + andOr + " Max(MWC_I_E_AS) IS NOT NULL " + andOr + " Max(MWC_I_F_AS) IS NOT NULL " + andOr + " Max(MWC_I_G_AS) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC - AS - Class II, III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        column.Add("MWC - Class II, III - " + item + " - AS");
                        case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 2' and mtype = '" + stype + "' THEN file_reference_no END AS MWC_II_" + colStr + "_AS");
                        select_clause.Add("Max(MWC_II_" + colStr + "_AS)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWC_II_" + colStr + "_AS) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWC_II_" + colStr + "_AS) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    column.Add("MWC - Class II, III - (A) - AS");
                    column.Add("MWC - Class II, III - (B) - AS");
                    column.Add("MWC - Class II, III - (C) - AS");
                    column.Add("MWC - Class II, III - (D) - AS");
                    column.Add("MWC - Class II, III - (E) - AS");
                    column.Add("MWC - Class II, III - (F) - AS");
                    column.Add("MWC - Class II, III - (G) - AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 2' and mtype = 'Type A' THEN file_reference_no END AS MWC_II_A_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 2' and mtype = 'Type B' THEN file_reference_no END AS MWC_II_B_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 2' and mtype = 'Type C' THEN file_reference_no END AS MWC_II_C_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 2' and mtype = 'Type D' THEN file_reference_no END AS MWC_II_D_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 2' and mtype = 'Type E' THEN file_reference_no END AS MWC_II_E_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 2' and mtype = 'Type F' THEN file_reference_no END AS MWC_II_F_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 2' and mtype = 'Type G' THEN file_reference_no END AS MWC_II_G_AS");
                    select_clause.Add("Max(MWC_II_A_AS)");
                    select_clause.Add("Max(MWC_II_B_AS)");
                    select_clause.Add("Max(MWC_II_C_AS)");
                    select_clause.Add("Max(MWC_II_D_AS)");
                    select_clause.Add("Max(MWC_II_E_AS)");
                    select_clause.Add("Max(MWC_II_F_AS)");
                    select_clause.Add("Max(MWC_II_G_AS)");

                    allAnd_clause.Add(" (Max(MWC_II_A_AS) IS NOT NULL " + andOr + " Max(MWC_II_B_AS) IS NOT NULL " + andOr + " Max(MWC_II_C_AS) IS NOT NULL " + andOr + " Max(MWC_II_D_AS) IS NOT NULL " + andOr + " Max(MWC_II_E_AS) IS NOT NULL " + andOr + " Max(MWC_II_F_AS) IS NOT NULL " + andOr + " Max(MWC_II_G_AS) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC - AS - Class III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        column.Add("MWC - Class III - " + item + " - AS");
                        case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 3' and mtype = '" + stype + "' THEN file_reference_no END AS MWC_III_" + colStr + "_AS");
                        select_clause.Add("Max(MWC_III_" + colStr + "_AS)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWC_III_" + colStr + "_AS) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWC_III_" + colStr + "_AS) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    column.Add("MWC - Class III - (A) - AS");
                    column.Add("MWC - Class III - (B) - AS");
                    column.Add("MWC - Class III - (C) - AS");
                    column.Add("MWC - Class III - (D) - AS");
                    column.Add("MWC - Class III - (E) - AS");
                    column.Add("MWC - Class III - (F) - AS");
                    column.Add("MWC - Class III - (G) - AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 3' and mtype = 'Type A' THEN file_reference_no END AS MWC_III_A_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 3' and mtype = 'Type B' THEN file_reference_no END AS MWC_III_B_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 3' and mtype = 'Type C' THEN file_reference_no END AS MWC_III_C_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 3' and mtype = 'Type D' THEN file_reference_no END AS MWC_III_D_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 3' and mtype = 'Type E' THEN file_reference_no END AS MWC_III_E_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 3' and mtype = 'Type F' THEN file_reference_no END AS MWC_III_F_AS");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'AS' and Class = 'Class 3' and mtype = 'Type G' THEN file_reference_no END AS MWC_III_G_AS");
                    select_clause.Add("Max(MWC_III_A_AS)");
                    select_clause.Add("Max(MWC_III_B_AS)");
                    select_clause.Add("Max(MWC_III_C_AS)");
                    select_clause.Add("Max(MWC_III_D_AS)");
                    select_clause.Add("Max(MWC_III_E_AS)");
                    select_clause.Add("Max(MWC_III_F_AS)");
                    select_clause.Add("Max(MWC_III_G_AS)");

                    allAnd_clause.Add(" (Max(MWC_III_A_AS) IS NOT NULL " + andOr + " Max(MWC_III_B_AS) IS NOT NULL " + andOr + " Max(MWC_III_C_AS) IS NOT NULL " + andOr + " Max(MWC_III_D_AS) IS NOT NULL " + andOr + " Max(MWC_III_E_AS) IS NOT NULL " + andOr + " Max(MWC_III_F_AS) IS NOT NULL " + andOr + " Max(MWC_III_G_AS) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC - TD - Class I, II, III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        column.Add("MWC - Class I, II, III - " + item + " - TD");
                        case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 1' and mtype = '" + stype + "' THEN file_reference_no END AS MWC_I_" + colStr + "_TD");
                        select_clause.Add("Max(MWC_I_" + colStr + "_TD)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWC_I_" + colStr + "_TD) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWC_I_" + colStr + "_TD) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    column.Add("MWC - Class I, II, III - (A) - TD");
                    column.Add("MWC - Class I, II, III - (B) - TD");
                    column.Add("MWC - Class I, II, III - (C) - TD");
                    column.Add("MWC - Class I, II, III - (D) - TD");
                    column.Add("MWC - Class I, II, III - (E) - TD");
                    column.Add("MWC - Class I, II, III - (F) - TD");
                    column.Add("MWC - Class I, II, III - (G) - TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 1' and mtype = 'Type A' THEN file_reference_no END AS MWC_I_A_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 1' and mtype = 'Type B' THEN file_reference_no END AS MWC_I_B_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 1' and mtype = 'Type C' THEN file_reference_no END AS MWC_I_C_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 1' and mtype = 'Type D' THEN file_reference_no END AS MWC_I_D_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 1' and mtype = 'Type E' THEN file_reference_no END AS MWC_I_E_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 1' and mtype = 'Type F' THEN file_reference_no END AS MWC_I_F_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 1' and mtype = 'Type G' THEN file_reference_no END AS MWC_I_G_TD");
                    select_clause.Add("Max(MWC_I_A_TD)");
                    select_clause.Add("Max(MWC_I_B_TD)");
                    select_clause.Add("Max(MWC_I_C_TD)");
                    select_clause.Add("Max(MWC_I_D_TD)");
                    select_clause.Add("Max(MWC_I_E_TD)");
                    select_clause.Add("Max(MWC_I_F_TD)");
                    select_clause.Add("Max(MWC_I_G_TD)");

                    allAnd_clause.Add(" (Max(MWC_I_A_TD) IS NOT NULL " + andOr + " Max(MWC_I_B_TD) IS NOT NULL " + andOr + " Max(MWC_I_C_TD) IS NOT NULL " + andOr + " Max(MWC_I_D_TD) IS NOT NULL " + andOr + " Max(MWC_I_E_TD) IS NOT NULL " + andOr + " Max(MWC_I_F_TD) IS NOT NULL " + andOr + " Max(MWC_I_G_TD) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC - TD - Class II, III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        column.Add("MWC - Class II, III - " + item + " - TD");
                        case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 2' and mtype = '" + stype + "' THEN file_reference_no END AS MWC_II_" + colStr + "_TD");
                        select_clause.Add("Max(MWC_II_" + colStr + "_TD)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWC_II_" + colStr + "_TD) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWC_II_" + colStr + "_TD) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    column.Add("MWC - Class II, III - (A) - TD");
                    column.Add("MWC - Class II, III - (B) - TD");
                    column.Add("MWC - Class II, III - (C) - TD");
                    column.Add("MWC - Class II, III - (D) - TD");
                    column.Add("MWC - Class II, III - (E) - TD");
                    column.Add("MWC - Class II, III - (F) - TD");
                    column.Add("MWC - Class II, III - (G) - TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 2' and mtype = 'Type A' THEN file_reference_no END AS MWC_II_A_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 2' and mtype = 'Type B' THEN file_reference_no END AS MWC_II_B_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 2' and mtype = 'Type C' THEN file_reference_no END AS MWC_II_C_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 2' and mtype = 'Type D' THEN file_reference_no END AS MWC_II_D_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 2' and mtype = 'Type E' THEN file_reference_no END AS MWC_II_E_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 2' and mtype = 'Type F' THEN file_reference_no END AS MWC_II_F_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 2' and mtype = 'Type G' THEN file_reference_no END AS MWC_II_G_TD");
                    select_clause.Add("Max(MWC_II_A_TD)");
                    select_clause.Add("Max(MWC_II_B_TD)");
                    select_clause.Add("Max(MWC_II_C_TD)");
                    select_clause.Add("Max(MWC_II_D_TD)");
                    select_clause.Add("Max(MWC_II_E_TD)");
                    select_clause.Add("Max(MWC_II_F_TD)");
                    select_clause.Add("Max(MWC_II_G_TD)");

                    allAnd_clause.Add(" (Max(MWC_II_A_TD) IS NOT NULL " + andOr + " Max(MWC_II_B_TD) IS NOT NULL " + andOr + " Max(MWC_II_C_TD) IS NOT NULL " + andOr + " Max(MWC_II_D_TD) IS NOT NULL " + andOr + " Max(MWC_II_E_TD) IS NOT NULL " + andOr + " Max(MWC_II_F_TD) IS NOT NULL " + andOr + " Max(MWC_II_G_TD) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC - TD - Class III"))
            {
                c2 = c2.Replace(")", "").Replace("(", "Type ");
                if (c2.IndexOf("Type") != -1)
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string stype = item.Replace(")", "").Replace("(", "Type ");
                        string colStr = item.Replace(")", "").Replace("(", "").Replace(" ", "_");
                        column.Add("MWC - Class III - " + item + " - TD");
                        case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 3' and mtype = '" + stype + "' THEN file_reference_no END AS MWC_III_" + colStr + "_TD");
                        select_clause.Add("Max(MWC_III_" + colStr + "_TD)");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWC_III_" + colStr + "_TD) IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWC_III_" + colStr + "_TD) IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    column.Add("MWC - Class III - (A) - TD");
                    column.Add("MWC - Class III - (B) - TD");
                    column.Add("MWC - Class III - (C) - TD");
                    column.Add("MWC - Class III - (D) - TD");
                    column.Add("MWC - Class III - (E) - TD");
                    column.Add("MWC - Class III - (F) - TD");
                    column.Add("MWC - Class III - (G) - TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 3' and mtype = 'Type A' THEN file_reference_no END AS MWC_III_A_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 3' and mtype = 'Type B' THEN file_reference_no END AS MWC_III_B_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 3' and mtype = 'Type C' THEN file_reference_no END AS MWC_III_C_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 3' and mtype = 'Type D' THEN file_reference_no END AS MWC_III_D_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 3' and mtype = 'Type E' THEN file_reference_no END AS MWC_III_E_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 3' and mtype = 'Type F' THEN file_reference_no END AS MWC_III_F_TD");
                    case_clause.Add("CASE WHEN category = 'MWC' and role = 'TD' and Class = 'Class 3' and mtype = 'Type G' THEN file_reference_no END AS MWC_III_G_TD");
                    select_clause.Add("Max(MWC_III_A_TD)");
                    select_clause.Add("Max(MWC_III_B_TD)");
                    select_clause.Add("Max(MWC_III_C_TD)");
                    select_clause.Add("Max(MWC_III_D_TD)");
                    select_clause.Add("Max(MWC_III_E_TD)");
                    select_clause.Add("Max(MWC_III_F_TD)");
                    select_clause.Add("Max(MWC_III_G_TD)");

                    allAnd_clause.Add(" (Max(MWC_III_A_TD) IS NOT NULL " + andOr + " Max(MWC_III_B_TD) IS NOT NULL " + andOr + " Max(MWC_III_C_TD) IS NOT NULL " + andOr + " Max(MWC_III_D_TD) IS NOT NULL " + andOr + " Max(MWC_III_E_TD) IS NOT NULL " + andOr + " Max(MWC_III_F_TD) IS NOT NULL " + andOr + " Max(MWC_III_G_TD) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("MWC(W)"))
            {
                if (c2.IndexOf("Item") != -1)
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];

                    string andOr_clause = "";
                    foreach (string item in itemList)
                    {
                        if (item.Equals("And") || item.Equals("Or"))
                        {
                            continue;
                        }
                        string sitem = item.Replace(" ", "_").Replace(".", "_");
                        column.Add("MWC(W) - " + item);
                        case_clause.Add("CASE WHEN category = 'MWC(W)' AND instr(item, '" + item + ",') > 0 THEN file_reference_no END AS MWC_W_" + sitem);
                        select_clause.Add("Max(MWC_W_" + sitem + ")");
                        if (andOr_clause.Equals(""))
                        {
                            andOr_clause += "(Max(MWC_W_" + sitem + ") IS NOT NULL ";
                        }
                        else
                        {
                            andOr_clause += andOr + " Max(MWC_W_" + sitem + ") IS NOT NULL ";
                        }
                    }
                    allAnd_clause.Add(andOr_clause + ")");
                }
                else
                {
                    string[] itemList = c2.Split('\\', '|');
                    string andOr = itemList[0];
                    //if(c2.Equals("") || c2.Equals("All") || null == c2){
                    column.Add("MWC(W) - Item 3.1");
                    column.Add("MWC(W) - Item 3.2");
                    column.Add("MWC(W) - Item 3.3");
                    column.Add("MWC(W) - Item 3.4");
                    column.Add("MWC(W) - Item 3.5");
                    column.Add("MWC(W) - Item 3.6");
                    column.Add("MWC(W) - Item 3.7");
                    column.Add("MWC(W) - Item 3.8");
                    column.Add("MWC(W) - Item 3.9");
                    column.Add("MWC(W) - Item 3.10");
                    column.Add("MWC(W) - Item 3.11");
                    column.Add("MWC(W) - Item 3.12");
                    column.Add("MWC(W) - Item 3.13");
                    column.Add("MWC(W) - Item 3.14");
                    column.Add("MWC(W) - Item 3.15");
                    column.Add("MWC(W) - Item 3.16");
                    column.Add("MWC(W) - Item 3.17");
                    column.Add("MWC(W) - Item 3.18");
                    column.Add("MWC(W) - Item 3.19");
                    column.Add("MWC(W) - Item 3.20");
                    column.Add("MWC(W) - Item 3.21");
                    column.Add("MWC(W) - Item 3.22");
                    column.Add("MWC(W) - Item 3.23");
                    column.Add("MWC(W) - Item 3.24");
                    column.Add("MWC(W) - Item 3.25");
                    column.Add("MWC(W) - Item 3.26");
                    column.Add("MWC(W) - Item 3.27");
                    column.Add("MWC(W) - Item 3.28");
                    column.Add("MWC(W) - Item 3.29");
                    column.Add("MWC(W) - Item 3.30");
                    column.Add("MWC(W) - Item 3.31");
                    column.Add("MWC(W) - Item 3.32");
                    column.Add("MWC(W) - Item 3.33");
                    column.Add("MWC(W) - Item 3.34");
                    column.Add("MWC(W) - Item 3.35");
                    column.Add("MWC(W) - Item 3.36");
                    column.Add("MWC(W) - Item 3.37");
                    column.Add("MWC(W) - Item 3.38");
                    column.Add("MWC(W) - Item 3.39");
                    column.Add("MWC(W) - Item 3.40");
                    column.Add("MWC(W) - Item 3.41");
                    column.Add("MWC(W) - Item 3.42");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.1,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_1");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.2,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_2");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.3,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_3");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.4,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_4");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.5,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_5");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.6,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_6");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.7,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_7");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.8,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_8");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.9,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_9");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.10,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_10");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.11,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_11");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.12,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_12");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.13,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_13");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.14,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_14");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.15,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_15");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.16,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_16");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.17,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_17");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.18,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_18");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.19,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_19");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.20,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_20");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.21,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_21");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.22,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_22");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.23,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_23");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.24,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_24");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.25,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_25");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.26,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_26");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.27,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_27");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.28,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_28");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.29,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_29");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.30,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_30");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.31,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_31");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.32,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_32");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.33,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_33");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.34,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_34");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.35,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_35");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.36,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_36");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.37,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_37");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.38,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_38");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.39,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_39");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.40,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_40");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.41,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_41");
                    case_clause.Add("CASE WHEN category = 'MWC(W)' and instr(item, 'Item 3.42,') > 0 THEN file_reference_no END AS MWCW_ITEM_3_42");
                    select_clause.Add("Max(MWCW_ITEM_3_1)");
                    select_clause.Add("Max(MWCW_ITEM_3_2)");
                    select_clause.Add("Max(MWCW_ITEM_3_3)");
                    select_clause.Add("Max(MWCW_ITEM_3_4)");
                    select_clause.Add("Max(MWCW_ITEM_3_5)");
                    select_clause.Add("Max(MWCW_ITEM_3_6)");
                    select_clause.Add("Max(MWCW_ITEM_3_7)");
                    select_clause.Add("Max(MWCW_ITEM_3_8)");
                    select_clause.Add("Max(MWCW_ITEM_3_9)");
                    select_clause.Add("Max(MWCW_ITEM_3_10)");
                    select_clause.Add("Max(MWCW_ITEM_3_11)");
                    select_clause.Add("Max(MWCW_ITEM_3_12)");
                    select_clause.Add("Max(MWCW_ITEM_3_13)");
                    select_clause.Add("Max(MWCW_ITEM_3_14)");
                    select_clause.Add("Max(MWCW_ITEM_3_15)");
                    select_clause.Add("Max(MWCW_ITEM_3_16)");
                    select_clause.Add("Max(MWCW_ITEM_3_17)");
                    select_clause.Add("Max(MWCW_ITEM_3_18)");
                    select_clause.Add("Max(MWCW_ITEM_3_19)");
                    select_clause.Add("Max(MWCW_ITEM_3_20)");
                    select_clause.Add("Max(MWCW_ITEM_3_21)");
                    select_clause.Add("Max(MWCW_ITEM_3_22)");
                    select_clause.Add("Max(MWCW_ITEM_3_23)");
                    select_clause.Add("Max(MWCW_ITEM_3_24)");
                    select_clause.Add("Max(MWCW_ITEM_3_25)");
                    select_clause.Add("Max(MWCW_ITEM_3_26)");
                    select_clause.Add("Max(MWCW_ITEM_3_27)");
                    select_clause.Add("Max(MWCW_ITEM_3_28)");
                    select_clause.Add("Max(MWCW_ITEM_3_29)");
                    select_clause.Add("Max(MWCW_ITEM_3_30)");
                    select_clause.Add("Max(MWCW_ITEM_3_31)");
                    select_clause.Add("Max(MWCW_ITEM_3_32)");
                    select_clause.Add("Max(MWCW_ITEM_3_33)");
                    select_clause.Add("Max(MWCW_ITEM_3_34)");
                    select_clause.Add("Max(MWCW_ITEM_3_35)");
                    select_clause.Add("Max(MWCW_ITEM_3_36)");
                    select_clause.Add("Max(MWCW_ITEM_3_37)");
                    select_clause.Add("Max(MWCW_ITEM_3_38)");
                    select_clause.Add("Max(MWCW_ITEM_3_39)");
                    select_clause.Add("Max(MWCW_ITEM_3_40)");
                    select_clause.Add("Max(MWCW_ITEM_3_41)");
                    select_clause.Add("Max(MWCW_ITEM_3_42)");
                    allAnd_clause.Add(" (Max(MWCW_ITEM_3_1) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_2) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_3) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_4) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_5) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_6) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_7) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_8) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_9) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_10) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_11) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_12) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_13) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_14) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_15) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_16) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_17) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_18) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_19) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_20) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_21) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_22) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_23) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_24) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_25) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_26) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_27) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_28) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_29) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_30) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_31) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_32) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_33) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_34) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_35) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_36) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_37) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_38) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_39) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_40) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_41) IS NOT NULL " + andOr + " Max(MWCW_ITEM_3_42) IS NOT NULL) ");
                }
            }
            else if (c1.Equals("name"))
            {
                select_clause.Add("name");
            }
            else if (c1.Equals("hkid"))
            {
                select_clause.Add("hkid");
            }
            header.Add(column);
            header.Add(case_clause);
            header.Add(select_clause);
            header.Add(allAnd_clause);
        }
        private List<List<object>> retrieveASTDOOInformation(CRMReportModel model, string registrationType)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();
            string queryStr = ""
                + "SELECT DISTINCT "
                + "S_CAT.CODE AS CATEGORY_CODE, "
                + "C_APPL.FILE_REFERENCE_NO AS FILE_REF, "
                + "C_APPL.ENGLISH_COMPANY_NAME AS E_COMP_NAME, "
                + "C_APPL.CHINESE_COMPANY_NAME AS C_COMP_NAME, "
                + "to_char(C_APPL.EXPIRY_DATE, 'dd/mm/yyyy') AS EXPIRY_DATE, "
                + "S_ROLE.CODE AS ROLE, "
                + "S_TITLE.ENGLISH_DESCRIPTION AS TITLE, "
                + "APP.SURNAME AS SURNAME, "
                + "APP.GIVEN_NAME_ON_ID AS GIVEN_NAME, "
                + "APP.CHINESE_NAME AS CH_NAME, "
                + "CASE WHEN   APP.HKID IS NOT NULL AND   APP.PASSPORT_NO  IS NOT NULL"
                + " THEN "
                + EncryptDecryptUtil.getDecryptSQL("APP.HKID") + " ||'/'|| "
                + EncryptDecryptUtil.getDecryptSQL("APP.PASSPORT_NO")
                + " ELSE "
                + EncryptDecryptUtil.getDecryptSQL("APP.HKID") + "||"
                + EncryptDecryptUtil.getDecryptSQL("APP.PASSPORT_NO")
                + " END AS HKID_PASSPORT_NO,"
                + "CAI.RES_ADDRESS_E1 AS E_RESIDENTIAL_ADDR1, "
                + "CAI.RES_ADDRESS_E2 AS E_RESIDENTIAL_ADDR2, "
                + "CAI.RES_ADDRESS_E3 AS E_RESIDENTIAL_ADDR3, "
                + "CAI.RES_ADDRESS_E4 AS E_RESIDENTIAL_ADDR4, "
                + "CAI.RES_ADDRESS_E5 AS E_RESIDENTIAL_ADDR5, "
                + "CAI.RES_ADDRESS_C1 AS C_RESIDENTIAL_ADDR1, "
                + "CAI.RES_ADDRESS_C2 AS C_RESIDENTIAL_ADDR2, "
                + "CAI.RES_ADDRESS_C3 AS C_RESIDENTIAL_ADDR3, "
                + "CAI.RES_ADDRESS_C4 AS C_RESIDENTIAL_ADDR4, "
                + "CAI.RES_ADDRESS_C5 AS C_RESIDENTIAL_ADDR5, "
                + "CAI.OFFICE_TEL AS OFFICE_TEL, "
                + "CAI.MOBILE_TEL AS MOBILE_TEL, "
                + "CAI.RES_TEL AS RESIDENTIAL_TEL, "
                + "CAI.EMAIL1 AS EMAIL1, "
                + "CAI.EMAIL2 AS EMAIL2, "
                + "S_STATUS.ENGLISH_DESCRIPTION AS STATUS, "
                + "CAI.ACCEPT_DATE AS DATE_OF_ACCEPTANCE, "
                + "CAI.REMOVAL_DATE AS DATE_OF_REMOVAL, "
                + "CAI.INTERVIEW_WITHDRAWN_DATE AS DATE_OF_WITHDRAWAL, "
                + "CAI.INTERVIEW_REFUSAL_DATE AS DATE_OF_REFUSAL, "
                + "CAI.CARD_ISSUE_DATE AS ISSUE_DATE_OF_QP_CARD, "
                + "CAI.CARD_EXPIRY_DATE AS EXPIRY_DATE_OF_QP_CARD, "
                + "CAI.CARD_SERIAL_NO AS SERIAL_NO_OF_QP_CARD, "
                + "CAI.CARD_RETURN_DATE AS RETURN_DATE_OF_QP_CARD, "
                + "CAI.REMARK AS REMARKS "
                + "FROM C_COMP_APPLICATION C_APPL "
                + "LEFT JOIN C_COMP_APPLICANT_INFO CAI ON C_APPL.UUID = CAI.MASTER_ID "
                + "LEFT JOIN C_APPLICANT APP ON APP.UUID = CAI.APPLICANT_ID "
                + "LEFT JOIN C_S_SYSTEM_VALUE S_ROLE ON CAI.APPLICANT_ROLE_ID = S_ROLE.UUID "
                + "LEFT JOIN C_S_SYSTEM_VALUE S_TITLE ON APP.TITLE_ID = S_TITLE.UUID "
                + "LEFT JOIN C_S_SYSTEM_VALUE S_STATUS ON CAI.APPLICANT_STATUS_ID = S_STATUS.UUID "
                + "INNER JOIN C_S_CATEGORY_CODE S_CAT ON S_CAT.UUID = C_APPL.CATEGORY_ID "
                + " where 1=1 ";
            if (!string.IsNullOrWhiteSpace(model.txtExpFileRef))
            {
                queryStr += "\r\n" + "\t" + "and UPPER(C_APPL.FILE_REFERENCE_NO) like '"
                            + model.txtExpFileRef + "%' ";
            }
            if (!string.IsNullOrWhiteSpace(model.as_ddCtrCode))
            {
                if ("ALL_GBC".Equals(model.as_ddCtrCode))
                {
                    queryStr += "AND (UPPER(S_CAT.CODE) like '%GBC%' ";
                    queryStr += "OR UPPER(S_CAT.CODE) like '%SC(D)%' ";
                    queryStr += "OR UPPER(S_CAT.CODE) like '%SC(F)%' ";
                    queryStr += "OR UPPER(S_CAT.CODE) like '%SC(GI)%' ";
                    queryStr += "OR UPPER(S_CAT.CODE) like '%SC(SF)%' ";
                    queryStr += "OR UPPER(S_CAT.CODE) like '%SC(V)%') ";
                }
                else if ("ALL_MWC".Equals(model.as_ddCtrCode))
                {
                    queryStr += "AND UPPER(S_CAT.CODE) like '%MWC%' ";
                }
                else
                {
                    queryStr += "AND UPPER(S_CAT.CODE) = '" + model.as_ddCtrCode.ToUpper() + "' ";
                }
            }
            if (!string.IsNullOrWhiteSpace(model.ExpASTOOFileRef))
            {
                queryStr += "AND UPPER(C_APPL.FILE_REFERENCE_NO) like '%" + model.ExpASTOOFileRef.ToUpper() + "%'";
            }
            if (model.CompExpiryFrDate != null)
            {
                queryStr +=
                 "AND to_char(C_APPL.EXPIRY_DATE, 'yyyymmdd') >= '"
                + model.CompExpiryFrDate.ToString() + "' ";
            }
            if (model.CompExpiryToDate != null)
            {
                queryStr += "AND to_char(C_APPL.EXPIRY_DATE, 'yyyymmdd') <= '"
                            + model.CompExpiryToDate.ToString() + "' ";
            }
            if (!string.IsNullOrWhiteSpace(model.Surname))
            {
                queryStr += "AND UPPER(APP.SURNAME) like '%" + model.Surname.ToUpper() + "%'";
            }
            if (!string.IsNullOrWhiteSpace(model.GivenName))
            {
                queryStr += "AND UPPER(APP.GIVEN_NAME_ON_ID) like '%" + model.GivenName.ToUpper() + "%'";
            }
            if (!string.IsNullOrWhiteSpace(model.ChineseName))
            {
                queryStr += "AND UPPER(APP.CHINESE_NAME) like '%" + model.ChineseName.ToUpper() + "%'";
            }
            if (!string.IsNullOrWhiteSpace(model.HKID_PASSPORT_DISPLAY))
            {
                queryStr += "AND (UPPER(" + EncryptDecryptUtil.getDecryptSQL("APP.PASSPORT_NO") + ") = '" + model.HKID_PASSPORT_DISPLAY.ToUpper() + "' " +
                        " OR UPPER(" + EncryptDecryptUtil.getDecryptSQL("APP.HKID") + ") =  '" + model.HKID_PASSPORT_DISPLAY.ToUpper() + "' )";
            }
            if (!string.IsNullOrWhiteSpace(model.RoleCode))
            {
                queryStr += "AND UPPER(S_ROLE.CODE) like '%" + model.RoleCode.ToUpper() + "%'";
            }
            if (!string.IsNullOrWhiteSpace(model.StatusDesc))
            {
                queryStr += "AND UPPER(S_STATUS.ENGLISH_DESCRIPTION) = '" + model.StatusDesc.ToUpper() + "'";
            }

            if (model.AcceptFrDate != null)
            {
                queryStr +=
                 "AND to_char(CAI.ACCEPT_DATE, 'yyyymmdd') >= '"
                + model.AcceptFrDate.ToString() + "' ";
            }
            if (model.AcceptToDate != null)
            {
                queryStr +=
                 "AND to_char(CAI.ACCEPT_DATE, 'yyyymmdd') <= '"
                + model.AcceptToDate.ToString() + "' ";
            }

            if (model.RemovalFrDate != null)
            {
                queryStr +=
                 "AND to_char(CAI.REMOVAL_DATE, 'yyyymmdd') >= '"
                + model.RemovalFrDate.ToString() + "' ";
            }
            if (model.RemovalToDate != null)
            {
                queryStr +=
                 "AND to_char(CAI.REMOVAL_DATE, 'yyyymmdd') <= '"
                + model.RemovalToDate.ToString() + "' ";
            }

            if (model.QpIssueFrDate != null)
            {
                queryStr +=
                 "AND to_char(CAI.CARD_ISSUE_DATE, 'yyyymmdd') >= '"
                + model.QpIssueFrDate + "' ";
            }
            if (model.QpIssueToDate != null)
            {
                queryStr +=
                 "AND to_char(CAI.CARD_ISSUE_DATE, 'yyyymmdd') <= '"
                + model.QpIssueToDate.ToString() + "' ";
            }

            if (model.QpExpiryFrDate != null)
            {
                queryStr +=
                 "AND to_char(CAI.CARD_EXPIRY_DATE, 'yyyymmdd') >= '"
                + model.QpExpiryFrDate.ToString() + "' ";
            }
            if (model.QpExpiryToDate != null)
            {
                queryStr +=
                 "AND to_char(CAI.CARD_EXPIRY_DATE, 'yyyymmdd') <= '"
                + model.QpExpiryToDate.ToString() + "' ";
            }
            if (!string.IsNullOrWhiteSpace(model.QpSerialNo))
            {
                queryStr +=
                 "AND UPPER(CAI.CARD_SERIAL_NO) like '%" + model.QpSerialNo.ToUpper() + "%' ";
            }

            if (model.QpReturnFrDate != null)
            {
                queryStr +=
                 "AND to_char(CAI.CARD_RETURN_DATE, 'yyyymmdd') >= '"
                + model.QpReturnFrDate.ToString() + "' ";
            }
            if (model.QpReturnToDate != null)
            {
                queryStr +=
                 "AND to_char(CAI.CARD_RETURN_DATE, 'yyyymmdd') <= '"
                + model.QpReturnToDate.ToString() + "' ";
            }
            queryStr += " ORDER BY C_APPL.FILE_REFERENCE_NO";
            using (EntitiesRegistration db = new EntitiesRegistration())
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
        public FileStreamResult ExportAPRSERGEExpInfoReport(CRMReportModel model, string registrationType)
        {
            List<string> headerList;
            List<List<object>> data = new List<List<object>>();
            //List<List<string>> header = new List<List<string>>();
            //string wherestr = "";
            //wherestr += processRegisteredPersonReportWhereClause(model.rc_applicationType1, model.rc_subType1);
            //printHeader(model.rc_applicationType1, model.rc_subType1, header);
            if (registrationType == RegistrationConstant.REGISTRATION_TYPE_MWIA)
            {
                headerList = new List<string>() {
                        "file_ref", "reg_no", "title",
                        "surname", "given_name", "chn_name",
                        "HKID/Passport No.", "Qualification/Education", "Trade",
                        //"e_home1", "e_home2", "e_home3", "e_home4", "e_home5",
                        //"c_home1", "c_home2","c_home3", "c_home4", "c_home5",
                        "c_o", "chn_c_o",
                        //"e_office1", "e_office2", "e_office3", "e_office4",
                        //"e_office5", "c_office1", "c_office2", "c_office3",
                        //"c_office4", "c_office5", "emrg_no1", "emrg_no2", "emrg_no3", 
                        // "tel1", "tel2", "tel3",
                        // "fax1", "fax2", 
                        //"email", 
                        "pnap", "qualification", "category_code",
                        "expiry_date", "form_used", "period_of_validity",
                        "app_status", "date_of_registration", "date_of_gazette",
                        "date_of_approval", "retention_application_submitted",
                        "retention_commenced", "restoration_application_submitted",
                        "restoration_commenced", "removed_from_register",
                        "extended_date_of_expiry","DATE_OF_DISPOSAL",
                        "eng_authority_name",
                        "chn_authority_name", "eng_authority_title",
                        "chn_authority_title", "letter_file_ref",
                        "Interested_in_Providing_Services_of_QP",
                        "Interested_in_Providing_Services_in_Fire_Safety",
                        "issue_date_of_QpCard","expiry_date_of_QpCard",
                        "serial_no_of_QpCard","return_date_of_QpCard"

                };
                //headerList.Add(wherestr);
                // add all option fields at the end
                if (model.checkboxAddr)
                {
                    List<string> optionFields = new List<string>() {
                        "e_home1", "e_home2", "e_home3", "e_home4", "e_home5",
                        "c_home1", "c_home2","c_home3", "c_home4", "c_home5",
                        "e_office1", "e_office2", "e_office3", "e_office4", "e_office5",
                        "c_office1", "c_office2", "c_office3", "c_office4", "c_office5"
                    };
                    headerList.AddRange(optionFields);
                }
                if (model.checkboxTel)
                {
                    List<string> optionFields = new List<string>() {
                        "tel1", "tel2", "tel3"
                    };
                    headerList.AddRange(optionFields);
                }
                if (model.checkboxEmail)
                {
                    headerList.Add("email");
                }
                if (model.checkboxFax)
                {
                    List<string> optionFields = new List<string>() {
                        "fax1", "fax2"
                    };
                    headerList.AddRange(optionFields);
                }
                if (model.checkboxEmergency)
                {
                    List<string> optionFields = new List<string>() {
                        "emrg_no1", "emrg_no2", "emrg_no3"
                    };
                    headerList.AddRange(optionFields);
                }
            }
            else
            {
                headerList = new List<string>() {
                        "file_ref", "reg_no", "title",
                        "surname", "given_name", "chn_name", "e_home1", "e_home2",
                        "e_home3", "e_home4", "e_home5", "c_home1", "c_home2",
                        "c_home3", "c_home4", "c_home5", "c_o", "chn_c_o",
                        "e_office1", "e_office2", "e_office3", "e_office4",
                        "e_office5", "c_office1", "c_office2", "c_office3",
                        "c_office4", "c_office5", "emrg_no1", "emrg_no2",
                        "emrg_no3", "tel1", "tel2", "tel3", "fax1", "fax2",
                        "email", "pnap", "qualification", "category_code",
                        "expiry_date", "form_used", "period_of_validity",
                        "app_status", "date_of_registration", "date_of_gazette",
                        "date_of_approval", "retention_application_submitted",
                        "retention_commenced", "restoration_application_submitted",
                        "restoration_commenced", "removed_from_register",
                        "extended_date_of_expiry",
                        "eng_authority_name",
                        "chn_authority_name", "eng_authority_title",
                        "chn_authority_title", "letter_file_ref",
                        "Interested_in_Providing_Services_of_QP",
                        "Interested_in_Providing_Services_in_Fire_Safety",
                        "issue_date_of_QpCard","expiry_date_of_QpCard",
                        "serial_no_of_QpCard","return_date_of_QpCard"
                };
                //headerList.Add(wherestr);
            }
            data = retrieveAPRSERGEInformation(model, registrationType);
            return this.exportExcelFile("appln_info_" + registrationType, headerList, data, "RESTRICTED");
        }
        public string processRegisteredPersonReportWhereClause(string c1, string c2)
        {
            string _where = "";
            if (string.IsNullOrWhiteSpace(c1) || c1.Equals("")) { return ""; }
            if (string.IsNullOrWhiteSpace(c2))
            {
                c2 = "";
            }


            if (/*c1 == */"AP".Equals(c1))
            {
                c2 = c2.Replace("(", "AP(");
                if (c2.IndexOf("AP") != -1)
                {
                    _where += c2;
                }
                else
                {
                    _where += "AP(A)AP(E)AP(S)";
                }
            }
            else if (/*c1 == */"RSE".Equals(c1))
            {
                _where += "RSE";
            }
            else if (/*c1 == */"RGE".Equals(c1))
            {
                _where += "RGE";
            }
            else if (/*c1 == */"RI".Equals(c1))
            {
                c2 = c2.Replace("(", "RI(");
                if (c2.IndexOf("RI") != -1)
                {
                    _where += c2;
                }
                else
                {
                    _where += "RI(A)RI(E)RI(S)";
                }
            }
            else if (/*c1 == */"RGBC - AS".Equals(c1))
            {
                _where += "GBC AS";
            }
            else if (c1 == "RGBC - TD")
            {
                _where += "GBC TD";
            }
            else if (c1 == "SC - AS")
            {
                c2 = c2.Replace("(", "SC(");
                if (c2.IndexOf("SC") != -1)
                {
                    _where += c2 + " AS";
                }
                else
                {
                    _where += "SC(D)SC(F)SC(GI)SC(SF)SC(V) AS";
                }
            }
            else if (/*c1 == */"SC - TD".Equals(c1))
            {
                c2 = c2.Replace("(", "SC(");
                if (c2.IndexOf("SC") != -1)
                {
                    _where += c2 + " TD";
                }
                else
                {
                    _where += "SC(D)SC(F)SC(GI)SC(SF)SC(V) TD";
                }
            }
            else if (/*c1 == */"MWC - AS - All Classes".Equals(c1))
            {
                _where += "MWC AS";
            }
            else if (/*c1 == */"MWC - AS - Class I, II, III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC Class 1 AS " + c2;
                }
                else
                {
                    _where += "MWC Class 1 AS ";
                }
            }
            else if (/*c1 == */"MWC - AS - Class II, III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC Class 2 AS " + c2;
                }
                else
                {
                    _where += "MWC Class 2 AS ";
                }
            }
            else if (/*c1 == */"MWC - AS - Class III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC Class 3 AS " + c2;
                }
                else
                {
                    _where += "MWC Class 3 AS ";
                }
            }
            else if (/*c1 == */"MWC - TD - Class I, II, III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC Class 1 TD " + c2;
                }
                else
                {
                    _where += "MWC Class 1 TD ";
                }
            }
            else if (/*c1 == */"MWC - TD - Class II, III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC Class 2 TD " + c2;
                }
                else
                {
                    _where += "MWC Class 2 TD ";
                }
            }
            else if (/*c1 == */"MWC - TD - Class III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC Class 3 TD " + c2;
                }
                else
                {
                    _where += "MWC Class 3 TD ";
                }
            }
            else if (/*c1 == */"MWC(P) - AS - All Classes".Equals(c1))
            {
                _where += "MWC(P) AS";
            }
            else if (/*c1 == */"MWC(P) - AS - Class I, II, III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC(P) Class 1 AS " + c2;
                }
                else
                {
                    _where += "MWC(P) Class 1 AS ";
                }
            }
            else if (/*c1 == */"MWC(P) - AS - Class II, III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC(P) Class 2 AS " + c2;
                }
                else
                {
                    _where += "MWC(P) Class 2 AS ";
                }
            }
            else if (/*c1 == */"MWC(P) - AS - Class III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC(P) Class 3 AS " + c2;
                }
                else
                {
                    _where += "MWC(P) Class 3 AS ";
                }
            }
            else if (/*c1 == */"MWC(P) - TD - Class I, II, III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC(P) Class 1 TD " + c2;
                }
                else
                {
                    _where += "MWC(P) Class 1 TD ";
                }
            }
            else if (/*c1 == */"MWC(P) - TD - Class II, III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC(P) Class 2 TD " + c2;
                }
                else
                {
                    _where += "MWC(P) Class 2 TD ";
                }
            }
            else if (/*c1 == */"MWC(P) - TD - Class III".Equals(c1))
            {
                c2 = c2.Replace("(", "Type ").Replace(")", "");
                if (c2.IndexOf("Type") != -1)
                {
                    _where += "MWC(P) Class 3 TD " + c2;
                }
                else
                {
                    _where += "MWC(P) Class 3 TD ";
                }
            }
            else
            {
                if (c2.IndexOf("Item") != -1)
                {
                    _where += "MWC(W) " + c2;
                }
                else
                {
                    _where += "MWC(W)";
                }
            }

            if (_where != "")
            {
                _where = "," + _where;
            }
            return _where;
        }
        public byte[] ExportApplicationStagesReport(Fn08ADM_ReportModel model)
        {
            List<string> headerList = new List<string>();
            List<string> headerList2 = new List<string>();
            List<List<object>> data = new List<List<object>>();
            List<List<object>> data2 = new List<List<object>>();
            List<string> category = new List<string>();
            List<FileStreamResult> FileList = new List<FileStreamResult>();

            //if (String.IsNullOrWhiteSpace(model.AllTypeOfApp))
            //{
            //    category.Add(RegistrationConstant.REGISTRATION_TYPE_IP);
            //    category.Add(RegistrationConstant.REGISTRATION_TYPE_CGA);
            //    category.Add(RegistrationConstant.REGISTRATION_TYPE_MWCA);
            //    category.Add(RegistrationConstant.REGISTRATION_TYPE_MWIA);

            //}
            if(model.CategoryIP)
            {
                category.Add(RegistrationConstant.REGISTRATION_TYPE_IP);
            }
            if(model.CategoryGBC)
            {
                category.Add(RegistrationConstant.REGISTRATION_TYPE_CGA);
            }
            if (model.CategoryMWC)
            {
                category.Add(RegistrationConstant.REGISTRATION_TYPE_MWCA);
            }
            if (model.CategoryMWI)
            {
                category.Add(RegistrationConstant.REGISTRATION_TYPE_MWIA);
            }


            if ("".Equals(category[0]) || category.Contains(RegistrationConstant.REGISTRATION_TYPE_IP))
            {
                data = GetApplicationStagesReportResultListInd(model, RegistrationConstant.REGISTRATION_TYPE_IP);
                headerList = new List<string>() {
                        "File Ref.",
                        "Name",
                        "Application Type",
                        "Cat.",
                        "Vetting Officer",
                        "Rec'd Date",
                        "Days Elapsed",
                        "Latest Date of Suppl. Document",
                        "Due Date",
                        "Result",
                        "Date of Result Letter",
                        "Case Status" };
                try
                {
                    //FileList.Add(exportExcelFile("Professional_Application", headerList, data));
                    FileList.Add(AdminIndExportExcelFile("Professional_Application", headerList, data, "Application Stage Report"));
                    //return this.exportExcelFile("Professional_Application", headerList, data);
                }
                catch (IOException e)
                {
                    Log.Fatal("Output error ", e);
                }
            }
            if ("".Equals(category[0]) || category.Contains(RegistrationConstant.REGISTRATION_TYPE_MWIA))
            {
                data = GetApplicationStagesReportResultListInd(model, RegistrationConstant.REGISTRATION_TYPE_MWIA);
                headerList = new List<string>() {
                        "File Ref.",
                        "Name",
                        "Application Type",
                        "Cat.",
                        "Vetting Officer",
                        "Rec'd Date",
                        "Days Elapsed",
                        "Latest Date of Suppl. Document",
                        "Due Date",
                        "Result",
                        "Date of Result Letter",
                        "Case Status" };
                try
                {
                    FileList.Add(AdminIndExportExcelFile("MW_Individual_Application", headerList, data, "Application Stage Report"));
                    //FileList.Add(exportExcelFile("MW_Individual_Application", headerList, data));
                    //return this.exportExcelFile("MW_Individual_Application", headerList, data);
                }
                catch (IOException e)
                {
                    Log.Fatal("Output error ", e);
                }
            }
            if ("".Equals(category[0]) || category.Contains(RegistrationConstant.REGISTRATION_TYPE_MWCA))
            {
                data = GetApplicationStagesReportResultListComp(model, RegistrationConstant.REGISTRATION_TYPE_MWCA);
                headerList = new List<string>()
                        {"File Ref.",
                        "Application Type",
                        "Role",
                        "Applicant Name",
                        "Vetting Officer",
                        "Rec'd Date",
                        "Days Elapsed",
                        "Due Date",
                        "Interview Date",
                        "Result",
                        "Date of Result Letter",
                        "Case Status",
                        "Remark" };
                try
                {
                    FileList.Add(AdminCompExportExcelFile("MW_Company_Application", headerList, data, "Application Stage Report"));
                    //return this.exportExcelFile("MW_Company_Application", headerList, data);
                }
                catch (IOException e)
                {
                    Log.Fatal("Output error ", e);
                }
            }
            if ("".Equals(category[0]) || category.Contains(RegistrationConstant.REGISTRATION_TYPE_CGA))
            {
                data = GetApplicationStagesReportResultListGCAComp(model, RegistrationConstant.REGISTRATION_TYPE_CGA);
                headerList = new List<string>()
                        {"File Ref.",
                        "Application Type",
                        "Cat.",
                        "Role",
                        "Applicant Name",
                        "Vetting Officer",
                        "Rec'd Date",
                        "Days Elapsed",
                        "Fast Track\n(for BA2A) ",
                        "Due Date",
                        "Interview Date",
                        "Result",
                        "Date of Result Letter",
                        "Case Status",
                        "Remark"};

                data2 = GetApplicationStagesReportResultListGCA2Comp(model, RegistrationConstant.REGISTRATION_TYPE_CGA);
                headerList2 = new List<string>()
                            {
                            "File Ref.",
                            "Application Type",
                            "Cat.",
                            "Nature",
                            "Role",
                            "Applicant Name",
                            "Vetting Officer",
                            "Rec'd Date",
                            "Days Elapsed",
                            "INITIAL CHECKING Letter [1]/[3] Pledge Date",
                            "INITIAL CHECKING Letter [1]/[3] Issue Date",
                            "Two Months Case",
                            "DETAILED CHECKING Letter [6] Pledge Date",
                            "DETAILED CHECKING Letter [6] Issue Date",
                            //"Two Months Case",
                            "DETAILED CHECKING Letter [8] Pledge Date",
                            "DETAILED CHECKING Letter [8] Issue Date",
                            "Date of CRC Mtg.",
                            "Result",
                            "Result Letter Pledge Date",
                            "Result Letter Issue Date",
                            "Case Status",
                            "Remark" };
                try
                {
                    FileList.Add(AdminExportGCAExcelFile("Comp_General_Contractor_Restoration_Renewal", headerList, data, "Application Stage Report"));
                    FileList.Add(AdminExportGCA2ExcelFile("Comp_General_Contractor_New_Addition", headerList2, data2, "Application Stage Report"));
                    //return this.exportExcelFile("MW_Individual_Application", headerList, data);
                }
                catch (IOException e)
                {
                    Log.Fatal("Output error ", e);
                }
            }

            byte[] fileBytes = null;
            using (var memoryStream = new MemoryStream())
            {
                using (ZipArchive zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var f in FileList)
                    {
                        ZipArchiveEntry zipItem = zip.CreateEntry(f.FileDownloadName);
                        using (Stream entryStream = zipItem.Open())
                        {
                            f.FileStream.CopyTo(entryStream);
                        }
                    }
                }
                fileBytes = memoryStream.ToArray();
            }
            return fileBytes;

            //return CreateZipFileFileStreamResult(FileList, "Admin");
        }



        private List<List<object>> GetApplicationStagesReportResultListInd(Fn08ADM_ReportModel model, string category)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();
            string where_q = "";
            string orderby = "";
            string where_2q = "";
            if (!string.IsNullOrWhiteSpace(model.fileReference))
            {
                where_q += "\r\n" + " AND upper(ia.file_reference_no) like upper(:fileRef) ";
                queryParameters.Add("fileRef", "%" + model.fileReference + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.VettingOfficer))
            {
                where_q += "\r\n" + " AND upper(i.vetting_officer) in (:vettingOfficer) ";
                queryParameters.Add("vettingOfficer", model.VettingOfficer);
            }

            if (model.received_fr_date != null)
            {
                where_q += "\r\n" + " AND i.received_date >= :receivedDateFrom ";
                queryParameters.Add("receivedDateFrom", model.received_fr_date);
            }
            if (model.received_to_date != null)
            {
                where_q += "\r\n" + " AND i.received_date <= :receivedDateTo ";
                queryParameters.Add("receivedDateTo", model.received_to_date);
            }
            if (model.due_fr_date != null)
            {
                where_q += "\r\n" + " AND i.due_date >= :dueDateFrom ";
                queryParameters.Add("dueDateFrom", model.due_fr_date);
            }
            if (model.due_to_date != null)
            {
                where_q += "\r\n" + " AND i.due_date <= :dueDateTo ";
                queryParameters.Add("dueDateTo", model.due_to_date);
            }
            if (model.interview_fr_date != null)
            {
                where_q += "\r\n" + " AND i.interview_date >= :interviewDateFrom ";
                queryParameters.Add("interviewDateFrom", model.interview_fr_date);
            }
            if (model.interview_to_date != null)
            {
                where_q += "\r\n" + " AND i.interview_date <= :interviewDateTo ";
                queryParameters.Add("interviewDateTo", model.interview_to_date);
            }
            if (model.CaseStatus != null)
            {
                where_2q += "\r\n" + " AND status = :caseStatus ";
                queryParameters.Add("caseStatus", model.CaseStatus);
            }

            where_q += "\r\n" + " AND scat.registration_type = :category ";
            queryParameters.Add("category", category);

            //if (model.arrTypeOfApplication !=null)
            //{
            //    queryStr += "\r\n" + " AND application_type in (:typeOfApplicationList) and registration_type = :category ";
            //    queryParameters.Add("typeOfApplicationList", model.arrTypeOfApplication);
            //    queryParameters.Add("category", category);
            //}

            orderby += "\r\n" + "order by received_date desc, file_reference_no ";

            string queryStr = "" +
                            "\r\n" + "select * from (                                                                                       " +
                            "\r\n" + " SELECT                                                                                                " +
                            "\r\n" + " ia.FILE_REFERENCE_NO as file_reference_no,                                                            " +
                            "\r\n" + " a.SURNAME || ' ' || a.GIVEN_NAME_ON_ID AS name,                                                       " +
                            "\r\n" + " ssv.CODE as application_type,                                                                         " +
                            "\r\n" + " scat.code AS category,                                                                                " +
                            "\r\n" + " i.VETTING_OFFICER as vetting_officer,                                                                 " +
                            "\r\n" + " i.RECEIVED_DATE as received_date,                                                                     " +
                            "\r\n" + " '' as Days_Elapsed,                                                                                   " +
                            "\r\n" + " i.SUPPLE_DOCUMENT_DATE as supple_document_date,                                                       " +
                            "\r\n" + " i.DUE_DATE as due_date,                                                                               " +
                            "\r\n" + " case                                                                                                  " +
                            "\r\n" + " when i.RESULT_ACCEPT_DATE is not null then 'ACCEPTED'                                                 " +
                            "\r\n" + " when i.RESULT_DEFER_DATE is not null then 'DEFERRED'                                                  " +
                            "\r\n" + " when i.RESULT_REFUSE_DATE is not null then 'REFUSED'                                                  " +
                            "\r\n" + " when i.WITHDRAWAL_DATE is not null then 'WITHDRAWN'                                                   " +
                            "\r\n" + " when i.OS_DATE is not null then 'OS/ LETTER ISSUED'                                                   " +
                            "\r\n" + " else null end AS SResult,                                                                             " +
                            "\r\n" + " case                                                                                                  " +
                            "\r\n" + " when i.RESULT_ACCEPT_DATE is not null then i.RESULT_ACCEPT_DATE                                       " +
                            "\r\n" + " when i.RESULT_DEFER_DATE is not null then i.RESULT_DEFER_DATE                                         " +
                            "\r\n" + " when i.RESULT_REFUSE_DATE is not null then i.RESULT_REFUSE_DATE                                       " +
                            "\r\n" + " when i.WITHDRAWAL_DATE is not null then i.WITHDRAWAL_DATE                                             " +
                            "\r\n" + " when i.OS_DATE is not null then i.OS_DATE                                                             " +
                            "\r\n" + " else null end AS SResult_Date,                                                                        " +
                            "\r\n" + " CASE                                                                                                  " +
                            "\r\n" + " WHEN i.RESULT_ACCEPT_DATE IS NOT NULL OR                                                              " +
                            "\r\n" + " i.RESULT_DEFER_DATE IS NOT NULL OR                                                                    " +
                            "\r\n" + " i.RESULT_REFUSE_DATE IS NOT NULL OR                                                                   " +
                            "\r\n" + " i.DEFER_DATE IS NOT NULL OR                                                                           " +
                            "\r\n" + " i.NOTIFICATION_LETTER_DUEDATE IS NOT NULL OR                                                          " +
                            "\r\n" + " i.WITHDRAWAL_DATE IS NOT NULL OR                                                                      " +
                            "\r\n" + " i.OS_DATE IS NOT NULL                                                                                 " +
                            "\r\n" + " THEN 'Completed'                                                                                      " +
                            "\r\n" + " WHEN i.DUE_DATE >= CURRENT_DATE THEN 'In Progress'                                                    " +
                            "\r\n" + " WHEN i.DUE_DATE < CURRENT_DATE THEN 'Overdue'                                                         " +
                            "\r\n" + " ELSE 'In Progress'                                                                                    " +
                            "\r\n" + " END AS status                                                                                         " +
                            //"\r\n" + " scat.registration_type as registration_type,                                                          " +
                            //"\r\n" + " i.INTERVIEW_DATE                                                                                      " +
                            "\r\n" + " FROM C_IND_PROCESS_MONITOR i                                                                          " +
                            "\r\n" + " LEFT JOIN C_IND_APPLICATION ia ON ia.UUID = i.MASTER_ID                                               " +
                            "\r\n" + " LEFT JOIN C_APPLICANT a ON a.UUID = ia.APPLICANT_ID                                                   " +
                            "\r\n" + " LEFT JOIN C_S_SYSTEM_VALUE ssv ON ssv.UUID = i.APPLICATION_FORM_ID                                    " +
                            "\r\n" + " LEFT JOIN C_S_CATEGORY_CODE scat ON scat.UUID = i.CATEGORY_ID                                         " +
                            "\r\n" + " LEFT JOIN C_IND_CERTIFICATE icer ON icer.MASTER_ID = i.MASTER_ID AND icer.CATEGORY_ID = i.CATEGORY_ID " +
                            where_q +
                            "\r\n" + " )                                                                                                     " +
                            "\r\n" + " WHERE 1=1                                                                                             " +
                            where_2q +
                            orderby
                            ;



            using (EntitiesRegistration db = new EntitiesRegistration())
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
        private List<List<object>> GetApplicationStagesReportResultListComp(Fn08ADM_ReportModel model, string category)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();
            string whereQ = "";
            string orderby = "";
            string where2Q = "";
            if (!string.IsNullOrWhiteSpace(model.fileReference))
            {
                whereQ += " AND upper(file_reference_no) like upper(:fileRef) ";
                queryParameters.Add("fileRef", "%" + model.fileReference + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.VettingOfficer))
            {
                whereQ += " AND upper(vetting_officer) in (:vettingOfficer) ";
                queryParameters.Add("vettingOfficer", model.VettingOfficer);
            }

            if (model.received_fr_date != null)
            {
                whereQ += "\r\n" + " AND received_date >= :receivedDateFrom ";
                queryParameters.Add("receivedDateFrom", model.received_fr_date);
            }
            if (model.received_to_date != null)
            {
                whereQ += "\r\n" + " AND received_date <= :receivedDateTo ";
                queryParameters.Add("receivedDateTo", model.received_to_date);
            }
            if (model.due_fr_date != null)
            {
                whereQ += "\r\n" + " AND due_date >= :dueDateFrom ";
                queryParameters.Add("dueDateFrom", model.due_fr_date);
            }
            if (model.due_to_date != null)
            {
                whereQ += "\r\n" + " AND due_date <= :dueDateTo ";
                queryParameters.Add("dueDateTo", model.due_to_date);
            }
            if (model.interview_fr_date != null)
            {
                whereQ += "\r\n" + " AND interview_date >= :interviewDateFrom ";
                queryParameters.Add("interviewDateFrom", model.interview_fr_date);
            }
            if (model.interview_to_date != null)
            {
                whereQ += "\r\n" + " AND interview_date <= :interviewDateTo ";
                queryParameters.Add("interviewDateTo", model.interview_to_date);
            }

            where2Q += " AND scat.registration_type = :category ";
            queryParameters.Add("category", category);

            //if (model.arrTypeOfApplication != null)
            //{
            //    queryStr += "\r\n" + " AND application_type in (:typeOfApplicationList) and registration_type = :category ";
            //    queryParameters.Add("typeOfApplicationList", model.arrTypeOfApplication);
            //    queryParameters.Add("category", category);
            //}

            orderby += "order by received_date desc, file_reference_no";

            string queryStr = "" +
                              //"\r\n" + " select * from (SELECT " +
                              "\r\n" + " select FILE_REFERENCE_NO,application_type,role,appName,VETTING_OFFICER," +
                              "\r\n" + " RECEIVED_DATE,Days_Elapsed,DUE_DATE,INTERVIEW_DATE,app_status,Date_of_result_letter,case_status,REMARKS " +
                              "\r\n" + " from (SELECT " +
                              "\r\n" + " ca.FILE_REFERENCE_NO as FILE_REFERENCE_NO, " +
                              "\r\n" + " ssv.CODE as application_type, " +
                              "\r\n" + " c.VETTING_OFFICER AS VETTING_OFFICER, " +
                              "\r\n" + " c.RECEIVED_DATE AS RECEIVED_DATE, " +
                              //new add 0909201
                              "\r\n" + " '' AS Days_Elapsed, " +
                              "\r\n" + " c.DUE_DATE AS DUE_DATE, " +
                              "\r\n" + " c.INTERVIEW_DATE as INTERVIEW_DATE, " +//5
                              "\r\n" + " '' as SRESULT, " +
                              "\r\n" + " status.ENGLISH_DESCRIPTION AS app_status, " +
                              "\r\n" + " CASE " +
                              "\r\n" + " WHEN c.result_letter_date IS null THEN c.withdraw_date " +
                              "\r\n" + " WHEN c.withdraw_date IS null THEN c.result_letter_date " +
                              "\r\n" + " WHEN c.result_letter_date>c.withdraw_date THEN c.result_letter_date " +
                              "\r\n" + " WHEN c.result_letter_date<c.withdraw_date THEN c.withdraw_date " +
                              "\r\n" + " ELSE NULL " +
                              "\r\n" + " END AS Date_of_result_letter, " +
                              "\r\n" + " CASE " +
                              "\r\n" + " WHEN c.RESULT_LETTER_DATE IS NOT NULL OR c.WITHDRAW_DATE IS NOT NULL THEN 'Completed' " +
                              "\r\n" + " ELSE '' " +
                              "\r\n" + " END AS case_status, " +
                              "\r\n" + " c.REMARKS as REMARKS, " +
                              "\r\n" + " CASE WHEN (" +
                              "\r\n" + " SELECT count(c1.uuid) from c_comp_process_monitor c1 " +
                              "\r\n" + " where c.master_id = c1.master_id " +
                              "\r\n" + " and c.received_date = c1.received_date " +
                              "\r\n" + " and c1.monitor_type='FaskTrack' " +
                              "\r\n" + " and c.monitor_type='UPM' " +
                              "\r\n" + " and c1.fast_trck='Y' " +
                              "\r\n" + " )>0 THEN 'Y' " +
                              "\r\n" + " ELSE 'N' END AS FastTrack, " +//10
                              "\r\n" + " CASE WHEN (" +
                              "\r\n" + " SELECT count(c1.uuid) from c_comp_process_monitor c1 " +
                              "\r\n" + " where c.master_id = c1.master_id " +
                              "\r\n" + " and c.received_date = c1.received_date " +
                              "\r\n" + " and c1.monitor_type='FaskTrack' " +
                              "\r\n" + " and c.monitor_type='UPM' " +
                              "\r\n" + " and c1.fast_trck='Y' " +
                              "\r\n" + " )>0 THEN c.RECEIVED_DATE+28 " +
                              "\r\n" + " ELSE ADD_MONTHS(c.RECEIVED_DATE, 3) END AS Due_date_cgc, " +
                              "\r\n" + " '' as letter1_issue_date, " +
                              "\r\n" + " C_GET_AFTER_WORKING_DAY(c.RECEIVED_DATE , 10 )  as pledge_date, " +
                              "\r\n" + " c.Two_month_case, " +
                              "\r\n" + " CASE " +
                              "\r\n" + " WHEN c.two_month_case='Y' " +
                              "\r\n" + " THEN 'N.A.' " +
                              "\r\n" + " ELSE to_char(add_months(c.RECEIVED_DATE ,2),'dd/mm/yyyy') " +
                              "\r\n" + " END as LETTER6_PLEDGE_DATE, " +//15
                              "\r\n" + " c.DATE_OF_LETTER_6, " +
                              "\r\n" + " CASE " +
                              "\r\n" + " WHEN c.two_month_case='Y' " +
                              "\r\n" + " THEN add_months(c.RECEIVED_DATE ,1) " +
                              "\r\n" + " ELSE add_months(c.RECEIVED_DATE ,4) " +
                              "\r\n" + " END as LETTER8_PLEDGE_DATE, " +
                              "\r\n" + " c.DATE_OF_LETTER_8, " +
                              "\r\n" + " add_months(C.INTERVIEW_DATE ,3) as RESULT_LETTER_PLEDGE_DATE, " +
                              "\r\n" + " CASE " +
                              "\r\n" + " WHEN c.WITHDRAW_DATE is NULL THEN c.RESULT_LETTER_DATE " +
                              "\r\n" + " WHEN c.RESULT_LETTER_DATE is NULL THEN c.WITHDRAW_DATE " +
                              "\r\n" + " WHEN c.WITHDRAW_DATE  >= c.RESULT_LETTER_DATE  THEN c.WITHDRAW_DATE " +
                              "\r\n" + " WHEN c.WITHDRAW_DATE  < c.RESULT_LETTER_DATE  THEN add_months(C.INTERVIEW_DATE ,3) " +
                              "\r\n" + " ELSE NULL " +
                              "\r\n" + " END AS result_letter_issue_date, " +//20
                              "\r\n" + " scat.code AS category, " +
                              "\r\n" + " C.NATURE, " +
                              "\r\n" + " C.MONITOR_TYPE, " +
                              "\r\n" + " scat.registration_type as registration_type, " +
                              "\r\n" + " c.APPLY_STATUS as apply_status, " + //25
                              "\r\n" + " (SELECT max(c2.PLEDGE_INITIAL_DATE) from c_comp_process_monitor c2 " +
                              "\r\n" + " where c.master_id = c2.master_id " +
                              "\r\n" + " and c.received_date = c2.received_date " +
                              "\r\n" + " and c2.monitor_type='UPM_10DAYS' " +
                              "\r\n" + " and c.monitor_type='UPM') as pledge_issue_date, " +
                              "\r\n" + " ssv2.CODE as role, " +
                              "\r\n" + " CASE " +
                              "\r\n" + " WHEN c.RESULT_LETTER_DATE IS NOT NULL OR c.WITHDRAW_DATE IS NOT NULL THEN 'Completed' " +
                              "\r\n" + " ELSE '' " +
                              "\r\n" + " END AS case_status_new " +
                              "\r\n" + " , apt.surname ||' '|| apt.given_name_on_id as appName " +
                              "\r\n" + " FROM C_COMP_PROCESS_MONITOR c " +
                              "\r\n" + " LEFT JOIN C_COMP_APPLICATION ca ON ca.UUID = c.MASTER_ID " +
                              "\r\n" + " LEFT JOIN C_S_SYSTEM_VALUE ssv ON ssv.UUID = c.APPLICATION_FORM_ID " +
                              "\r\n" + " LEFT JOIN C_S_SYSTEM_VALUE status ON c.INTERVIEW_RESULT_ID = status.UUID " +
                              "\r\n" + " LEFT JOIN C_S_CATEGORY_CODE scat ON scat.UUID = ca.CATEGORY_ID " +
                              "\r\n" + " LEFT JOIN C_S_SYSTEM_VALUE scatgrp ON scatgrp.UUID = scat.CATEGORY_GROUP_ID " +
                              "\r\n" + " INNER JOIN C_COMP_APPLICANT_INFO cai ON cai.UUID = c.COMPANY_APPLICANTS_ID " +
                              "\r\n" + " INNER JOIN C_S_SYSTEM_VALUE ssv2 ON ssv2.UUID = cai.APPLICANT_ROLE_ID " +
                              "\r\n" + " LEFT JOIN C_APPLICANT apt ON cai.APPLICANT_ID = apt.UUID" +
                              where2Q +
                              "\r\n" + ") " +
                              "\r\n" + " WHERE 1=1 and MONITOR_TYPE= 'UPM' " +
                              "\r\n" + " And category = 'MWC%' " //Add by bill 20191126
                              + whereQ
                              + orderby
                              ;



            using (EntitiesRegistration db = new EntitiesRegistration())
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
        private List<List<object>> GetApplicationStagesReportResultListGCAComp(Fn08ADM_ReportModel model, string category)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();
            string whereQ = "";
            string orderby = "";
            string where2Q = "";
            if (!string.IsNullOrWhiteSpace(model.fileReference))
            {
                whereQ += " AND upper(file_reference_no) like upper(:fileRef) ";
                queryParameters.Add("fileRef", "%" + model.fileReference + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.VettingOfficer))
            {
                whereQ += " AND upper(vetting_officer) in (:vettingOfficer) ";
                queryParameters.Add("vettingOfficer", model.VettingOfficer);
            }

            if (model.received_fr_date != null)
            {
                whereQ += "\r\n" + " AND received_date >= :receivedDateFrom ";
                queryParameters.Add("receivedDateFrom", model.received_fr_date);
            }
            if (model.received_to_date != null)
            {
                whereQ += "\r\n" + " AND received_date <= :receivedDateTo ";
                queryParameters.Add("receivedDateTo", model.received_to_date);
            }
            if (model.due_fr_date != null)
            {
                whereQ += "\r\n" + " AND due_date >= :dueDateFrom ";
                queryParameters.Add("dueDateFrom", model.due_fr_date);
            }
            if (model.due_to_date != null)
            {
                whereQ += "\r\n" + " AND due_date <= :dueDateTo ";
                queryParameters.Add("dueDateTo", model.due_to_date);
            }
            if (model.interview_fr_date != null)
            {
                whereQ += "\r\n" + " AND interview_date >= :interviewDateFrom ";
                queryParameters.Add("interviewDateFrom", model.interview_fr_date);
            }
            if (model.interview_to_date != null)
            {
                whereQ += "\r\n" + " AND interview_date <= :interviewDateTo ";
                queryParameters.Add("interviewDateTo", model.interview_to_date);
            }

            where2Q += " AND scat.registration_type = :category ";
            queryParameters.Add("category", category);

            //if (model.arrTypeOfApplication != null)
            //{
            //    queryStr += "\r\n" + " AND application_type in (:typeOfApplicationList) and registration_type = :category ";
            //    queryParameters.Add("typeOfApplicationList", model.arrTypeOfApplication);
            //    queryParameters.Add("category", category);
            //}

            orderby += "order by received_date desc, file_reference_no";

            string queryStr = "" +
                              //"\r\n" + " select * from (SELECT " +
                              "\r\n" + " select FILE_REFERENCE_NO,application_type,role,appName,VETTING_OFFICER," +
                              "\r\n" + " RECEIVED_DATE,Days_Elapsed,FastTrack,Due_date_cgc,INTERVIEW_DATE,SRESULT,Date_of_result_letter,case_status,REMARKS " +
                              "\r\n" + " from (SELECT " +
                              "\r\n" + " ca.FILE_REFERENCE_NO as FILE_REFERENCE_NO, " +
                              "\r\n" + " ssv.CODE as application_type, " +
                              "\r\n" + " c.VETTING_OFFICER AS VETTING_OFFICER, " +
                              "\r\n" + " c.RECEIVED_DATE AS RECEIVED_DATE, " +
                              //new add 0909201
                              "\r\n" + " '' AS Days_Elapsed, " +
                              "\r\n" + " c.DUE_DATE AS DUE_DATE, " +
                              "\r\n" + " c.INTERVIEW_DATE as INTERVIEW_DATE, " +//5
                              "\r\n" + " '' as SRESULT, " +
                              "\r\n" + " status.ENGLISH_DESCRIPTION AS app_status, " +
                              "\r\n" + " CASE " +
                              "\r\n" + " WHEN c.result_letter_date IS null THEN c.withdraw_date " +
                              "\r\n" + " WHEN c.withdraw_date IS null THEN c.result_letter_date " +
                              "\r\n" + " WHEN c.result_letter_date>c.withdraw_date THEN c.result_letter_date " +
                              "\r\n" + " WHEN c.result_letter_date<c.withdraw_date THEN c.withdraw_date " +
                              "\r\n" + " ELSE NULL " +
                              "\r\n" + " END AS Date_of_result_letter, " +
                              "\r\n" + " CASE " +
                              "\r\n" + " WHEN c.RESULT_LETTER_DATE IS NOT NULL OR c.WITHDRAW_DATE IS NOT NULL THEN 'Completed' " +
                              "\r\n" + " ELSE '' " +
                              "\r\n" + " END AS case_status, " +
                              "\r\n" + " c.REMARKS as REMARKS, " +
                              "\r\n" + " CASE WHEN (" +
                              "\r\n" + " SELECT count(c1.uuid) from c_comp_process_monitor c1 " +
                              "\r\n" + " where c.master_id = c1.master_id " +
                              "\r\n" + " and c.received_date = c1.received_date " +
                              "\r\n" + " and c1.monitor_type='FaskTrack' " +
                              "\r\n" + " and c.monitor_type='UPM' " +
                              "\r\n" + " and c1.fast_trck='Y' " +
                              "\r\n" + " )>0 THEN 'Y' " +
                              "\r\n" + " ELSE 'N' END AS FastTrack, " +//10
                              "\r\n" + " CASE WHEN (" +
                              "\r\n" + " SELECT count(c1.uuid) from c_comp_process_monitor c1 " +
                              "\r\n" + " where c.master_id = c1.master_id " +
                              "\r\n" + " and c.received_date = c1.received_date " +
                              "\r\n" + " and c1.monitor_type='FaskTrack' " +
                              "\r\n" + " and c.monitor_type='UPM' " +
                              "\r\n" + " and c1.fast_trck='Y' " +
                              "\r\n" + " )>0 THEN c.RECEIVED_DATE+28 " +
                              "\r\n" + " ELSE ADD_MONTHS(c.RECEIVED_DATE, 3) END AS Due_date_cgc, " +
                              "\r\n" + " '' as letter1_issue_date, " +
                              "\r\n" + " C_GET_AFTER_WORKING_DAY(c.RECEIVED_DATE , 10 )  as pledge_date, " +
                              "\r\n" + " c.Two_month_case, " +
                              "\r\n" + " CASE " +
                              "\r\n" + " WHEN c.two_month_case='Y' " +
                              "\r\n" + " THEN 'N.A.' " +
                              "\r\n" + " ELSE to_char(add_months(c.RECEIVED_DATE ,2),'dd/mm/yyyy') " +
                              "\r\n" + " END as LETTER6_PLEDGE_DATE, " +//15
                              "\r\n" + " c.DATE_OF_LETTER_6, " +
                              "\r\n" + " CASE " +
                              "\r\n" + " WHEN c.two_month_case='Y' " +
                              "\r\n" + " THEN add_months(c.RECEIVED_DATE ,1) " +
                              "\r\n" + " ELSE add_months(c.RECEIVED_DATE ,4) " +
                              "\r\n" + " END as LETTER8_PLEDGE_DATE, " +
                              "\r\n" + " c.DATE_OF_LETTER_8, " +
                              "\r\n" + " add_months(C.INTERVIEW_DATE ,3) as RESULT_LETTER_PLEDGE_DATE, " +
                              "\r\n" + " CASE " +
                              "\r\n" + " WHEN c.WITHDRAW_DATE is NULL THEN c.RESULT_LETTER_DATE " +
                              "\r\n" + " WHEN c.RESULT_LETTER_DATE is NULL THEN c.WITHDRAW_DATE " +
                              "\r\n" + " WHEN c.WITHDRAW_DATE  >= c.RESULT_LETTER_DATE  THEN c.WITHDRAW_DATE " +
                              "\r\n" + " WHEN c.WITHDRAW_DATE  < c.RESULT_LETTER_DATE  THEN add_months(C.INTERVIEW_DATE ,3) " +
                              "\r\n" + " ELSE NULL " +
                              "\r\n" + " END AS result_letter_issue_date, " +//20
                              "\r\n" + " scat.code AS category, " +
                              "\r\n" + " C.NATURE, " +
                              "\r\n" + " C.MONITOR_TYPE, " +
                              "\r\n" + " scat.registration_type as registration_type, " +
                              "\r\n" + " c.APPLY_STATUS as apply_status, " + //25
                              "\r\n" + " (SELECT max(c2.PLEDGE_INITIAL_DATE) from c_comp_process_monitor c2 " +
                              "\r\n" + " where c.master_id = c2.master_id " +
                              "\r\n" + " and c.received_date = c2.received_date " +
                              "\r\n" + " and c2.monitor_type='UPM_10DAYS' " +
                              "\r\n" + " and c.monitor_type='UPM') as pledge_issue_date, " +
                              "\r\n" + " ssv2.CODE as role, " +
                              "\r\n" + " CASE " +
                              "\r\n" + " WHEN c.RESULT_LETTER_DATE IS NOT NULL OR c.WITHDRAW_DATE IS NOT NULL THEN 'Completed' " +
                              "\r\n" + " ELSE '' " +
                              "\r\n" + " END AS case_status_new " +
                              "\r\n" + " , apt.surname ||' '|| apt.given_name_on_id as appName " +
                              "\r\n" + " FROM C_COMP_PROCESS_MONITOR c " +
                              "\r\n" + " LEFT JOIN C_COMP_APPLICATION ca ON ca.UUID = c.MASTER_ID " +
                              "\r\n" + " LEFT JOIN C_S_SYSTEM_VALUE ssv ON ssv.UUID = c.APPLICATION_FORM_ID " +
                              "\r\n" + " LEFT JOIN C_S_SYSTEM_VALUE status ON c.INTERVIEW_RESULT_ID = status.UUID " +
                              "\r\n" + " LEFT JOIN C_S_CATEGORY_CODE scat ON scat.UUID = ca.CATEGORY_ID " +
                              "\r\n" + " LEFT JOIN C_S_SYSTEM_VALUE scatgrp ON scatgrp.UUID = scat.CATEGORY_GROUP_ID " +
                              "\r\n" + " INNER JOIN C_COMP_APPLICANT_INFO cai ON cai.UUID = c.COMPANY_APPLICANTS_ID " +
                              "\r\n" + " INNER JOIN C_S_SYSTEM_VALUE ssv2 ON ssv2.UUID = cai.APPLICANT_ROLE_ID " +
                              "\r\n" + " LEFT JOIN C_APPLICANT apt ON cai.APPLICANT_ID = apt.UUID" +
                              where2Q +
                              "\r\n" + ") " +
                              "\r\n" + " WHERE 1=1 and MONITOR_TYPE= 'UPM' " + 
                              "\r\n" + " And ( category = 'GBC' or category like 'SC%') " //Add by bill 20191126
                              + whereQ
                              + orderby
                              ;



            using (EntitiesRegistration db = new EntitiesRegistration())
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
        private List<List<object>> GetApplicationStagesReportResultListGCA2Comp(Fn08ADM_ReportModel model, string category)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();
            string whereQ = "";
            string orderby = "";
            string where2Q = "";
            if (!string.IsNullOrWhiteSpace(model.fileReference))
            {
                whereQ += " AND upper(file_reference_no) like upper(:fileRef) ";
                queryParameters.Add("fileRef", "%" + model.fileReference + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.VettingOfficer))
            {
                whereQ += " AND upper(vetting_officer) in (:vettingOfficer) ";
                queryParameters.Add("vettingOfficer", model.VettingOfficer);
            }

            if (model.received_fr_date != null)
            {
                whereQ += "\r\n" + " AND received_date >= :receivedDateFrom ";
                queryParameters.Add("receivedDateFrom", model.received_fr_date);
            }
            if (model.received_to_date != null)
            {
                whereQ += "\r\n" + " AND received_date <= :receivedDateTo ";
                queryParameters.Add("receivedDateTo", model.received_to_date);
            }
            if (model.due_fr_date != null)
            {
                whereQ += "\r\n" + " AND due_date >= :dueDateFrom ";
                queryParameters.Add("dueDateFrom", model.due_fr_date);
            }
            if (model.due_to_date != null)
            {
                whereQ += "\r\n" + " AND due_date <= :dueDateTo ";
                queryParameters.Add("dueDateTo", model.due_to_date);
            }
            if (model.interview_fr_date != null)
            {
                whereQ += "\r\n" + " AND interview_date >= :interviewDateFrom ";
                queryParameters.Add("interviewDateFrom", model.interview_fr_date);
            }
            if (model.interview_to_date != null)
            {
                whereQ += "\r\n" + " AND interview_date <= :interviewDateTo ";
                queryParameters.Add("interviewDateTo", model.interview_to_date);
            }

            where2Q += " AND scat.registration_type = :category ";
            queryParameters.Add("category", category);

            //if (model.arrTypeOfApplication != null)
            //{
            //    queryStr += "\r\n" + " AND application_type in (:typeOfApplicationList) and registration_type = :category ";
            //    queryParameters.Add("typeOfApplicationList", model.arrTypeOfApplication);
            //    queryParameters.Add("category", category);
            //}

            orderby += "order by received_date desc, file_reference_no";

            string queryStr = "" +
                              //"\r\n" + " select * from (SELECT " +
                              "\r\n" + " select " +
                              "\r\n" + " FILE_REFERENCE_NO,application_type,category,NATURE,role,appName,VETTING_OFFICER, " +
                              "\r\n" + " RECEIVED_DATE,Days_Elapsed,pledge_date,pledge_issue_date,Two_month_case,LETTER6_PLEDGE_DATE,DATE_OF_LETTER_6, " +
                              "\r\n" + " LETTER8_PLEDGE_DATE,DATE_OF_LETTER_8,INTERVIEW_DATE" +
                              //",app_status" +
                              ",Date_of_result_letter,result_letter_issue_date,RESULT_LETTER_PLEDGE_DATE, " +
                              "\r\n" + " case_status_new,REMARKS " +
                              "\r\n" + " from (SELECT " +
                              "\r\n" + " ca.FILE_REFERENCE_NO as FILE_REFERENCE_NO, " +
                              "\r\n" + " case                                   " +
                              "\r\n" + " when ssv.CODE='BA2C' THEN 'N.A.'       " +
                              "\r\n" + " when ssv.CODE='LETTER' THEN 'N.A.'     " +
                              "\r\n" + " Else ssv.CODE end as application_type, " +
                              //"\r\n" + " ssv.CODE as application_type, " +
                              "\r\n" + " c.VETTING_OFFICER AS VETTING_OFFICER, " +
                              "\r\n" + " c.RECEIVED_DATE AS RECEIVED_DATE, " +
                              //new add 0909201
                              "\r\n" + " '' AS Days_Elapsed, " +
                              "\r\n" + " c.DUE_DATE AS DUE_DATE, " +
                              "\r\n" + " c.INTERVIEW_DATE as INTERVIEW_DATE, " +//5
                              "\r\n" + " status.ENGLISH_DESCRIPTION AS app_status, " +
                              "\r\n" + " CASE " +
                              "\r\n" + " WHEN c.result_letter_date IS null THEN c.withdraw_date " +
                              "\r\n" + " WHEN c.withdraw_date IS null THEN c.result_letter_date " +
                              "\r\n" + " WHEN c.result_letter_date>c.withdraw_date THEN c.result_letter_date " +
                              "\r\n" + " WHEN c.result_letter_date<c.withdraw_date THEN c.withdraw_date " +
                              "\r\n" + " ELSE NULL " +
                              "\r\n" + " END AS Date_of_result_letter, " +
                              "\r\n" + " CASE " +
                              "\r\n" + " WHEN c.RESULT_LETTER_DATE IS NOT NULL OR c.WITHDRAW_DATE IS NOT NULL THEN 'Completed' " +
                              "\r\n" + " ELSE '' " +
                              "\r\n" + " END AS case_status, " +
                              "\r\n" + " c.REMARKS as REMARKS, " +
                              "\r\n" + " CASE WHEN (" +
                              "\r\n" + " SELECT count(c1.uuid) from c_comp_process_monitor c1 " +
                              "\r\n" + " where c.master_id = c1.master_id " +
                              "\r\n" + " and c.received_date = c1.received_date " +
                              "\r\n" + " and c1.monitor_type='FaskTrack' " +
                              "\r\n" + " and c.monitor_type='UPM' " +
                              "\r\n" + " and c1.fast_trck='Y' " +
                              "\r\n" + " )>0 THEN 'Y' " +
                              "\r\n" + " ELSE 'N' END AS FastTrack, " +//10
                              "\r\n" + " CASE WHEN (" +
                              "\r\n" + " SELECT count(c1.uuid) from c_comp_process_monitor c1 " +
                              "\r\n" + " where c.master_id = c1.master_id " +
                              "\r\n" + " and c.received_date = c1.received_date " +
                              "\r\n" + " and c1.monitor_type='FaskTrack' " +
                              "\r\n" + " and c.monitor_type='UPM' " +
                              "\r\n" + " and c1.fast_trck='Y' " +
                              "\r\n" + " )>0 THEN c.RECEIVED_DATE+28 " +
                              "\r\n" + " ELSE ADD_MONTHS(c.RECEIVED_DATE, 3) END AS Due_date_cgc, " +
                              "\r\n" + " '' as letter1_issue_date, " +
                              "\r\n" + " C_GET_AFTER_WORKING_DAY(c.RECEIVED_DATE , 10 )  as pledge_date, " +
                              "\r\n" + " c.Two_month_case, " +
                              "\r\n" + " CASE " +
                              "\r\n" + " WHEN c.two_month_case='Y' " +
                              "\r\n" + " THEN 'N.A.' " +
                              "\r\n" + " ELSE to_char(add_months(c.RECEIVED_DATE ,2),'dd/mm/yyyy') " +
                              "\r\n" + " END as LETTER6_PLEDGE_DATE, " +//15
                              "\r\n" + " c.DATE_OF_LETTER_6, " +
                              "\r\n" + " CASE " +
                              "\r\n" + " WHEN c.two_month_case='Y' " +
                              "\r\n" + " THEN add_months(c.RECEIVED_DATE ,1) " +
                              "\r\n" + " ELSE add_months(c.RECEIVED_DATE ,4) " +
                              "\r\n" + " END as LETTER8_PLEDGE_DATE, " +
                              "\r\n" + " c.DATE_OF_LETTER_8, " +
                              "\r\n" + " add_months(C.INTERVIEW_DATE ,3) as RESULT_LETTER_PLEDGE_DATE, " +
                              "\r\n" + " CASE " +
                              "\r\n" + " WHEN c.WITHDRAW_DATE is NULL THEN c.RESULT_LETTER_DATE " +
                              "\r\n" + " WHEN c.RESULT_LETTER_DATE is NULL THEN c.WITHDRAW_DATE " +
                              "\r\n" + " WHEN c.WITHDRAW_DATE  >= c.RESULT_LETTER_DATE  THEN c.WITHDRAW_DATE " +
                              "\r\n" + " WHEN c.WITHDRAW_DATE  < c.RESULT_LETTER_DATE  THEN add_months(C.INTERVIEW_DATE ,3) " +
                              "\r\n" + " ELSE NULL " +
                              "\r\n" + " END AS result_letter_issue_date, " +//20
                              "\r\n" + " scat.code AS category, " +
                              "\r\n" + " C.NATURE, " +
                              "\r\n" + " C.MONITOR_TYPE, " +
                              "\r\n" + " scat.registration_type as registration_type, " +
                              "\r\n" + " c.APPLY_STATUS as apply_status, " + //25
                              "\r\n" + " (SELECT max(c2.PLEDGE_INITIAL_DATE) from c_comp_process_monitor c2 " +
                              "\r\n" + " where c.master_id = c2.master_id " +
                              "\r\n" + " and c.received_date = c2.received_date " +
                              "\r\n" + " and c2.monitor_type='UPM_10DAYS' " +
                              "\r\n" + " and c.monitor_type='UPM') as pledge_issue_date, " +
                              "\r\n" + " ssv2.CODE as role, " +
                              "\r\n" + " CASE " +
                              "\r\n" + " WHEN c.RESULT_LETTER_DATE IS NOT NULL OR c.WITHDRAW_DATE IS NOT NULL THEN 'Completed' " +
                              "\r\n" + " ELSE '' " +
                              "\r\n" + " END AS case_status_new " +
                              "\r\n" + " , apt.surname ||' '|| apt.given_name_on_id as appName " +
                              "\r\n" + " FROM C_COMP_PROCESS_MONITOR c " +
                              "\r\n" + " LEFT JOIN C_COMP_APPLICATION ca ON ca.UUID = c.MASTER_ID " +
                              "\r\n" + " LEFT JOIN C_S_SYSTEM_VALUE ssv ON ssv.UUID = c.APPLICATION_FORM_ID " +
                              "\r\n" + " LEFT JOIN C_S_SYSTEM_VALUE status ON c.INTERVIEW_RESULT_ID = status.UUID " +
                              "\r\n" + " LEFT JOIN C_S_CATEGORY_CODE scat ON scat.UUID = ca.CATEGORY_ID " +
                              "\r\n" + " LEFT JOIN C_S_SYSTEM_VALUE scatgrp ON scatgrp.UUID = scat.CATEGORY_GROUP_ID " +
                              "\r\n" + " INNER JOIN C_COMP_APPLICANT_INFO cai ON cai.UUID = c.COMPANY_APPLICANTS_ID " +
                              "\r\n" + " INNER JOIN C_S_SYSTEM_VALUE ssv2 ON ssv2.UUID = cai.APPLICANT_ROLE_ID " +
                              "\r\n" + " LEFT JOIN C_APPLICANT apt ON cai.APPLICANT_ID = apt.UUID" +
                              where2Q +
                              "\r\n" + ") " +
                              "\r\n" + " WHERE 1=1 and MONITOR_TYPE= 'UPM' " +
                              "\r\n" + " And ( category = 'GBC' or category like 'SC%') " //Add by bill 20191126
                              + whereQ
                              + orderby
                              ;



            using (EntitiesRegistration db = new EntitiesRegistration())
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
        public Fn08ADM_ReportModel GetCheckBox(string RegType)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_S_SYSTEM_VALUE query = (from sv in db.C_S_SYSTEM_VALUE
                                          join st in db.C_S_SYSTEM_TYPE on sv.SYSTEM_TYPE_ID equals st.UUID
                                          where sv.REGISTRATION_TYPE == RegType
                                          select sv).FirstOrDefault();

                return new Fn08ADM_ReportModel
                {
                    C_S_SYSTEM_VALUE = query
                };


            }

        }
        public void loadApplication(Fn08ADM_ReportModel model)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                List<C_S_SYSTEM_VALUE> temp =
                    db.C_S_SYSTEM_VALUE
                    .Where(o => o.C_S_SYSTEM_TYPE.TYPE == RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM).OrderBy(o => o.CODE).ToList();

                List<C_S_SYSTEM_VALUE> temp2 =
                    db.C_S_SYSTEM_VALUE
                    .Where(o => o.C_S_SYSTEM_TYPE.TYPE == RegistrationConstant.SYSTEM_TYPE_INTERVIEW_RESULT && o.IS_ACTIVE == "Y").OrderBy(o => o.ORDERING).ToList();




                Dictionary<string, string> ApplicationTypeList = new Dictionary<string, string>();

                //List<string> InterviewResultList = new List<string>();
                Dictionary<string, string> InterviewResultList = new Dictionary<string, string>();


                for (int i = 0; i < temp.Count; i++)
                {
                    if (!ApplicationTypeList.ContainsKey(temp[i].CODE))
                    {
                        ApplicationTypeList.Add(temp[i].CODE, temp[i].REGISTRATION_TYPE);
                    }
                    else
                    {
                        ApplicationTypeList[temp[i].CODE] += " " + temp[i].REGISTRATION_TYPE;
                    }
                }
                model.ApplicationTypeList = ApplicationTypeList;

                foreach (var item in temp2)
                {
                    if (!InterviewResultList.ContainsKey(item.CODE))
                    {
                        InterviewResultList.Add(item.CODE, item.ENGLISH_DESCRIPTION != null ? item.ENGLISH_DESCRIPTION : "null");
                    }
                }

                model.InterViewResultList = InterviewResultList;

                if(model.received_fr_date == null)
                {
                    model.received_fr_date = DateTime.Now.AddMonths(-12);
                }
                if(model.received_to_date == null)
                {
                    model.received_to_date = DateTime.Today;
                }

            }
        }
        public FileStreamResult AdminIndExportExcelFile(string fileName,
                List<string> Columns, List<List<object>> Data, string contentTitle = null)
        {
            MemoryStream stream = new MemoryStream();
            ExcelPackage ep = new ExcelPackage();

            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Export");
            DateTime currentD = DateTime.Now;
            for (int i = 0; i < Data.Count(); i++)
            {
                int dayDiff = DateUtil.getDayDiff(currentD, DateUtil.getDisplayDateToDBDate(stringUtil.getDisplay(Data[i][5])));
                Data[i][7] = dayDiff;
            }
            //            string headerRange = "A1:" + Char.ConvertFromUtf32(Columns.Count + 64) + "1";
            //            sheet.Cells[headerRange].LoadFromArrays(
            //                new List<object[]>() { Columns.ToArray() });
            //            sheet.Cells[headerRange].Style.Font.Bold = true;
            //           sheet.Cells[headerRange].Style.Font.Size = 14;
            //          sheet.Cells[headerRange].Style.Font.Color.SetColor(System.Drawing.Color.Blue);

            int startRow = 1;
            string endColumnName = string.Empty;
            var dividend = Columns.Count;
            while (dividend > 0)
            {
                var module = (dividend - 1) % 26;
                endColumnName = Convert.ToChar(65 + module) + endColumnName;
                dividend = (dividend - module) / 26;
            }
            if (!string.IsNullOrEmpty(contentTitle))
            {
                string headerRange = "A1:" + endColumnName + "1";
                sheet.Cells[headerRange].LoadFromText(contentTitle);
                sheet.Cells[headerRange].Merge = true;
                sheet.Cells[headerRange].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[headerRange].Style.Font.Bold = true;
                sheet.Cells[headerRange].Style.Font.Size = 20;
                sheet.Cells[headerRange].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                sheet.Cells.AutoFitColumns();
                startRow += 1;
            }
           
            string headerRangeDate = "A" + startRow + ":" + endColumnName + startRow;
            sheet.Cells[headerRangeDate].LoadFromText("Date: "+ DateTime.Now.ToShortDateString());
            sheet.Cells[headerRangeDate].Merge = true;
            sheet.Cells[headerRangeDate].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            sheet.Cells[headerRangeDate].Style.Font.Bold = true;
            sheet.Cells[headerRangeDate].Style.Font.Size = 14;
            sheet.Cells[headerRangeDate].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            sheet.Cells.AutoFitColumns();
            startRow += 1;

            if (!string.IsNullOrEmpty(fileName))
            {
                string reportNameRange = headerRangeDate = "A" + startRow + ":" + endColumnName + startRow;
                sheet.Cells[headerRangeDate].LoadFromText(fileName);
                sheet.Cells[headerRangeDate].Merge = true;
                sheet.Cells[headerRangeDate].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[headerRangeDate].Style.Font.Bold = true;
                sheet.Cells[headerRangeDate].Style.Font.Size = 14;
                sheet.Cells[headerRangeDate].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                sheet.Cells.AutoFitColumns();
                startRow += 1;
            }
           

            //for (int i = 0; i < Columns.Count; i++)
            //{
            //    sheet.Cells[1, i + 1].LoadFromText(Columns[i].ToString());
            //    sheet.Cells[1, i + 1].Style.Font.Bold = true;
            //    sheet.Cells[1, i + 1].Style.Font.Size = 14;
            //    sheet.Cells[1, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            //}
            for (int i = 0; i < Columns.Count; i++)
            {
                sheet.Cells[startRow, i + 1].LoadFromText(Columns[i].ToString());
                sheet.Cells[startRow, i + 1].Style.Font.Bold = true;
                sheet.Cells[startRow, i + 1].Style.Font.Size = 14;
                sheet.Cells[startRow, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            }
            sheet.Cells.AutoFitColumns();

            if (Data != null)
            {
                for (int i = 0; i < Data.Count; i++)
                {
                    List<object> eachRow = Data[i];
                    for (int j = 0; j < eachRow.Count; j++)
                    {

                        //sheet.Cells[i + 2, j + 1].Value =
                        //    (getString(eachRow[j].ToString()));
                        sheet.Cells[i + startRow + 1, j + 1].Value =
                            (getString(eachRow[j].ToString()));


                        //sheet.Cells[i + 2, j + 1].LoadFromText(getString(eachRow[j].ToString()));
                    }
                }
            }
            /**
            string path = @"C:\MWMS2\test.xlsx "; 
            Stream streamDDD = File.Create(path);
            ep.SaveAs(streamDDD);
            streamDDD.Close();
               **/
            sheet.Cells.AutoFitColumns();
            ep.SaveAs(stream);
            stream.Position = 0;
            FileStreamResult fsr = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            fsr.FileDownloadName = fileName + ".xlsx";  //.xlsx
            //DateTime nowDt = DateTime.Now;
            // addMemory(nowDt, fsr);
            //return nowDt.Ticks.ToString();
            return fsr;
        }
        public FileStreamResult AdminCompExportExcelFile(string fileName,
        List<string> Columns, List<List<object>> Data,string contentTitle = null)
        {
            MemoryStream stream = new MemoryStream();
            ExcelPackage ep = new ExcelPackage();

            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Export");
            for (int i = 0; i < Data.Count(); i++)
            {
                DateTime? receivedDate = DateUtil.getDisplayDateToDBDate(stringUtil.getDisplay(Data[i][5]));
                DateTime? interviewDate = DateUtil.getDisplayDateToDBDate(stringUtil.getDisplay(Data[i][8]));
                DateTime? d = receivedDate;
                if (interviewDate != null)
                {
                    int c = DateUtil.compareDate(receivedDate.Value, interviewDate.Value);
                    if (c == 2)
                    {
                        d = receivedDate;
                    }
                    else
                    {
                        d = interviewDate;
                    }
                }
                d.Value.AddMonths(3);
                DateTime? dueDate = d;
                string status = stringUtil.getDisplay(Data[i][11]);
                if (!status.Equals("Completed"))
                {
                    DateTime today = DateTime.Now;
                    int c2 = DateUtil.compareDate(dueDate.Value, today);
                    if (c2 == 2)
                    {
                        status = "In Progress";
                    }
                    else
                    {
                        status = "Overdue";
                    }
                }
                DateTime currentD = DateTime.Now;
                int dayDiff = DateUtil.getDayDiff(currentD, receivedDate.Value);
                Data[i][6] = dayDiff;
            }


            int startRow = 1;
            string endColumnName = string.Empty;
            var dividend = Columns.Count;
            while (dividend > 0)
            {
                var module = (dividend - 1) % 26;
                endColumnName = Convert.ToChar(65 + module) + endColumnName;
                dividend = (dividend - module) / 26;
            }
            if (!string.IsNullOrEmpty(contentTitle))
            {
                string headerRange = "A1:" + endColumnName + "1";
                sheet.Cells[headerRange].LoadFromText(contentTitle);
                sheet.Cells[headerRange].Merge = true;
                sheet.Cells[headerRange].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[headerRange].Style.Font.Bold = true;
                sheet.Cells[headerRange].Style.Font.Size = 20;
                sheet.Cells[headerRange].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                sheet.Cells.AutoFitColumns();
                startRow += 1;
            }

            string headerRangeDate = "A" + startRow + ":" + endColumnName + startRow;
            sheet.Cells[headerRangeDate].LoadFromText("Date: " + DateTime.Now.ToShortDateString());
            sheet.Cells[headerRangeDate].Merge = true;
            sheet.Cells[headerRangeDate].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            sheet.Cells[headerRangeDate].Style.Font.Bold = true;
            sheet.Cells[headerRangeDate].Style.Font.Size = 14;
            sheet.Cells[headerRangeDate].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            sheet.Cells.AutoFitColumns();
            startRow += 1;

            if (!string.IsNullOrEmpty(fileName))
            {
                string reportNameRange = headerRangeDate = "A" + startRow + ":" + endColumnName + startRow;
                sheet.Cells[headerRangeDate].LoadFromText(fileName);
                sheet.Cells[headerRangeDate].Merge = true;
                sheet.Cells[headerRangeDate].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[headerRangeDate].Style.Font.Bold = true;
                sheet.Cells[headerRangeDate].Style.Font.Size = 14;
                sheet.Cells[headerRangeDate].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                sheet.Cells.AutoFitColumns();
                startRow += 1;
            }


            //for (int i = 0; i < Columns.Count; i++)
            //{
            //    sheet.Cells[1, i + 1].LoadFromText(Columns[i].ToString());
            //    sheet.Cells[1, i + 1].Style.Font.Bold = true;
            //    sheet.Cells[1, i + 1].Style.Font.Size = 14;
            //    sheet.Cells[1, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            //}
            for (int i = 0; i < Columns.Count; i++)
            {
                sheet.Cells[startRow, i + 1].LoadFromText(Columns[i].ToString());
                sheet.Cells[startRow, i + 1].Style.Font.Bold = true;
                sheet.Cells[startRow, i + 1].Style.Font.Size = 14;
                sheet.Cells[startRow, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            }
            sheet.Cells.AutoFitColumns();

            if (Data != null)
            {
                for (int i = 0; i < Data.Count; i++)
                {
                    List<object> eachRow = Data[i];
                    for (int j = 0; j < eachRow.Count; j++)
                    {

                        //sheet.Cells[i + 2, j + 1].Value =
                        //    (getString(eachRow[j].ToString()));
                        sheet.Cells[i + startRow + 1, j + 1].Value =
                            (getString(eachRow[j].ToString()));


                        //sheet.Cells[i + 2, j + 1].LoadFromText(getString(eachRow[j].ToString()));
                    }
                }
            }
            /**
            string path = @"C:\MWMS2\test.xlsx "; 
            Stream streamDDD = File.Create(path);
            ep.SaveAs(streamDDD);
            streamDDD.Close();
               **/
            sheet.Cells.AutoFitColumns();
            ep.SaveAs(stream);
            stream.Position = 0;
            FileStreamResult fsr = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            fsr.FileDownloadName = fileName + ".xlsx";  //.xlsx
            //DateTime nowDt = DateTime.Now;
            // addMemory(nowDt, fsr);
            //return nowDt.Ticks.ToString();
            return fsr;
        }

        public FileStreamResult AdminExportGCA2ExcelFile(string fileName,
        List<string> Columns, List<List<object>> Data,string contentTitle = null)
        {
            MemoryStream stream = new MemoryStream();
            ExcelPackage ep = new ExcelPackage();

            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Export");



            for (int i = 0; i < Data.Count(); i++)
            {
                string status = stringUtil.getDisplay(Data[i][21]);
                DateTime today = DateTime.Now;
                int c3 = 0;
                if (!status.Equals("Completed"))
                {
                    DateTime? letter8PledgeDate = DateUtil.getDisplayDateToDBDate(stringUtil.getDisplay(Data[i][14]));
                    DateTime? pledgeDateResultLetter = DateUtil.getDisplayDateToDBDate(stringUtil.getDisplay(Data[i][20]));
                    if (letter8PledgeDate != null && pledgeDateResultLetter != null)
                    {
                        int c2 = DateUtil.compareDate(letter8PledgeDate.Value, pledgeDateResultLetter.Value);
                        if (c2 == 2)
                        {
                            c3 = DateUtil.compareDate(letter8PledgeDate.Value, today);
                        }
                        else
                        {
                            c3 = DateUtil.compareDate(pledgeDateResultLetter.Value, today);
                        }
                    }
                    else if (letter8PledgeDate != null)
                    {
                        c3 = DateUtil.compareDate(letter8PledgeDate.Value, today);
                    }
                    else if (pledgeDateResultLetter != null)
                    {
                        c3 = DateUtil.compareDate(pledgeDateResultLetter.Value, today);
                    }
                    if (c3 == 2)
                    {
                        status = "In Progress";
                    }
                    else
                    {
                        status = "Overdue";
                    }
                    DateTime currentD = DateTime.Now;
                    int dayDiff = DateUtil.getDayDiff(currentD, DateUtil.getDisplayDateToDBDate(stringUtil.getDisplay(Data[i][7])));
                    Data[i][8] = dayDiff;
                    Data[i][21] = status;
                }
            }

            int startRow = 1;
            string endColumnName = string.Empty;
            var dividend = Columns.Count;
            while (dividend > 0)
            {
                var module = (dividend - 1) % 26;
                endColumnName = Convert.ToChar(65 + module) + endColumnName;
                dividend = (dividend - module) / 26;
            }
            if (!string.IsNullOrEmpty(contentTitle))
            {
                string headerRange = "A1:" + endColumnName + "1";
                sheet.Cells[headerRange].LoadFromText(contentTitle);
                sheet.Cells[headerRange].Merge = true;
                sheet.Cells[headerRange].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[headerRange].Style.Font.Bold = true;
                sheet.Cells[headerRange].Style.Font.Size = 20;
                sheet.Cells[headerRange].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                sheet.Cells.AutoFitColumns();
                startRow += 1;
            }

            string headerRangeDate = "A" + startRow + ":" + endColumnName + startRow;
            sheet.Cells[headerRangeDate].LoadFromText("Date: " + DateTime.Now.ToShortDateString());
            sheet.Cells[headerRangeDate].Merge = true;
            sheet.Cells[headerRangeDate].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            sheet.Cells[headerRangeDate].Style.Font.Bold = true;
            sheet.Cells[headerRangeDate].Style.Font.Size = 14;
            sheet.Cells[headerRangeDate].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            sheet.Cells.AutoFitColumns();
            startRow += 1;

            if (!string.IsNullOrEmpty(fileName))
            {
                string reportNameRange = headerRangeDate = "A" + startRow + ":" + endColumnName + startRow;
                sheet.Cells[headerRangeDate].LoadFromText(fileName);
                sheet.Cells[headerRangeDate].Merge = true;
                sheet.Cells[headerRangeDate].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[headerRangeDate].Style.Font.Bold = true;
                sheet.Cells[headerRangeDate].Style.Font.Size = 14;
                sheet.Cells[headerRangeDate].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                sheet.Cells.AutoFitColumns();
                startRow += 1;
            }

            //for (int i = 0; i < Columns.Count; i++)
            //{
            //    sheet.Cells[1, i + 1].LoadFromText(Columns[i].ToString());
            //    sheet.Cells[1, i + 1].Style.Font.Bold = true;
            //    sheet.Cells[1, i + 1].Style.Font.Size = 14;
            //    sheet.Cells[1, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            //}
            for (int i = 0; i < Columns.Count; i++)
            {
                sheet.Cells[startRow, i + 1].LoadFromText(Columns[i].ToString());
                sheet.Cells[startRow, i + 1].Style.Font.Bold = true;
                sheet.Cells[startRow, i + 1].Style.Font.Size = 14;
                sheet.Cells[startRow, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            }
            sheet.Cells.AutoFitColumns();

            if (Data != null)
            {
                for (int i = 0; i < Data.Count; i++)
                {
                    List<object> eachRow = Data[i];
                    for (int j = 0; j < eachRow.Count; j++)
                    {

                        //sheet.Cells[i + 2, j + 1].Value =
                        //    (getString(eachRow[j].ToString()));
                        sheet.Cells[i + startRow + 1, j + 1].Value =
                        (getString(eachRow[j].ToString()));

                        //sheet.Cells[i + 2, j + 1].LoadFromText(getString(eachRow[j].ToString()));
                    }
                }
            }
            /**
            string path = @"C:\MWMS2\test.xlsx "; 
            Stream streamDDD = File.Create(path);
            ep.SaveAs(streamDDD);
            streamDDD.Close();
               **/
            sheet.Cells.AutoFitColumns();
            ep.SaveAs(stream);
            stream.Position = 0;
            FileStreamResult fsr = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            fsr.FileDownloadName = fileName + ".xlsx";  //.xlsx
            //DateTime nowDt = DateTime.Now;
            // addMemory(nowDt, fsr);
            //return nowDt.Ticks.ToString();
            return fsr;
        }
        public FileStreamResult AdminExportGCAExcelFile(string fileName, List<string> Columns, List<List<object>> Data,string contentTitle = null)
        {
            //R
            MemoryStream stream = new MemoryStream();
            ExcelPackage ep = new ExcelPackage();

            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Export");

            for (int i = 0; i < Data.Count(); i++)
            {
                DateTime? dueDate = DateUtil.getDisplayDateToDBDate(stringUtil.getDisplay(Data[i][8]));
                if (!stringUtil.getDisplay(Data[i][9]).Equals(""))
                {
                    DateTime? interviewDate = DateUtil.getDisplayDateToDBDate(stringUtil.getDisplay(Data[i][9]));
                    interviewDate.Value.AddMonths(3);
                }
                string status = stringUtil.getDisplay(Data[i][12]);
                if (!status.Equals("Completed"))
                {
                    DateTime today = DateTime.Now;
                    int c2 = DateUtil.compareDate(dueDate.Value, today);
                    if (c2 == 2)
                    {
                        status = "In Progress";
                    }
                    else
                    {
                        status = "Overdue";
                    }
                }
                DateTime? currentD = DateTime.Now;
                int dayDiff = DateUtil.getDayDiff(currentD, DateUtil.getDisplayDateToDBDate(stringUtil.getDisplay(Data[i][3])));
                Data[i][6] = dayDiff;
                Data[i][8] = dueDate;
                Data[i][12] = status;
            }

            int startRow = 1;
            string endColumnName = string.Empty;
            var dividend = Columns.Count;
            while (dividend > 0)
            {
                var module = (dividend - 1) % 26;
                endColumnName = Convert.ToChar(65 + module) + endColumnName;
                dividend = (dividend - module) / 26;
            }
            if (!string.IsNullOrEmpty(contentTitle))
            {
                string headerRange = "A1:" + endColumnName + "1";
                sheet.Cells[headerRange].LoadFromText(contentTitle);
                sheet.Cells[headerRange].Merge = true;
                sheet.Cells[headerRange].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                sheet.Cells[headerRange].Style.Font.Bold = true;
                sheet.Cells[headerRange].Style.Font.Size = 20;
                sheet.Cells[headerRange].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                sheet.Cells.AutoFitColumns();
                startRow += 1;
            }

            string headerRangeDate = "A" + startRow + ":" + endColumnName + startRow;
            sheet.Cells[headerRangeDate].LoadFromText("Date: " + DateTime.Now.ToShortDateString());
            sheet.Cells[headerRangeDate].Merge = true;
            sheet.Cells[headerRangeDate].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            sheet.Cells[headerRangeDate].Style.Font.Bold = true;
            sheet.Cells[headerRangeDate].Style.Font.Size = 14;
            sheet.Cells[headerRangeDate].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            sheet.Cells.AutoFitColumns();
            startRow += 1;

            if (!string.IsNullOrEmpty(fileName))
            {
                string reportNameRange = headerRangeDate = "A" + startRow + ":" + endColumnName + startRow;
                sheet.Cells[headerRangeDate].LoadFromText(fileName);
                sheet.Cells[headerRangeDate].Merge = true;
                sheet.Cells[headerRangeDate].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                sheet.Cells[headerRangeDate].Style.Font.Bold = true;
                sheet.Cells[headerRangeDate].Style.Font.Size = 14;
                sheet.Cells[headerRangeDate].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                sheet.Cells.AutoFitColumns();
                startRow += 1;
            }

            //for (int i = 0; i < Columns.Count; i++)
            //{
            //    sheet.Cells[1, i + 1].LoadFromText(Columns[i].ToString());
            //    sheet.Cells[1, i + 1].Style.Font.Bold = true;
            //    sheet.Cells[1, i + 1].Style.Font.Size = 14;
            //    sheet.Cells[1, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            //}
            for (int i = 0; i < Columns.Count; i++)
            {
                sheet.Cells[startRow, i + 1].LoadFromText(Columns[i].ToString());
                sheet.Cells[startRow, i + 1].Style.Font.Bold = true;
                sheet.Cells[startRow, i + 1].Style.Font.Size = 14;
                sheet.Cells[startRow, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            }
            sheet.Cells.AutoFitColumns();

            if (Data != null)
            {
                for (int i = 0; i < Data.Count; i++)
                {
                    List<object> eachRow = Data[i];
                    for (int j = 0; j < eachRow.Count; j++)
                    {

                        //sheet.Cells[i + 2, j + 1].Value =
                        //    (getString(eachRow[j].ToString()));
                        sheet.Cells[i + startRow + 1, j + 1].Value =
                       (getString(eachRow[j].ToString()));


                        //sheet.Cells[i + 2, j + 1].LoadFromText(getString(eachRow[j].ToString()));
                    }
                }
            }
            /**
            string path = @"C:\MWMS2\test.xlsx "; 
            Stream streamDDD = File.Create(path);
            ep.SaveAs(streamDDD);
            streamDDD.Close();
               **/
            sheet.Cells.AutoFitColumns();
            ep.SaveAs(stream);
            stream.Position = 0;
            FileStreamResult fsr = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            fsr.FileDownloadName = fileName + ".xlsx";  //.xlsx
            //DateTime nowDt = DateTime.Now;
            // addMemory(nowDt, fsr);
            //return nowDt.Ticks.ToString();
            return fsr;
        }

        public FileContentResult getIndCertImgByUUID(string UUID)
        {
            using(EntitiesRegistration db = new EntitiesRegistration())
            {
                var cert = db.C_IND_CERTIFICATE.Find(UUID);

                if(cert != null)
                {
                    var fullPath = ApplicationConstant.CRMFilePath + cert.FILE_PATH_NONRESTRICTED;
                    if (File.Exists(fullPath))
                    {
                        byte[] fileBytes = getFileByte(fullPath);
                        if (fullPath.Contains("jpg"))
                        {
                            return new FileContentResult(fileBytes, System.Net.Mime.MediaTypeNames.Image.Jpeg);
                        }
                        else if (fullPath.Contains("gif"))
                        {
                            return new FileContentResult(fileBytes, System.Net.Mime.MediaTypeNames.Image.Gif);
                        }
                        else if (fullPath.Contains("pdf"))
                        {
                            return new FileContentResult(fileBytes, "application/pdf");
                        }
                    }
                    else
                    {
                        return null;
                    }
                }

                return null;
            }
        }

        public byte[] getFileByte(String fullPath)
        {
            try
            {
                return System.IO.File.ReadAllBytes(@fullPath);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
