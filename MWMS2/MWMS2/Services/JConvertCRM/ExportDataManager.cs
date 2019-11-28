using MWMS2.Areas.Registration.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Utility;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services
{
    public class stringUtil
    {
        public static string getDisplay(string v)
        {
            return v == null ? "" : v.Trim();
        }
        public static string getDisplay(object v)
        {
            return v == null ? "" : v.ToString().Trim();
        }
        public static bool isBlank(string v)
        {
            return "".Equals(getDisplay(v));
        }
        public static bool isChineseChar(string all)
        {

            if (all == null)
            {
                return false;
            }

            if (all.Count() == 0)
            {
                return false;
            }
            char ch = all[0];

            int v = (int)ch;
            if (v >= 19968 && v <= 171941)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class IndQualificationDetailDAO
    { 
    }
    public class IndApplicationManager {

        public List<object[]> getIndApplicationForDataExport(string registrationType, List<string> categoryCodeList, string pnrcUUID, bool isPRB)
        {
            return null;
        }
        public List<object[]> getIndApplicationForDataExport(string registrationType, string categoryCodeList, string pnrcUUID, bool isPRB)
        {
            return null;
        }
        public List<object[]> getMWIApplicationForDataExportRegisters()
        {
            string q = ""
                + "\r\n\t" + " SELECT  C.CERTIFICATION_NO, APP.SURNAME, APP.GIVEN_NAME_ON_ID                                                                                              "
                + "\r\n\t" + " , APP.CHINESE_NAME,  C.EXPIRY_DATE, A.bs_telephone_no1 as TELEPHONE_NO1                                                                                    "
                + "\r\n\t" + " ,  C_GET_MW_INDIVIDUAL_ITEMS(A.UUID) AS ITEMS                                                                                                                "
                + "\r\n\t" + " ,  (SELECT REGION_CHI.ENGLISH_DESCRIPTION FROM C_S_SYSTEM_VALUE  REGION_CHI  WHERE REGION_CHI.UUID= A.BS_REGION_CODE_ID ) AS REGION_ENG                      "
                + "\r\n\t" + " , (SELECT REGION_CHI.CHINESE_DESCRIPTION FROM C_S_SYSTEM_VALUE REGION_CHI  WHERE REGION_CHI.UUID= A.BS_REGION_CODE_ID )  AS REGION_CHI                       "
                + "\r\n\t" + " , A.BS_EMAIL AS EMAIL, A.BS_FAX_NO1                                                                                                                        "
                + "\r\n\t" + " , CASE WHEN trunc(current_date) > C.EXPIRY_DATE AND add_months(C.retention_application_date, 24) > trunc(current_date)  THEN '@' ELSE '' END as flag       "
                + "\r\n\t" + " , CASE WHEN SVALUE_FSS.ENGLISH_DESCRIPTION = 'Yes' THEN '?' ELSE '-' END INTERESTED_FSS                                                                    "
                + "\r\n\t" + " , CASE WHEN SVALUE_FSS.ENGLISH_DESCRIPTION = 'Yes' THEN '?' ELSE '-' END INTERESTED_FSS_CHI                                                                "

                + "\r\n\t" + " FROM  C_IND_CERTIFICATE C, C_APPLICANT APP, C_S_SYSTEM_VALUE SV, C_IND_APPLICATION A                                                                               "
                + "\r\n\t" + " LEFT JOIN C_S_SYSTEM_TYPE STYPE_FSS  ON  STYPE_FSS.TYPE = 'FSS_DROPDOWN'                                                                                     "
                + "\r\n\t" + " LEFT JOIN C_S_SYSTEM_VALUE SVALUE_FSS  ON  STYPE_FSS.UUID = SVALUE_FSS.SYSTEM_TYPE_ID  AND NVL(A.INTERESTED_FSS, 'I') = SVALUE_FSS.CODE                      ";

            string whereQ = ""
                + "\r\n\t" + " WHERE  C.MASTER_ID=A.UUID                                                                                                                                  "
                + "\r\n\t" + " AND A.APPLICANT_ID =APP.UUID                                                                                                                               "
                + "\r\n\t" + " AND A.REGISTRATION_TYPE= :REGISTRATION_TYPE                                                                                                                "
                + "\r\n\t" + " AND  SV.UUID=C.application_status_id                                                                                                                       "
                + "\r\n\t" + " AND ( ( SV.CODE= :SV_CODE ) or (C.RETENTION_APPLICATION_DATE is not null ))                                                                                "
                + "\r\n\t" + " AND C.CERTIFICATION_NO IS NOT NULL                                                                                                                         "
                + "\r\n\t" + " AND  (  (C.EXPIRY_DATE IS NOT NULL and C.EXPIRY_DATE >= trunc(current_date) )                                                                              "
                + "\r\n\t" + " or     (C.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd')                                                                                     "
                + "\r\n\t" + " and (C.EXPIRY_DATE < trunc(current_date)) )  )                                                                                                             "
                + "\r\n\t" + " AND  ( (C.REMOVAL_DATE IS NULL) OR  (C.REMOVAL_DATE > trunc(current_date)))                                                                                ";

            DisplayGrid dg = new DisplayGrid();


            dg.Columns = new Dictionary<string, string>[] {
                new Dictionary<string, string> { ["columnName"] = "CERTIFICATION_NO" }
                , new Dictionary<string, string> { ["columnName"] = "SURNAME" }
                , new Dictionary<string, string> { ["columnName"] = "GIVEN_NAME_ON_ID" }
                , new Dictionary<string, string> { ["columnName"] = "CHINESE_NAME" }
                , new Dictionary<string, string> { ["columnName"] = "EXPIRY_DATE" }
                , new Dictionary<string, string> { ["columnName"] = "TELEPHONE_NO1" }
                , new Dictionary<string, string> { ["columnName"] = "ITEMS" }
                , new Dictionary<string, string> { ["columnName"] = "REGION_ENG" }
                , new Dictionary<string, string> { ["columnName"] = "REGION_CHI" }
                , new Dictionary<string, string> { ["columnName"] = "EMAIL" }
                , new Dictionary<string, string> { ["columnName"] = "BS_FAX_NO1" }
                , new Dictionary<string, string> { ["columnName"] = "FLAG" }
                , new Dictionary<string, string> { ["columnName"] = "INTERESTED_FSS" }
                , new Dictionary<string, string> { ["columnName"] = "INTERESTED_FSS_CHI" }
            };

            dg.QueryParameters.Add("REGISTRATION_TYPE", RegistrationConstant.REGISTRATION_TYPE_MWIA);
            dg.QueryParameters.Add("SV_CODE", RegistrationConstant.STATUS_ACTIVE);

            dg.Query = q;
            dg.QueryWhere = whereQ;
            dg.SortType = 0;
            dg.Sort = "upper(APP.SURNAME)  || ' '  ||  upper(APP.GIVEN_NAME_ON_ID) ,  C.CERTIFICATION_NO, C.UUID  ";

            dg.Rpp = -1;
            dg.Search();
            List<object[]> r = new List<object[]>();
            for (int i = 0; i < dg.Data.Count; i++)
            {
                List<object> row = new List<object>();
                for (int j = 0; j < dg.Columns.Length; j++)
                    row.Add(dg.Data[i][dg.Columns[j]["columnName"]]);
                r.Add(row.ToArray());
            }
            return r;
        }
        public List<object[]> getMWIApplicationForDataExportRegistersQP()
        {

            return null;
        }

        public List<object[]> getIndApplicationForDataExportRegistersQP(string regType, C_S_CATEGORY_CODE cat1, C_S_CATEGORY_CODE cat2)
        {
            return null;
        }
        public List<object[]> getIndApplicationForDataExportRegisters(string regType, C_S_CATEGORY_CODE cat1, C_S_CATEGORY_CODE cat2)
        {

            string q = ""
                + "\r\n\t" + " SELECT C.UUID                                                                                                                                        "
                + "\r\n\t" + "  ,  C.CERTIFICATION_NO                                                                                                                               "
                + "\r\n\t" + "  , APP.SURNAME                                                                                                                                       "
                + "\r\n\t" + "  , APP.GIVEN_NAME_ON_ID                                                                                                                              "
                + "\r\n\t" + "  , APP.CHINESE_NAME                                                                                                                                  "
                + "\r\n\t" + "  , C.EXPIRY_DATE                                                                                                                                     "
                + "\r\n\t" + "  , A.bs_telephone_no1 as TELEPHONE_NO1                                                                                                               "
                + "\r\n\t" + "  ,  BS.BUILDING_SAFETY_ID                                                                                                                            "
                + "\r\n\t" + "  ,   CASE                                                                                                                                            "
                + "\r\n\t" + "  WHEN trunc(current_date) > C.EXPIRY_DATE AND add_months(C.retention_application_date, 24) > trunc(current_date)  THEN '@' ELSE '' END as flag       "
                + "\r\n\t" + "  ,  A.MBIS_RI as MBIS                                                                                                                                "
                + "\r\n\t" + "  ,  SVALUE_FSS.ENGLISH_DESCRIPTION AS INTERESTED_FSS                                                                                                 "
                + "\r\n\t" + "  ,  SVALUE_FSS.CHINESE_DESCRIPTION AS INTERESTED_FSS_CHI                                                                                             "
                + "\r\n\t" + "  FROM  C_IND_CERTIFICATE C, C_S_CATEGORY_CODE CAT,  C_APPLICANT APP, C_IND_APPLICATION A                                                             "
                + "\r\n\t" + "  LEFT OUTER JOIN C_BUILDING_SAFETY_INFO BS ON A.REGISTRATION_TYPE = BS.REGISTRATION_TYPE AND A.UUID = BS.MASTER_ID                                   "
                + "\r\n\t" + "  LEFT OUTER JOIN C_S_SYSTEM_VALUE BSV ON  BS.BUILDING_SAFETY_ID = BSV.UUID                                                                           "
                + "\r\n\t" + "  LEFT JOIN C_S_SYSTEM_TYPE STYPE_FSS  ON STYPE_FSS.TYPE = 'FSS_DROPDOWN'                                                                             "
                + "\r\n\t" + "  LEFT JOIN C_S_SYSTEM_VALUE SVALUE_FSS  ON STYPE_FSS.UUID = SVALUE_FSS.SYSTEM_TYPE_ID                                                                "
                + "\r\n\t" + "  AND NVL(A.INTERESTED_FSS, 'I') = SVALUE_FSS.CODE                                                                                                    ";

            string whereQ = ""
                + "\r\n\t" + "  WHERE C.CATEGORY_ID = CAT.UUID                                                                                                                      "
                + "\r\n\t" + "  AND C.MASTER_ID = A.UUID                                                                                                                            "
                + "\r\n\t" + "    AND A.REGISTRATION_TYPE = :REGISTRATION_TYPE                                                                                                      ";



            if (cat2 != null)
            {
                whereQ = whereQ + " AND (CAT.UUID= :categoryCode OR  CAT.UUID= :secCategoryCode)";
            }
            else
            {
                whereQ = whereQ + " AND (CAT.UUID= :categoryCode) ";
            }


            whereQ = whereQ

                + "\r\n\t" + "  AND A.APPLICANT_ID = APP.UUID  AND C.CERTIFICATION_NO IS NOT NULL                                                                                   "
                + "\r\n\t" + "  AND((C.EXPIRY_DATE IS NOT NULL and C.EXPIRY_DATE >= trunc(current_date))                                                                            "
                + "\r\n\t" + "  or(C.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd')                                                                                   "
                + "\r\n\t" + "  and(C.EXPIRY_DATE < trunc(current_date))) ) AND((C.REMOVAL_DATE IS NULL)                                                                            "
                + "\r\n\t" + "  OR(C.REMOVAL_DATE > trunc(current_date)))                                                                                                           ";






            DisplayGrid dg = new DisplayGrid();


            dg.Columns = new Dictionary<string, string>[] {
                  new Dictionary<string, string> { ["columnName"] = "UUID" }
                , new Dictionary<string, string> { ["columnName"] = "CERTIFICATION_NO" }
                , new Dictionary<string, string> { ["columnName"] = "SURNAME" }
                , new Dictionary<string, string> { ["columnName"] = "GIVEN_NAME_ON_ID" }
                , new Dictionary<string, string> { ["columnName"] = "CHINESE_NAME" }
                , new Dictionary<string, string> { ["columnName"] = "EXPIRY_DATE" }
                , new Dictionary<string, string> { ["columnName"] = "TELEPHONE_NO1" }
                , new Dictionary<string, string> { ["columnName"] = "BUILDING_SAFETY_ID" }
                , new Dictionary<string, string> { ["columnName"] = "FLAG" }
                , new Dictionary<string, string> { ["columnName"] = "MBIS" }
                , new Dictionary<string, string> { ["columnName"] = "INTERESTED_FSS" }
                , new Dictionary<string, string> { ["columnName"] = "INTERESTED_FSS_CHI" }
            };


            dg.QueryParameters.Add("REGISTRATION_TYPE", regType);
            dg.QueryParameters.Add("categoryCode", cat1.UUID);

            if (cat2 != null)
            {
                dg.QueryParameters.Add("secCategoryCode", cat2.UUID);
            }



            dg.Query = q;
                dg.QueryWhere = whereQ;
                dg.SortType = 0;
                dg.Sort = "upper(APP.SURNAME) || ' ' || upper(APP.GIVEN_NAME_ON_ID) ,  C.CERTIFICATION_NO, C.UUID, BSV.ORDERING ";

                dg.Rpp = -1;
                dg.Search();
                List<object[]> r = new List<object[]>();
                for (int i = 0; i < dg.Data.Count; i++)
                {
                    List<object> row = new List<object>();
                    for (int j = 0; j < dg.Columns.Length; j++)
                        row.Add(dg.Data[i][dg.Columns[j]["columnName"]]);
                    r.Add(row.ToArray());
                }
                return r;


            }


    }
    public class CompApplicantInfoDAO
    {
        public List<C_COMP_APPLICANT_INFO> findByCompApplication(C_COMP_APPLICATION application)
        {
            return null;
        }

    }

    public class CompApplicationManager
    {
        public List<C_COMP_APPLICATION> getCompApplicationForDataExport(string registrationType, string categoryUUID, string applicantRoleUUID, string pnrcUUID)
        {
            return null;
        }

        public List<object[]> getMWCompApplicationForDataExportRegisters(string regType, string categoryCodeUUID)
        {





            string query = ""
                + "\r\n\t" + " SELECT C.UUID AS MASTER_ID                                                                                              "
                + "\r\n\t" + " , C.FILE_REFERENCE_NO                                                                                                   "
                + "\r\n\t" + " , C.CERTIFICATION_NO                                                                                                    "
                + "\r\n\t" + " , C.ENGLISH_COMPANY_NAME                                                                                                "
                + "\r\n\t" + " , C.CHINESE_COMPANY_NAME                                                                                                "
                + "\r\n\t" + " , C.EXPIRY_DATE,C.BS_TELEPHONE_NO1 as TELEPHONE_NO                                                                      "
                + "\r\n\t" + " , C_GET_MW_COMP_CLASS_TYPE(C.UUID, 1) AS COMPANY_TYPE_ONE                                                               "
                + "\r\n\t" + " , C_GET_MW_COMP_CLASS_TYPE(C.UUID, 2) AS COMPANY_TYPE_TWO                                                               "
                + "\r\n\t" + " , C_GET_MW_COMP_CLASS_TYPE(C.UUID, 3) AS COMPANY_TYPE_THREE                                                             "
                + "\r\n\t" + " , APP_INFO.UUID AS COMP_APPLICANT_INFO_ID                                                                               "
                + "\r\n\t" + " , APP_INFO.SURNAME                                                                                                      "
                + "\r\n\t" + " , APP_INFO.GIVEN_NAME_ON_ID                                                                                             "
                + "\r\n\t" + " , APP_INFO.CHINESE_NAME                                                                                                 "
                + "\r\n\t" + " , C_GET_MW_COMP_AS_CLASS_TYPE(APP_INFO.UUID, 3) AS AS_TYPE_ONE                                                          "
                + "\r\n\t" + " , C_GET_MW_COMP_AS_CLASS_TYPE(APP_INFO.UUID, 2) AS AS_TYPE_TWO                                                          "
                + "\r\n\t" + " , C_GET_MW_COMP_AS_CLASS_TYPE(APP_INFO.UUID, 1) AS AS_TYPE_THREE                                                        "
                + "\r\n\t" + " , (SELECT REGION_CHI.ENGLISH_DESCRIPTION  FROM C_S_SYSTEM_VALUE REGION_CHI                                              "
                + "\r\n\t" + " WHERE  	  REGION_CHI.UUID= C.BS_REGION_CODE_ID )  AS REGION_ENG                                                        "
                + "\r\n\t" + " , (SELECT REGION_CHI.CHINESE_DESCRIPTION   FROM C_S_SYSTEM_VALUE REGION_CHI                                             "
                + "\r\n\t" + " WHERE REGION_CHI.UUID= C.BS_REGION_CODE_ID )  AS REGION_CHI                                                             "
                + "\r\n\t" + " ,  C.BS_EMAIL_ADDRESS AS EMAIL_ADDRESS                                                                                  "
                + "\r\n\t" + " , C.BS_FAX_NO1                                                                                                          "
                + "\r\n\t" + " , C_GET_MWC_HTML_STAR(C.BR_NO) AS STAR                                                                                  "
                + "\r\n\t" + " , CASE WHEN TRUNC(CURRENT_DATE) > C.EXPIRY_DATE AND add_months(C.retention_application_date, 24) > trunc(current_date)  "
                + "\r\n\t" + " THEN '@' ELSE '' END as flag                                                                                            "
                + "\r\n\t" + " , SVALUE_FSS.ENGLISH_DESCRIPTION AS INTERESTED_FSS                                                                      "
                + "\r\n\t" + " , SVALUE_FSS.CHINESE_DESCRIPTION AS INTERESTED_FSS_CHI                                                                  "

                + "\r\n\t" + " FROM  C_COMP_APPLICATION C                                                                                                       "
                + "\r\n\t" + " LEFT OUTER JOIN (                                                                                                                "
                + "\r\n\t" + " SELECT A.UUID, A.MASTER_ID,  APP.SURNAME , APP.GIVEN_NAME_ON_ID, APP.CHINESE_NAME 			                                     "
                + "\r\n\t" + " FROM C_COMP_APPLICANT_INFO A, C_APPLICANT APP, C_S_SYSTEM_VALUE S_ROLE, C_S_SYSTEM_VALUE S_STATUS 						         "
                + "\r\n\t" + " WHERE A.APPLICANT_ID=APP.UUID 						                                                                             "
                + "\r\n\t" + " AND A.APPLICANT_ROLE_ID=S_ROLE.UUID 						                                                                     "
                + "\r\n\t" + " AND A.APPLICANT_STATUS_ID= S_STATUS.UUID  						                                                                 "
                + "\r\n\t" + " AND S_STATUS.CODE= :activeCode AND S_ROLE.CODE LIKE 'A%'  						                                                 "
                + "\r\n\t" + " AND A.accept_date IS NOT NULL  						                                                                             "
                + "\r\n\t" + " AND ((A.REMOVAL_DATE IS NULL) OR (A.REMOVAL_DATE >= TRUNC(CURRENT_DATE)) ) )	APP_INFO ON C.UUID= APP_INFO.MASTER_ID           "
                + "\r\n\t" + " LEFT JOIN C_S_SYSTEM_TYPE STYPE_FSS  ON  STYPE_FSS.TYPE = 'FSS_DROPDOWN'                                                         "
                + "\r\n\t" + " LEFT JOIN C_S_SYSTEM_VALUE SVALUE_FSS  ON  STYPE_FSS.UUID = SVALUE_FSS.SYSTEM_TYPE_ID                                            "
                + "\r\n\t" + " AND NVL(C.INTERESTED_FSS, 'I') = SVALUE_FSS.CODE                                                                                 ";



            string whereQ = " WHERE 1=1"
                + "\r\n\t" + " AND C.REGISTRATION_TYPE = :regType    	                                            "
                + "\r\n\t" + " AND C.CATEGORY_ID = :catCode    	                                            "
            + "\r\n\t" + " AND          C.CERTIFICATION_NO IS NOT NULL                                                                                      "
            + "\r\n\t" + " AND         ( (C.EXPIRY_DATE IS NOT NULL and  C.EXPIRY_DATE >= TRUNC(CURRENT_DATE)) or                                           "
            + "\r\n\t" + " ( (C.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd')                                                                "
             + "\r\n\t" + " and    (C.EXPIRY_DATE < TRUNC(CURRENT_DATE))        )    ) )                                                                     "
              + "\r\n\t" + " AND ( (C.REMOVAL_DATE IS NULL) OR                 (C.REMOVAL_DATE > TRUNC(CURRENT_DATE))  )                                      ";

            DisplayGrid dg = new DisplayGrid();

            dg.Columns = new Dictionary<string, string>[] {
                  new Dictionary<string, string> { ["columnName"] = "MASTER_ID" }
                 , new Dictionary<string, string> { ["columnName"] = "FILE_REFERENCE_NO" }
                 , new Dictionary<string, string> { ["columnName"] = "CERTIFICATION_NO" }
                 , new Dictionary<string, string> { ["columnName"] = "ENGLISH_COMPANY_NAME" }
                 , new Dictionary<string, string> { ["columnName"] = "CHINESE_COMPANY_NAME" }
                 , new Dictionary<string, string> { ["columnName"] = "EXPIRY_DATE" }
                 , new Dictionary<string, string> { ["columnName"] = "TELEPHONE_NO" }
                 , new Dictionary<string, string> { ["columnName"] = "COMPANY_TYPE_ONE" }
                 , new Dictionary<string, string> { ["columnName"] = "COMPANY_TYPE_TWO" }
                 , new Dictionary<string, string> { ["columnName"] = "COMPANY_TYPE_THREE" }
                 , new Dictionary<string, string> { ["columnName"] = "COMP_APPLICANT_INFO_ID" }
                 , new Dictionary<string, string> { ["columnName"] = "SURNAME" }
                 , new Dictionary<string, string> { ["columnName"] = "GIVEN_NAME_ON_ID" }
                 , new Dictionary<string, string> { ["columnName"] = "CHINESE_NAME" }
                 , new Dictionary<string, string> { ["columnName"] = "AS_TYPE_ONE" }
                 , new Dictionary<string, string> { ["columnName"] = "AS_TYPE_TWO" }
                 , new Dictionary<string, string> { ["columnName"] = "AS_TYPE_THREE" }
                 , new Dictionary<string, string> { ["columnName"] = "REGION_ENG" }
                 , new Dictionary<string, string> { ["columnName"] = "REGION_CHI" }
                 , new Dictionary<string, string> { ["columnName"] = "EMAIL_ADDRESS" }
                 , new Dictionary<string, string> { ["columnName"] = "BS_FAX_NO1" }
                 , new Dictionary<string, string> { ["columnName"] = "STAR" }
                 , new Dictionary<string, string> { ["columnName"] = "FLAG" }
                 , new Dictionary<string, string> { ["columnName"] = "INTERESTED_FSS" }
                 , new Dictionary<string, string> { ["columnName"] = "INTERESTED_FSS_CHI" }
            };


            dg.QueryParameters.Add("activeCode", RegistrationConstant.STATUS_ACTIVE);
            dg.QueryParameters.Add("regType", regType);
           dg.QueryParameters.Add("catCode", categoryCodeUUID);

            
            //dg.Query = "SELECT * FROM AA_TEST_TABLE";
             dg.Query = query;
             dg.QueryWhere = whereQ;
            // dg.Sort = "upper(C.ENGLISH_COMPANY_NAME), C.CERTIFICATION_NO, C.UUID, APP_INFO.SURNAME , APP_INFO.GIVEN_NAME_ON_ID, APP_INFO.UUID";

            dg.SortType = 0;
            dg.Rpp = -1;
            dg.Search();
            List<object[]> r = new List<object[]>();
            for (int i = 0; i < dg.Data.Count; i++)
            {
                List<object> row = new List<object>();
                for (int j = 0; j < dg.Columns.Length; j++)
                    row.Add(dg.Data[i][dg.Columns[j]["columnName"]]);
                r.Add(row.ToArray());
            }
            return r;
        }
        public List<object[]> getMWCompApplicationForDataExportRegistersQP(string type, string catCodeUUID)
        {
            return null;
        }
        public List<object[]> getCompApplicationForDataExportRegisters(string regType, C_S_CATEGORY_CODE cat1, C_S_CATEGORY_CODE cat2)
        {
            string query = ""
                + "\r\n\t" + "SELECT C.UUID,  C.CERTIFICATION_NO,  C.ENGLISH_COMPANY_NAME , C.CHINESE_COMPANY_NAME                                                        "
                + "\r\n\t" + "    ,  C_GET_AS_LIST_HTML(C.UUID) AS AS_NAME, C.EXPIRY_DATE                                                                                   "
                + "\r\n\t" + "    ,  CASE                                                                                                                                 "
                + "\r\n\t" + "    WHEN TRUNC(CURRENT_DATE) > C.EXPIRY_DATE AND add_months(C.retention_application_date, 24) > trunc(current_date)                         "
                + "\r\n\t" + "    THEN 'x' ELSE '' END as flag                                                                                                            "
                + "\r\n\t" + "    ,  SVALUE_FSS.ENGLISH_DESCRIPTION AS INTERESTED_FSS                                                                                     "
                + "\r\n\t" + "    ,  SVALUE_FSS.CHINESE_DESCRIPTION AS INTERESTED_FSS_CHI                                                                                 "
                + "\r\n\t" + "    ,  C.bs_telephone_no1 as TELEPHONE_NO1                                                                                                  "
                + "\r\n\t" + "    ,  BS.BUILDING_SAFETY_ID, C_GET_AS_LIST_WITH_CHINESE_HTML(C.UUID) AS AS_NAME_chiniese                                                     "
                + "\r\n\t" + "FROM C_COMP_APPLICATION C                                                                                                                     "
                + "\r\n\t" + "LEFT OUTER JOIN C_BUILDING_SAFETY_INFO BS ON C.UUID = BS.MASTER_ID AND C.REGISTRATION_TYPE = BS.REGISTRATION_TYPE                             "
                + "\r\n\t" + "LEFT OUTER JOIN C_S_SYSTEM_VALUE BSV ON  BS.BUILDING_SAFETY_ID = BSV.UUID                                                                     "
                + "\r\n\t" + "LEFT JOIN C_S_SYSTEM_TYPE STYPE_FSS  ON STYPE_FSS.TYPE = 'FSS_DROPDOWN'                                                                       "
                + "\r\n\t" + "LEFT JOIN C_S_SYSTEM_VALUE SVALUE_FSS  ON STYPE_FSS.UUID = SVALUE_FSS.SYSTEM_TYPE_ID  AND NVL(C.INTERESTED_FSS, 'I') = SVALUE_FSS.CODE        ";

            string whereQ = "where 1=1";
            if (cat2 != null)
            {
                whereQ = whereQ + "AND ( C.CATEGORY_ID= :categoryCode OR  C.CATEGORY_ID= :secCategoryCode) ";
            }
            else
            {
                whereQ = whereQ + "AND ( C.CATEGORY_ID= :categoryCode) ";
            }

            whereQ = whereQ
            + "\r\n\t" + "AND C.REGISTRATION_TYPE = :regType                                                                                                "
            + "\r\n\t" + "AND C.CERTIFICATION_NO IS NOT NULL                                                                                                          "
            + "\r\n\t" + "AND((C.EXPIRY_DATE IS NOT NULL and  C.EXPIRY_DATE >= TRUNC(CURRENT_DATE))                                                                   "
            + "\r\n\t" + "or((C.RETENTION_APPLICATION_DATE > TO_DATE('20040831', 'yyyymmdd') and(C.EXPIRY_DATE < TRUNC(CURRENT_DATE))) ) )                            "
            + "\r\n\t" + "AND((C.REMOVAL_DATE IS NULL) OR(C.REMOVAL_DATE > TRUNC(CURRENT_DATE))      )                                                                ";
            //    + "\r\n\t" + "ORDER BY upper(C.ENGLISH_COMPANY_NAME) ,      C.CERTIFICATION_NO, C.UUID, BSV.ORDERING                                                      ";


            DisplayGrid dg = new DisplayGrid();

            dg.Columns = new Dictionary<string, string>[] {
                  new Dictionary<string, string> { ["columnName"] = "UUID" }
                , new Dictionary<string, string> { ["columnName"] = "CERTIFICATION_NO" }
                , new Dictionary<string, string> { ["columnName"] = "ENGLISH_COMPANY_NAME" }
                , new Dictionary<string, string> { ["columnName"] = "CHINESE_COMPANY_NAME" }
                , new Dictionary<string, string> { ["columnName"] = "AS_NAME" }
                , new Dictionary<string, string> { ["columnName"] = "EXPIRY_DATE" }
                , new Dictionary<string, string> { ["columnName"] = "FLAG" }
                , new Dictionary<string, string> { ["columnName"] = "INTERESTED_FSS" }
                , new Dictionary<string, string> { ["columnName"] = "INTERESTED_FSS_CHI" }
                , new Dictionary<string, string> { ["columnName"] = "TELEPHONE_NO1" }
                , new Dictionary<string, string> { ["columnName"] = "BUILDING_SAFETY_ID" }
                , new Dictionary<string, string> { ["columnName"] = "AS_NAME_CHINIESE" }
            };


            dg.QueryParameters.Add("categoryCode", cat1.UUID);
            if (cat2 != null) dg.QueryParameters.Add("secCategoryCode", cat2.UUID);

            dg.QueryParameters.Add("regType", regType);


            dg.Query = query;
            dg.QueryWhere = whereQ;
            dg.SortType = 0;
            dg.Sort = "upper(C.ENGLISH_COMPANY_NAME) ,      C.CERTIFICATION_NO, C.UUID, BSV.ORDERING";

            dg.Rpp = -1;
            dg.Search();
            List<object[]> r = new List<object[]>();
            for (int i = 0; i < dg.Data.Count; i++)
            {
                List<object> row = new List<object>();
                for (int j = 0; j < dg.Columns.Length; j++)
                    row.Add(dg.Data[i][dg.Columns[j]["columnName"]]);
                r.Add(row.ToArray());
            }
            return r;
        }

    }


    public class BuildingSafetyInfoManager {
        public List<C_S_SYSTEM_VALUE> getBS(string registrationType, string compApplicationUuid)
        {
            return null;
        }
            //bsManager.getBS(registrationType, compApplication.getUuid()
    }

    
    public class PrintWriter {
        string _v = "";
        Stream _Stream;
        public PrintWriter(Stream steam)
        {
            _Stream = steam;
        }
        public void print(string v)
        {
            _v += v;
        }
        public void println(string v)
        {

            _v += "\r\n" + v;
        }
        public void println()
        {

            _v += "\r\n";
        }
        public void flush()
        {
            byte[] bytes = Encoding.UTF8.GetBytes(_v);
            _Stream.Write(bytes, 0, bytes.Length);
        }
    }
    /*public class DateUtil
    {

        public static int compareDate(DateTime? t1, DateTime t2) {
            return -1;
        }

        public static string getExportDateDisplay(DateTime? t1)
        {
            return null;
        }
        public static string getDateDisplayFormat(DateTime? t1)
        {
            return null;
        }
    }*/
    public class Logger
    {
        public void fatal(string str, Exception ex)
        {

        }
        public void fatal(string str)
        {

        }
        public void info(string str) {
        }
    }
    public class GobalValue
    {
        public static string CRMExportPath = ApplicationConstant.CRMFilePath + "Template";
    }










    public class ExportDataManager
    {


        protected Logger log = new Logger();
        string SEPARATOR = "\t";

        string DOUBLEQUOTE = "";

        string REG_EXCEL_REMARK = "REG_EXCEL_REMARK";
        string REG_EXCEL_NOTE = "REG_EXCEL_NOTE";

        string EXCEL_FILE_EXPORT = "EXCEL_FILE_EXPORT";

        public static string LANG_ENG = "ENG";
        public static string LANG_CHI = "CHI";



        public void exportApplicationData(HttpRequest request, HttpResponse response, ExportDataForm exportDataForm)
        {

            string regType = exportDataForm.getRegistrationType();
            if (regType.Equals(OldApplicationConstants.COMPANY_GENERAL_CONTRACTOR))
            {
                exportCompApplicationData(request, response, exportDataForm);
            }
            else if (regType.Equals(OldApplicationConstants.COMPANY_MINOR_WORK))
            {
                exportCompApplicationData(request, response, exportDataForm);
            }
            else if (regType.Equals(OldApplicationConstants.INDIVIDUAL_PROFESSIONAL))
            {
                exportProfApplicationData(request, response, exportDataForm);
            }
            else if (regType.Equals(OldApplicationConstants.INDIVIDUAL_MINOR_WORK))
            {
                exportProfApplicationData(request, response, exportDataForm);
            }

        }

        private void doPrint(PrintWriter printer, Object col)
        {
            if (col == null)
            {
                col = "";
            }
            printer.print(DOUBLEQUOTE + col + DOUBLEQUOTE);
            printer.print(SEPARATOR);
        }
        private void doPrintln(PrintWriter print, string col)
        {
            print.println(col);
        }
        private void doPrintSep(PrintWriter printer)
        {
            printer.print(SEPARATOR);
        }
        
        private string getBSItem(List<C_S_SYSTEM_VALUE> bs)
        {
            string result = "";
            for (int i = 0; i < bs.Count; i++)
            {
                if (result.Equals(""))
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
        
        private string getQualificationDetails(List<C_IND_QUALIFICATION_DETAIL> bs)
        {
            string result = "";
            for (int i = 0; i < bs.Count; i++)
            {
                if (result.Equals(""))
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

        private C_COMP_APPLICANT_INFO getApplicant(List<C_COMP_APPLICANT_INFO> compApplicantList, string code)
        {

            for (int i = 0; i < compApplicantList.Count; i++)
            {
                C_COMP_APPLICANT_INFO compApplicantInfo = (C_COMP_APPLICANT_INFO)compApplicantList[i];
                if (compApplicantInfo.C_S_SYSTEM_VALUE.CODE.Equals(code))
                {
                    return compApplicantInfo;
                }
            }

            return null;
        }
        private List<C_COMP_APPLICANT_INFO> getApplicants(List<C_COMP_APPLICANT_INFO> compApplicantList, string code)
        {
            List<C_COMP_APPLICANT_INFO> compApplicantInfos = new List<C_COMP_APPLICANT_INFO>();
            for (int i = 0; i < compApplicantList.Count; i++)
            {
                C_COMP_APPLICANT_INFO compApplicantInfo = (C_COMP_APPLICANT_INFO)compApplicantList[i];
                if (compApplicantInfo.C_S_SYSTEM_VALUE.CODE.Equals(code))
                {
                    if (compApplicantInfo.REMOVAL_DATE == null ||
                            OldDateUtil.compareDate(compApplicantInfo.REMOVAL_DATE, DateTime.Now) == 0)
                    {

                        if (compApplicantInfo.C_S_SYSTEM_VALUE1.CODE.Equals(OldApplicationConstants.STATUS_ACTIVE))
                        {
                            compApplicantInfos.Add(compApplicantInfo);
                        }
                    }
                }
            }

            return compApplicantInfos;
        }



        private void exportCompApplicationData(HttpRequest request, HttpResponse response, ExportDataForm exportDataForm)
        {

            string registrationType = exportDataForm.getRegistrationType();
            string categoryUUID = exportDataForm.getCategoryUUID();
            string applicantRoleUUID = exportDataForm.getApplicantRoleUUID();
            string pnrcUUID = exportDataForm.getPnrcUUID();
            bool isOfficeOne = exportDataForm.isSelectONE();
            bool isOfficeTwo = exportDataForm.isSelectONE();
            bool isAddress = exportDataForm.isSelectTWO();
            bool isBS = exportDataForm.isSelectTHREE();

            PrintWriter printWriter = null;
            CompApplicationManager compApplicationManager = null;
            BuildingSafetyInfoManager bsManager = null;
            //Session session = null;
            List<C_COMP_APPLICATION> resultList = null;

            CompApplicantInfoDAO compApplicantInfoDAO = null;
            try {
                //compApplicationManager = (CompApplicationManager)dac.getManager(CompApplicationManager.class.getName());
				//bsManager = (BuildingSafetyInfoManager) dac.getManager(BuildingSafetyInfoManager.class.getName());;
				// get new session
				//session = dac.getHibernateSession();	
				//compApplicantInfoDAO = new C_COMP_APPLICANT_INFODAO(session);

                resultList = compApplicationManager.getCompApplicationForDataExport(registrationType, categoryUUID, applicantRoleUUID, pnrcUUID);

                if (resultList == null){
					resultList = new List<C_COMP_APPLICATION>();
				}
				try {
				//	printWriter = new PrintWriter(response.getOutputStream());
					
					//printWriter = new PrintWriter(new BufferedWriter(

                    //        new OutputStreamWriter(response.OutputStream, "UTF-8")));

                    printWriter = new PrintWriter(response.OutputStream);


                    response.ContentEncoding = System.Text.Encoding.UTF8;
                    //response.setCharacterEncoding("UTF8");	
                    response.ContentType = "application/text";
                    //response.setContentType("application/text");
					//response.setHeader("Content-disposition", "attachment;filename=\""+registrationType+"_Comp.csv\"");
                    response.Headers.Add("Content-disposition", "attachment;filename=\"" + registrationType + "_Comp.csv\"");
                    string[] exportCloumnCommon =
                    { "file_ref", "reg_no", "category", "en_name", "ch_name", "expiry_dt"};

                    string[] exportCloumnAddress = 
                    { "c_o", "addr1", "addr2", "addr3", "addr4", "addr5", "addr_cn1", "addr_cn2", "addr_cn3", "addr_cn4", "addr_cn5", "pnrc"};

                    string[] exportCloumnOfficeOne =
                    { "tel_no1", "tel_no2", "tel_no3", "email"};

                    string[] exportCloumnBS =
                    { "bs_addr1", "bs_addr2", "bs_addr3", "bs_addr4",
                    "bs_addr5", "bs_tel_no1", "bs_fax_no1", "bs_codes"};
                    string[] exportCloumnOfficeTwo =
                    { "fax_no1", "fax_no2", "AS", "AS_chn", "TD", "TD_chn"};
				

					for (int i = 0; i<exportCloumnCommon.Length; i++) {
						doPrint(printWriter, exportCloumnCommon[i]);
}
					if(isAddress){
						for (int i = 0; i<exportCloumnAddress.Length; i++) {
							doPrint(printWriter, exportCloumnAddress[i]);
						}
					}
					
					if(isOfficeOne){
						for (int i = 0; i<exportCloumnOfficeOne.Length; i++) {
							doPrint(printWriter, exportCloumnOfficeOne[i]);
						}
					}
					if(isBS){
						for (int i = 0; i<exportCloumnBS.Length; i++) {
							doPrint(printWriter, exportCloumnBS[i]);
						}	
					}
					if(isOfficeTwo){
						for (int i = 0; i<exportCloumnOfficeTwo.Length; i++) {
							doPrint(printWriter, exportCloumnOfficeTwo[i]);
						}
					}
					printWriter.println();

					for (int i = 0; i<resultList.Count; i++){
                        C_COMP_APPLICATION compApplication = resultList[i];

                        doPrint(printWriter, compApplication.FILE_REFERENCE_NO);
                        doPrint(printWriter, compApplication.CERTIFICATION_NO);
                        doPrint(printWriter, compApplication.C_S_CATEGORY_CODE.CODE);
                        doPrint(printWriter, compApplication.ENGLISH_COMPANY_NAME);
                        doPrint(printWriter, compApplication.CHINESE_COMPANY_NAME);
                        doPrint(printWriter, OldDateUtil.getExportDateDisplay(compApplication.EXPIRY_DATE));

                        if (isAddress){

                            C_ADDRESS address = compApplication.C_ADDRESS2;
                            C_ADDRESS address_cn = compApplication.C_ADDRESS;
							if(address == null){
								address = new C_ADDRESS();
							}
							if(address_cn == null){
								address_cn = new C_ADDRESS();
							}
							
							doPrint(printWriter, compApplication.ENGLISH_CARE_OF);
                            doPrint(printWriter, address.ADDRESS_LINE1);
                            doPrint(printWriter, address.ADDRESS_LINE2);
                            doPrint(printWriter, address.ADDRESS_LINE3);
                            doPrint(printWriter, address.ADDRESS_LINE4);
                            doPrint(printWriter, address.ADDRESS_LINE5);

                            doPrint(printWriter, address_cn.ADDRESS_LINE1);
                            doPrint(printWriter, address_cn.ADDRESS_LINE2);
                            doPrint(printWriter, address_cn.ADDRESS_LINE3);
                            doPrint(printWriter, address_cn.ADDRESS_LINE4);
                            doPrint(printWriter, address_cn.ADDRESS_LINE5);

                            //for printing "pnrc" column
                            C_S_SYSTEM_VALUE prac = compApplication.C_S_SYSTEM_VALUE3;

							if(prac ==null){
								doPrint(printWriter, "");
							}else{
								doPrint(printWriter, prac.ENGLISH_DESCRIPTION);
							}
							
						}
				
						if(isOfficeOne){
							doPrint(printWriter, compApplication.TELEPHONE_NO1);
                            doPrint(printWriter, compApplication.TELEPHONE_NO2);
                            doPrint(printWriter, compApplication.TELEPHONE_NO3);
                            doPrint(printWriter, compApplication.EMAIL_ADDRESS);
                        }
						if(isBS){
							
							C_ADDRESS address = compApplication.C_ADDRESS3;
							
							if(address == null){
								address = new C_ADDRESS();
							}
							
                            doPrint(printWriter, address.ADDRESS_LINE1);
                            doPrint(printWriter, address.ADDRESS_LINE2);
                            doPrint(printWriter, address.ADDRESS_LINE3);
                            doPrint(printWriter, address.ADDRESS_LINE4);
                            doPrint(printWriter, address.ADDRESS_LINE5);
                            doPrint(printWriter, compApplication.BS_TELEPHONE_NO1);
                            doPrint(printWriter, compApplication.BS_FAX_NO1);
                            doPrint(printWriter, this.getBSItem(bsManager.getBS(registrationType, compApplication.UUID)));
						}
						if(isOfficeTwo){
							doPrint(printWriter, compApplication.FAX_NO1);
                            doPrint(printWriter, compApplication.FAX_NO2);

                            List<C_COMP_APPLICANT_INFO> compApplicantList = compApplicantInfoDAO.findByCompApplication(compApplication);




//C_COMP_APPLICANT_INFO compApplicantInfo = getApplicant(compApplicantList, "AS");
List<C_COMP_APPLICANT_INFO> compApplicantInfos = getApplicants(compApplicantList, "AS");
							

							if(compApplicantInfos != null && compApplicantInfos.Count > 0){
								string eng_names = "";
string chi_names = "";
								for(int j=  0 ;j<compApplicantInfos.Count;j++) {
                                    C_APPLICANT applicant = compApplicantInfos[j].C_APPLICANT;
									if(j > 0) eng_names += " | ";
									if(j > 0) chi_names += " | ";
									
									if(applicant.SURNAME == null && applicant.GIVEN_NAME_ON_ID == null) {
										if(applicant.CHINESE_NAME == null)
											eng_names += "";
										else
											eng_names += applicant.CHINESE_NAME;
									} else {
										eng_names += applicant.SURNAME+" "+applicant.GIVEN_NAME_ON_ID;
									}
									if(applicant.CHINESE_NAME == null) {
										if(applicant.SURNAME == null && applicant.GIVEN_NAME_ON_ID == null)
											chi_names += "";
										else
											chi_names += applicant.SURNAME +" "+ applicant.GIVEN_NAME_ON_ID;
									} else {
										chi_names += applicant.CHINESE_NAME;
									}
								}
								 doPrint(printWriter, eng_names);
doPrint(printWriter, chi_names);
						    }else{
						    	doPrint(printWriter, "");
doPrint(printWriter, "");
						    }
							 compApplicantInfos = getApplicants(compApplicantList, "TD");
	
							if(compApplicantInfos != null && compApplicantInfos.Count > 0){
								string eng_names = "";
string chi_names = "";
								for(int j=  0 ;j<compApplicantInfos.Count;j++) {
                                    C_APPLICANT applicant = compApplicantInfos[j].C_APPLICANT;
									if(j > 0) eng_names += " | ";
									if(j > 0) chi_names += " | ";
									if(applicant.SURNAME == null && applicant.GIVEN_NAME_ON_ID == null) {
										if(applicant.CHINESE_NAME == null)
											eng_names += "";
										else
											eng_names += applicant.CHINESE_NAME;
									} else {
										eng_names += applicant.SURNAME+" "+applicant.GIVEN_NAME_ON_ID;
									}
									if(applicant.CHINESE_NAME == null) {
										if(applicant.SURNAME == null && applicant.GIVEN_NAME_ON_ID == null)
											chi_names += "";
										else
											chi_names += applicant.SURNAME +" "+ applicant.GIVEN_NAME_ON_ID;
									} else {
										chi_names += applicant.CHINESE_NAME;
									}
								}
								 doPrint(printWriter, eng_names);
doPrint(printWriter, chi_names);
						    }else{
						    	doPrint(printWriter, "");
doPrint(printWriter, "");
						    }
							
							
							
							
							/*if(compApplicantInfo != null){
						    	Applicant applicant= compApplicantInfo.getApplicant();
								doPrint(printWriter,  applicant.SURNAME+" "+applicant.GIVEN_NAME_ON_ID);
  					        	doPrint(printWriter, applicant.CHINESE_NAME);
						    }else{
						    	doPrint(printWriter, "");
						    	doPrint(printWriter, "");
						    }
							compApplicantInfo = getApplicant(compApplicantList, "TD");
						    if(compApplicantInfo != null){
						    	Applicant applicant= compApplicantInfo.getApplicant();
								doPrint(printWriter,  applicant.SURNAME+" "+applicant.GIVEN_NAME_ON_ID);
						    	doPrint(printWriter, applicant.CHINESE_NAME);
						    }else{
						    	doPrint(printWriter, "");
						    	doPrint(printWriter, "");
						    }*/
						}
						printWriter.println();
					}
					printWriter.flush();
				}catch(Exception e){
					log.fatal("Output error ", e);
				}
		//} catch (ManagerNotFoundException mnfe) {
		//	log.fatal("Manager not found!", mnfe);
		//} catch (HibernateException e) {
		//	log.fatal("Error exportCompApplicationData: " + e.getMessage());
		} catch (Exception ex) {
			log.fatal("Exception - exportCompApplicationData " + ex.Message);
		} finally {
			//dac.returnManager(compApplicationManager);
			//dac.returnManager(bsManager);
			//dac.closeHibernateSession(session);
		}
        }

        private void exportProfApplicationData(HttpRequest request, HttpResponse response, ExportDataForm exportDataForm)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {

                string registrationType = exportDataForm.getRegistrationType();
                string categoryUUID = exportDataForm.getCategoryUUID();
                string pnrcUUID = exportDataForm.getPnrcUUID();

                bool isTelNo = exportDataForm.isSelectONE();
                bool isEmergency = exportDataForm.isSelectTWO();
                bool isAddress = exportDataForm.isSelectTHREE();
                bool isBS = exportDataForm.isSelectFour();
                bool isPRB = exportDataForm.isSelectFive();

                PrintWriter printWriter = null;

                IndApplicationManager indApplicationManager = null;
                BuildingSafetyInfoManager bsManager = null;

                IndQualificationDetailDAO indQualificationDetailDAO = null;
                //Session session = null;
                List<object[]> resultList = null;

                try
                {
                    //indApplicationManager = (IndApplicationManager)dac.getManager(IndApplicationManager.class.getName());
                    //bsManager = (BuildingSafetyInfoManager) dac.getManager(BuildingSafetyInfoManager.class.getName());;
                    // get new session
                    //session = dac.getHibernateSession();	

                    indQualificationDetailDAO = new IndQualificationDetailDAO();
                    if (registrationType.Equals(OldApplicationConstants.INDIVIDUAL_PROFESSIONAL))
                    {
                        string[] arrCategoryCode = exportDataForm.getArrCategoryUUID();
                        List<string> categoryCodeList = new List<string>();
                        for (int i = 0; i < arrCategoryCode.Length; i++)
                        {
                            categoryCodeList.Add(arrCategoryCode[i]);
                            if (arrCategoryCode[i].Equals("AP(A)"))
                            {
                                categoryCodeList.Add("API");
                            }
                            if (arrCategoryCode[i].Equals("AP(E)"))
                            {
                                categoryCodeList.Add("APII");
                            }
                            if (arrCategoryCode[i].Equals("AP(S)"))
                            {
                                categoryCodeList.Add("APIII");
                            }
                        }
                        resultList = indApplicationManager.getIndApplicationForDataExport(
                                   registrationType,
                                  categoryCodeList, pnrcUUID, isPRB);

                    }
                    else
                    {
                        resultList = indApplicationManager.getIndApplicationForDataExport(
                                       registrationType,
                                      categoryUUID, pnrcUUID, isPRB);
                    }
                    if (resultList == null)
                    {
                        resultList = new List<object[]>();
                    }
                    try
                    {
                        printWriter = new PrintWriter(response.OutputStream);

                        //printWriter = new PrintWriter(new BufferedWriter(
                        //        new OutputStreamWriter(response.getOutputStream(), "UTF-8")));
                        response.ContentEncoding = Encoding.UTF8;// ("UTF8");
                        //response.setCharacterEncoding("UTF8");
                        response.ContentType ="application/text";
                        response.AddHeader("Content-disposition", "attachment;filename=\"" + registrationType + "_Indv.csv\"");

                        string[] exportCloumnCommon =
                        { "file_ref", "reg_no", "category",
                    "surname", "given_name", "ch_name", "expiry_dt"};
                        string[] exportCloumnTelNo =
                        { "tel_no1", "tel_no2", "tel_no3", "email", "fax_no1", "fax_no2"};
                        string[] exportCloumnEmergency =
                        { "emrg_no1", "emrg_no2", "emrg_no3"};
                        string[] exportCloumnAddress =
                        { "c_o", "pnrc", "addr1", "addr2", "addr3", "addr4", "addr5", "addr_cn1", "addr_cn2", "addr_cn3", "addr_cn4", "addr_cn5"};
                        string[] exportCloumnBS =
                        { "bs_addr1", "bs_addr2", "bs_addr3", "bs_addr4",
                    "bs_addr5", "bs_tel_no1", "bs_fax_no1", "bs_codes"};
                        string[] exportCloumnPRB =
                        { "prb_code", "prb_reg_no", "prb_expiry_dt", "prb_ctr_code", "prb_details"};
                        for (int i = 0; i < exportCloumnCommon.Length; i++)
                        {
                            doPrint(printWriter, exportCloumnCommon[i]);
                        }
                        if (isTelNo)
                        {
                            for (int i = 0; i < exportCloumnTelNo.Length; i++)
                            {
                                doPrint(printWriter, exportCloumnTelNo[i]);
                            }
                        }
                        if (isEmergency)
                        {
                            for (int i = 0; i < exportCloumnEmergency.Length; i++)
                            {
                                doPrint(printWriter, exportCloumnEmergency[i]);
                            }
                        }
                        if (isAddress)
                        {
                            for (int i = 0; i < exportCloumnAddress.Length; i++)
                            {
                                doPrint(printWriter, exportCloumnAddress[i]);
                            }
                        }

                        if (isBS)
                        {
                            for (int i = 0; i < exportCloumnBS.Length; i++)
                            {
                                doPrint(printWriter, exportCloumnBS[i]);
                            }
                        }

                        if (isPRB)
                        {
                            for (int i = 0; i < exportCloumnPRB.Length; i++)
                            {
                                doPrint(printWriter, exportCloumnPRB[i]);
                            }
                        }
                        printWriter.println();


                        for (int i = 0; i < resultList.Count; i++)
                        {
                            Object[] objectList = (Object[])resultList[i];

                            C_IND_APPLICATION indApplication = (C_IND_APPLICATION)objectList[0];
                            C_IND_CERTIFICATE indCertificates = (C_IND_CERTIFICATE)objectList[1];

                            C_IND_QUALIFICATION indQualifications = null;

                            if (isPRB)
                            {
                                indQualifications = (C_IND_QUALIFICATION)objectList[2];
                            }
                            else
                            {
                                indQualifications = new C_IND_QUALIFICATION();
                            }
                            //System.out.println(indApplication.getUuid());

                            C_APPLICANT applicant = indApplication.C_APPLICANT;

                            doPrint(printWriter, indApplication.FILE_REFERENCE_NO);
                            doPrint(printWriter, indCertificates.CERTIFICATION_NO);
                            doPrint(printWriter, indCertificates.C_S_CATEGORY_CODE.CODE);
                            doPrint(printWriter, applicant.SURNAME);
                            doPrint(printWriter, applicant.GIVEN_NAME_ON_ID);
                            doPrint(printWriter, applicant.CHINESE_NAME);
                            doPrint(printWriter, OldDateUtil.getExportDateDisplay(indCertificates.EXPIRY_DATE));

                            if (isTelNo)
                            {
                                doPrint(printWriter, indApplication.TELEPHONE_NO1);
                                doPrint(printWriter, indApplication.TELEPHONE_NO2);
                                doPrint(printWriter, indApplication.TELEPHONE_NO3);
                                doPrint(printWriter, indApplication.EMAIL);
                                doPrint(printWriter, indApplication.FAX_NO1);
                                doPrint(printWriter, indApplication.FAX_NO2);
                            }

                            if (isEmergency)
                            {
                                doPrint(printWriter, indApplication.EMERGENCY_NO1);
                                doPrint(printWriter, indApplication.EMERGENCY_NO2);
                                doPrint(printWriter, indApplication.EMERGENCY_NO3);
                            }

                            if (isAddress)
                            {
                                C_ADDRESS address;
                                C_ADDRESS address_cn;
                                if (indApplication.POST_TO.Equals("H"))
                                {
                                    address = indApplication.C_ADDRESS;
                                    address_cn = indApplication.C_ADDRESS1;
                                }
                                else
                                {
                                    address = indApplication.C_ADDRESS3;
                                    address_cn = indApplication.C_ADDRESS2;
                                }
                                if (address == null)
                                {
                                    address = new C_ADDRESS();
                                }
                                if (address_cn == null)
                                {
                                    address_cn = new C_ADDRESS();
                                }

                                C_S_SYSTEM_VALUE prac = indApplication.C_S_SYSTEM_VALUE1;

                                doPrint(printWriter, indApplication.ENGLISH_CARE_OF);
                                if (prac == null)
                                {
                                    doPrint(printWriter, "");
                                }
                                else
                                {
                                    doPrint(printWriter, prac.ENGLISH_DESCRIPTION);
                                }

                                doPrint(printWriter, address.ADDRESS_LINE1);
                                doPrint(printWriter, address.ADDRESS_LINE2);
                                doPrint(printWriter, address.ADDRESS_LINE3);
                                doPrint(printWriter, address.ADDRESS_LINE4);
                                doPrint(printWriter, address.ADDRESS_LINE5);

                                doPrint(printWriter, address_cn.ADDRESS_LINE1);
                                doPrint(printWriter, address_cn.ADDRESS_LINE2);
                                doPrint(printWriter, address_cn.ADDRESS_LINE3);
                                doPrint(printWriter, address_cn.ADDRESS_LINE4);
                                doPrint(printWriter, address_cn.ADDRESS_LINE5);

                            }

                            if (isBS)
                            {
                                C_ADDRESS address = indApplication.C_ADDRESS4;
                                if (address == null)
                                {
                                    address = new C_ADDRESS();
                                }
                                doPrint(printWriter, address.ADDRESS_LINE1);
                                doPrint(printWriter, address.ADDRESS_LINE2);
                                doPrint(printWriter, address.ADDRESS_LINE3);
                                doPrint(printWriter, address.ADDRESS_LINE4);
                                doPrint(printWriter, address.ADDRESS_LINE5);
                                doPrint(printWriter, indApplication.BS_TELEPHONE_NO1);
                                doPrint(printWriter, indApplication.BS_FAX_NO1);
                                doPrint(printWriter, this.getBSItem(bsManager.getBS(registrationType, indApplication.UUID)));
                            }
                            if (isPRB)
                            {

                                if (indQualifications == null)
                                {
                                    doPrint(printWriter, "");
                                    doPrint(printWriter, "");
                                    doPrint(printWriter, "");
                                    doPrint(printWriter, "");
                                    doPrint(printWriter, "");
                                }
                                else
                                {
                                    doPrint(printWriter, indQualifications.C_S_SYSTEM_VALUE1.CODE);
                                    doPrint(printWriter, indQualifications.REGISTRATION_NUMBER);
                                    doPrint(printWriter, OldDateUtil.getExportDateDisplay(indQualifications.EXPIRY_DATE));
                                    doPrint(printWriter, indQualifications.C_S_CATEGORY_CODE.CODE);
                                    List<C_IND_QUALIFICATION_DETAIL> indList = db.C_IND_QUALIFICATION_DETAIL.Where(o => o.C_IND_QUALIFICATION.UUID == indQualifications.UUID).ToList();
                                   //     (List<C_IND_QUALIFICATION_DETAIL>)indQualificationDetailDAO.findByProperty("indQualification", indQualifications);
                                    doPrint(printWriter, this.getQualificationDetails(indList));
                                }
                            }
                            printWriter.println();
                        }
                        printWriter.flush();
                    }
                    catch (Exception e)
                    {
                        log.fatal("Output error ", e);
                    }
                }
                //catch (ManagerNotFoundException mnfe)
                //{
                //    log.fatal("Manager not found!", mnfe);
                //}
                //catch (HibernateException e)
                //{
                //    log.fatal("Error exportProfApplicationData: " + e.getMessage());
                //}
                catch (Exception ex)
                {
                    log.fatal("Exception - exportProfApplicationData " + ex.Message);
                }
                finally
                {

                    //dac.returnManager(indApplicationManager);
                    //dac.returnManager(bsManager);
                    //dac.closeHibernateSession(session);
                }
            }

        }
        private string getTempFolder(HttpRequestBase request)
        {
            string exportPath = GobalValue.CRMExportPath;// ((gov.bd.mwms.crm.core.system.Dac)dac).getExportFolderPath();
            string tempFolder = exportPath + OldApplicationConstants.FILE_SEPARATOR +
                CommonUtil.NewUuid();// request.getSession().getId();
            //File tFile = new File(tempFolder);
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);// tFile.mkdirs();
            }
            return tempFolder;
        }
        private string getTemplateFolder(string typeName)
        {
            string exportPath = GobalValue.CRMExportPath;// ((gov.bd.mwms.crm.core.system.Dac)dac).getExportFolderPath();
            string tempFolder = exportPath + OldApplicationConstants.FILE_SEPARATOR +
                                typeName + OldApplicationConstants.FILE_SEPARATOR;
            return tempFolder;
        }
        private string getZipFile(string filePath, string filename, List<FileInfo> files)//throws Exception
        {
            string zipFile = filePath + ApplicationConstant.FileSeparator + filename;
            //using (FileStream zipToOpen = new FileStream(@zipFile, FileMode.OpenOrCreate))
            //{
                using (ZipArchive archive = ZipFile.Open(zipFile, ZipArchiveMode.Create))
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        archive.CreateEntryFromFile(files[i].FullName, files[i].Name);
                    }
                
                }
            
             //   zipToOpen.Close();

            //}
            return zipFile;
        }






        private List<object[]> getCustomCategoryCode(List<C_S_CATEGORY_CODE> codeList)
        {
            string[] API = new string[] { "API", "AP(A)" };
            string[] APII = new string[] { "APII", "AP(E)" };
            string[] APIII = new string[] { "APIII", "AP(S)" };

            Dictionary<string, C_S_CATEGORY_CODE> lookupCategoryCode = new Dictionary<string, C_S_CATEGORY_CODE>();
            for (int i = 0; i < codeList.Count; i++)
            {
                C_S_CATEGORY_CODE categoryCode = codeList[i];
                lookupCategoryCode.Add(categoryCode.CODE, categoryCode);
            }
            List<object[]> result = new List<object[]>();
            for (int i = 0; i < codeList.Count; i++)
            {
                C_S_CATEGORY_CODE categoryCode = (C_S_CATEGORY_CODE)codeList[i];
                C_S_CATEGORY_CODE seccategoryCode = null;
                string code = categoryCode.CODE;

                if (code.Equals(API[0]))
                {
                    seccategoryCode = lookupCategoryCode[API[1]];
                }
                if (code.Equals(APII[0]))
                {
                    seccategoryCode = lookupCategoryCode[APII[1]];
                }
                if (code.Equals(APIII[0]))
                {
                    seccategoryCode = lookupCategoryCode[APIII[1]];
                }

                if (code.Equals(API[1]) || code.Equals(APII[1]) || code.Equals(APIII[1]))
                {
                }
                else
                {
                    result.Add(new Object[] { categoryCode, seccategoryCode });
                }

            }
            return result;
        }

        private List<object[]> getCustomCategoryCodeQP(List<C_S_CATEGORY_CODE> codeList)
        {

            List<object[]> result = new List<object[]>();
            string[] qp_arr = { "AP(A)", "RSE", "RI(A)", "GBC", "MWC", "MWC(W)" };
            List<string> qp_list = qp_arr.ToList();// Arrays.asList(qp_arr);

            for (int i = 0; i < codeList.Count; i++)
            {
                if (qp_list.Contains(codeList[i].CODE))
                {
                    result.Add(new object[] { codeList[i], null });
                }
            }

            return result;
        }

        private string getFileName(string code)
        {
            if (code.Equals("SC(D)"))
            {
                code = "rsc_d";
            }
            else if (code.Equals("SC(F)"))
            {
                code = "rsc_f";
            }
            else if (code.Equals("SC(GI)"))
            {
                code = "rsc_gi";
            }
            else if (code.Equals("SC(SF)"))
            {
                code = "rsc_sf";
            }
            else if (code.Equals("SC(V)"))
            {
                code = "rsc_v";
            }
            return code.ToLower() + "_";
        }

        private string getFileNameQP(string code)
        {
            if (code.IndexOf("AP") != -1)
            {
                code = "qp_ap";
            }
            else if (code.IndexOf("RI(A)") != -1)
            {
                code = "qp_ri";
            }
            else if (code.Equals("MWC(W)"))
            {
                code = "qp_mwc(w)";
            }
            else if (code.Equals("MWC"))
            {
                code = "qp_mwc";
            }
            else if (code.Equals("GBC"))
            {
                code = "qp_gbc";
            }
            else if (code.Equals("RSE"))
            {
                code = "qp_rse";
            }
            return code.ToLower() + "_";
        }


        private string getBoardsComitteesFileName(string code, bool panel)
        {
            if (panel)
            {
                return code + "Panel";
            }
            else
            {
                if (code.Equals("GBC"))
                {
                    return "CRC_gbc";
                }
                else if (code.Equals("SC"))
                {
                    return "CRC_sc";
                }
                else
                {
                    return code;
                }
            }
        }

        public List<FileInfo> getMWWebSite(string catCode, string catCodeUUID, string templateFolder, string filePath, string outputType)
        {
            
            List<FileInfo> result = new List<FileInfo>();
            IndApplicationManager indApplicationManager = new IndApplicationManager();
            CompApplicationManager compApplicationManager = new CompApplicationManager();
            //SystemValueManager systemValueManager = null;
            try
            {
                using (EntitiesRegistration db = new EntitiesRegistration()) {
                    //    indApplicationManager = (IndApplicationManager)dac.getManager(IndApplicationManager.class.getName());
                    //compApplicationManager = (CompApplicationManager) dac.getManager(CompApplicationManager.class.getName());
                    //systemValueManager = (SystemValueManager) dac.getManager(SystemValueManager.class.getName());

                    string file_name = this.getFileName(catCode);

                    C_S_SYSTEM_VALUE title = db.C_S_SYSTEM_VALUE.Where(o => o.CODE == file_name).FirstOrDefault();
                    //C_S_SYSTEM_VALUE title = systemValueManager.getFirstSystemValueByCode(file_name);

                    if (catCode.Equals("MWC(W)")) {

                        List<object[]> registerList = indApplicationManager.getMWIApplicationForDataExportRegisters();



                        result.AddRange(generatorMWWebSite(outputType,
                                OldRegisterHTMLGenerator.LANG_ENG,
                                templateFolder, OldMWRegisterHTMLGenerator.ENGLISH_MWI_TEMPLATE,
                                filePath, "e_" + file_name,
                                        registerList, title == null ? "" : title.ENGLISH_DESCRIPTION));

                        result.AddRange(generatorMWWebSite(outputType,
                             OldRegisterHTMLGenerator.LANG_CHI,
                             templateFolder, OldMWRegisterHTMLGenerator.CHINESE_MWI_TEMPLATE,
                             filePath, "c_" + file_name,
                             registerList, title == null ? "" : title.CHINESE_DESCRIPTION));

                    }
                    else
                    {
                        List<Object[]> registerList = compApplicationManager.getMWCompApplicationForDataExportRegisters(OldApplicationConstants.COMPANY_MINOR_WORK, catCodeUUID);

                        List<OldMWCompanyObjectForHTML> registerData = new List<OldMWCompanyObjectForHTML>();


                        string companyKeyID = "";


                        for (int i = 0; i < registerList.Count; i++)
                        {
                            int j = 0;
                            Object[] register = registerList[i];

                            string masterID = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string fileReferenceNo = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string certificationNo = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string englishCompanyName = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string chineseCompanyName = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string expiryDate = register[j] == DBNull.Value ? "" : OldDateUtil.getDateDisplayFormat((DateTime)register[j]); j++;
                            string telNo = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string classTypeOne = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string classTypeTwo = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string classTypeThree = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string compApplicantInfoID = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string asEnglishSurName = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string asEnglishGivenName = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string asChineseName = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string asClassTypeOne = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string asCLassTypeTwo = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string asClassTypeThree = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);

                            string compRegionEng = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string compRegionChi = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string emailAddress = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string bsFaxNumber = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);

                            string star = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);

                            string flag = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string interestedFSS = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string interestedFSSChi = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);

                            if ("".Equals(asClassTypeOne) && "".Equals(asCLassTypeTwo) && "".Equals(asClassTypeThree))
                            {
                                //continue;
                            }

                            if ("Yes".Equals(interestedFSS) || "YES".Equals(interestedFSS))
                            {
                                interestedFSS = stringUtil.getDisplay("\u2713");
                                interestedFSSChi = stringUtil.getDisplay("\u2713");
                            }
                            else
                            {
                                interestedFSS = "";
                                interestedFSSChi = "";

                            }

                            string asEnglishName = (asEnglishSurName + " " + asEnglishGivenName).ToUpper();

                            OldMWCompanyObjectForHTML companyObject = new OldMWCompanyObjectForHTML();

                            if (!companyKeyID.Equals(masterID))
                            {
                                companyObject = new OldMWCompanyObjectForHTML();
                                companyKeyID = masterID;
                            }
                            else
                            {
                                companyObject = registerData[registerData.Count - 1];
                                registerData.RemoveAt(registerData.Count - 1);
                            }

                            companyObject.setCompRegionChi(compRegionChi);
                            companyObject.setCompRegionEng(compRegionEng);
                            companyObject.setEmailAddress(emailAddress);
                            companyObject.setBsFaxNumber(bsFaxNumber);

                            companyObject.setStar(star);

                            companyObject.setCompanyName(englishCompanyName);
                            companyObject.setCompanyChineseName(chineseCompanyName);

                            companyObject.setTypeOne(classTypeOne);
                            companyObject.setTypeTwo(classTypeTwo);
                            companyObject.setTypeThree(classTypeThree);
                            companyObject.setExpiryDate(expiryDate + flag);
                            companyObject.setRegistrationNumber(fileReferenceNo);
                            companyObject.setTelephoneNumber(telNo);

                            companyObject.setInterestedFSS(interestedFSS);
                            companyObject.setInterestedFSSChi(interestedFSSChi);

                            OldMWCompanyauthorizedSignatoryObject asObject =
                                new OldMWCompanyauthorizedSignatoryObject();

                            asObject.setAsName(asEnglishName);
                            asObject.setAsChineseName(asChineseName);
                            asObject.setTypeOne(asClassTypeOne);
                            asObject.setTypeTwo(asCLassTypeTwo);
                            asObject.setTypeThree(asClassTypeThree);

                            companyObject.addASObject(asObject);

                            registerData.Add(companyObject);

                        }

                        if (catCode.Equals("MWC")) {

                            log.info(catCode + " Number of Records: " + registerData.Count);

                            result.AddRange(generatorMWCompanyWebSite(outputType,
                                    OldRegisterHTMLGenerator.LANG_ENG,
                                    templateFolder, OldMWRegisterHTMLGenerator.ENGLISH_MWC_TEMPLATE,
                                    filePath, "e_" + file_name, registerData, title == null ? "" : title.CHINESE_DESCRIPTION));
                            result.AddRange(generatorMWCompanyWebSite(outputType,
                                    OldRegisterHTMLGenerator.LANG_CHI,
                                    templateFolder, OldMWRegisterHTMLGenerator.CHINESE_MWC_TEMPLATE,
                                    filePath, "c_" + file_name, registerData, title == null ? "" : title.CHINESE_DESCRIPTION));
                        } else {
                            log.info(catCode + " Number of Records: " + registerData.Count);

                            result.AddRange(generatorMWCompanyWebSite(outputType,
                                    OldRegisterHTMLGenerator.LANG_ENG,
                                    templateFolder, OldMWRegisterHTMLGenerator.ENGLISH_MWC_P_TEMPLATE,
                                    filePath, "e_" + file_name, registerData, title == null ? "" : title.CHINESE_DESCRIPTION));
                            result.AddRange(generatorMWCompanyWebSite(outputType,
                                    OldRegisterHTMLGenerator.LANG_CHI,
                                    templateFolder, OldMWRegisterHTMLGenerator.CHINESE_MWC_P_TEMPLATE,
                                    filePath, "c_" + file_name, registerData, title == null ? "" : title.CHINESE_DESCRIPTION));
                        }


                    }
                    //} catch (ManagerNotFoundException mnfe) {
                    //	log.fatal("Manager not found!", mnfe);
                    //} catch (HibernateException e) {
                    //	log.fatal("Error exportCompApplicationData: " + e.getMessage());
                }
		} catch (Exception ex) {
			log.fatal("Exception - exportCompApplicationData " + ex.Message);
		} finally {
			//dac.returnManager(indApplicationManager);
			//dac.returnManager(compApplicationManager);
			//dac.returnManager(systemValueManager);
		}
		
		return result;
	}
	
	public List<FileInfo> getMWWebSiteQP(string catCode, string catCodeUUID,            string templateFolder, string filePath, string outputType)
{

            List<FileInfo> result = new List<FileInfo>();
    IndApplicationManager indApplicationManager = null;
    CompApplicationManager compApplicationManager = null;
    //SystemValueManager systemValueManager = null;

    try
    {
                using (EntitiesRegistration db = new EntitiesRegistration())
                {
                    //indApplicationManager = (IndApplicationManager)dac.getManager(IndApplicationManager.class.getName());
                    //	compApplicationManager = (CompApplicationManager) dac.getManager(CompApplicationManager.class.getName());
                    //	systemValueManager = (SystemValueManager) dac.getManager(SystemValueManager.class.getName());

                    string file_name = this.getFileNameQP(catCode);
                    string findCode = file_name.ToLower();
                    C_S_SYSTEM_VALUE title = db.C_S_SYSTEM_VALUE.Where(o => o.CODE == findCode).FirstOrDefault();
                    //systemValueManager.getFirstSystemValueByCode(file_name.ToLower());
                    //ZHConverter converter = ZHConverter.getInstance(ZHConverter.SIMPLIFIED);

                    if (catCode.Equals("MWC(W)"))
                    {

                        List<Object[]> registerList = indApplicationManager.getMWIApplicationForDataExportRegistersQP();

                        result.AddRange(generatorMWWebSite(outputType,
                                OldRegisterHTMLGenerator.LANG_ENG,
                                templateFolder, OldMWRegisterHTMLGenerator.ENGLISH_QP_MWI_TEMPLATE,
                                filePath, "e_" + file_name,
                                                registerList, title == null ? "" : title.ENGLISH_DESCRIPTION));

                        result.AddRange(generatorMWWebSite(outputType,
                             OldRegisterHTMLGenerator.LANG_CHI,
                             templateFolder, OldMWRegisterHTMLGenerator.CHINESE_QP_MWI_TEMPLATE,
                             filePath, "c_" + file_name,
                             registerList, title == null ? "" : title.CHINESE_DESCRIPTION));

                        /*
                        result.AddRange(MWOldRegisterHTMLGenerator.generatorWebSite(
                                 OldRegisterHTMLGenerator.LANG_SCH,
                                 templateFolder, MWOldRegisterHTMLGenerator.SIMPLIFIED_CHINESE_MWI_TEMPLATE,	
                                 filePath, "s_"+file_name,	
                                 registerList, title == null ? "" : converter.convert(title.CHINESE_DESCRIPTION)));
                        */
                    }
                    else
                    {
                        List<object[]> registerList =
                            compApplicationManager.getMWCompApplicationForDataExportRegistersQP(
                                    OldApplicationConstants.COMPANY_MINOR_WORK, catCodeUUID);

                        List<OldMWCompanyObjectForHTML> registerData = new List<OldMWCompanyObjectForHTML>();


                        string companyKeyID = "";


                        for (int i = 0; i < registerList.Count; i++)
                        {
                            int j = 0;
                            object[] register = registerList[i];

                            string masterID = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string fileReferenceNo = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string certificationNo = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string englishCompanyName = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string chineseCompanyName = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string expiryDate = register[j] == DBNull.Value ? "" : OldDateUtil.getDateDisplayFormat((DateTime)register[j]); j++;
                            string telNo = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string classTypeOne = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string classTypeTwo = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string classTypeThree = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string compApplicantInfoID = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string asEnglishSurName = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string asEnglishGivenName = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string asChineseName = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string asClassTypeOne = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string asCLassTypeTwo = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string asClassTypeThree = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string flag = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string compRegionEng = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string compRegionChi = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string emailAddress = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                            string bsFaxNumber = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);

                            if (asClassTypeOne.IndexOf("A") == -1)
                            {
                                asClassTypeOne = "";
                            }
                            else
                            {
                                asClassTypeOne = "A";
                            }
                            if (asCLassTypeTwo.IndexOf("A") == -1)
                            {
                                asCLassTypeTwo = "";
                            }
                            else
                            {
                                asCLassTypeTwo = "A";
                            }
                            if (asClassTypeThree.IndexOf("A") == -1)
                            {
                                asClassTypeThree = "";
                            }
                            else
                            {
                                asClassTypeThree = "A";
                            }
                            if (classTypeOne.IndexOf("A") == -1)
                            {
                                classTypeOne = "";
                            }
                            else
                            {
                                classTypeOne = "A";
                            }
                            if (classTypeTwo.IndexOf("A") == -1)
                            {
                                classTypeTwo = "";
                            }
                            else
                            {
                                classTypeTwo = "A";
                            }
                            if (classTypeThree.IndexOf("A") == -1)
                            {
                                classTypeThree = "";
                            }
                            else
                            {
                                classTypeThree = "A";
                            }

                            if ("".Equals(asClassTypeOne) && "".Equals(asCLassTypeTwo) && "".Equals(asClassTypeThree))
                            {
                                continue;
                            }

                            string asEnglishName = (asEnglishSurName + " " + asEnglishGivenName).ToUpper();

                            OldMWCompanyObjectForHTML companyObject = new OldMWCompanyObjectForHTML();


                            if (!companyKeyID.Equals(masterID))
                            {
                                companyObject = new OldMWCompanyObjectForHTML();
                                companyKeyID = masterID;
                            }
                            else
                            {
                                companyObject = registerData[registerData.Count - 1];
                                registerData.RemoveAt(registerData.Count - 1);
                            }

                            companyObject.setCompRegionChi(compRegionChi);
                            companyObject.setCompRegionEng(compRegionEng);
                            companyObject.setEmailAddress(emailAddress);
                            companyObject.setBsFaxNumber(bsFaxNumber);


                            companyObject.setCompanyName(englishCompanyName);
                            companyObject.setCompanyChineseName(chineseCompanyName);

                            companyObject.setTypeOne(classTypeOne);
                            companyObject.setTypeTwo(classTypeTwo);
                            companyObject.setTypeThree(classTypeThree);
                            companyObject.setExpiryDate(expiryDate);
                            companyObject.setRegistrationNumber(fileReferenceNo);
                            companyObject.setTelephoneNumber(telNo);

                            OldMWCompanyauthorizedSignatoryObject asObject = new OldMWCompanyauthorizedSignatoryObject();

                            asObject.setAsName(asEnglishName);
                            asObject.setAsChineseName(asChineseName);
                            asObject.setTypeOne(asClassTypeOne);
                            asObject.setTypeTwo(asCLassTypeTwo);
                            asObject.setTypeThree(asClassTypeThree);

                            companyObject.addASObject(asObject);

                            registerData.Add(companyObject);

                        }

                        if (catCode.Equals("MWC") || catCode.Equals("MWC(P)"))
                        {

                            log.info(catCode + " Number of Records: " + registerData.Count);

                            result.AddRange(generatorMWCompanyWebSite(outputType,
                                    OldRegisterHTMLGenerator.LANG_ENG,
                                    templateFolder, OldMWRegisterHTMLGenerator.ENGLISH_QP_MWC_TEMPLATE,
                                    filePath, "e_" + file_name, registerData, title == null ? "" : title.ENGLISH_DESCRIPTION));

                            result.AddRange(generatorMWCompanyWebSite(outputType,
                                    OldRegisterHTMLGenerator.LANG_CHI,
                                    templateFolder, OldMWRegisterHTMLGenerator.CHINESE_QP_MWC_TEMPLATE,
                                    filePath, "c_" + file_name, registerData, title == null ? "" : title.CHINESE_DESCRIPTION));
                            /*
                            result.AddRange(MWOldRegisterHTMLGenerator.generatorMWCompanyWebSite(
                                    OldRegisterHTMLGenerator.LANG_SCH,
                                    templateFolder, MWOldRegisterHTMLGenerator.SIMPLIFIED_CHINESE_MWC_TEMPLATE,	
                                    filePath, "s_"+file_name,registerData,converter.convert(title == null ? "" : title.CHINESE_DESCRIPTION)));
                            */
                        }


                    }
                    //} catch (ManagerNotFoundException mnfe) {
                    //	log.fatal("Manager not found!", mnfe);
                    //} catch (HibernateException e) {
                    //	log.fatal("Error exportCompApplicationData: " + e.getMessage());
                }
		} catch (Exception ex) {
			log.fatal("Exception - exportCompApplicationData " + ex.Message);
		} finally {
			//dac.returnManager(indApplicationManager);
			//dac.returnManager(compApplicationManager);
			//dac.returnManager(systemValueManager);
		}
		
		return result;
	}

	
	public List<FileInfo> generatorMWWebSite(string outputType,            string lang,            string templateFilePath, string templateName,            string filePath, string fileNameStart,            List<Object[]> registerData,            string title)
{

    if ("EXCEL".Equals(outputType))
    {
        List<C_S_SYSTEM_VALUE> remarkList = new List<C_S_SYSTEM_VALUE>();
        List<C_S_SYSTEM_VALUE> noteList = new List<C_S_SYSTEM_VALUE>();
        C_S_SYSTEM_VALUE sValue = new C_S_SYSTEM_VALUE();
        //SystemValueManager systemValueManager = null;
        try
        {
                    using (EntitiesRegistration db = new EntitiesRegistration()) {
                        //systemValueManager = (SystemValueManager)dac.getManager(SystemValueManager.class.getName());
                        //remarkList = systemValueManager.getSystemValueBySystemType(templateName);
                        //noteList = systemValueManager.getSystemValueBySystemType("REMARK_"+templateName);

                        remarkList = db.C_S_SYSTEM_VALUE.Where(o => o.C_S_SYSTEM_TYPE.TYPE == REG_EXCEL_REMARK).ToList();// systemValueManager.getSystemValueBySystemType(REG_EXCEL_REMARK);
                        noteList = db.C_S_SYSTEM_VALUE.Where(o => o.C_S_SYSTEM_TYPE.TYPE == REG_EXCEL_NOTE).ToList();// systemValueManager.getSystemValueBySystemType(REG_EXCEL_NOTE);
                        string searchCode = fileNameStart.Substring(2);
                        sValue =
                            db.C_S_SYSTEM_VALUE
                            .Where(o => o.REGISTRATION_TYPE == "ALL")
                            .Where(o => o.C_S_SYSTEM_TYPE.TYPE == EXCEL_FILE_EXPORT)
                            .Where(o => o.CODE == searchCode).FirstOrDefault();
                            //systemValueManager
                            //.getSystemValueByRegistrationTypeAndCode(EXCEL_FILE_EXPORT, "ALL", fileNameStart.substring(2));
				if(!stringUtil.isBlank(sValue.UUID)){
					if(lang.Equals(LANG_ENG)){
						title = sValue.ENGLISH_DESCRIPTION;
					}else{
						title = sValue.CHINESE_DESCRIPTION;
					}
				}
}
			}catch(Exception e){
                    Console.WriteLine(e.Message);
			} finally {
				//dac.returnManager(systemValueManager);
			}
			return OldMWRegisterExcelGenerator.generatorMWWebSite(
                    lang, templateFilePath, templateName,
                     filePath, fileNameStart,
                     registerData,
                     title, noteList, remarkList);
		}else{
			return OldMWRegisterHTMLGenerator.generatorMWWebSite(
                    lang, templateFilePath, templateName,
                     filePath, fileNameStart,
                     registerData,
                     title);
				
		}
				
		
	}
	public List<FileInfo> generatorMWCompanyWebSite(string outputType,
            string lang,
            string templateFilePath, string templateName,
            string filePath, string fileNameStart,
            List<OldMWCompanyObjectForHTML> registerData, string title)
{
    if ("EXCEL".Equals(outputType))
    {

        List<C_S_SYSTEM_VALUE> remarkList = new List<C_S_SYSTEM_VALUE>();
        List<C_S_SYSTEM_VALUE> noteList = new List<C_S_SYSTEM_VALUE>();
        C_S_SYSTEM_VALUE sValue = new C_S_SYSTEM_VALUE();
        //SystemValueManager systemValueManager = null;
        try
                {
                    using (EntitiesRegistration db = new EntitiesRegistration()) { 
                    // systemValueManager = (SystemValueManager)dac.getManager(SystemValueManager.class.getName());
                    //remarkList = systemValueManager.getSystemValueBySystemType(templateName);
                    //noteList = systemValueManager.getSystemValueBySystemType("REMARK_"+templateName);

                    remarkList = db.C_S_SYSTEM_VALUE.Where(o => o.C_S_SYSTEM_TYPE.TYPE == REG_EXCEL_REMARK).ToList();// systemValueManager.getSystemValueBySystemType(REG_EXCEL_REMARK);
                    noteList = db.C_S_SYSTEM_VALUE.Where(o => o.C_S_SYSTEM_TYPE.TYPE == REG_EXCEL_NOTE).ToList();// systemValueManager.getSystemValueBySystemType(REG_EXCEL_NOTE);
                    string searchCode = fileNameStart.Substring(2);
                    sValue =
                        db.C_S_SYSTEM_VALUE
                        .Where(o => o.REGISTRATION_TYPE == "ALL")
                        .Where(o => o.C_S_SYSTEM_TYPE.TYPE == EXCEL_FILE_EXPORT)
                        .Where(o => o.CODE == searchCode).FirstOrDefault();

               //     remarkList = systemValueManager.getSystemValueBySystemType(REG_EXCEL_REMARK);
				//noteList = systemValueManager.getSystemValueBySystemType(REG_EXCEL_NOTE);
				//sValue = systemValueManager.getSystemValueByRegistrationTypeAndCode(EXCEL_FILE_EXPORT, "ALL", fileNameStart.substring(2));
				if(!stringUtil.isBlank(sValue.UUID)){
					if(lang.Equals(LANG_ENG)){
						title = sValue.ENGLISH_DESCRIPTION;
					}else{
						title = sValue.CHINESE_DESCRIPTION;
					}
				}
}
			}catch(Exception e){
                    Console.WriteLine(e.Message);
			} finally {
				//dac.returnManager(systemValueManager);
			}
			return OldMWRegisterExcelGenerator.generatorMWCompanyWebSite(
                    lang,
                     templateFilePath, templateName,
                     filePath, fileNameStart,
                    registerData, title, noteList, remarkList);
		}else{
			return OldMWRegisterHTMLGenerator.generatorMWCompanyWebSite(
                    lang,
                     templateFilePath, templateName,
                     filePath, fileNameStart,
                    registerData, title);
		}
		
	}
	
	public List<FileInfo> generatorWebSite(
            string outputType,
            string langCode, string templateFilePath, string templateName,
            bool isComp, bool isShowBuildingSafetly,
            string filePath, string fileNameStart,
            string title,
            List<string[]> registerList,
            List<C_S_SYSTEM_VALUE> bsitem,
            List<C_S_HTML_NOTES> notes,
            string catCode)
{
    if ("EXCEL".Equals(outputType))
    {


        List<C_S_SYSTEM_VALUE> remarkList = new List<C_S_SYSTEM_VALUE>();
        List<C_S_SYSTEM_VALUE> noteList = new List<C_S_SYSTEM_VALUE>();
        C_S_SYSTEM_VALUE sValue = new C_S_SYSTEM_VALUE();
        //SystemValueManager systemValueManager = null;
        try
        {
                    using(EntitiesRegistration db = new EntitiesRegistration()) { 
            //systemValueManager = (SystemValueManager)dac.getManager(SystemValueManager.class.getName());
				//remarkList = systemValueManager.getSystemValueBySystemType(templateName);
				//remarkList = systemValueManager.getSystemValueBySystemType(REG_EXCEL_REMARK);
				//noteList = systemValueManager.getSystemValueBySystemType(REG_EXCEL_NOTE);
				//sValue = systemValueManager.getSystemValueByRegistrationTypeAndCode(EXCEL_FILE_EXPORT, "ALL", fileNameStart.substring(2));
				
            
             remarkList = db.C_S_SYSTEM_VALUE.Where(o => o.C_S_SYSTEM_TYPE.TYPE == REG_EXCEL_REMARK).ToList();// systemValueManager.getSystemValueBySystemType(REG_EXCEL_REMARK);
        noteList = db.C_S_SYSTEM_VALUE.Where(o => o.C_S_SYSTEM_TYPE.TYPE == REG_EXCEL_NOTE).ToList();// systemValueManager.getSystemValueBySystemType(REG_EXCEL_NOTE);
        string searchCode = fileNameStart.Substring(2);
        sValue =
                            db.C_S_SYSTEM_VALUE
                            .Where(o => o.REGISTRATION_TYPE == "ALL")
                            .Where(o => o.C_S_SYSTEM_TYPE.TYPE == EXCEL_FILE_EXPORT)
                            .Where(o => o.CODE == searchCode).FirstOrDefault();




                        if (sValue != null)
                        {
                            if (!stringUtil.isBlank(sValue.UUID))
                            {
                                if (langCode.Equals(LANG_ENG))
                                {
                                    title = sValue.ENGLISH_DESCRIPTION;
                                }
                                else
                                {
                                    title = sValue.CHINESE_DESCRIPTION;
                                }
                            }

                        }
        
}
			}catch(Exception e){
                    Console.WriteLine(e.Message);
                } finally {
				//dac.returnManager(systemValueManager);
			}
			
			return 	OldRegisterExcelGenerator.generatorWebSite(
                     langCode, templateFilePath, templateName,
                     isComp, isShowBuildingSafetly,
                     filePath, fileNameStart,
                     title,
                     registerList,
                     bsitem,
                     notes,
                     catCode, remarkList, noteList);
		}else{
			return 	OldRegisterHTMLGenerator.generatorWebSite(
                     langCode, templateFilePath, templateName,
                     isComp, isShowBuildingSafetly,
                     filePath, fileNameStart,
                     title,
                     registerList,
                     bsitem,
                     notes,
                     catCode);
		}
		
		

		
		
	}

    public FileStreamResult exportRegistersData(HttpRequestBase request, HttpResponseBase response, ExportDataForm exportDataForm) //throws Exception
    {

    string tempFolder = getTempFolder(request);
    string templateFolder = getTemplateFolder("Register");

    string filePath = tempFolder+OldApplicationConstants.FILE_SEPARATOR;;

            List<FileInfo> webSiteFile = new List<FileInfo>();
            //SystemValueManager systemValueManager = null;
            IndApplicationManager indApplicationManager = new IndApplicationManager();
            CompApplicationManager compApplicationManager = new CompApplicationManager() ;
            //CategoryCodeManager categoryCodeManager = null;
            //HTMLNotesManager hTMLNotesManager = null;
            //Session session = null;

            try {
                using (EntitiesRegistration db = new EntitiesRegistration())
                {
                    if (exportDataForm.getWebSiteFile() != null)
                    {
                        webSiteFile = exportDataForm.getWebSiteFile();
                    }

                    //indApplicationManager = (IndApplicationManager) dac.getManager(IndApplicationManager.class.getName());
                    //compApplicationManager = (CompApplicationManager) dac.getManager(CompApplicationManager.class.getName());
                    //systemValueManager = (SystemValueManager) dac.getManager(SystemValueManager.class.getName());
                    //categoryCodeManager = (CategoryCodeManager) dac.getManager(CategoryCodeManager.class.getName());
                    //hTMLNotesManager = (HTMLNotesManager) dac.getManager(HTMLNotesManager.class.getName());
                    //session = dac.getHibernateSession();	
                    //ZHConverter converter = ZHConverter.getInstance(ZHConverter.SIMPLIFIED);

                    //Register People List for HTML
                    List<Object[]> registerList = null;

                    //Use for Look Up Building Safely Item
                    Dictionary<string, string> lookupBS = new Dictionary<string, string>();


                    //Look Up Title for HTML
                    Dictionary<string, C_S_SYSTEM_VALUE> lookupTitle = new Dictionary<string, C_S_SYSTEM_VALUE>();

                    List<C_S_SYSTEM_VALUE> exportTileList =
                    db.C_S_SYSTEM_VALUE
                        .Where(o => o.C_S_SYSTEM_TYPE.TYPE == OldApplicationConstants.SYSTEM_TYPE_EXPORT_TITLE)
                        .Where(o => o.REGISTRATION_TYPE == "ALL").ToList();
                    //List<C_S_SYSTEM_VALUE> exportTileList = systemValueManager.getActiveSystemValueBySystemTypeAndRegistrationType(
                    // OldApplicationConstants.SYSTEM_TYPE_EXPORT_TITLE, "ALL");
                    for (int j = 0; j < exportTileList.Count; j++)
                    {
                        C_S_SYSTEM_VALUE sSystemValue = exportTileList[j];
                        if (exportDataForm.getRegisterType().Equals("QP") && sSystemValue.CODE.ToLower().IndexOf("qp_") == -1)
                        {
                            continue;
                        }
                        lookupTitle.Add(sSystemValue.CODE.ToLower(), sSystemValue);
                    }

                    //Get the Category Code and Override it
                    // List catCodeListORG = categoryCodeManager.getCategoryCodeList();
                    List<C_S_CATEGORY_CODE> catCodeListORG = db.C_S_CATEGORY_CODE.ToList();
                     List<Object[]> catCodeList = null;
                    if (exportDataForm.getRegisterType().Equals("QP"))
                    {
                        catCodeList = getCustomCategoryCodeQP(catCodeListORG);
                    }
                    else
                    {
                        catCodeList = getCustomCategoryCode(catCodeListORG);
                    }

                    //Loop the overrride Category Code to get the People
                    for (int i = 0; i < catCodeList.Count; i++)
                    {

                        //Get the Category Code Object
                        Object[] catCodeObject = catCodeList[i];
                        C_S_CATEGORY_CODE categoryCode = (C_S_CATEGORY_CODE)catCodeObject[0];
                        C_S_CATEGORY_CODE secCategoryCode = null;
                        if (catCodeObject[1] != null)
                        {
                            secCategoryCode = (C_S_CATEGORY_CODE)catCodeObject[1];
                        }
                        //Get Category Information
                        string catCode = stringUtil.getDisplay(categoryCode.CODE);
                        string regType = stringUtil.getDisplay(categoryCode.REGISTRATION_TYPE);



                        if (regType.Equals(OldApplicationConstants.INDIVIDUAL_MINOR_WORK) ||
                            regType.Equals(OldApplicationConstants.COMPANY_MINOR_WORK))
                        {

                            List<FileInfo> webSiteFiles;
                            if (exportDataForm.getRegisterType().Equals("QP"))
                            {
                                webSiteFiles = getMWWebSiteQP(catCode, categoryCode.UUID, templateFolder, filePath, exportDataForm.getOutputType());
                            }
                            else
                            {
                                webSiteFiles = getMWWebSite(catCode, categoryCode.UUID, templateFolder, filePath, exportDataForm.getOutputType());
                            }
                            webSiteFile.AddRange(webSiteFiles);
                            continue;
                        }
                        else
                        {
                            //if(1==1)continue;
                        }

                        string englishTitle = stringUtil.getDisplay(categoryCode.ENGLISH_DESCRIPTION);
                        string chineseTitle = stringUtil.getDisplay(categoryCode.CHINESE_DESCRIPTION);
                        string simplifiedChineseTitle = chineseTitle;
                        //string catGroupUUID = stringUtil.getDisplay(categoryCode.C_S_SYSTEM_VALUE.UUID);
                        string catGroupUUID = stringUtil.getDisplay(categoryCode.CATEGORY_GROUP_ID);

                        string fileNameStart = "";
                        if (exportDataForm.getRegisterType().Equals("QP"))
                        {
                            fileNameStart = getFileNameQP(stringUtil.getDisplay(catCode));
                        }
                        else
                        {
                            fileNameStart = getFileName(stringUtil.getDisplay(catCode));
                        }
                        //List<C_S_HTML_NOTES> htmlNoteList = hTMLNotesManager.getHTMLNotesByCatGroupUUID(catGroupUUID);
                        List<C_S_HTML_NOTES> htmlNoteList = db.C_S_HTML_NOTES.Where(o => o.CATEGORY_GROUP_ID == catGroupUUID).ToList();
                        List<C_S_SYSTEM_VALUE> bsList =
                        db.C_S_SYSTEM_VALUE
                            .Where(o => o.C_S_SYSTEM_TYPE.TYPE == OldApplicationConstants.SYSTEM_TYPE_BUILDING_SAFETY_CODE)
                            .Where(o => o.REGISTRATION_TYPE == regType).ToList();

                        //List<C_S_SYSTEM_VALUE> bsList = systemValueManager.getActiveSystemValueBySystemTypeAndRegistrationType(
                        //            OldApplicationConstants.SYSTEM_TYPE_BUILDING_SAFETY_CODE, regType);
                        //Set up the Look Up Building Safely
                        for (int j = 0; j < bsList.Count; j++)
                        {
                            C_S_SYSTEM_VALUE sSystemValue = bsList[j];
                            if (lookupBS.ContainsKey(sSystemValue.UUID)) lookupBS[sSystemValue.UUID] = sSystemValue.CODE; else lookupBS.Add(sSystemValue.UUID, sSystemValue.CODE);
                        }

                        //Get Register People List for HTML by Category Code and Sec CategoryCode
                        bool isCompany = false;
                        if (regType.Equals(OldApplicationConstants.COMPANY_GENERAL_CONTRACTOR) ||
                           regType.Equals(OldApplicationConstants.COMPANY_MINOR_WORK))
                        {
                            isCompany = true;
                            if (exportDataForm.getRegisterType().Equals("QP"))
                            {
                                /*
                                if(!regType.Equals(OldApplicationConstants.COMPANY_GENERAL_CONTRACTOR)){
                                    continue;
                                }
                                */
                                registerList = compApplicationManager.getCompApplicationForDataExportRegisters(regType, categoryCode, secCategoryCode);
                                //continue;
                            }
                            else
                            {
                                registerList = compApplicationManager.getCompApplicationForDataExportRegisters(regType, categoryCode, secCategoryCode);
                            }
                        }
                        else
                        {
                            isCompany = false;
                            if (exportDataForm.getRegisterType().Equals("QP"))
                            {
                                registerList = indApplicationManager.getIndApplicationForDataExportRegistersQP(regType, categoryCode, secCategoryCode);
                            }
                            else
                            {
                                registerList = indApplicationManager.getIndApplicationForDataExportRegisters(regType, categoryCode, secCategoryCode);
                            }
                        }
                        if (registerList == null)
                        {
                            registerList = new List<Object[]>();
                        }

                        if (registerList == null || registerList.Count == 0)
                        {
                            if (regType.Equals(OldApplicationConstants.COMPANY_GENERAL_CONTRACTOR) ||
                                regType.Equals(OldApplicationConstants.INDIVIDUAL_PROFESSIONAL))
                            {
                                continue;
                            }
                        }

                        //Register List in English and Chinese
                        List<string[]> registerListsEnglish = new List<string[]>();
                        List<string[]> registerListsChinese = new List<string[]>();
                        List<string[]> registerListsSimplifiedChinese = new List<string[]>();

                        //Register List in English and Chinese in bs

                        Dictionary<string, List<string[]>> bsRegisterListsEnglish = new Dictionary<string, List<string[]>>();
                        Dictionary<string, List<string[]>> bsRegisterListsChinese = new Dictionary<string, List<string[]>>();
                        Dictionary<string, List<string[]>> bsRegisterListsSimplifiedChinese = new Dictionary<string, List<string[]>>();

                        int lastCounter = 0;
                        string lastCerUUID = "";

                        for (int r = 0; r < registerList.Count; r++)
                        {
                            Console.WriteLine(r);
                            Object[] register = registerList[r];
                            string cerUUID, cerNo, englishName, chineseName, schineseName, asNameListEnglish, asNameListChinese, sasNameListChinese,
                            expiryDate, flag, interestedFSS, interestedFSSChi, mbisRI, telNo, bsCodeList, fax, email, englishRegion, chineseRegion;
                            if (!isCompany)
                            {
                                int j = 0;
                                cerUUID = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j].ToString());j++;
                                cerNo = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j].ToString()); j++;
                                string surName = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j].ToString()); j++;
                                string giveName = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j].ToString()); j++;
                                englishName = (surName + " " + giveName).ToUpper();
                                chineseName = (stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j]).ToString() + " " + englishName).ToUpper(); j++;
                                schineseName = chineseName;
                                asNameListEnglish = "";

                                expiryDate = register[j] == DBNull.Value ? "" : OldDateUtil.getDateDisplayFormat((DateTime)register[j]); j++;
                                telNo = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j].ToString()); j++;
                                string bsID = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j].ToString()); j++;
                                if (!string.IsNullOrWhiteSpace(bsID)) bsCodeList = stringUtil.getDisplay(lookupBS[bsID]);
                                else bsCodeList = "";
                                asNameListChinese = "";
                                sasNameListChinese = "";
                                flag = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j].ToString()); j++;

                                if ("Y".Equals(stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j].ToString())))
                                {
                                    mbisRI = stringUtil.getDisplay("\u2713");
                                }
                                else
                                {
                                    mbisRI = "";
                                }
                                j++;
                                interestedFSS = "";
                                interestedFSSChi = "";
                                if (!exportDataForm.getRegisterType().Equals("QP"))
                                {
                                    if ("Yes".Equals(register[j++].ToString()) || "YES".Equals(register[j++].ToString()))
                                    {
                                        interestedFSS = stringUtil.getDisplay("\u2713");
                                        interestedFSSChi = stringUtil.getDisplay("\u2713");
                                    }
                                }


                                /*englishRegion = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                                chineseRegion = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                                email = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                                fax = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);*/
                            }
                            else
                            {
                                int j = 0;
                                cerUUID = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j].ToString()); j++;
                                cerNo = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j].ToString()); j++;
                                englishName = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j].ToString()).ToUpper(); j++;
                                chineseName = (stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j].ToString()) + " " + englishName).ToUpper(); j++;
                                schineseName = chineseName;
                                asNameListEnglish = (stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j]).ToString()).ToUpper(); j++;
                                expiryDate = register[j] == DBNull.Value ? "" : OldDateUtil.getDateDisplayFormat((DateTime)register[j]); j++;
                                flag = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j].ToString()); j++;

                                //interestedFSS = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                                //interestedFSSChi = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);

                                if ("Yes".Equals(register[j].ToString()) || "YES".Equals(register[j].ToString()))
                                {
                                    interestedFSS = stringUtil.getDisplay("\u2713");
                                    interestedFSSChi = stringUtil.getDisplay("\u2713");
                                }
                                else
                                {
                                    interestedFSS = "";
                                    interestedFSSChi = "";

                                }
                                j++; j++;

                                mbisRI = "";

                                telNo = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j].ToString()); j++;
                                string bsID = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j].ToString()); j++;
                                if (!string.IsNullOrWhiteSpace(bsID)) bsCodeList = stringUtil.getDisplay(lookupBS[bsID].ToString()); else bsCodeList = "";
                                asNameListChinese = (stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j].ToString())).ToUpper(); j++;
                                sasNameListChinese = asNameListChinese;
                                /*englishRegion = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                                chineseRegion = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                                email = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);
                                fax = stringUtil.getDisplay(register[j] == DBNull.Value ? "" : register[j++]);*/
                            }

                            /*string[] eachRegisterEng = new string[10];
                            string[] eachRegisterChi = new string[10];
                            string[] eachRegisterSch = new string[10];*/

                            string[] eachRegisterEng = new string[9];
                            string[] eachRegisterChi = new string[9];
                            string[] eachRegisterSch = new string[9];

                            if ((catCode.Equals("GBC") && bsCodeList.Equals("3")) ||
                                    (catCode.Equals("RGE") && !bsCodeList.Equals("3")) ||
                                    (catCode.Equals("SC(D)") && !bsCodeList.Equals("1")) ||
                                    (catCode.Equals("SC(SF)") && !bsCodeList.Equals("3"))

                            )
                            {
                                bsCodeList = "";
                            }

                            int erC = 0;

                            eachRegisterEng[erC++] = englishName;
                            eachRegisterEng[erC++] = cerNo;
                            eachRegisterEng[erC++] = expiryDate;
                            eachRegisterEng[erC++] = bsCodeList;
                            eachRegisterEng[erC++] = telNo;
                            eachRegisterEng[erC++] = asNameListEnglish;
                            eachRegisterEng[erC++] = flag;
                            eachRegisterEng[erC++] = interestedFSS;
                            eachRegisterEng[erC++] = mbisRI;
                            /*eachRegisterEng[erC++] = englishRegion;
                            eachRegisterEng[erC++] = email;
                            eachRegisterEng[erC++] = fax;*/


                            erC = 0;
                            eachRegisterChi[erC++] = chineseName;
                            eachRegisterChi[erC++] = cerNo;
                            eachRegisterChi[erC++] = expiryDate;
                            eachRegisterChi[erC++] = bsCodeList;
                            eachRegisterChi[erC++] = telNo;
                            eachRegisterChi[erC++] = asNameListChinese;
                            eachRegisterChi[erC++] = flag;
                            eachRegisterChi[erC++] = interestedFSSChi;
                            eachRegisterChi[erC++] = mbisRI;
                            /*eachRegisterChi[erC++] = chineseRegion;
                            eachRegisterChi[erC++] = email;
                            eachRegisterChi[erC++] = fax;*/


                            erC = 0;
                            eachRegisterSch[erC++] = schineseName;
                            eachRegisterSch[erC++] = cerNo;
                            eachRegisterSch[erC++] = expiryDate;
                            eachRegisterSch[erC++] = bsCodeList;
                            eachRegisterSch[erC++] = telNo;
                            eachRegisterSch[erC++] = sasNameListChinese;
                            eachRegisterSch[erC++] = flag;
                            eachRegisterSch[erC++] = interestedFSSChi;
                            eachRegisterSch[erC++] = mbisRI;
                            /*eachRegisterSch[erC++] = chineseRegion;
                            eachRegisterSch[erC++] = email;
                            eachRegisterSch[erC++] = fax;*/



                            if (lastCerUUID.Equals(cerUUID))
                            {

                                eachRegisterEng = registerListsEnglish[registerListsEnglish.Count - 1];
                                registerListsEnglish.Remove(eachRegisterEng);

                                eachRegisterChi = registerListsChinese[registerListsChinese.Count - 1];
                                registerListsChinese.Remove(eachRegisterChi);

                                eachRegisterSch = registerListsSimplifiedChinese[registerListsSimplifiedChinese.Count - 1];
                                registerListsSimplifiedChinese.Remove(eachRegisterSch);

                                if (!bsCodeList.Equals(""))
                                {
                                    eachRegisterEng[3] = eachRegisterEng[3] + " " + bsCodeList;
                                    eachRegisterChi[3] = eachRegisterChi[3] + " " + bsCodeList;
                                    eachRegisterSch[3] = eachRegisterSch[3] + " " + bsCodeList;
                                }
                            }
                            registerListsEnglish.Add(eachRegisterEng);
                            registerListsChinese.Add(eachRegisterChi);
                            registerListsSimplifiedChinese.Add(eachRegisterSch);
                            lastCerUUID = cerUUID;


                            //BS English
                            List<string[]> bsRegList = new List<string[]>();

                            if (bsRegisterListsEnglish.ContainsKey(bsCodeList))
                            {
                                bsRegList = bsRegisterListsEnglish[bsCodeList];
                            }
                            bsRegList.Add(eachRegisterEng);
                            if (bsRegisterListsEnglish.ContainsKey(bsCodeList)) bsRegisterListsEnglish[bsCodeList] = bsRegList; else bsRegisterListsEnglish.Add(bsCodeList, bsRegList);

                            //BS Chinese
                            bsRegList = new List<string[]>();

                            if (bsRegisterListsChinese.ContainsKey(bsCodeList))
                            {
                                bsRegList = bsRegisterListsChinese[bsCodeList];
                            }
                            bsRegList.Add(eachRegisterChi);
                            if (bsRegisterListsChinese.ContainsKey(bsCodeList)) bsRegisterListsChinese[bsCodeList] = bsRegList; else bsRegisterListsChinese.Add(bsCodeList, bsRegList);

                            //BS Simplified Chinese
                            bsRegList = new List<string[]>();

                            if (bsRegisterListsSimplifiedChinese.ContainsKey(bsCodeList))
                            {
                                bsRegList = bsRegisterListsSimplifiedChinese[bsCodeList];
                            }
                            bsRegList.Add(eachRegisterSch);
                            if (bsRegisterListsSimplifiedChinese.ContainsKey(bsCodeList)) bsRegisterListsSimplifiedChinese[bsCodeList] = bsRegList; else bsRegisterListsSimplifiedChinese.Add(bsCodeList, bsRegList);


                        }

                        List<FileInfo> webSiteFileEach = null;

                        //Override the Title
                        if (lookupTitle.ContainsKey(fileNameStart))
                        {
                            C_S_SYSTEM_VALUE sSystemValue = lookupTitle[fileNameStart];
                            chineseTitle = stringUtil.getDisplay(sSystemValue.CHINESE_DESCRIPTION);
                            englishTitle = stringUtil.getDisplay(sSystemValue.ENGLISH_DESCRIPTION);
                            simplifiedChineseTitle = chineseTitle;
                        }

                        if (isCompany)
                        {

                            string chineseTemplate = OldRegisterHTMLGenerator.CHINESE_COMP_TEMPLATE;
                            string englishTemplate = OldRegisterHTMLGenerator.ENGLISH_COMP_TEMPLATE;
                            //string simplifiedChineseTemplate = OldRegisterHTMLGenerator.SIMPLIFIED_CHINESE_COMP_TEMPLATE;

                            if (exportDataForm.getRegisterType().Equals("QP"))
                            {
                                chineseTemplate = OldRegisterHTMLGenerator.CHINESE_QP_COMP_TEMPLATE;
                                englishTemplate = OldRegisterHTMLGenerator.ENGLISH_QP_COMP_TEMPLATE;
                            }

                            bool isbs = true;

                            if (catCode.Equals("SC(F)"))
                            {
                                chineseTemplate = OldRegisterHTMLGenerator.CHINESE_COMP_NOBS_TEMPLATE;
                                englishTemplate = OldRegisterHTMLGenerator.ENGLISH_COMP_NOBS_TEMPLATE;
                                isbs = false;
                            }

                            if (catCode.Equals("SC(GI)"))
                            {
                                chineseTemplate = OldRegisterHTMLGenerator.CHINESE_COMP_NOBS_TEMPLATE;
                                englishTemplate = OldRegisterHTMLGenerator.ENGLISH_COMP_NOBS_TEMPLATE;
                                isbs = false;
                            }

                            if (catCode.Equals("SC(V)"))
                            {
                                chineseTemplate = OldRegisterHTMLGenerator.CHINESE_COMP_NOBS_TEMPLATE;
                                englishTemplate = OldRegisterHTMLGenerator.ENGLISH_COMP_NOBS_TEMPLATE;
                                isbs = false;
                            }

                            //ALL
                            webSiteFileEach =
                                generatorWebSite(exportDataForm.getOutputType(),
                                    OldRegisterHTMLGenerator.LANG_ENG,
                                    templateFolder,
                                    englishTemplate,
                                    isCompany, isbs,
                                    filePath, "e_" + fileNameStart, englishTitle,
                                    registerListsEnglish, bsList, htmlNoteList, catCode);
                            webSiteFile.AddRange(webSiteFileEach);

                            if (!"EXCEL".Equals(exportDataForm.getOutputType()))
                            {
                                if (!exportDataForm.getRegisterType().Equals("QP"))
                                {
                                    //BS

                                    foreach (string key in bsRegisterListsEnglish.Keys)
                                    {
                                        string code = key;
                                        if (code.Equals("") || code.Equals("-"))
                                        {
                                            continue;
                                        }

                                        /*Iterator<string> iter = bsRegisterListsEnglish.keySet().iterator();
                                        while (iter.hasNext()){
                                            string code = iter.next();

                                            if(code.Equals("") || code.Equals("-") ){
                                                continue;
                                            }*/

                                        List<string[]> bsPPs = bsRegisterListsEnglish[code];

                                        string bsFileName = fileNameStart + "b" + code + "_";

                                        string title = englishTitle;

                                        if (lookupTitle.ContainsKey(bsFileName))
                                        {
                                            C_S_SYSTEM_VALUE sSystemValue = lookupTitle[bsFileName];
                                            title = stringUtil.getDisplay(sSystemValue.ENGLISH_DESCRIPTION);
                                        }
                                        bsFileName = "e_" + bsFileName;
                                        webSiteFileEach =
                                            generatorWebSite(exportDataForm.getOutputType(),
                                                OldRegisterHTMLGenerator.LANG_ENG,
                                                templateFolder,
                                                englishTemplate,
                                                isCompany, isbs,
                                                filePath, bsFileName, title,
                                                bsPPs, bsList, htmlNoteList, catCode);
                                        webSiteFile.AddRange(webSiteFileEach);
                                    }
                                }
                            }

                            //ALL CHI
                            webSiteFileEach =
                                generatorWebSite(exportDataForm.getOutputType(),
                                    OldRegisterHTMLGenerator.LANG_CHI,
                                    templateFolder,
                                    chineseTemplate,
                                    isCompany, isbs,
                                    filePath, "c_" + fileNameStart, chineseTitle,
                                    registerListsChinese, bsList, htmlNoteList, catCode);
                            webSiteFile.AddRange(webSiteFileEach);

                            //BS CHI
                            if (!"EXCEL".Equals(exportDataForm.getOutputType()))
                            {

                                if (!exportDataForm.getRegisterType().Equals("QP"))
                                {


                                    foreach (string key in bsRegisterListsChinese.Keys)
                                    {
                                        string code = key;
                                        if (code.Equals("") || code.Equals("-"))
                                        {
                                            continue;
                                        }
                                        /*Iterator<string> iter = bsRegisterListsChinese.keySet().iterator();
                                        while (iter.hasNext()){
                                            string code = iter.next();
                                            if(code.Equals("") || code.Equals("-") ){
                                                continue;
                                            }
                                            */
                                        List<string[]> bsPPs = bsRegisterListsChinese[code];

                                        string title = chineseTitle;

                                        string bsFileName = fileNameStart + "b" + code + "_";
                                        if (lookupTitle.ContainsKey(bsFileName))
                                        {
                                            C_S_SYSTEM_VALUE sSystemValue = lookupTitle[bsFileName];
                                            title = stringUtil.getDisplay(sSystemValue.CHINESE_DESCRIPTION);
                                        }
                                        bsFileName = "c_" + bsFileName;
                                        webSiteFileEach =
                                            generatorWebSite(exportDataForm.getOutputType(),
                                                OldRegisterHTMLGenerator.LANG_CHI,
                                                templateFolder,
                                                chineseTemplate,
                                                isCompany, isbs,
                                                filePath, bsFileName, title,
                                                bsPPs, bsList, htmlNoteList, catCode);
                                        webSiteFile.AddRange(webSiteFileEach);
                                    }
                                }
                            }
                            /*
                            //ALL SIMPLIFIED
                            webSiteFileEach = 
                                OldRegisterHTMLGenerator.generatorWebSite(
                                    OldRegisterHTMLGenerator.LANG_SCH,
                                    templateFolder,
                                    chineseTemplate,
                                    isCompany,isbs,
                                    filePath, "s_"+fileNameStart,converter.convert(chineseTitle),		
                                    registerListsSimplifiedChinese, bsList, htmlNoteList, catCode);
                            webSiteFile.AddRange(webSiteFileEach);	

                            //BS SIMPLIFIED
                            if(!exportDataForm.getRegisterType().Equals("QP")){
                                Iterator<string> iter = bsRegisterListsChinese.keySet().iterator();
                                while (iter.hasNext()){
                                    string code = iter.next();
                                    if(code.Equals("") || code.Equals("-") ){
                                        continue;
                                    }

                                    List<string[]> bsPPs = bsRegisterListsSimplifiedChinese.get(code);

                                    string title = converter.convert(chineseTitle);

                                    string bsFileName =fileNameStart+ "b"+code+"_";
                                    if(lookupTitle.ContainsKey(bsFileName)){
                                        C_S_SYSTEM_VALUE sSystemValue = lookupTitle.get(bsFileName);
                                        title = stringUtil.getDisplay(sSystemValue.CHINESE_DESCRIPTION);
                                        title = converter.convert(title);
                                    }
                                    bsFileName = "s_"+bsFileName;
                                    webSiteFileEach = 
                                        OldRegisterHTMLGenerator.generatorWebSite(
                                            OldRegisterHTMLGenerator.LANG_CHI,
                                            templateFolder,
                                            chineseTemplate,
                                            isCompany,isbs,
                                            filePath, bsFileName, title,		
                                            bsPPs , bsList, htmlNoteList, catCode);
                                    webSiteFile.AddRange(webSiteFileEach);	
                                }
                            }
                            */

                        }
                        else
                        {
                            string chineseTemplate = OldRegisterHTMLGenerator.CHINESE_REGISTER_TEMPLATE;
                            string englishTemplate = OldRegisterHTMLGenerator.ENGLISH_REGISTER_TEMPLATE;

                            if (exportDataForm.getRegisterType().Equals("QP"))
                            {
                                chineseTemplate = OldRegisterHTMLGenerator.CHINESE_QP_REGISTER_TEMPLATE;
                                englishTemplate = OldRegisterHTMLGenerator.ENGLISH_QP_REGISTER_TEMPLATE;
                            }

                            if (catCode.Equals("API"))
                            {
                                chineseTemplate = OldRegisterHTMLGenerator.CHINESE_REGISTER_API_TEMPLATE;
                                englishTemplate = OldRegisterHTMLGenerator.ENGLISH_REGISTER_API_TEMPLATE;
                            }
                            if (catCode.Equals("APII"))
                            {
                                chineseTemplate = OldRegisterHTMLGenerator.CHINESE_REGISTER_APII_TEMPLATE;
                                englishTemplate = OldRegisterHTMLGenerator.ENGLISH_REGISTER_APII_TEMPLATE;
                            }
                            if (catCode.Equals("APIII"))
                            {
                                chineseTemplate = OldRegisterHTMLGenerator.CHINESE_REGISTER_APIII_TEMPLATE;
                                englishTemplate = OldRegisterHTMLGenerator.ENGLISH_REGISTER_APIII_TEMPLATE;
                            }

                            if (catCode.Equals("RI(A)") || catCode.Equals("RI(E)") || catCode.Equals("RI(S)"))
                            {
                                if (exportDataForm.getRegisterType().Equals("QP"))
                                {
                                    chineseTemplate = OldRegisterHTMLGenerator.CHINESE_QP_RI_TEMPLATE;
                                    englishTemplate = OldRegisterHTMLGenerator.ENGLISH_QP_RI_TEMPLATE;
                                }
                                else
                                {
                                    chineseTemplate = OldRegisterHTMLGenerator.CHINESE_RI_TEMPLATE;
                                    englishTemplate = OldRegisterHTMLGenerator.ENGLISH_RI_TEMPLATE;
                                }
                            }
                            //ENG
                            webSiteFileEach =
                                generatorWebSite(exportDataForm.getOutputType(),
                                        OldRegisterHTMLGenerator.LANG_ENG,
                                        templateFolder,
                                        englishTemplate,
                                        isCompany, false,
                                        filePath, "e_" + fileNameStart, englishTitle,
                                        registerListsEnglish, bsList, htmlNoteList, catCode);
                            webSiteFile.AddRange(webSiteFileEach);

                            if (!"EXCEL".Equals(exportDataForm.getOutputType()))
                            {

                                if (!exportDataForm.getRegisterType().Equals("QP"))
                                {
                                    foreach (string key in bsRegisterListsEnglish.Keys)
                                    {
                                        string code = key;

                                        if (code.Equals("") || code.Equals("-"))
                                        {
                                            continue;
                                        }
                                        List<string[]> bsPPs = bsRegisterListsEnglish[code];
                                        /*Iterator<string> iter = bsRegisterListsEnglish.keySet().iterator();
                                        while (iter.hasNext())
                                        {
                                            string code = iter.next();
                                            List<string[]> bsPPs = bsRegisterListsEnglish[code];//.get(code);

                                            if (code.Equals("") || code.Equals("-"))
                                            {
                                                continue;
                                            }*/

                                        string bsFileName = fileNameStart + "b" + code + "_";

                                        string title = englishTitle;

                                        if (lookupTitle.ContainsKey(bsFileName))
                                        {
                                            C_S_SYSTEM_VALUE sSystemValue = lookupTitle[bsFileName];
                                            title = stringUtil.getDisplay(sSystemValue.ENGLISH_DESCRIPTION);
                                        }
                                        bsFileName = "e_" + bsFileName;
                                        webSiteFileEach =
                                            generatorWebSite(
                                                exportDataForm.getOutputType(),
                                                OldRegisterHTMLGenerator.LANG_ENG,
                                                templateFolder,
                                                englishTemplate,
                                                isCompany, false,
                                                filePath, bsFileName, title,
                                                bsPPs, bsList, htmlNoteList, catCode);
                                        webSiteFile.AddRange(webSiteFileEach);
                                    }
                                }
                            }
                            //CHI
                            webSiteFileEach =
                                generatorWebSite(exportDataForm.getOutputType(),
                                        OldRegisterHTMLGenerator.LANG_CHI,
                                        templateFolder,
                                        chineseTemplate, isCompany, false,
                                        filePath, "c_" + fileNameStart, chineseTitle,
                                        registerListsChinese, bsList, htmlNoteList, catCode);
                            webSiteFile.AddRange(webSiteFileEach);


                            if (!"EXCEL".Equals(exportDataForm.getOutputType()))
                            {
                                if (!exportDataForm.getRegisterType().Equals("QP"))
                                {
                                    foreach (string key in bsRegisterListsEnglish.Keys)
                                    {
                                        string code = key;
                                        if (code.Equals("") || code.Equals("-"))
                                        {
                                            continue;
                                        }
                                        /*Iterator<string> iter = bsRegisterListsEnglish.keySet().iterator();
                                        while (iter.hasNext()){
                                            string code = iter.next();
                                            if(code.Equals("") || code.Equals("-") ){
                                                continue;
                                            }*/


                                        List<string[]> bsPPs = bsRegisterListsChinese[code];

                                        string title = chineseTitle;

                                        string bsFileName = fileNameStart + "b" + code + "_";
                                        if (lookupTitle.ContainsKey(bsFileName))
                                        {
                                            C_S_SYSTEM_VALUE sSystemValue = lookupTitle[bsFileName];
                                            title = stringUtil.getDisplay(sSystemValue.CHINESE_DESCRIPTION);
                                        }
                                        bsFileName = "c_" + bsFileName;
                                        webSiteFileEach =
                                            generatorWebSite(
                                                exportDataForm.getOutputType(),
                                                OldRegisterHTMLGenerator.LANG_CHI,
                                                templateFolder,
                                                chineseTemplate,
                                                isCompany, false,
                                                filePath, bsFileName, title,
                                                bsPPs, bsList, htmlNoteList, catCode);
                                        webSiteFile.AddRange(webSiteFileEach);
                                    }
                                }
                            }

                            /*
                            //SCH
                            webSiteFileEach = 
                                OldRegisterHTMLGenerator.generatorWebSite(
                                        OldRegisterHTMLGenerator.LANG_SCH,
                                        templateFolder,
                                        chineseTemplate,isCompany, false,
                                        filePath, "s_"+fileNameStart,converter.convert(chineseTitle),		
                                        registerListsSimplifiedChinese, bsList, htmlNoteList, catCode);
                                webSiteFile.AddRange(webSiteFileEach);

                                if(!exportDataForm.getRegisterType().Equals("QP")){
                                    Iterator<string> iter = bsRegisterListsEnglish.keySet().iterator();
                                    while (iter.hasNext()){
                                        string code = iter.next();
                                        if(code.Equals("") || code.Equals("-") ){
                                            continue;
                                        }

                                        List<string[]> bsPPs = bsRegisterListsSimplifiedChinese.get(code);

                                        string title = converter.convert(chineseTitle);

                                        string bsFileName =fileNameStart+ "b"+code+"_";
                                        if(lookupTitle.ContainsKey(bsFileName)){
                                            C_S_SYSTEM_VALUE sSystemValue = lookupTitle.get(bsFileName);
                                            title = stringUtil.getDisplay(sSystemValue.CHINESE_DESCRIPTION);
                                            title = converter.convert(title);
                                        }
                                        bsFileName = "s_"+bsFileName;
                                        webSiteFileEach = 
                                            OldRegisterHTMLGenerator.generatorWebSite(
                                                OldRegisterHTMLGenerator.LANG_SCH,
                                                templateFolder,
                                                chineseTemplate,
                                                isCompany, false,
                                                filePath, bsFileName, title,		
                                                bsPPs, bsList, htmlNoteList, catCode);
                                        webSiteFile.AddRange(webSiteFileEach);
                                    }			
                                }
                                */
                        }
                    }

                    //} catch (ManagerNotFoundException mnfe) {
                    //	log.fatal("Manager not found!", mnfe);
                    //} catch (HibernateException e) {
                    //	log.fatal("Error exportCompApplicationData: " + e.getMessage());
                }
		} catch (Exception ex) {
                Console.WriteLine(ex);
			//ex.printStackTrace();
			log.fatal("Exception - exportCompApplicationData " + ex.Message);
		} finally {
			//dac.returnManager(indApplicationManager);
			//dac.returnManager(compApplicationManager);
			//dac.returnManager(systemValueManager);
			//dac.returnManager(categoryCodeManager);
			//dac.returnManager(hTMLNotesManager);
			//dac.closeHibernateSession(session);
		}
		
		
		
		if("EXCEL".Equals(exportDataForm.getOutputType()) || exportDataForm.getRegisterType().Equals("QP")){
           string zipFile = getZipFile(tempFolder, "register.zip", webSiteFile);
           FileStream fis = null;//  InputStream fis = null;
			try{
                FileInfo f = new FileInfo(zipFile);
                MemoryStream ms = new MemoryStream();
                using (FileStream file = new FileStream(zipFile, FileMode.Open, FileAccess.Read))

                file.CopyTo(ms);

                 var memoryStream = new MemoryStream(File.ReadAllBytes(zipFile));

                    var stream = ms;
                    var mimeType = "application/zip";
                    FileStreamResult result = new FileStreamResult(memoryStream, mimeType);
                    result.FileDownloadName = f.Name;
                    return result;


                   // servletOut.Flush();
               // FileUtil.deleteFile(filePath);
	          }
	          catch (Exception e){
	            //e.printStackTrace();
	          }
		}else{
			exportDataForm.setWebSiteFile(webSiteFile);
		}
            return null;
	}
		
	
	public void exportQPExcel(HttpRequest request, HttpResponse response, ExportDataForm exportDataForm) //throws Exception
{

    //QualifiedPersonManager qualifiedPersonManager = null;

    try
    {
                using (EntitiesRegistration db = new EntitiesRegistration())
                {
                    // qualifiedPersonManager = (QualifiedPersonManager)dac.getManager(QualifiedPersonManager.class.getName());
                    List<C_VQPEXPORT> qpex = db.C_VQPEXPORT.ToList();// qualifiedPersonManager.getQPExportData();
                    List<object[]> resultList =  qpex.Select(o => new object[] {
                        o.CERTIFICATION_NO  ,                        o.ENGLISH_NAME      ,                        o.CHINESE_NAME      ,                        o.ENGLISH_LINE1     ,
                        o.ENGLISH_LINE2     ,                        o.ENGLISH_LINE3     ,                        o.ENGLISH_LINE4     ,                        o.ENGLISH_LINE5     ,
                        o.CHINESE_LINE1     ,                        o.CHINESE_LINE2     ,                        o.CHINESE_LINE3     ,                        o.CHINESE_LINE4     ,
                        o.CHINESE_LINE5     ,                        o.FAX_NO1           ,                        o.FAX_NO2
                    }).ToList();
                    //List<Object[]> resultList = new List<Object[]> resultList();
                    HSSFWorkbook wb = new HSSFWorkbook();
                    HSSFSheet sheet = wb.CreateSheet("QP List") as HSSFSheet;

                    int idx = 0;
                    int columnLength = 0;
                    for (int j = 0; j < resultList.Count; j++)
                    {
                        HSSFRow dataRow = sheet.CreateRow(idx++) as HSSFRow;
                        Object[] datas = resultList[j];

                        columnLength = datas.Length;
                        for (int k = 0; k < datas.Length; k++)
                        {
                            if (datas[k] is int)
                            {
                                dataRow.CreateCell(k).SetCellValue((int)datas[k]);
                            }
                            else if (datas[k] is long)
                            {
                                dataRow.CreateCell(k).SetCellValue((long)datas[k]);
                                //} else if(datas[k] is decimal) {
                                //	dataRow.CreateCell(k).SetCellValue(((decimal) datas[k]).intValue());
                            }
                            else
                            {
                                dataRow.CreateCell(k).SetCellValue(stringUtil.getDisplay(datas[k]));
                            }
                        }
                    }
                    idx++;
                    for (short i = 0; i < columnLength; i++)
                    {
                        sheet.AutoSizeColumn(i, false);
                    }

                    response.ContentType = "application/vnd.ms-excel";
                    response.AddHeader("Content-Disposition", "attachment;filename=QP.xls");
                    try
                    {
                        wb.Write(response.OutputStream);
                    }
                    catch (IOException ex)
                    {

                    }


                    //} catch (ManagerNotFoundException mnfe) {
                    //	log.fatal("Manager not found!", mnfe);
                }
		} catch (Exception ex) {
			log.fatal("Exception - exportCompApplicationData " + ex.Message);
		} finally {
			//dac.returnManager(qualifiedPersonManager);
		
		}
		
		
	}



        /*
	public void exportBoardsComittees(HttpRequest request, HttpResponse response, ExportDataForm exportDataForm) //throws Exception
{

    string tempFolder = getTempFolder(request);
    string filePath = tempFolder + OldApplicationConstants.FILE_SEPARATOR;
    string templateFolderCommittee = getTemplateFolder("Committee");
    string templateFolderPanel = getTemplateFolder("Panel");
    List<FileInfo> webSiteFile = new List<FileInfo>();
    //SystemValueManager systemValueManager = null;
    //CommitteePanelMemberManager committeePanelMemberManager = null;
    //CommitteeGroupMemberManager committeeGroupMemberManager = null;

    try
    {
                using (EntitiesRegistration db = new EntitiesRegistration())
                {
                    // systemValueManager = (SystemValueManager)dac.getManager(SystemValueManager.class.getName());
                    //	committeePanelMemberManager = (CommitteePanelMemberManager) dac.getManager(CommitteePanelMemberManager.class.getName());
                    //	committeeGroupMemberManager = (CommitteeGroupMemberManager) dac.getManager(CommitteeGroupMemberManager.class.getName());

                    List<Object[]> panelMemberList = null;
                    //List committePanelList = systemValueManager.getSystemValueBySystemType(
                    //                                    OldApplicationConstants.SYSTEM_TYPE_PANEL_TYPE);
                    List<C_S_SYSTEM_VALUE> committePanelList = db.C_S_SYSTEM_VALUE
                        .Where(o => o.C_S_SYSTEM_TYPE.TYPE == OldApplicationConstants.SYSTEM_TYPE_PANEL_TYPE).ToList();
                    //Committee Panel
                    for (int i = 0; i < committePanelList.Count; i++)
                    {
                        C_S_SYSTEM_VALUE panelType = (C_S_SYSTEM_VALUE)committePanelList[i];
                        string panelTypeUUID = stringUtil.getDisplay(panelType.UUID);
                        string englishTitle = stringUtil.getDisplay(panelType.ENGLISH_DESCRIPTION);
                        string chineseTitle = stringUtil.getDisplay(panelType.CHINESE_DESCRIPTION);
                        string code = stringUtil.getDisplay(panelType.CODE);
                        string fileNameStart = getBoardsComitteesFileName(stringUtil.getFilePathstring(code), true);

                        string sectionEnglish = "";
                        string sectionChinese = "";
                        C_S_SYSTEM_VALUE sectionSysValue = systemValueManager.getSystemValueByRegistrationTypeAndCode(
                                OldApplicationConstants.SYSTEM_TYPE_COMM_PANEL_SECTION,
                                OldApplicationConstants.ALL_TYPE, code);
                        if (sectionSysValue != null)
                        {
                            sectionEnglish = stringUtil.getDisplay(sectionSysValue.ENGLISH_DESCRIPTION);
                            sectionChinese = stringUtil.getDisplay(sectionSysValue.CHINESE_DESCRIPTION);
                        }
                        panelMemberList =
                            committeePanelMemberManager.getCommitteePanelMemberForExport(panelTypeUUID, (long)DateUtil.getCurrentYear());

                        if (panelMemberList == null)
                        {
                            panelMemberList = new List<Object[]>();
                        }
                        List<string[]> registerListsEnglish = new List<string[]>();
                        List<string[]> registerListsChinese = new List<string[]>();

                        for (int r = 0; r < panelMemberList.Count; r++)
                        {
                            Object[] register = panelMemberList.get(r);
                            int j = 0;
                            string year = stringUtil.getDisplay(register[j++]);
                            string title_english = stringUtil.getDisplay(register[j++]);
                            string title_chinese = stringUtil.getDisplay(register[j++]);
                            string chineseName = stringUtil.getDisplay(register[j++]);
                            string surname = stringUtil.getDisplay(register[j++]).ToUpper();
                            string givenNameOnId = stringUtil.propercase(register[j] == DBNull.Value ? "" : register[j++]);
                            string c_role = stringUtil.getDisplay(register[j++]);
                            string e_role = stringUtil.getDisplay(register[j++]);
                            DateTime expiryDate = DateUtil.getDisplayDateToDBDate(DateUtil.getDateDisplayFormat(register[j++]));


                            string[] splitGivenName;

                            if (givenNameOnId.IndexOf(",") != -1)
                            {
                                splitGivenName = stringUtil.getDisplay(givenNameOnId).split(",");
                                splitGivenName[1] = " " + splitGivenName[1];
                            }
                            else
                            {
                                splitGivenName = new string[2];
                                splitGivenName[0] = stringUtil.getDisplay(givenNameOnId);
                                splitGivenName[1] = "";
                            }

                            registerListsEnglish.Add(

                                    new string[]{
                                     title_english + splitGivenName[1]+ " "+ surname + " "+ splitGivenName[0].trim(),
                                     DateUtil.getEnglishFormatDate(expiryDate)});

                            if (chineseName.Equals(""))
                            {
                                registerListsChinese.Add(new string[]{
                                title_english + splitGivenName[1]+ " "+ surname + " "+ splitGivenName[0].trim(),

                                DateUtil.getChineseFormatDate(expiryDate) });
                            }
                            else
                            {
                                registerListsChinese.Add(new string[]{
                                chineseName +title_chinese  ,
                                DateUtil.getChineseFormatDate(expiryDate) });
                            }

                        }

                        List<FileInfo> webSiteFileEach = new List<FileInfo>();

                        webSiteFileEach =
                                            CommitteePanelHTMLGenerator.generatorWebSite(
                                                CommitteePanelHTMLGenerator.LANG_ENG,
                                                templateFolderPanel,
                                                CommitteePanelHTMLGenerator.ENGLISH_PANEL_TEMPLATE,
                                                filePath, fileNameStart,
                                                englishTitle, sectionEnglish, code, registerListsEnglish);
                        webSiteFile.AddRange(webSiteFileEach);

                        webSiteFileEach =
                            CommitteePanelHTMLGenerator.generatorWebSite(
                                CommitteePanelHTMLGenerator.LANG_CHI,
                                templateFolderPanel,
                                CommitteePanelHTMLGenerator.CHINESE_PANEL_TEMPLATE,
                                filePath, fileNameStart + "_c",
                                chineseTitle, sectionChinese, code, registerListsChinese);
                        webSiteFile.AddRange(webSiteFileEach);
                    }



                    List<Object[]> committeeMemberList = null;
                    List<C_S_SYSTEM_VALUE> committeList =
                        db.C_S_SYSTEM_VALUE
                        .Where(o => o.C_S_SYSTEM_TYPE.TYPE == OldApplicationConstants.SYSTEM_TYPE_COMMITTEE_TYPE)
                        .ToList();
                        //systemValueManager.getSystemValueBySystemType(
                        //                                OldApplicationConstants.SYSTEM_TYPE_COMMITTEE_TYPE);

                    //committeList
                    for (int i = 0; i < committeList.Count; i++)
                    {
                        C_S_SYSTEM_VALUE sSystemValue = committeList[i];
                        string committeeTypeUUID = stringUtil.getDisplay(sSystemValue.UUID);
                        string englishTitle = stringUtil.getDisplay(sSystemValue.ENGLISH_DESCRIPTION);
                        string chineseTitle = stringUtil.getDisplay(sSystemValue.CHINESE_DESCRIPTION);
                        string committeeCode = stringUtil.getDisplay(sSystemValue.CODE);

                        //User required to merge into SC
                        if (committeeCode.Equals("SC(D)") || committeeCode.Equals("SC(F)") ||
                                committeeCode.Equals("SC(GI)") || committeeCode.Equals("SC(SF)") ||
                                committeeCode.Equals("SC(V)"))
                        {
                            continue;
                        }
                        string fileNameStart = getBoardsComitteesFileName(stringUtil.getFilePathstring(committeeCode), false);


                        string sectionEnglish = "";
                        string sectionChinese = "";

                       // C_S_SYSTEM_VALUE sectionSysValue = systemValueManager.getSystemValueByRegistrationTypeAndCode(
                       //         OldApplicationConstants.SYSTEM_TYPE_COMM_TYPE_SECTION,
                        //        OldApplicationConstants.ALL_TYPE, committeeCode);

                        C_S_SYSTEM_VALUE sectionSysValue = db.C_S_SYSTEM_VALUE
                            .Where(o => o.CODE == committeeCode && o.REGISTRATION_TYPE == OldApplicationConstants.ALL_TYPE)
                            .Where(o=>o.C_S_SYSTEM_TYPE.TYPE == OldApplicationConstants.SYSTEM_TYPE_COMM_TYPE_SECTION)
                            .FirstOrDefault();
                        if (sectionSysValue != null)
                        {
                            sectionEnglish = stringUtil.getDisplay(sectionSysValue.ENGLISH_DESCRIPTION);
                            sectionChinese = stringUtil.getDisplay(sectionSysValue.CHINESE_DESCRIPTION);
                        }
                        if (committeeCode.Equals("SC"))
                        {
                            committeeMemberList =
                                committeeGroupMemberManager.getCommitteeGroupMemberForSpecialistContractor((long)DateUtil.getCurrentYear());
                        }
                        else if (committeeCode.Equals("GBC"))
                        {
                            committeeMemberList =
                                committeeGroupMemberManager.getCommitteeGroupMemberForGBC(committeeTypeUUID, (long)DateUtil.getCurrentYear());
                        }
                        else if (committeeCode.Equals("APRC"))
                        {
                            committeeMemberList =
                                committeeGroupMemberManager.getCommitteeGroupMemberAPRCForExport(committeeTypeUUID, (long)DateUtil.getCurrentYear());
                        }
                        else
                        {
                            committeeMemberList =
                                committeeGroupMemberManager.getCommitteeGroupMemberForExport(committeeTypeUUID, (long)DateUtil.getCurrentYear());
                        }

                        if (committeeMemberList == null)
                        {
                            committeeMemberList = new List<Object[]>();
                        }
                        List<string[]> registerListsEnglish = new List<string[]>();
                        List<string[]> registerListsChinese = new List<string[]>();

                        for (int r = 0; r < committeeMemberList.Count; r++)
                        {
                            Object[] register = committeeMemberList[r];
                            int j = 0;

                            string committe_chinese = stringUtil.getDisplay(register[j++]);
                            string committe_english = stringUtil.getDisplay(register[j++]);
                            string group_name = stringUtil.getDisplay(register[j++]);
                            string group_year = stringUtil.getDisplay(register[j++]);
                            string group_month = stringUtil.getDisplay(register[j++]);
                            string role_chinese = stringUtil.getDisplay(register[j++]);
                            string role_english = stringUtil.propercase(register[j] == DBNull.Value ? "" : register[j++]);

                            string title_chinese = stringUtil.getDisplay(register[j++]);
                            string title_english = stringUtil.getDisplay(register[j++]);

                            string name_chinese = stringUtil.getDisplay(register[j++]);
                            string name_sur = stringUtil.getDisplay(register[j++]).ToUpper();

                            string name_given = stringUtil.propercase(register[j] == DBNull.Value ? "" : register[j++]);

                            DateTime expiry_Date =
                                DateUtil.getDisplayDateToDBDate(DateUtil.getDateDisplayFormat(register[j++]));

                            string applUUID = stringUtil.getDisplay(register[j++]);

                            string role_code = stringUtil.getDisplay(register[j++]);


                            string[] splitGivenName;

                            if (name_given.IndexOf(",") != -1)
                            {
                                splitGivenName = stringUtil.getDisplay(name_given).split(",");
                                splitGivenName[1] = " " + splitGivenName[1];

                            }
                            else
                            {
                                splitGivenName = new string[2];
                                splitGivenName[0] = stringUtil.getDisplay(name_given);
                                splitGivenName[1] = "";
                            }


                            string english_name =
                                title_english + splitGivenName[1] + " " + name_sur + " " + splitGivenName[0].trim();

                            string chinese_name = "";

                            if (name_chinese.Equals(""))
                            {
                                chinese_name = english_name;
                            }
                            else
                            {
                                chinese_name = name_chinese + title_chinese;
                            }

                            registerListsEnglish.Add(new string[]{group_name, group_month,role_english, english_name,
                            DateUtil.getEnglishFormatDate(expiry_Date) , applUUID, role_code });
                            registerListsChinese.Add(new string[]{group_name, group_month, role_chinese,chinese_name ,
                            DateUtil.getChineseFormatDate(expiry_Date), applUUID, role_code });
                        }

                        List<File> webSiteFileEach = null;

                        webSiteFileEach =
                                            CommitteeHTMLGenerator.generatorWebSite(
                                                    CommitteeHTMLGenerator.LANG_ENG,
                                                    templateFolderCommittee,
                                                    CommitteeHTMLGenerator.ENGLISH_PANEL_TEMPLATE,
                                                filePath, fileNameStart,
                                                englishTitle, sectionEnglish, committeeCode, registerListsEnglish);
                        webSiteFile.AddRange(webSiteFileEach);

                        webSiteFileEach =
                            CommitteeHTMLGenerator.generatorWebSite(
                                    CommitteeHTMLGenerator.LANG_CHI,
                                    templateFolderCommittee,
                                    CommitteeHTMLGenerator.CHINESE_PANEL_TEMPLATE,
                                filePath, fileNameStart + "_c",
                                chineseTitle, sectionChinese, committeeCode, registerListsChinese);
                        webSiteFile.AddRange(webSiteFileEach);
                    }




                }
		//	
		//} catch (ManagerNotFoundException mnfe) {
		//	log.fatal("Manager not found!", mnfe);
		//} catch (HibernateException e) {
		//	log.fatal("Error exportBoardsComittees: " + e.getMessage());
		} catch (Exception ex) {
			log.fatal("Exception - exportBoardsComittees " + ex.Message);
		} finally {
			//dac.returnManager(committeePanelMemberManager);
			//dac.returnManager(committeeGroupMemberManager);
			
			//dac.returnManager(systemValueManager);
		}
		
		
		
		
		
		
		

		string zipFile = getZipFile(tempFolder, "BoardsComittees.zip", webSiteFile);
InputStream fis = null;
			try{
				FileInfo f = new FileInfo(zipFile);
response.setContentType("application/zip");
	            response.setContentLength((int) f.length());
	            response.setHeader("Content-Disposition", "attachment; filename="  + f.getName());
	            ServletOutputStream servletOut = (ServletOutputStream)response.getOutputStream();
int read = -1;
fis = new FileInputStream(f);
	            if(fis.available() != 0){
	            	byte b[] = new byte[fis.available()];
read = fis.read(b);
	            	while (read != -1){
	            		servletOut.write(b, 0, read);
	            		read = fis.read(b);
	            	}
	            }
            	servletOut.flush();
                FileUtil.deleteFile(filePath);

	          }
	          catch (Exception e)
	          {
	            e.printStackTrace();
	          }
   
	}
	*/
    /*
	public void exportRegistersDataPDF(HttpRequest request, HttpResponse response, ExportDataForm exportDataForm)// throws Exception
{
    string ENGLISH_PROFESSIONAL_TEMPLATE = "e_prof";
    string CHINESE_PROFESSIONAL_TEMPLATE = "c_prof";
    string ENGLISH_RI_TEMPLATE = "e_ri";
    string CHINESE_RI_TEMPLATE = "c_ri";
    string ENGLISH_COMPANY_TEMPLATE = "e_comp";
    string CHINESE_COMPANY_TEMPLATE = "c_comp";
    string ENGLISH_MWCOMPANY_TEMPLATE = "e_mwc";
    string CHINESE_MWCOMPANY_TEMPLATE = "c_mwc";
    string ENGLISH_COMPANY_NOBS_TEMPLATE = "e_comp_nobs";
    string CHINESE_COMPANY_NOBS_TEMPLATE = "c_comp_nobs";
    string ENGLISH_QP_PROFESSIONAL_TEMPLATE = "e_qp_ap";
    string CHINESE_QP_PROFESSIONAL_TEMPLATE = "c_qp_ap";

    string filePath = getTempFolder(request) + OldApplicationConstants.FILE_SEPARATOR;

    string[]
    report_list = { "qp_gbc", "qp_mwc", "qp_ri", "qp_mwc(w)", "mwc(w)" };

    string[]
    qp_ap_list = { "qp_ap", "qp_rse" };

    string[]
    comp_list = { "gbc", "gbc_b1", "gbc_b2", "gbc_b4", "rsc_d", "rsc_d_b1", "rsc_sf", "rsc_sf_b3" };

    string[]
    comp_nobs_list = { "rsc_gi", "rsc_f", "rsc_f_b3", "rsc_v" };

    string[]
    mwc_list = { "mwc", "mwc(p)" };

    string[]
    prof_list = {
        "api", "apii", "apiii",
                "api_b1", "api_b2", "api_b3", "api_b4", "apii_b1", "apii_b2", "apii_b3", "apii_b4",
                "apiii_b1", "apiii_b2", "apiii_b3", "apiii_b4", "rse", "rse_b1", "rse_b2", "rse_b3",
                "rse_b4", "rge", "rge_b3"};

    string[]
    ri_list = {
        "ri(a)", "ri(e)", "ri(s)", "ri(a)_b1",
                "ri(a)_b2", "ri(a)_b3", "ri(a)_b4", "ri(e)_b1", "ri(e)_b2", "ri(e)_b3", "ri(e)_b4",
                "ri(s)_b1", "ri(s)_b2", "ri(s)_b3", "ri(s)_b4"};

    List<FileInfo> pdf_files = new List<FileInfo>();

    //pdf_files.Add(exportToPDF(request, "c_" + report_list[0], "c_" + report_list[0]));
    //pdf_files.Add(exportToPDF(request, "e_" + report_list[0], "e_" + report_list[0]));
    //pdf_files.Add(exportToPDF(request, "c_" + report_list[1], "c_" + report_list[1]));
    //pdf_files.Add(exportToPDF(request, "e_" + report_list[1], "e_" + report_list[1]));

    for (int i = 0; i < qp_ap_list.Length; i++)
    {
        pdf_files.Add(exportToPDF(request, "e_" + qp_ap_list[i], ENGLISH_QP_PROFESSIONAL_TEMPLATE));
        pdf_files.Add(exportToPDF(request, "c_" + qp_ap_list[i], CHINESE_QP_PROFESSIONAL_TEMPLATE));
    }

    for (int i = 0; i < report_list.Length; i++)
    {
        pdf_files.Add(exportToPDF(request, "e_" + report_list[i], "e_" + report_list[i]));
        pdf_files.Add(exportToPDF(request, "c_" + report_list[i], "c_" + report_list[i]));
    }

    for (int i = 0; i < comp_nobs_list.Length; i++)
    {
        pdf_files.Add(exportToPDF(request, "e_" + comp_nobs_list[i], ENGLISH_COMPANY_NOBS_TEMPLATE));
        pdf_files.Add(exportToPDF(request, "c_" + comp_nobs_list[i], CHINESE_COMPANY_NOBS_TEMPLATE));
    }

    for (int i = 0; i < comp_list.Length; i++)
    {
        pdf_files.Add(exportToPDF(request, "e_" + comp_list[i], ENGLISH_COMPANY_TEMPLATE));
        pdf_files.Add(exportToPDF(request, "c_" + comp_list[i], CHINESE_COMPANY_TEMPLATE));
    }

    for (int i = 0; i < mwc_list.Length; i++)
    {
        pdf_files.Add(exportToPDF(request, "e_" + mwc_list[i], ENGLISH_MWCOMPANY_TEMPLATE));
        pdf_files.Add(exportToPDF(request, "c_" + mwc_list[i], CHINESE_MWCOMPANY_TEMPLATE));
    }
    for (int i = 0; i < ri_list.Length; i++)
    {
        pdf_files.Add(exportToPDF(request, "e_" + ri_list[i], ENGLISH_RI_TEMPLATE));
        pdf_files.Add(exportToPDF(request, "c_" + ri_list[i], CHINESE_RI_TEMPLATE));
    }

    for (int i = 0; i < prof_list.Length; i++)
    {
        pdf_files.Add(exportToPDF(request, "e_" + prof_list[i], ENGLISH_PROFESSIONAL_TEMPLATE));
        pdf_files.Add(exportToPDF(request, "c_" + prof_list[i], CHINESE_PROFESSIONAL_TEMPLATE));
    }

    if (pdf_files.Count > 0)
    {
        string zipFile = getZipFile(filePath, "register_pdf.zip", pdf_files);
        FileStream fis = null;
        try
        {
            FileInfo f = new FileInfo(zipFile);
            response.ContentType = ("application/zip");
            response.setContentLength((int)f.Length);
            response.AddHeader("Content-Disposition", "attachment; filename=" + f.Name);
                    Stream servletOut = response.OutputStream;
            int read = -1;
            fis = new FileStream(f.FullName, FileMode.OpenOrCreate);
            if (fis.available() != 0)
            {
                byte[] b = new byte[fis.available()];
                read = fis.Read(b, 0, b.Length);
                        while (read != -1)
                {
                    servletOut.Write(b, 0, read);
                    read = fis.Read(b,0,b.Length);
                }
            }
            servletOut.Flush();
            FileUtil.deleteFile(filePath);
        }
        catch (Exception e)
        {
            //e.printStackTrace();
        }
    }

}
*/
/*
public FileInfo exportToPDF(HttpRequest request, string reportName, string templateName)
{
    //SystemValueManager systemValueManager = null;
    //Session session = null;

    try
    {
        using (EntitiesRegistration db = new EntitiesRegistration()) { 
            //  systemValueManager = (SystemValueManager)dac.getManager(SystemValueManager.class.getName());

            //string templateName = ENGLISH_PROFESSIONAL_TEMPLATE;
            //string reportName = prof_list[i];
            string subReportName = reportName.Substring(2) + "_";
        string reportPath = ReportGeneratorManager.REPORT_PATH + templateName;
        string title = "";
        string date = "";
        Dictionary<string, string> parameterMap = new Dictionary<string, string>();
                    //session = dac.getHibernateSession();
                    //SCategoryCodeDAO sCategoryCodeDAO = new SCategoryCodeDAO(session);
                    List<C_S_SYSTEM_VALUE> exportTileList =
                    db.C_S_SYSTEM_VALUE
                        .Where(o => o.C_S_SYSTEM_TYPE.TYPE == OldApplicationConstants.SYSTEM_TYPE_EXPORT_TITLE)
                        .Where(o => o.REGISTRATION_TYPE == "All").ToList();
                    // List<C_S_SYSTEM_VALUE> exportTileList = systemValueManager.getActiveSystemValueBySystemTypeAndRegistrationType(
                    //         OldApplicationConstants.SYSTEM_TYPE_EXPORT_TITLE, "ALL");


                    for (int t = 0; t < exportTileList.Count; t++)
        {

            C_S_SYSTEM_VALUE sSystemValue = exportTileList[t];
            if (subReportName.Equals(sSystemValue.CODE))
            {
                if (reportName.StartsWith("e_"))
                {
                    title = sSystemValue.ENGLISH_DESCRIPTION;
                }
                else
                {
                    title = sSystemValue.CHINESE_DESCRIPTION;
                }
                break;
            }
        }

        if (stringUtil.isBlank(title))
        {
            C_S_CATEGORY_CODE sCategoryCode = null;
            if (reportName.IndexOf("ri(a)") != -1)
            {
                    // sCategoryCode = (C_S_CATEGORY_CODE)(sCategoryCodeDAO.findByCode("RI(A)").get(0));
                    sCategoryCode = db.C_S_CATEGORY_CODE.Where(o => o.CODE == "RI(A)").FirstOrDefault();
                }
            else if (reportName.IndexOf("ri(e)") != -1)
            {
                //sCategoryCode = (C_S_CATEGORY_CODE)(sCategoryCodeDAO.findByCode("RI(E)").get(0));
                    sCategoryCode = db.C_S_CATEGORY_CODE.Where(o => o.CODE == "RI(E)").FirstOrDefault();
                }
            else if (reportName.IndexOf("ri(s)") != -1)
            {
               // sCategoryCode = (C_S_CATEGORY_CODE)(sCategoryCodeDAO.findByCode("RI(S)").get(0));
                    sCategoryCode = db.C_S_CATEGORY_CODE.Where(o => o.CODE == "RI(S)").FirstOrDefault();
                }
            else if (reportName.IndexOf("mwc(w)") != -1)
            {
                //sCategoryCode = (C_S_CATEGORY_CODE)(sCategoryCodeDAO.findByCode("MWC(W)").get(0));
                    sCategoryCode = db.C_S_CATEGORY_CODE.Where(o => o.CODE == "MWC(W)").FirstOrDefault();
                }
            else if (reportName.IndexOf("mwc(p)") != -1)
            {
               // sCategoryCode = (C_S_CATEGORY_CODE)(sCategoryCodeDAO.findByCode("MWC(P)").get(0));
                    sCategoryCode = db.C_S_CATEGORY_CODE.Where(o => o.CODE == "MWC(P)").FirstOrDefault();
                }
            else if (reportName.IndexOf("mwc") != -1)
            {
                //sCategoryCode = (C_S_CATEGORY_CODE)(sCategoryCodeDAO.findByCode("MWC").get(0));
                    sCategoryCode = db.C_S_CATEGORY_CODE.Where(o => o.CODE == "MWC").FirstOrDefault();
                }
            else if (reportName.IndexOf("rsc_f_b3") != -1)
            {
                //sCategoryCode = (C_S_CATEGORY_CODE)(sCategoryCodeDAO.findByCode("SC(F)").get(0));
                    sCategoryCode = db.C_S_CATEGORY_CODE.Where(o => o.CODE == "SC(F)").FirstOrDefault();
                }
            if (sCategoryCode != null)
            {
                if (reportName.StartsWith("e_"))
                {
                    title = sCategoryCode.ENGLISH_DESCRIPTION;
                }
                else
                {
                    title = sCategoryCode.CHINESE_DESCRIPTION;
                }
            }
        }

        parameterMap.Add("title", title);

        if (reportName.StartsWith("e_"))
        {
            date = "Last revision date: " + OldDateUtil.getEnglishFormatDate(DateTime.Now);
        }
        else
        {
            date = "修訂日期 : " + OldDateUtil.getChineseFormatDate(DateTime.Now);
        }
        parameterMap.Add("date", date);

        if (reportName.IndexOf("api") != -1)
        {
            if (reportName.IndexOf("apiii") != -1)
            {
                parameterMap.Add("category", "AP(S)");
                parameterMap.Add("category2", "APIII");
            }
            else if (reportName.IndexOf("apii") != -1)
            {
                parameterMap.Add("category", "AP(E)");
                parameterMap.Add("category2", "APII");
            }
            else if (reportName.IndexOf("api") != -1)
            {
                parameterMap.Add("category", "AP(A)");
                parameterMap.Add("category2", "API");
            }
            parameterMap.Add("reg_type", "IP");
            parameterMap.Add("cat_grp", "AP");
        }
        else if (reportName.IndexOf("rse") != -1)
        {
            parameterMap.Add("category", "RSE");
            parameterMap.Add("reg_type", "IP");
            parameterMap.Add("cat_grp", "RSE");
        }
        else if (reportName.IndexOf("rge") != -1)
        {
            parameterMap.Add("category", "RGE");
            parameterMap.Add("reg_type", "IP");
            parameterMap.Add("cat_grp", "RGE");
        }
        else if (reportName.IndexOf("ri") != -1)
        {
            if (reportName.IndexOf("ri(a)") != -1)
            {
                parameterMap.Add("category", "RI(A)");
            }
            else if (reportName.IndexOf("ri(e)") != -1)
            {
                parameterMap.Add("category", "RI(E)");
            }
            else if (reportName.IndexOf("ri(s)") != -1)
            {
                parameterMap.Add("category", "RI(S)");
            }
        }
        else if (reportName.IndexOf("gbc") != -1)
        {
            parameterMap.Add("category", "GBC");
        }
        else if (reportName.IndexOf("rsc_d") != -1)
        {
            parameterMap.Add("category", "SC(D)");
        }
        else if (reportName.IndexOf("rsc_f") != -1)
        {
            parameterMap.Add("category", "SC(F)");
        }
        else if (reportName.IndexOf("rsc_gi") != -1)
        {
            parameterMap.Add("category", "SC(GI)");
        }
        else if (reportName.IndexOf("rsc_sf") != -1)
        {
            parameterMap.Add("category", "SC(SF)");
        }
        else if (reportName.IndexOf("rsc_v") != -1)
        {
            parameterMap.Add("category", "SC(V)");
        }
        else if (reportName.IndexOf("mwc(p)") != -1)
        {
            parameterMap.Add("category", "MWC(P)");
        }
        else if (reportName.IndexOf("mwc") != -1)
        {
            parameterMap.Add("category", "MWC");
        }
        else if (reportName.IndexOf("qp_rse") != -1)
        {
            parameterMap.Add("cat_grp", "RSE");
        }
        else if (reportName.IndexOf("qp_ap") != -1)
        {
            parameterMap.Add("cat_grp", "AP");
        }

        if (reportName.IndexOf("_b") != -1)
        {
            string bs = reportName.Substring(reportName.Length - 1);
            parameterMap.Add("where_clause", " WHERE (bs_code LIKE '%" + bs + "%' OR remark IS NOT NULL) ");
        }

        //Connection connection = dac.getHibernateSession().connection();

        //File reportFile = new File(request.getRealPath(reportPath + ReportGeneratorManager.REPORT_EXTENTION));
        //JasperDesign jasperDesign = JRXmlLoader.load(reportFile);
        //JasperReport jasperReport = JasperCompileManager.compileReport(jasperDesign);
        //
        //
        //byte[] bytes = JasperRunManager.runReportToPdf(jasperReport, parameterMap, connection);
        //
        //string fileName = getTempFolder(request) + OldApplicationConstants.FILE_SEPARATOR + reportName + ".pdf";
        //FileOutputStream fos = new FileOutputStream(fileName);
        //fos.write(bytes);
        //fos.close();
        //
        //return new File(fileName);
        return null;
    }
    }
    catch (Exception e)
    {
       // e.printStackTrace();
    }
    return null;
}
*/
}

}