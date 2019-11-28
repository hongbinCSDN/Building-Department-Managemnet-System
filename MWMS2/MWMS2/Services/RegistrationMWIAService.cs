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
using System.Web.Configuration;
using System.IO;
using System.Data.Entity;

namespace MWMS2.Services
{
    public class RegistrationMWIAService
    {
        String SearchGCA_q_copy = ""
               + "\r\n" + "\t" + "SELECT                                                               "
               + "\r\n" + "\t" + "T1.UUID                                                              "
               + "\r\n" + "\t" + ", T1.FILE_REFERENCE_NO                                               "
               + "\r\n" + "\t" + ", T1.ENGLISH_COMPANY_NAME                                            "
               + "\r\n" + "\t" + ", T1.CERTIFICATION_NO                                                "
               + "\r\n" + "\t" + ", T2.ENGLISH_DESCRIPTION                                             "
               + "\r\n" + "\t" + "FROM C_COMP_APPLICATION T1                                           "
               + "\r\n" + "\t" + "LEFT JOIN C_S_SYSTEM_VALUE T2 ON T1.APPLICATION_STATUS_ID = T2.UUID  "
               + "\r\n" + "\t" + "WHERE T1.REGISTRATION_TYPE   ='CGC'                                  ";


        public string ExportMWIA(Dictionary<string, string>[] Columns, FormCollection post)
        {
            DisplayGrid dlr = new DisplayGrid() { Query = SearchGCA_q_copy, Columns = Columns, Parameters = post };
            //dlr.Sort = "indCert.certification_No";
            //dlr.SortType = 2;
            return dlr.Export("ExportData");
        }



        //FN04
        public void SearchMWIA(Fn05MWIA_MWIASearchModel model)
        {
            //model.Query = SearchCA_q;
            model.Query = SearchGCA_q_copy;  //need to change query after all
            model.Search();
        }

        public void SearchPM(Fn05MWIA_PMSearchModel model)
        {
            //model.Query = SearchCA_q;
            model.Query = SearchGCA_q_copy;  //need to change query after all
            model.Search();
        }

        public void SearchGCN(Fn05MWIA_GCNSearchModel model)
        {
            //model.Query = SearchCA_q;
            model.Query = SearchGCA_q_copy;  //need to change query after all
            model.Search();
        }

        public void SearchMRA(Fn05MWIA_MRASearchModel model)
        {
            //model.Query = SearchCA_q;
            model.Query = SearchGCA_q_copy;  //need to change query after all
            model.Search();
        }

        public void SearchIC(Fn05MWIA_ICSearchModel model)
        {
            //model.Query = SearchCA_q;
            model.Query = SearchGCA_q_copy;  //need to change query after all
            model.Search();
        }

