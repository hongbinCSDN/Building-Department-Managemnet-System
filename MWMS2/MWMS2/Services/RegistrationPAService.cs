using MWMS2.Areas.Registration.Models;
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
using MWMS2.Constant;
using System.IO;
using System.Web.Configuration;

namespace MWMS2.Services
{
    public class RegistrationPAService
    {

        //add on 29/4/2019
        private static string SearchPA_PM_q(Fn03PA_PMSearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.FileRef))
            {
                whereQ += "\r\n\t" + " AND (upper(IND_APP.FILE_REFERENCE_NO) LIKE :FileRef)";
                model.QueryParameters.Add("FileRef", "%" + model.FileRef.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.SurName))
            {
                whereQ += "\r\n\t" + " AND (upper(APP.SURNAME) LIKE :SurName)";
                model.QueryParameters.Add("SurName", "%" + model.SurName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.GivenName))
            {
                whereQ += "\r\n\t" + " AND (upper(APP.GIVEN_NAME_ON_ID) LIKE :GivenName)";
                model.QueryParameters.Add("GivenName", "%" + model.GivenName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.ChiName))
            {
                whereQ += "\r\n\t" + " AND (upper(APP.CHINESE_NAME) LIKE :ChiName)";
                model.QueryParameters.Add("ChiName", "%" + model.ChiName.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.HKID))
            {
                whereQ += "\r\n\t" + "AND " + EncryptDecryptUtil.getDecryptSQL("APP.hkid") + " LIKE :HKID";
                model.QueryParameters.Add("HKID", "%" + model.HKID.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.PassportNo))
            {
                whereQ += "\r\n\t" + "AND " + EncryptDecryptUtil.getDecryptSQL("APP.passport_no") + " LIKE :PassportNo";
                model.QueryParameters.Add("PassportNo", "%" + model.PassportNo.Trim().ToUpper() + "%");
            }
            return ""
                + "\r\n" + "\t" + " SELECT CASE WHEN A.PM_UUID is null THEN 'No' ELSE 'Yes' END as UM, "
                + "\r\n" + "\t" + " A.COMP_UUID as COMP_UUID, A.COMP_APPL_UUID as COMP_APPL_UUID, "
                + "\r\n" + "\t" + " A.RECORD_TYPE as RECORD_TYPE,  A.FILE_REFERENCE_NO as FILE_REFERENCE_NO,"
                + "\r\n" + "\t" + " concat(concat( A.SURNAME,' '), A.GIVEN_NAME_ON_ID) AS NAME, "
                + "\r\n" + "\t" + " A.CODE as CODE, A.APPLICATION_TYPE as APPLICATION_TYPE, "
                + "\r\n" + "\t" + " A.RECEIVED_DATE as RECEIVED_DATE, A.ENGLISH_COMPANY_NAME as ENGLISH_COMPANY_NAME "
                + "\r\n" + "\t" + " FROM  (  SELECT  '' AS PM_UUID,  COM.UUID AS COMP_UUID, "
                + "\r\n" + "\t" + " APPINFO.UUID AS COMP_APPL_UUID,  'NEW' AS RECORD_TYPE, "
                + "\r\n" + "\t" + " COM.FILE_REFERENCE_NO,   APP.SURNAME,   APP.GIVEN_NAME_ON_ID, "
                + "\r\n" + "\t" + " APP.CHINESE_NAME,   COM.ENGLISH_COMPANY_NAME, "
                + "\r\n" + "\t" + " V.CODE, V2.CODE AS APPLICATION_TYPE, NULL AS RECEIVED_DATE "
                + "\r\n" + "\t" + " FROM C_COMP_APPLICATION COM,  C_COMP_APPLICANT_INFO APPINFO, "
                + "\r\n" + "\t" + " C_APPLICANT APP, C_S_SYSTEM_VALUE V, C_S_SYSTEM_VALUE V2 "
                + "\r\n" + "\t" + " WHERE COM.UUID=APPINFO.MASTER_ID "
                + "\r\n" + "\t" + " AND COM.REGISTRATION_TYPE= 'IP' "
                + "\r\n" + "\t" + " AND APPINFO.APPLICANT_ID =APP.UUID "
                + "\r\n" + "\t" + " AND APPINFO.APPLICANT_ROLE_ID=V.UUID "
                + "\r\n" + "\t" + " AND COM.APPLICATION_FORM_ID=V2.UUID "
                + whereQ
                + "\r\n" + "\t" + " UNION ALL "
                + "\r\n" + "\t" + " SELECT  PRM.UUID AS PM_UUID,  PRM.MASTER_ID AS COMP_UUID, "
                + "\r\n" + "\t" + " PRM.COMPANY_APPLICANTS_ID AS COMP_APPL_UUID,  'EDIT', "
                + "\r\n" + "\t" + " COM.FILE_REFERENCE_NO,  APP.SURNAME,  APP.GIVEN_NAME_ON_ID, "
                + "\r\n" + "\t" + " APP.CHINESE_NAME,  COM.ENGLISH_COMPANY_NAME,  V.CODE, "
                + "\r\n" + "\t" + " V2.CODE AS APPLICATION_TYPE, PRM.RECEIVED_DATE  AS RECEIVED_DATE "
                + "\r\n" + "\t" + " FROM C_COMP_PROCESS_MONITOR PRM, C_COMP_APPLICATION COM, "
                + "\r\n" + "\t" + " C_COMP_APPLICANT_INFO APPINFO,  C_APPLICANT APP, C_S_SYSTEM_VALUE V, C_S_SYSTEM_VALUE V2 "
                + "\r\n" + "\t" + " WHERE  PRM.MONITOR_TYPE= 'UPM' AND COM.REGISTRATION_TYPE= 'IP' "
                + "\r\n" + "\t" + " AND PRM.COMPANY_APPLICANTS_ID = APPINFO.UUID  AND APPINFO.APPLICANT_ID =APP.UUID "
                + "\r\n" + "\t" + " AND APPINFO.APPLICANT_ROLE_ID=V.UUID  AND PRM.APPLICATION_FORM_ID=V2.UUID "
                + whereQ
                + "\r\n" + "\t" + " ) A "
                ;

        }


        // sql for search Professional Application    + "\r\n" + "\t" + EncryptDecryptUtil.getDecryptSQL("app.hkid") + " as hkid ,"
        string SearchPA_q = ""
                         + "\r\n" + "\t" + "select ind.*, app.SURNAME,app.GIVEN_NAME_ON_ID,"
                         + "\r\n" + "\t" + EncryptDecryptUtil.getDecryptSQL("app.hkid") + " as hkid ,"
                         + "\r\n" + "\t" + EncryptDecryptUtil.getDecryptSQL("app.passport_no") + " as passport_no ,"
                         + "\r\n" + "\t" + " (" + EncryptDecryptUtil.getDecryptSQL("app.hkid") + " || '/' || " + EncryptDecryptUtil.getDecryptSQL("app.passport_no") + " ) as HKIDPASSPORT"
                         + "\r\n" + "\t" + " ,(app.SURNAME || ' ' || app.GIVEN_NAME_ON_ID )as NAME,"
                         + "\r\n" + "\t" + " app.CHINESE_NAME "
                         + "\r\n" + "\t" + "from C_Ind_Application ind, C_Applicant app                      "
                         + "\r\n" + "\t" + "where ind.APPLICANT_ID = app.UUID and ind.registration_Type = '" + RegistrationConstant.REGISTRATION_TYPE_IP + "'";


        string SearchGCN_q = ""
                         + "\r\n" + "\t" + "select ind.*, app.SURNAME,app.GIVEN_NAME_ON_ID,"
                         + "\r\n" + "\t" + EncryptDecryptUtil.getDecryptSQL("app.hkid") + " as hkid ,"
                         + "\r\n" + "\t" + EncryptDecryptUtil.getDecryptSQL("app.passport_no") + " as passport_no ,"
                         + "\r\n" + "\t" + " (" + EncryptDecryptUtil.getDecryptSQL("app.hkid") + " || '/' || " + EncryptDecryptUtil.getDecryptSQL("app.passport_no") + " ) as HKIDPASSPORT"
                         + "\r\n" + "\t" + " ,(app.SURNAME || ' ' || app.GIVEN_NAME_ON_ID )as NAME,"
                         + "\r\n" + "\t" + " app.CHINESE_NAME "
                         + "\r\n" + "\t" + "from C_Ind_Application ind, C_Applicant app                      "
                         + "\r\n" + "\t" + "where ind.APPLICANT_ID = app.UUID and ind.registration_Type = 'IP' ";

        public Fn03PA_PASearchModel SearchPA(Fn03PA_PASearchModel model)
        {
            model.Query = SearchPA_q;

            model.QueryWhere = SearchPA_whereQ(model);

            model.Search();
            return model;
        }

        private string SearchPA_whereQ(Fn03PA_PASearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.FileRef))
            {
                whereQ += "\r\n\t" + "AND upper(ind.FILE_REFERENCE_NO) LIKE :FileRef";
                model.QueryParameters.Add("FileRef", "%" + model.FileRef.Trim().ToUpper() + "%");
            }

            if (!string.IsNullOrWhiteSpace(model.HKID))
            {
                whereQ += "\r\n\t" + "AND " + EncryptDecryptUtil.getDecryptSQL("app.hkid") + " LIKE :HKID";
                model.QueryParameters.Add("HKID", "%" + model.HKID.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.PassportNo))
            {
                whereQ += "\r\n\t" + "AND " + EncryptDecryptUtil.getDecryptSQL("app.passport_no") + " LIKE :PassportNo";
                model.QueryParameters.Add("PassportNo", "%" + model.PassportNo.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.SurnName))
            {
                whereQ += "\r\n\t" + "AND UPPER(app.SURNAME) LIKE :SurnName";
                model.QueryParameters.Add("SurnName", "%" + model.SurnName.Trim().ToUpper() + "%");

            }
            if (!string.IsNullOrWhiteSpace(model.GivenName))
            {
                whereQ += "\r\n\t" + "AND UPPER(app.GIVEN_NAME_ON_ID) LIKE :GivenName";
                model.QueryParameters.Add("GivenName", "%" + model.GivenName.Trim().ToUpper() + "%");

            }

            if (!string.IsNullOrWhiteSpace(model.ChiName))
            {
                whereQ += "\r\n\t" + "AND app.CHINESE_NAME LIKE :ChiName";
                model.QueryParameters.Add("ChiName", "%" + model.ChiName.Trim().ToUpper() + "%");
            }

            return whereQ;
        }


        public string ExportPA(Fn03PA_PASearchModel model)
        {
            model.Query = SearchPA_q;
            model.QueryWhere = SearchPA_whereQ(model);
            //DisplayGrid dlr = new DisplayGrid() { Query = SearchGCA_q, Columns = Columns, Parameters = post };
            return model.Export("ExportData");
        }



        public Fn03PA_PMSearchModel SearchPM(Fn03PA_PMSearchModel model)
        {
            //model.Query = SearchCA_q;
            //model.Query = SearchPA_q;  //need to change query after all
            model.Query = SearchPA_PM_q(model);
            model.Search();
            return model;
        }

        public Fn03PA_GCNSearchModel SearchGCN(Fn03PA_GCNSearchModel model)
        {
            model.Query = SearchGCN_q;
            model.QueryWhere = SearchGCN_whereQ(model);
            model.Search();
            return model;
        }

        private string SearchGCN_whereQ(Fn03PA_GCNSearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.FileRef))
            {
                whereQ += "\r\n\t" + "AND ind.FILE_REFERENCE_NO LIKE :FileRef";
                model.QueryParameters.Add("FileRef", "%" + model.FileRef.Trim().ToUpper() + "%");
            }

            if (!string.IsNullOrWhiteSpace(model.SurName))
            {
                whereQ += "\r\n\t" + "AND UPPER(app.SURNAME) LIKE :SurnName";
                model.QueryParameters.Add("SurnName", "%" + model.SurName.Trim().ToUpper() + "%");

            }
            if (!string.IsNullOrWhiteSpace(model.GivenName))
            {
                whereQ += "\r\n\t" + "AND UPPER(app.GIVEN_NAME_ON_ID) LIKE :GivenName";
                model.QueryParameters.Add("GivenName", "%" + model.GivenName.Trim().ToUpper() + "%");

            }
            if (!string.IsNullOrWhiteSpace(model.ChiName))
            {
                whereQ += "\r\n\t" + "AND app.CHINESE_NAME LIKE :ChiName";
                model.QueryParameters.Add("ChiName", "%" + model.ChiName.Trim().ToUpper() + "%");
            }

            //whereQ += "\r\n\t" + " ORDER BY ind.FILE_REFERENCE_NO ";

            return whereQ;
        }

        public void SearchMRA(Fn03PA_MRASearchModel model)
        {
            //model.Query = SearchCA_q;
            model.Query = SearchPA_q;  //need to change query after all
            model.Search();
        }

        public void SearchIC(Fn03PA_ICSearchModel model)
        {
            //model.Query = SearchCA_q;
            model.Query = SearchGCN_q;  //need to change query after all
            model.Search();
        }

        public void SearchIR(Fn03PA_IRSearchModel model)
        {
            //model.Query = SearchCA_q;
            model.Query = SearchPA_q;  //need to change query after all
            model.Search();
        }

        public void SavePA(Fn01Search_PADisplayModel model, IEnumerable<HttpPostedFileBase> UploadDoc)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                C_IND_APPLICATION ApplicationQuery = new C_IND_APPLICATION();
                var ApplcationQ = db.C_IND_APPLICATION.Where(x => x.UUID == model.C_IND_APPLICATION.UUID).FirstOrDefault();
                if (ApplcationQ != null)
                {
                    ApplicationQuery = ApplcationQ;
                }

                C_APPLICANT ApplicantQuery = new C_APPLICANT();
                var ApplicantQ = db.C_APPLICANT.Where(x => x.UUID == ApplicationQuery.APPLICANT_ID).FirstOrDefault();

                C_ADDRESS HomeChineseQuery = new C_ADDRESS();
                var HomeChineseQ = db.C_ADDRESS.Where(x => x.UUID == ApplicationQuery.CHINESE_HOME_ADDRESS_ID).FirstOrDefault();

                C_ADDRESS HomeEnglishQuery = new C_ADDRESS();
                var HomeEnglishQ = db.C_ADDRESS.Where(x => x.UUID == ApplicationQuery.ENGLISH_HOME_ADDRESS_ID).FirstOrDefault();

                C_ADDRESS OfficeChineseQuery = new C_ADDRESS();
                var OfficeChineseQ = db.C_ADDRESS.Where(x => x.UUID == ApplicationQuery.CHINESE_OFFICE_ADDRESS_ID).FirstOrDefault();

                C_ADDRESS OfficeEnglishQuery = new C_ADDRESS();
                var OfficeEnglishQ = db.C_ADDRESS.Where(x => x.UUID == ApplicationQuery.ENGLISH_OFFICE_ADDRESS_ID).FirstOrDefault();


                C_ADDRESS BSChineseQuery = new C_ADDRESS();
                var BSChineseQ = db.C_ADDRESS.Where(x => x.UUID == ApplicationQuery.CHINESE_BS_ADDRESS_ID).FirstOrDefault();

                C_ADDRESS BSEnglishQuery = new C_ADDRESS();
                var BSEnglishQ = db.C_ADDRESS.Where(x => x.UUID == ApplicationQuery.ENGLISH_BS_ADDRESS_ID).FirstOrDefault();


                if (ApplcationQ != null)
                {
                   // ApplicationQuery = ApplcationQ;
                    ApplicantQuery = ApplicantQ;
                    HomeChineseQuery = HomeChineseQ;
                    HomeEnglishQuery = HomeEnglishQ;
                    OfficeChineseQuery = OfficeChineseQ;
                    OfficeEnglishQuery = OfficeEnglishQ;
                    BSChineseQuery = BSChineseQ;
                    BSEnglishQuery = BSEnglishQ;

                }
                else
                {

                    ApplicantQuery.UUID = Guid.NewGuid().ToString().Replace("-", "");
                    ApplicantQuery.CREATED_BY = SystemParameterConstant.UserName;
                    ApplicantQuery.CREATED_DATE = DateTime.Now;

                
                    ApplicationQuery.REGISTRATION_TYPE = RegistrationConstant.REGISTRATION_TYPE_IP;
                    ApplicationQuery.CREATED_BY = SystemParameterConstant.UserName;
                    ApplicationQuery.CREATED_DATE =DateTime.Now;


                    HomeChineseQuery.UUID = Guid.NewGuid().ToString().Replace("-", "");
                    HomeChineseQuery.CREATED_BY = SystemParameterConstant.UserName;
                    HomeChineseQuery.CREATED_DATE = DateTime.Now;


                    HomeEnglishQuery.UUID = Guid.NewGuid().ToString().Replace("-", "");
                    HomeEnglishQuery.CREATED_BY = SystemParameterConstant.UserName;
                    HomeEnglishQuery.CREATED_DATE = DateTime.Now;

                    OfficeChineseQuery.UUID = Guid.NewGuid().ToString().Replace("-", "");
                    OfficeChineseQuery.CREATED_BY = SystemParameterConstant.UserName;
                    OfficeChineseQuery.CREATED_DATE = DateTime.Now;


                    OfficeEnglishQuery.UUID = Guid.NewGuid().ToString().Replace("-", "");
                    OfficeEnglishQuery.CREATED_BY = SystemParameterConstant.UserName;
                    OfficeEnglishQuery.CREATED_DATE = DateTime.Now;

                    BSChineseQuery.UUID = Guid.NewGuid().ToString().Replace("-", "");
                    BSChineseQuery.CREATED_BY = SystemParameterConstant.UserName;
                    BSChineseQuery.CREATED_DATE = DateTime.Now;

                    BSEnglishQuery.UUID = Guid.NewGuid().ToString().Replace("-", "");
                    BSEnglishQuery.CREATED_BY = SystemParameterConstant.UserName;
                    BSEnglishQuery.CREATED_DATE = DateTime.Now;

                    ApplicationQuery.UUID = Guid.NewGuid().ToString().Replace("-", "");
                    ApplicationQuery.APPLICANT_ID = ApplicantQuery.UUID;
                    ApplicationQuery.CHINESE_HOME_ADDRESS_ID = HomeChineseQuery.UUID;
                    ApplicationQuery.ENGLISH_HOME_ADDRESS_ID = HomeEnglishQuery.UUID;
                    ApplicationQuery.CHINESE_OFFICE_ADDRESS_ID = OfficeChineseQuery.UUID;
                    ApplicationQuery.ENGLISH_OFFICE_ADDRESS_ID = OfficeEnglishQuery.UUID;
                    ApplicationQuery.CHINESE_BS_ADDRESS_ID = BSChineseQuery.UUID;
                    ApplicationQuery.ENGLISH_BS_ADDRESS_ID = BSEnglishQuery.UUID ;

                }

                ApplicantQuery.HKID = model.C_APPLICANT.HKID == null ? null : (EncryptDecryptUtil.getEncrypt(model.C_APPLICANT.HKID.Trim().ToUpper()));
                ApplicantQuery.PASSPORT_NO = model.C_APPLICANT.PASSPORT_NO == null ? null : (EncryptDecryptUtil.getEncrypt(model.C_APPLICANT.PASSPORT_NO.Trim().ToUpper()));

                ApplicantQuery.CHINESE_NAME = model.C_APPLICANT.CHINESE_NAME;

                ApplicantQuery.SURNAME = model.C_APPLICANT.SURNAME;
                ApplicantQuery.GIVEN_NAME_ON_ID = model.C_APPLICANT.GIVEN_NAME_ON_ID;
                ApplicantQuery.GENDER = model.C_APPLICANT.GENDER;
                ApplicantQuery.TITLE_ID = model.C_APPLICANT.TITLE_ID;
                ApplicantQuery.DOB = model.C_APPLICANT.DOB;

                ApplicantQuery.MODIFIED_BY = SystemParameterConstant.UserName;
                ApplicantQuery.MODIFIED_DATE = DateTime.Now;


                ApplicationQuery.FILE_REFERENCE_NO = model.C_IND_APPLICATION.FILE_REFERENCE_NO;
                ApplicationQuery.CORRESPONDENCE_LANG_ID = model.C_IND_APPLICATION.CORRESPONDENCE_LANG_ID;
                ApplicationQuery.ENGLISH_CARE_OF = model.C_IND_APPLICATION.ENGLISH_CARE_OF;
                ApplicationQuery.CHINESE_CARE_OF = model.C_IND_APPLICATION.CHINESE_CARE_OF;

                ApplicationQuery.REGION_CODE_ID = model.C_IND_APPLICATION.REGION_CODE_ID;
                ApplicationQuery.EMERGENCY_NO1 = model.C_IND_APPLICATION.EMERGENCY_NO1;
                ApplicationQuery.EMERGENCY_NO2 = model.C_IND_APPLICATION.EMERGENCY_NO2;
                ApplicationQuery.EMERGENCY_NO3 = model.C_IND_APPLICATION.EMERGENCY_NO3;

                ApplicationQuery.TELEPHONE_NO1 = model.C_IND_APPLICATION.TELEPHONE_NO1;
                ApplicationQuery.TELEPHONE_NO2 = model.C_IND_APPLICATION.TELEPHONE_NO2;
                ApplicationQuery.TELEPHONE_NO3 = model.C_IND_APPLICATION.TELEPHONE_NO3;

                ApplicationQuery.FAX_NO1 = model.C_IND_APPLICATION.FAX_NO1;
                ApplicationQuery.FAX_NO2 = model.C_IND_APPLICATION.FAX_NO2;
                ApplicationQuery.EMAIL = model.C_IND_APPLICATION.EMAIL;
                ApplicationQuery.REMARKS = model.C_IND_APPLICATION.REMARKS;
                ApplicationQuery.PRACTICE_NOTES_ID = model.C_IND_APPLICATION.PRACTICE_NOTES_ID;
                ApplicationQuery.QUALIFICATION = model.C_IND_APPLICATION.QUALIFICATION;

                ApplicationQuery.WILLINGNESS_QP = model.C_IND_APPLICATION.WILLINGNESS_QP;
                ApplicationQuery.INTERESTED_FSS = model.C_IND_APPLICATION.INTERESTED_FSS;
                //new update
                ApplicationQuery.SERVICE_IN_MWIS = model.ServiceInMWIS;
                ApplicationQuery.CHINESE_BS_CARE_OF = model.C_IND_APPLICATION.CHINESE_BS_CARE_OF;
                ApplicationQuery.ENGLISH_BS_CARE_OF = model.C_IND_APPLICATION.ENGLISH_BS_CARE_OF;
                ApplicationQuery.BS_TELEPHONE_NO1 = model.C_IND_APPLICATION.BS_TELEPHONE_NO1;
                ApplicationQuery.BS_FAX_NO1 = model.C_IND_APPLICATION.BS_FAX_NO1;


                ApplicationQuery.MODIFIED_BY = SystemParameterConstant.UserName;
                ApplicationQuery.MODIFIED_DATE = DateTime.Now;



                HomeChineseQuery.ADDRESS_LINE1 = model.HOME_ADDRESS_CHI.ADDRESS_LINE1;
                HomeChineseQuery.ADDRESS_LINE2 = model.HOME_ADDRESS_CHI.ADDRESS_LINE2;
                HomeChineseQuery.ADDRESS_LINE3 = model.HOME_ADDRESS_CHI.ADDRESS_LINE3;
                HomeChineseQuery.ADDRESS_LINE4 = model.HOME_ADDRESS_CHI.ADDRESS_LINE4;
                HomeChineseQuery.ADDRESS_LINE5 = model.HOME_ADDRESS_CHI.ADDRESS_LINE5;
                HomeChineseQuery.MODIFIED_BY = SystemParameterConstant.UserName;
                HomeChineseQuery.MODIFIED_DATE = DateTime.Now;

                HomeEnglishQuery.ADDRESS_LINE1 = model.HOME_ADDRESS_ENG.ADDRESS_LINE1;
                HomeEnglishQuery.ADDRESS_LINE2 = model.HOME_ADDRESS_ENG.ADDRESS_LINE2;
                HomeEnglishQuery.ADDRESS_LINE3 = model.HOME_ADDRESS_ENG.ADDRESS_LINE3;
                HomeEnglishQuery.ADDRESS_LINE4 = model.HOME_ADDRESS_ENG.ADDRESS_LINE4;
                HomeEnglishQuery.ADDRESS_LINE5 = model.HOME_ADDRESS_ENG.ADDRESS_LINE5;
                HomeEnglishQuery.MODIFIED_BY = SystemParameterConstant.UserName;
                HomeEnglishQuery.MODIFIED_DATE = DateTime.Now;



                OfficeChineseQuery.ADDRESS_LINE1 = model.OFFICE_ADDRESS_CHI.ADDRESS_LINE1;
                OfficeChineseQuery.ADDRESS_LINE2 = model.OFFICE_ADDRESS_CHI.ADDRESS_LINE2;
                OfficeChineseQuery.ADDRESS_LINE3 = model.OFFICE_ADDRESS_CHI.ADDRESS_LINE3;
                OfficeChineseQuery.ADDRESS_LINE4 = model.OFFICE_ADDRESS_CHI.ADDRESS_LINE4;
                OfficeChineseQuery.ADDRESS_LINE5 = model.OFFICE_ADDRESS_CHI.ADDRESS_LINE5;
                OfficeChineseQuery.MODIFIED_BY = SystemParameterConstant.UserName;
                OfficeChineseQuery.MODIFIED_DATE = DateTime.Now;


                OfficeEnglishQuery.ADDRESS_LINE1 = model.OFFICE_ADDRESS_ENG.ADDRESS_LINE1;
                OfficeEnglishQuery.ADDRESS_LINE2 = model.OFFICE_ADDRESS_ENG.ADDRESS_LINE2;
                OfficeEnglishQuery.ADDRESS_LINE3 = model.OFFICE_ADDRESS_ENG.ADDRESS_LINE3;
                OfficeEnglishQuery.ADDRESS_LINE4 = model.OFFICE_ADDRESS_ENG.ADDRESS_LINE4;
                OfficeEnglishQuery.ADDRESS_LINE5 = model.OFFICE_ADDRESS_ENG.ADDRESS_LINE5;
                OfficeEnglishQuery.MODIFIED_BY = SystemParameterConstant.UserName;
                OfficeEnglishQuery.MODIFIED_DATE = DateTime.Now;





                BSChineseQuery.ADDRESS_LINE1 = model.BS_ADDRESS_CHI.ADDRESS_LINE1;
                BSChineseQuery.ADDRESS_LINE2 = model.BS_ADDRESS_CHI.ADDRESS_LINE2;
                BSChineseQuery.ADDRESS_LINE3 = model.BS_ADDRESS_CHI.ADDRESS_LINE3;
                BSChineseQuery.ADDRESS_LINE4 = model.BS_ADDRESS_CHI.ADDRESS_LINE4;
                BSChineseQuery.ADDRESS_LINE5 = model.BS_ADDRESS_CHI.ADDRESS_LINE5;
                BSChineseQuery.MODIFIED_BY = SystemParameterConstant.UserName;
                BSChineseQuery.MODIFIED_DATE = DateTime.Now;


                BSEnglishQuery.ADDRESS_LINE1 = model.BS_ADDRESS_ENG.ADDRESS_LINE1;
                BSEnglishQuery.ADDRESS_LINE2 = model.BS_ADDRESS_ENG.ADDRESS_LINE2;
                BSEnglishQuery.ADDRESS_LINE3 = model.BS_ADDRESS_ENG.ADDRESS_LINE3;
                BSEnglishQuery.ADDRESS_LINE4 = model.BS_ADDRESS_ENG.ADDRESS_LINE4;
                BSEnglishQuery.ADDRESS_LINE5 = model.BS_ADDRESS_ENG.ADDRESS_LINE5;
                BSEnglishQuery.MODIFIED_BY = SystemParameterConstant.UserName;
                BSEnglishQuery.MODIFIED_DATE = DateTime.Now;


                if (model.SelectedCertiUUID != null)
                {
                    var queryCert = db.C_IND_CERTIFICATE.Where(x => x.UUID == model.SelectedCertiUUID).FirstOrDefault();
                    queryCert.CARD_APP_DATE = model.SelectedCertificate.CARD_APP_DATE;
                    queryCert.CARD_EXPIRY_DATE = model.SelectedCertificate.CARD_EXPIRY_DATE;
                    queryCert.CARD_ISSUE_DATE = model.SelectedCertificate.CARD_ISSUE_DATE;
                    queryCert.CARD_RETURN_DATE = model.SelectedCertificate.CARD_RETURN_DATE;
                    queryCert.CARD_SERIAL_NO = model.SelectedCertificate.CARD_SERIAL_NO;
                }

                if (ApplcationQ == null)
                {

                    db.C_IND_APPLICATION.Add(ApplicationQuery);
                    db.C_APPLICANT.Add(ApplicantQuery);
                    db.C_ADDRESS.Add(HomeChineseQuery);
                    db.C_ADDRESS.Add(HomeEnglishQuery);
                    db.C_ADDRESS.Add(OfficeChineseQuery);
                    db.C_ADDRESS.Add(OfficeEnglishQuery);
                    db.C_ADDRESS.Add(BSChineseQuery);
                    db.C_ADDRESS.Add(BSEnglishQuery);

                         

                }


                #region Qualification
                if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_QUALIFICATION))
                {
                    List<QualifcationDisplayListModel> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_QUALIFICATION] as List<QualifcationDisplayListModel>;
                    for (int i = 0; i < v.Count; i++)
                    {
                        C_IND_QUALIFICATION q = new C_IND_QUALIFICATION();
                        //q.UUID = Guid.NewGuid().ToString();.
                        string tempUUID = v[i].UUID;
                        var query = db.C_IND_QUALIFICATION.Where(x => x.UUID == tempUUID);
                        if (query.Any())
                        {
                            q = query.FirstOrDefault();
                        }
                        else
                        {
                            q.UUID = Guid.NewGuid().ToString().Replace("-", "");
                        }
                        q.MASTER_ID = ApplicationQuery.UUID;
                        q.PRB_ID = v[i].PRB;
                        q.CATEGORY_ID = v[i].QUALIFICATIONCODE;
                        q.EXPIRY_DATE = v[i].EXPIRYDATE;
                        q.REGISTRATION_NUMBER = v[i].REGISTRATIONNO;
                        q.QUALIFICATION_TYPE = v[i].QUALIFICATIONCODETYPE;
                  
                        q.MODIFIED_BY = "Admin";
                        q.MODIFIED_DATE = DateTime.Now;
                        if (!query.Any())
                        {
                            q.CREATED_BY = "Admin";
                            q.CREATED_DATE = DateTime.Now;
                            db.C_IND_QUALIFICATION.Add(q);
                        }


                        db.C_IND_QUALIFICATION_DETAIL.RemoveRange
                            (db.C_IND_QUALIFICATION_DETAIL.Where(x => x.IND_QUALIFICATION_ID == q.UUID));

                        if (v[i].SelectedCatCodeDetail != null )
                        {
                            for (int j = 0; j < v[i].SelectedCatCodeDetail.Count(); j++)
                            {
                                C_IND_QUALIFICATION_DETAIL qd = new C_IND_QUALIFICATION_DETAIL();
                                qd.UUID = Guid.NewGuid().ToString().Replace("-", "");
                                qd.IND_QUALIFICATION_ID = q.UUID;
                                qd.S_CATEGORY_CODE_DETAIL_ID = v[i].SelectedCatCodeDetail[j];
                                qd.CREATED_BY = "Admin";
                                qd.CREATED_DATE = DateTime.Now;
                                qd.MODIFIED_BY = "Admin";
                                qd.MODIFIED_DATE = DateTime.Now;
                                db.C_IND_QUALIFICATION_DETAIL.Add(qd);
                            }
                        }

                    }
                }


                if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DELETE_KEY_QUALIFICATION))
                {
                    List<string> v = SessionUtil.DraftObject[ApplicationConstant.DELETE_KEY_QUALIFICATION] as List<string>;

                    for (int i = 0; i < v.Count; i++)
                    {
                        string tempUUID = v[i];
                        var query = db.C_IND_QUALIFICATION_DETAIL.Where(x => x.IND_QUALIFICATION_ID == tempUUID);
                        db.C_IND_QUALIFICATION_DETAIL.RemoveRange(query);
                        var queryQ = db.C_IND_QUALIFICATION.Where(x => x.UUID == tempUUID).FirstOrDefault();
                        if(queryQ != null)
                        db.C_IND_QUALIFICATION.Remove(queryQ);

                    }
                }
                #endregion

                #region Certificate
                if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_CERTIFICATE))
                {
                    List<CertificateDisplayListModel> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_CERTIFICATE] as List<CertificateDisplayListModel>;
                    for (int i = 0; i < v.Count; i++)
                    {
                        C_IND_CERTIFICATE c = new C_IND_CERTIFICATE();
                        string tempUUID = v[i].UUID;
                        var query = db.C_IND_CERTIFICATE.Where(x => x.UUID == tempUUID);
                        if (query.Any())
                        {
                            c = query.FirstOrDefault();
                        }
                        else
                        {
                            int checkInt = 0;
                            if(string.IsNullOrEmpty(v[i].UUID)||int.TryParse(tempUUID,out checkInt))
                            {
                                c.UUID = Guid.NewGuid().ToString().Replace("-", "");
                            }
                            else
                            {
                                c.UUID = v[i].UUID;
                            }
                        }
                        c.APPLICATION_DATE = v[i].APPLICATION_DATE;
                        c.APPLICATION_FORM_ID = v[i].APPFORM_SV_CODE;
                        c.APPLICATION_STATUS_ID = v[i].STATUS;
                        c.APPROVAL_DATE = v[i].APPROVAL_DATE;
                        c.CARD_SERIAL_NO = v[i].CARD_SERIAL_NO;
                        c.MASTER_ID = ApplicationQuery.UUID;
                        //c.MASTER_ID = v[i].C_IND_APPLICATION.UUID;
                        c.CATEGORY_ID = v[i].CATEGORY_CODE;
                        c.CERTIFICATION_NO = v[i].REGISTRATION_NO;
                        c.PERIOD_OF_VALIDITY_ID = v[i].PERIOD_VADLIDITY_SV_CODE;
                        c.EXTENDED_DATE = v[i].RETENTION_DATE;
                        c.GAZETTE_DATE = v[i].GAZETTE_DATE;
                        c.APPROVAL_DATE = v[i].APPROVAL_DATE;
                        c.REGISTRATION_DATE = v[i].REGISTRATION_DATE;
                        c.EXPIRY_DATE = v[i].EXPIRY_DATE;
                        c.REMOVAL_DATE = v[i].REMOVAL_DATE;
                        c.REMARKS = v[i].REMARKS;
                        c.RESTORE_DATE = v[i].RESTORE_DATE;
                        c.RESTORATION_APPLICATION_DATE = v[i].RESTORATION_APPLICATION_SUMBITTED_DATE;
                        c.RETENTION_DATE = v[i].RETENTION_DATE;
                        c.RETENTION_APPLICATION_DATE = v[i].RETENTION_APPLICATION_SUMBITTED_DATE;
                        c.APPLICATION_DATE = v[i].APPLICATION_DATE;

                        #region
                        //add photo 
                        if (v[i].UploadDoc != null)
                        {
                            if (v[i].UploadDoc.ElementAt(0) != null)
                            {
                                c.FILE_PATH_NONRESTRICTED = RegistrationConstant.SIGNATURE_PATH + c.CERTIFICATION_NO.Replace("(", "_").Replace(")", "_").Replace(" ", "_").Replace("/", "_") + "\\" +
                                                     c.UUID + System.IO.Path.GetExtension(UploadDoc.ElementAt(0).FileName);
                                string filePath = ApplicationConstant.CRMFilePath + RegistrationConstant.SIGNATURE_PATH + c.CERTIFICATION_NO.Replace("(", "_").Replace(")", "_").Replace(" ","_").Replace("/","_") + "\\";
                                System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                                file.Directory.Create();
                                // filePath = filePath.Replace(@"\\", @"\");
                        
                               


                                v[i].UploadDoc.ElementAt(0).SaveAs(Path.Combine(ApplicationConstant.CRMFilePath, c.FILE_PATH_NONRESTRICTED));

                            }
                        }


                        #endregion

                     //   c.FILE_PATH_NONRESTRICTED = v[i].FILE_PATH_NON_RESTRICTED;
                     
                        c.MODIFIED_BY = "Admin";
                        c.MODIFIED_DATE = DateTime.Now;
                        if (!query.Any())
                        {
                            c.CREATED_BY = "Admin";
                            c.CREATED_DATE = DateTime.Now;
                            db.C_IND_CERTIFICATE.Add(c);
                        }

                        }

                }

                if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DELETE_KEY_CERTIFICATE))
                {
                    List<string> v = SessionUtil.DraftObject[ApplicationConstant.DELETE_KEY_CERTIFICATE] as List<string>;

                    for (int i = 0; i < v.Count; i++)
                    {
                        string tempUUID = v[i];
                       
                        var queryC = db.C_IND_CERTIFICATE.Where(x => x.UUID == tempUUID).FirstOrDefault();
                        var queryCH = db.C_IND_CERTIFICATE_HISTORY.Where(x => x.CERTIFICATE_ID == tempUUID).ToList();
                        db.C_IND_CERTIFICATE_HISTORY.RemoveRange(queryCH);
                        if (queryC != null)
                            db.C_IND_CERTIFICATE.Remove(queryC);

                    }
                }

                #endregion


                #region BSITEM
                db.C_BUILDING_SAFETY_INFO.RemoveRange(db.C_BUILDING_SAFETY_INFO.Where(o => o.MASTER_ID == model.C_IND_APPLICATION.UUID).ToList());

                for (int i = 0; i < model.BsItems.Count; i++)
                {
                    if (!model.BsItems[i].Checked) continue;
                    C_BUILDING_SAFETY_INFO bs = new C_BUILDING_SAFETY_INFO();
                    bs.UUID = Guid.NewGuid().ToString().Replace("-", "");
                    bs.MASTER_ID = model.C_IND_APPLICATION.UUID;
                    bs.BUILDING_SAFETY_ID = model.BsItems[i].CheckListUuid;
                    bs.REGISTRATION_TYPE = RegistrationConstant.REGISTRATION_TYPE_CGA;
                    bs.CREATED_BY = "Admin";
                    bs.CREATED_DATE = DateTime.Now;
                    bs.MODIFIED_BY = "Admin";
                    bs.MODIFIED_DATE = DateTime.Now;
                    db.C_BUILDING_SAFETY_INFO.Add(bs);
                }
                #endregion




                db.SaveChanges();
            }

        }

        ////public bool CanDeletePA(string id)
        ////{

    

        ////    Dictionary<string, object> QueryParameters = new Dictionary<string, object>();
        ////    QueryParameters.Add("UUID", id);
        

        ////    String q = "SELECT  C_CAN_IND_APPLICATION_DELETE( :UUID ) as result  FROM DUAL";
           
     

        ////    bool result = false;
        ////    using (EntitiesRegistration db = new EntitiesRegistration())
        ////    {
        ////        using (DbConnection conn = db.Database.Connection)
        ////        {
        ////            DbDataReader dr = CommonUtil.GetDataReader(conn, q, QueryParameters);
        ////            List<Dictionary<string, object>> Data = CommonUtil.LoadDbData(dr);
        ////            if (Data.Count > 0)
        ////            {
        ////                int s = Int32.Parse(Data[0]["RESULT"].ToString());
        ////                if (s > 0)
        ////                {
        ////                    result = false;
        ////                }
        ////                else
        ////                {
        ////                    result = true;
        ////                }
                      
        ////            }
        ////            conn.Close();
        ////        }
        ////    }

        ////    return result;
        ////}

        ////public bool DeletePA(string id)
        ////{



        ////    Dictionary<string, object> QueryParameters = new Dictionary<string, object>();
        ////    QueryParameters.Add("UUID", id);
        ////    QueryParameters.Add("USERNAME", SystemParameterConstant.UserName);

        ////    String q = " call C_IND_APPLICATION_DELETE( :UUID , :USERNAME ) ";
        ////    //q = q.Replace(":UUID", id);
        ////    //q = q.Replace(":user", SystemParameterConstant.UserName);

        ////    bool result = false;
        ////    using (EntitiesRegistration db = new EntitiesRegistration())
        ////    {
        ////        using (DbConnection conn = db.Database.Connection)
        ////        {
        ////           // CommonUtil.CallStoredProc(conn, q, QueryParameters);


        ////            DbDataReader dr =  CommonUtil.GetDataReader(conn, q, QueryParameters);
        ////            List<Dictionary<string, object>> Data = CommonUtil.LoadDbData(dr);

        ////            conn.Close();
        ////        }
        ////    }

        ////    return result;

        ////    //C_IND_QUALIFICATION
        ////    //C_IND_CERTIFICATE
        ////    //C_BUILDING_SAFETY_INFO
        ////    //C_IND_APPLICATION
        ////    //C_APPLICANT
        ////    //C_ADDRESS
        ////    //C_ADDRESS
        ////    //C_ADDRESS
        ////    //C_ADDRESS
        ////    //C_ADDRESS
        ////    //C_ADDRESS
        ////}

        //const string DRAFT_KEY_QUALIFICATION = "DRAFT_KEY_QUALIFICATION";
        //const string DELETE_KEY_QUALIFICATION = "DELETE_KEY_QUALIFICATION";
        ////public ServiceResult AddDraftQualification(QualifcationDisplayListModel model)
        ////{
        ////    if (!SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_QUALIFICATION))
        ////    {
        ////        SessionUtil.DraftObject.Add(ApplicationConstant.DRAFT_KEY_QUALIFICATION, new List<QualifcationDisplayListModel>());
        ////    }

        ////    List<QualifcationDisplayListModel> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_QUALIFICATION] as List<QualifcationDisplayListModel>;
        ////    if (model.UUID == null)
        ////    {
        ////        model.UUID = Guid.NewGuid().ToString().Replace("-", "");
        ////        v.Add(model);
        ////    }
        ////    else
        ////    {


        ////        bool inDraftSession = false;
        ////        for (int i = 0; i < v.Count(); i++)
        ////        {
        ////            if (v[i].UUID == model.UUID)
        ////            {
        ////                v[i] = model;
        ////                //v[i].PRB = model.PRB;
        ////                //v[i].QUALIFICATIONCODE = model.QUALIFICATIONCODE;
        ////                //v[i].QUALIFICATIONCODETYPE = model.QUALIFICATIONCODETYPE;
        ////                //v[i].SelectedCatCodeDetail = model.SelectedCatCodeDetail;
        ////                //v[i].REGISTRATIONNO = model.REGISTRATIONNO;
        ////                //v[i].EXPIRYDATE = model.EXPIRYDATE;
        ////                inDraftSession = true;
        ////            }

        ////        }
        ////        if (inDraftSession == false)
        ////        {
        ////            v.Add(model);
        ////        }


        ////    }

        ////    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        ////}
        //public ServiceResult DeleteQualification(string id)
        //{
        //    if (!SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DELETE_KEY_QUALIFICATION))
        //    {
        //        SessionUtil.DraftObject.Add(ApplicationConstant.DELETE_KEY_QUALIFICATION, new List<string>());
        //    }
        //    List<string> v = SessionUtil.DraftObject[ApplicationConstant.DELETE_KEY_QUALIFICATION] as List<string>;
        //    v.Add(id);
        //    if (!SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_QUALIFICATION))
        //    {
        //        SessionUtil.DraftObject.Add(ApplicationConstant.DRAFT_KEY_QUALIFICATION, new List<QualifcationDisplayListModel>());
        //    }

        //    List<QualifcationDisplayListModel> x = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_QUALIFICATION] as List<QualifcationDisplayListModel>;

        //    var ToRemoveDraft = x.Where(y => y.UUID == id);
        //    if (ToRemoveDraft != null)
        //        x.Remove(ToRemoveDraft.FirstOrDefault());


        //    return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        //}
        //public QualifcationDisplayListModel GetQualificationByUUID(string uuid)
        //{
        //    using (EntitiesRegistration db = new EntitiesRegistration())
        //    {
        //        QualifcationDisplayListModel qc = new QualifcationDisplayListModel();
        //        if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_QUALIFICATION))
        //        {
        //            List<QualifcationDisplayListModel> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_QUALIFICATION] as List<QualifcationDisplayListModel>;
        //            qc = (from temp in v
        //                  where temp.UUID == uuid
        //                  select temp).FirstOrDefault();

        //        }
        //        if (qc.UUID==null)
        //        {
        //            qc = (from Quali in db.C_IND_QUALIFICATION

        //                  join cat in db.C_S_CATEGORY_CODE on Quali.CATEGORY_ID equals cat.UUID
        //                  join sv in db.C_S_SYSTEM_VALUE on cat.CATEGORY_GROUP_ID equals sv.UUID
        //                  join svprbDescrption in db.C_S_SYSTEM_VALUE on Quali.PRB_ID equals svprbDescrption.UUID
        //                  where Quali.UUID == uuid
        //                  select new QualifcationDisplayListModel()
        //                  {
        //                      UUID = Quali.UUID,
        //                      PRB = Quali.PRB_ID,
        //                      QUALIFICATIONCODE = Quali.CATEGORY_ID,
        //                      QUALIFICATIONCODETYPE = Quali.QUALIFICATION_TYPE,
        //                      REGISTRATIONNO = Quali.REGISTRATION_NUMBER,
        //                      EXPIRYDATE = Quali.EXPIRY_DATE
        //                  }).FirstOrDefault();

        //        var qcd = from Quali in db.C_IND_QUALIFICATION
        //                  join QualiDetail in db.C_IND_QUALIFICATION_DETAIL on Quali.UUID equals QualiDetail.IND_QUALIFICATION_ID
        //                  where Quali.UUID==uuid
        //                  select QualiDetail.S_CATEGORY_CODE_DETAIL_ID;
        //            qc.SelectedCatCodeDetail = qcd.ToList();
        //        }

        //        return qc;
        //    }
        //}


        //const string DRAFT_KEY_CERTIFICATE = "DRAFT_KEY_CERTIFICATE";
        //const string DELETE_KEY_CERTIFICATE = "DELETE_KEY_CERTIFICATE";
        public ServiceResult AddDraftCertificate(CertificateDisplayListModel model)
        {
            if (!SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_CERTIFICATE))
            {
                SessionUtil.DraftObject.Add(ApplicationConstant.DRAFT_KEY_CERTIFICATE, new List<CertificateDisplayListModel>());
            }
           
            List<CertificateDisplayListModel> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_CERTIFICATE] as List<CertificateDisplayListModel>;
            //v.Add(model);

            if (model.UUID == null)
            {
                model.UUID ="1";
                v.Add(model);
            }
            else
            {


                bool inDraftSession = false;
                for (int i = 0; i < v.Count(); i++)
                {
                    if (v[i].UUID == model.UUID)
                    {
                        v[i] = model;

                        inDraftSession = true;
                    }

                }
                if (inDraftSession == false)
                {
                    v.Add(model);
                }
            }
                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }
        public ServiceResult AddEformCertificate(string EFSS_PROFESSIONAL_UUID, string FILE_REFNO)
        {          
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                string CERT_NO = db.C_EFSS_PROFESSIONAL.Find(EFSS_PROFESSIONAL_UUID).CERTNO;
                
                C_EFSS_PROFESSIONAL eform = db.C_EFSS_PROFESSIONAL.Find(EFSS_PROFESSIONAL_UUID);
                C_IND_APPLICATION indApplication = db.C_IND_APPLICATION.Where(x => x.FILE_REFERENCE_NO == FILE_REFNO).FirstOrDefault();
                
                CertificateDisplayListModel new_cert = new CertificateDisplayListModel();

                RegistrationPAService rs = new RegistrationPAService();
                List<CertificateDisplayListModel> applicantCertList = rs.GetCertificateByIndUuid(indApplication.UUID);

                CertificateDisplayListModel indCertificate = new CertificateDisplayListModel();

                DateTime ? TempRegDate = null;
                int index = 0;
                for (int i = 0; i < applicantCertList.Count(); i++)
                {
                    if (applicantCertList[i].CATEGORY_CODE.Equals(RegistrationCommonService.getUUIDbyCODE(eform.CATCODE)) ||
                        RegistrationCommonService.getUUIDbyCODE(applicantCertList[i].CATEGORY_CODE).Equals(RegistrationCommonService.getUUIDbyCODE(eform.CATCODE)) )
                    {
                        if (TempRegDate == null)
                        {
                            indCertificate = applicantCertList[i];
                            TempRegDate = applicantCertList[i].APPLICATION_DATE;
                            index = i;
                        }
                        else if (applicantCertList[i].APPLICATION_DATE != null)
                        {
                            if (DateTime.Compare(applicantCertList[i].APPLICATION_DATE.Value, TempRegDate.Value) > 0)
                            {
                                indCertificate = applicantCertList[i];
                                TempRegDate = applicantCertList[i].APPLICATION_DATE;
                                index = i;
                            }
                        }
                    }
                }
                if(indCertificate.UUID == null) // new cert
                {
                    new_cert.UUID = Guid.NewGuid().ToString().Replace("-", "");
                    new_cert.REGISTRATION_NO = eform.CERTNO;
                    new_cert.IndApplicationUUID = indApplication.UUID;
                    new_cert.CATEGORY_CODE = RegistrationCommonService.getUUIDbyCODE(eform.CATCODE); // id
                    new_cert.PERIOD_VADLIDITY_SV_CODE = RegistrationCommonService.getUUIDbyTypeAndCode(RegistrationConstant.SYSTEM_TYPE_PERIOD_OF_VALIDITY, "5"); // id
                    new_cert.APPLICATION_DATE = eform.APPLICATIONDATE;

                    new_cert.APPFORM_SV_CODE = RegistrationCommonService.getUUIDbyFormTypeCode(eform.FORMTYPE, "IP"); // id
                    // c.APPLICATION_FORM_ID = v[i].APPFORM_SV_CODE;

                    new_cert.isYellow = "background-color: #F3F781;";
                    new_cert.STATUS = RegistrationCommonService.getUUIDbyTypeAndCode(RegistrationConstant.SYSTEM_TYPE_APPLICANT_STATUS, "2");

                }
                else // modify existing cert
                {
                    new_cert = indCertificate;
                    if(new_cert.PERIOD_VADLIDITY_SV_CODE.Equals("5"))
                    {
                        new_cert.PERIOD_VADLIDITY_SV_CODE = RegistrationCommonService.getUUIDbyTypeAndCode(RegistrationConstant.SYSTEM_TYPE_PERIOD_OF_VALIDITY, "5");
                    }
                    else if (new_cert.PERIOD_VADLIDITY_SV_CODE.Equals("3"))
                    {
                        new_cert.PERIOD_VADLIDITY_SV_CODE = RegistrationCommonService.getUUIDbyTypeAndCode(RegistrationConstant.SYSTEM_TYPE_PERIOD_OF_VALIDITY, "3");
                    }
                    new_cert.CATEGORY_CODE = RegistrationCommonService.getUUIDbyCODE(eform.CATCODE);
                    if (indCertificate.APPLICATION_DATE != null)
                    {
                        if (DateTime.Compare(eform.APPLICATIONDATE, indCertificate.APPLICATION_DATE.Value) > 0)
                        {
                            new_cert.APPLICATION_DATE = eform.APPLICATIONDATE;

                        }
                        else
                        {
                            new_cert.APPLICATION_DATE = indCertificate.APPLICATION_DATE;
                        }
                    }
                    else
                    {
                        new_cert.APPLICATION_DATE = eform.APPLICATIONDATE;
                    }
                    
                    new_cert.APPFORM_SV_CODE = RegistrationCommonService.getUUIDbyFormTypeCode(eform.FORMTYPE, "IP");
                    new_cert.isYellow = "background-color: #F3F781;";
                }

                if (!SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_CERTIFICATE))
                {
                    SessionUtil.DraftObject.Add(ApplicationConstant.DRAFT_KEY_CERTIFICATE, new List<CertificateDisplayListModel>());
                }
                List<CertificateDisplayListModel> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_CERTIFICATE] as List<CertificateDisplayListModel>;
                if (!v.Contains(new_cert))
                {
                    v.Add(new_cert);
                }
            }

                return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }

        public ServiceResult DeleteCertificate(string id)
        {
            if (!SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DELETE_KEY_CERTIFICATE))
            {
                SessionUtil.DraftObject.Add(ApplicationConstant.DELETE_KEY_CERTIFICATE, new List<string>());
            }
            List<string> v = SessionUtil.DraftObject[ApplicationConstant.DELETE_KEY_CERTIFICATE] as List<string>;
            v.Add(id);
            if (!SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DELETE_KEY_CERTIFICATE))
            {
                SessionUtil.DraftObject.Add(ApplicationConstant.DELETE_KEY_CERTIFICATE, new List<CertificateDisplayListModel>());
            }

            if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_CERTIFICATE))
            {
                List<CertificateDisplayListModel> x = SessionUtil.DraftObject[ApplicationConstant.DELETE_KEY_CERTIFICATE] as List<CertificateDisplayListModel>;

                if(x != null)
                {
                    var ToRemoveDraft = x.Where(y => y.UUID == id);
                    if (ToRemoveDraft != null)
                        x.Remove(ToRemoveDraft.FirstOrDefault());
                }

            }
           


            return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };
        }

        public CertificateDisplayListModel GetCertificateByUuid(string uuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                CertificateDisplayListModel Certi = new CertificateDisplayListModel();
                if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_CERTIFICATE))
                {
                    List<CertificateDisplayListModel> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_CERTIFICATE] as List<CertificateDisplayListModel>;
                    if (Certi != null)
                    {
                        Certi = (from draft in v
                                 where draft.UUID == uuid
                                 select draft).FirstOrDefault();

                    }
                }
                if (Certi.UUID == null)
                {
                    Certi = (from indcert in db.C_IND_CERTIFICATE
                                 // join svperiod in db.C_S_SYSTEM_VALUE on indcert.PERIOD_OF_VALIDITY_ID equals svperiod.UUID
                                 // join svappform in db.C_S_SYSTEM_VALUE on indcert.APPLICATION_FORM_ID equals svappform.UUID
                             join catcode in db.C_S_CATEGORY_CODE on indcert.CATEGORY_ID equals catcode.UUID
                             where indcert.UUID == uuid
                             select new CertificateDisplayListModel
                             {
                                 UUID = indcert.UUID,
                                 IndApplicationUUID = indcert.MASTER_ID,
                                 REGISTRATION_NO = indcert.CERTIFICATION_NO,
                                 CATEGORY_CODE = catcode.UUID,
                                 STATUS = indcert.APPLICATION_STATUS_ID,
                                 PERIOD_VADLIDITY_SV_CODE = indcert.PERIOD_OF_VALIDITY_ID,
                                 APPLICATION_DATE = indcert.APPLICATION_DATE,
                                 REGISTRATION_DATE = indcert.REGISTRATION_DATE,
                                 EXPIRY_DATE = indcert.EXPIRY_DATE,
                                 EXPIRY_EXTEND_DATE = indcert.EXTENDED_DATE,
                                 RETENTION_DATE = indcert.RETENTION_DATE,
                                 RESTORE_DATE = indcert.RESTORE_DATE,
                                 REMOVAL_DATE = indcert.REMOVAL_DATE,
                                 GAZETTE_DATE = indcert.GAZETTE_DATE,
                                 APPROVAL_DATE = indcert.APPROVAL_DATE,
                                 RETENTION_APPLICATION_SUMBITTED_DATE = indcert.RETENTION_APPLICATION_DATE,
                                 RESTORATION_APPLICATION_SUMBITTED_DATE = indcert.RESTORATION_APPLICATION_DATE,
                                 APPFORM_SV_CODE = indcert.APPLICATION_FORM_ID,
                                 REMARKS = indcert.REMARKS,
                                 FILE_PATH_NON_RESTRICTED = indcert.FILE_PATH_NONRESTRICTED,
                                 CARD_SERIAL_NO = indcert.CARD_SERIAL_NO,

                       

                             }).FirstOrDefault();




                    RegistrationCommonService tempS = new RegistrationCommonService();
                    var tmp = tempS.GetQualificationByIndUuid(Certi.IndApplicationUUID);
                    Certi.PRBTableList = tmp;
                }
                return Certi;

            }

        }
        //public List<QualifcationDisplayListModel> GetQualificationByIndUuid(string uuid)
        //{
        //    using (EntitiesRegistration db = new EntitiesRegistration())
        //    {

        //        List<QualifcationDisplayListModel> list = (from Quali in db.C_IND_QUALIFICATION
        //                                                    join cat in db.C_S_CATEGORY_CODE on Quali.CATEGORY_ID equals cat.UUID
        //                                                    join sv in db.C_S_SYSTEM_VALUE on cat.CATEGORY_GROUP_ID equals sv.UUID
        //                                                    join svprbDescrption in db.C_S_SYSTEM_VALUE on Quali.PRB_ID equals svprbDescrption.UUID
        //                                                    where Quali.MASTER_ID == uuid
        //                                                    select new QualifcationDisplayListModel()
        //                                                    {
        //                                                        UUID = Quali.UUID,
        //                                                        PRB = svprbDescrption.ENGLISH_DESCRIPTION,
        //                                                        QUALIFICATIONCODE = cat.CODE,
        //                                                        REGISTRATIONNO = Quali.REGISTRATION_NUMBER,
        //                                                        EXPIRYDATE = Quali.EXPIRY_DATE
        //                                                  }).ToList();
          
        //        if (SessionUtil.DraftObject.ContainsKey(DRAFT_KEY_QUALIFICATION))
        //        {
        //            List<QualifcationDisplayListModel> v = SessionUtil.DraftObject[DRAFT_KEY_QUALIFICATION] as List<QualifcationDisplayListModel>;
            


        //            for (int i = 0; i < v.Count; i++)
        //            {

        //                list.Remove(list.Find(x=>x.UUID ==v[i].UUID));
        //                list.Add(new QualifcationDisplayListModel()
        //                {
        //                    UUID = v[i].UUID == null ? null : v[i].UUID ,
        //                    PRB = v[i].PRB == null ? null : SystemListUtil.GetSVByUUID(v[i].PRB).ENGLISH_DESCRIPTION,
        //                    QUALIFICATIONCODE = v[i].QUALIFICATIONCODE == null ? null : SystemListUtil.GetCatCodeByUUID(v[i].QUALIFICATIONCODE).CODE,
        //                    QUALIFICATIONCODETYPE = v[i].QUALIFICATIONCODETYPE == null ? null : v[i].QUALIFICATIONCODETYPE,
        //                    REGISTRATIONNO = v[i].REGISTRATIONNO == null ? null : v[i].REGISTRATIONNO,
        //                    EXPIRYDATE = v[i].EXPIRYDATE == null ? null : v[i].EXPIRYDATE,
        //                });
        //            }
        //        }
        //        if (SessionUtil.DraftObject.ContainsKey(DELETE_KEY_QUALIFICATION))
        //        {
        //            List<string> deleteList = SessionUtil.DraftObject[DELETE_KEY_QUALIFICATION] as List<string>;
                   
        //            list = list.Where(o => !deleteList.Contains(o.UUID)).ToList();
        //        }


        //        return list;
        //    }
        //}
        public List<CertificateDisplayListModel> GetCertificateByIndUuid(string uuid)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                List<CertificateDisplayListModel> CertiList = new List<CertificateDisplayListModel>();
                if (!string.IsNullOrWhiteSpace(uuid))
                { 
                 CertiList = (from indcert in db.C_IND_CERTIFICATE
                                 join svperiod in db.C_S_SYSTEM_VALUE on indcert.PERIOD_OF_VALIDITY_ID equals svperiod.UUID
                                 join svappform in db.C_S_SYSTEM_VALUE on indcert.APPLICATION_FORM_ID equals svappform.UUID into gp
                                 from svappform in gp.DefaultIfEmpty()
                                 join catcode in db.C_S_CATEGORY_CODE on indcert.CATEGORY_ID equals catcode.UUID
                                 where indcert.MASTER_ID == uuid
                                 select new CertificateDisplayListModel
                                 {
                                     UUID = indcert.UUID,
                                     CATEGORY_CODE =catcode.CODE,
                                     PERIOD_VADLIDITY_SV_CODE  =svperiod.CODE,
                                     APPLICATION_DATE = indcert.APPLICATION_DATE,
                                     REGISTRATION_DATE = indcert.REGISTRATION_DATE,
                                     EXPIRY_DATE = indcert.EXPIRY_DATE,
                                     RETENTION_DATE = indcert.RETENTION_DATE,
                                     RESTORE_DATE = indcert.RESTORE_DATE,
                                     REMOVAL_DATE = indcert.REMOVAL_DATE,
                                     APPFORM_SV_CODE = svappform.CODE,
                                     REMARKS = indcert.REMARKS,
                                     FILE_PATH_NON_RESTRICTED = indcert.FILE_PATH_NONRESTRICTED,
                                     CARD_SERIAL_NO = indcert.CARD_SERIAL_NO,
                                     REGISTRATION_NO = indcert.CERTIFICATION_NO,

                                 }).ToList();
                }
                if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_CERTIFICATE))
                {
                    List<CertificateDisplayListModel> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_CERTIFICATE] as List<CertificateDisplayListModel>;
                    

                    for (int i = 0; i < v.Count; i++)
                    {
                        CertiList.Remove(CertiList.Find(x => x.UUID == v[i].UUID));
                    }
                        CertiList.AddRange(v);
                }
                if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DELETE_KEY_CERTIFICATE))
                {
                    List<string> deleteList = SessionUtil.DraftObject[ApplicationConstant.DELETE_KEY_CERTIFICATE] as List<string>;

                    CertiList = CertiList.Where(o => !deleteList.Contains(o.UUID)).ToList();
                }
                return CertiList;

            }

        }
        public IEnumerable<SelectListItem> GetCatCodeDetailByCode(string catCode)
        {
            return (from list in SystemListUtil.GetCatCodeDetailByCode(catCode)
                    select new SelectListItem
                    {
                        Text = list.ENGLISH_DESCRIPTION,
                        Value = list.UUID
                    }).ToList();

        }

        public void GenerateSerialNo(string id, ref string serialNo, ref string dt)
        {
            serialNo = RegistrationConstant.QP_PREFIX_A + SystemListUtil.GetSVListByType(RegistrationConstant.QPCARD + RegistrationConstant.QP_PREFIX_A).FirstOrDefault().ORDERING.Value.ToString("000000");

            SystemListUtil.AddSerialNoOrdering(RegistrationConstant.QPCARD + RegistrationConstant.QP_PREFIX_A);
            RegistrationCommonService rs = new RegistrationCommonService();

            dt = rs.getCurrentCompExpiryDateMwInd(id);


        }


        //public Fn01Search_PADisplayModel GetQualificationForm(Fn01Search_PADisplayModel model)
        //{

        //    return model;
        //}

        //public void SaveTempPRB(Fn01Search_PADisplayModel model)
        //   {
        //       Fn01Search_PADisplayModel.PRBLIST prbList = new Fn01Search_PADisplayModel.PRBLIST();
        //       prbList.PRB_c_IND_QUALIFICATION = new C_IND_QUALIFICATION();

        //       prbList.PRB_c_IND_QUALIFICATION.REGISTRATION_NUMBER = model.tempPRBDetail.RegistrationNo;
        //       prbList.PRB_c_IND_QUALIFICATION.EXPIRY_DATE = model.tempPRBDetail.ExpiryDate;
        //       prbList.PRB_c_S_CATEGORY_CODE = new C_S_CATEGORY_CODE();
        //       prbList.PRB_c_S_CATEGORY_CODE.CODE = model.tempPRBDetail.QualificationCode;
        //       prbList.PRB_Des_c_S_SYSTEM_VALUE = new C_S_SYSTEM_VALUE();
        //       prbList.PRB_Des_c_S_SYSTEM_VALUE.ENGLISH_DESCRIPTION = model.tempPRBDetail.PRB;



        //       HttpContext.Current.Session["TempPRBTable"] = prbList;
        //       //model.PRBLISTs.Add(prbList);

        //   }

    }
}