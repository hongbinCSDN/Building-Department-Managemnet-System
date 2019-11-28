using MWMS2.Areas.Registration.Models;
using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MWMS2.Utility;
using MWMS2.Constant;
using System.Data.Entity;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using System.Web;
using System.IO;
using System.Data.Entity.Validation;

namespace MWMS2.Services
{
    public class RegistrationCompAppService
    {

        private const string GetAllType_q = ""
                 + "\r\n\t" + "SELECT T1.UUID, T1.UUID AS TUUID, PARENT.UUID  AS CUUID, T1.CODE AS T, PARENT.CODE  AS C                                 "
                 + "\r\n\t" + "FROM C_S_SYSTEM_VALUE T1, C_S_SYSTEM_TYPE T2, C_S_SYSTEM_VALUE PARENT "
                 + "\r\n\t" + "WHERE T1.SYSTEM_TYPE_ID = T2.UUID                                     "
                 + "\r\n\t" + "AND T1.PARENT_ID = PARENT.UUID                                        "
                 + "\r\n\t" + "AND T2.TYPE = 'MINOR_WORKS_TYPE'                                      ";

        private const string SearchComp_q = ""
                 + "\r\n" + "\t" + "SELECT                                                               "
                 + "\r\n" + "\t" + "T1.UUID                                                              "
                 + "\r\n" + "\t" + ", T1.FILE_REFERENCE_NO                                               "
                 + "\r\n" + "\t" + ", T1.ENGLISH_COMPANY_NAME                                            "
                 + "\r\n" + "\t" + ", T1.CERTIFICATION_NO                                                "
                 + "\r\n" + "\t" + ", T1.BR_NO                                                           "
                 + "\r\n" + "\t" + ", T1.APPLICATION_DATE                                                "
                 + "\r\n" + "\t" + ", T1.GAZETTE_DATE                                                    "
                 + "\r\n" + "\t" + ", T1.REGISTRATION_DATE                                               "
                 + "\r\n" + "\t" + ", T1.EXPIRY_DATE                                                     "
                 + "\r\n" + "\t" + ", T1.REMOVAL_DATE                                                    "
                 + "\r\n" + "\t" + ", T1.RETENTION_DATE                                                  "
                 + "\r\n" + "\t" + ", T1.RESTORE_DATE                                                    "
                 + "\r\n" + "\t" + ", T2.ENGLISH_DESCRIPTION                                             "
                 + "\r\n" + "\t" + "FROM C_COMP_APPLICATION T1                                           "
                 + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE T2 ON T1.APPLICATION_STATUS_ID = T2.UUID  ";