        public void SearchIR(Fn05MWIA_IRSearchModel model)
        {
            //model.Query = SearchCA_q;
            model.Query = SearchGCA_q_copy;  //need to change query after all
            model.Search();
        }
        public void SaveMWIA(Fn01Search_MWIADisplayModel model, IEnumerable<HttpPostedFileBase> UploadDoc)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {

                    #region application
                    C_IND_APPLICATION ApplicationQuery = new C_IND_APPLICATION();

                    var ApplicationQ = db.C_IND_APPLICATION.Where(x => x.UUID == model.C_IND_APPLICATION.UUID).FirstOrDefault();
                    if (ApplicationQ != null)
                    {
                        ApplicationQuery = ApplicationQ;
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



                    C_IND_CERTIFICATE CertiQuery = new C_IND_CERTIFICATE();
                    var CertiQ = db.C_IND_CERTIFICATE.Where(x => x.MASTER_ID == ApplicationQuery.UUID).FirstOrDefault();

                    if (ApplicationQ != null)
                    {
                        // ApplicationQuery = ApplcationQ;
                        ApplicantQuery = ApplicantQ;
                        HomeChineseQuery = HomeChineseQ;
                        HomeEnglishQuery = HomeEnglishQ;
                        OfficeChineseQuery = OfficeChineseQ;
                        OfficeEnglishQuery = OfficeEnglishQ;
                        BSChineseQuery = BSChineseQ;
                        BSEnglishQuery = BSEnglishQ;
                        CertiQuery = CertiQ;
                    }
                    else
                    {

                        ApplicantQuery.UUID = Guid.NewGuid().ToString().Replace("-", "");
                        ApplicantQuery.CREATED_BY = SystemParameterConstant.UserName;
                        ApplicantQuery.CREATED_DATE = DateTime.Now;


                        ApplicationQuery.REGISTRATION_TYPE = RegistrationConstant.REGISTRATION_TYPE_MWIA;
                        ApplicationQuery.CREATED_BY = SystemParameterConstant.UserName;
                        ApplicationQuery.CREATED_DATE = DateTime.Now;


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
                        ApplicationQuery.ENGLISH_BS_ADDRESS_ID = BSEnglishQuery.UUID;

                        CertiQuery.UUID = Guid.NewGuid().ToString().Replace("-", "");
                        CertiQuery.MASTER_ID = ApplicationQuery.UUID;
                        CertiQuery.CATEGORY_ID = model.C_IND_CERTIFICATE.CATEGORY_ID;
                        CertiQuery.CREATED_BY = SystemParameterConstant.UserName;
                        CertiQuery.CREATED_DATE = DateTime.Now;

                        //20190920 for uploadfilename
                        ApplicationQuery.FILE_REFERENCE_NO = model.C_IND_CERTIFICATE.CERTIFICATION_NO;
                        CertiQuery.CERTIFICATION_NO = SystemListUtil.GetCatCodeByUUID(model.C_IND_CERTIFICATE.CATEGORY_ID).CODE
                                                        + " " + model.C_IND_CERTIFICATE.CERTIFICATION_NO;



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


                    #region
                    //add photo 
                    if (UploadDoc != null && UploadDoc.ElementAt(0) != null)
                    {
                        var catCode = db.C_S_CATEGORY_CODE.Find(CertiQuery.CATEGORY_ID).CODE;
                        string fileRefeNo = ApplicationQuery.FILE_REFERENCE_NO;
                        string fileRefNo_Suffix = ApplicationQuery.FILE_REFERENCE_NO;
                        if (!fileRefeNo.Contains(catCode))
                        {
                            fileRefeNo = catCode + " " + fileRefeNo; // MWC(W) 1234/56
                        }
                        else
                        {
                            fileRefNo_Suffix.Replace(catCode + " ", "");
                        }
                        string fileExt = System.IO.Path.GetExtension(UploadDoc.ElementAt(0).FileName).Substring(1);
                        string fileName = "";
                        //string fileName = UploadDoc.ElementAt(0).FileName;

                        //fileName = ApplicantQuery.CHINESE_NAME + ApplicantQuery.GIVEN_NAME_ON_ID;
                        //if (model.C_IND_APPLICATION.C_APPLICANT != null && string.IsNullOrEmpty(model.C_IND_APPLICATION.C_APPLICANT.SURNAME) && string.IsNullOrEmpty(model.C_IND_APPLICATION.C_APPLICANT.GIVEN_NAME_ON_ID))
                        //    fileName = model.C_IND_APPLICATION.C_APPLICANT.SURNAME + model.C_IND_APPLICATION.C_APPLICANT.GIVEN_NAME_ON_ID;
                        //else
                        //    fileName = ApplicantQuery.CHINESE_NAME + ApplicantQuery.GIVEN_NAME_ON_ID;

                        fileName = fileRefNo_Suffix.Replace("/", "-") + "." + fileExt;
                        string subPath = getUploadFolderPath(fileRefeNo, catCode);
                        string directory = ApplicationConstant.CRMFilePath + subPath;
                        if (!Directory.Exists(@directory))
                        {
                            System.IO.Directory.CreateDirectory(@directory);
                        }
                        CertiQuery.FILE_PATH_NONRESTRICTED = subPath + fileName;
                        UploadDoc.ElementAt(0).SaveAs(Path.Combine(ApplicationConstant.CRMFilePath, CertiQuery.FILE_PATH_NONRESTRICTED));
                        //CertiQuery.FILE_PATH_NONRESTRICTED = RegistrationConstant.SIGNATURE_PATH +
                        //                     CertiQuery.UUID + System.IO.Path.GetExtension(UploadDoc.ElementAt(0).FileName);

                        //UploadDoc.ElementAt(0).SaveAs(Path.Combine(WebConfigurationManager.AppSettings["ImagePath"], CertiQuery.FILE_PATH_NONRESTRICTED));


                    }


                    #endregion

                    CertiQuery.APPLICATION_FORM_ID = model.C_IND_CERTIFICATE.APPLICATION_FORM_ID;
                    CertiQuery.APPLICATION_STATUS_ID = model.C_IND_CERTIFICATE.APPLICATION_STATUS_ID;
                    CertiQuery.PERIOD_OF_VALIDITY_ID = model.C_IND_CERTIFICATE.PERIOD_OF_VALIDITY_ID;
                    CertiQuery.APPLICATION_DATE = model.C_IND_CERTIFICATE.APPLICATION_DATE;
                    CertiQuery.CARD_APP_DATE = model.C_IND_CERTIFICATE.CARD_APP_DATE;
                    CertiQuery.CARD_ISSUE_DATE = model.C_IND_CERTIFICATE.CARD_ISSUE_DATE;
                    CertiQuery.CARD_EXPIRY_DATE = model.C_IND_CERTIFICATE.CARD_EXPIRY_DATE;
                    CertiQuery.CARD_SERIAL_NO = model.C_IND_CERTIFICATE.CARD_SERIAL_NO;
                    CertiQuery.CARD_RETURN_DATE = model.C_IND_CERTIFICATE.CARD_RETURN_DATE;
                    CertiQuery.QP_ADDRESS_C1 = model.C_IND_CERTIFICATE.QP_ADDRESS_C1;
                    CertiQuery.QP_ADDRESS_C2 = model.C_IND_CERTIFICATE.QP_ADDRESS_C2;
                    CertiQuery.QP_ADDRESS_C3 = model.C_IND_CERTIFICATE.QP_ADDRESS_C3;
                    CertiQuery.QP_ADDRESS_C4 = model.C_IND_CERTIFICATE.QP_ADDRESS_C4;
                    CertiQuery.QP_ADDRESS_C5 = model.C_IND_CERTIFICATE.QP_ADDRESS_C5;
                    CertiQuery.QP_ADDRESS_E1 = model.C_IND_CERTIFICATE.QP_ADDRESS_E1;
                    CertiQuery.QP_ADDRESS_E2 = model.C_IND_CERTIFICATE.QP_ADDRESS_E2;
                    CertiQuery.QP_ADDRESS_E3 = model.C_IND_CERTIFICATE.QP_ADDRESS_E3;
                    CertiQuery.QP_ADDRESS_E4 = model.C_IND_CERTIFICATE.QP_ADDRESS_E4;
                    CertiQuery.QP_ADDRESS_E5 = model.C_IND_CERTIFICATE.QP_ADDRESS_E5;
                    CertiQuery.MODIFIED_BY = SystemParameterConstant.UserName;
                    CertiQuery.MODIFIED_DATE = DateTime.Now;

                    ApplicationQuery.DATE_OF_DISPOSAL = model.C_IND_APPLICATION.DATE_OF_DISPOSAL;
                    ApplicationQuery.FILE_REFERENCE_NO = model.C_IND_APPLICATION.FILE_REFERENCE_NO;
                    ApplicationQuery.CORRESPONDENCE_LANG_ID = model.C_IND_APPLICATION.CORRESPONDENCE_LANG_ID;
                    ApplicationQuery.ENGLISH_CARE_OF = model.C_IND_APPLICATION.ENGLISH_CARE_OF;
                    ApplicationQuery.CHINESE_CARE_OF = model.C_IND_APPLICATION.CHINESE_CARE_OF;

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
                    //add new
                    ApplicationQuery.SERVICE_IN_MWIS = model.ServiceInMWIS;
                    ApplicationQuery.CHINESE_BS_CARE_OF = model.C_IND_APPLICATION.CHINESE_BS_CARE_OF;
                    ApplicationQuery.ENGLISH_BS_CARE_OF = model.C_IND_APPLICATION.ENGLISH_BS_CARE_OF;
                    ApplicationQuery.BS_TELEPHONE_NO1 = model.C_IND_APPLICATION.BS_TELEPHONE_NO1;
                    ApplicationQuery.BS_FAX_NO1 = model.C_IND_APPLICATION.BS_FAX_NO1;
                    ApplicationQuery.REGION_CODE_ID = model.C_IND_APPLICATION.REGION_CODE_ID;
                    ApplicationQuery.BS_REGION_CODE_ID = model.C_IND_APPLICATION.BS_REGION_CODE_ID;
                    ApplicationQuery.BS_EMAIL = model.C_IND_APPLICATION.BS_EMAIL;
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


                    if (ApplicationQ == null)
                    {

                        db.C_IND_APPLICATION.Add(ApplicationQuery);
                        db.C_APPLICANT.Add(ApplicantQuery);
                        db.C_ADDRESS.Add(HomeChineseQuery);
                        db.C_ADDRESS.Add(HomeEnglishQuery);
                        db.C_ADDRESS.Add(OfficeChineseQuery);
                        db.C_ADDRESS.Add(OfficeEnglishQuery);
                        db.C_ADDRESS.Add(BSChineseQuery);
                        db.C_ADDRESS.Add(BSEnglishQuery);
                        db.C_IND_CERTIFICATE.Add(CertiQuery);



                    }


                    #endregion
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
                            q.MASTER_ID = model.C_IND_APPLICATION.UUID;
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




                        }
                    }


                    if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DELETE_KEY_QUALIFICATION))
                    {
                        List<string> v = SessionUtil.DraftObject[ApplicationConstant.DELETE_KEY_QUALIFICATION] as List<string>;

                        for (int i = 0; i < v.Count; i++)
                        {
                            string tempUUID = v[i];

                            var queryQ = db.C_IND_QUALIFICATION.Where(x => x.UUID == tempUUID).FirstOrDefault();
                            if (queryQ != null)
                                db.C_IND_QUALIFICATION.Remove(queryQ);

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

                    #region MWItem
                    db.SaveChanges();
                    Dictionary<string, string> FinalTraySelection = new Dictionary<string, string>();
                    bool TrayVersion = false;

                    if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DELETE_KEY_MWITEM))
                    {
                        List<string> v = SessionUtil.DraftObject[ApplicationConstant.DELETE_KEY_MWITEM] as List<string>;

                        for (int i = 0; i < v.Count; i++)
                        {
                            string tempUUID = v[i];

                            var queryM = db.C_IND_APPLICATION_MW_ITEM_MASTER.Where(x => x.UUID == tempUUID).FirstOrDefault();
                            if (queryM != null)
                            {
                                db.C_IND_APPLICATION_MW_ITEM_MASTER.Remove(queryM);
                            }
                            var queryD = db.C_IND_APPLICATION_MW_ITEM_DETAIL.Where(x => x.IND_APP_MW_ITEM_MASTER_ID == tempUUID);
                            if (queryD != null)
                            {
                                db.C_IND_APPLICATION_MW_ITEM_DETAIL.RemoveRange(queryD);

                            }
                            foreach (var item in db.C_MW_IND_CAPA_DETAIL.Where(x => x.MW_IND_MW_ITEM_MASTER_ID == tempUUID))
                            {
                                db.C_MW_IND_CAPA_DETAIL_ITEM.RemoveRange(db.C_MW_IND_CAPA_DETAIL_ITEM.Where(x => x.MW_IND_CAPA_DETAIL_ID == item.UUID));

                                db.C_MW_IND_CAPA_DETAIL.Remove(item);
                            }


                            SetLatestAprrovedItem(model.C_IND_APPLICATION.UUID, v, db);

                        }
                        db.SaveChanges();
                    }
                    if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_MWITEM))
                    {

                        List<MWItemDetailListModel> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_MWITEM] as List<MWItemDetailListModel>;
                        int newFormIndex = -1;


                        for (int i = 0; i < v.Count; i++)
                        {
                            C_IND_APPLICATION_MW_ITEM_MASTER master = new C_IND_APPLICATION_MW_ITEM_MASTER();
                            string tempUUID = v[i].m_UUID;
                            var query = db.C_IND_APPLICATION_MW_ITEM_MASTER.Where(x => x.UUID == tempUUID);

                            if (query.Any())
                            {
                                master = query.FirstOrDefault();
                            }

                            master.MASTER_ID = string.IsNullOrWhiteSpace(model.C_IND_APPLICATION.UUID) ? ApplicationQuery.UUID : model.C_IND_APPLICATION.UUID;
                            master.APPLICATION_FORM_ID = v[i].m_APPLICATION_FORM_ID;
                            master.APPLICATION_DATE = v[i].m_APPLICATION_DATE;
                            master.APPROVED_BY = v[i].m_APPROVED_BY;
                            master.APPROVED_DATE = v[i].m_APPROVED_DATE;
                            master.STATUS_CODE = v[i].m_STATUS_CODE;
                            master.ITEM_REMOVE = v[i].m_MWItemDeleteByApplicant;
                            master.MODIFIED_BY = SessionUtil.LoginPost.UUID;
                            master.MODIFIED_DATE = DateTime.Now;
                            if (v[i].isTray)
                            {
                                master.SV_MWITEM_VERSION_ID = SystemListUtil.GetMWItemVersion("3").FirstOrDefault().UUID;
                            }
                            else
                            {

                                master.SV_MWITEM_VERSION_ID = SystemListUtil.GetMWItemVersion("1").FirstOrDefault().UUID;
                            }

                            if (!query.Any())
                            {
                                master.UUID = Guid.NewGuid().ToString().Replace("-", "");
                                master.CREATED_BY = SessionUtil.LoginPost.UUID;
                                master.CREATED_DATE = DateTime.Now.AddSeconds(i);
                                db.C_IND_APPLICATION_MW_ITEM_MASTER.Add(master);
                            }

                            db.SaveChanges();

                            db.C_IND_APPLICATION_MW_ITEM_DETAIL.RemoveRange(db.C_IND_APPLICATION_MW_ITEM_DETAIL.Where(x => x.IND_APP_MW_ITEM_MASTER_ID == master.UUID));
                            foreach (var item in db.C_MW_IND_CAPA_DETAIL.Where(x => x.MW_IND_MW_ITEM_MASTER_ID == master.UUID))
                            {
                                db.C_MW_IND_CAPA_DETAIL_ITEM.RemoveRange(db.C_MW_IND_CAPA_DETAIL_ITEM.Where(x => x.MW_IND_CAPA_DETAIL_ID == item.UUID));

                                db.C_MW_IND_CAPA_DETAIL.Remove(item);
                            }

                            if (v[i].isTray)
                            {

                                if (v[i].NewVerApprovedItems != null)
                                {

                                    foreach (var item in v[i].NewVerApprovedItems)
                                    {
                                        var querySV = (from svItem in db.C_S_SYSTEM_VALUE
                                                       join svType in db.C_S_SYSTEM_VALUE on svItem.PARENT_ID equals svType.UUID
                                                       join svClass in db.C_S_SYSTEM_VALUE on svType.PARENT_ID equals svClass.UUID
                                                       join sversion in db.C_S_SYSTEM_VALUE on svItem.SV_MWITEM_VERSION_ID equals sversion.UUID
                                                       where svItem.CODE == "Item " + item && sversion.CODE == "3"
                                                       select new { svItem, svType, svClass }).FirstOrDefault();

                                        C_IND_APPLICATION_MW_ITEM_DETAIL detail = new C_IND_APPLICATION_MW_ITEM_DETAIL();
                                        detail.UUID = Guid.NewGuid().ToString().Replace("-", "");
                                        detail.IND_APP_MW_ITEM_MASTER_ID = master.UUID;
                                        detail.ITEM_DETAILS_ID = querySV.svItem.UUID;
                                        detail.SUPPORTED_BY_ID = "N";
                                        detail.ITEM_TYPE_ID = querySV.svType.UUID;
                                        detail.ITEM_CLASS_ID = querySV.svClass.UUID;
                                        detail.STATUS_CODE = RegistrationConstant.MWITEM_APPROVED;
                                        detail.CREATED_BY = SessionUtil.LoginPost.UUID;
                                        detail.CREATED_DATE = DateTime.Now;
                                        detail.MODIFIED_BY = SessionUtil.LoginPost.UUID;
                                        detail.MODIFIED_DATE = DateTime.Now;
                                        db.C_IND_APPLICATION_MW_ITEM_DETAIL.Add(detail);

                                        C_IND_APPLICATION_MW_ITEM_DETAIL detailA = new C_IND_APPLICATION_MW_ITEM_DETAIL();
                                        detailA.UUID = Guid.NewGuid().ToString().Replace("-", "");
                                        detailA.IND_APP_MW_ITEM_MASTER_ID = master.UUID;
                                        detailA.ITEM_DETAILS_ID = querySV.svItem.UUID;
                                        detailA.SUPPORTED_BY_ID = "N";
                                        detailA.ITEM_TYPE_ID = querySV.svType.UUID;
                                        detailA.ITEM_CLASS_ID = querySV.svClass.UUID;
                                        detailA.STATUS_CODE = RegistrationConstant.MWITEM_APPLY;
                                        detailA.CREATED_BY = SessionUtil.LoginPost.UUID;
                                        detailA.CREATED_DATE = DateTime.Now;
                                        detailA.MODIFIED_BY = SessionUtil.LoginPost.UUID;
                                        detailA.MODIFIED_DATE = DateTime.Now;
                                        db.C_IND_APPLICATION_MW_ITEM_DETAIL.Add(detailA);


                                    }
                                }
                                db.SaveChanges();

                                foreach (var mwTray in v[i].m_NewSelectedMWitem)
                                {
                                    var a = SystemListUtil.GetNewMWItemByUUID(mwTray);

                                    C_MW_IND_CAPA_DETAIL capaD = new C_MW_IND_CAPA_DETAIL();
                                    capaD.UUID = Guid.NewGuid().ToString().Replace("-", "");
                                    capaD.MW_IND_CAPA_DISPLAY_ID = a.S_MW_IND_CAPA_ID;
                                    //capaD.MW_IND_MW_ITEM_MASTER_ID = master.UUID;
                                    capaD.MW_IND_MW_ITEM_MASTER_ID = "8a8591a22d4bb15e012d72dd42f9018d";
                                    capaD.SUPPORT_BY = v[i].m_NewSelectedMWitemSupportedBy[mwTray];

                                    capaD.CREATED_BY = SessionUtil.LoginPost.CODE;
                                    //capaD.CREATED_BY = "ADMIN";
                                    capaD.CREATED_DATE = DateTime.Now;
                                    capaD.MODIFIED_BY = SessionUtil.LoginPost.CODE;
                                    //capaD.MODIFIED_BY = "ADMIN";
                                    capaD.MODIFIED_DATE = DateTime.Now;
                                    db.C_MW_IND_CAPA_DETAIL.Add(capaD);

                                    db.SaveChanges();

                                    C_MW_IND_CAPA_DETAIL_ITEM capaDI = new C_MW_IND_CAPA_DETAIL_ITEM();
                                    capaDI.UUID = Guid.NewGuid().ToString().Replace("-", "");
                                    capaDI.MW_IND_CAPA_DETAIL_ID = capaD.UUID;
                                    capaDI.CODE = a.CHECKBOX_CODE;
                                    capaDI.CREATED_BY = SessionUtil.LoginPost.UUID;
                                    capaDI.CREATED_DATE = DateTime.Now;
                                    capaDI.MODIFIED_BY = SessionUtil.LoginPost.UUID;
                                    capaDI.MODIFIED_DATE = DateTime.Now;
                                    db.C_MW_IND_CAPA_DETAIL_ITEM.Add(capaDI);

                                    db.SaveChanges();
                                }



                            }
                            else
                            {

                                if (v[i].ApplyItems != null)
                                {
                                    foreach (var item in v[i].ApplyItems)
                                    {

                                        var querySV = (from svItem in db.C_S_SYSTEM_VALUE
                                                       join svType in db.C_S_SYSTEM_VALUE on svItem.PARENT_ID equals svType.UUID
                                                       join svClass in db.C_S_SYSTEM_VALUE on svType.PARENT_ID equals svClass.UUID
                                                       where svItem.UUID == item.Key
                                                       select new { svItem, svType, svClass }).FirstOrDefault();

                                        C_IND_APPLICATION_MW_ITEM_DETAIL detail = new C_IND_APPLICATION_MW_ITEM_DETAIL();
                                        detail.UUID = Guid.NewGuid().ToString().Replace("-", "");
                                        detail.IND_APP_MW_ITEM_MASTER_ID = master.UUID;
                                        detail.ITEM_DETAILS_ID = item.Key;
                                        detail.ITEM_TYPE_ID = querySV.svType.UUID;
                                        detail.ITEM_CLASS_ID = querySV.svClass.UUID;
                                        detail.SUPPORTED_BY_ID = item.Value;
                                        detail.STATUS_CODE = RegistrationConstant.MWITEM_APPLY;
                                        detail.CREATED_BY = SessionUtil.LoginPost.UUID;
                                        detail.CREATED_DATE = DateTime.Now;
                                        detail.MODIFIED_BY = SessionUtil.LoginPost.UUID;
                                        detail.MODIFIED_DATE = DateTime.Now;
                                        db.C_IND_APPLICATION_MW_ITEM_DETAIL.Add(detail);
                                    }
                                    db.SaveChanges();
                                }
                                if (v[i].ApprovedItems != null)
                                {
                                    foreach (var item in v[i].ApprovedItems)
                                    {
                                        var querySV = (from svItem in db.C_S_SYSTEM_VALUE
                                                       join svType in db.C_S_SYSTEM_VALUE on svItem.PARENT_ID equals svType.UUID
                                                       join svClass in db.C_S_SYSTEM_VALUE on svType.PARENT_ID equals svClass.UUID
                                                       where svItem.UUID == item.Key
                                                       select new { svItem, svType, svClass }).FirstOrDefault();

                                        C_IND_APPLICATION_MW_ITEM_DETAIL detail = new C_IND_APPLICATION_MW_ITEM_DETAIL();
                                        detail.UUID = Guid.NewGuid().ToString().Replace("-", "");
                                        detail.IND_APP_MW_ITEM_MASTER_ID = master.UUID;
                                        detail.ITEM_DETAILS_ID = item.Key;
                                        detail.SUPPORTED_BY_ID = item.Value;
                                        detail.ITEM_TYPE_ID = querySV.svType.UUID;
                                        detail.ITEM_CLASS_ID = querySV.svClass.UUID;
                                        detail.STATUS_CODE = RegistrationConstant.MWITEM_APPROVED;
                                        detail.CREATED_BY = SessionUtil.LoginPost.UUID;
                                        detail.CREATED_DATE = DateTime.Now;
                                        detail.MODIFIED_BY = SessionUtil.LoginPost.UUID;
                                        detail.MODIFIED_DATE = DateTime.Now;
                                        db.C_IND_APPLICATION_MW_ITEM_DETAIL.Add(detail);
                                    }
                                    db.SaveChanges();
                                }

                            }


                            string formCode = SystemListUtil.GetSVByUUID(v[i].m_APPLICATION_FORM_ID).CODE;
                            if ((formCode == "BA26" || formCode == "BA26A" || formCode == "BA26B") && v[i].m_STATUS_CODE == RegistrationConstant.APPLICATION_STATUS_CONFIRMED)
                            {
                                newFormIndex = i;
                            }
                        }

                        db.SaveChanges();

                        Dictionary<string, string> NewApprovedItems = new Dictionary<string, string>();

                        for (int i = v.Count - 1; i >= newFormIndex; i--)
                        {
                            if (i == -1 || v[i].m_STATUS_CODE == RegistrationConstant.APPLICATION_STATUS_APPLICANTION_IN_PROGRESS)
                            {
                                break;
                            }

                            if (v[i].isTray)
                            {
                                TrayVersion = true;
                                foreach (var item in v[i].NewVerApprovedItems)
                                {
                                    if (!NewApprovedItems.ContainsKey(item))
                                    {
                                        NewApprovedItems.Add(item, "N");
                                    }
                                }
                            }
                            else
                            {
                                foreach (var item in v[i].ApprovedItems)
                                {
                                    if (!NewApprovedItems.ContainsKey(item.Key))
                                    {
                                        NewApprovedItems.Add(item.Key, item.Value);
                                    }
                                }
                            }

                        }

                        db.SaveChanges();

                        if (newFormIndex == -1)
                        {
                            using (EntitiesRegistration db2 = new EntitiesRegistration())
                            {
                                List<string> TempUUIDList = new List<string>();

                                TempUUIDList = v.Select(x => x.m_UUID).ToList();

                                var CurrentqueryMWItem = (from mwMaster in db2.C_IND_APPLICATION_MW_ITEM_MASTER
                                                          join sv in db2.C_S_SYSTEM_VALUE on mwMaster.APPLICATION_FORM_ID equals sv.UUID
                                                          where mwMaster.MASTER_ID == model.C_IND_APPLICATION.UUID
                                                          && (sv.CODE == "BA26" || sv.CODE == "BA26A" || sv.CODE == "BA26B")
                                                          && !TempUUIDList.Contains(mwMaster.UUID)
                                                          orderby mwMaster.CREATED_DATE descending
                                                          select mwMaster).FirstOrDefault();
                                var CurrentqueryMWItemExtraForm = (from mwMaster in db2.C_IND_APPLICATION_MW_ITEM_MASTER
                                                                   join sv in db2.C_S_SYSTEM_VALUE on mwMaster.APPLICATION_FORM_ID equals sv.UUID
                                                                   where mwMaster.MASTER_ID == model.C_IND_APPLICATION.UUID
                                                                   && mwMaster.CREATED_DATE >= CurrentqueryMWItem.CREATED_DATE

                                                                   select mwMaster.UUID).ToList();

                                var CurrentMWitemDetail = from mwItemDetail in db2.C_IND_APPLICATION_MW_ITEM_DETAIL
                                                          where mwItemDetail.IND_APP_MW_ITEM_MASTER_ID == CurrentqueryMWItem.UUID
                                                          && mwItemDetail.STATUS_CODE == RegistrationConstant.MWITEM_APPROVED
                                                          select mwItemDetail;


                                var NewTrayDetail = db2.C_MW_IND_CAPA_DETAIL.Where(x => x.MW_IND_MW_ITEM_MASTER_ID == CurrentqueryMWItem.UUID).Include(x => x.C_S_MW_IND_CAPA);
                                foreach (var item in NewTrayDetail)
                                {
                                    TrayVersion = true;
                                    FinalTraySelection.Add(db2.C_S_MW_IND_CAPA.Find(item.MW_IND_CAPA_DISPLAY_ID).C_S_MW_IND_CAPA_MAIN.FirstOrDefault().UUID, item.SUPPORT_BY);
                                }
                                db.SaveChanges();
                                foreach (var item in v)
                                {
                                    if (CurrentqueryMWItemExtraForm.Contains(item.m_UUID))
                                    {
                                        TrayVersion = true;
                                    }
                                    CurrentqueryMWItemExtraForm.Remove(item.m_UUID);
                                }
                                db.SaveChanges();
                                foreach (var item in CurrentqueryMWItemExtraForm)
                                {
                                    var NewExtraTrayDetail = db2.C_MW_IND_CAPA_DETAIL.Where(x => x.MW_IND_MW_ITEM_MASTER_ID == item);
                                    foreach (var tray in NewExtraTrayDetail)
                                    {
                                        if (!FinalTraySelection.ContainsKey(db2.C_S_MW_IND_CAPA.Find(tray.MW_IND_CAPA_DISPLAY_ID).C_S_MW_IND_CAPA_MAIN.FirstOrDefault().UUID))
                                        {
                                            FinalTraySelection.Add(db2.C_S_MW_IND_CAPA.Find(tray.MW_IND_CAPA_DISPLAY_ID).C_S_MW_IND_CAPA_MAIN.FirstOrDefault().UUID, tray.SUPPORT_BY);
                                        }
                                    }

                                }
                                db.SaveChanges();


                                foreach (var item in CurrentMWitemDetail)
                                {
                                    var tempCode = SystemListUtil.GetSVByUUID(item.ITEM_DETAILS_ID).CODE.Substring(5);
                                    if (!NewApprovedItems.ContainsKey(item.ITEM_DETAILS_ID) && !NewApprovedItems.ContainsKey(tempCode))
                                    {
                                        NewApprovedItems.Add(item.ITEM_DETAILS_ID, item.SUPPORTED_BY_ID);
                                    }
                                }
                                db.SaveChanges();
                                foreach (var item in CurrentqueryMWItemExtraForm)
                                {
                                    //var CurrentMWitemDetailExtra = from mwItemDetail in db.C_IND_APPLICATION_MW_ITEM_DETAIL
                                    //                          where mwItemDetail.IND_APP_MW_ITEM_MASTER_ID == item
                                    //                          && mwItemDetail.STATUS_CODE == RegistrationConstant.MWITEM_APPROVED
                                    //                          select mwItemDetail;
                                    //using (EntitiesRegistration db2 = new EntitiesRegistration())
                                    //{
                                    List<C_IND_APPLICATION_MW_ITEM_DETAIL> CurrentMWitemDetailExtra = db2.C_IND_APPLICATION_MW_ITEM_DETAIL.Where(x => x.IND_APP_MW_ITEM_MASTER_ID == item && x.STATUS_CODE == RegistrationConstant.MWITEM_APPROVED).ToList();

                                    foreach (var ExtraMWItem in CurrentMWitemDetailExtra)
                                    {

                                        if (!NewApprovedItems.ContainsKey(ExtraMWItem.ITEM_DETAILS_ID) && !NewApprovedItems.ContainsKey(SystemListUtil.GetSVByUUID(ExtraMWItem.ITEM_DETAILS_ID).CODE.Substring(5)))
                                        {
                                            NewApprovedItems.Add(ExtraMWItem.ITEM_DETAILS_ID, ExtraMWItem.SUPPORTED_BY_ID);
                                        }
                                        // }
                                    }
                                }
                                db.SaveChanges();
                            }


                        }


                        db.C_IND_APPLICATION_MW_ITEM.RemoveRange
                        (db.C_IND_APPLICATION_MW_ITEM.Where(x => x.MASTER_ID == model.C_IND_APPLICATION.UUID));


                        foreach (var item in NewApprovedItems)
                        {
                            bool isTray = false;
                            var querySV = (from svItem in db.C_S_SYSTEM_VALUE
                                           join svType in db.C_S_SYSTEM_VALUE on svItem.PARENT_ID equals svType.UUID
                                           join svClass in db.C_S_SYSTEM_VALUE on svType.PARENT_ID equals svClass.UUID
                                           where svItem.UUID == item.Key
                                           select new { svItem, svType, svClass }).FirstOrDefault();
                            if (querySV == null)
                            {
                                isTray = true;
                                querySV = (from svItem in db.C_S_SYSTEM_VALUE
                                           join svType in db.C_S_SYSTEM_VALUE on svItem.PARENT_ID equals svType.UUID
                                           join svClass in db.C_S_SYSTEM_VALUE on svType.PARENT_ID equals svClass.UUID
                                           join sversion in db.C_S_SYSTEM_VALUE on svItem.SV_MWITEM_VERSION_ID equals sversion.UUID
                                           where svItem.CODE == "Item " + item.Key && sversion.CODE == "3"
                                           select new { svItem, svType, svClass }).FirstOrDefault();

                            }

                            C_IND_APPLICATION_MW_ITEM m = new C_IND_APPLICATION_MW_ITEM();
                            m.UUID = Guid.NewGuid().ToString().Replace("-", "");
                            m.MASTER_ID = string.IsNullOrWhiteSpace(model.C_IND_APPLICATION.UUID) ? ApplicationQuery.UUID : model.C_IND_APPLICATION.UUID;
                            if (isTray)
                            { m.ITEM_DETAILS_ID = querySV.svItem.UUID; }
                            else
                            { m.ITEM_DETAILS_ID = item.Key; }

                            m.ITEM_TYPE_ID = querySV.svType.UUID;
                            m.ITEM_CLASS_ID = querySV.svClass.UUID;
                            m.SUPPORTED_BY_ID = item.Value;
                            m.CREATED_BY = SessionUtil.LoginPost.UUID;
                            m.CREATED_DATE = DateTime.Now;
                            m.MODIFIED_BY = SessionUtil.LoginPost.UUID;
                            m.MODIFIED_DATE = DateTime.Now;
                            db.C_IND_APPLICATION_MW_ITEM.Add(m);
                        }


                        if (TrayVersion == true)
                        {
                            var toDel = db.C_MW_IND_CAPA_FINAL.Where(x => x.MASTER_ID == model.C_IND_APPLICATION.UUID);
                            foreach (var item in toDel)
                            {
                                db.C_MW_IND_CAPA_FINAL_ITEM.RemoveRange(db.C_MW_IND_CAPA_FINAL_ITEM.Where(x => x.MW_IND_CAPA_FINAL_ID == item.UUID));

                                db.C_MW_IND_CAPA_FINAL.Remove(item);

                            }



                            for (int i = 0; i < v.Count; i++)
                            {
                                if (v[i].m_NewSelectedMWitem != null)
                                {
                                    foreach (var mwTray in v[i].m_NewSelectedMWitem)
                                    {
                                        if (!FinalTraySelection.ContainsKey(mwTray))
                                        {
                                            FinalTraySelection.Add(mwTray, v[i].m_NewSelectedMWitemSupportedBy[mwTray]);
                                        }

                                    }
                                }

                            }
                            foreach (var item in FinalTraySelection)
                            {
                                var a = SystemListUtil.GetNewMWItemByUUID(item.Key);
                                C_MW_IND_CAPA_FINAL capaF = new C_MW_IND_CAPA_FINAL();
                                capaF.UUID = Guid.NewGuid().ToString().Replace("-", "");
                                capaF.CREATED_BY = SessionUtil.LoginPost.UUID;
                                capaF.CREATED_DATE = DateTime.Now;
                                capaF.MODIFIED_BY = SessionUtil.LoginPost.UUID;
                                capaF.MODIFIED_DATE = DateTime.Now;
                                capaF.MW_IND_CAPA_DISPLAY_ID = a.S_MW_IND_CAPA_ID;
                                capaF.MASTER_ID = string.IsNullOrWhiteSpace(model.C_IND_APPLICATION.UUID) ? ApplicationQuery.UUID : model.C_IND_APPLICATION.UUID;
                                capaF.SUPPORT_BY = item.Value;
                                db.C_MW_IND_CAPA_FINAL.Add(capaF);


                                C_MW_IND_CAPA_FINAL_ITEM capaFI = new C_MW_IND_CAPA_FINAL_ITEM();
                                capaFI.UUID = Guid.NewGuid().ToString().Replace("-", "");
                                capaFI.CREATED_BY = SessionUtil.LoginPost.UUID;
                                capaFI.CREATED_DATE = DateTime.Now;
                                capaFI.MODIFIED_BY = SessionUtil.LoginPost.UUID;
                                capaFI.MODIFIED_DATE = DateTime.Now;
                                capaFI.MW_IND_CAPA_FINAL_ID = capaF.UUID;
                                capaFI.CODE = a.CHECKBOX_CODE;
                                db.C_MW_IND_CAPA_FINAL_ITEM.Add(capaFI);


                            }

                        }






                    }




                    #endregion

                    db.SaveChanges();

                    tran.Commit();
                }





            }

        }

        public void SetLatestAprrovedItem(string applicationUUID, List<string> v, EntitiesRegistration db)
        {
            //  using (EntitiesRegistration db = new EntitiesRegistration())
            //  {
            Dictionary<string, string> FinalTraySelection = new Dictionary<string, string>();

            Dictionary<string, string> NewApprovedItems = new Dictionary<string, string>();
            db.C_IND_APPLICATION_MW_ITEM.RemoveRange
                 (db.C_IND_APPLICATION_MW_ITEM.Where(x => x.MASTER_ID == applicationUUID));
            var CurrentqueryMWItemList = (from mwMaster in db.C_IND_APPLICATION_MW_ITEM_MASTER
                                          join sv in db.C_S_SYSTEM_VALUE on mwMaster.APPLICATION_FORM_ID equals sv.UUID
                                          where mwMaster.MASTER_ID == applicationUUID
                                          && (sv.CODE == "BA26" || sv.CODE == "BA26A" || sv.CODE == "BA26B")
                                          orderby mwMaster.CREATED_DATE descending
                                          select mwMaster);

            //  var test = CurrentqueryMWItemList.Where(x => !v.Contains(x.UUID)).OrderByDescending(x=>x.CREATED_DATE);
            var CurrentqueryMWItem = CurrentqueryMWItemList.Where(x => !v.Contains(x.UUID)).FirstOrDefault();
            if (CurrentqueryMWItem != null)
            {
                var CurrentqueryMWItemExtraFormFullList = (from mwMaster in db.C_IND_APPLICATION_MW_ITEM_MASTER
                                                           join sv in db.C_S_SYSTEM_VALUE on mwMaster.APPLICATION_FORM_ID equals sv.UUID
                                                           where mwMaster.MASTER_ID == applicationUUID
                                                           && mwMaster.CREATED_DATE > CurrentqueryMWItem.CREATED_DATE
                                                           && !v.Contains(mwMaster.UUID)
                                                           select mwMaster).ToList();

                var CurrentqueryMWItemExtraFormList = CurrentqueryMWItemExtraFormFullList.Select(x => x.UUID).ToList();

                var CurrentqueryMWItemExtraForm = CurrentqueryMWItemExtraFormList.Where(x => !v.Contains(x));





                var CurrentMWitemDetail = from mwItemDetail in db.C_IND_APPLICATION_MW_ITEM_DETAIL
                                          where mwItemDetail.IND_APP_MW_ITEM_MASTER_ID == CurrentqueryMWItem.UUID
                                          && mwItemDetail.STATUS_CODE == RegistrationConstant.MWITEM_APPROVED
                                          select mwItemDetail;

                foreach (var item in CurrentMWitemDetail)
                {
                    if (!NewApprovedItems.ContainsKey(item.ITEM_DETAILS_ID))
                    {
                        NewApprovedItems.Add(item.ITEM_DETAILS_ID, item.SUPPORTED_BY_ID);
                    }
                }
                foreach (var item in CurrentqueryMWItemExtraForm)
                {
                    var CurrentMWitemDetailExtra = from mwItemDetail in db.C_IND_APPLICATION_MW_ITEM_DETAIL
                                                   where mwItemDetail.IND_APP_MW_ITEM_MASTER_ID == item
                                                   && mwItemDetail.STATUS_CODE == RegistrationConstant.MWITEM_APPROVED
                                                   select mwItemDetail;

                    foreach (var ExtraMWItem in CurrentMWitemDetailExtra)
                    {
                        if (!NewApprovedItems.ContainsKey(ExtraMWItem.ITEM_DETAILS_ID))
                        {
                            NewApprovedItems.Add(ExtraMWItem.ITEM_DETAILS_ID, ExtraMWItem.SUPPORTED_BY_ID);
                        }
                    }


                }





                foreach (var item in NewApprovedItems)
                {
                    var querySV = (from svItem in db.C_S_SYSTEM_VALUE
                                   join svType in db.C_S_SYSTEM_VALUE on svItem.PARENT_ID equals svType.UUID
                                   join svClass in db.C_S_SYSTEM_VALUE on svType.PARENT_ID equals svClass.UUID
                                   where svItem.UUID == item.Key
                                   select new { svItem, svType, svClass }).FirstOrDefault();

                    C_IND_APPLICATION_MW_ITEM m = new C_IND_APPLICATION_MW_ITEM();
                    m.UUID = Guid.NewGuid().ToString().Replace("-", "");
                    m.MASTER_ID = applicationUUID;
                    m.ITEM_DETAILS_ID = item.Key;
                    m.ITEM_TYPE_ID = querySV.svType.UUID;
                    m.ITEM_CLASS_ID = querySV.svClass.UUID;
                    m.SUPPORTED_BY_ID = item.Value;
                    m.CREATED_BY = SystemParameterConstant.UserName;
                    m.CREATED_DATE = DateTime.Now;
                    m.MODIFIED_BY = SystemParameterConstant.UserName;
                    m.MODIFIED_DATE = DateTime.Now;
                    db.C_IND_APPLICATION_MW_ITEM.Add(m);

                }
                var toDel = db.C_MW_IND_CAPA_FINAL.Where(x => x.MASTER_ID == applicationUUID);
                foreach (var item in toDel)
                {
                    db.C_MW_IND_CAPA_FINAL_ITEM.RemoveRange(db.C_MW_IND_CAPA_FINAL_ITEM.Where(x => x.MW_IND_CAPA_FINAL_ID == item.UUID));

                    db.C_MW_IND_CAPA_FINAL.Remove(item);

                }

                if (CurrentqueryMWItem.SV_MWITEM_VERSION_ID == SystemListUtil.GetMWItemVersion("3").FirstOrDefault().UUID)
                {
                    var NewTrayDetail = db.C_MW_IND_CAPA_DETAIL.Where(x => x.MW_IND_MW_ITEM_MASTER_ID == CurrentqueryMWItem.UUID);
                    foreach (var item in NewTrayDetail)
                    {
                        if (!FinalTraySelection.ContainsKey(db.C_S_MW_IND_CAPA.Find(item.MW_IND_CAPA_DISPLAY_ID).C_S_MW_IND_CAPA_MAIN.FirstOrDefault().UUID))
                        {
                            FinalTraySelection.Add(db.C_S_MW_IND_CAPA.Find(item.MW_IND_CAPA_DISPLAY_ID).C_S_MW_IND_CAPA_MAIN.FirstOrDefault().UUID, item.SUPPORT_BY);
                        }
                    }
                }

                foreach (var xItem in CurrentqueryMWItemExtraFormFullList)
                {
                    if (xItem.SV_MWITEM_VERSION_ID == SystemListUtil.GetMWItemVersion("3").FirstOrDefault().UUID)
                    {

                        var NewTrayDetail = db.C_MW_IND_CAPA_DETAIL.Where(x => x.MW_IND_MW_ITEM_MASTER_ID == xItem.UUID);
                        foreach (var item in NewTrayDetail)
                        {
                            if (!FinalTraySelection.ContainsKey(db.C_S_MW_IND_CAPA.Find(item.MW_IND_CAPA_DISPLAY_ID).C_S_MW_IND_CAPA_MAIN.FirstOrDefault().UUID))
                            {
                                FinalTraySelection.Add(db.C_S_MW_IND_CAPA.Find(item.MW_IND_CAPA_DISPLAY_ID).C_S_MW_IND_CAPA_MAIN.FirstOrDefault().UUID, item.SUPPORT_BY);
                            }
                        }
                        foreach (var item in CurrentqueryMWItemExtraForm)
                        {
                            var NewExtraTrayDetail = db.C_MW_IND_CAPA_DETAIL.Where(x => x.MW_IND_MW_ITEM_MASTER_ID == item);
                            foreach (var tray in NewExtraTrayDetail)
                            {
                                if (!FinalTraySelection.ContainsKey(db.C_S_MW_IND_CAPA.Find(tray.MW_IND_CAPA_DISPLAY_ID).C_S_MW_IND_CAPA_MAIN.FirstOrDefault().UUID))
                                {
                                    FinalTraySelection.Add(db.C_S_MW_IND_CAPA.Find(tray.MW_IND_CAPA_DISPLAY_ID).C_S_MW_IND_CAPA_MAIN.FirstOrDefault().UUID, tray.SUPPORT_BY);
                                }
                            }



                        }

                    }


                }
                foreach (var _item in FinalTraySelection)
                {
                    var a = SystemListUtil.GetNewMWItemByUUID(_item.Key);
                    C_MW_IND_CAPA_FINAL capaF = new C_MW_IND_CAPA_FINAL();
                    capaF.UUID = Guid.NewGuid().ToString().Replace("-", "");
                    capaF.CREATED_BY = SessionUtil.LoginPost.UUID;
                    capaF.CREATED_DATE = DateTime.Now;
                    capaF.MODIFIED_BY = SessionUtil.LoginPost.UUID;
                    capaF.MODIFIED_DATE = DateTime.Now;
                    capaF.MW_IND_CAPA_DISPLAY_ID = a.S_MW_IND_CAPA_ID;
                    capaF.MASTER_ID = applicationUUID;
                    capaF.SUPPORT_BY = _item.Value;
                    db.C_MW_IND_CAPA_FINAL.Add(capaF);


                    C_MW_IND_CAPA_FINAL_ITEM capaFI = new C_MW_IND_CAPA_FINAL_ITEM();
                    capaFI.UUID = Guid.NewGuid().ToString().Replace("-", "");
                    capaFI.CREATED_BY = SessionUtil.LoginPost.UUID;
                    capaFI.CREATED_DATE = DateTime.Now;
                    capaFI.MODIFIED_BY = SessionUtil.LoginPost.UUID;
                    capaFI.MODIFIED_DATE = DateTime.Now;
                    capaFI.MW_IND_CAPA_FINAL_ID = capaF.UUID;
                    capaFI.CODE = a.CHECKBOX_CODE;
                    db.C_MW_IND_CAPA_FINAL_ITEM.Add(capaFI);


                }


                db.SaveChanges();
                //    }

            }


        }


        public void GenerateSerialNo(string id, ref string serialNo, ref string dt)
        {
            serialNo = RegistrationConstant.QP_PREFIX_D + SystemListUtil.GetSVListByType(RegistrationConstant.QPCARD + RegistrationConstant.QP_PREFIX_D).FirstOrDefault().ORDERING.Value.ToString("000000");

            SystemListUtil.AddSerialNoOrdering(RegistrationConstant.QPCARD + RegistrationConstant.QP_PREFIX_D);
            RegistrationCommonService rs = new RegistrationCommonService();

            dt = rs.getCurrentCompExpiryDateMwInd(id);


        }
        public void DelDoc(string id)
        {
            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = db.C_IND_APPLICATION.Where(x => x.UUID == id).Include(x => x.C_IND_CERTIFICATE);

                query.FirstOrDefault().C_IND_CERTIFICATE.FirstOrDefault().FILE_PATH_NONRESTRICTED = null;

                db.SaveChanges();
            }
        }

        public List<C_MW_IND_CAPA_FINAL> GetFinalTrayMWItemByMasterUUID(string uuid)
        {

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = db.C_MW_IND_CAPA_FINAL.Where(x => x.MASTER_ID == uuid).Include(x => x.C_MW_IND_CAPA_FINAL_ITEM).ToList();


                return query;

            }
        }
        public List<C_MW_IND_CAPA_DETAIL> GetTrayMWItemByMasterUUID(string uuid)
        {

            using (EntitiesRegistration db = new EntitiesRegistration())
            {
                var query = db.C_MW_IND_CAPA_DETAIL.Where(x => x.MW_IND_MW_ITEM_MASTER_ID == uuid).Include(x => x.C_MW_IND_CAPA_DETAIL_ITEM).ToList();

                if (query.Count == 0)
                {
                    if (SessionUtil.DraftObject.ContainsKey(ApplicationConstant.DRAFT_KEY_MWITEM))
                    {
                        List<MWItemDetailListModel> v = SessionUtil.DraftObject[ApplicationConstant.DRAFT_KEY_MWITEM] as List<MWItemDetailListModel>;
                        v.Find(x => x.m_UUID == uuid);
                        if (v != null)
                        {
                            query = new List<C_MW_IND_CAPA_DETAIL>();

                            foreach (var mwTray in v.FirstOrDefault().m_NewSelectedMWitem)
                            {
                                C_MW_IND_CAPA_DETAIL d = new C_MW_IND_CAPA_DETAIL();
                                var a = SystemListUtil.GetNewMWItemByUUID(mwTray);
                                d.MW_IND_CAPA_DISPLAY_ID = a.S_MW_IND_CAPA_ID;
                                d.SUPPORT_BY = v.FirstOrDefault().m_NewSelectedMWitemSupportedBy[mwTray];
                                query.Add(d);

                            }


                        }
                    }
                }
                return query;

            }
        }
        public string getUploadFolderPath(string fileReferenceNo, string category)
        {
            string fileSeparator = Char.ToString(ApplicationConstant.FileSeparator);
            string path = RegistrationConstant.SIGNATURE_PATH;
            string subPath = fileReferenceNo.Replace("(", "_").Replace(")", "_").Replace(" ", "__").Replace("/", "_")
                + fileSeparator + category.Replace("(", "_").Replace(")", "_") + fileSeparator;
            path += subPath;
            return path;
        }

        public string getMWItemCapByRefNoAndFormNo(string refNo, string FormNo)
        {
            string result = "";



            string query = "select  C_GET_CAPABILITY_BY_REFNO(" + "'" + refNo + "'" + ",'" + FormNo + "'" + ")" + " from dual";

            // setup connection
            EntitiesRegistration db = new EntitiesRegistration();
            DbConnection conn = db.Database.Connection;
            DbCommand comm = conn.CreateCommand();
            comm.CommandText = query;
            conn.Open();
            DbDataReader dr = comm.ExecuteReader(CommandBehavior.CloseConnection);

            dr.Read();
            string mwitem = "";
            if (!dr.IsDBNull(0))
            {
                mwitem = dr.GetString(0);
            }

            dr.Close();

            conn.Close();
            if (!string.IsNullOrEmpty(mwitem))
            {
                var temp = mwitem.Split(',');
                foreach (var item in temp)
                {
                    string t = item.Replace(" ", "");
                    result += t.Substring(5, t.Length - 2 - 5) + ",";
                }
                result = result.Substring(0, result.Length - 1);


            }
            return result;

        }

    }
}