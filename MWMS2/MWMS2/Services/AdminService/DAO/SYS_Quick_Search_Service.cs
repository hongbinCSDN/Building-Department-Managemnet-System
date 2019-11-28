using MWMS2.Areas.CMN.Models;
using MWMS2.Areas.Registration.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MWMS2.Services.AdminService.DAO
{
    public class SYS_Quick_Search_Service
    {
         public void RegistrationQuickSearch(CMN02Model model)
        {

            string q = ""
            + "\r\n\t" + " SELECT DISTINCT                                                                               "
            + "\r\n\t" + " CASE                                                                                          "
            + "\r\n\t" + " 	WHEN COMP_IND_TYPE = 'I' THEN 1                                                              "
            + "\r\n\t" + " 	WHEN COMP_IND_TYPE = 'C' THEN 2                                                              "
            + "\r\n\t" + " 	ELSE 3                                                                                       "
            + "\r\n\t" + " END AS ORDERING                                                                               "
            + "\r\n\t" + " , CASE                                                                                        "
            + "\r\n\t" + " 	WHEN COMP_IND_TYPE = 'I' THEN TRIM(TRIM(APPLICANT_SURNAME)||' '||TRIM(APPLICANT_GIVEN_NAME)) "
            + "\r\n\t" + " 	WHEN COMP_IND_TYPE = 'C' THEN COMP_NAME                                                      "
            + "\r\n\t" + " 	ELSE '<INVALID COMP_IND_TYPE>'                                                               "
            + "\r\n\t" + " END AS NAME                                                                                   "
            + "\r\n\t" + " , EXPIRY_DATE                                                                                 "
            + "\r\n\t" + " ,    REGISTRATION_NO                                                                          "
            + "\r\n\t" + " , TRIM(TRIM(AS_SURNAME)||' '||TRIM(AS_GIVEN_NAME)) AS AS_NAME                                 "
            + "\r\n\t" + " ,    UKEY                                                                                     "
            + "\r\n\t" + " FROM SYS_QUICK_SEARCH                                                                         ";

            model.Query = q;

            string where = "WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(model.Search_EngName))
            {
                where = where + "\r\n\t" + " AND (UPPER(APPLICANT_SURNAME) LIKE :Search_EngName1  OR UPPER(APPLICANT_GIVEN_NAME) LIKE :Search_EngName2 or UPPER(APPLICANT_SURNAME ||  ' ' ||APPLICANT_GIVEN_NAME) like :Search_EngName3 )";
                model.QueryParameters.Add("Search_EngName1", "%" + model.Search_EngName.Trim().ToUpper() + "%");
                model.QueryParameters.Add("Search_EngName2", "%" + model.Search_EngName.Trim().ToUpper() + "%");
                model.QueryParameters.Add("Search_EngName3", "%" + model.Search_EngName.Trim().ToUpper() + "%");

            }
            if (!string.IsNullOrWhiteSpace(model.Search_ChiName))
            {
                where = where + "\r\n\t" + " AND APPLICANT_CHINESE_NAME LIKE :Search_ChiName";
                model.QueryParameters.Add("Search_ChiName", "%" + model.Search_ChiName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Search_RegistrationNo))
            {
                where = where + "\r\n\t" + " AND REGISTRATION_NO = :Search_RegistrationNo";
                model.QueryParameters.Add("Search_RegistrationNo", model.Search_RegistrationNo.Trim().ToUpper());
            }
            if (model.Search_ExpiryDateFrom!=null)
            {
                where = where + "\r\n\t" + " AND EXPIRY_DATE >= :Search_ExpiryDateFrom";
                model.QueryParameters.Add("Search_ExpiryDateFrom", model.Search_ExpiryDateFrom);
                
            }
            if (model.Search_ExpiryDateTo != null)
            {
                where = where + "\r\n\t" + " AND EXPIRY_DATE <= :Search_ExpiryDateTo";
                model.QueryParameters.Add("Search_ExpiryDateTo", model.Search_ExpiryDateTo);
            }
            if (!string.IsNullOrWhiteSpace(model.Search_EngCompName))
            {
                where = where + "\r\n\t" + " AND COMP_NAME LIKE :Search_EngCompName";
                model.QueryParameters.Add("Search_EngCompName", "%" + model.Search_EngCompName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Search_ChiCompName))
            {
                where = where + "\r\n\t" + " AND COMP_CHI_NAME LIKE :Search_ChiCompName";
                model.QueryParameters.Add("Search_ChiCompName", "%" + model.Search_ChiCompName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Search_ASEngName))
            {
                where = where + "\r\n\t" + " AND (AS_SURNAME LIKE :Search_ASEngName1 OR AS_GIVEN_NAME LIKE :Search_ASEngName2)";
                model.QueryParameters.Add("Search_ASEngName1", "%" + model.Search_ASEngName.Trim().ToUpper() + "%");
                model.QueryParameters.Add("Search_ASEngName2", "%" + model.Search_ASEngName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Search_ASChiName))
            {
                where = where + "\r\n\t" + " AND AS_CHI_NAME LIKE :Search_ASChiName";
                model.QueryParameters.Add("Search_ASChiName", "%" + model.Search_ASChiName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Search_ServicesBS))
            {
                where = where + "\r\n\t" + " AND BS LIKE :Search_ServicesBS";
                model.QueryParameters.Add("Search_ServicesBS", "%" + model.Search_ServicesBS.Trim() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Search_TelBS))
            {
                where = where + "\r\n\t" + " AND BS_TEL LIKE :Search_TelBS";
                model.QueryParameters.Add("Search_TelBS", "%" + model.Search_TelBS.Trim() + "%");
            }


            //model.Query = "SELECT DISTINCT KEYWORD, RECORD_UUID, RECORD_TYPE, REGISTRATION_NO, APPLICANT_SURNAME, APPLICANT_GIVEN_NAME, APPLICANT_CHINESE_NAME, COMP_NAME, COMP_CHI_NAME, AS_SURNAME, AS_GIVEN_NAME, AS_CHI_NAME, HKID, PASSPORT, UKEY, KEYWORD_TYPE, COMP_IND_TYPE, EXPIRY_DATE FROM SYS_QUICK_SEARCH WHERE 1=1 ";
            
if (model.Keyword != null)
            {
                where += "\r\n\t" + " AND (UPPER(KEYWORD) LIKE :Keyword OR KEYWORD = :EncKeyword)";
                model.QueryParameters.Add("Keyword", "%" + model.Keyword.Trim().ToUpper() + "%");
                model.QueryParameters.Add("EncKeyword", EncryptDecryptUtil.getEncrypt(model.Keyword.Trim().ToUpper()));
            }

            model.QueryWhere = where;




            model.Sort = "1,UKEY, 2";
          
            model.Search();
            model.Rpp = -1;
            model.QSList.AddRange(from qs in model.Data
                             select new QSTable()
                             {
                                 //RECORD_UUID = qs["RECORD_UUID"]?.ToString(),
                                /* RECORD_TYPE =qs["RECORD_TYPE"]?.ToString(),
                                 REGISTRATION_NO =qs["REGISTRATION_NO"]?.ToString(),
                                 APPLICANT_SURNAME =qs["APPLICANT_SURNAME"]?.ToString(),
                                 APPLICANT_GIVEN_NAME =qs["APPLICANT_GIVEN_NAME"]?.ToString(),
                                 APPLICANT_CHINESE_NAME =qs["APPLICANT_CHINESE_NAME"]?.ToString(),
                                 COMP_NAME =qs["COMP_NAME"]?.ToString(),
                                 COMP_CHI_NAME =qs["COMP_CHI_NAME"]?.ToString(),
                                 AS_SURNAME =qs["AS_SURNAME"]?.ToString(),
                                 AS_GIVEN_NAME =qs["AS_GIVEN_NAME"]?.ToString(),
                                 AS_CHI_NAME =qs["AS_CHI_NAME"]?.ToString(),
                                 HKID =qs["HKID"]?.ToString(),
                                 PASSPORT =qs["PASSPORT"]?.ToString(),
                                 UKEY =qs["UKEY"]?.ToString(),
                                 KEYWORD_TYPE =qs["KEYWORD_TYPE"]?.ToString(),
                                 COMP_IND_TYPE =qs["COMP_IND_TYPE"]?.ToString(),
                                 EXPIRY_DATE =qs["EXPIRY_DATE"]==null? (DateTime?)null : (DateTime)qs["EXPIRY_DATE"],


    */













                             }


                             );
            //using (EntitiesQS db = new EntitiesQS())
            //{

            //    //var ListOfUniKey = db.SYS_QUICK_SEARCH.Where(x => x.KEYWORD.ToLower().Contains(model.Keyword.ToLower())).Select(x => x.UKEY).Distinct().ToList();

            //    //var test = db.SYS_QUICK_SEARCH.GroupBy(x => x.UKEY).
            //    //                Where(x => x.FirstOrDefault().KEYWORD.ToLower().Contains(model.Keyword.ToLower())).
            //    //                Select(x => x.FirstOrDefault()).ToList();


            //   // List<SYS_QUICK_SEARCH> q = db.SYS_QUICK_SEARCH.Where(x => x.KEYWORD.ToLower() ==model.Keyword.ToLower()).ToList();

            //   // List<SYS_QUICK_SEARCH> qT = db.SYS_QUICK_SEARCH.Where(x => x.UUID == "2E48F79EBA6A46C8A188E97B69680F41").ToList();
            //   //var test = db.SYS_QUICK_SEARCH.SqlQuery("SELECT *" +
            //   //     " FROM MWMS2.SYS_QUICK_SEARCH" +
            //   //     " WHERE KEYWORD" +
            //   //     " = 'THERMCO ASIA LIMITED'" +
            //   //     "" +
            //   //     "" +
            //   //     "").ToList<SYS_QUICK_SEARCH>(); ;

            //    //model.QSTable.AddRange(q);






            //    //var query = db.SYS_QUICK_SEARCH.Where(x=>x.KEYWORD.ToLower().Contains(model.Keyword.ToLower())).GroupBy(x => x.UKEY).Select(x => x.FirstOrDefault()).ToList();
            //    //model.QSList.AddRange((
            //    //    from qs in db.SYS_QUICK_SEARCH
            //    //    where qs.KEYWORD.ToLower().Contains(model.Keyword.ToLower())
            //    //    group qs by qs.UKEY into grp
            //    //    select new QSTable()
            //    //    {
            //    //         RECORD_UUID =qs["RECORD_UUID,
            //    //         RECORD_TYPE =qs["RECORD_TYPE,
            //    //         REGISTRATION_NO =qs["REGISTRATION_NO,
            //    //         APPLICANT_SURNAME =qs["APPLICANT_SURNAME,
            //    //         APPLICANT_GIVEN_NAME =qs["APPLICANT_GIVEN_NAME,
            //    //         APPLICANT_CHINESE_NAME =qs["APPLICANT_CHINESE_NAME,
            //    //         COMP_NAME =qs["COMP_NAME,
            //    //         COMP_CHI_NAME =qs["COMP_CHI_NAME,
            //    //         AS_SURNAME =qs["AS_SURNAME,
            //    //         AS_GIVEN_NAME =qs["AS_GIVEN_NAME,
            //    //         AS_CHI_NAME =qs["AS_CHI_NAME,
            //    //         HKID =qs["HKID,
            //    //         PASSPORT =qs["PASSPORT,
            //    //         UKEY =qs["UKEY,
            //    //         KEYWORD_TYPE =qs["KEYWORD_TYPE,
            //    //         COMP_IND_TYPE =qs["COMP_IND_TYPE,
            //    //         EXPIRY_DATE =qs["EXPIRY_DATE,
            //    //    }
            //    //    ).OrderBy(x=>x.REGISTRATION_NO)
            //    //    );

            //    //model.QSList = db.SYS_QUICK_SEARCH.Where(x => x.KEYWORD.Contains(model.Keyword)).GroupBy(x=>x.UKEY).ToList();

            //}

        }

        private string ProcessingQuery = "SELECT * FROM SYS_QUICK_SEARCH_MW WHERE 1=1 ";
        public string ExportProcessing(CMN02Model model)
        {
            model.Query = ProcessingQuery;
            model.QueryWhere = "\r\n\t" + "AND UPPER(Keyword) LIKE :Keyword";
            model.QueryParameters.Add("Keyword", "%" + model.Keyword.Trim().ToUpper() + "%");

            return model.Export("ExportData");
        }
        public CMN02Model ProcessingQuickSearch(CMN02Model model)
        {
            model.Query = ProcessingQuery;
            string q = "";
            if (!string.IsNullOrWhiteSpace(model.Keyword))
            {
                string[] spls = model.Keyword.Split('/');

                if (spls.Length == 3)
                {
                    try
                    {
                        int v1, v2, v3;
                        int.TryParse(spls[0], out v1);
                        int.TryParse(spls[1], out v2);
                        int.TryParse(spls[2], out v3);
                        DateTime d = new DateTime(v3, v2, v1);

                        q = q + "\r\n\t" + "AND (UPPER(Keyword) LIKE :Keyword OR COMMENCEMENT_DATE LIKE :KeywordD1 OR COMPLETION_DATE LIKE :KeywordD2)";
                        model.QueryParameters.Add("KeywordD1", d);
                        model.QueryParameters.Add("KeywordD2", d);
                        model.QueryParameters.Add("Keyword", "%" + model.Keyword.Trim().ToUpper() + "%");

                    }
                    catch (Exception ex)
                    {
                        q = q + "\r\n\t" + "AND (UPPER(Keyword) LIKE :Keyword)";
                        model.QueryParameters.Add("Keyword", "%" + model.Keyword.Trim().ToUpper() + "%");

                    }
                }
            }
            if (q == "")
            {
                q = q + "\r\n\t" + "AND (UPPER(Keyword) LIKE :Keyword)";
                model.QueryParameters.Add("Keyword", "%" + model.Keyword.Trim().ToUpper() + "%");
            }
            model.QueryWhere = q;
            model.Search();
            return model;
            //model.ProcessingMWList.AddRange(from mw in model.Data
            //                                select new ProcessingMWTable()
            //                                {                                              
            //                                    KEYWORD = mw["KEYWORD"]?.ToString(),
            //                                    KEYWORD_TYPE = mw["KEYWORD_TYPE"]?.ToString(),
            //                                    SUBMISSION_NATURE = mw["SUBMISSION_NATURE"]?.ToString(),
            //                                    MW_DSN = mw["MW_DSN"]?.ToString(),
            //                                    FIRST_RECEIVED_DATE = mw["FIRST_RECEIVED_DATE"] == null ? (DateTime?)null : (DateTime)mw["FIRST_RECEIVED_DATE"],
            //                                    S_FORM_TYPE_CODE = mw["S_FORM_TYPE_CODE"]?.ToString(),
            //                                    REFERENCE_NO = mw["REFERENCE_NO"]?.ToString(),
            //                                    CERTIFICATION_NO_AP = mw["CERTIFICATION_NO_AP"]?.ToString(),
            //                                    CERTIFICATION_NO_PRC = mw["CERTIFICATION_NO_PRC"]?.ToString(),
            //                                    LOCATION_OF_MINOR_WORK = mw["LOCATION_OF_MINOR_WORK"]?.ToString(),
            //                                    STREE_NAME = mw["STREE_NAME"]?.ToString(),
            //                                    STREET_NO = mw["STREET_NO"]?.ToString(),
            //                                    BUILDING_NAME = mw["BUILDING_NAME"]?.ToString(),
            //                                    FLOOR = mw["FLOOR"]?.ToString(),
            //                                    FLAT = mw["FLAT"]?.ToString(),
            //                                    SITE_AUDIT_RELATED = mw["SITE_AUDIT_RELATED"]?.ToString(),
            //                                    PRE_SITE_AUDIT_RELATED = mw["PRE_SITE_AUDIT_RELATED"]?.ToString(),
            //                                    AUDIT_RELATED = mw["AUDIT_RELATED"]?.ToString(),
            //                                    VERIFICATION_SPO = mw["VERIFICATION_SPO"]?.ToString(),
            //                                    COMMENCEMENT_DATE = mw["COMMENCEMENT_DATE"] == null ? (DateTime?)null : (DateTime)mw["COMMENCEMENT_DATE"],
            //                                    COMPLETION_DATE = mw["COMPLETION_DATE"] == null ? (DateTime?)null : (DateTime)mw["COMPLETION_DATE"],
            //                                    NAME_ENGLISH_OWNER = mw["NAME_ENGLISH_OWNER"]?.ToString(),
            //                                    CONTACT_NO_OWNER = mw["CONTACT_NO_OWNER"]?.ToString(),
            //                                    NAME_ENGLISH_OI = mw["NAME_ENGLISH_OI"]?.ToString(),
            //                                    CONTACT_NO_OI = mw["CONTACT_NO_OI"]?.ToString(),
            //                                    FILEREF_FOUR_TWO = mw["FILEREF_FOUR_TWO"]?.ToString(),

            //                                }


            //               );

        }

        public RegistrationModel ViewRegistrationDetail(string RegNo)
        {
            RegistrationModel model = new RegistrationModel();
            string regType="";
            string telNo = "";
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (EntitiesQS qsDB = new EntitiesQS())
                {
                    SYS_QUICK_SEARCH rs = qsDB.SYS_QUICK_SEARCH.Where(o => o.REGISTRATION_NO == RegNo).FirstOrDefault();

                    string RECORD_UUID = rs.RECORD_UUID;

                    //var Compquery = db.C_COMP_APPLICATION.Where(x => x.UUID == uuid).FirstOrDefault();
                    if (rs.COMP_IND_TYPE == "I")
                    {
                        model.Type = "I";
                        var q = db.C_IND_APPLICATION.Where(x => x.UUID == RECORD_UUID).Include(x => x.C_APPLICANT).FirstOrDefault();
                        var Indquery = db.C_IND_CERTIFICATE.Where(x => x.MASTER_ID == RECORD_UUID && x.CERTIFICATION_NO == RegNo).FirstOrDefault();

                        regType = q.REGISTRATION_TYPE;
                        model.CERTIFICATION_NO = Indquery.CERTIFICATION_NO;
                        model.EXPIRY_DATE = Indquery.EXPIRY_DATE;


                        model.NAME = q.C_APPLICANT.SURNAME + " " + q.C_APPLICANT.GIVEN_NAME_ON_ID;
                        model.CHINESE_NAME = q.C_APPLICANT.CHINESE_NAME;

                        telNo = q.BS_TELEPHONE_NO1;


                    }
                    else
                    {
                        model.Type = "C";
                        C_COMP_APPLICATION comp = db.C_COMP_APPLICATION.Where(x => x.UUID == RECORD_UUID).FirstOrDefault();
                        model.CERTIFICATION_NO = comp.CERTIFICATION_NO;
                        model.EXPIRY_DATE = comp.EXPIRY_DATE;


                        model.NAME = comp.ENGLISH_COMPANY_NAME;
                        model.CHINESE_NAME = comp.CHINESE_COMPANY_NAME;

                        RegistrationCompAppService ss = new RegistrationCompAppService();
                        regType = comp.REGISTRATION_TYPE;

                        List<C_COMP_APPLICANT_INFO> ASQuery = ss.GetApplicantsByCompUuid(RECORD_UUID, null, false);
                        foreach (var item in ASQuery)
                        {
                            model.AS_NAME.Add(item.C_APPLICANT.SURNAME + " " + item.C_APPLICANT.GIVEN_NAME_ON_ID);
                            model.AS_CHI_NAME.Add(item.C_APPLICANT.CHINESE_NAME);
                        }
                        telNo = comp.BS_TELEPHONE_NO1;

                    }

                    BuildingSafetyCheckListModel newbsModel = new BuildingSafetyCheckListModel()
                    { MasterUuid = RECORD_UUID, RegType = regType };
                    newbsModel.init();

                    foreach (var item in newbsModel.BsItems)
                    {
                        model.BS_ITEM = item.Code + ",";
                    }
                    if(model.BS_ITEM !=null)
                    model.BS_ITEM = model.BS_ITEM.Substring(0, model.BS_ITEM.Length - 1);
                    model.TEL_NO = telNo;
                }
            }

            return model;

        }

    }
}