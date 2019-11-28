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
    public class RegistrationSearchExportQPService : RegistrationCommonService
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
            "		WHERE     (SEARCH_TYPE = 'QP' )  ";
            //"		ANd REGISTRATION_TYPE IN :TYPE  "; 
           // "		ORDER BY REGISTRATION_TYPE, ENGLISH_COMPANY_NAME,CHINESE_COMPANY_NAME , ENGLISH_NAME,CHINESE_NAME   ";

        public FileStreamResult exportQPExcelData(Fn01Search_ExportQPSearchModel model)
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


          
            String UUID = "";
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    string Select1 = model.check1;
                    string Select2 = model.check2;
                    string Select3 = model.check3;
                    string Select4 = model.check4;
                    string Select5 = model.check5;
                    string Select6 = model.check6;

                    List<string> dataList = new List<string>();

                    if (!string.IsNullOrWhiteSpace(Select1)) { dataList.Add(Select1); }
                    if (!string.IsNullOrWhiteSpace(Select2)) { dataList.Add(Select2); }
                    if (!string.IsNullOrWhiteSpace(Select3)) { dataList.Add(Select3); }
                    if (!string.IsNullOrWhiteSpace(Select4)) { dataList.Add(Select4); }
                    if (!string.IsNullOrWhiteSpace(Select5)) { dataList.Add(Select5); }
                    if (!string.IsNullOrWhiteSpace(Select6)) { dataList.Add(Select6); }


                    if (dataList != null && dataList.Count > 0)
                    {
                        string parStr = "";
                        for(int i = 0;i < dataList.Count; i++)
                        {
                            parStr += ", :TYPE" + i;
                        }
                        parStr = parStr.Substring(2);

                        QP_SQL += "\r\n" + "\t" + " AND" + " REGISTRATION_TYPE IN ("  + parStr+ ")";
                    }

                    Dictionary<string, object> queryParameters = new Dictionary<string, object>();//
                    for (int i = 0; i < dataList.Count; i++)
                    {
                        queryParameters.Add("TYPE" + i, dataList);
                    }
                  //  Dictionary<string, object> queryParameters = new Dictionary<string, object>();
                  // / queryParameters.Add("TYPE", dataList);

                    //Dictionary<string, object> dataInput = new Dictionary<string, object>();
                    //dataInput.Add("TYPE", dataList);

                    String sql = this.QP_SQL;
                    DbDataReader dr = CommonUtil.GetDataReader(conn, sql, queryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return this.exportExcelFile("QP_LIST", headerList, data);

        }


    }

}