        private string SearchComp_whereQ(CompSearchModel model)
        {
            string whereQ = "\r\n\t" + "WHERE T1.REGISTRATION_TYPE = :RegType";
            model.QueryParameters.Add("RegType", model.RegType);
            if (!string.IsNullOrWhiteSpace(model.FileRef))
            {
                whereQ += "\r\n\t" + "AND UPPER(T1.FILE_REFERENCE_NO) LIKE :FileRef";
                model.QueryParameters.Add("FileRef", "%" + model.FileRef.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.RegNo))
            {
                whereQ += "\r\n\t" + "AND UPPER(T1.CERTIFICATION_NO) LIKE :RegNo";
                model.QueryParameters.Add("RegNo", "%" + model.RegNo.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.CompNameEng))
            {
                whereQ += "\r\n\t" + "AND UPPER(T1.ENGLISH_COMPANY_NAME) LIKE :CompNameEng";
                model.QueryParameters.Add("CompNameEng", "%" + model.CompNameEng.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.CompNameChn))
            {
                whereQ += "\r\n\t" + "AND T1.CHINESE_COMPANY_NAME LIKE :CompNameChn";
                model.QueryParameters.Add("CompNameChn", "%" + model.CompNameChn.Trim() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.BrNo))
            {
                whereQ += "\r\n\t" + "AND UPPER(T1.BR_NO) LIKE :BrNo";
                model.QueryParameters.Add("BrNo", "%" + model.BrNo.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.AddressEng))
            {
                whereQ += "\r\n\t" + "AND EXISTS (SELECT 1 FROM C_ADDRESS T2 WHERE T2.UUID = T1.ENGLISH_ADDRESS_ID AND UPPER(T2.ADDRESS_LINE1||T2.ADDRESS_LINE2||T2.ADDRESS_LINE3||T2.ADDRESS_LINE4||T2.ADDRESS_LINE5) LIKE :AddressEng)";
                model.QueryParameters.Add("AddressEng", "%" + model.AddressEng.Trim().ToUpper() + "%");
            }

            if (!string.IsNullOrWhiteSpace(model.AddressChn))
            {
                whereQ += "\r\n\t" + "AND EXISTS (SELECT 1 FROM C_ADDRESS T2 WHERE T2.UUID = T1.CHINESE_ADDRESS_ID AND UPPER(T2.ADDRESS_LINE1||T2.ADDRESS_LINE2||T2.ADDRESS_LINE3||T2.ADDRESS_LINE4||T2.ADDRESS_LINE5) LIKE :AddressChn)";
                model.QueryParameters.Add("AddressChn", "%" + model.AddressChn.Trim().ToUpper() + "%");
            }

            if (!string.IsNullOrWhiteSpace(model.Surname))
            {
                whereQ += "\r\n\t" + "AND EXISTS(SELECT 1 FROM C_COMP_APPLICANT_INFO T2 WHERE T2.MASTER_ID = T1.UUID AND EXISTS(SELECT 1 FROM C_APPLICANT T3 WHERE T3.UUID = T2.APPLICANT_ID AND UPPER(T3.SURNAME) LIKE :Surname))";
                model.QueryParameters.Add("Surname", model.Surname.Trim().ToUpper() + "%");
            }

            if (!string.IsNullOrWhiteSpace(model.GivenName))
            {
                whereQ += "\r\n\t" + "AND EXISTS(SELECT 1 FROM C_COMP_APPLICANT_INFO T2 WHERE T2.MASTER_ID = T1.UUID AND EXISTS(SELECT 1 FROM C_APPLICANT T3 WHERE T3.UUID = T2.APPLICANT_ID AND UPPER(T3.GIVEN_NAME_ON_ID) LIKE :GivenName))";
                model.QueryParameters.Add("GivenName", (model.KeywordSearch ? "%" : "") + model.GivenName.Trim().ToUpper() + "%");
            }

            if (!string.IsNullOrWhiteSpace(model.ChineseName))
            {
                whereQ += "\r\n\t" + "AND EXISTS(SELECT 1 FROM C_COMP_APPLICANT_INFO T2 WHERE T2.MASTER_ID = T1.UUID AND EXISTS(SELECT 1 FROM C_APPLICANT T3 WHERE T3.UUID = T2.APPLICANT_ID AND UPPER(T3.CHINESE_NAME) LIKE :ChineseName))";
                model.QueryParameters.Add("ChineseName", "%" + model.ChineseName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.TelNo))
            {
                whereQ += "\r\n\t" + "AND ( (replace(T1.TELEPHONE_NO1,' ','') LIKE :TelNo) or (replace(T1.TELEPHONE_NO2,' ','') LIKE :TelNo1) or (replace(T1.TELEPHONE_NO3,' ','' ) LIKE :TelNo2) ) ";
                model.QueryParameters.Add("TelNo", "%" + model.TelNo.Trim().Replace(" ", "") + "%");
                model.QueryParameters.Add("TelNo1", "%" + model.TelNo.Trim().Replace(" ", "") + "%");
                model.QueryParameters.Add("TelNo2", "%" + model.TelNo.Trim().Replace(" ", "") + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                whereQ += "\r\n\t" + "AND UPPER(EMAIL_ADDRESS) like :Email";
                model.QueryParameters.Add("Email", "%" + model.Email.Trim().ToUpper() + "%");


            }


            if (!string.IsNullOrWhiteSpace(model.Pnrc))
            {
                whereQ += "\r\n\t" + "AND T1.PRACTICE_NOTES_ID = :Pnrc";
                model.QueryParameters.Add("Pnrc", model.Pnrc);
            }
            if (!string.IsNullOrWhiteSpace(model.ServiceInBS))
            {
                whereQ += "\r\n\t" + "AND EXISTS(SELECT 1 FROM C_BUILDING_SAFETY_INFO T2 WHERE T2.MASTER_ID = T1.UUID AND T2.BUILDING_SAFETY_ID = :ServiceInBS)";
                model.QueryParameters.Add("ServiceInBS", model.ServiceInBS);
            }

            if (!string.IsNullOrWhiteSpace(model.DateType)
                && RegistrationConstant.DATE_TYPE_MAP.ContainsKey(model.DateType)
                && (model.DateFrom != null || model.DateTo != null)
                )
            {
                if (model.DateFrom != null)
                {
                    whereQ += "\r\n\t" + "AND T1." + RegistrationConstant.DATE_TYPE_MAP[model.DateType] + " >= :DateFrom";
                    model.QueryParameters.Add("DateFrom", model.DateFrom);
                }
                if (model.DateTo != null)
                {
                    whereQ += "\r\n\t" + "AND T1." + RegistrationConstant.DATE_TYPE_MAP[model.DateType] + " <= :DateTo";
                    model.QueryParameters.Add("DateTo", model.DateTo);
                }
            }
            return whereQ;
        }
        public CompSearchModel SearchComp(CompSearchModel model)
        {
            model.Query = SearchComp_q;
            model.QueryWhere = SearchComp_whereQ(model);
            model.Search();
            return model;
        }
        public string ExportComp(CompSearchModel model)
        {
            model.Query = SearchComp_q;
            model.QueryWhere = SearchComp_whereQ(model);

            //model.Columns = model.Columns.ToList()//displayName: "BR No.", columnName: "BR_NO"
            //    .Append(new Dictionary<string, string> { ["displayName"] = "UUID", ["columnName"] = "UUID" })
            //    .Append(new Dictionary<string, string> { ["displayName"] = "UUID", ["columnName"] = "UUID" })
            //    .ToArray();
            return model.Export("ExportData");
        }


        public BrDetailsModel SearchBr(BrDetailsModel model)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                // model.Data =
                List<C_COMP_APPLICATION> r = db.C_COMP_APPLICATION
                .Include(o => o.C_S_SYSTEM_VALUE5)
                .Where(o => o.BR_NO == model.BR_NO)
                .OrderBy(o => o.ENGLISH_COMPANY_NAME)
                .ToList();
                List<Dictionary<String, object>> data = new List<Dictionary<String, object>>();
                for (int i = 0; i < r.Count; i++)
                {
                    data.Add(new Dictionary<string, object>() {
                    { "V1" ,r[i].FILE_REFERENCE_NO }
                    , { "V2" ,r[i].ENGLISH_COMPANY_NAME }
                    , { "V3" ,r[i].CHINESE_COMPANY_NAME }
                    , { "V4" ,r[i].C_S_SYSTEM_VALUE5?.ENGLISH_DESCRIPTION }});
                }
                model.Data = data;
                model.Total = model.Data.Count;
            }
            return model;
        }
        public PoolingDetailsModel SearchPooling(PoolingDetailsModel model)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                // model.Data =
                List<C_POOLING> r = db.C_POOLING
                .Include(o => o.C_COMP_APPLICATION)
                .Where(o => o.POOL_NO == model.m_PoolingRefNo)
                .OrderBy(o => o.C_COMP_APPLICATION.ENGLISH_COMPANY_NAME)
                .ToList();
                List<Dictionary<String, object>> data = new List<Dictionary<String, object>>();
                for (int i = 0; i < r.Count; i++)
                {
                    data.Add(new Dictionary<string, object>() {
                    { "V1" ,r[i].C_COMP_APPLICATION.FILE_REFERENCE_NO }
                    , { "V2" ,r[i].C_COMP_APPLICATION.ENGLISH_COMPANY_NAME }
                    , { "V3" ,r[i].C_COMP_APPLICATION.CHINESE_COMPANY_NAME }
                    , { "V5" ,r[i].C_COMP_APPLICATION.EXPIRY_DATE }
                    , { "V4" ,r[i].C_COMP_APPLICATION.C_S_SYSTEM_VALUE5?.ENGLISH_DESCRIPTION }});

                }
                model.Data = data;
                model.Total = model.Data.Count;
            }
            return model;
        }

        public CompanyNamesModel SearchCompName(CompanyNamesModel model)
        {
            model.Query = ""
                + "\r\n\t" + "SELECT ENGLISH_COMPANY_NAME, CHINESE_COMPANY_NAME, MIN(CREATED_DATE) AS CREATED_DATE"
                + "\r\n\t" + "FROM C_COMP_APPLICATION_HISTORY WHERE MASTER_ID = :C_COMP_APPLICATION_UUID"
                + "\r\n\t" + "GROUP BY ENGLISH_COMPANY_NAME, CHINESE_COMPANY_NAME";
            model.QueryParameters.Add("C_COMP_APPLICATION_UUID", model.C_COMP_APPLICATION_UUID);
            model.Search();
            return model;
        }
        public Dictionary<string, string> GetMWCapViewTable(string applicationUuid)
        {
            string q = ""
                + "\r\n\t" + " SELECT                                                                                                                                                                                                    "
                + "\r\n\t" + "   CASE WHEN A1 IS NOT NULL THEN 'Type A, ' ELSE '' END	||CASE WHEN B1 IS NOT NULL THEN 'Type B, ' ELSE '' END                                                                                           "
                + "\r\n\t" + " ||CASE WHEN C1 IS NOT NULL THEN 'Type C, ' ELSE '' END	||CASE WHEN D1 IS NOT NULL THEN 'Type D, ' ELSE '' END                                                                                           "
                + "\r\n\t" + " ||CASE WHEN E1 IS NOT NULL THEN 'Type E, ' ELSE '' END	||CASE WHEN F1 IS NOT NULL THEN 'Type F, ' ELSE '' END                                                                                           "
                + "\r\n\t" + " ||CASE WHEN G1 IS NOT NULL THEN 'Type G, ' ELSE '' END AS CLASS1                                                                                                                                          "
                + "\r\n\t" + " , CASE WHEN A2 IS NOT NULL THEN 'Type A, ' ELSE '' END	||CASE WHEN B2 IS NOT NULL THEN 'Type B, ' ELSE '' END                                                                                           "
                + "\r\n\t" + " ||CASE WHEN C2 IS NOT NULL THEN 'Type C, ' ELSE '' END	||CASE WHEN D2 IS NOT NULL THEN 'Type D, ' ELSE '' END                                                                                           "
                + "\r\n\t" + " ||CASE WHEN E2 IS NOT NULL THEN 'Type E, ' ELSE '' END	||CASE WHEN F2 IS NOT NULL THEN 'Type F, ' ELSE '' END                                                                                           "
                + "\r\n\t" + " ||CASE WHEN G2 IS NOT NULL THEN 'Type G, ' ELSE '' END AS CLASS2                                                                                                                                          "
                + "\r\n\t" + " , CASE WHEN A3 IS NOT NULL THEN 'Type A, ' ELSE '' END	||CASE WHEN B3 IS NOT NULL THEN 'Type B, ' ELSE '' END                                                                                           "
                + "\r\n\t" + " ||CASE WHEN C3 IS NOT NULL THEN 'Type C, ' ELSE '' END	||CASE WHEN D3 IS NOT NULL THEN 'Type D, ' ELSE '' END                                                                                           "
                + "\r\n\t" + " ||CASE WHEN E3 IS NOT NULL THEN 'Type E, ' ELSE '' END	||CASE WHEN F3 IS NOT NULL THEN 'Type F, ' ELSE '' END                                                                                           "
                + "\r\n\t" + " ||CASE WHEN G3 IS NOT NULL THEN 'Type G, ' ELSE '' END AS CLASS3                                                                                                                                          "
                + "\r\n\t" + " FROM (                                                                                                                                                                                                    "
                + "\r\n\t" + " 	SELECT                                                                                                                                                                                                   "
                + "\r\n\t" + " 	  CASE WHEN A1 = 'Y' THEN 'Y' ELSE NULL END AS A1		, CASE WHEN B1 = 'Y' THEN 'Y' ELSE NULL END AS B1                                                                                                "
                + "\r\n\t" + " 	, CASE WHEN C1 = 'Y' THEN 'Y' ELSE NULL END AS C1		, CASE WHEN D1 = 'Y' THEN 'Y' ELSE NULL END AS D1                                                                                                "
                + "\r\n\t" + " 	, CASE WHEN E1 = 'Y' THEN 'Y' ELSE NULL END AS E1		, CASE WHEN F1 = 'Y' THEN 'Y' ELSE NULL END AS F1                                                                                                "
                + "\r\n\t" + " 	, CASE WHEN G1 = 'Y' THEN 'Y' ELSE NULL END AS G1                                                                                                                                                        "
                + "\r\n\t" + " 	, CASE WHEN A2 = 'Y' AND A1 IS NULL THEN 'Y' ELSE NULL END AS A2		, CASE WHEN B2 = 'Y' AND B1 IS NULL THEN 'Y' ELSE NULL END AS B2                                                                 "
                + "\r\n\t" + " 	, CASE WHEN C2 = 'Y' AND C1 IS NULL THEN 'Y' ELSE NULL END AS C2		, CASE WHEN D2 = 'Y' AND D1 IS NULL THEN 'Y' ELSE NULL END AS D2                                                                 "
                + "\r\n\t" + " 	, CASE WHEN E2 = 'Y' AND E1 IS NULL THEN 'Y' ELSE NULL END AS E2		, CASE WHEN F2 = 'Y' AND F1 IS NULL THEN 'Y' ELSE NULL END AS F2                                                                 "
                + "\r\n\t" + " 	, CASE WHEN G2 = 'Y' AND G1 IS NULL THEN 'Y' ELSE NULL END AS G2                                                                                                                                         "
                + "\r\n\t" + " 	, CASE WHEN A3 = 'Y' AND A2 IS NULL AND A1 IS NULL THEN 'Y' ELSE NULL END AS A3		, CASE WHEN B3 = 'Y' AND B2 IS NULL AND B1 IS NULL THEN 'Y' ELSE NULL END AS B3                                      "
                + "\r\n\t" + " 	, CASE WHEN C3 = 'Y' AND C2 IS NULL AND C1 IS NULL THEN 'Y' ELSE NULL END AS C3		, CASE WHEN D3 = 'Y' AND D2 IS NULL AND D1 IS NULL THEN 'Y' ELSE NULL END AS D3                                      "
                + "\r\n\t" + " 	, CASE WHEN E3 = 'Y' AND E2 IS NULL AND E1 IS NULL THEN 'Y' ELSE NULL END AS E3		, CASE WHEN F3 = 'Y' AND F2 IS NULL AND F1 IS NULL THEN 'Y' ELSE NULL END AS F3                                      "
                + "\r\n\t" + " 	, CASE WHEN G3 = 'Y' AND G2 IS NULL AND G1 IS NULL THEN 'Y' ELSE NULL END AS G3                                                                                                                          "
                + "\r\n\t" + " 	FROM (                                                                                                                                                                                                   "
                + "\r\n\t" + " 		SELECT                                                                                                                                                                                               "
                + "\r\n\t" + " 		 MAX(CASE WHEN ITEM_CLASS = 'Class 1' AND ITEM_TYPE = 'Type A' THEN 'Y' ELSE NULL END) AS A1			, MAX(CASE WHEN ITEM_CLASS = 'Class 1' AND ITEM_TYPE = 'Type B' THEN 'Y' ELSE NULL END) AS B1"
                + "\r\n\t" + " 		, MAX(CASE WHEN ITEM_CLASS = 'Class 1' AND ITEM_TYPE = 'Type C' THEN 'Y' ELSE NULL END) AS C1			, MAX(CASE WHEN ITEM_CLASS = 'Class 1' AND ITEM_TYPE = 'Type D' THEN 'Y' ELSE NULL END) AS D1"
                + "\r\n\t" + " 		, MAX(CASE WHEN ITEM_CLASS = 'Class 1' AND ITEM_TYPE = 'Type E' THEN 'Y' ELSE NULL END) AS E1			, MAX(CASE WHEN ITEM_CLASS = 'Class 1' AND ITEM_TYPE = 'Type F' THEN 'Y' ELSE NULL END) AS F1"
                + "\r\n\t" + " 		, MAX(CASE WHEN ITEM_CLASS = 'Class 1' AND ITEM_TYPE = 'Type G' THEN 'Y' ELSE NULL END) AS G1                                                                                                        "
                + "\r\n\t" + " 		, MAX(CASE WHEN ITEM_CLASS = 'Class 2' AND ITEM_TYPE = 'Type A' THEN 'Y' ELSE NULL END) AS A2			, MAX(CASE WHEN ITEM_CLASS = 'Class 2' AND ITEM_TYPE = 'Type B' THEN 'Y' ELSE NULL END) AS B2"
                + "\r\n\t" + " 		, MAX(CASE WHEN ITEM_CLASS = 'Class 2' AND ITEM_TYPE = 'Type C' THEN 'Y' ELSE NULL END) AS C2			, MAX(CASE WHEN ITEM_CLASS = 'Class 2' AND ITEM_TYPE = 'Type D' THEN 'Y' ELSE NULL END) AS D2"
                + "\r\n\t" + " 		, MAX(CASE WHEN ITEM_CLASS = 'Class 2' AND ITEM_TYPE = 'Type E' THEN 'Y' ELSE NULL END) AS E2			, MAX(CASE WHEN ITEM_CLASS = 'Class 2' AND ITEM_TYPE = 'Type F' THEN 'Y' ELSE NULL END) AS F2"
                + "\r\n\t" + " 		, MAX(CASE WHEN ITEM_CLASS = 'Class 2' AND ITEM_TYPE = 'Type G' THEN 'Y' ELSE NULL END) AS G2                                                                                                        "
                + "\r\n\t" + " 		, MAX(CASE WHEN ITEM_CLASS = 'Class 3' AND ITEM_TYPE = 'Type A' THEN 'Y' ELSE NULL END) AS A3			, MAX(CASE WHEN ITEM_CLASS = 'Class 3' AND ITEM_TYPE = 'Type B' THEN 'Y' ELSE NULL END) AS B3"
                + "\r\n\t" + " 		, MAX(CASE WHEN ITEM_CLASS = 'Class 3' AND ITEM_TYPE = 'Type C' THEN 'Y' ELSE NULL END) AS C3			, MAX(CASE WHEN ITEM_CLASS = 'Class 3' AND ITEM_TYPE = 'Type D' THEN 'Y' ELSE NULL END) AS D3"
                + "\r\n\t" + " 		, MAX(CASE WHEN ITEM_CLASS = 'Class 3' AND ITEM_TYPE = 'Type E' THEN 'Y' ELSE NULL END) AS E3			, MAX(CASE WHEN ITEM_CLASS = 'Class 3' AND ITEM_TYPE = 'Type F' THEN 'Y' ELSE NULL END) AS F3"
                + "\r\n\t" + " 		, MAX(CASE WHEN ITEM_CLASS = 'Class 3' AND ITEM_TYPE = 'Type G' THEN 'Y' ELSE NULL END) AS G3                                                                                                        "
                + "\r\n\t" + " 		FROM (                                                                                                                                                                                               "
                + "\r\n\t" + " 			SELECT T3.CODE AS ITEM_CLASS, T4.CODE AS ITEM_TYPE                                                                                                                                               "
                + "\r\n\t" + " 			FROM C_COMP_APPLICANT_MW_ITEM T1                                                                                                                                                                 "
                + "\r\n\t" + " 			INNER JOIN C_COMP_APPLICANT_INFO T2 ON T1.COMPANY_APPLICANTS_ID = T2.UUID                                                                                                                        "
                + "\r\n\t" + " 			INNER JOIN C_S_SYSTEM_VALUE T3 ON T3.UUID = T1.ITEM_CLASS_ID                                                                                                                                     "
                + "\r\n\t" + " 			INNER JOIN C_S_SYSTEM_VALUE T4 ON T4.UUID = T1.ITEM_TYPE_ID                                                                                                                                      "
                + "\r\n\t" + " 			WHERE T2.MASTER_ID = :applicationUuid                                                                                                                                          "
                + "\r\n\t" + " 		)                                                                                                                                                                                                    "
                + "\r\n\t" + " 	)                                                                                                                                                                                                        "
                + "\r\n\t" + " )                                                                                                                                                                                                         ";
            DisplayGrid grid = new DisplayGrid() { Query = q, QueryParameters = new Dictionary<string, object>() { { "applicationUuid", applicationUuid } } };
            grid.Search();
            Dictionary<string, string> d = new Dictionary<string, string>() {
                {"CLASS1" , grid.Data[0]["CLASS1"] as string },
                {"CLASS2" , grid.Data[0]["CLASS2"] as string},
                {"CLASS3" , grid.Data[0]["CLASS3"]as string }
            };
            if (!string.IsNullOrWhiteSpace(d["CLASS1"]) && d["CLASS1"].Length > 2) d["CLASS1"] = d["CLASS1"].Substring(0, d["CLASS1"].Length - 2);
            if (!string.IsNullOrWhiteSpace(d["CLASS2"]) && d["CLASS2"].Length > 2) d["CLASS2"] = d["CLASS2"].Substring(0, d["CLASS2"].Length - 2);
            if (!string.IsNullOrWhiteSpace(d["CLASS3"]) && d["CLASS3"].Length > 2) d["CLASS3"] = d["CLASS3"].Substring(0, d["CLASS3"].Length - 2);
            return d;
        }
        public CompDisplayModel ViewComp(string id, string regType)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                string ServiceInMWIS = "";
                List<C_S_SYSTEM_VALUE> bs = (from t1 in db.C_S_SYSTEM_VALUE
                                             join t2 in db.C_BUILDING_SAFETY_INFO on t1 equals t2.C_S_SYSTEM_VALUE
                                             where t2.MASTER_ID == id
                                             select t1).ToList();
                List<ApplicantDisplayListModel> aplts = ApplicantDisplayListModel.Load(GetApplicantsByCompUuid(id, null, true));
                BuildingSafetyCheckListModel bsModel = new BuildingSafetyCheckListModel() { MasterUuid = id, RegType = RegistrationConstant.REGISTRATION_TYPE_CGA };
                bsModel.init();

                if (string.IsNullOrWhiteSpace(id))
                {
                    id = SessionUtil.DRAFT_NEXT_ID;
                    return new CompDisplayModel()
                    {
                        C_COMP_APPLICATION = new C_COMP_APPLICATION() { UUID = id },
                        C_S_SYSTEM_VALUEs_BS = bs,
                        C_APPLICANTs = aplts,
                        BsModel = bsModel,
                        RegType = regType
                    };
                }
                else
                {

                    Dictionary<string, string> d = GetMWCapViewTable(id);
                    C_COMP_APPLICATION application =
                        db.C_COMP_APPLICATION
                        .Where(o => o.UUID == id)
                        .Include(o => o.C_ADDRESS)
                        .Include(o => o.C_ADDRESS1)
                        .Include(o => o.C_ADDRESS2)
                        .Include(o => o.C_ADDRESS3)
                        .FirstOrDefault();
                    var PoolingQuery = db.C_POOLING.Where(x => x.MASTER_ID == application.UUID).OrderBy(x => x.CREATED_DATE);
                    if (application.SERVICE_IN_MWIS == null)
                    {
                        ServiceInMWIS = "-";
                    }
                    else
                    {
                        ServiceInMWIS = application.SERVICE_IN_MWIS;
                    }
                    return new CompDisplayModel()
                    {
                        C_COMP_APPLICATION = application,
                        C_S_SYSTEM_VALUEs_BS = bs,
                        C_ADDRESS_Chinese = application?.C_ADDRESS,
                        C_ADDRESS_English = application?.C_ADDRESS2,
                        C_APPLICANTs = aplts,
                        C_ADDRESS_BS_Chinese = application?.C_ADDRESS1,
                        C_ADDRESS_BS_English = application?.C_ADDRESS3,
                        EditFormKey = application == null ? -1 : application.MODIFIED_DATE.Ticks,
                        BsModel = bsModel,
                        RegType = regType,
                        ListPoolingRefNo = PoolingQuery.ToList(),
                        Class1 = d["CLASS1"],
                        Class2 = d["CLASS2"],
                        Class3 = d["CLASS3"],
                        ServiceInMWIS = ServiceInMWIS
                    };
                }

            }
        }


        public ServiceResult DeleteComp(CompDisplayModel model)
        {
            try
            {
                using (EntitiesRegistration db = new EntitiesRegistration())
                {
                    using (DbContextTransaction transaction = db.Database.BeginTransaction())
                    {
                        string uuid = model.C_COMP_APPLICATION.UUID;
                        db.C_COMP_APPLICATION_HISTORY.RemoveRange(db.C_COMP_APPLICATION_HISTORY.Where(o => o.MASTER_ID == uuid));
                        db.C_COMP_PROCESS_MONITOR.RemoveRange(db.C_COMP_PROCESS_MONITOR.Where(o => o.MASTER_ID == uuid));
                        db.C_COMP_APPLICANT_INFO.RemoveRange(db.C_COMP_APPLICANT_INFO.Where(o => o.MASTER_ID == uuid));
                        db.C_COMP_APPLN_MW_ITEM_HISTORY.RemoveRange(db.C_COMP_APPLN_MW_ITEM_HISTORY.Where(o => o.C_COMP_APPLICANT_INFO.MASTER_ID == uuid));
                        db.C_COMP_APPLICANT_MW_ITEM.RemoveRange(db.C_COMP_APPLICANT_MW_ITEM.Where(o => o.C_COMP_APPLICANT_INFO.MASTER_ID == uuid));
                        db.C_COMP_APPLICANT_INFO_HISTORY.RemoveRange(db.C_COMP_APPLICANT_INFO_HISTORY.Where(o => o.C_COMP_APPLICANT_INFO.MASTER_ID == uuid));
                        db.C_COMP_APPLICANT_INFO_MASTER.RemoveRange(db.C_COMP_APPLICANT_INFO_MASTER.Where(o => o.C_COMP_APPLICANT_INFO.MASTER_ID == uuid));
                        db.C_COMP_APPLICANT_INFO_DETAIL.RemoveRange(db.C_COMP_APPLICANT_INFO_DETAIL.Where(o => o.C_COMP_APPLICANT_INFO_MASTER.C_COMP_APPLICANT_INFO.MASTER_ID == uuid));
                        db.C_POOLING.RemoveRange(db.C_POOLING.Where(x => x.MASTER_ID == uuid));
                        db.C_COMP_APPLICATION.RemoveRange(db.C_COMP_APPLICATION.Where(o => o.UUID == uuid));
                        db.SaveChanges();
                        transaction.Commit();
                        return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error :" + ex.Message);
                return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
            }
        }

        public ServiceResult SaveComp(CompDisplayModel model)
        {
            int dummy;
            C_COMP_APPLICATION appTemp = model.C_COMP_APPLICATION;

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        C_COMP_APPLICATION app = null;
                        if (int.TryParse(appTemp.UUID, out dummy))
                        {
                            appTemp.UUID = null;
                            app = new C_COMP_APPLICATION();
                        }
                        else
                        {
                            app = db.C_COMP_APPLICATION.Where(o => o.UUID == appTemp.UUID && o.MODIFIED_DATE == new DateTime(model.EditFormKey)).FirstOrDefault();
                        }
                        //if (app == null) return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { "Record have been update by another session, plesae update." } };
                        app.REGISTRATION_TYPE = model.RegType;
                        //first part
                        app.CERTIFICATION_NO = appTemp.FILE_REFERENCE_NO;
                        app.FILE_REFERENCE_NO = model.RegPrefix == null ? appTemp.FILE_REFERENCE_NO : (SystemListUtil.GetCatCodeByUUID(model.RegPrefix).CODE + " " + appTemp.FILE_REFERENCE_NO);
                        app.BR_NO = appTemp.BR_NO;
                        app.BRANCH_NO = appTemp.BRANCH_NO;
                        app.ENGLISH_COMPANY_NAME = appTemp.ENGLISH_COMPANY_NAME;
                        app.CHINESE_COMPANY_NAME = appTemp.CHINESE_COMPANY_NAME;
                        app.APPLICATION_STATUS_ID = appTemp.APPLICATION_STATUS_ID;

                        //Company Info
                        app.COMPANY_TYPE_ID = appTemp.COMPANY_TYPE_ID;
                        app.APPLICANT_NAME = appTemp.APPLICANT_NAME;
                        app.ENGLISH_CARE_OF = appTemp.ENGLISH_CARE_OF;
                        app.CHINESE_CARE_OF = appTemp.CHINESE_CARE_OF;

                        if (app.C_ADDRESS != null) app.C_ADDRESS = db.C_ADDRESS.Where(o => o.UUID == app.C_ADDRESS.UUID).FirstOrDefault();
                        else app.C_ADDRESS = new C_ADDRESS();
                        app.C_ADDRESS.ADDRESS_LINE1 = appTemp.C_ADDRESS.ADDRESS_LINE1;
                        app.C_ADDRESS.ADDRESS_LINE2 = appTemp.C_ADDRESS.ADDRESS_LINE2;
                        app.C_ADDRESS.ADDRESS_LINE3 = appTemp.C_ADDRESS.ADDRESS_LINE3;
                        app.C_ADDRESS.ADDRESS_LINE4 = appTemp.C_ADDRESS.ADDRESS_LINE4;
                        app.C_ADDRESS.ADDRESS_LINE5 = appTemp.C_ADDRESS.ADDRESS_LINE5;

                        if (app.C_ADDRESS2 != null) app.C_ADDRESS2 = db.C_ADDRESS.Where(o => o.UUID == app.C_ADDRESS2.UUID).FirstOrDefault();
                        else app.C_ADDRESS2 = new C_ADDRESS();
                        app.C_ADDRESS2.ADDRESS_LINE1 = appTemp.C_ADDRESS2.ADDRESS_LINE1;
                        app.C_ADDRESS2.ADDRESS_LINE2 = appTemp.C_ADDRESS2.ADDRESS_LINE2;
                        app.C_ADDRESS2.ADDRESS_LINE3 = appTemp.C_ADDRESS2.ADDRESS_LINE3;
                        app.C_ADDRESS2.ADDRESS_LINE4 = appTemp.C_ADDRESS2.ADDRESS_LINE4;
                        app.C_ADDRESS2.ADDRESS_LINE5 = appTemp.C_ADDRESS2.ADDRESS_LINE5;


                        app.TELEPHONE_NO1 = appTemp.TELEPHONE_NO1;
                        app.TELEPHONE_NO2 = appTemp.TELEPHONE_NO2;
                        app.TELEPHONE_NO3 = appTemp.TELEPHONE_NO3;
                        app.FAX_NO1 = appTemp.FAX_NO1;
                        app.FAX_NO2 = appTemp.FAX_NO2;
                        app.EMAIL_ADDRESS = appTemp.EMAIL_ADDRESS;

                        app.OLD_FILE_REFERENCE = appTemp.OLD_FILE_REFERENCE;
                        app.PERIOD_OF_VALIDITY_ID = appTemp.PERIOD_OF_VALIDITY_ID;
                        app.FIRST_APPLICATION_DATE = appTemp.FIRST_APPLICATION_DATE;
                        app.APPLICATION_DATE = appTemp.APPLICATION_DATE;
                        app.APPLICATION_FORM_ID = appTemp.APPLICATION_FORM_ID;

                        app.DUE_DATE = appTemp.DUE_DATE;

                        //Applicants

                        //Status
                        app.PRACTICE_NOTES_ID = appTemp.PRACTICE_NOTES_ID;
                        app.PROSECUTION_DISCIPLINARY_REC = appTemp.PROSECUTION_DISCIPLINARY_REC;
                        app.REMARKS = appTemp.REMARKS;

                        //Building Safety
                        app.WILLINGNESS_QP = appTemp.WILLINGNESS_QP;
                        app.INTERESTED_FSS = appTemp.INTERESTED_FSS;
                        //new add
                        app.SERVICE_IN_MWIS = model.ServiceInMWIS;
                        app.ENGLISH_BS_CARE_OF = appTemp.ENGLISH_BS_CARE_OF;
                        app.CHINESE_BS_CARE_OF = appTemp.CHINESE_BS_CARE_OF;

                        if (app.C_ADDRESS1 != null) app.C_ADDRESS1 = db.C_ADDRESS.Where(o => o.UUID == app.C_ADDRESS1.UUID).FirstOrDefault();
                        else app.C_ADDRESS1 = new C_ADDRESS();
                        app.C_ADDRESS1.ADDRESS_LINE1 = appTemp.C_ADDRESS1.ADDRESS_LINE1;
                        app.C_ADDRESS1.ADDRESS_LINE2 = appTemp.C_ADDRESS1.ADDRESS_LINE2;
                        app.C_ADDRESS1.ADDRESS_LINE3 = appTemp.C_ADDRESS1.ADDRESS_LINE3;
                        app.C_ADDRESS1.ADDRESS_LINE4 = appTemp.C_ADDRESS1.ADDRESS_LINE4;
                        app.C_ADDRESS1.ADDRESS_LINE5 = appTemp.C_ADDRESS1.ADDRESS_LINE5;

                        if (app.C_ADDRESS3 != null) app.C_ADDRESS3 = db.C_ADDRESS.Where(o => o.UUID == app.C_ADDRESS3.UUID).FirstOrDefault();
                        else app.C_ADDRESS3 = new C_ADDRESS();
                        app.C_ADDRESS3.ADDRESS_LINE1 = appTemp.C_ADDRESS3.ADDRESS_LINE1;
                        app.C_ADDRESS3.ADDRESS_LINE2 = appTemp.C_ADDRESS3.ADDRESS_LINE2;
                        app.C_ADDRESS3.ADDRESS_LINE3 = appTemp.C_ADDRESS3.ADDRESS_LINE3;
                        app.C_ADDRESS3.ADDRESS_LINE4 = appTemp.C_ADDRESS3.ADDRESS_LINE4;
                        app.C_ADDRESS3.ADDRESS_LINE5 = appTemp.C_ADDRESS3.ADDRESS_LINE5;

                        app.BS_TELEPHONE_NO1 = appTemp.BS_TELEPHONE_NO1;
                        app.BS_FAX_NO1 = appTemp.BS_FAX_NO1;
                        if (app.UUID == null)
                        {
                            db.C_COMP_APPLICATION.Add(app);
                            db.SaveChanges();
                        }
                        db.C_BUILDING_SAFETY_INFO.RemoveRange(db.C_BUILDING_SAFETY_INFO.Where(o => o.MASTER_ID == app.UUID));

                        for (int i = 0; i < model.BsItems.Count; i++)
                        {
                            if (!model.BsItems[i].Checked) continue;
                            C_BUILDING_SAFETY_INFO bs = new C_BUILDING_SAFETY_INFO();
                            bs.MASTER_ID = app.UUID;
                            bs.BUILDING_SAFETY_ID = model.BsItems[i].CheckListUuid;
                            bs.REGISTRATION_TYPE = RegistrationConstant.REGISTRATION_TYPE_CGA;
                            db.C_BUILDING_SAFETY_INFO.Add(bs);
                        }
                        //session
                        //applicant
                        Dictionary<string, C_COMP_APPLICANT_INFO> MAP_Applicant = new Dictionary<string, C_COMP_APPLICANT_INFO>();
                        List<C_COMP_APPLICANT_INFO> draftApplicants = SessionUtil.DraftList<C_COMP_APPLICANT_INFO>(ApplicationConstant.DRAFT_KEY_APPLICANTS);
                        List<string> deleteApplicants = SessionUtil.DraftList<string>(ApplicationConstant.DELETE_KEY_APPLICANTS);

                        //applicant - delete

                        IQueryable<C_COMP_APPLICANT_INFO> deleteList = db.C_COMP_APPLICANT_INFO
                            .Where(o => deleteApplicants.Contains(o.UUID))
                            .Include(o => o.C_APPLICANT)
                            .Include(o => o.C_COMP_APPLICANT_INFO_MASTER.Select(o2 => o2.C_COMP_APPLICANT_INFO_DETAIL));



                        foreach (var item in deleteList)
                        {
                            var query = db.C_COMP_APPLICANT_INFO_HISTORY.Where(x => x.COMPANY_APPLICANTS_ID == item.UUID).ToList();
                            db.C_COMP_APPLICANT_INFO_HISTORY.RemoveRange(query);
                            db.C_LEAVE_FORM.RemoveRange(db.C_LEAVE_FORM.Where(x => x.COMPANY_APPLICANT_ID == item.UUID));
                            db.SaveChanges();
                        }
                        db.C_COMP_APPLICANT_INFO.RemoveRange(deleteList);
                        db.SaveChanges();
                        //applicant - edit 
                        List<C_COMP_APPLICANT_INFO> editApplicantList = draftApplicants.Where(o => !int.TryParse(o.UUID, out dummy)).ToList();
                        List<string> editUuid = (from t1 in editApplicantList select t1.UUID).ToList();
                        List<C_COMP_APPLICANT_INFO> dbEditApplicantList = db.C_COMP_APPLICANT_INFO.Where(o =>
                        editUuid.Contains(o.UUID)
                        && o.MASTER_ID == app.UUID)
                        .Include(o => o.C_APPLICANT)
                        .Include(o => o.C_S_SYSTEM_VALUE)
                        .Include(o => o.C_S_SYSTEM_VALUE1)
                        .Include(o => o.C_APPLICANT.C_S_SYSTEM_VALUE)
                        .ToList();
                        for (int i = 0; i < dbEditApplicantList.Count; i++)
                        {
                            editApplicantList[i].refNo = app.FILE_REFERENCE_NO;
                            dbEditApplicantList[i].MergeByList(editApplicantList);
                            dbEditApplicantList[i].C_APPLICANT?.Encrypt();
                        }
                        db.SaveChanges();
                        //applicant - create 
                        List<C_COMP_APPLICANT_INFO> createList = draftApplicants.Where(o => int.TryParse(o.UUID, out dummy)).ToList();

                        for (int i = 0; i < createList.Count; i++)
                        {
                            MAP_Applicant.Add(createList[i].UUID, createList[i]);
                            createList[i].UUID = null;
                            createList[i].MASTER_ID = app.UUID;
                            createList[i].C_APPLICANT.Encrypt();
                            createList[i].C_APPLICANT.C_S_SYSTEM_VALUE = null;
                            createList[i].C_S_SYSTEM_VALUE = null;
                            createList[i].C_S_SYSTEM_VALUE1 = null;


                            if (createList[i].Applicant_File != null)
                            {
                                createList[i].FILE_PATH_NONRESTRICTED = RegistrationConstant.SIGNATURE_PATH + app.FILE_REFERENCE_NO.Replace("(", "_").Replace(")", "_").Replace(" ", "_").Replace("/", "_") + "\\" +
                                                    createList[i].C_APPLICANT.SURNAME + ", " + createList[i].C_APPLICANT.GIVEN_NAME_ON_ID + System.IO.Path.GetExtension(createList[i].Applicant_File.FileName);
                                string filePath = ApplicationConstant.CRMFilePath + RegistrationConstant.SIGNATURE_PATH + app.FILE_REFERENCE_NO.Replace("(", "_").Replace(")", "_").Replace(" ", "_").Replace("/", "_") + "\\";
                                System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                                file.Directory.Create();
                                // filePath = filePath.Replace(@"\\", @"\");
                                createList[i].Applicant_File.SaveAs(Path.Combine(ApplicationConstant.CRMFilePath, createList[i].FILE_PATH_NONRESTRICTED));

                            }

                            db.C_COMP_APPLICANT_INFO.Add(createList[i]);
                        }
                        db.SaveChanges();
                        //master

                        List<C_COMP_APPLICANT_INFO_MASTER> draftMasters = SessionUtil.DraftList<C_COMP_APPLICANT_INFO_MASTER>(ApplicationConstant.DRAFT_KEY_APPLICANT_MASTERS);
                        List<string> deleteMasters = SessionUtil.DraftList<string>(ApplicationConstant.DELETE_KEY_APPLICANT_MASTERS);


                        //master - delete

                        //IQueryable<C_COMP_APPLICANT_INFO_MASTER> delList22 = db.C_COMP_APPLICANT_INFO_MASTER.Where(o => deleteMasters.Contains(o.UUID)).ToList();
                        //for (int i = 0; i < delList22.Count; i++) db.Entry(delList22[i]).State = EntityState.Deleted;
                        IQueryable<C_COMP_APPLICANT_INFO_MASTER> delList22 =
                            db.C_COMP_APPLICANT_INFO_MASTER.Where(o => deleteMasters.Contains(o.UUID)).Include(o => o.C_COMP_APPLICANT_INFO_DETAIL);
                        db.C_COMP_APPLICANT_INFO_MASTER.RemoveRange(delList22);

                        db.SaveChanges();

                        //applicant - edit 
                        List<C_COMP_APPLICANT_INFO_MASTER> editAppMasterList = draftMasters.Where(o => !int.TryParse(o.UUID, out dummy)).ToList();
                        List<string> editUuid2 = (from t1 in editAppMasterList select t1.UUID).ToList();


                        List<C_COMP_APPLICANT_INFO_MASTER> dbEditAppMasterList = db.C_COMP_APPLICANT_INFO_MASTER.Where(o =>
                        editUuid2.Contains(o.UUID))
                        .ToList();
                        for (int i = 0; i < dbEditAppMasterList.Count; i++) dbEditAppMasterList[i].MergeByList(editAppMasterList);





                        //edit selected  Type/Class

                        Dictionary<string, List<string>> cos_classList = new Dictionary<string, List<string>>() {
                            { "1",new List<string>() { "Class 1", "Class 2", "Class 3" } },
                            { "2",new List<string>() { "Class 2", "Class 3" } },
                            { "3", new List<string>(){ "Class 3" } }
                        };

                        if (dbEditAppMasterList != null && dbEditAppMasterList.Count > 0)
                        {
                            List<C_COMP_APPLICANT_INFO_DETAIL> newAppDetails1 = new List<C_COMP_APPLICANT_INFO_DETAIL>();
                            List<string> searchUuid = dbEditAppMasterList.Select(o2 => o2.UUID).ToList();
                            List<C_COMP_APPLICANT_INFO_DETAIL> delAppDetails1 =
                                db.C_COMP_APPLICANT_INFO_DETAIL.Where(o => searchUuid.Contains(o.COMPANY_APPLICANTS_MASTER_ID)).ToList();
                            DisplayGrid g = new DisplayGrid() { Query = GetAllType_q, Rpp = 9999 };
                            g.Search();
                            List<string> searchClassList = new List<string>();
                            string searchType = "";
                            string stage;
                            for (int i = 0; i < dbEditAppMasterList.Count; i++)
                            {
                                foreach (string key in dbEditAppMasterList[i].TypeApply.Keys)
                                {
                                    if (!string.IsNullOrWhiteSpace(dbEditAppMasterList[i].TypeApply[key]))
                                    {
                                        searchType = key;
                                        searchClassList = cos_classList[dbEditAppMasterList[i].TypeApply[key]];
                                        stage = "APPLY";

                                        List<Dictionary<string, object>> oo =
                                        g.Data.Where(o => o["T"] as string == searchType
                                        && searchClassList.Contains(o["C"] as string)).ToList();
                                        for (int j = 0; j < oo.Count; j++)
                                        {
                                            C_COMP_APPLICANT_INFO_DETAIL details = new C_COMP_APPLICANT_INFO_DETAIL()
                                            {
                                                C_COMP_APPLICANT_INFO_MASTER = dbEditAppMasterList[i],
                                                ITEM_CLASS_ID = oo[j]["CUUID"] as string,
                                                ITEM_TYPE_ID = oo[j]["TUUID"] as string,
                                                STATUS_CODE = stage
                                            };
                                            newAppDetails1.Add(details);
                                        }
                                    }
                                }

                                foreach (string key in dbEditAppMasterList[i].TypeApprove.Keys)
                                {
                                    if (!string.IsNullOrWhiteSpace(dbEditAppMasterList[i].TypeApprove[key]))
                                    {
                                        searchType = key;
                                        searchClassList = cos_classList[dbEditAppMasterList[i].TypeApprove[key]];
                                        stage = "APPROVED";

                                        List<Dictionary<string, object>> oo =
                                        g.Data.Where(o => o["T"] as string == searchType
                                        && searchClassList.Contains(o["C"] as string)).ToList();
                                        for (int j = 0; j < oo.Count; j++)
                                        {
                                            C_COMP_APPLICANT_INFO_DETAIL details = new C_COMP_APPLICANT_INFO_DETAIL()
                                            {
                                                C_COMP_APPLICANT_INFO_MASTER = dbEditAppMasterList[i],
                                                ITEM_CLASS_ID = oo[j]["CUUID"] as string,
                                                ITEM_TYPE_ID = oo[j]["TUUID"] as string,
                                                STATUS_CODE = stage
                                            };

                                            newAppDetails1.Add(details);
                                        }
                                    }
                                }
                            }
                            db.C_COMP_APPLICANT_INFO_DETAIL.RemoveRange(delAppDetails1);
                            db.C_COMP_APPLICANT_INFO_DETAIL.AddRange(newAppDetails1);
                        }







                        //master - create 
                        List<C_COMP_APPLICANT_INFO_MASTER> createList2 = draftMasters.Where(o => int.TryParse(o.UUID, out dummy)).ToList();
                        for (int i = 0; i < createList2.Count; i++)
                        {
                            createList2[i].UUID = null;
                            if (MAP_Applicant.Count() > 0 && MAP_Applicant[createList2[i].C_COMP_APPLICANT_INFO.UUID] != null)
                            {
                                //createList2[i].UUID = null;
                                createList2[i].C_COMP_APPLICANT_INFO = MAP_Applicant[createList2[i].C_COMP_APPLICANT_INFO.UUID];
                                //                            db.C_COMP_APPLICANT_INFO_MASTER.Add(createList2[i]);
                            }
                            else
                            {

                                createList2[i].COMPANY_APPLICANTS_ID = createList2[i].C_COMP_APPLICANT_INFO.UUID;
                                createList2[i].C_COMP_APPLICANT_INFO = null;

                            }
                            db.C_COMP_APPLICANT_INFO_MASTER.Add(createList2[i]);
                        }
                        db.SaveChanges();
                        if (createList2 != null && createList2.Count > 0)
                        {
                            List<C_COMP_APPLICANT_INFO_DETAIL> newAppDetails1 = new List<C_COMP_APPLICANT_INFO_DETAIL>();
                            List<string> searchUuid = createList2.Select(o2 => o2.UUID).ToList();
                            List<C_COMP_APPLICANT_INFO_DETAIL> delAppDetails1 =
                                db.C_COMP_APPLICANT_INFO_DETAIL.Where(o => searchUuid.Contains(o.COMPANY_APPLICANTS_MASTER_ID)).ToList();
                            DisplayGrid g = new DisplayGrid() { Query = GetAllType_q, Rpp = 9999 };
                            g.Search();
                            List<string> searchClassList = new List<string>();
                            string searchType = "";
                            string stage;
                            for (int i = 0; i < createList2.Count; i++)
                            {
                                foreach (string key in createList2[i].TypeApply.Keys)
                                {
                                    if (!string.IsNullOrWhiteSpace(createList2[i].TypeApply[key]))
                                    {
                                        searchType = key;
                                        searchClassList = cos_classList[createList2[i].TypeApply[key]];
                                        stage = "APPLY";

                                        List<Dictionary<string, object>> oo =
                                        g.Data.Where(o => o["T"] as string == searchType
                                        && searchClassList.Contains(o["C"] as string)).ToList();
                                        for (int j = 0; j < oo.Count; j++)
                                        {
                                            C_COMP_APPLICANT_INFO_DETAIL details = new C_COMP_APPLICANT_INFO_DETAIL()
                                            {
                                                C_COMP_APPLICANT_INFO_MASTER = createList2[i],
                                                ITEM_CLASS_ID = oo[j]["CUUID"] as string,
                                                ITEM_TYPE_ID = oo[j]["TUUID"] as string,
                                                STATUS_CODE = stage
                                            };
                                            newAppDetails1.Add(details);
                                        }
                                    }
                                }

                                foreach (string key in createList2[i].TypeApprove.Keys)
                                {
                                    if (!string.IsNullOrWhiteSpace(createList2[i].TypeApprove[key]))
                                    {
                                        searchType = key;
                                        searchClassList = cos_classList[createList2[i].TypeApprove[key]];
                                        stage = "APPROVED";

                                        List<Dictionary<string, object>> oo =
                                        g.Data.Where(o => o["T"] as string == searchType
                                        && searchClassList.Contains(o["C"] as string)).ToList();
                                        for (int j = 0; j < oo.Count; j++)
                                        {
                                            C_COMP_APPLICANT_INFO_DETAIL details = new C_COMP_APPLICANT_INFO_DETAIL()
                                            {
                                                C_COMP_APPLICANT_INFO_MASTER = createList2[i],
                                                ITEM_CLASS_ID = oo[j]["CUUID"] as string,
                                                ITEM_TYPE_ID = oo[j]["TUUID"] as string,
                                                STATUS_CODE = stage
                                            };

                                            newAppDetails1.Add(details);
                                        }
                                    }
                                }
                            }
                            db.C_COMP_APPLICANT_INFO_DETAIL.RemoveRange(delAppDetails1);
                            db.C_COMP_APPLICANT_INFO_DETAIL.AddRange(newAppDetails1);
                            db.SaveChanges();
                        }


                        #region Pooling

                        var PoolingQuery = db.C_POOLING.Where(x => x.MASTER_ID == model.C_COMP_APPLICATION.UUID);
                        db.C_POOLING.RemoveRange(PoolingQuery);
                        if (model.PoolingRefNo != null)
                        {
                            var createDate = DateTime.Now;
                            foreach (var item in model.PoolingRefNo)
                            {
                                if (!string.IsNullOrWhiteSpace(item))
                                {
                                    createDate = createDate.AddSeconds(1);
                                    C_POOLING c_POOLING = new C_POOLING();
                                    c_POOLING.UUID = Guid.NewGuid().ToString().Replace(",", "");
                                    c_POOLING.MODIFIED_DATE = DateTime.Now;
                                    c_POOLING.MODIFIED_BY = SystemParameterConstant.UserName;
                                    c_POOLING.CREATED_BY = SystemParameterConstant.UserName;
                                    c_POOLING.CREATED_DATE = createDate;
                                    c_POOLING.POOL_NO = item;
                                    c_POOLING.MASTER_ID = model.C_COMP_APPLICATION.UUID;
                                    db.C_POOLING.Add(c_POOLING);
                                }

                            }
                        }




                        #endregion






                        List<C_COMP_APPLICANT_INFO> finalApplicants =
                            db.C_COMP_APPLICANT_INFO.Where(o => o.MASTER_ID == app.UUID).ToList();
                        for (int i = 0; i < finalApplicants.Count; i++)
                        {
                            string q = "SELECT T1.UUID AS COMPANY_APPLICANTS_ID, T4.UUID AS ITEM_TYPE_ID, T5.UUID AS ITEM_CLASS_ID,"
                            + "\r\n\t" + "'X' AS UUID, 'X' AS CREATED_BY, SYSDATE AS CREATED_DATE, 'X' AS MODIFIED_BY, SYSDATE AS MODIFIED_DATE                  "
                            + "\r\n\t" + "FROM C_COMP_APPLICANT_INFO T1                                                                                               "
                            + "\r\n\t" + "INNER JOIN C_COMP_APPLICANT_INFO_MASTER T2 ON T1.UUID = T2.COMPANY_APPLICANTS_ID                                            "
                            + "\r\n\t" + "INNER JOIN C_COMP_APPLICANT_INFO_DETAIL T3 ON T3.COMPANY_APPLICANTS_MASTER_ID = T2.UUID                                     "
                            + "\r\n\t" + "INNER JOIN C_S_SYSTEM_VALUE T4 ON T3.ITEM_TYPE_ID = T4.UUID                                                                 "
                            + "\r\n\t" + "INNER JOIN C_S_SYSTEM_VALUE T5 ON T3.ITEM_CLASS_ID = T5.UUID                                                                "
                            + "\r\n\t" + "WHERE T1.UUID = :applicantUuid                                                                          "
                            + "\r\n\t" + "AND T2.STATUS_CODE = 'Confirmed' AND T3.STATUS_CODE = 'APPROVED' AND T2.APPLICATION_DATE >= (                               "
                            + "\r\n\t" + "	SELECT CASE WHEN MAX(J1.APPLICATION_DATE) IS NULL THEN TO_DATE('18000101','YYYYMMDD') ELSE MAX(J1.APPLICATION_DATE) END  "
                            + "\r\n\t" + "	FROM C_COMP_APPLICANT_INFO_MASTER J1 INNER JOIN C_S_SYSTEM_VALUE J2 ON J1.APPLICATION_FORM_ID = J2.UUID                  "
                            + "\r\n\t" + "	WHERE J2.CODE IN ('BA25A', 'BA25B', 'BA25D', 'BA25') AND J1.COMPANY_APPLICANTS_ID = :applicantUuid   "
                            + "\r\n\t" + ")              ";
                            List<C_COMP_APPLICANT_MW_ITEM> addList = db.Database.SqlQuery<C_COMP_APPLICANT_MW_ITEM>(q, new OracleParameter("applicantUuid", finalApplicants[i].UUID))
                                .ToList().Select(o => { o.UUID = null; o.MODIFIED_BY = null; o.MODIFIED_DATE = finalApplicants[i].MODIFIED_DATE; o.CREATED_BY = null; o.CREATED_DATE = finalApplicants[i].MODIFIED_DATE; return o; }).ToList();

                            string finalApplicantsiUUID = finalApplicants[i].UUID;
                            db.C_COMP_APPLICANT_MW_ITEM.RemoveRange(db.C_COMP_APPLICANT_MW_ITEM.Where(o => o.COMPANY_APPLICANTS_ID == finalApplicantsiUUID));
                            db.C_COMP_APPLICANT_MW_ITEM.AddRange(addList);

                        }
                        db.SaveChanges();
                        transaction.Commit();
                    }
                    catch (DbEntityValidationException e)
                    {
                        string exception = "";
                        foreach (var item in e.EntityValidationErrors)
                        {
                            foreach (var i in item.ValidationErrors)
                            {
                                exception += i.ErrorMessage + " *** ";
                            }


                        }
                        Console.WriteLine("[" + exception + "]");
                        Console.WriteLine("DbEntityValidationException:" + e.EntityValidationErrors);

                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { "!!!" + exception + "!!!" } };

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                        return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
                    }
                }
                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
            }
        }

        public CompanyNamesModel GetCompanyNames(CompanyNamesModel model)
        {

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_COMP_APPLICATION v = db.C_COMP_APPLICATION.Find(model.C_COMP_APPLICATION_UUID);
                model.ENGLISH_COMPANY_NAME = v?.ENGLISH_COMPANY_NAME;
                model.CHINESE_COMPANY_NAME = v?.CHINESE_COMPANY_NAME;
                model.FILE_REFERENCE_NO = v?.FILE_REFERENCE_NO;
                return model;
            }
        }

        public ApplicantDisplayListModel GetCompApplicantInfo(string C_COMP_APPLICATION_UUID, string C_COMP_APPLICANT_INFO_UUID, string regType)
        {
            ApplicantDisplayListModel model = new ApplicantDisplayListModel() { RegType = regType };
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                List<C_COMP_APPLICANT_INFO> savingList = SessionUtil.DraftList<C_COMP_APPLICANT_INFO>(ApplicationConstant.DRAFT_KEY_APPLICANTS);

                if (string.IsNullOrWhiteSpace(C_COMP_APPLICANT_INFO_UUID))
                    model.C_COMP_APPLICANT_INFO = new C_COMP_APPLICANT_INFO() { C_COMP_APPLICATION = new C_COMP_APPLICATION() { UUID = C_COMP_APPLICATION_UUID } };

                else
                {
                    model.C_COMP_APPLICANT_INFO = savingList.Where(o => o.UUID == C_COMP_APPLICANT_INFO_UUID).FirstOrDefault();
                    if (model.C_COMP_APPLICANT_INFO == null)
                    {
                        model.C_COMP_APPLICANT_INFO = db.C_COMP_APPLICANT_INFO.Where(o => o.UUID == C_COMP_APPLICANT_INFO_UUID).Include(o => o.C_APPLICANT).Include(o => o.C_COMP_APPLICATION).FirstOrDefault();
                        model.C_COMP_APPLICANT_INFO?.C_APPLICANT?.Decrypt();
                    }
                }
                return model;
            }
        }
        public ServiceResult DraftApplicant(CompApplicantEditModel model)
        {
            C_COMP_APPLICANT_INFO C_COMP_APPLICANT_INFO = model.C_COMP_APPLICANT_INFO;
            List<C_COMP_APPLICANT_INFO> draftApplicant = SessionUtil.DraftList<C_COMP_APPLICANT_INFO>(ApplicationConstant.DRAFT_KEY_APPLICANTS);
            List<C_COMP_APPLICANT_INFO_MASTER> draftMaster = SessionUtil.DraftList<C_COMP_APPLICANT_INFO_MASTER>(ApplicationConstant.DRAFT_KEY_APPLICANT_MASTERS);



            if (C_COMP_APPLICANT_INFO.UUID == null) C_COMP_APPLICANT_INFO.UUID = SessionUtil.DRAFT_NEXT_ID;

            C_COMP_APPLICANT_INFO savingItem = draftApplicant.Where(o => o.UUID == C_COMP_APPLICANT_INFO.UUID).FirstOrDefault();
            if (savingItem != null) draftApplicant.Remove(savingItem);
            if (C_COMP_APPLICANT_INFO.UUID.Equals("TEMP", StringComparison.OrdinalIgnoreCase))
            {
                C_COMP_APPLICANT_INFO.UUID = SessionUtil.DRAFT_NEXT_ID;
                foreach (var item in draftMaster.Where(c => c.C_COMP_APPLICANT_INFO.UUID == "TEMP"))
                {
                    item.C_COMP_APPLICANT_INFO.UUID = C_COMP_APPLICANT_INFO.UUID;
                }
            }
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_COMP_APPLICANT_INFO.C_S_SYSTEM_VALUE = db.C_S_SYSTEM_VALUE.Find(C_COMP_APPLICANT_INFO.APPLICANT_ROLE_ID);
                C_COMP_APPLICANT_INFO.C_S_SYSTEM_VALUE1 = db.C_S_SYSTEM_VALUE.Find(C_COMP_APPLICANT_INFO.APPLICANT_STATUS_ID);
                C_COMP_APPLICANT_INFO.C_APPLICANT.C_S_SYSTEM_VALUE = db.C_S_SYSTEM_VALUE.Find(C_COMP_APPLICANT_INFO.C_APPLICANT.TITLE_ID);
            }
            C_COMP_APPLICANT_INFO.Applicant_File = model.APPLICANT_FILE.ElementAt(0);
            C_COMP_APPLICANT_INFO.FILEEXSIST = C_COMP_APPLICANT_INFO.Applicant_File != null ? true : false;
            draftApplicant.Add(C_COMP_APPLICANT_INFO);
            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }

        public ServiceResult AddEformApplicantGC(string formId, string formType, string refNo)
        {
            var ApplicantData = getEFSS_ApplicantData(formId, formType);
            var compUUID = "";
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var compApplication = db.C_COMP_APPLICATION.Where(x => x.FILE_REFERENCE_NO == refNo).FirstOrDefault();
                compUUID = compApplication != null ? compApplication.UUID : "";
            }
            var compApplicantList = AjaxMergedApplicantList(compUUID);

            C_EFSS_APPLICANT targetApplicant = new C_EFSS_APPLICANT();


            List<C_COMP_APPLICANT_INFO> draftApplicant = SessionUtil.DraftList<C_COMP_APPLICANT_INFO>(ApplicationConstant.DRAFT_KEY_APPLICANTS);


            for (int i = 0; i < ApplicantData.Count(); i++)
            {
                if (string.IsNullOrWhiteSpace(ApplicantData[i].ROLE)) continue;
                if (string.IsNullOrWhiteSpace(ApplicantData[i].HKID) && string.IsNullOrWhiteSpace(ApplicantData[i].PASSPORTNO)) continue;

                targetApplicant = ApplicantData[i];

                ApplicantDisplayListModel applicant = new ApplicantDisplayListModel();
                C_COMP_APPLICANT_INFO C_COMP_APPLICANT_INFO = new C_COMP_APPLICANT_INFO();

                // check with existing applicant list
                if (applicant.C_APPLICANT_UUID == null)
                {
                    for (int j = 0; j < compApplicantList.Count(); j++)
                    {
                        if ((compApplicantList[j].HKID.Equals(EncryptDecryptUtil.getDecryptHKID(targetApplicant.HKID))
                             && !string.IsNullOrWhiteSpace(targetApplicant.HKID)) ||
                             (compApplicantList[j].PASSPORT.Equals(EncryptDecryptUtil.getDecryptHKID(targetApplicant.PASSPORTNO))
                             && !string.IsNullOrWhiteSpace(targetApplicant.PASSPORTNO)) &&
                             (RegistrationCommonService.getCodebyUUID(compApplicantList[i].ROLE).Equals(targetApplicant.ROLE))
                            )
                        {
                            applicant = compApplicantList[j];
                            break;
                        }
                    }
                }
                if (applicant.C_APPLICANT_UUID == null)
                {
                    for (int j = 0; j < compApplicantList.Count(); j++)
                    {
                        ApplicantDisplayListModel applicant2 = compApplicantList[j];
                        if (!string.IsNullOrWhiteSpace(targetApplicant.HKID))
                        {
                            if (EncryptDecryptUtil.getDecryptHKID(targetApplicant.HKID).Equals(applicant2.HKID))
                            {
                                applicant = applicant2;
                                break;
                            }
                            else if (!string.IsNullOrWhiteSpace(targetApplicant.PASSPORTNO))
                            {
                                if (EncryptDecryptUtil.getDecryptHKID(targetApplicant.PASSPORTNO).Equals(applicant2.PASSPORT))
                                {
                                    applicant = applicant2;
                                    break;
                                }
                            }
                        }

                    }
                }

                if (applicant.C_APPLICANT_UUID == null) // new applicant
                {
                    C_COMP_APPLICANT_INFO.UUID = SessionUtil.DRAFT_NEXT_ID;
                    C_COMP_APPLICANT_INFO.C_APPLICANT = new C_APPLICANT();
                    C_COMP_APPLICANT_INFO.C_APPLICANT.HKID = targetApplicant.HKID;
                    C_COMP_APPLICANT_INFO.C_APPLICANT.PASSPORT_NO = targetApplicant.PASSPORTNO;
                    C_COMP_APPLICANT_INFO.C_APPLICANT.SURNAME = targetApplicant.SURNAME;
                    C_COMP_APPLICANT_INFO.C_APPLICANT.GIVEN_NAME_ON_ID = targetApplicant.GIVENNAME;
                    C_COMP_APPLICANT_INFO.C_APPLICANT.CHINESE_NAME = targetApplicant.CHNNAME;
                    C_COMP_APPLICANT_INFO.C_APPLICANT.GENDER = targetApplicant.SEX;
                    C_COMP_APPLICANT_INFO.APPLICANT_ROLE_ID = RegistrationCommonService.getUUIDbyTypeAndCode(RegistrationConstant.SYSTEM_TYPE_APPLICANT_ROLE, targetApplicant.ROLE);
                    C_COMP_APPLICANT_INFO.CORRESPONDENCE_LANG_ID = targetApplicant.COMLANG; // CORRESPONDENCE_LANG_ID: E/C
                    C_COMP_APPLICANT_INFO.C_APPLICANT.IS_POOLING = "N";
                    C_COMP_APPLICANT_INFO.EFORM = true;
                    using (EntitiesRegistration db = new EntitiesRegistration())
                    {
                        C_COMP_APPLICANT_INFO.C_S_SYSTEM_VALUE = db.C_S_SYSTEM_VALUE.Find(C_COMP_APPLICANT_INFO.APPLICANT_ROLE_ID);
                        C_COMP_APPLICANT_INFO.C_S_SYSTEM_VALUE1 = db.C_S_SYSTEM_VALUE.Find(C_COMP_APPLICANT_INFO.APPLICANT_STATUS_ID);
                        //C_COMP_APPLICANT_INFO.C_APPLICANT.C_S_SYSTEM_VALUE = db.C_S_SYSTEM_VALUE.Find(C_COMP_APPLICANT_INFO.C_APPLICANT.TITLE_ID);
                    }
                }
                else // modify existing applicant
                {
                    using (EntitiesRegistration db = new EntitiesRegistration())
                    {
                        C_COMP_APPLICANT_INFO = db.C_COMP_APPLICANT_INFO.Find(applicant.C_COMP_APPLICANT_INFO_UUID);
                        C_COMP_APPLICANT_INFO.C_APPLICANT = db.C_APPLICANT.Find(applicant.C_APPLICANT_UUID);
                    }
                    C_COMP_APPLICANT_INFO.UUID = applicant.C_COMP_APPLICANT_INFO_UUID;
                    C_COMP_APPLICANT_INFO.MASTER_ID = applicant.C_COMP_APPLICANT_INFO_MASTER_UUID;

                    if (!string.IsNullOrWhiteSpace(targetApplicant.HKID))
                    {
                        C_COMP_APPLICANT_INFO.C_APPLICANT.HKID = targetApplicant.HKID;
                    }
                    if (!string.IsNullOrWhiteSpace(targetApplicant.PASSPORTNO))
                    {
                        C_COMP_APPLICANT_INFO.C_APPLICANT.PASSPORT_NO = targetApplicant.PASSPORTNO;
                    }
                    if (!string.IsNullOrWhiteSpace(targetApplicant.SURNAME))
                    {
                        C_COMP_APPLICANT_INFO.C_APPLICANT.SURNAME = targetApplicant.SURNAME;
                    }
                    if (!string.IsNullOrWhiteSpace(targetApplicant.GIVENNAME))
                    {
                        C_COMP_APPLICANT_INFO.C_APPLICANT.GIVEN_NAME_ON_ID = targetApplicant.GIVENNAME;
                    }
                    if (!string.IsNullOrWhiteSpace(targetApplicant.CHNNAME))
                    {
                        C_COMP_APPLICANT_INFO.C_APPLICANT.CHINESE_NAME = targetApplicant.CHNNAME;
                    }
                    if (!string.IsNullOrWhiteSpace(targetApplicant.SEX))
                    {
                        C_COMP_APPLICANT_INFO.C_APPLICANT.GENDER = targetApplicant.SEX;
                    }
                    if (!string.IsNullOrWhiteSpace(targetApplicant.ROLE))
                    {
                        C_COMP_APPLICANT_INFO.APPLICANT_ROLE_ID = RegistrationCommonService.getUUIDbyTypeAndCode(RegistrationConstant.SYSTEM_TYPE_APPLICANT_ROLE, targetApplicant.ROLE);
                    }
                    if (!string.IsNullOrWhiteSpace(targetApplicant.COMLANG))
                    {
                        C_COMP_APPLICANT_INFO.CORRESPONDENCE_LANG_ID = targetApplicant.COMLANG; // CORRESPONDENCE_LANG_ID: E/C
                    }
                    C_COMP_APPLICANT_INFO.C_APPLICANT.IS_POOLING = "N";
                    using (EntitiesRegistration db = new EntitiesRegistration())
                    {
                        C_COMP_APPLICANT_INFO.C_S_SYSTEM_VALUE = db.C_S_SYSTEM_VALUE.Find(C_COMP_APPLICANT_INFO.APPLICANT_ROLE_ID);
                        C_COMP_APPLICANT_INFO.C_S_SYSTEM_VALUE1 = db.C_S_SYSTEM_VALUE.Find(C_COMP_APPLICANT_INFO.APPLICANT_STATUS_ID);
                        C_COMP_APPLICANT_INFO.C_APPLICANT.C_S_SYSTEM_VALUE = db.C_S_SYSTEM_VALUE.Find(C_COMP_APPLICANT_INFO.C_APPLICANT.TITLE_ID);
                    }
                }
                C_COMP_APPLICANT_INFO.EFORM = true;
                C_COMP_APPLICANT_INFO.C_APPLICANT.Decrypt();
                draftApplicant.Add(C_COMP_APPLICANT_INFO);
            }

            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }

        public ServiceResult AddEformApplicantMWC(string formId, string formType, string refNo)
        {
            var ApplicantData = getEFSS_ApplicantData(formId, formType);
            var compUUID = "";
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var compApplication = db.C_COMP_APPLICATION.Where(x => x.FILE_REFERENCE_NO == refNo).FirstOrDefault();
                compUUID = compApplication != null ? compApplication.UUID : "";
            }
            var compApplicantList = AjaxMergedApplicantList(compUUID);

            C_EFSS_APPLICANT targetApplicant = new C_EFSS_APPLICANT();


            List<C_COMP_APPLICANT_INFO> draftApplicant = SessionUtil.DraftList<C_COMP_APPLICANT_INFO>(ApplicationConstant.DRAFT_KEY_APPLICANTS);
            //List<C_COMP_APPLICANT_INFO_MASTER> draftMaster = SessionUtil.DraftList<C_COMP_APPLICANT_INFO_MASTER>(ApplicationConstant.DRAFT_KEY_APPLICANT_MASTERS);


            for (int i = 0; i < ApplicantData.Count(); i++)
            {
                if (string.IsNullOrWhiteSpace(ApplicantData[i].ROLE)) continue;
                if (string.IsNullOrWhiteSpace(ApplicantData[i].HKID) && string.IsNullOrWhiteSpace(ApplicantData[i].PASSPORTNO)) continue;

                targetApplicant = ApplicantData[i];

                ApplicantDisplayListModel applicant = new ApplicantDisplayListModel();
                C_COMP_APPLICANT_INFO C_COMP_APPLICANT_INFO = new C_COMP_APPLICANT_INFO();
                CompAppHistEditModel modelHist = new CompAppHistEditModel();

                // check with existing applicant list
                if (applicant.C_APPLICANT_UUID == null)
                {
                    for (int j = 0; j < compApplicantList.Count(); j++)
                    {
                        compApplicantList[j].HKID = compApplicantList[j].HKID != null ? compApplicantList[j].HKID : "";
                        targetApplicant.HKID = targetApplicant.HKID != null ? targetApplicant.HKID : "";
                        compApplicantList[j].PASSPORT = compApplicantList[j].PASSPORT != null ? compApplicantList[j].PASSPORT : "";
                        targetApplicant.PASSPORTNO = targetApplicant.PASSPORTNO != null ? targetApplicant.PASSPORTNO : "";
                        if ((compApplicantList[j].HKID.Equals(EncryptDecryptUtil.getDecryptHKID(targetApplicant.HKID))
                             && !string.IsNullOrWhiteSpace(targetApplicant.HKID) && targetApplicant.HKID != null) ||
                             (compApplicantList[j].PASSPORT.Equals(EncryptDecryptUtil.getDecryptHKID(targetApplicant.PASSPORTNO))
                             && !string.IsNullOrWhiteSpace(targetApplicant.PASSPORTNO) && targetApplicant.PASSPORTNO != null) &&
                             (RegistrationCommonService.getCodebyUUID(compApplicantList[i].ROLE).Equals(targetApplicant.ROLE))
                            )
                        {
                            applicant = compApplicantList[j];
                            break;
                        }
                    }
                }
                if (applicant.C_APPLICANT_UUID == null)
                {
                    for (int j = 0; j < compApplicantList.Count(); j++)
                    {
                        ApplicantDisplayListModel applicant2 = compApplicantList[j];
                        if (!string.IsNullOrWhiteSpace(targetApplicant.HKID))
                        {
                            if (EncryptDecryptUtil.getDecryptHKID(targetApplicant.HKID).Equals(applicant2.HKID))
                            {
                                applicant = applicant2;
                                break;
                            }
                            else if (!string.IsNullOrWhiteSpace(targetApplicant.PASSPORTNO))
                            {
                                if (EncryptDecryptUtil.getDecryptHKID(targetApplicant.PASSPORTNO).Equals(applicant2.PASSPORT))
                                {
                                    applicant = applicant2;
                                    break;
                                }
                            }
                        }

                    }
                }

                if (applicant.C_APPLICANT_UUID == null) // new applicant
                {
                    C_COMP_APPLICANT_INFO.UUID = SessionUtil.DRAFT_NEXT_ID;
                    C_COMP_APPLICANT_INFO.C_APPLICANT = new C_APPLICANT();
                    C_COMP_APPLICANT_INFO.C_APPLICANT.HKID = targetApplicant.HKID;
                    C_COMP_APPLICANT_INFO.C_APPLICANT.PASSPORT_NO = targetApplicant.PASSPORTNO;
                    C_COMP_APPLICANT_INFO.C_APPLICANT.SURNAME = targetApplicant.SURNAME;
                    C_COMP_APPLICANT_INFO.C_APPLICANT.GIVEN_NAME_ON_ID = targetApplicant.GIVENNAME;
                    C_COMP_APPLICANT_INFO.C_APPLICANT.CHINESE_NAME = targetApplicant.CHNNAME;
                    C_COMP_APPLICANT_INFO.C_APPLICANT.GENDER = targetApplicant.SEX;
                    C_COMP_APPLICANT_INFO.APPLICANT_ROLE_ID = RegistrationCommonService.getUUIDbyTypeAndCode(RegistrationConstant.SYSTEM_TYPE_APPLICANT_ROLE, targetApplicant.ROLE);
                    C_COMP_APPLICANT_INFO.CORRESPONDENCE_LANG_ID = targetApplicant.COMLANG; // CORRESPONDENCE_LANG_ID: E/C
                    C_COMP_APPLICANT_INFO.C_APPLICANT.IS_POOLING = "N";
                    C_COMP_APPLICANT_INFO.EFORM = true;
                    using (EntitiesRegistration db = new EntitiesRegistration())
                    {
                        C_COMP_APPLICANT_INFO.C_S_SYSTEM_VALUE = db.C_S_SYSTEM_VALUE.Find(C_COMP_APPLICANT_INFO.APPLICANT_ROLE_ID);
                        C_COMP_APPLICANT_INFO.C_S_SYSTEM_VALUE1 = db.C_S_SYSTEM_VALUE.Find(C_COMP_APPLICANT_INFO.APPLICANT_STATUS_ID);
                        //C_COMP_APPLICANT_INFO.C_APPLICANT.C_S_SYSTEM_VALUE = db.C_S_SYSTEM_VALUE.Find(C_COMP_APPLICANT_INFO.C_APPLICANT.TITLE_ID);
                        C_COMP_APPLICANT_INFO.C_COMP_APPLICANT_INFO_MASTER = db.C_COMP_APPLICANT_INFO_MASTER.Where(x => x.COMPANY_APPLICANTS_ID == C_COMP_APPLICANT_INFO.UUID).ToList();
                    }
                    if (!formType.Equals("BA24"))
                    {
                        List<C_EFSS_APPLICANT_MW_CAPA> ApplicantHistData = new List<C_EFSS_APPLICANT_MW_CAPA>();
                        using (EntitiesRegistration db = new EntitiesRegistration())
                        {
                            ApplicantHistData = db.C_EFSS_APPLICANT_MW_CAPA.Where(x => x.APPLICANT_ID == targetApplicant.ID).ToList();
                        }
                        string APPLICANT_ID = "";
                        string CLSTYPEA = "";
                        string CLSTYPEB = "";
                        string CLSTYPEC = "";
                        string CLSTYPED = "";
                        string CLSTYPEE = "";
                        string CLSTYPEF = "";
                        string CLSTYPEG = "";
                        DateTime? APPDATE = null;
                        string APPFORM = "";
                        for (int k = 0; k < ApplicantHistData.Count(); k++)
                        {
                            APPLICANT_ID = ApplicantHistData[k].APPLICANT_ID;
                            CLSTYPEA = ApplicantHistData[k].CLSTYPEA != null ? ApplicantHistData[k].CLSTYPEA : "";
                            CLSTYPEB = ApplicantHistData[k].CLSTYPEB != null ? ApplicantHistData[k].CLSTYPEB : "";
                            CLSTYPEC = ApplicantHistData[k].CLSTYPEC != null ? ApplicantHistData[k].CLSTYPEC : "";
                            CLSTYPED = ApplicantHistData[k].CLSTYPED != null ? ApplicantHistData[k].CLSTYPED : "";
                            CLSTYPEE = ApplicantHistData[k].CLSTYPEE != null ? ApplicantHistData[k].CLSTYPEE : "";
                            CLSTYPEF = ApplicantHistData[k].CLSTYPEF != null ? ApplicantHistData[k].CLSTYPEF : "";
                            CLSTYPEG = ApplicantHistData[k].CLSTYPEG != null ? ApplicantHistData[k].CLSTYPEG : "";
                            APPDATE = ApplicantHistData[k].APPDATE;
                            APPFORM = ApplicantHistData[k].APPFORM;

                            string AppFormUUID = RegistrationCommonService.getUUIDbyFormTypeCode("CMW", "CMW");
                            //string compApplicantInfoUUID = C_COMP_APPLICANT_INFO.UUID;
                            //string compApplicantInfoMasterUUID = C_COMP_APPLICANT_INFO.C_COMP_APPLICANT_INFO_MASTER.FirstOrDefault().UUID; // == applicant.C_COMP_APPLICANT_INFO_MASTER_UUID
                            string compApplicantInfoMasterUUID = SessionUtil.DRAFT_NEXT_ID;

                            C_COMP_APPLICANT_INFO compApplicantInfo = applicant.C_COMP_APPLICANT_INFO;

                            //List<C_COMP_APPLICANT_INFO_MASTER> mwCapMasterList = compApplicantInfo.C_COMP_APPLICANT_INFO_MASTER.ToList();
                            C_COMP_APPLICANT_INFO_MASTER selectedMaster = new C_COMP_APPLICANT_INFO_MASTER();

                            //for (int m = 0; m < mwCapMasterList.Count(); m++)
                            //{
                            //    if(mwCapMasterList[m].UUID.Equals(compApplicantInfoMasterUUID))
                            //    {
                            //        selectedMaster = mwCapMasterList[m];
                            //        break;
                            //    }
                            //}
                            if (selectedMaster.UUID == null)
                            {
                                modelHist.SelectedTypeApply0 = CLSTYPEA;
                                modelHist.SelectedTypeApply1 = CLSTYPEB;
                                modelHist.SelectedTypeApply2 = CLSTYPEC;
                                modelHist.SelectedTypeApply3 = CLSTYPED;
                                modelHist.SelectedTypeApply4 = CLSTYPEE;
                                modelHist.SelectedTypeApply5 = CLSTYPEF;
                                modelHist.SelectedTypeApply6 = CLSTYPEG;

                                modelHist.AppDate = APPDATE;
                                modelHist.AppFormId = APPFORM;
                                modelHist.AppStatus = RegistrationConstant.COMP_APPLICANT_INFO_DETAIL_STATUS_APPLY;
                                modelHist.EFORM = true;

                                modelHist.C_COMP_APPLICANT_INFO_MASTER = new C_COMP_APPLICANT_INFO_MASTER();
                                modelHist.C_COMP_APPLICANT_INFO_MASTER.C_COMP_APPLICANT_INFO = C_COMP_APPLICANT_INFO;
                                modelHist.C_COMP_APPLICANT_INFO_MASTER.APPLICATION_FORM_ID = RegistrationCommonService.getUUIDbyFormTypeCode(APPFORM, "CMW");
                            }
                        }
                    }
                }
                else // modify existing applicant
                {
                    using (EntitiesRegistration db = new EntitiesRegistration())
                    {
                        C_COMP_APPLICANT_INFO = db.C_COMP_APPLICANT_INFO.Find(applicant.C_COMP_APPLICANT_INFO_UUID);
                        C_COMP_APPLICANT_INFO.C_APPLICANT = db.C_APPLICANT.Find(applicant.C_APPLICANT_UUID);
                        C_COMP_APPLICANT_INFO.C_COMP_APPLICANT_INFO_MASTER = db.C_COMP_APPLICANT_INFO_MASTER.Where(x => x.COMPANY_APPLICANTS_ID == C_COMP_APPLICANT_INFO.UUID).ToList();
                    }
                    C_COMP_APPLICANT_INFO.UUID = applicant.C_COMP_APPLICANT_INFO_UUID;
                    C_COMP_APPLICANT_INFO.MASTER_ID = applicant.C_COMP_APPLICANT_INFO_MASTER_UUID;

                    if (!string.IsNullOrWhiteSpace(targetApplicant.HKID))
                    {
                        C_COMP_APPLICANT_INFO.C_APPLICANT.HKID = targetApplicant.HKID;
                    }
                    if (!string.IsNullOrWhiteSpace(targetApplicant.PASSPORTNO))
                    {
                        C_COMP_APPLICANT_INFO.C_APPLICANT.PASSPORT_NO = targetApplicant.PASSPORTNO;
                    }
                    if (!string.IsNullOrWhiteSpace(targetApplicant.SURNAME))
                    {
                        C_COMP_APPLICANT_INFO.C_APPLICANT.SURNAME = targetApplicant.SURNAME;
                    }
                    if (!string.IsNullOrWhiteSpace(targetApplicant.GIVENNAME))
                    {
                        C_COMP_APPLICANT_INFO.C_APPLICANT.GIVEN_NAME_ON_ID = targetApplicant.GIVENNAME;
                    }
                    if (!string.IsNullOrWhiteSpace(targetApplicant.CHNNAME))
                    {
                        C_COMP_APPLICANT_INFO.C_APPLICANT.CHINESE_NAME = targetApplicant.CHNNAME;
                    }
                    if (!string.IsNullOrWhiteSpace(targetApplicant.SEX))
                    {
                        C_COMP_APPLICANT_INFO.C_APPLICANT.GENDER = targetApplicant.SEX;
                    }
                    if (!string.IsNullOrWhiteSpace(targetApplicant.ROLE))
                    {
                        C_COMP_APPLICANT_INFO.APPLICANT_ROLE_ID = RegistrationCommonService.getUUIDbyTypeAndCode(RegistrationConstant.SYSTEM_TYPE_APPLICANT_ROLE, targetApplicant.ROLE);
                    }
                    if (!string.IsNullOrWhiteSpace(targetApplicant.COMLANG))
                    {
                        C_COMP_APPLICANT_INFO.CORRESPONDENCE_LANG_ID = targetApplicant.COMLANG; // CORRESPONDENCE_LANG_ID: E/C
                    }
                    C_COMP_APPLICANT_INFO.C_APPLICANT.IS_POOLING = "N";
                    using (EntitiesRegistration db = new EntitiesRegistration())
                    {
                        C_COMP_APPLICANT_INFO.C_S_SYSTEM_VALUE = db.C_S_SYSTEM_VALUE.Find(C_COMP_APPLICANT_INFO.APPLICANT_ROLE_ID);
                        C_COMP_APPLICANT_INFO.C_S_SYSTEM_VALUE1 = db.C_S_SYSTEM_VALUE.Find(C_COMP_APPLICANT_INFO.APPLICANT_STATUS_ID);
                        C_COMP_APPLICANT_INFO.C_APPLICANT.C_S_SYSTEM_VALUE = db.C_S_SYSTEM_VALUE.Find(C_COMP_APPLICANT_INFO.C_APPLICANT.TITLE_ID);
                    }
                    if (!formType.Equals("BA24"))
                    {
                        List<C_EFSS_APPLICANT_MW_CAPA> ApplicantHistData = new List<C_EFSS_APPLICANT_MW_CAPA>();
                        using (EntitiesRegistration db = new EntitiesRegistration())
                        {
                            ApplicantHistData = db.C_EFSS_APPLICANT_MW_CAPA.Where(x => x.APPLICANT_ID == targetApplicant.ID).ToList();
                        }
                        string APPLICANT_ID = "";
                        string CLSTYPEA = "";
                        string CLSTYPEB = "";
                        string CLSTYPEC = "";
                        string CLSTYPED = "";
                        string CLSTYPEE = "";
                        string CLSTYPEF = "";
                        string CLSTYPEG = "";
                        DateTime? APPDATE = null;
                        string APPFORM = "";
                        for (int k = 0; k < ApplicantHistData.Count(); k++)
                        {
                            APPLICANT_ID = ApplicantHistData[k].APPLICANT_ID;
                            CLSTYPEA = ApplicantHistData[k].CLSTYPEA != null ? ApplicantHistData[k].CLSTYPEA : "";
                            CLSTYPEB = ApplicantHistData[k].CLSTYPEB != null ? ApplicantHistData[k].CLSTYPEB : "";
                            CLSTYPEC = ApplicantHistData[k].CLSTYPEC != null ? ApplicantHistData[k].CLSTYPEC : "";
                            CLSTYPED = ApplicantHistData[k].CLSTYPED != null ? ApplicantHistData[k].CLSTYPED : "";
                            CLSTYPEE = ApplicantHistData[k].CLSTYPEE != null ? ApplicantHistData[k].CLSTYPEE : "";
                            CLSTYPEF = ApplicantHistData[k].CLSTYPEF != null ? ApplicantHistData[k].CLSTYPEF : "";
                            CLSTYPEG = ApplicantHistData[k].CLSTYPEG != null ? ApplicantHistData[k].CLSTYPEG : "";
                            APPDATE = ApplicantHistData[k].APPDATE;
                            APPFORM = ApplicantHistData[k].APPFORM;

                            string AppFormUUID = RegistrationCommonService.getUUIDbyFormTypeCode("CMW", "CMW");
                            //string compApplicantInfoUUID = C_COMP_APPLICANT_INFO.UUID;
                            //string compApplicantInfoMasterUUID = C_COMP_APPLICANT_INFO.C_COMP_APPLICANT_INFO_MASTER.FirstOrDefault().UUID; // == applicant.C_COMP_APPLICANT_INFO_MASTER_UUID
                            string compApplicantInfoMasterUUID = SessionUtil.DRAFT_NEXT_ID;

                            C_COMP_APPLICANT_INFO compApplicantInfo = applicant.C_COMP_APPLICANT_INFO;

                            //List<C_COMP_APPLICANT_INFO_MASTER> mwCapMasterList = compApplicantInfo.C_COMP_APPLICANT_INFO_MASTER.ToList();
                            C_COMP_APPLICANT_INFO_MASTER selectedMaster = new C_COMP_APPLICANT_INFO_MASTER();

                            //for (int m = 0; m < mwCapMasterList.Count(); m++)
                            //{
                            //    if(mwCapMasterList[m].UUID.Equals(compApplicantInfoMasterUUID))
                            //    {
                            //        selectedMaster = mwCapMasterList[m];
                            //        break;
                            //    }
                            //}
                            if (selectedMaster.UUID == null)
                            {
                                modelHist.SelectedTypeApply0 = CLSTYPEA;
                                modelHist.SelectedTypeApply1 = CLSTYPEB;
                                modelHist.SelectedTypeApply2 = CLSTYPEC;
                                modelHist.SelectedTypeApply3 = CLSTYPED;
                                modelHist.SelectedTypeApply4 = CLSTYPEE;
                                modelHist.SelectedTypeApply5 = CLSTYPEF;
                                modelHist.SelectedTypeApply6 = CLSTYPEG;

                                modelHist.AppDate = APPDATE;
                                modelHist.AppFormId = APPFORM;
                                modelHist.AppStatus = RegistrationConstant.COMP_APPLICANT_INFO_DETAIL_STATUS_APPLY;
                                modelHist.EFORM = true;

                                modelHist.C_COMP_APPLICANT_INFO_MASTER = new C_COMP_APPLICANT_INFO_MASTER();
                                modelHist.C_COMP_APPLICANT_INFO_MASTER.C_COMP_APPLICANT_INFO = C_COMP_APPLICANT_INFO;
                                modelHist.C_COMP_APPLICANT_INFO_MASTER.APPLICATION_FORM_ID = RegistrationCommonService.getUUIDbyFormTypeCode(APPFORM, "CMW");
                            }
                        }
                    }
                }
                C_COMP_APPLICANT_INFO.EFORM = true;
                C_COMP_APPLICANT_INFO.C_APPLICANT.Decrypt();
                draftApplicant.Add(C_COMP_APPLICANT_INFO);
                DraftAppHist(modelHist);
            }

            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }

        public ServiceResult DeleteApplicant(string C_COMP_APPLICANT_INFO_UUID)
        {
            List<C_COMP_APPLICANT_INFO> draftApplicant = SessionUtil.DraftList<C_COMP_APPLICANT_INFO>(ApplicationConstant.DRAFT_KEY_APPLICANTS);
            List<C_COMP_APPLICANT_INFO_MASTER> draftMaster = SessionUtil.DraftList<C_COMP_APPLICANT_INFO_MASTER>(ApplicationConstant.DRAFT_KEY_APPLICANT_MASTERS);

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var checking = db.C_COMP_PROCESS_MONITOR.Where(x => x.COMPANY_APPLICANTS_ID == C_COMP_APPLICANT_INFO_UUID);
                if (checking.Count() != 0)
                {
                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_FAILURE,
                        Message = new List<string>() { "The Applicant related to Process Monitor, cannot delete" }
                    };

                }

            }

            draftMaster.RemoveAll(o => o.C_COMP_APPLICANT_INFO.UUID == C_COMP_APPLICANT_INFO_UUID);
            int rseult = draftApplicant.RemoveAll(o => C_COMP_APPLICANT_INFO_UUID != null && o.UUID == C_COMP_APPLICANT_INFO_UUID);

            if (rseult == 0) SessionUtil.DraftList<string>(ApplicationConstant.DELETE_KEY_APPLICANTS).Add(C_COMP_APPLICANT_INFO_UUID);
            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }

        public AppHistDisplayModel GetCompApplicantInfoMaster(ApplicantDisplayListModel model, string regType)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                List<C_COMP_APPLICANT_INFO> draftApplicant = SessionUtil.DraftList<C_COMP_APPLICANT_INFO>(ApplicationConstant.DRAFT_KEY_APPLICANTS);
                List<C_COMP_APPLICANT_INFO_MASTER> draftMaster = SessionUtil.DraftList<C_COMP_APPLICANT_INFO_MASTER>(ApplicationConstant.DRAFT_KEY_APPLICANT_MASTERS);
                AppHistDisplayModel r = new AppHistDisplayModel() { RegType = regType };
                if (string.IsNullOrWhiteSpace(model.C_COMP_APPLICANT_INFO.UUID))
                {
                    model.C_COMP_APPLICANT_INFO.UUID = "TEMP";
                    draftMaster.RemoveAll(o => o.C_COMP_APPLICANT_INFO.UUID == "TEMP");
                    draftApplicant.RemoveAll(o => o.UUID == "TEMP");
                    draftApplicant.Add(model.C_COMP_APPLICANT_INFO);
                }

                if (string.IsNullOrWhiteSpace(model.C_COMP_APPLICANT_INFO_MASTER_UUID))
                {

                    //r.SURNAME =q.
                    var applicantInfo = db.C_COMP_APPLICANT_INFO.Where(x => x.UUID == model.C_COMP_APPLICANT_INFO.UUID).FirstOrDefault();
                    if (applicantInfo != null)
                    {
                        var query = db.C_APPLICANT.Where(x => x.UUID == applicantInfo.APPLICANT_ID).FirstOrDefault();
                        r.SURNAME = query.SURNAME;
                        r.ROLE = SystemListUtil.GetSVByUUID(applicantInfo.APPLICANT_ROLE_ID).CODE;
                        r.GIVEN_NAME_ON_ID = query.GIVEN_NAME_ON_ID;
                        r.HKID = EncryptDecryptUtil.getDecryptHKID(query.HKID);
                    }


                    r.C_COMP_APPLICANT_INFO_MASTER = new C_COMP_APPLICANT_INFO_MASTER() { C_COMP_APPLICANT_INFO = new C_COMP_APPLICANT_INFO() { UUID = model.C_COMP_APPLICANT_INFO.UUID } };


                }
                else
                {
                    r.C_COMP_APPLICANT_INFO_MASTER = draftMaster.Where(o => o.UUID == model.C_COMP_APPLICANT_INFO_MASTER_UUID).FirstOrDefault();
                    if (r.C_COMP_APPLICANT_INFO_MASTER == null)
                    {
                        C_COMP_APPLICANT_INFO_MASTER q = db.C_COMP_APPLICANT_INFO_MASTER.Where(o => o.UUID == model.C_COMP_APPLICANT_INFO_MASTER_UUID).Include(o => o.C_COMP_APPLICANT_INFO).FirstOrDefault();
                        r.C_COMP_APPLICANT_INFO_MASTER = q;
                        //r.SURNAME =q.
                        var query = db.C_APPLICANT.Where(x => x.UUID == q.C_COMP_APPLICANT_INFO.APPLICANT_ID).FirstOrDefault();
                        r.SURNAME = query.SURNAME;
                        r.ROLE = SystemListUtil.GetSVByUUID(q.C_COMP_APPLICANT_INFO.APPLICANT_ROLE_ID).CODE;
                        r.GIVEN_NAME_ON_ID = query.GIVEN_NAME_ON_ID;
                        r.HKID = EncryptDecryptUtil.getDecryptHKID(query.HKID);
                    }
                }

                return r;
            }
        }
        public ServiceResult DraftAppHist(CompAppHistEditModel model)
        {
            C_COMP_APPLICANT_INFO_MASTER C_COMP_APPLICANT_INFO_MASTER = model.C_COMP_APPLICANT_INFO_MASTER;
            List<C_COMP_APPLICANT_INFO_MASTER> draftMaster = SessionUtil.DraftList<C_COMP_APPLICANT_INFO_MASTER>(ApplicationConstant.DRAFT_KEY_APPLICANT_MASTERS);
            if (C_COMP_APPLICANT_INFO_MASTER.UUID == null) C_COMP_APPLICANT_INFO_MASTER.UUID = SessionUtil.DRAFT_NEXT_ID;
            C_COMP_APPLICANT_INFO_MASTER savingItem = draftMaster.Where(o => o.UUID == C_COMP_APPLICANT_INFO_MASTER.UUID).FirstOrDefault();
            if (savingItem != null) draftMaster.Remove(savingItem);
            C_COMP_APPLICANT_INFO_MASTER.TypeAApprove = model.SelectedTypeApprove0;
            C_COMP_APPLICANT_INFO_MASTER.TypeBApprove = model.SelectedTypeApprove1;
            C_COMP_APPLICANT_INFO_MASTER.TypeCApprove = model.SelectedTypeApprove2;
            C_COMP_APPLICANT_INFO_MASTER.TypeDApprove = model.SelectedTypeApprove3;
            C_COMP_APPLICANT_INFO_MASTER.TypeEApprove = model.SelectedTypeApprove4;
            C_COMP_APPLICANT_INFO_MASTER.TypeFApprove = model.SelectedTypeApprove5;
            C_COMP_APPLICANT_INFO_MASTER.TypeGApprove = model.SelectedTypeApprove6;

            C_COMP_APPLICANT_INFO_MASTER.TypeAApply = model.SelectedTypeApply0;
            C_COMP_APPLICANT_INFO_MASTER.TypeBApply = model.SelectedTypeApply1;
            C_COMP_APPLICANT_INFO_MASTER.TypeCApply = model.SelectedTypeApply2;
            C_COMP_APPLICANT_INFO_MASTER.TypeDApply = model.SelectedTypeApply3;
            C_COMP_APPLICANT_INFO_MASTER.TypeEApply = model.SelectedTypeApply4;
            C_COMP_APPLICANT_INFO_MASTER.TypeFApply = model.SelectedTypeApply5;
            C_COMP_APPLICANT_INFO_MASTER.TypeGApply = model.SelectedTypeApply6;

            C_COMP_APPLICANT_INFO_MASTER.APPLICATION_DATE = model.C_COMP_APPLICANT_INFO_MASTER.APPLICATION_DATE;
            C_COMP_APPLICANT_INFO_MASTER.APPLICATION_FORM_ID = model.C_COMP_APPLICANT_INFO_MASTER.APPLICATION_FORM_ID;
            //C_COMP_APPLICANT_INFO_MASTER.APPLICATION_DATE = model.AppDate;
            //C_COMP_APPLICANT_INFO_MASTER.APPLICATION_FORM_ID = model.AppFormId;
            C_COMP_APPLICANT_INFO_MASTER.STATUS_CODE = model.AppStatus;
            C_COMP_APPLICANT_INFO_MASTER.EFORM = model.EFORM;
            draftMaster.Add(C_COMP_APPLICANT_INFO_MASTER);
            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }
        public ServiceResult DeleteAppHist(string C_COMP_APPLICANT_INFO_MASTER_UUID)
        {
            //List<C_COMP_APPLICANT_INFO> draftApplicant = SessionUtil.DraftList<C_COMP_APPLICANT_INFO>(ApplicationConstant.DRAFT_KEY_APPLICANTS);
            List<C_COMP_APPLICANT_INFO_MASTER> draftMaster = SessionUtil.DraftList<C_COMP_APPLICANT_INFO_MASTER>(ApplicationConstant.DRAFT_KEY_APPLICANT_MASTERS);
            int rseult = draftMaster.RemoveAll(o => C_COMP_APPLICANT_INFO_MASTER_UUID != null && o.UUID == C_COMP_APPLICANT_INFO_MASTER_UUID);
            if (rseult == 0) SessionUtil.DraftList<string>(ApplicationConstant.DELETE_KEY_APPLICANT_MASTERS).Add(C_COMP_APPLICANT_INFO_MASTER_UUID);
            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }


        public List<C_COMP_APPLICANT_INFO> GetApplicantsByCompUuid(string compUuid, string applicantUuid, bool hideNullAcceptDate)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                List<C_COMP_APPLICANT_INFO> r = (compUuid != null || applicantUuid != null)
                    ? db.C_COMP_APPLICANT_INFO
                    .Where(o => o.MASTER_ID == compUuid || compUuid == null)
                    .Where(o => o.C_APPLICANT.UUID == applicantUuid || applicantUuid == null)
                    .Where(o => o.ACCEPT_DATE != null || !hideNullAcceptDate)
                    .Include(o => o.C_APPLICANT)
                    .Include(o => o.C_S_SYSTEM_VALUE)
                    .Include(o => o.C_S_SYSTEM_VALUE1)
                    .Include(o => o.C_APPLICANT.C_S_SYSTEM_VALUE).ToList()
                    : new List<C_COMP_APPLICANT_INFO>();
                for (int i = 0; i < r.Count; i++) r[i].C_APPLICANT.Decrypt();
                return r;
            }
        }
        public List<ApplicantDisplayListModel> AjaxMergedApplicantList(string compUuid)
        {
            List<C_COMP_APPLICANT_INFO> draftApplicant = SessionUtil.DraftList<C_COMP_APPLICANT_INFO>(ApplicationConstant.DRAFT_KEY_APPLICANTS);
            List<string> deleteApplicant = SessionUtil.DraftList<string>(ApplicationConstant.DELETE_KEY_APPLICANTS);
            List<C_COMP_APPLICANT_INFO> dbList = GetApplicantsByCompUuid(compUuid, null, false);
            int dummy;
            List<C_COMP_APPLICANT_INFO> createList = draftApplicant.Where(o => int.TryParse(o.UUID, out dummy)).ToList();
            List<C_COMP_APPLICANT_INFO> editList = draftApplicant.Where(o => !int.TryParse(o.UUID, out dummy)).ToList();
            editList.ForEach(x => x.refNo = "");
            return ApplicantDisplayListModel.Load(dbList
                .Where(o => !deleteApplicant.Contains(o.UUID))
                .Concat(createList)
                .Select(o => o.MergeByList(editList)).ToList());
        }


        public DisplayGrid AjaxMergedAppHistList(string applicantUuid)
        {
            string Query = ""
            + "\r\n\t" + "SELECT T1.CREATED_DATE , T1.APPLICATION_DATE                                           "
            + "\r\n\t" + ", T1.COMPANY_APPLICANTS_ID AS C_COMP_APPLICANT_INFO_UUID               "
            + "\r\n\t" + ", T1.UUID                                                                                 "
            + "\r\n\t" + ", (SELECT CODE FROM C_S_SYSTEM_VALUE WHERE UUID = T1.APPLICATION_FORM_ID) AS APPLICATION_FORM_ID                  "
            + "\r\n\t" + ", (SELECT LISTAGG(REPLACE(CODE,'Type ',''), ', ') WITHIN GROUP (ORDER BY CODE) FROM (                             "
            + "\r\n\t" + "	(SELECT J1.CODE AS CODE FROM C_S_SYSTEM_VALUE J1 WHERE EXISTS (                                                 "
            + "\r\n\t" + "		SELECT 1 FROM C_COMP_APPLICANT_INFO_DETAIL J2                                                               "
            + "\r\n\t" + "		WHERE EXISTS (SELECT 1 FROM C_S_SYSTEM_VALUE J3 WHERE J3.CODE = 'Class 1' AND J2.ITEM_CLASS_ID = J3.UUID)   "
            + "\r\n\t" + "		AND J2.COMPANY_APPLICANTS_MASTER_ID = T1.UUID AND J2.ITEM_TYPE_ID = J1.UUID AND J2.STATUS_CODE = 'APPLY'    "
            + "\r\n\t" + "	))                                                                                                              "
            + "\r\n\t" + ")) AS APPLY_I                                                                                                     "
            + "\r\n\t" + ", (SELECT LISTAGG(REPLACE(CODE,'Type',''), ', ') WITHIN GROUP (ORDER BY CODE) FROM (                              "
            + "\r\n\t" + "	(SELECT J1.CODE AS CODE FROM C_S_SYSTEM_VALUE J1 WHERE EXISTS (                                                 "
            + "\r\n\t" + "		SELECT 1 FROM C_COMP_APPLICANT_INFO_DETAIL J2                                                               "
            + "\r\n\t" + "		WHERE EXISTS (SELECT 1 FROM C_S_SYSTEM_VALUE J3 WHERE J3.CODE = 'Class 2' AND J2.ITEM_CLASS_ID = J3.UUID)   "
            + "\r\n\t" + "		AND J2.COMPANY_APPLICANTS_MASTER_ID = T1.UUID AND J2.ITEM_TYPE_ID = J1.UUID AND J2.STATUS_CODE = 'APPLY'    "
            + "\r\n\t" + "	))                                                                                                              "
            + "\r\n\t" + "	MINUS (SELECT J1.CODE AS CODE FROM C_S_SYSTEM_VALUE J1 WHERE EXISTS (                                           "
            + "\r\n\t" + "		SELECT 1 FROM C_COMP_APPLICANT_INFO_DETAIL J2                                                               "
            + "\r\n\t" + "		WHERE EXISTS (SELECT 1 FROM C_S_SYSTEM_VALUE J3 WHERE J3.CODE = 'Class 1' AND J2.ITEM_CLASS_ID = J3.UUID)   "
            + "\r\n\t" + "		AND J2.COMPANY_APPLICANTS_MASTER_ID = T1.UUID AND J2.ITEM_TYPE_ID = J1.UUID AND J2.STATUS_CODE = 'APPLY'    "
            + "\r\n\t" + "	))                                                                                                              "
            + "\r\n\t" + ")) AS APPLY_II                                                                                                    "
            + "\r\n\t" + ", (SELECT LISTAGG(REPLACE(CODE,'Type',''), ', ') WITHIN GROUP (ORDER BY CODE) FROM (                              "
            + "\r\n\t" + "	(SELECT J1.CODE AS CODE FROM C_S_SYSTEM_VALUE J1 WHERE EXISTS (                                                 "
            + "\r\n\t" + "		SELECT 1 FROM C_COMP_APPLICANT_INFO_DETAIL J2                                                               "
            + "\r\n\t" + "		WHERE EXISTS (SELECT 1 FROM C_S_SYSTEM_VALUE J3 WHERE J3.CODE = 'Class 3' AND J2.ITEM_CLASS_ID = J3.UUID)   "
            + "\r\n\t" + "		AND J2.COMPANY_APPLICANTS_MASTER_ID = T1.UUID AND J2.ITEM_TYPE_ID = J1.UUID AND J2.STATUS_CODE = 'APPLY'    "
            + "\r\n\t" + "	))                                                                                                              "
            + "\r\n\t" + "	MINUS (SELECT J1.CODE AS CODE FROM C_S_SYSTEM_VALUE J1 WHERE EXISTS (                                           "
            + "\r\n\t" + "		SELECT 1 FROM C_COMP_APPLICANT_INFO_DETAIL J2                                                               "
            + "\r\n\t" + "		WHERE EXISTS (SELECT 1 FROM C_S_SYSTEM_VALUE J3 WHERE J3.CODE = 'Class 2' AND J2.ITEM_CLASS_ID = J3.UUID)   "
            + "\r\n\t" + "		AND J2.COMPANY_APPLICANTS_MASTER_ID = T1.UUID AND J2.ITEM_TYPE_ID = J1.UUID AND J2.STATUS_CODE = 'APPLY'    "
            + "\r\n\t" + "	))                                                                                                              "
            + "\r\n\t" + "	MINUS (SELECT J1.CODE AS CODE FROM C_S_SYSTEM_VALUE J1 WHERE EXISTS (                                           "
            + "\r\n\t" + "		SELECT 1 FROM C_COMP_APPLICANT_INFO_DETAIL J2                                                               "
            + "\r\n\t" + "		WHERE EXISTS (SELECT 1 FROM C_S_SYSTEM_VALUE J3 WHERE J3.CODE = 'Class 1' AND J2.ITEM_CLASS_ID = J3.UUID)   "
            + "\r\n\t" + "		AND J2.COMPANY_APPLICANTS_MASTER_ID = T1.UUID AND J2.ITEM_TYPE_ID = J1.UUID AND J2.STATUS_CODE = 'APPLY'    "
            + "\r\n\t" + "	))                                                                                                              "
            + "\r\n\t" + ")) AS APPLY_III                                                                                                   "
            + "\r\n\t" + ", T1.APPROVED_BY                                                                                                  "
            + "\r\n\t" + ", T1.APPROVED_DATE                                                                                                "
            + "\r\n\t" + ", (SELECT LISTAGG(REPLACE(CODE,'Type ',''), ', ') WITHIN GROUP (ORDER BY CODE) FROM (                             "
            + "\r\n\t" + "	(SELECT J1.CODE AS CODE FROM C_S_SYSTEM_VALUE J1 WHERE EXISTS (                                                 "
            + "\r\n\t" + "		SELECT 1 FROM C_COMP_APPLICANT_INFO_DETAIL J2                                                               "
            + "\r\n\t" + "		WHERE EXISTS (SELECT 1 FROM C_S_SYSTEM_VALUE J3 WHERE J3.CODE = 'Class 1' AND J2.ITEM_CLASS_ID = J3.UUID)   "
            + "\r\n\t" + "		AND J2.COMPANY_APPLICANTS_MASTER_ID = T1.UUID AND J2.ITEM_TYPE_ID = J1.UUID AND J2.STATUS_CODE = 'APPROVED' "
            + "\r\n\t" + "	))                                                                                                              "
            + "\r\n\t" + ")) AS APPROVED_I                                                                                                  "
            + "\r\n\t" + ", (SELECT LISTAGG(REPLACE(CODE,'Type',''), ', ') WITHIN GROUP (ORDER BY CODE) FROM (                              "
            + "\r\n\t" + "	(SELECT J1.CODE AS CODE FROM C_S_SYSTEM_VALUE J1 WHERE EXISTS (                                                 "
            + "\r\n\t" + "		SELECT 1 FROM C_COMP_APPLICANT_INFO_DETAIL J2                                                               "
            + "\r\n\t" + "		WHERE EXISTS (SELECT 1 FROM C_S_SYSTEM_VALUE J3 WHERE J3.CODE = 'Class 2' AND J2.ITEM_CLASS_ID = J3.UUID)   "
            + "\r\n\t" + "		AND J2.COMPANY_APPLICANTS_MASTER_ID = T1.UUID AND J2.ITEM_TYPE_ID = J1.UUID AND J2.STATUS_CODE = 'APPROVED' "
            + "\r\n\t" + "	)) MINUS                                                                                                        "
            + "\r\n\t" + "	(SELECT J1.CODE AS CODE FROM C_S_SYSTEM_VALUE J1 WHERE EXISTS (                                                 "
            + "\r\n\t" + "		SELECT 1 FROM C_COMP_APPLICANT_INFO_DETAIL J2                                                               "
            + "\r\n\t" + "		WHERE EXISTS (SELECT 1 FROM C_S_SYSTEM_VALUE J3 WHERE J3.CODE = 'Class 1' AND J2.ITEM_CLASS_ID = J3.UUID)   "
            + "\r\n\t" + "		AND J2.COMPANY_APPLICANTS_MASTER_ID = T1.UUID AND J2.ITEM_TYPE_ID = J1.UUID AND J2.STATUS_CODE = 'APPROVED' "
            + "\r\n\t" + "	))                                                                                                              "
            + "\r\n\t" + ")) AS APPROVED_II                                                                                                 "
            + "\r\n\t" + ", (SELECT LISTAGG(REPLACE(CODE,'Type',''), ', ') WITHIN GROUP (ORDER BY CODE) FROM (                              "
            + "\r\n\t" + "	(SELECT J1.CODE AS CODE FROM C_S_SYSTEM_VALUE J1 WHERE EXISTS (                                                 "
            + "\r\n\t" + "		SELECT 1 FROM C_COMP_APPLICANT_INFO_DETAIL J2                                                               "
            + "\r\n\t" + "		WHERE EXISTS (SELECT 1 FROM C_S_SYSTEM_VALUE J3 WHERE J3.CODE = 'Class 3' AND J2.ITEM_CLASS_ID = J3.UUID)   "
            + "\r\n\t" + "		AND J2.COMPANY_APPLICANTS_MASTER_ID = T1.UUID AND J2.ITEM_TYPE_ID = J1.UUID AND J2.STATUS_CODE = 'APPROVED' "
            + "\r\n\t" + "	))                                                                                                              "
            + "\r\n\t" + "	MINUS (SELECT J1.CODE AS CODE FROM C_S_SYSTEM_VALUE J1 WHERE EXISTS (                                           "
            + "\r\n\t" + "		SELECT 1 FROM C_COMP_APPLICANT_INFO_DETAIL J2                                                               "
            + "\r\n\t" + "		WHERE EXISTS (SELECT 1 FROM C_S_SYSTEM_VALUE J3 WHERE J3.CODE = 'Class 2' AND J2.ITEM_CLASS_ID = J3.UUID)   "
            + "\r\n\t" + "		AND J2.COMPANY_APPLICANTS_MASTER_ID = T1.UUID AND J2.ITEM_TYPE_ID = J1.UUID AND J2.STATUS_CODE = 'APPROVED' "
            + "\r\n\t" + "	))                                                                                                              "
            + "\r\n\t" + "	MINUS (SELECT J1.CODE AS CODE FROM C_S_SYSTEM_VALUE J1 WHERE EXISTS (                                           "
            + "\r\n\t" + "		SELECT 1 FROM C_COMP_APPLICANT_INFO_DETAIL J2                                                               "
            + "\r\n\t" + "		WHERE EXISTS (SELECT 1 FROM C_S_SYSTEM_VALUE J3 WHERE J3.CODE = 'Class 1' AND J2.ITEM_CLASS_ID = J3.UUID)   "
            + "\r\n\t" + "		AND J2.COMPANY_APPLICANTS_MASTER_ID = T1.UUID AND J2.ITEM_TYPE_ID = J1.UUID AND J2.STATUS_CODE = 'APPROVED' "
            + "\r\n\t" + "	))                                                                                                              "
            + "\r\n\t" + ")) AS APPROVED_III                                                                                                "
            + "\r\n\t" + ", CASE WHEN T1.STATUS_CODE = 'Confirmed' THEN 'Approved' ELSE T1.STATUS_CODE END   AS STATUS_CODE                                                                                              "
            + "\r\n\t" + "FROM C_COMP_APPLICANT_INFO_MASTER T1                                                                              ";



            DisplayGrid grid = new DisplayGrid { };
            // if (applicantUuid != null)
            if (!string.IsNullOrEmpty(applicantUuid))
            {
                Query += "\r\n\t" + "WHERE T1.COMPANY_APPLICANTS_ID =  :applicantUuid                                              ";
                grid.QueryParameters.Add("applicantUuid", applicantUuid);//''

            }
            else
            {
                Query += "\r\n\t" + "WHERE 1=2                                             ";
            }
            grid.Query = Query;
            grid.Search();




            List<C_COMP_APPLICANT_INFO_MASTER> draftMaster = SessionUtil.DraftList<C_COMP_APPLICANT_INFO_MASTER>(ApplicationConstant.DRAFT_KEY_APPLICANT_MASTERS);
            List<string> deleteMaster = SessionUtil.DraftList<string>(ApplicationConstant.DELETE_KEY_APPLICANT_MASTERS);

            int dummy;
            List<C_COMP_APPLICANT_INFO_MASTER> createList = draftMaster.Where(o => int.TryParse(o.UUID, out dummy) && o.C_COMP_APPLICANT_INFO?.UUID == applicantUuid).ToList();
            List<C_COMP_APPLICANT_INFO_MASTER> editList = draftMaster.Where(o => !int.TryParse(o.UUID, out dummy)).ToList();
            grid.Data =
                grid.Data.Where(o => o.Where(r => r.Key == "UUID" && deleteMaster.Contains(r.Value)).ToList().Count == 0).ToList();

            for (int i = 0; i < createList.Count; i++)
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                //row.Add("C_COMP_APPLICANT_INFO_UUID", createList[i]);
                row.Add("APPLICATION_DATE", createList[i].APPLICATION_DATE);
                row.Add("APPLICATION_FORM_ID", SystemListUtil.GetSVByUUID(createList[i].APPLICATION_FORM_ID).CODE);
                row.Add("UUID", createList[i].UUID);
                row.Add("APPLY_I", createList[i].APPLY_I);
                row.Add("APPLY_II", createList[i].APPLY_II);
                row.Add("APPLY_III", createList[i].APPLY_III);
                row.Add("APPROVED_BY", createList[i].APPROVED_BY);
                row.Add("APPROVED_DATE", createList[i].APPROVED_DATE);
                row.Add("APPROVED_I", createList[i].APPROVE_I);
                row.Add("APPROVED_II", createList[i].APPROVE_II);
                row.Add("APPROVED_III", createList[i].APPROVE_III);
                row.Add("STATUS_CODE", createList[i].STATUS_CODE);
                row.Add("EFORM", createList[i].EFORM);
                grid.Data.Add(row);
            }
            for (int i = 0; i < editList.Count; i++)
            {
                Dictionary<string, object> row =
                    grid.Data
                    .Where(o => o
                        .Where(r => r.Key == "UUID" && ((string)r.Value).Equals(editList[i].UUID))
                        .ToList().Count > 0)
                    .FirstOrDefault();

                if (row != null)
                {
                    //Dictionary<string, object> row = new Dictionary<string, object>();
                    //row["C_COMP_APPLICANT_INFO_UUID"], createList[i]);
                    row["APPLICATION_DATE"] = editList[i].APPLICATION_DATE;
                    row["UUID"] = editList[i].UUID;
                    row["APPLY_I"] = editList[i].APPLY_I;
                    row["APPLY_II"] = editList[i].APPLY_II;
                    row["APPLY_III"] = editList[i].APPLY_III;
                    row["APPROVED_BY"] = editList[i].APPROVED_BY;
                    row["APPROVED_DATE"] = editList[i].APPROVED_DATE;
                    row["APPROVED_I"] = editList[i].APPROVE_I;
                    row["APPROVED_II"] = editList[i].APPROVE_II;
                    row["APPROVED_III"] = editList[i].APPROVE_III;
                    row["STATUS_CODE"] = editList[i].STATUS_CODE;
                    row["EFORM"] = editList[i].EFORM;
                }
            }
            grid.Total = grid.Data.Count;
            //grid.AppSort("APPLICATION_DATE");
            return grid;
        }


        public DisplayGrid AjaxMWCapability(string applicantUuid, string masterUuid)
        {
            DisplayGrid grid;


            string q = ""
                + "\r\n\t" + "SELECT BASE.ITEM_TYPE, BASE.ITEM_DESCRIPTION                                                                       "
                + "\r\n\t" + ", CASE WHEN CLASSLIST_APPLY.ITEM_CLASS = '1, 2, 3' THEN 'Y' ELSE 'N' END AS CLS_I_APPLY                                  "
                + "\r\n\t" + ", CASE WHEN CLASSLIST_APPLY.ITEM_CLASS = '2, 3' THEN 'Y' ELSE 'N' END AS CLS_II_APPLY                                    "
                + "\r\n\t" + ", CASE WHEN CLASSLIST_APPLY.ITEM_CLASS = '3' THEN 'Y' ELSE 'N' END AS CLS_III_APPLY                                      "
                + "\r\n\t" + "							                                                                                          "
                + "\r\n\t" + ", CASE WHEN CLASSLIST_APPROVED.ITEM_CLASS = '1, 2, 3' THEN 'Y' ELSE 'N' END AS CLS_I_APPROVE                               "
                + "\r\n\t" + ", CASE WHEN CLASSLIST_APPROVED.ITEM_CLASS = '2, 3' THEN 'Y' ELSE 'N' END AS CLS_II_APPROVE                                 "
                + "\r\n\t" + ", CASE WHEN CLASSLIST_APPROVED.ITEM_CLASS = '3' THEN 'Y' ELSE 'N' END AS CLS_III_APPROVE                                   "
                + "\r\n\t" + "FROM (                                                                                                             "
                + "\r\n\t" + "	SELECT DISTINCT                                                                                                   "
                + "\r\n\t" + "	T1.CODE AS ITEM_TYPE, T1.ENGLISH_DESCRIPTION AS ITEM_DESCRIPTION                                                  "
                + "\r\n\t" + "	FROM C_S_SYSTEM_VALUE T1, C_S_SYSTEM_TYPE T2                                                                      "
                + "\r\n\t" + "	WHERE T1.SYSTEM_TYPE_ID = T2.UUID AND T2.TYPE = 'MINOR_WORKS_TYPE'                                                "
                + "\r\n\t" + "	ORDER BY 1                                                                                                        "
                + "\r\n\t" + ") BASE                                                                                                             "
                + "\r\n\t" + "LEFT JOIN                                                                                                          "
                + "\r\n\t" + "(                                                                                                                  "
                + "\r\n\t" + "	SELECT ITEM_CLASS, ITEM_TYPE                                                                                      "
                + "\r\n\t" + "	FROM (                                                                                                            "
                + "\r\n\t" + "		SELECT                                                                                                        "
                + "\r\n\t" + "			LISTAGG(REPLACE(ITEM_CLASS,'Class ',''), ', ') WITHIN GROUP (ORDER BY ITEM_CLASS) AS ITEM_CLASS, ITEM_TYPE"
                + "\r\n\t" + "		FROM (                                                                                                        "
                + "\r\n\t" + "			SELECT                                                                                                    "
                + "\r\n\t" + "			DISTINCT T3.CODE AS ITEM_CLASS, T4.CODE  AS ITEM_TYPE                                                     "
                + "\r\n\t" + "			FROM C_COMP_APPLICANT_INFO_MASTER T1                                                                      "
                + "\r\n\t" + "			INNER JOIN C_COMP_APPLICANT_INFO_DETAIL T2 ON T1.UUID = T2.COMPANY_APPLICANTS_MASTER_ID                   "
                + "\r\n\t" + "			INNER JOIN C_S_SYSTEM_VALUE T3 ON T3.UUID = T2.ITEM_CLASS_ID                                              "
                + "\r\n\t" + "			INNER JOIN C_S_SYSTEM_VALUE T4 ON T4.UUID = T2.ITEM_TYPE_ID                                               ";

            if (!string.IsNullOrWhiteSpace(applicantUuid)) q += "\r\n\t" + "			WHERE T1.COMPANY_APPLICANTS_ID = :applicantUuid               ";
            else if (!string.IsNullOrWhiteSpace(masterUuid)) q += "\r\n\t" + "			WHERE T1.UUID = :masterUuid                                   ";
              //else q += "\r\n\t" + "WHERE 1=2";
            q = q
                + "\r\n\t" + "			AND T2.STATUS_CODE  = 'APPLY'                                                                             "
                + "\r\n\t" + "		)                                                                                                             "
                + "\r\n\t" + "		GROUP BY ITEM_TYPE                                                                                            "
                + "\r\n\t" + "	)                                                                                                                 "
                + "\r\n\t" + ") CLASSLIST_APPLY                                                                                                  "
                + "\r\n\t" + "ON BASE.ITEM_TYPE = CLASSLIST_APPLY.ITEM_TYPE                                                                      "
                + "\r\n\t" + "																							                          "
                + "\r\n\t" + "LEFT JOIN                                                                                                          "
                + "\r\n\t" + "(                                                                                                                  "
                + "\r\n\t" + "	SELECT ITEM_CLASS, ITEM_TYPE                                                                                      "
                + "\r\n\t" + "	FROM (                                                                                                            "
                + "\r\n\t" + "		SELECT                                                                                                        "
                + "\r\n\t" + "			LISTAGG(REPLACE(ITEM_CLASS,'Class ',''), ', ') WITHIN GROUP (ORDER BY ITEM_CLASS) AS ITEM_CLASS, ITEM_TYPE"
                + "\r\n\t" + "		FROM (                                                                                                        "
                + "\r\n\t" + "			SELECT                                                                                                    "
                + "\r\n\t" + "			DISTINCT T3.CODE AS ITEM_CLASS, T4.CODE  AS ITEM_TYPE                                                     "
                + "\r\n\t" + "			FROM C_COMP_APPLICANT_INFO_MASTER T1                                                                      "
                + "\r\n\t" + "			INNER JOIN C_COMP_APPLICANT_INFO_DETAIL T2 ON T1.UUID = T2.COMPANY_APPLICANTS_MASTER_ID                   "
                + "\r\n\t" + "			INNER JOIN C_S_SYSTEM_VALUE T3 ON T3.UUID = T2.ITEM_CLASS_ID                                              "
                + "\r\n\t" + "			INNER JOIN C_S_SYSTEM_VALUE T4 ON T4.UUID = T2.ITEM_TYPE_ID                                               ";
            if (!string.IsNullOrWhiteSpace(applicantUuid)) q += "\r\n\t" + "			WHERE T1.COMPANY_APPLICANTS_ID = :applicantUuid               ";
            else if (!string.IsNullOrWhiteSpace(masterUuid)) q += "\r\n\t" + "			WHERE T1.UUID = :masterUuid                                   ";
            //    else q += "\r\n\t" + "WHERE 1=2";
            q = q
                + "\r\n\t" + "			AND T2.STATUS_CODE  = 'APPROVED'                                                                          "
                + "\r\n\t" + "		)                                                                                                             "
                + "\r\n\t" + "		GROUP BY ITEM_TYPE                                                                                            "
                + "\r\n\t" + "	)                                                                                                                 "
                + "\r\n\t" + ") CLASSLIST_APPROVED                                                                                               "
                + "\r\n\t" + "ON BASE.ITEM_TYPE = CLASSLIST_APPROVED.ITEM_TYPE                                                                   ";






            grid = new DisplayGrid { Query = q };
            if (!string.IsNullOrWhiteSpace(applicantUuid)) grid.QueryParameters.Add("applicantUuid", applicantUuid);
            else if (!string.IsNullOrWhiteSpace(masterUuid)) grid.QueryParameters.Add("masterUuid", masterUuid);
            grid.Search();
            if (string.IsNullOrWhiteSpace(applicantUuid))
            {
                foreach (var item in grid.Data)
                {
                    item["CLS_I_APPROVE"] = "N";
                }
              
            }

            List<C_COMP_APPLICANT_INFO_MASTER> draftMaster = SessionUtil.DraftList<C_COMP_APPLICANT_INFO_MASTER>(ApplicationConstant.DRAFT_KEY_APPLICANT_MASTERS);
            C_COMP_APPLICANT_INFO_MASTER savingItem = draftMaster.Where(o => o.UUID == masterUuid).FirstOrDefault();
            if (savingItem != null)
            {
                grid.Data[0]["CLS_I_APPLY"] = savingItem.TypeAApply != null && savingItem.TypeAApply.Equals("1") ? "Y" : "N";
                grid.Data[0]["CLS_II_APPLY"] = savingItem.TypeAApply != null && savingItem.TypeAApply.Equals("2") ? "Y" : "N";
                grid.Data[0]["CLS_III_APPLY"] = savingItem.TypeAApply != null && savingItem.TypeAApply.Equals("3") ? "Y" : "N";
                grid.Data[1]["CLS_I_APPLY"] = savingItem.TypeBApply != null && savingItem.TypeBApply.Equals("1") ? "Y" : "N";
                grid.Data[1]["CLS_II_APPLY"] = savingItem.TypeBApply != null && savingItem.TypeBApply.Equals("2") ? "Y" : "N";
                grid.Data[1]["CLS_III_APPLY"] = savingItem.TypeBApply != null && savingItem.TypeBApply.Equals("3") ? "Y" : "N";
                grid.Data[2]["CLS_I_APPLY"] = savingItem.TypeCApply != null && savingItem.TypeCApply.Equals("1") ? "Y" : "N";
                grid.Data[2]["CLS_II_APPLY"] = savingItem.TypeCApply != null && savingItem.TypeCApply.Equals("2") ? "Y" : "N";
                grid.Data[2]["CLS_III_APPLY"] = savingItem.TypeCApply != null && savingItem.TypeCApply.Equals("3") ? "Y" : "N";
                grid.Data[3]["CLS_I_APPLY"] = savingItem.TypeDApply != null && savingItem.TypeDApply.Equals("1") ? "Y" : "N";
                grid.Data[3]["CLS_II_APPLY"] = savingItem.TypeDApply != null && savingItem.TypeDApply.Equals("2") ? "Y" : "N";
                grid.Data[3]["CLS_III_APPLY"] = savingItem.TypeDApply != null && savingItem.TypeDApply.Equals("3") ? "Y" : "N";
                grid.Data[4]["CLS_I_APPLY"] = savingItem.TypeEApply != null && savingItem.TypeEApply.Equals("1") ? "Y" : "N";
                grid.Data[4]["CLS_II_APPLY"] = savingItem.TypeEApply != null && savingItem.TypeEApply.Equals("2") ? "Y" : "N";
                grid.Data[4]["CLS_III_APPLY"] = savingItem.TypeEApply != null && savingItem.TypeEApply.Equals("3") ? "Y" : "N";
                grid.Data[5]["CLS_I_APPLY"] = savingItem.TypeFApply != null && savingItem.TypeFApply.Equals("1") ? "Y" : "N";
                grid.Data[5]["CLS_II_APPLY"] = savingItem.TypeFApply != null && savingItem.TypeFApply.Equals("2") ? "Y" : "N";
                grid.Data[5]["CLS_III_APPLY"] = savingItem.TypeFApply != null && savingItem.TypeFApply.Equals("3") ? "Y" : "N";
                grid.Data[6]["CLS_I_APPLY"] = savingItem.TypeGApply != null && savingItem.TypeGApply.Equals("1") ? "Y" : "N";
                grid.Data[6]["CLS_II_APPLY"] = savingItem.TypeGApply != null && savingItem.TypeGApply.Equals("2") ? "Y" : "N";
                grid.Data[6]["CLS_III_APPLY"] = savingItem.TypeGApply != null && savingItem.TypeGApply.Equals("3") ? "Y" : "N";


                grid.Data[0]["CLS_I_APPROVE"] = savingItem.TypeAApprove != null && savingItem.TypeAApprove.Equals("1") ? "Y" : "N";
                grid.Data[0]["CLS_II_APPROVE"] = savingItem.TypeAApprove != null && savingItem.TypeAApprove.Equals("2") ? "Y" : "N";
                grid.Data[0]["CLS_III_APPROVE"] = savingItem.TypeAApprove != null && savingItem.TypeAApprove.Equals("3") ? "Y" : "N";
                grid.Data[1]["CLS_I_APPROVE"] = savingItem.TypeBApprove != null && savingItem.TypeBApprove.Equals("1") ? "Y" : "N";
                grid.Data[1]["CLS_II_APPROVE"] = savingItem.TypeBApprove != null && savingItem.TypeBApprove.Equals("2") ? "Y" : "N";
                grid.Data[1]["CLS_III_APPROVE"] = savingItem.TypeBApprove != null && savingItem.TypeBApprove.Equals("3") ? "Y" : "N";
                grid.Data[2]["CLS_I_APPROVE"] = savingItem.TypeCApprove != null && savingItem.TypeCApprove.Equals("1") ? "Y" : "N";
                grid.Data[2]["CLS_II_APPROVE"] = savingItem.TypeCApprove != null && savingItem.TypeCApprove.Equals("2") ? "Y" : "N";
                grid.Data[2]["CLS_III_APPROVE"] = savingItem.TypeCApprove != null && savingItem.TypeCApprove.Equals("3") ? "Y" : "N";
                grid.Data[3]["CLS_I_APPROVE"] = savingItem.TypeDApprove != null && savingItem.TypeDApprove.Equals("1") ? "Y" : "N";
                grid.Data[3]["CLS_II_APPROVE"] = savingItem.TypeDApprove != null && savingItem.TypeDApprove.Equals("2") ? "Y" : "N";
                grid.Data[3]["CLS_III_APPROVE"] = savingItem.TypeDApprove != null && savingItem.TypeDApprove.Equals("3") ? "Y" : "N";
                grid.Data[4]["CLS_I_APPROVE"] = savingItem.TypeEApprove != null && savingItem.TypeEApprove.Equals("1") ? "Y" : "N";
                grid.Data[4]["CLS_II_APPROVE"] = savingItem.TypeEApprove != null && savingItem.TypeEApprove.Equals("2") ? "Y" : "N";
                grid.Data[4]["CLS_III_APPROVE"] = savingItem.TypeEApprove != null && savingItem.TypeEApprove.Equals("3") ? "Y" : "N";
                grid.Data[5]["CLS_I_APPROVE"] = savingItem.TypeFApprove != null && savingItem.TypeFApprove.Equals("1") ? "Y" : "N";
                grid.Data[5]["CLS_II_APPROVE"] = savingItem.TypeFApprove != null && savingItem.TypeFApprove.Equals("2") ? "Y" : "N";
                grid.Data[5]["CLS_III_APPROVE"] = savingItem.TypeFApprove != null && savingItem.TypeFApprove.Equals("3") ? "Y" : "N";
                grid.Data[6]["CLS_I_APPROVE"] = savingItem.TypeGApprove != null && savingItem.TypeGApprove.Equals("1") ? "Y" : "N";
                grid.Data[6]["CLS_II_APPROVE"] = savingItem.TypeGApprove != null && savingItem.TypeGApprove.Equals("2") ? "Y" : "N";
                grid.Data[6]["CLS_III_APPROVE"] = savingItem.TypeGApprove != null && savingItem.TypeGApprove.Equals("3") ? "Y" : "N";

            }
            return grid;
        }

        public string AjaxProcDueDate(DateTime? applicationDate)
        {
            if (applicationDate == null) return null;
            DateTime r = applicationDate.Value.AddDays(90);

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                DisplayGrid displayGrid = new DisplayGrid()
                {
                    Query = "SELECT C_GET_IND_PROC_DUEDATE(:sDate) AS DUE_DATE FROM DUAL",
                    QueryParameters = new Dictionary<string, object>() { { "sdate", r } }
                };
                displayGrid.Search();

                return displayGrid.Data[0]["DUE_DATE_DATE_DISPLAY"] as string;
            }
        }
        public ServiceResult SaveMWSISStatus(Fn01Search_QPSearchModel model)
        {

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {

                    foreach (var item in model.PanelMWIS)
                    {
                        var a = (from comp_app in db.C_COMP_APPLICATION
                                 where comp_app.FILE_REFERENCE_NO == item.Key
                                 select comp_app).FirstOrDefault();
                        //var a = db.C_COMP_APPLICATION.Where(item.Key);
                        if (a != null)
                        {
                            a.SERVICE_IN_MWIS = item.Value;
                        }
                        else
                        {
                            //var b =db.C_IND_APPLICATION.Find(item.Key);
                            var b = (from ind_app in db.C_IND_APPLICATION
                                     where ind_app.FILE_REFERENCE_NO == item.Key
                                     select ind_app).FirstOrDefault();
                            b.SERVICE_IN_MWIS = item.Value;
                        }
                    }
                    db.SaveChanges();
                    transaction.Commit();
                    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
                }
            }
        }

        public List<List<object>> getEformData(string registrationType, string fileRefSuffix, string tableName, string editMode)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();

            string queryReferenceNo = "(select CODE || ' ' || :fileRefSuffix from C_S_CATEGORY_CODE where UUID =:registrationType )";
            if (string.IsNullOrWhiteSpace(fileRefSuffix))
            {
                queryReferenceNo = "(select CODE from C_S_CATEGORY_CODE where UUID =:registrationType )";

                if (string.IsNullOrWhiteSpace(registrationType))
                {
                    queryReferenceNo = "";
                }
            }
            if (string.IsNullOrWhiteSpace(registrationType))
            {
                queryReferenceNo = ":fileRefSuffix ";
                if (string.IsNullOrWhiteSpace(fileRefSuffix))
                {
                    queryReferenceNo = "";
                }
            }

            string Validform = "";
            string BA24query = ")";
            string sqlCompQuery = "";

            // Generator Contractor Application
            if (tableName.Equals("C_EFSS_COMPANY"))
            {
                if (editMode.Equals("edit"))
                {
                    Validform = "and (FORMTYPE like '%BA%%2A' or FORMTYPE like '%BA%%2B' or FORMTYPE like '%BA%%2C' or FORMTYPE like '%BA%%24' or FORMTYPE like '%BA%2' or FORMTYPE like '%BA%2') ";
                    BA24query = "UNION " +
                    "select ba.ID, ba.FORMTYPE,null,null,null, ba.FILEREF, ba.ADDRESS_LINE1," +
                    "ba.ADDRESS_LINE2,ba.ADDRESS_LINE3,ba.ADDRESS_LINE4,ba.ADDRESS_LINE5," +
                    "ba.C_ADDRESS_LINE1," +
                    "ba.C_ADDRESS_LINE2,ba.C_ADDRESS_LINE3,ba.C_ADDRESS_LINE4,ba.C_ADDRESS_LINE5," +
                    "ba.TELNO1, ba.FAXNO1, ba.EMAIL,null,null,null," +
                    "ba.LASTUPDDATE,ba.CREATEDATE, ba.APPLICATIONDATE, ba.TELNO2,ba.TELNO3, ba.FAXNO2, ba.EFSS_NO, ba.STATUS from C_EFSS_BA24 ba)";
                }
                else if (editMode.Equals("create"))
                {
                    Validform = "and FORMTYPE like '%BA%2'";

                }

                sqlCompQuery = " select * from( " +
                " select ID,FORMTYPE,CHNCOMPNAME,ENGCOMPNAME,FILEREFTYPE,FILEREF,ADDRESS_LINE1," +
                " ADDRESS_LINE2,ADDRESS_LINE3,ADDRESS_LINE4,ADDRESS_LINE5," +
                " C_ADDRESS_LINE1,C_ADDRESS_LINE2,C_ADDRESS_LINE3,C_ADDRESS_LINE4,C_ADDRESS_LINE5," +
                " TELNO,FAXNO," +
                " EMAIL,BRNO,COMTYPE," +
                " APPNAME,LASTUPDDATE,CREATEDATE,APPLICATIONDATE, null as TELNO2, null as TELNO3, null as FAXNO2, EFSS_NO, STATUS " +
                " from C_EFSS_COMPANY " +
                BA24query +
                " where FILEREF = " + queryReferenceNo +
                " " + Validform +
                " order by LASTUPDDATE desc";
            }

            // Professional Application
            else if (tableName.Equals("C_EFSS_PROFESSIONAL"))
            {
                if (editMode.Equals("edit"))
                {
                    Validform = "and (FORMTYPE like '%BA%%1A' or FORMTYPE like '%BA%%1B' or FORMTYPE like '%BA%%1C' or FORMTYPE like '%BA%%24' or FORMTYPE like '%BA%%1') ";
                    BA24query = " UNION ALL" +
                     " select ba.ID, ba.FORMTYPE,null,ba.FILEREF ," +
                     " null, ba.ADDRESS_LINE1,ba.ADDRESS_LINE2,ba.ADDRESS_LINE3,ba.ADDRESS_LINE4,ba.ADDRESS_LINE5," +
                     " ba.C_ADDRESS_LINE1,ba.C_ADDRESS_LINE2,ba.C_ADDRESS_LINE3,ba.C_ADDRESS_LINE4,ba.C_ADDRESS_LINE5," +
                     " ba.FAXNO1,ba.EMAIL,null,null,null,null,null,null,null,null,null,null," +
                     " ba.TELNO1,ba.TELNO2, null,null,utl_raw.cast_to_varchar2(DBMS_LOB.SUBSTR(EMPTY_BLOB())) ,null,null," +
                     " null,null,null,null," +
                     " ba.CREATEDATE,ba.LASTUPDDATE, ba.APPLICATIONDATE,ba.TELNO3,ba.FAXNO2, ba.EFSS_NO, ba.STATUS from C_EFSS_BA24 ba) ";

                }
                else if (editMode.Equals("create"))
                {
                    Validform = "and FORMTYPE like '%BA%%1'";

                }
                sqlCompQuery = "select * " +
                 "from( " +
                 "select ID,FORMTYPE,CATCODE," +
                 "  FILEREF,CERTNO," +
                 "  ADDRESS_LINE1,ADDRESS_LINE2,ADDRESS_LINE3,ADDRESS_LINE4," +
                 "  ADDRESS_LINE5," +
                 "  C_ADDRESS_LINE1,C_ADDRESS_LINE2,C_ADDRESS_LINE3,C_ADDRESS_LINE4," +
                 "  C_ADDRESS_LINE5," +
                 "  FAXNO,EMAIL,OFFICE_ADDRESS_LINE1,OFFICE_ADDRESS_LINE2," +
                 "  OFFICE_ADDRESS_LINE3,OFFICE_ADDRESS_LINE4,OFFICE_ADDRESS_LINE5," +
                 "  C_OFFICE_ADDRESS_LINE1,C_OFFICE_ADDRESS_LINE2," +
                 "  C_OFFICE_ADDRESS_LINE3,C_OFFICE_ADDRESS_LINE4,C_OFFICE_ADDRESS_LINE5," +

                 "  TELNO1," +
                 "  TELNO2,EMERGNO1,EMERGNO2,utl_raw.cast_to_varchar2(DBMS_LOB.SUBSTR(SPECSIGN)) as SPECSIGN,BSCODE1,BSCODE2,BSCODE3," +
                 "  BSCODE4,ISINTERESTQP,LANG,CREATEDATE,LASTUPDDATE, APPLICATIONDATE,null as TELNO3, null as FAXNO2, EFSS_NO, STATUS" +
                 " from C_EFSS_PROFESSIONAL " +
                 BA24query +
                 " where FILEREF = " + queryReferenceNo +
                 " " + Validform +
                 " order by LASTUPDDATE desc";
            }

            // MW Company Application
            else if (tableName.Equals("C_EFSS_MWC"))
            {
                if (editMode.Equals("edit"))
                {
                    Validform = "and (FORMTYPE like '%BA%%25A' or FORMTYPE like '%BA%%25B' or FORMTYPE like '%BA%%25C' or FORMTYPE like '%BA%%24' or FORMTYPE like '%BA%%25' or FORMTYPE like '%BA%%25D') ";
                    BA24query = " UNION " +
                    " select ba.ID, ba.FORMTYPE,ba.FILEREF,null,null,null, null,null, ba.ADDRESS_LINE1," +
                    " ba.ADDRESS_LINE2,ba.ADDRESS_LINE3,ba.ADDRESS_LINE4,ba.ADDRESS_LINE5," +
                    " ba.C_ADDRESS_LINE1,ba.C_ADDRESS_LINE2,ba.C_ADDRESS_LINE3,ba.C_ADDRESS_LINE4,ba.C_ADDRESS_LINE5," +
                    " ba.TELNO1 , ba.TELNO2,ba.TELNO3, ba.FAXNO1,ba.FAXNO2 ,ba.EMAIL,ba.LASTUPDDATE, ba.CREATEDATE, ba.APPLICATIONDATE, ba.EFSS_NO, ba.STATUS From C_EFSS_BA24 ba) ";
                }
                else if (editMode.Equals("create"))
                {
                    Validform = "and FORMTYPE like '%BA%%25'";

                }
                sqlCompQuery = "select *" +
                 "from( " +
                "select ID,FORMTYPE,FILEREF,BRNO,ENGCOMNAME,CHNCOMNAME,COMTYPE,APPNAME," +
                        "E_ADDRESS_LINE1,E_ADDRESS_LINE2,E_ADDRESS_LINE3,E_ADDRESS_LINE4," +
                        "E_ADDRESS_LINE5,C_ADDRESS_LINE1,C_ADDRESS_LINE2,C_ADDRESS_LINE3," +
                        "C_ADDRESS_LINE4,C_ADDRESS_LINE5,TELNO1,TELNO2,TELNO3,FAXNO1,FAXNO2," +
                        "EMAIL," +
                       " LASTUPDDATE,CREATEDATE,APPLICATIONDATE, EFSS_NO, STATUS" +
                " from C_EFSS_MWC " +
                 BA24query +
                " where FILEREF = " + queryReferenceNo +
                " " + Validform +
                " order by LASTUPDDATE desc";
            }

            // MW Individual Application
            else if (tableName.Equals("C_EFSS_MWCW"))
            {
                if (editMode.Equals("edit"))
                {
                    Validform = "and (FORMTYPE like '%BA%%26A' or FORMTYPE like '%BA%%26B' or FORMTYPE like '%BA%%26C'  or FORMTYPE like '%BA%%26D' or FORMTYPE like '%BA%%24' or FORMTYPE like '%BA%%26') ";
                    BA24query = "  UNION " +
                    "  select ba.ID, ba.FORMTYPE,ba.FILEREF," +
                    "  ba.ADDRESS_LINE1,ba.ADDRESS_LINE2,ba.ADDRESS_LINE3,ba.ADDRESS_LINE4,ba.ADDRESS_LINE5," +
                    "  ba.C_ADDRESS_LINE1,ba.C_ADDRESS_LINE2,ba.C_ADDRESS_LINE3,ba.C_ADDRESS_LINE4,ba.C_ADDRESS_LINE5," +
                    "  null,null,null,null,null,null,null,null,null,null," +
                    "  ba.TELNO1 , ba.TELNO2,ba.TELNO3, ba.FAXNO1,ba.FAXNO2 ,ba.EMAIL,ba.LASTUPDDATE, ba.CREATEDATE, ba.APPLICATIONDATE, ba.EFSS_NO, STATUS " +
                    "  From C_EFSS_BA24 ba)";
                }
                else if (editMode.Equals("create"))
                {
                    Validform = "and FORMTYPE like '%BA%%26'";


                }
                sqlCompQuery = " select *" +
                    " from( " +
                    " select ID,FORMTYPE,FILEREF,CORR_E_ADDRESS_LINE1," +
                    "  CORR_E_ADDRESS_LINE2,CORR_E_ADDRESS_LINE3,CORR_E_ADDRESS_LINE4," +
                    "  CORR_E_ADDRESS_LINE5,CORR_C_ADDRESS_LINE1,CORR_C_ADDRESS_LINE2," +
                    "  CORR_C_ADDRESS_LINE3,CORR_C_ADDRESS_LINE4,CORR_C_ADDRESS_LINE5," +
                    "  COMP_E_ADDRESS_LINE1,COMP_E_ADDRESS_LINE2,COMP_E_ADDRESS_LINE3," +
                    "  COMP_E_ADDRESS_LINE4,COMP_E_ADDRESS_LINE5,COMP_C_ADDRESS_LINE1," +
                    "  COMP_C_ADDRESS_LINE2,COMP_C_ADDRESS_LINE3,COMP_C_ADDRESS_LINE4," +
                    "  COMP_C_ADDRESS_LINE5,TELNO1,TELNO2,TELNO3," +
                    "  FAXNO1,FAXNO2,EMAIL,LASTUPDDATE,CREATEDATE,APPLICATIONDATE, EFSS_NO, STATUS " +
                    "  from C_EFSS_MWCW " +
                     BA24query +
                    " where FILEREF = " + queryReferenceNo +
                    " " + Validform +
                    "  order by LASTUPDDATE desc";
            }


            if (!string.IsNullOrWhiteSpace(registrationType))
            {
                queryParameters.Add("registrationType", registrationType);
            }
            if (!string.IsNullOrWhiteSpace(fileRefSuffix))
            {
                queryParameters.Add("fileRefSuffix", fileRefSuffix);
            }

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, sqlCompQuery, queryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return data;
        }

        public List<C_EFSS_APPLICANT> getEFSS_ApplicantData(string formId, string formType)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var result = db.C_EFSS_APPLICANT.Where(x => x.FORM_ID == formId).Where(x => x.FORMTYPE == formType).ToList();
                return result;
            }
        }

        public List<List<object>> getEFSS_PROFESSIONALData(string uuid, string refNo, string fileType, string editMode)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();

            string queryString = "";
            string BA24query = ")";
            string Validform = "";

            if (editMode.Equals("edit"))
            {
                if (fileType.Equals("BA24"))
                {
                    Validform = "FORMTYPE like '%BA%%24' ";
                    BA24query = "UNION " +

                    " select ba.ID, ba.FORMTYPE,null, ba.FILEREF," +
                    " null, ba.ADDRESS_LINE1,ba.ADDRESS_LINE2,ba.ADDRESS_LINE3,ba.ADDRESS_LINE4,ba.ADDRESS_LINE5," +
                    " ba.C_ADDRESS_LINE1,ba.C_ADDRESS_LINE2,ba.C_ADDRESS_LINE3,ba.C_ADDRESS_LINE4,ba.C_ADDRESS_LINE5," +
                    " ba.FAXNO1,ba.EMAIL,null,null,null,null,null,null,null,null,null,null,ba.TELNO1,ba.TELNO2, null,null,utl_raw.cast_to_varchar2(DBMS_LOB.SUBSTR(EMPTY_BLOB())) ,null,null," +
                    " null ,null,null,null," +
                    " ba.CREATEDATE,ba.LASTUPDDATE, ba.APPLICATIONDATE,ba.TELNO3,ba.FAXNO2, ba.EFSS_NO, ba.STATUS from C_EFSS_BA24 ba)";
                }
                else
                {
                    Validform = " (FORMTYPE like '%BA%%1A%' or FORMTYPE like '%BA%%1B' or FORMTYPE like '%BA%%1' )";
                }
            }
            else
            {
                Validform = "FORMTYPE like '%BA%%1'";
            }

            queryString += "select * from (select ID,FORMTYPE,CATCODE," +
            " FILEREF,CERTNO,ADDRESS_LINE1,ADDRESS_LINE2,ADDRESS_LINE3,ADDRESS_LINE4," +
            " ADDRESS_LINE5," +
            " C_ADDRESS_LINE1,C_ADDRESS_LINE2,C_ADDRESS_LINE3,C_ADDRESS_LINE4," +
            " C_ADDRESS_LINE5," +
            " FAXNO,EMAIL," +
            " OFFICE_ADDRESS_LINE1,OFFICE_ADDRESS_LINE2," +
            " OFFICE_ADDRESS_LINE3,OFFICE_ADDRESS_LINE4,OFFICE_ADDRESS_LINE5," +
            " C_OFFICE_ADDRESS_LINE1,C_OFFICE_ADDRESS_LINE2," +
            " C_OFFICE_ADDRESS_LINE3,C_OFFICE_ADDRESS_LINE4,C_OFFICE_ADDRESS_LINE5," +
            " TELNO1," +
            " TELNO2,EMERGNO1,EMERGNO2,utl_raw.cast_to_varchar2(DBMS_LOB.SUBSTR(SPECSIGN))as SPECSIGN,BSCODE1,BSCODE2,BSCODE3," +
            " BSCODE4,ISINTERESTQP,LANG,CREATEDATE,LASTUPDDATE,APPLICATIONDATE,null as TELNO3, null as FAXNO2, EFSS_NO, STATUS" +
            " from C_EFSS_PROFESSIONAL  " + BA24query +
            " where " + Validform + " and ID = :id and FILEREF = :referenceNo";

            queryParameters.Add("id", uuid);
            queryParameters.Add("referenceNo", refNo);

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryString, queryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }

            return data;
        }

        public List<List<object>> getEFSS_COMPANYData(string uuid, string refNo, string fileType, string editMode)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();

            string queryString = "";
            string BA24query = ")";
            string Validform = "";

            if (editMode.Equals("edit"))
            {
                if (fileType.Equals("BA24"))
                {
                    Validform = "FORMTYPE like '%BA%%24' ";
                    BA24query = "UNION " +

                    " select ba.ID, ba.FORMTYPE,null,null,null, ba.FILEREF, ba.ADDRESS_LINE1," +
                    " ba.ADDRESS_LINE2,ba.ADDRESS_LINE3,ba.ADDRESS_LINE4,ba.ADDRESS_LINE5," +
                    " ba.C_ADDRESS_LINE1," +
                    " ba.C_ADDRESS_LINE2,ba.C_ADDRESS_LINE3,ba.C_ADDRESS_LINE4,ba.C_ADDRESS_LINE5," +
                    " ba.TELNO1, ba.FAXNO1, ba.EMAIL,null,null,null," +
                    " ba.LASTUPDDATE,ba.CREATEDATE, ba.APPLICATIONDATE, ba.TELNO2,ba.TELNO3,ba.FAXNO2, ba.EFSS_NO, ba.STATUS from C_EFSS_BA24 ba)";
                }
                else
                {
                    Validform = " (FORMTYPE like '%BA%%2A%' or FORMTYPE like '%BA%%2B' or FORMTYPE like '%BA%%2C' or FORMTYPE like '%BA%%2') ";
                }
            }
            else
            {
                Validform = "FORMTYPE like '%BA%%2'";
            }

            queryString += "select * from (select ID,FORMTYPE,CHNCOMPNAME,ENGCOMPNAME,FILEREFTYPE,FILEREF,ADDRESS_LINE1," +
            " ADDRESS_LINE2,ADDRESS_LINE3,ADDRESS_LINE4,ADDRESS_LINE5," +
            " C_ADDRESS_LINE1," +
            " C_ADDRESS_LINE2,C_ADDRESS_LINE3,C_ADDRESS_LINE4,C_ADDRESS_LINE5, " +
            " TELNO,FAXNO," +
            " EMAIL,BRNO,COMTYPE," +
            " APPNAME,LASTUPDDATE,CREATEDATE,APPLICATIONDATE,null as TELNO2,null as TELNO3,null as FAXNO2 , EFSS_NO, STATUS" +
            " from C_EFSS_COMPANY  " + BA24query + " where " + Validform + " and ID = :id and FILEREF = :referenceNo";

            queryParameters.Add("id", uuid);
            queryParameters.Add("referenceNo", refNo);

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryString, queryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }

            return data;
        }
        public List<List<object>> getEFSS_MWCData(string uuid, string refNo, string fileType, string editMode)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();

            string queryString = "";
            string BA24query = ")";
            string Validform = "";

            if (editMode.Equals("edit"))
            {
                if (fileType.Equals("BA24"))
                {
                    Validform = "FORMTYPE like '%BA%%24' ";
                    BA24query = "UNION " +

                    " select ba.ID, ba.FORMTYPE,ba.FILEREF,null,null,null, null,null," +
                    " ba.ADDRESS_LINE1," +
                    " ba.ADDRESS_LINE2,ba.ADDRESS_LINE3,ba.ADDRESS_LINE4,ba.ADDRESS_LINE5," +
                    " ba.C_ADDRESS_LINE1," +
                    " ba.C_ADDRESS_LINE2,ba.C_ADDRESS_LINE3,ba.C_ADDRESS_LINE4,ba.C_ADDRESS_LINE5," +
                    " ba.TELNO1 , ba.TELNO2,ba.TELNO3, ba.FAXNO1,ba.FAXNO2 ,ba.EMAIL,ba.LASTUPDDATE,ba.CREATEDATE, " +
                    " ba.APPLICATIONDATE, ba.EFSS_NO, ba.STATUS From C_EFSS_BA24 ba)";
                }
                else
                {
                    Validform = " (FORMTYPE like '%BA%%25A%' or FORMTYPE like '%BA%%25B' or FORMTYPE like '%BA%%25C' or FORMTYPE like '%BA%%25' or FORMTYPE like '%BA%%25D') ";
                }
            }
            else
            {
                Validform = "(FORMTYPE like '%BA%%25' )";
            }

            queryString += "select * " +
            "from( 	" +
            "select ID,FORMTYPE,FILEREF,BRNO,ENGCOMNAME,CHNCOMNAME,COMTYPE,APPNAME," +
            "E_ADDRESS_LINE1,E_ADDRESS_LINE2,E_ADDRESS_LINE3,E_ADDRESS_LINE4," +
            "E_ADDRESS_LINE5,C_ADDRESS_LINE1,C_ADDRESS_LINE2,C_ADDRESS_LINE3," +
            "C_ADDRESS_LINE4,C_ADDRESS_LINE5,TELNO1,TELNO2,TELNO3,FAXNO1,FAXNO2," +
            " EMAIL," +
            " LASTUPDDATE, CREATEDATE, APPLICATIONDATE, EFSS_NO, STATUS" +
            " from C_EFSS_MWC  " + BA24query + " where " + Validform + " and ID = :id and FILEREF = :referenceNo";

            queryParameters.Add("id", uuid);
            queryParameters.Add("referenceNo", refNo);

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryString, queryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }

            return data;
        }

        public List<List<object>> getEFSS_MWCWData(string uuid, string refNo, string fileType, string editMode)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();

            string queryString = "";
            string BA24query = ")";
            string Validform = "";

            if (editMode.Equals("edit"))
            {
                if (fileType.Equals("BA24"))
                {
                    Validform = "FORMTYPE like '%BA%%24' ";
                    BA24query = "UNION " +
                    " select ba.ID, ba.FORMTYPE,ba.FILEREF," +
                    " ba.ADDRESS_LINE1,ba.ADDRESS_LINE2,ba.ADDRESS_LINE3,ba.ADDRESS_LINE4,ba.ADDRESS_LINE5," +
                    " ba.C_ADDRESS_LINE1,ba.C_ADDRESS_LINE2,ba.C_ADDRESS_LINE3,ba.C_ADDRESS_LINE4,ba.C_ADDRESS_LINE5," +
                    " null,null,null,null,null,null,null,null,null,null," +
                    " ba.TELNO1 , ba.TELNO2,ba.TELNO3, ba.FAXNO1,ba.FAXNO2 ,ba.EMAIL,ba.LASTUPDDATE, ba.CREATEDATE," +
                    " ba.APPLICATIONDATE, ba.EFSS_NO, ba.STATUS " +
                    " From C_EFSS_BA24 ba)";
                }
                else
                {
                    Validform = " (FORMTYPE like '%BA%%26A' or FORMTYPE like '%BA%%26B' or FORMTYPE like '%BA%%26C' or FORMTYPE like '%BA%%26D' or FORMTYPE like '%BA%%26')";
                }
            }
            else
            {
                Validform = "(FORMTYPE like '%BA%%26' )";
            }

            queryString += "select * " +
            " from( 	" +
            " select ID,FORMTYPE,FILEREF,CORR_E_ADDRESS_LINE1," +
            " CORR_E_ADDRESS_LINE2,CORR_E_ADDRESS_LINE3,CORR_E_ADDRESS_LINE4," +
            " CORR_E_ADDRESS_LINE5,CORR_C_ADDRESS_LINE1,CORR_C_ADDRESS_LINE2," +
            " CORR_C_ADDRESS_LINE3,CORR_C_ADDRESS_LINE4,CORR_C_ADDRESS_LINE5," +
            " COMP_E_ADDRESS_LINE1,COMP_E_ADDRESS_LINE2,COMP_E_ADDRESS_LINE3," +
            " COMP_E_ADDRESS_LINE4,COMP_E_ADDRESS_LINE5,COMP_C_ADDRESS_LINE1," +
            " COMP_C_ADDRESS_LINE2,COMP_C_ADDRESS_LINE3,COMP_C_ADDRESS_LINE4," +
            " COMP_C_ADDRESS_LINE5,TELNO1,TELNO2,TELNO3," +
            " FAXNO1,FAXNO2,EMAIL,LASTUPDDATE,CREATEDATE,APPLICATIONDATE, EFSS_NO, STATUS" +
            " from C_EFSS_MWCW  " + BA24query + " where " + Validform + " and ID = :id and FILEREF = :referenceNo";

            queryParameters.Add("id", uuid);
            queryParameters.Add("referenceNo", refNo);

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryString, queryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }

            return data;
        }


        public void GenerateSerialNo(string id, ref string serialNo, ref string dt)
        {
            serialNo = RegistrationConstant.QP_PREFIX_B + SystemListUtil.GetSVListByType(RegistrationConstant.QPCARD + RegistrationConstant.QP_PREFIX_B).FirstOrDefault().ORDERING.Value.ToString("000000");

            SystemListUtil.AddSerialNoOrdering(RegistrationConstant.QPCARD + RegistrationConstant.QP_PREFIX_B);
            RegistrationCommonService rs = new RegistrationCommonService();

            dt = rs.getCurrentCompExpiryDateComp(id);


        }






    }
}