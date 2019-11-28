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
using MWMS2.Areas.Registration.Models;
using MWMS2.Constant;
using System.Data.Entity;
using MWMS2.Areas.Signboard.Models;
using System.Globalization;
using MWMS2.Services.Signborad.SignboardDAO;
using System.IO;

namespace MWMS2.Services.Signborad.SignboardServices
{
    public class SignboardEformService : BaseCommonService
    {
        public string SearchSCUR_EA_q = "SELECT * FROM (SELECT master.ID AS EFSS_ID, master.SUBMISSIONNO AS EFSS_SUBMISSIONNO, master.FOURPLUSTWO AS EFSS_FOURPLUSTWO,"
                                        + " \r\n\t map.SMM_REFERENCE_NO AS MAP_SC_NO, map.DSN AS MAP_DSN,"
                                        + " \r\n\t CASE WHEN SC01.ID IS NOT NULL THEN 'SC01'"
                                        + " \r\n\t WHEN SC01C.ID IS NOT NULL THEN 'SC01C'"
                                        + " \r\n\t WHEN SC02.ID IS NOT NULL THEN 'SC02'"
                                        + " \r\n\t WHEN SC02C.ID IS NOT NULL THEN 'SC02C'"
                                        + " \r\n\t WHEN SC03.ID IS NOT NULL THEN 'SC03' END AS FORM_CODE"
                                        + " \r\n   FROM B_EFSS_FORM_MASTER master"
                                        + " \r\n\t LEFT JOIN B_EFSS_SUBMISSION_MAP map ON master.ID = map.EFSS_SUBMISSION_NO"
                                        + " \r\n\t LEFT JOIN B_EFSCU_tbl_SC01 SC01 ON master.FORMCONTENTID = SC01.ID"
                                        + " \r\n\t LEFT JOIN B_EFSCU_tbl_SC01C SC01C ON master.FORMCONTENTID = SC01C.ID"
                                        + " \r\n\t LEFT JOIN B_EFSCU_tbl_SC02 SC02 ON master.FORMCONTENTID = SC02.ID"
                                        + " \r\n\t LEFT JOIN B_EFSCU_tbl_SC02C SC02C ON master.FORMCONTENTID = SC02C.ID"
                                        + " \r\n\t LEFT JOIN B_EFSCU_tbl_SC03 SC03 ON master.FORMCONTENTID = SC03.ID)"
                                        + " \r\n   WHERE 1=1";

        public Fn01SCUR_EASearchModel SearchSCUR_EA(Fn01SCUR_EASearchModel model)
        {
            model.Query = SearchSCUR_EA_q;
            model.QueryWhere = SearchSCUR_EA_whereQ(model);

            model.Search();
          
            return model;
        }

        private string SearchSCUR_EA_whereQ(Fn01SCUR_EASearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.SCNo))
            {
                whereQ += "\r\n\t AND upper(MAP_SC_NO) LIKE :SMM_REFERENCE_NO ";
                model.QueryParameters.Add("SMM_REFERENCE_NO", "%" + model.SCNo.Trim().ToUpper() + "%");
            }
            if(!string.IsNullOrWhiteSpace(model.DSN))
            {
                whereQ += "\r\n\t AND upper(MAP_DSN) LIKE :DSN";
                model.QueryParameters.Add("DSN", "%" + model.DSN.Trim().ToUpper() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.EfssNo))
            {
                whereQ += "\r\n\t AND upper(EFSS_SUBMISSIONNO) LIKE :EFSS_SUBMISSION_NO";
                model.QueryParameters.Add("EFSS_SUBMISSION_NO", "%" + model.EfssNo.Trim().ToUpper() + "%");
            }
            if (model.Status.Equals("0")) // all
            {

            }
            else if (model.Status.Equals("1")) // submitted
            {
                whereQ += "\r\n\t AND MAP_DSN IS NOT NULL";
            }
            else if (model.Status.Equals("2")) // not yet submitted
            {
                whereQ += "\r\n\t AND MAP_DSN IS NULL";
            }

            return whereQ;
        }

        public string ExportSCUR_EA(Fn01SCUR_EASearchModel model)
        {
            model.Query = SearchSCUR_EA_q;
            model.QueryWhere = SearchSCUR_EA_whereQ(model);

            return model.Export("E-form Application Report");
        }


        public B_EFSS_SUBMISSION_MAP createEfssSubmissionMap(string EFSS_ID, SCUR_Models model)
        {
            using(EntitiesSignboard db = new EntitiesSignboard())
            {
                SignboardCommentService SignboardCommentService = new SignboardCommentService();

                B_SV_SUBMISSION sv_submission = db.B_SV_SUBMISSION.Where(x => x.REFERENCE_NO == model.SubmissionNo && x.FORM_CODE == model.FormCode).FirstOrDefault();

                B_EFSS_SUBMISSION_MAP map = new B_EFSS_SUBMISSION_MAP();
                map.EFSS_SUBMISSION_NO = EFSS_ID;
                map.SMM_REFERENCE_NO = model.SubmissionNo; // B_SV_SUBMISSION: REFERNCE_NO
                map.DSN = db.B_SV_SCANNED_DOCUMENT.Where(x => x.UUID == sv_submission.SV_SCANNED_DOCUMENT_ID).FirstOrDefault().DSN_NUMBER;

                db.B_EFSS_SUBMISSION_MAP.Add(map);
                db.SaveChanges();

                return map;
            }
        }

        public void dataEntry_SC01(string EFSS_ID, SCUR_Models scurModel)
        {
            using(EntitiesSignboard db = new EntitiesSignboard())
            {
                B_EFSS_FORM_MASTER master = db.B_EFSS_FORM_MASTER.Find(EFSS_ID);
                B_EFSCU_TBL_SC01 detail = db.B_EFSCU_TBL_SC01.Find(master.FORMCONTENTID);
                List<B_EFSCU_TBL_SC01_ITEM> items = db.B_EFSCU_TBL_SC01_ITEM.Where(x => x.SC01ID == detail.ID).ToList();
                List<B_EFSS_FORM_ATTACHMENTS> files = db.B_EFSS_FORM_ATTACHMENTS.Where(x => x.RECVFORMID == master.ID).ToList();

                B_SV_SUBMISSION sv_submission = db.B_SV_SUBMISSION.Where(x => x.REFERENCE_NO == scurModel.SubmissionNo && x.FORM_CODE == scurModel.FormCode)
                    .Include(x => x.B_SV_SCANNED_DOCUMENT)
                    .FirstOrDefault();

                SignboardCommonDAOService ss = new SignboardCommonDAOService();

                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region signboard owner & address
                        B_SV_ADDRESS owner_addr = new B_SV_ADDRESS();
                        if (detail.SOCORRADDRSAMEASOPADDR.ToString().Equals("1")) // 1: coor addr same as sb addr
                        {
                            owner_addr.FLAT = detail.UNAUTHSIGNBOARDFLAT;
                            owner_addr.FLOOR = detail.UNAUTHSIGNBOARDFLOOR;
                            owner_addr.BUILDINGNAME = detail.BUILDING;
                            owner_addr.STREET = detail.STREETROADVILLAGE;
                            owner_addr.STREET_NO = detail.STREETNO;
                            owner_addr.DISTRICT = detail.UNAUTHSIGNBOARDDISTRICT;
                            owner_addr.REGION = detail.UNAUTHSIGNBOARDAREA;
                            //owner_addr.FULL_ADDRESS = ;
                            owner_addr.FILE_REFERENCE_NO = master.FOURPLUSTWO;
                            //owner_addr.BCIS_DISTRICT = ;
                        }
                        else
                        {
                            owner_addr.FLAT = detail.SOCORRFLAT;
                            owner_addr.FLOOR = detail.SOCORRFLOOR;
                            owner_addr.BUILDINGNAME = detail.SOCORRBUILDING;
                            owner_addr.STREET = detail.SOCORRSTREET;
                            owner_addr.STREET_NO = detail.SOCORRSTREETNO;
                            owner_addr.DISTRICT = detail.SOCORRDISTRICT;
                            owner_addr.REGION = detail.SOCORRAREA;
                            //owner_addr.FULL_ADDRESS = ;
                            owner_addr.FILE_REFERENCE_NO = master.FOURPLUSTWO;
                            //owner_addr.BCIS_DISTRICT = ;
                        }
                        

                        db.B_SV_ADDRESS.Add(owner_addr);
                        db.SaveChanges();

                        B_SV_PERSON_CONTACT owner = new B_SV_PERSON_CONTACT();
                        owner.NAME_CHINESE = detail.SOCHINAMEOFBUSINESS;
                        owner.NAME_ENGLISH = detail.SOENGNAMEOFBUSINESS1 + " " + detail.SOENGNAMEOFBUSINESS2;
                        owner.EMAIL = detail.EMAIL;
                        owner.CONTACT_NO = detail.SOTEL;
                        owner.FAX_NO = detail.SOFAX;
                        owner.ID_TYPE = "3"; // BR #
                        owner.ID_NUMBER = detail.SOBRN;
                        //owner.ID_ISSUE_COUNTRY = ;
                        //owner.CONTACT_PERSON_TITLE = ;
                        //owner.FIRST_NAME = ;
                        //owner.LAST_NAME = ;
                        //owner.MOBILE = ;
                        //owner.SIGNATURE_DATE = ;
                        //owner.PRC_NAME = ;
                        //owner.PRC_CONTACT_NO = ;
                        //owner.PBP_NAME;
                        //owner.PBP_CONTACT_NO = ;
                        //owner.PAW_SAME_AS;

                        // FK
                        owner.SV_ADDRESS_ID = owner_addr.UUID;
                        db.B_SV_PERSON_CONTACT.Add(owner);
                        db.SaveChanges();

                        #endregion

                        #region paw & address
                        B_SV_ADDRESS paw_addr = new B_SV_ADDRESS();
                        //paw_addr.FLAT = 
                        //paw_addr.FLOOR = 
                        //paw_addr.BUILDINGNAME = 
                        //paw_addr.STREET = 
                        //paw_addr.STREET_NO =
                        //paw_addr.DISTRICT = 
                        //paw_addr.REGION = 


                        db.B_SV_ADDRESS.Add(paw_addr);
                        db.SaveChanges();

                        B_SV_PERSON_CONTACT paw = new B_SV_PERSON_CONTACT();
                        paw.NAME_CHINESE = detail.ARRNAME;
                        //paw.NAME_ENGLISH = 
                        //paw.EMAIL = 
                        //paw.CONTACT_NO = 
                        //paw.FAX_NO = 
                        //paw.ID_TYPE = 
                        //paw.ID_NUMBER = 
                        //paw.ID_ISSUE_COUNTRY
                        //paw.CONTACT_PERSON_TITLE = 
                        //paw.FIRST_NAME = 
                        //paw.LAST_NAME = 
                        //paw.MOBILE = 
                        paw.SIGNATURE_DATE = detail.ARRSIGNDATE;
                        //paw.PRC_NAME = 
                        //paw.PRC_CONTACT_NO = 
                        //paw.PBP_NAME = 
                        //paw.PBP_CONTACT_NO = 
                        string paw_same_as = "";
                        if(detail.ARRISAPOPTION.ToString().Equals("1"))
                        {
                            paw_same_as = SignboardConstant.CODE_PBP;
                        }
                        else if(detail.ARRISRGBCOPTION.ToString().Equals("1"))
                        {
                            paw_same_as = SignboardConstant.CODE_GBC;
                        }
                        else if(detail.ARRISRMWCTYPECOPTION.ToString().Equals("1"))
                        {
                            paw_same_as = SignboardConstant.PRC_CODE;
                        }
                        else if(detail.ARRISRSEOPTION.ToString().Equals("1"))
                        {
                            paw_same_as = SignboardConstant.CODE_PBP;
                        }
                        else if(detail.ARRISSOOPTION.ToString().Equals("1"))
                        {
                            paw_same_as = SignboardConstant.CODE_SB_OWNER;
                        }
                        paw.PAW_SAME_AS = paw_same_as;


                        // FK
                        paw.SV_ADDRESS_ID = paw_addr.UUID;
                        db.B_SV_PERSON_CONTACT.Add(paw);
                        db.SaveChanges();

                        #endregion

                        #region oi & address
                        B_SV_ADDRESS oi_addr = new B_SV_ADDRESS();
                        B_SV_PERSON_CONTACT oi = new B_SV_PERSON_CONTACT();
                        if (detail.OWNERCORPFORMEDOPTION.ToString().Equals("1"))
                        {
                            //oi_addr.BLOCK = 
                            oi_addr.STREET = detail.OWNERCORPSTREET;
                            oi_addr.STREET_NO = detail.OWNERCORPSTREETNO;
                            oi_addr.BUILDINGNAME = detail.OWNERCORPBUILDING;
                            oi_addr.FLOOR = detail.OWNERCORPFLOOR;
                            oi_addr.FLAT = detail.OWNERCORPFLAT;
                            oi_addr.DISTRICT = detail.OWNERCORPDISTRICT;
                            oi_addr.REGION = detail.OWNERCORPAREA;
                            db.B_SV_ADDRESS.Add(oi_addr);
                            db.SaveChanges();

                            
                            //oi.NAME_CHINESE = 
                            //oi.NAME_ENGLISH = 
                            //oi.EMAIL = 
                            //oi.CONTACT_NO = 
                            //oi.FAX_NO = 
                            //oi.ID_TYPE = 
                            //oi.ID_NUMBER = 
                            //oi.ID_ISSUE_COUNTRY = 
                            //oi.CONTACT_PERSON_TITLE = 
                            //oi.FIRST_NAME = 
                            //oi.LAST_NAME = 
                            //oi.MOBILE = 
                            //oi.SIGNATURE_DATE = 
                            //oi.PRC_NAME = 
                            //oi.PRC_CONTACT_NO = 
                            //oi.PBP_NAME = 
                            //oi.PBP_CONTACT_NO = 
                            //oi.PAW_SAME_AS = 

                            // FK
                            oi.SV_ADDRESS_ID = oi_addr.UUID;

                            db.B_SV_PERSON_CONTACT.Add(oi);
                            db.SaveChanges();
                        }
                        #endregion

                        #region signboard & address
                        B_SV_ADDRESS signboard_addr = new B_SV_ADDRESS();
                        signboard_addr.FLAT = detail.UNAUTHSIGNBOARDFLAT;
                        signboard_addr.FLOOR = detail.UNAUTHSIGNBOARDFLOOR;
                        signboard_addr.BUILDINGNAME = detail.BUILDING;
                        signboard_addr.STREET_NO = detail.STREETNO;
                        signboard_addr.STREET = detail.STREETROADVILLAGE;
                        signboard_addr.DISTRICT = detail.UNAUTHSIGNBOARDDISTRICT;
                        signboard_addr.REGION = detail.UNAUTHSIGNBOARDAREA;
                        //signboard_addr.FULL_ADDRESS = detail.;

                        db.B_SV_ADDRESS.Add(signboard_addr);
                        db.SaveChanges();

                        B_SV_SIGNBOARD sv_signboard = new B_SV_SIGNBOARD();
                        sv_signboard.LOCATION_OF_SIGNBOARD = detail.UNAUTHSIGNBOARDDETAILEDLOCATION;
                        //sv_signboard.RVD_NO = ;
                        //sv_signboard.FACADE = ;
                        //sv_signboard.TYPE = ;
                        //sv_signboard.BTM_FLOOR = ;
                        //sv_signboard.TOP_FLOOR = ;
                        //sv_signboard.A_M2 = ;
                        //sv_signboard.P_M = ;
                        //sv_signboard.H_M = ;
                        //sv_signboard.H2_M = ;
                        //sv_signboard.LED = ;
                        //sv_signboard.BUILDING_PORTION = ;
                        //sv_signboard.STATUS = ;
                        //sv_signboard.DESCRIPTION = ;
                        //sv_signboard.S24_ORDER_NO = ;
                        //sv_signboard.S24_ORDER_TYPE = ;

                        // FK
                        sv_signboard.LOCATION_ADDRESS_ID = signboard_addr.UUID;
                        sv_signboard.OWNER_ID = owner.UUID;

                        db.B_SV_SIGNBOARD.Add(sv_signboard);
                        db.SaveChanges();


                        #endregion

                        #region sv_record
                        B_SV_RECORD sv_record = new B_SV_RECORD();
                        //sv_record.TO_USER_ID;
                        //sv_record.PO_USER_ID;
                        //sv_record.SPO_USER_ID;

                        //sv_record.AREA_CODE;
                        //sv_record.RECOMMENDATION;
                        //sv_record.NO_OF_SIGNBOARD_VALIDATED;
                        //sv_record.NO_OF_SIGNBOARD_INVOLVED;
                        //sv_record.LSO_VS_OPERATION_YEAR;
                        //sv_record.PREVIOUS_SUBMISSION_NUMBER;
                        //sv_record.NO_OF_SUBMISSION_ERECTION;
                        //sv_record.NO_OF_SUBMISSION_REMOVAL;

                        //sv_record.VALIDITY_AP;
                        //sv_record.SIGNATURE_AP;
                        //sv_record.ITEM_STATED;
                        //sv_record.VALIDITY_PRC;
                        //sv_record.SIGNATURE_AS;
                        //sv_record.INFO_SIGNBOARD_OWNER_PROVIDED;
                        //sv_record.OTHER_IRREGULARITIES;

                        sv_record.RECEIVED_DATE = master.CREATEDDATE;
                        sv_record.REFERENCE_NO = sv_submission.REFERENCE_NO;
                        //sv_record.LANGUAGE_CODE = SignboardConstant.LANG_CHINESE;
                        sv_record.WITH_ALTERATION = detail.ASWORKOPTION.ToString();
                        sv_record.INSPECTION_DATE = detail.INSPECTIONDATE;
                        //sv_record.PROPOSED_ALTERATION_COMM_DATE;
                        sv_record.ACTUAL_ALTERATION_COMM_DATE = detail.ASDATE;
                        //sv_record.ACTUAL_ALTERATION_COMP_DATE;
                        //sv_record.VALIDATION_EXPIRY_DATE;
                        //sv_record.SIGNBOARD_REMOVAL;
                        //sv_record.SIGNBOARD_REMOVAL_DISCOV_DATE;

                        //sv_record.S_CHK_VS_NO = ;
                        //sv_record.S_CHK_INSP_DATE = ;
                        //sv_record.S_CHK_WORK_DATE = ;
                        //sv_record.S_CHK_SIGNBOARD = ;
                        //sv_record.S_CHK_SIG = ;
                        //sv_record.S_CHK_SIG_DATE = ;
                        //sv_record.S_CHK_MW_ITEM_NO = ;
                        //sv_record.S_CHK_SUPPORT_DOC = ;
                        //sv_record.S_CHK_SBO_PWA_AP =;
                        //sv_record.S_CHK_OTHERS = ;

                        //sv_record.P_CHK_APP_AP_MW_ITEM =;
                        //sv_record.P_CHK_VAL_AP = ; 
                        //sv_record.P_CHK_SIG_AP = ;
                        //sv_record.P_CHK_VAL_RSE = ;
                        //sv_record.P_CHK_SIG_RSE = ;
                        //sv_record.P_CHK_VAL_RI = ;
                        //sv_record.P_CHK_SIG_RI = ;
                        //sv_record.P_CHK_VAL_PRC = ;
                        //sv_record.P_CHK_SIG_AS = ;
                        //sv_record.P_CHK_CAP_AS_MW_ITEM = ;

                        //sv_record.ACK_LETTERISS_DATE;

                        sv_record.STATUS_CODE = SignboardConstant.SV_RECORD_STATUS_CODE_DATA_ENTRY_DRAFT;

                        // FK
                        sv_record.SV_SUBMISSION_ID = sv_submission.UUID;
                        sv_record.SV_SIGNBOARD_ID = sv_signboard.UUID;
                        sv_record.PAW_ID = paw.UUID;
                        sv_record.OI_ID = oi.UUID;

                        db.B_SV_RECORD.Add(sv_record);
                        db.SaveChanges();
                        #endregion

                        #region professional
                        B_SV_APPOINTED_PROFESSIONAL ap = new B_SV_APPOINTED_PROFESSIONAL();
                        ap.CERTIFICATION_NO = "AP(" + detail.APCFMCRNUM1 + ") " + detail.APCFMCRNUM2 + "/" + detail.APCFMCRNUM3;
                        ap.CHINESE_NAME = detail.APCFMCHINAME;
                        ap.ENGLISH_NAME = detail.APCFMENGNAME;
                        //ap.AS_CHINESE_NAME;
                        //ap.AS_ENGLISH_NAME;
                        //ap.STATUS_CODE;
                        ap.EXPIRY_DATE = detail.APCFMSIGNEXPIREDDATE;
                        ap.SIGNATURE_DATE = detail.APCFMSIGNDATE;
                        //ap.CONTACT_NO;
                        //ap.FAX_NO;
                        ap.IDENTIFY_FLAG = SignboardConstant.PBP_CODE_AP;

                        // FK
                        ap.SV_RECORD_ID = sv_record.UUID;
                        db.B_SV_APPOINTED_PROFESSIONAL.Add(ap);
                        db.SaveChanges();


                        B_SV_APPOINTED_PROFESSIONAL prc = new B_SV_APPOINTED_PROFESSIONAL();
                        prc.CERTIFICATION_NO = detail.PRCCFMCRNUM1 + "/" + detail.PRCCFMCRNUM2;
                        prc.CHINESE_NAME = detail.PRCCFMCHINAME;
                        prc.ENGLISH_NAME = detail.PRCCFMENGNAME1 + " " + detail.PRCCFMENGNAME2;
                        prc.AS_CHINESE_NAME = detail.PRCASCFMCHINAME;
                        prc.AS_ENGLISH_NAME = detail.PRCASCFMENGNAME;
                        //prc.STATUS_CODE;
                        prc.EXPIRY_DATE = detail.PRCCFMCREXPIREDDATE;
                        prc.SIGNATURE_DATE = detail.PRCCFMSIGNDATE;
                        //prc.CONTACT_NO;
                        //prc.FAX_NO;
                        prc.IDENTIFY_FLAG = SignboardConstant.PRC_CODE;

                        // FK
                        prc.SV_RECORD_ID = sv_record.UUID;
                        db.B_SV_APPOINTED_PROFESSIONAL.Add(prc);
                        db.SaveChanges();

                        B_SV_APPOINTED_PROFESSIONAL rse = new B_SV_APPOINTED_PROFESSIONAL();
                        rse.CERTIFICATION_NO = "RSE " + detail.RSECFMCRNUM1 + "/" + detail.RSECFMCRNUM2;
                        rse.CHINESE_NAME = detail.RSECFMCHINAME;
                        rse.ENGLISH_NAME = detail.RSECFMENGNAME;
                        //rse.AS_CHINESE_NAME = ;
                        //rse.AS_ENGLISH_NAME;
                        //rse.STATUS_CODE;
                        rse.EXPIRY_DATE = detail.RSECFMCREXPIREDDATE;
                        rse.SIGNATURE_DATE = detail.RSECFMSIGNDATE;
                        //rse.CONTACT_NO;
                        //rse.FAX_NO;
                        rse.IDENTIFY_FLAG = SignboardConstant.PBP_CODE_RSE;
                        
                        // FK
                        rse.SV_RECORD_ID = sv_record.UUID;
                        db.B_SV_APPOINTED_PROFESSIONAL.Add(rse);
                        db.SaveChanges();

                        B_SV_APPOINTED_PROFESSIONAL rge = new B_SV_APPOINTED_PROFESSIONAL();
                        rge.IDENTIFY_FLAG = SignboardConstant.PBP_CODE_RGE;
                        rge.SV_RECORD_ID = sv_record.UUID;
                        db.B_SV_APPOINTED_PROFESSIONAL.Add(rge);
                        db.SaveChanges();

                        #endregion

                        #region mw items
                        var count = 0;
                        foreach(var item in items)
                        {
                            B_SV_RECORD_ITEM mwitem = new B_SV_RECORD_ITEM();
                            mwitem.MW_ITEM_CODE = item.MWITEMNO;
                            mwitem.LOCATION_DESCRIPTION = item.MWDESC;
                            mwitem.CLASS_CODE = SignboardCommentService.getClassCode(item.MWITEMNO);
                            mwitem.ORDERING = new decimal(count);

                            // FK
                            mwitem.SV_RECORD_ID = sv_record.UUID;
                            db.B_SV_RECORD_ITEM.Add(mwitem);
                            db.SaveChanges();
                            count++;
                        }
                        #endregion

                        #region validation items
                        var counter = 0;
                        // 1(a)
                        if(detail.INSPECTEDUNAUTHSIGNBOARDITEM1A != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM1A.ToString().Equals("1"))
                        {
                            for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM1AQTY; i++)
                            {
                                B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                                validation.VALIDATION_ITEM = "1(a)";
                                validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                                validation.ORDERING = new decimal(counter);

                                // FK
                                validation.SV_RECORD_ID = sv_record.UUID;

                                db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                                db.SaveChanges();
                                counter++;
                            }
                        }
                        // 1(b)
                        if(detail.INSPECTEDUNAUTHSIGNBOARDITEM1B != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM1B.ToString().Equals("1"))
                        {
                            for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM1BQTY; i++)
                            {
                                B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                                validation.VALIDATION_ITEM = "1(b)";
                                validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                                validation.ORDERING = new decimal(counter);

                                // FK
                                validation.SV_RECORD_ID = sv_record.UUID;
                                db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                                db.SaveChanges();
                                counter++;
                            }
                        }
                        // 1(c)
                        if (detail.INSPECTEDUNAUTHSIGNBOARDITEM1C != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM1C.ToString().Equals("1"))
                        {
                            for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM1CQTY; i++)
                            {
                                B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                                validation.VALIDATION_ITEM = "1(c)";
                                validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                                validation.ORDERING = new decimal(counter);
                                

                                // FK
                                validation.SV_RECORD_ID = sv_record.UUID;
                                db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                                db.SaveChanges();
                                counter++;
                            }
                        }
                        // 2(a)
                        if(detail.INSPECTEDUNAUTHSIGNBOARDITEM2A != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM2A.ToString().Equals("1"))
                        {
                            for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM2AQTY; i++)
                            {
                                B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                                validation.VALIDATION_ITEM = "2(a)";
                                validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                                validation.ORDERING = new decimal(counter);

                                // FK
                                validation.SV_RECORD_ID = sv_record.UUID;
                                db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                                db.SaveChanges();
                                counter++;
                            }
                        }
                        // 2(b)
                        if(detail.INSPECTEDUNAUTHSIGNBOARDITEM2B != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM2B.ToString().Equals("1"))
                        {
                            for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM2BQTY; i++)
                            {
                                B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                                validation.VALIDATION_ITEM = "2(b)";
                                validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                                validation.ORDERING = new decimal(counter);

                                // FK
                                validation.SV_RECORD_ID = sv_record.UUID;
                                db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                                db.SaveChanges();
                                counter++;
                            }
                        }
                        // 2(c)
                        if(detail.INSPECTEDUNAUTHSIGNBOARDITEM2C != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM2C.ToString().Equals("1")) {
                            for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM2CQTY; i++)
                            {
                                B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                                validation.VALIDATION_ITEM = "2(c)";
                                validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                                validation.ORDERING = new decimal(counter);

                                // FK
                                validation.SV_RECORD_ID = sv_record.UUID;
                                db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                                db.SaveChanges();
                                counter++;
                            }
                        }
                        // 3
                        if(detail.INSPECTEDUNAUTHSIGNBOARDITEM3 != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM3.ToString().Equals("1"))
                        {
                            for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM3QTY; i++)
                            {
                                B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                                validation.VALIDATION_ITEM = "3";
                                validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                                validation.ORDERING = new decimal(counter);

                                // FK
                                validation.SV_RECORD_ID = sv_record.UUID;
                                db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                                db.SaveChanges();
                                counter++;
                            }
                        }
                        // 4(a)
                        if(detail.INSPECTEDUNAUTHSIGNBOARDITEM4A != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM4A.ToString().Equals("1"))
                        {
                            for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM4AQTY; i++)
                            {
                                B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                                validation.VALIDATION_ITEM = "4(a)";
                                validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                                validation.ORDERING = new decimal(counter);

                                // FK
                                validation.SV_RECORD_ID = sv_record.UUID;
                                db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                                db.SaveChanges();
                                counter++;
                            }
                        }
                        // 4(b)
                        if(detail.INSPECTEDUNAUTHSIGNBOARDITEM4B != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM4B.ToString().Equals("1"))
                        {
                            for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM4BQTY; i++)
                            {
                                B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                                validation.VALIDATION_ITEM = "4(b)";
                                validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                                validation.ORDERING = new decimal(counter);

                                // FK
                                validation.SV_RECORD_ID = sv_record.UUID;
                                db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                                db.SaveChanges();
                                counter++;
                            }
                        }
                        // 5
                        if(detail.INSPECTEDUNAUTHSIGNBOARDITEM5 != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM5.ToString().Equals("1"))
                        {
                            for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM5QTY; i++)
                            {
                                B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                                validation.VALIDATION_ITEM = "5";
                                validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                                validation.ORDERING = new decimal(counter);

                                // FK
                                validation.SV_RECORD_ID = sv_record.UUID;
                                db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                                db.SaveChanges();
                                counter++;
                            }
                        }
                        // 6
                        if(detail.INSPECTEDUNAUTHSIGNBOARDITEM6 != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM6.ToString().Equals("1"))
                        {
                            for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM6QTY; i++)
                            {
                                B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                                validation.VALIDATION_ITEM = "6";
                                validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                                validation.ORDERING = new decimal(counter);

                                // FK
                                validation.SV_RECORD_ID = sv_record.UUID;
                                db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                                db.SaveChanges();
                                counter++;
                            }
                        }
                        #endregion

                        #region file upload
                        // form
                        B_SV_SCANNED_DOCUMENT sd = db.B_SV_SCANNED_DOCUMENT.Find(sv_submission.SV_SCANNED_DOCUMENT_ID);
                        string fileName = master.FILENAME;
                        string fileExtension = master.FILEEXTENSION;
                        //string DSN = ss.GetNumber();
                        string DSN = sd.DSN_NUMBER;
                        string directory = ApplicationConstant.SMMFilePath + "smm_scan" + getSvDocRelativeFilePath(DSN);
                        string relativePath = getSvDocRelativeFilePath(DSN) + fileName + fileExtension;
                        string fullPath = ApplicationConstant.SMMFilePath + "smm_scan" + relativePath;
                        if(!Directory.Exists(directory))
                        {
                            System.IO.Directory.CreateDirectory(directory);
                        }
                        File.WriteAllBytes(fullPath, master.FILECONTENT);

                        sd.FILE_PATH = fullPath;
                        sd.RELATIVE_FILE_PATH = relativePath;
                        sd.RECORD_ID = sv_submission.REFERENCE_NO;
                        sd.DSN_NUMBER = DSN;
                        sd.RECORD_TYPE = SignboardConstant.SCAN_DOC_RECORD_TYPE_VALIDATION;
                        sd.DOCUMENT_TYPE = "Form";
                        //sd.FOLDER_TYPE = SignboardConstant.SCANNED_DOC_FOLDER_TYPE_PRIVATE_SCU;
                        sd.SUBMIT_TYPE = SignboardConstant.SV_Validation;

                        db.SaveChanges();


                        foreach (var file in files)
                        {
                            string file_name = file.FILENAME;
                            string file_extension = file.FILEEXTENSION;
                            string DSN_no = ss.GetNumber();
                            string file_directory = ApplicationConstant.SMMFilePath + "smm_scan" + getSvDocRelativeFilePath(DSN_no);
                            string relative_path = getSvDocRelativeFilePath(DSN_no) + file_name + file_extension;
                            string full_path = ApplicationConstant.SMMFilePath + "smm_scan" + relative_path;
                            if (!Directory.Exists(file_directory))
                            {
                                System.IO.Directory.CreateDirectory(file_directory);
                            }
                            File.WriteAllBytes(full_path, file.FILECONTENT);

                            B_SV_SCANNED_DOCUMENT doc = new B_SV_SCANNED_DOCUMENT();
                            //doc.AS_THUMBNAIL = "N";
                            doc.FILE_PATH = full_path;
                            sd.RELATIVE_FILE_PATH = relative_path;
                            doc.RECORD_ID = sv_submission.REFERENCE_NO;
                            doc.DSN_NUMBER = DSN_no;
                            doc.RECORD_TYPE = SignboardConstant.SCAN_DOC_RECORD_TYPE_VALIDATION;
                            doc.DOCUMENT_TYPE = file.DOCTYPE;
                            doc.FOLDER_TYPE = file.ATTTYPE;
                            doc.SUBMIT_TYPE = SignboardConstant.SV_Validation;
                            

                            db.B_SV_SCANNED_DOCUMENT.Add(doc);
                            db.SaveChanges();
                        }
                        #endregion

                        transaction.Commit();
                    }
                    catch(Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }

        public void dataEntry_SC02(string EFSS_ID, SCUR_Models scurModel)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                B_EFSS_FORM_MASTER master = db.B_EFSS_FORM_MASTER.Find(EFSS_ID);
                B_EFSCU_TBL_SC02 detail = db.B_EFSCU_TBL_SC02.Find(master.FORMCONTENTID);
                List<B_EFSCU_TBL_SC02_ITEM> items = db.B_EFSCU_TBL_SC02_ITEM.Where(x => x.SC02ID == detail.ID).ToList();
                List<B_EFSS_FORM_ATTACHMENTS> files = db.B_EFSS_FORM_ATTACHMENTS.Where(x => x.RECVFORMID == master.ID).ToList();

                B_SV_SUBMISSION sv_submission = db.B_SV_SUBMISSION.Where(x => x.REFERENCE_NO == scurModel.SubmissionNo && x.FORM_CODE == scurModel.FormCode)
                    .Include(x => x.B_SV_SCANNED_DOCUMENT)
                    .FirstOrDefault();

                SignboardCommonDAOService ss = new SignboardCommonDAOService();

                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region signboard owner & address
                        B_SV_ADDRESS owner_addr = new B_SV_ADDRESS();
                        if (detail.SOCORRADDRSAMEASOPADDR.ToString().Equals("1")) // 1: corr addr same as sb addr
                        {
                            owner_addr.FLAT = detail.UNAUTHSIGNBOARDFLAT;
                            owner_addr.FLOOR = detail.UNAUTHSIGNBOARDFLOOR;
                            owner_addr.BUILDINGNAME = detail.BUILDING;
                            owner_addr.STREET = detail.STREETROADVILLAGE;
                            owner_addr.STREET_NO = detail.STREETNO;
                            owner_addr.DISTRICT = detail.UNAUTHSIGNBOARDDISTRICT;
                            owner_addr.REGION = detail.UNAUTHSIGNBOARDAREA;
                            //owner_addr.FULL_ADDRESS = ;
                            owner_addr.FILE_REFERENCE_NO = master.FOURPLUSTWO;
                            //owner_addr.BCIS_DISTRICT = ;
                        }
                        else
                        {
                            owner_addr.FLAT = detail.SOCORRFLAT;
                            owner_addr.FLOOR = detail.SOCORRFLOOR;
                            owner_addr.BUILDINGNAME = detail.SOCORRBUILDING;
                            owner_addr.STREET = detail.SOCORRSTREET;
                            owner_addr.STREET_NO = detail.SOCORRSTREETNO;
                            owner_addr.DISTRICT = detail.SOCORRDISTRICT;
                            owner_addr.REGION = detail.SOCORRAREA;
                            //owner_addr.FULL_ADDRESS = ;
                            owner_addr.FILE_REFERENCE_NO = master.FOURPLUSTWO;
                            //owner_addr.BCIS_DISTRICT = ;
                        }
                        

                        db.B_SV_ADDRESS.Add(owner_addr);
                        db.SaveChanges();

                        B_SV_PERSON_CONTACT owner = new B_SV_PERSON_CONTACT();
                        owner.NAME_CHINESE = detail.SOCHINAMEOFBUSINESS;
                        owner.NAME_ENGLISH = detail.SOENGNAMEOFBUSINESS1 + " " + detail.SOENGNAMEOFBUSINESS2;
                        owner.EMAIL = detail.EMAIL;
                        owner.CONTACT_NO = detail.SOTEL;
                        owner.FAX_NO = detail.SOFAX;
                        owner.ID_TYPE = "3"; // BR #
                        owner.ID_NUMBER = detail.SOBRN;
                        //owner.ID_ISSUE_COUNTRY = ;
                        //owner.CONTACT_PERSON_TITLE = ;
                        //owner.FIRST_NAME = ;
                        //owner.LAST_NAME = ;
                        //owner.MOBILE = ;
                        //owner.SIGNATURE_DATE = ;
                        //owner.PRC_NAME = ;
                        //owner.PRC_CONTACT_NO = ;
                        //owner.PBP_NAME;
                        //owner.PBP_CONTACT_NO = ;
                        //owner.PAW_SAME_AS;

                        // FK
                        owner.SV_ADDRESS_ID = owner_addr.UUID;
                        db.B_SV_PERSON_CONTACT.Add(owner);
                        db.SaveChanges();

                        #endregion

                        #region paw & address
                        B_SV_ADDRESS paw_addr = new B_SV_ADDRESS();
                        //paw_addr.FLAT = 
                        //paw_addr.FLOOR = 
                        //paw_addr.BUILDINGNAME = 
                        //paw_addr.STREET = 
                        //paw_addr.STREET_NO =
                        //paw_addr.DISTRICT = 
                        //paw_addr.REGION = 


                        db.B_SV_ADDRESS.Add(paw_addr);
                        db.SaveChanges();

                        B_SV_PERSON_CONTACT paw = new B_SV_PERSON_CONTACT();
                        paw.NAME_CHINESE = detail.ARRNAME;
                        //paw.NAME_ENGLISH = 
                        //paw.EMAIL = 
                        //paw.CONTACT_NO = 
                        //paw.FAX_NO = 
                        //paw.ID_TYPE = 
                        //paw.ID_NUMBER = 
                        //paw.ID_ISSUE_COUNTRY
                        //paw.CONTACT_PERSON_TITLE = 
                        //paw.FIRST_NAME = 
                        //paw.LAST_NAME = 
                        //paw.MOBILE = 
                        paw.SIGNATURE_DATE = detail.ARRSIGNDATE;
                        //paw.PRC_NAME = 
                        //paw.PRC_CONTACT_NO = 
                        //paw.PBP_NAME = 
                        //paw.PBP_CONTACT_NO = 
                        string paw_same_as = "";
                        if (detail.ARRISAPOPTION.ToString().Equals("1"))
                        {
                            paw_same_as = SignboardConstant.CODE_PBP;
                        }
                        else if (detail.ARRISRGBCOPTION.ToString().Equals("1"))
                        {
                            paw_same_as = SignboardConstant.CODE_GBC;
                        }
                        else if (detail.ARRISRMWCTYPECOPTION.ToString().Equals("1"))
                        {
                            paw_same_as = SignboardConstant.PRC_CODE;
                        }
                        else if (detail.ARRISRSEOPTION.ToString().Equals("1"))
                        {
                            paw_same_as = SignboardConstant.CODE_PBP;
                        }
                        else if (detail.ARRISSOOPTION.ToString().Equals("1"))
                        {
                            paw_same_as = SignboardConstant.CODE_SB_OWNER;
                        }
                        paw.PAW_SAME_AS = paw_same_as;


                        // FK
                        paw.SV_ADDRESS_ID = paw_addr.UUID;
                        db.B_SV_PERSON_CONTACT.Add(paw);
                        db.SaveChanges();

                        #endregion

                        #region oi & address
                        B_SV_ADDRESS oi_addr = new B_SV_ADDRESS();
                        B_SV_PERSON_CONTACT oi = new B_SV_PERSON_CONTACT();
                        if (detail.OWNERCORPFORMEDOPTION.ToString().Equals("1"))
                        {
                            //oi_addr.BLOCK = 
                            oi_addr.STREET = detail.OWNERCORPSTREET;
                            oi_addr.STREET_NO = detail.OWNERCORPSTREETNO;
                            oi_addr.BUILDINGNAME = detail.OWNERCORPBUILDING;
                            oi_addr.FLOOR = detail.OWNERCORPFLOOR;
                            oi_addr.FLAT = detail.OWNERCORPFLAT;
                            oi_addr.DISTRICT = detail.OWNERCORPDISTRICT;
                            oi_addr.REGION = detail.OWNERCORPAREA;

                            //oi.NAME_CHINESE = 
                            //oi.NAME_ENGLISH = 
                            //oi.EMAIL = 
                            //oi.CONTACT_NO = 
                            //oi.FAX_NO = 
                            //oi.ID_TYPE = 
                            //oi.ID_NUMBER = 
                            //oi.ID_ISSUE_COUNTRY = 
                            //oi.CONTACT_PERSON_TITLE = 
                            //oi.FIRST_NAME = 
                            //oi.LAST_NAME = 
                            //oi.MOBILE = 
                            //oi.SIGNATURE_DATE = 
                            //oi.PRC_NAME = 
                            //oi.PRC_CONTACT_NO = 
                            //oi.PBP_NAME = 
                            //oi.PBP_CONTACT_NO = 
                            //oi.PAW_SAME_AS = 
                        }
                        // FK
                        db.B_SV_ADDRESS.Add(oi_addr);
                        db.SaveChanges();

                        oi.SV_ADDRESS_ID = oi_addr.UUID;
                        db.B_SV_PERSON_CONTACT.Add(oi);
                        db.SaveChanges();
                        #endregion

                        #region signboard & address
                        B_SV_ADDRESS signboard_addr = new B_SV_ADDRESS();
                        signboard_addr.FLAT = detail.UNAUTHSIGNBOARDFLAT;
                        signboard_addr.FLOOR = detail.UNAUTHSIGNBOARDFLOOR;
                        signboard_addr.BUILDINGNAME = detail.BUILDING;
                        signboard_addr.STREET_NO = detail.STREETNO;
                        signboard_addr.STREET = detail.STREETROADVILLAGE;
                        signboard_addr.DISTRICT = detail.UNAUTHSIGNBOARDDISTRICT;
                        signboard_addr.REGION = detail.UNAUTHSIGNBOARDAREA;
                        //signboard_addr.FULL_ADDRESS = detail.;

                        db.B_SV_ADDRESS.Add(signboard_addr);
                        db.SaveChanges();

                        B_SV_SIGNBOARD sv_signboard = new B_SV_SIGNBOARD();
                        sv_signboard.LOCATION_OF_SIGNBOARD = detail.UNAUTHSIGNBOARDDETAILEDLOCATION;
                        //sv_signboard.RVD_NO = ;
                        //sv_signboard.FACADE = ;
                        //sv_signboard.TYPE = ;
                        //sv_signboard.BTM_FLOOR = ;
                        //sv_signboard.TOP_FLOOR = ;
                        //sv_signboard.A_M2 = ;
                        //sv_signboard.P_M = ;
                        //sv_signboard.H_M = ;
                        //sv_signboard.H2_M = ;
                        //sv_signboard.LED = ;
                        //sv_signboard.BUILDING_PORTION = ;
                        //sv_signboard.STATUS = ;
                        //sv_signboard.DESCRIPTION = ;
                        //sv_signboard.S24_ORDER_NO = ;
                        //sv_signboard.S24_ORDER_TYPE = ;

                        // FK
                        sv_signboard.LOCATION_ADDRESS_ID = signboard_addr.UUID;
                        sv_signboard.OWNER_ID = owner.UUID;

                        db.B_SV_SIGNBOARD.Add(sv_signboard);
                        db.SaveChanges();


                        #endregion

                        #region sv_record
                        B_SV_RECORD sv_record = new B_SV_RECORD();
                        //sv_record.TO_USER_ID;
                        //sv_record.PO_USER_ID;
                        //sv_record.SPO_USER_ID;

                        //sv_record.AREA_CODE;
                        //sv_record.RECOMMENDATION;
                        //sv_record.NO_OF_SIGNBOARD_VALIDATED;
                        //sv_record.NO_OF_SIGNBOARD_INVOLVED;
                        //sv_record.LSO_VS_OPERATION_YEAR;
                        //sv_record.PREVIOUS_SUBMISSION_NUMBER;
                        //sv_record.NO_OF_SUBMISSION_ERECTION;
                        //sv_record.NO_OF_SUBMISSION_REMOVAL;

                        //sv_record.VALIDITY_AP;
                        //sv_record.SIGNATURE_AP;
                        //sv_record.ITEM_STATED;
                        //sv_record.VALIDITY_PRC;
                        //sv_record.SIGNATURE_AS;
                        //sv_record.INFO_SIGNBOARD_OWNER_PROVIDED;
                        //sv_record.OTHER_IRREGULARITIES;

                        sv_record.RECEIVED_DATE = master.CREATEDDATE;
                        sv_record.REFERENCE_NO = sv_submission.REFERENCE_NO;
                        //sv_record.LANGUAGE_CODE = SignboardConstant.LANG_CHINESE;
                        sv_record.WITH_ALTERATION = detail.ASWORKOPTION.ToString();
                        sv_record.INSPECTION_DATE = detail.INSPECTIONDATE;
                        //sv_record.PROPOSED_ALTERATION_COMM_DATE;
                        sv_record.ACTUAL_ALTERATION_COMM_DATE = detail.ASDATE;
                        //sv_record.ACTUAL_ALTERATION_COMP_DATE;
                        //sv_record.VALIDATION_EXPIRY_DATE;
                        //sv_record.SIGNBOARD_REMOVAL;
                        //sv_record.SIGNBOARD_REMOVAL_DISCOV_DATE;

                        //sv_record.S_CHK_VS_NO = ;
                        //sv_record.S_CHK_INSP_DATE = ;
                        //sv_record.S_CHK_WORK_DATE = ;
                        //sv_record.S_CHK_SIGNBOARD = ;
                        //sv_record.S_CHK_SIG = ;
                        //sv_record.S_CHK_SIG_DATE = ;
                        //sv_record.S_CHK_MW_ITEM_NO = ;
                        //sv_record.S_CHK_SUPPORT_DOC = ;
                        //sv_record.S_CHK_SBO_PWA_AP =;
                        //sv_record.S_CHK_OTHERS = ;

                        //sv_record.P_CHK_APP_AP_MW_ITEM =;
                        //sv_record.P_CHK_VAL_AP = ; 
                        //sv_record.P_CHK_SIG_AP = ;
                        //sv_record.P_CHK_VAL_RSE = ;
                        //sv_record.P_CHK_SIG_RSE = ;
                        //sv_record.P_CHK_VAL_RI = ;
                        //sv_record.P_CHK_SIG_RI = ;
                        //sv_record.P_CHK_VAL_PRC = ;
                        //sv_record.P_CHK_SIG_AS = ;
                        //sv_record.P_CHK_CAP_AS_MW_ITEM = ;

                        //sv_record.ACK_LETTERISS_DATE;

                        sv_record.STATUS_CODE = SignboardConstant.SV_RECORD_STATUS_CODE_DATA_ENTRY_DRAFT;

                        // FK
                        sv_record.SV_SUBMISSION_ID = sv_submission.UUID;
                        sv_record.SV_SIGNBOARD_ID = sv_signboard.UUID;
                        sv_record.PAW_ID = paw.UUID;
                        sv_record.OI_ID = oi.UUID;

                        db.B_SV_RECORD.Add(sv_record);
                        db.SaveChanges();
                        #endregion

                        #region professional
                        B_SV_APPOINTED_PROFESSIONAL ap = new B_SV_APPOINTED_PROFESSIONAL();
                        B_SV_APPOINTED_PROFESSIONAL rse = new B_SV_APPOINTED_PROFESSIONAL();
                        B_SV_APPOINTED_PROFESSIONAL rge = new B_SV_APPOINTED_PROFESSIONAL();
                        B_SV_APPOINTED_PROFESSIONAL prc = new B_SV_APPOINTED_PROFESSIONAL();

                        if ((detail.APCFMISAP.ToString().Equals("1"))) // 1: is AP
                        {
                            ap.CERTIFICATION_NO = detail.APCRNUM1 + "/" + detail.APCRNUM2;
                            ap.CHINESE_NAME = detail.APCHINAME;
                            ap.ENGLISH_NAME = detail.APENGNAME;
                            //ap.AS_CHINESE_NAME;
                            //ap.AS_ENGLISH_NAME;
                            //ap.STATUS_CODE;
                            ap.EXPIRY_DATE = detail.APCFMCREXPIREDDATE;
                            ap.SIGNATURE_DATE = detail.APCFMSIGNDATE;
                            //ap.CONTACT_NO;
                            //ap.FAX_NO;
                        }
                        else if (detail.APCFMISRSE.ToString().Equals("1")) // 1: is RSE
                        {
                            rse.CERTIFICATION_NO = detail.APCRNUM1 + "/" + detail.APCRNUM2;
                            rse.CHINESE_NAME = detail.APCHINAME;
                            rse.ENGLISH_NAME = detail.APENGNAME;
                            //ap.AS_CHINESE_NAME;
                            //ap.AS_ENGLISH_NAME;
                            //ap.STATUS_CODE;
                            rse.EXPIRY_DATE = detail.APCFMCREXPIREDDATE;
                            rse.SIGNATURE_DATE = detail.APCFMSIGNDATE;
                            //ap.CONTACT_NO;
                            //ap.FAX_NO;
                        }
                        else if (detail.APCFMISRI.ToString().Equals("1")) // 1: is RI (RGE)
                        {
                            rge.CERTIFICATION_NO = detail.APCRNUM1 + "/" + detail.APCRNUM2;
                            rge.CHINESE_NAME = detail.APCHINAME;
                            rge.ENGLISH_NAME = detail.APENGNAME;
                            //ap.AS_CHINESE_NAME;
                            //ap.AS_ENGLISH_NAME;
                            //ap.STATUS_CODE;
                            rge.EXPIRY_DATE = detail.APCFMCREXPIREDDATE;
                            rge.SIGNATURE_DATE = detail.APCFMSIGNDATE;
                            //ap.CONTACT_NO;
                            //ap.FAX_NO;
                        }
                        else if (detail.APCFMISRGBC.ToString().Equals("1")) // 1: is GBC
                        {
                            prc.CERTIFICATION_NO = detail.APCRNUM1 + "/" + detail.APCRNUM2;
                            prc.CHINESE_NAME = detail.APCHINAME;
                            prc.ENGLISH_NAME = detail.APENGNAME;
                            //ap.AS_CHINESE_NAME;
                            //ap.AS_ENGLISH_NAME;
                            //ap.STATUS_CODE;
                            prc.EXPIRY_DATE = detail.APCFMCREXPIREDDATE;
                            prc.SIGNATURE_DATE = detail.APCFMSIGNDATE;
                            //ap.CONTACT_NO;
                            //ap.FAX_NO;
                        }
                        else if (detail.APCFMISRGBCTYPEC.ToString().Equals("1"))
                        {
                            prc.CERTIFICATION_NO = detail.APCRNUM1 + "/" + detail.APCRNUM2;
                            prc.CHINESE_NAME = detail.APCHINAME;
                            prc.ENGLISH_NAME = detail.APENGNAME;
                            //ap.AS_CHINESE_NAME;
                            //ap.AS_ENGLISH_NAME;
                            //ap.STATUS_CODE;
                            prc.EXPIRY_DATE = detail.APCFMCREXPIREDDATE;
                            prc.SIGNATURE_DATE = detail.APCFMSIGNDATE;
                            //ap.CONTACT_NO;
                            //ap.FAX_NO;
                        }

                        ap.IDENTIFY_FLAG = SignboardConstant.PBP_CODE_AP;
                        rse.IDENTIFY_FLAG = SignboardConstant.PBP_CODE_RSE;
                        rge.IDENTIFY_FLAG = SignboardConstant.PBP_CODE_RGE;
                        prc.IDENTIFY_FLAG = SignboardConstant.PRC_CODE;

                        // FK
                        ap.SV_RECORD_ID = sv_record.UUID;
                        rse.SV_RECORD_ID = sv_record.UUID;
                        rge.SV_RECORD_ID = sv_record.UUID;
                        prc.SV_RECORD_ID = sv_record.UUID;
                        prc.SV_RECORD_ID = sv_record.UUID;
                        db.B_SV_APPOINTED_PROFESSIONAL.Add(ap);
                        db.B_SV_APPOINTED_PROFESSIONAL.Add(rse);
                        db.B_SV_APPOINTED_PROFESSIONAL.Add(rge);
                        db.B_SV_APPOINTED_PROFESSIONAL.Add(prc);
                        db.SaveChanges();

                        if(prc.CERTIFICATION_NO == null)
                        {
                            if (!detail.PRCCFMCONFIRMOPTION.ToString().Equals("1")) // 1: not involve -> skip
                            {
                                prc.CERTIFICATION_NO = detail.PRCCFMCRNUM1 + "/" + detail.PRCCFMCRNUM2;
                                prc.CHINESE_NAME = detail.PRCCFMCHINAME;
                                prc.ENGLISH_NAME = detail.PRCCFMENGNAME1 + " " + detail.PRCCFMENGNAME2;
                                prc.AS_CHINESE_NAME = detail.PRCASCFMCHINAME;
                                prc.AS_ENGLISH_NAME = detail.PRCASCFMENGNAME;
                                //prc.STATUS_CODE;
                                prc.EXPIRY_DATE = detail.PRCCFMCREXPIREDDATE;
                                prc.SIGNATURE_DATE = detail.PRCCFMSIGNDATE;
                                //prc.CONTACT_NO;
                                //prc.FAX_NO;
                                prc.IDENTIFY_FLAG = SignboardConstant.PRC_CODE;
                            }
                            db.SaveChanges();
                        }


                        #endregion

                        #region mw items
                        var count = 0;
                        foreach (var item in items)
                        {
                            B_SV_RECORD_ITEM mwitem = new B_SV_RECORD_ITEM();
                            mwitem.MW_ITEM_CODE = item.MWITEMNO;
                            mwitem.LOCATION_DESCRIPTION = item.MWDESC;
                            mwitem.CLASS_CODE = SignboardCommentService.getClassCode(item.MWITEMNO);
                            mwitem.ORDERING = new decimal(count);

                            // FK
                            mwitem.SV_RECORD_ID = sv_record.UUID;
                            db.B_SV_RECORD_ITEM.Add(mwitem);
                            db.SaveChanges();
                            count++;
                        }
                        #endregion

                        #region validation items
                        var counter = 0;
                        // 1(b)
                        if (detail.INSPECTEDUNAUTHSIGNBOARDITEM1B != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM1B.ToString().Equals("1"))
                        {
                            for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM1BQTY; i++)
                            {
                                B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                                validation.VALIDATION_ITEM = "1(b)";
                                validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                                validation.ORDERING = new decimal(counter);

                                // FK
                                validation.SV_RECORD_ID = sv_record.UUID;
                                db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                                db.SaveChanges();
                                counter++;
                            }
                        }
                        // 1(c)
                        if (detail.INSPECTEDUNAUTHSIGNBOARDITEM1C != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM1C.ToString().Equals("1"))
                        {
                            for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM1CQTY; i++)
                            {
                                B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                                validation.VALIDATION_ITEM = "1(c)";
                                validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                                validation.ORDERING = new decimal(counter);


                                // FK
                                validation.SV_RECORD_ID = sv_record.UUID;
                                db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                                db.SaveChanges();
                                counter++;
                            }
                        }
                        // 2(b)
                        if (detail.INSPECTEDUNAUTHSIGNBOARDITEM2B != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM2B.ToString().Equals("1"))
                        {
                            for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM2BQTY; i++)
                            {
                                B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                                validation.VALIDATION_ITEM = "2(b)";
                                validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                                validation.ORDERING = new decimal(counter);

                                // FK
                                validation.SV_RECORD_ID = sv_record.UUID;
                                db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                                db.SaveChanges();
                                counter++;
                            }
                        }
                        // 2(c)
                        if (detail.INSPECTEDUNAUTHSIGNBOARDITEM2C != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM2C.ToString().Equals("1"))
                        {
                            for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM2CQTY; i++)
                            {
                                B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                                validation.VALIDATION_ITEM = "2(c)";
                                validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                                validation.ORDERING = new decimal(counter);

                                // FK
                                validation.SV_RECORD_ID = sv_record.UUID;
                                db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                                db.SaveChanges();
                                counter++;
                            }
                        }
                        // 4(b)
                        if (detail.INSPECTEDUNAUTHSIGNBOARDITEM4B != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM4B.ToString().Equals("1"))
                        {
                            for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM4BQTY; i++)
                            {
                                B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                                validation.VALIDATION_ITEM = "4(b)";
                                validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                                validation.ORDERING = new decimal(counter);

                                // FK
                                validation.SV_RECORD_ID = sv_record.UUID;
                                db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                                db.SaveChanges();
                                counter++;
                            }
                        }
                        // 5
                        if (detail.INSPECTEDUNAUTHSIGNBOARDITEM5 != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM5.ToString().Equals("1"))
                        {
                            for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM5QTY; i++)
                            {
                                B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                                validation.VALIDATION_ITEM = "5";
                                validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                                validation.ORDERING = new decimal(counter);

                                // FK
                                validation.SV_RECORD_ID = sv_record.UUID;
                                db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                                db.SaveChanges();
                                counter++;
                            }
                        }
                        // 6
                        if (detail.INSPECTEDUNAUTHSIGNBOARDITEM6 != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM6.ToString().Equals("1"))
                        {
                            for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM6QTY; i++)
                            {
                                B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                                validation.VALIDATION_ITEM = "6";
                                validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                                validation.ORDERING = new decimal(counter);

                                // FK
                                validation.SV_RECORD_ID = sv_record.UUID;
                                db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                                db.SaveChanges();
                                counter++;
                            }
                        }
                        #endregion

                        #region file upload
                        // form
                        B_SV_SCANNED_DOCUMENT sd = db.B_SV_SCANNED_DOCUMENT.Find(sv_submission.SV_SCANNED_DOCUMENT_ID);
                        string fileName = master.FILENAME;
                        string fileExtension = master.FILEEXTENSION;
                        //string DSN = ss.GetNumber();
                        string DSN = sd.DSN_NUMBER;
                        string directory = ApplicationConstant.SMMFilePath + "scan" + getSvDocRelativeFilePath(DSN);
                        string relativePath = getSvDocRelativeFilePath(DSN) + fileName + fileExtension;
                        string fullPath = ApplicationConstant.SMMFilePath + "scan" + relativePath;
                        if (!Directory.Exists(directory))
                        {
                            System.IO.Directory.CreateDirectory(directory);
                        }
                        File.WriteAllBytes(fullPath, master.FILECONTENT);

                        sd.FILE_PATH = fullPath;
                        sd.RELATIVE_FILE_PATH = relativePath;
                        sd.RECORD_ID = sv_submission.REFERENCE_NO;
                        sd.DSN_NUMBER = DSN;
                        sd.RECORD_TYPE = SignboardConstant.SCAN_DOC_RECORD_TYPE_VALIDATION;
                        sd.DOCUMENT_TYPE = "Form";
                        //sd.FOLDER_TYPE = SignboardConstant.SCANNED_DOC_FOLDER_TYPE_PRIVATE_SCU;
                        sd.SUBMIT_TYPE = SignboardConstant.SV_Validation;

                        db.SaveChanges();


                        foreach (var file in files)
                        {
                            string file_name = file.FILENAME;
                            string file_extension = file.FILEEXTENSION;
                            string DSN_no = ss.GetNumber();
                            string file_directory = ApplicationConstant.SMMFilePath + "scan" + getSvDocRelativeFilePath(DSN_no);
                            string relative_path = getSvDocRelativeFilePath(DSN_no) + file_name + file_extension;
                            string full_path = ApplicationConstant.SMMFilePath + "scan" + relative_path;
                            if (!Directory.Exists(file_directory))
                            {
                                System.IO.Directory.CreateDirectory(file_directory);
                            }
                            File.WriteAllBytes(full_path, file.FILECONTENT);

                            B_SV_SCANNED_DOCUMENT doc = new B_SV_SCANNED_DOCUMENT();
                            //doc.AS_THUMBNAIL = "N";
                            doc.FILE_PATH = full_path;
                            sd.RELATIVE_FILE_PATH = relative_path;
                            doc.RECORD_ID = sv_submission.REFERENCE_NO;
                            doc.DSN_NUMBER = DSN_no;
                            doc.RECORD_TYPE = SignboardConstant.SCAN_DOC_RECORD_TYPE_VALIDATION;
                            doc.DOCUMENT_TYPE = file.DOCTYPE;
                            doc.FOLDER_TYPE = file.ATTTYPE;
                            doc.SUBMIT_TYPE = SignboardConstant.SV_Validation;


                            db.B_SV_SCANNED_DOCUMENT.Add(doc);
                            db.SaveChanges();
                        }
                        #endregion

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }

        public void dataEntry_SC03(string EFSS_ID, SCUR_Models scurModel)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                B_EFSS_FORM_MASTER master = db.B_EFSS_FORM_MASTER.Find(EFSS_ID);
                B_EFSCU_TBL_SC03 detail = db.B_EFSCU_TBL_SC03.Find(master.FORMCONTENTID);
                List<B_EFSCU_TBL_SC03_ITEM> items = db.B_EFSCU_TBL_SC03_ITEM.Where(x => x.SC03ID == detail.ID).ToList();
                List<B_EFSS_FORM_ATTACHMENTS> files = db.B_EFSS_FORM_ATTACHMENTS.Where(x => x.RECVFORMID == master.ID).ToList();

                B_SV_SUBMISSION sv_submission = db.B_SV_SUBMISSION.Where(x => x.REFERENCE_NO == scurModel.SubmissionNo && x.FORM_CODE == scurModel.FormCode)
                    .Include(x => x.B_SV_SCANNED_DOCUMENT)
                    .FirstOrDefault();

                SignboardCommonDAOService ss = new SignboardCommonDAOService();

                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region signboard owner & address
                        B_SV_ADDRESS owner_addr = new B_SV_ADDRESS();
                        if(detail.SOCORRADDRSAMEASOPADDR.ToString().Equals("1")) // 1: coor addr same as sb addr
                        {
                            owner_addr.FLAT = detail.UNAUTHSIGNBOARDFLAT;
                            owner_addr.FLOOR = detail.UNAUTHSIGNBOARDFLOOR;
                            owner_addr.BUILDINGNAME = detail.BUILDING;
                            owner_addr.STREET = detail.STREETROADVILLAGE;
                            owner_addr.STREET_NO = detail.STREETNO;
                            owner_addr.DISTRICT = detail.UNAUTHSIGNBOARDDISTRICT;
                            owner_addr.REGION = detail.UNAUTHSIGNBOARDAREA;
                            //owner_addr.FULL_ADDRESS = ;
                            owner_addr.FILE_REFERENCE_NO = master.FOURPLUSTWO;
                            //owner_addr.BCIS_DISTRICT = ;
                        }
                        else
                        {
                            owner_addr.FLAT = detail.SOCORRFLAT;
                            owner_addr.FLOOR = detail.SOCORRFLOOR;
                            owner_addr.BUILDINGNAME = detail.SOCORRBUILDING;
                            owner_addr.STREET = detail.SOCORRSTREET;
                            owner_addr.STREET_NO = detail.SOCORRSTREETNO;
                            owner_addr.DISTRICT = detail.SOCORRDISTRICT;
                            owner_addr.REGION = detail.SOCORRAREA;
                            //owner_addr.FULL_ADDRESS = ;
                            owner_addr.FILE_REFERENCE_NO = master.FOURPLUSTWO;
                            //owner_addr.BCIS_DISTRICT = ;
                        }

                        db.B_SV_ADDRESS.Add(owner_addr);
                        db.SaveChanges();

                        B_SV_PERSON_CONTACT owner = new B_SV_PERSON_CONTACT();
                        owner.NAME_CHINESE = detail.SOCHINAMEOFBUSINESS;
                        owner.NAME_ENGLISH = detail.SOENGNAMEOFBUSINESS1 + " " + detail.SOENGNAMEOFBUSINESS2;
                        owner.EMAIL = detail.EMAIL;
                        owner.CONTACT_NO = detail.SOTEL;
                        owner.FAX_NO = detail.SOFAX;
                        owner.ID_TYPE = "3"; // BR #
                        owner.ID_NUMBER = detail.SOBRN;
                        //owner.ID_ISSUE_COUNTRY = ;
                        //owner.CONTACT_PERSON_TITLE = ;
                        //owner.FIRST_NAME = ;
                        //owner.LAST_NAME = ;
                        //owner.MOBILE = ;
                        //owner.SIGNATURE_DATE = ;
                        //owner.PRC_NAME = ;
                        //owner.PRC_CONTACT_NO = ;
                        //owner.PBP_NAME;
                        //owner.PBP_CONTACT_NO = ;
                        //owner.PAW_SAME_AS;

                        // FK
                        owner.SV_ADDRESS_ID = owner_addr.UUID;
                        db.B_SV_PERSON_CONTACT.Add(owner);
                        db.SaveChanges();

                        #endregion

                        #region paw & address
                        B_SV_ADDRESS paw_addr = new B_SV_ADDRESS();
                        //paw_addr.FLAT = 
                        //paw_addr.FLOOR = 
                        //paw_addr.BUILDINGNAME = 
                        //paw_addr.STREET = 
                        //paw_addr.STREET_NO =
                        //paw_addr.DISTRICT = 
                        //paw_addr.REGION = 


                        db.B_SV_ADDRESS.Add(paw_addr);
                        db.SaveChanges();

                        B_SV_PERSON_CONTACT paw = new B_SV_PERSON_CONTACT();
                        paw.NAME_CHINESE = detail.ARRNAME;
                        //paw.NAME_ENGLISH = 
                        //paw.EMAIL = 
                        //paw.CONTACT_NO = 
                        //paw.FAX_NO = 
                        //paw.ID_TYPE = 
                        //paw.ID_NUMBER = 
                        //paw.ID_ISSUE_COUNTRY
                        //paw.CONTACT_PERSON_TITLE = 
                        //paw.FIRST_NAME = 
                        //paw.LAST_NAME = 
                        //paw.MOBILE = 
                        paw.SIGNATURE_DATE = detail.ARRSIGNDATE;
                        //paw.PRC_NAME = 
                        //paw.PRC_CONTACT_NO = 
                        //paw.PBP_NAME = 
                        //paw.PBP_CONTACT_NO = 
                        string paw_same_as = "";
                        if (detail.ARRISAPOPTION.ToString().Equals("1"))
                        {
                            paw_same_as = SignboardConstant.CODE_PBP;
                        }
                        else if (detail.ARRISRGBCOPTION.ToString().Equals("1"))
                        {
                            paw_same_as = SignboardConstant.CODE_GBC;
                        }
                        else if (detail.ARRISRMWCTYPECOPTION.ToString().Equals("1"))
                        {
                            paw_same_as = SignboardConstant.PRC_CODE;
                        }
                        else if (detail.ARRISRSEOPTION.ToString().Equals("1"))
                        {
                            paw_same_as = SignboardConstant.CODE_PBP;
                        }
                        else if (detail.ARRISSOOPTION.ToString().Equals("1"))
                        {
                            paw_same_as = SignboardConstant.CODE_SB_OWNER;
                        }
                        else if(detail.ARRISRIOPTION.ToString().Equals("1"))
                        {
                            paw_same_as = SignboardConstant.PBP_CODE_RI;
                        }
                        else if(detail.ARRISRMWCOPTION.ToString().Equals("1"))
                        {
                            paw_same_as = SignboardConstant.CODE_MWC_W;
                        }
                        paw.PAW_SAME_AS = paw_same_as;

                        // FK
                        paw.SV_ADDRESS_ID = paw_addr.UUID;
                        db.B_SV_PERSON_CONTACT.Add(paw);
                        db.SaveChanges();

                        #endregion

                        #region oi & address
                        B_SV_ADDRESS oi_addr = new B_SV_ADDRESS();
                        B_SV_PERSON_CONTACT oi = new B_SV_PERSON_CONTACT();
                        if (detail.OWNERCORPFORMEDOPTION.ToString().Equals("1"))
                        {
                            //oi_addr.BLOCK = 
                            oi_addr.STREET = detail.OWNERCORPSTREET;
                            oi_addr.STREET_NO = detail.OWNERCORPSTREETNO;
                            oi_addr.BUILDINGNAME = detail.OWNERCORPBUILDING;
                            oi_addr.FLOOR = detail.OWNERCORPFLOOR;
                            oi_addr.FLAT = detail.OWNERCORPFLAT;
                            oi_addr.DISTRICT = detail.OWNERCORPDISTRICT;
                            oi_addr.REGION = detail.OWNERCORPAREA;

                            //oi.NAME_CHINESE = 
                            //oi.NAME_ENGLISH = 
                            //oi.EMAIL = 
                            //oi.CONTACT_NO = 
                            //oi.FAX_NO = 
                            //oi.ID_TYPE = 
                            //oi.ID_NUMBER = 
                            //oi.ID_ISSUE_COUNTRY = 
                            //oi.CONTACT_PERSON_TITLE = 
                            //oi.FIRST_NAME = 
                            //oi.LAST_NAME = 
                            //oi.MOBILE = 
                            //oi.SIGNATURE_DATE = 
                            //oi.PRC_NAME = 
                            //oi.PRC_CONTACT_NO = 
                            //oi.PBP_NAME = 
                            //oi.PBP_CONTACT_NO = 
                            //oi.PAW_SAME_AS = 
                        }
                        db.B_SV_ADDRESS.Add(oi_addr);
                        db.SaveChanges();

                        // FK
                        oi.SV_ADDRESS_ID = oi_addr.UUID;

                        db.B_SV_PERSON_CONTACT.Add(oi);
                        db.SaveChanges();
                        #endregion

                        #region signboard & address
                        B_SV_ADDRESS signboard_addr = new B_SV_ADDRESS();
                        signboard_addr.FLAT = detail.UNAUTHSIGNBOARDFLAT;
                        signboard_addr.FLOOR = detail.UNAUTHSIGNBOARDFLOOR;
                        signboard_addr.BUILDINGNAME = detail.BUILDING;
                        signboard_addr.STREET_NO = detail.STREETNO;
                        signboard_addr.STREET = detail.STREETROADVILLAGE;
                        signboard_addr.DISTRICT = detail.UNAUTHSIGNBOARDDISTRICT;
                        signboard_addr.REGION = detail.UNAUTHSIGNBOARDAREA;
                        //signboard_addr.FULL_ADDRESS = detail.;

                        db.B_SV_ADDRESS.Add(signboard_addr);
                        db.SaveChanges();

                        B_SV_SIGNBOARD sv_signboard = new B_SV_SIGNBOARD();
                        sv_signboard.LOCATION_OF_SIGNBOARD = detail.UNAUTHSIGNBOARDDETAILEDLOCATION;
                        //sv_signboard.RVD_NO = ;
                        //sv_signboard.FACADE = ;
                        //sv_signboard.TYPE = ;
                        //sv_signboard.BTM_FLOOR = ;
                        //sv_signboard.TOP_FLOOR = ;
                        //sv_signboard.A_M2 = ;
                        //sv_signboard.P_M = ;
                        //sv_signboard.H_M = ;
                        //sv_signboard.H2_M = ;
                        //sv_signboard.LED = ;
                        //sv_signboard.BUILDING_PORTION = ;
                        //sv_signboard.STATUS = ;
                        //sv_signboard.DESCRIPTION = ;
                        //sv_signboard.S24_ORDER_NO = ;
                        //sv_signboard.S24_ORDER_TYPE = ;

                        // FK
                        sv_signboard.LOCATION_ADDRESS_ID = signboard_addr.UUID;
                        sv_signboard.OWNER_ID = owner.UUID;

                        db.B_SV_SIGNBOARD.Add(sv_signboard);
                        db.SaveChanges();


                        #endregion

                        #region sv_record
                        B_SV_RECORD sv_record = new B_SV_RECORD();
                        //sv_record.TO_USER_ID;
                        //sv_record.PO_USER_ID;
                        //sv_record.SPO_USER_ID;

                        //sv_record.AREA_CODE;
                        //sv_record.RECOMMENDATION;
                        //sv_record.NO_OF_SIGNBOARD_VALIDATED;
                        //sv_record.NO_OF_SIGNBOARD_INVOLVED;
                        //sv_record.LSO_VS_OPERATION_YEAR;
                        //sv_record.PREVIOUS_SUBMISSION_NUMBER;
                        //sv_record.NO_OF_SUBMISSION_ERECTION;
                        //sv_record.NO_OF_SUBMISSION_REMOVAL;

                        //sv_record.VALIDITY_AP;
                        //sv_record.SIGNATURE_AP;
                        //sv_record.ITEM_STATED;
                        //sv_record.VALIDITY_PRC;
                        //sv_record.SIGNATURE_AS;
                        //sv_record.INFO_SIGNBOARD_OWNER_PROVIDED;
                        //sv_record.OTHER_IRREGULARITIES;

                        sv_record.RECEIVED_DATE = master.CREATEDDATE;
                        sv_record.REFERENCE_NO = sv_submission.REFERENCE_NO;
                        //sv_record.LANGUAGE_CODE = SignboardConstant.LANG_CHINESE;
                        sv_record.WITH_ALTERATION = detail.ASWORKOPTION.ToString();
                        sv_record.INSPECTION_DATE = detail.INSPECTIONDATE;
                        //sv_record.PROPOSED_ALTERATION_COMM_DATE;
                        sv_record.ACTUAL_ALTERATION_COMM_DATE = detail.COMMENCEDATE;
                        sv_record.ACTUAL_ALTERATION_COMP_DATE = detail.COMPLETEDATE;
                        //sv_record.VALIDATION_EXPIRY_DATE;
                        //sv_record.SIGNBOARD_REMOVAL;
                        //sv_record.SIGNBOARD_REMOVAL_DISCOV_DATE;

                        //sv_record.S_CHK_VS_NO = ;
                        //sv_record.S_CHK_INSP_DATE = ;
                        //sv_record.S_CHK_WORK_DATE = ;
                        //sv_record.S_CHK_SIGNBOARD = ;
                        //sv_record.S_CHK_SIG = ;
                        //sv_record.S_CHK_SIG_DATE = ;
                        //sv_record.S_CHK_MW_ITEM_NO = ;
                        //sv_record.S_CHK_SUPPORT_DOC = ;
                        //sv_record.S_CHK_SBO_PWA_AP =;
                        //sv_record.S_CHK_OTHERS = ;

                        //sv_record.P_CHK_APP_AP_MW_ITEM =;
                        //sv_record.P_CHK_VAL_AP = ; 
                        //sv_record.P_CHK_SIG_AP = ;
                        //sv_record.P_CHK_VAL_RSE = ;
                        //sv_record.P_CHK_SIG_RSE = ;
                        //sv_record.P_CHK_VAL_RI = ;
                        //sv_record.P_CHK_SIG_RI = ;
                        //sv_record.P_CHK_VAL_PRC = ;
                        //sv_record.P_CHK_SIG_AS = ;
                        //sv_record.P_CHK_CAP_AS_MW_ITEM = ;

                        //sv_record.ACK_LETTERISS_DATE;

                        sv_record.STATUS_CODE = SignboardConstant.SV_RECORD_STATUS_CODE_DATA_ENTRY_DRAFT;

                        // FK
                        sv_record.SV_SUBMISSION_ID = sv_submission.UUID;
                        sv_record.SV_SIGNBOARD_ID = sv_signboard.UUID;
                        sv_record.PAW_ID = paw.UUID;
                        sv_record.OI_ID = oi.UUID;

                        db.B_SV_RECORD.Add(sv_record);
                        db.SaveChanges();
                        #endregion

                        #region professional
                        
                        B_SV_APPOINTED_PROFESSIONAL ap = new B_SV_APPOINTED_PROFESSIONAL();
                        B_SV_APPOINTED_PROFESSIONAL rse = new B_SV_APPOINTED_PROFESSIONAL();
                        B_SV_APPOINTED_PROFESSIONAL rge = new B_SV_APPOINTED_PROFESSIONAL();
                        B_SV_APPOINTED_PROFESSIONAL prc = new B_SV_APPOINTED_PROFESSIONAL();

                        // confimation
                        if ((detail.APCFMISAPOPTION.ToString().Equals("1"))) // 1: is AP
                        {
                            ap.CERTIFICATION_NO = detail.APCFMCRNUM1 + "/" + detail.APCFMCRNUM2;
                            ap.CHINESE_NAME = detail.APCFMCHINAME;
                            ap.ENGLISH_NAME = detail.APCFMENGNAME;
                            //ap.AS_CHINESE_NAME;
                            //ap.AS_ENGLISH_NAME;
                            //ap.STATUS_CODE;
                            ap.EXPIRY_DATE = detail.APCFMCREXPIREDDATE;
                            ap.SIGNATURE_DATE = detail.APCFMSIGNDATE;
                            //ap.CONTACT_NO;
                            //ap.FAX_NO;
                        }
                        else if (detail.APCFMISRSEOPTION.ToString().Equals("1")) // 1: is RSE
                        {
                            rse.CERTIFICATION_NO = detail.APCFMCRNUM1 + "/" + detail.APCFMCRNUM2;
                            rse.CHINESE_NAME = detail.APCFMCHINAME;
                            rse.ENGLISH_NAME = detail.APCFMENGNAME;
                            //rse.AS_CHINESE_NAME;
                            //rse.AS_ENGLISH_NAME;
                            //rse.STATUS_CODE;
                            rse.EXPIRY_DATE = detail.APCFMCREXPIREDDATE;
                            rse.SIGNATURE_DATE = detail.APCFMSIGNDATE;
                            //rse.CONTACT_NO;
                            //rse.FAX_NO;
                        }
                        else if (detail.APCFMISRIOPTION.ToString().Equals("1")) // 1: is RI (RGE)
                        {
                            rge.CERTIFICATION_NO = detail.APCFMCRNUM1 + "/" + detail.APCFMCRNUM2;
                            rge.CHINESE_NAME = detail.APCFMCHINAME;
                            rge.ENGLISH_NAME = detail.APCFMENGNAME;
                            //rge.AS_CHINESE_NAME;
                            //rge.AS_ENGLISH_NAME;
                            //rge.STATUS_CODE;
                            rge.EXPIRY_DATE = detail.APCFMCREXPIREDDATE;
                            rge.SIGNATURE_DATE = detail.APCFMSIGNDATE;
                            //rge.CONTACT_NO;
                            //rge.FAX_NO;
                        }
                        else if (detail.APCFMISRGBCOPTION.ToString().Equals("1")) // 1: is GBC
                        {
                            prc.CERTIFICATION_NO = detail.APCFMCRNUM1 + "/" + detail.APCFMCRNUM2;
                            prc.CHINESE_NAME = detail.APCFMCHINAME;
                            prc.ENGLISH_NAME = detail.APCFMENGNAME;
                            //prc.AS_CHINESE_NAME;
                            //prc.AS_ENGLISH_NAME;
                            //prc.STATUS_CODE;
                            prc.EXPIRY_DATE = detail.APCFMCREXPIREDDATE;
                            prc.SIGNATURE_DATE = detail.APCFMSIGNDATE;
                            //prc.CONTACT_NO;
                            //prc.FAX_NO;
                        }
                        else if (detail.APCFMISRMWCOPTION.ToString().Equals("1")) // 1: is MWC
                        {
                            prc.CERTIFICATION_NO = detail.APCFMCRNUM1 + "/" + detail.APCFMCRNUM2;
                            prc.CHINESE_NAME = detail.APCFMCHINAME;
                            prc.ENGLISH_NAME = detail.APCFMENGNAME;
                            //prc.AS_CHINESE_NAME;
                            //prc.AS_ENGLISH_NAME;
                            //prc.STATUS_CODE;
                            prc.EXPIRY_DATE = detail.APCFMCREXPIREDDATE;
                            prc.SIGNATURE_DATE = detail.APCFMSIGNDATE;
                            //prc.CONTACT_NO;
                            //prc.FAX_NO;
                            prc.IDENTIFY_FLAG = SignboardConstant.PRC_CODE;
                        }
                        else if (detail.APCFMISRMWCTYPECOPTION.ToString().Equals("1")) // 1: is MWC W
                        {
                            prc.CERTIFICATION_NO = detail.APCFMCRNUM1 + "/" + detail.APCFMCRNUM2;
                            prc.CHINESE_NAME = detail.APCFMCHINAME;
                            prc.ENGLISH_NAME = detail.APCFMENGNAME;
                            //prc.AS_CHINESE_NAME;
                            //prc.AS_ENGLISH_NAME;
                            //prc.STATUS_CODE;
                            prc.EXPIRY_DATE = detail.APCFMCREXPIREDDATE;
                            prc.SIGNATURE_DATE = detail.APCFMSIGNDATE;
                            //prc.CONTACT_NO;
                            //prc.FAX_NO;
                        }

                        ap.IDENTIFY_FLAG = SignboardConstant.PBP_CODE_AP;
                        rse.IDENTIFY_FLAG = SignboardConstant.PBP_CODE_RSE;
                        rge.IDENTIFY_FLAG = SignboardConstant.PBP_CODE_RGE;
                        prc.IDENTIFY_FLAG = SignboardConstant.PRC_CODE;

                        // FK
                        ap.SV_RECORD_ID = sv_record.UUID;
                        rse.SV_RECORD_ID = sv_record.UUID;
                        rge.SV_RECORD_ID = sv_record.UUID;
                        prc.SV_RECORD_ID = sv_record.UUID;
                        db.B_SV_APPOINTED_PROFESSIONAL.Add(ap);
                        db.B_SV_APPOINTED_PROFESSIONAL.Add(rse);
                        db.B_SV_APPOINTED_PROFESSIONAL.Add(rge);
                        db.B_SV_APPOINTED_PROFESSIONAL.Add(prc);
                        db.SaveChanges();


                        if(prc.CERTIFICATION_NO == null)
                        {
                            if (!detail.ASNOTINVOLVEDOPTION.ToString().Equals("1")) // 1: not involve -> skip
                            {
                                prc.CERTIFICATION_NO = detail.PRCCFMCRNUM1 + "/" + detail.PRCCFMCRNUM2;
                                prc.CHINESE_NAME = detail.PRCCFMCHINAME;
                                prc.ENGLISH_NAME = detail.PRCCFMENGNAME1 + " " + detail.PRCCFMENGNAME2;
                                prc.AS_CHINESE_NAME = detail.PRCASCFMCHINAME;
                                prc.AS_ENGLISH_NAME = detail.PRCASCFMENGNAME;
                                //prc.STATUS_CODE;
                                prc.EXPIRY_DATE = detail.PRCCFMCREXPIREDDATE;
                                prc.SIGNATURE_DATE = detail.PRCCFMSIGNDATE;
                                //prc.CONTACT_NO;
                                //prc.FAX_NO;
                                prc.IDENTIFY_FLAG = SignboardConstant.PRC_CODE;
                            }
                            db.SaveChanges();
                        }
                        

                        // completion
                        if (detail.APCOMPLETEISRCOPTION.ToString().Equals("1"))
                        {
                            if ((detail.APCOMPLETEISAPOPTION.ToString().Equals("1"))) // 1: is AP
                            {
                                ap.CERTIFICATION_NO = detail.APCOMPLETECRNUM1 + "/" + detail.APCOMPLETECRNUM2;
                                ap.CHINESE_NAME = detail.APCOMPLETECHINAME;
                                ap.ENGLISH_NAME = detail.APCOMPLETEENGNAME;
                                //ap_2.AS_CHINESE_NAME;
                                //ap_2.AS_ENGLISH_NAME;
                                //ap_2.STATUS_CODE;
                                ap.EXPIRY_DATE = detail.APCOMPLETECREXPIREDDATE;
                                ap.SIGNATURE_DATE = detail.APCOMPLETESIGNDATE;
                                //ap_2.CONTACT_NO;
                                //ap_2.FAX_NO;
                            }
                            else if (detail.APCOMPLETEISRSEOPTION.ToString().Equals("1")) // 1: is RSE
                            {
                                rse.CERTIFICATION_NO = detail.APCOMPLETECRNUM1 + "/" + detail.APCOMPLETECRNUM2;
                                rse.CHINESE_NAME = detail.APCOMPLETECHINAME;
                                rse.ENGLISH_NAME = detail.APCOMPLETEENGNAME;
                                //ap_2.AS_CHINESE_NAME;
                                //ap_2.AS_ENGLISH_NAME;
                                //ap_2.STATUS_CODE;
                                rse.EXPIRY_DATE = detail.APCOMPLETECREXPIREDDATE;
                                rse.SIGNATURE_DATE = detail.APCOMPLETESIGNDATE;
                                //ap_2.CONTACT_NO;
                                //ap_2.FAX_NO;
                            }
                            else if (detail.APCOMPLETEISRIOPTION.ToString().Equals("1")) // 1: is RI (RGE)
                            {
                                rge.CERTIFICATION_NO = detail.APCOMPLETECRNUM1 + "/" + detail.APCOMPLETECRNUM2;
                                rge.CHINESE_NAME = detail.APCOMPLETECHINAME;
                                rge.ENGLISH_NAME = detail.APCOMPLETEENGNAME;
                                //ap_2.AS_CHINESE_NAME;
                                //ap_2.AS_ENGLISH_NAME;
                                //ap_2.STATUS_CODE;
                                rge.EXPIRY_DATE = detail.APCOMPLETECREXPIREDDATE;
                                rge.SIGNATURE_DATE = detail.APCOMPLETESIGNDATE;
                                //ap_2.CONTACT_NO;
                                //ap_2.FAX_NO;
                            }
                        }
                        db.SaveChanges();



                        #endregion

                        #region mw items
                        var count = 0;
                        foreach (var item in items)
                        {
                            B_SV_RECORD_ITEM mwitem = new B_SV_RECORD_ITEM();
                            mwitem.MW_ITEM_CODE = item.MWITEMNO;
                            mwitem.LOCATION_DESCRIPTION = item.MWDESC;
                            mwitem.CLASS_CODE = SignboardCommentService.getClassCode(item.MWITEMNO);
                            mwitem.ORDERING = new decimal(count);

                            // FK
                            mwitem.SV_RECORD_ID = sv_record.UUID;
                            db.B_SV_RECORD_ITEM.Add(mwitem);
                            db.SaveChanges();
                            count++;
                        }
                        #endregion

                        #region validation items
                        var counter = 0;
                        // 1(c)
                        if (detail.INSPECTEDUNAUTHSIGNBOARDITEM1C != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM1C.ToString().Equals("1"))
                        {
                            for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM1CQTY; i++)
                            {
                                B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                                validation.VALIDATION_ITEM = "1(c)";
                                validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                                validation.ORDERING = new decimal(counter);


                                // FK
                                validation.SV_RECORD_ID = sv_record.UUID;
                                db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                                db.SaveChanges();
                                counter++;
                            }
                        }
                        // 2(c)
                        if (detail.INSPECTEDUNAUTHSIGNBOARDITEM2C != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM2C.ToString().Equals("1"))
                        {
                            for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM2CQTY; i++)
                            {
                                B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                                validation.VALIDATION_ITEM = "2(c)";
                                validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                                validation.ORDERING = new decimal(counter);

                                // FK
                                validation.SV_RECORD_ID = sv_record.UUID;
                                db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                                db.SaveChanges();
                                counter++;
                            }
                        }
                        #endregion

                        #region file upload
                        // form
                        B_SV_SCANNED_DOCUMENT sd = db.B_SV_SCANNED_DOCUMENT.Find(sv_submission.SV_SCANNED_DOCUMENT_ID);
                        string fileName = master.FILENAME;
                        string fileExtension = master.FILEEXTENSION;
                        //string DSN = ss.GetNumber();
                        string DSN = sd.DSN_NUMBER;
                        string directory = ApplicationConstant.SMMFilePath + "scan" + getSvDocRelativeFilePath(DSN);
                        string relativePath = getSvDocRelativeFilePath(DSN) + fileName + fileExtension;
                        string fullPath = ApplicationConstant.SMMFilePath + "scan" + relativePath;
                        if (!Directory.Exists(directory))
                        {
                            System.IO.Directory.CreateDirectory(directory);
                        }
                        File.WriteAllBytes(fullPath, master.FILECONTENT);

                        sd.FILE_PATH = fullPath;
                        sd.RELATIVE_FILE_PATH = relativePath;
                        sd.RECORD_ID = sv_submission.REFERENCE_NO;
                        sd.DSN_NUMBER = DSN;
                        sd.RECORD_TYPE = SignboardConstant.SCAN_DOC_RECORD_TYPE_VALIDATION;
                        sd.DOCUMENT_TYPE = "Form";
                        //sd.FOLDER_TYPE = SignboardConstant.SCANNED_DOC_FOLDER_TYPE_PRIVATE_SCU;
                        sd.SUBMIT_TYPE = SignboardConstant.SV_Validation;

                        db.SaveChanges();


                        foreach (var file in files)
                        {
                            string file_name = file.FILENAME;
                            string file_extension = file.FILEEXTENSION;
                            string DSN_no = ss.GetNumber();
                            string file_directory = ApplicationConstant.SMMFilePath + "scan" + getSvDocRelativeFilePath(DSN_no);
                            string relative_path = getSvDocRelativeFilePath(DSN_no) + file_name + file_extension;
                            string full_path = ApplicationConstant.SMMFilePath + "scan" + relative_path;
                            if (!Directory.Exists(file_directory))
                            {
                                System.IO.Directory.CreateDirectory(file_directory);
                            }
                            File.WriteAllBytes(full_path, file.FILECONTENT);

                            B_SV_SCANNED_DOCUMENT doc = new B_SV_SCANNED_DOCUMENT();
                            //doc.AS_THUMBNAIL = "N";
                            doc.FILE_PATH = full_path;
                            sd.RELATIVE_FILE_PATH = relative_path;
                            doc.RECORD_ID = sv_submission.REFERENCE_NO;
                            doc.DSN_NUMBER = DSN_no;
                            doc.RECORD_TYPE = SignboardConstant.SCAN_DOC_RECORD_TYPE_VALIDATION;
                            doc.DOCUMENT_TYPE = file.DOCTYPE;
                            doc.FOLDER_TYPE = file.ATTTYPE;
                            doc.SUBMIT_TYPE = SignboardConstant.SV_Validation;


                            db.B_SV_SCANNED_DOCUMENT.Add(doc);
                            db.SaveChanges();
                        }
                        #endregion

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }

        public void dataEntry_SC01C(string EFSS_ID, SCUR_Models scurModel)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                B_EFSS_FORM_MASTER master = db.B_EFSS_FORM_MASTER.Find(EFSS_ID);
                B_EFSCU_TBL_SC01C detail = db.B_EFSCU_TBL_SC01C.Find(master.FORMCONTENTID);
                List<B_EFSCU_TBL_SC01C_ITEM> items = db.B_EFSCU_TBL_SC01C_ITEM.Where(x => x.SC01CID == detail.ID).ToList();
                List<B_EFSS_FORM_ATTACHMENTS> files = db.B_EFSS_FORM_ATTACHMENTS.Where(x => x.RECVFORMID == master.ID).ToList();

                B_SV_SUBMISSION sv_submission = db.B_SV_SUBMISSION.Where(x => x.REFERENCE_NO == scurModel.SubmissionNo && x.FORM_CODE == scurModel.FormCode)
                    .Include(x => x.B_SV_SCANNED_DOCUMENT)
                    .FirstOrDefault();

                SignboardCommonDAOService ss = new SignboardCommonDAOService();

                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region signboard owner & address
                        B_SV_ADDRESS owner_addr = new B_SV_ADDRESS();
                        //owner_addr.FLAT = detail.SOCORRFLAT;
                        //owner_addr.FLOOR = detail.SOCORRFLOOR;
                        //owner_addr.BUILDINGNAME = detail.SOCORRBUILDING;
                        //owner_addr.STREET = detail.SOCORRSTREET;
                        //owner_addr.STREET_NO = detail.SOCORRSTREETNO;
                        //owner_addr.DISTRICT = detail.SOCORRDISTRICT;
                        //owner_addr.REGION = detail.SOCORRAREA;
                        ////owner_addr.FULL_ADDRESS = ;
                        //owner_addr.FILE_REFERENCE_NO = master.FOURPLUSTWO;
                        ////owner_addr.BCIS_DISTRICT = ;

                        db.B_SV_ADDRESS.Add(owner_addr);
                        db.SaveChanges();

                        B_SV_PERSON_CONTACT owner = new B_SV_PERSON_CONTACT();
                        //owner.NAME_CHINESE = detail.SOCHINAMEOFBUSINESS;
                        //owner.NAME_ENGLISH = detail.SOENGNAMEOFBUSINESS1 + " " + detail.SOENGNAMEOFBUSINESS2;
                        //owner.EMAIL = detail.EMAIL;
                        //owner.CONTACT_NO = detail.SOTEL;
                        //owner.FAX_NO = detail.SOFAX;
                        //owner.ID_TYPE = "3"; // BR #
                        //owner.ID_NUMBER = detail.SOBRN;
                        ////owner.ID_ISSUE_COUNTRY = ;
                        ////owner.CONTACT_PERSON_TITLE = ;
                        ////owner.FIRST_NAME = ;
                        ////owner.LAST_NAME = ;
                        ////owner.MOBILE = ;
                        ////owner.SIGNATURE_DATE = ;
                        ////owner.PRC_NAME = ;
                        ////owner.PRC_CONTACT_NO = ;
                        ////owner.PBP_NAME;
                        ////owner.PBP_CONTACT_NO = ;
                        ////owner.PAW_SAME_AS;

                        // FK
                        owner.SV_ADDRESS_ID = owner_addr.UUID;
                        db.B_SV_PERSON_CONTACT.Add(owner);
                        db.SaveChanges();

                        #endregion

                        #region paw & address
                        B_SV_ADDRESS paw_addr = new B_SV_ADDRESS();
                        ////paw_addr.FLAT = 
                        ////paw_addr.FLOOR = 
                        ////paw_addr.BUILDINGNAME = 
                        ////paw_addr.STREET = 
                        ////paw_addr.STREET_NO =
                        ////paw_addr.DISTRICT = 
                        ////paw_addr.REGION = 


                        db.B_SV_ADDRESS.Add(paw_addr);
                        db.SaveChanges();

                        B_SV_PERSON_CONTACT paw = new B_SV_PERSON_CONTACT();
                        //paw.NAME_CHINESE = detail.ARRNAME;
                        ////paw.NAME_ENGLISH = 
                        ////paw.EMAIL = 
                        ////paw.CONTACT_NO = 
                        ////paw.FAX_NO = 
                        ////paw.ID_TYPE = 
                        ////paw.ID_NUMBER = 
                        ////paw.ID_ISSUE_COUNTRY
                        ////paw.CONTACT_PERSON_TITLE = 
                        ////paw.FIRST_NAME = 
                        ////paw.LAST_NAME = 
                        ////paw.MOBILE = 
                        ////paw.SIGNATURE_DATE = 
                        ////paw.PRC_NAME = 
                        ////paw.PRC_CONTACT_NO = 
                        ////paw.PBP_NAME = 
                        ////paw.PBP_CONTACT_NO = 
                        //string paw_same_as = "";
                        //if (detail.ARRISAPOPTION.ToString().Equals("1"))
                        //{
                        //    paw_same_as = "PBP";
                        //}
                        //else if (detail.ARRISRGBCOPTION.ToString().Equals("1"))
                        //{
                        //    paw_same_as = "PBP";
                        //}
                        //else if (detail.ARRISRMWCTYPECOPTION.ToString().Equals("1"))
                        //{
                        //    paw_same_as = "PRC";
                        //}
                        //else if (detail.ARRISRSEOPTION.ToString().Equals("1"))
                        //{
                        //    paw_same_as = "PBP";
                        //}
                        //else if (detail.ARRISSOOPTION.ToString().Equals("1"))
                        //{
                        //    paw_same_as = "SB_OWNER";
                        //}
                        //paw.PAW_SAME_AS = paw_same_as;


                        // FK
                        paw.SV_ADDRESS_ID = paw_addr.UUID;
                        db.B_SV_PERSON_CONTACT.Add(paw);
                        db.SaveChanges();

                        #endregion

                        #region oi & address
                        B_SV_ADDRESS oi_addr = new B_SV_ADDRESS();
                        B_SV_PERSON_CONTACT oi = new B_SV_PERSON_CONTACT();
                        //if (detail.OWNERCORPFORMEDOPTION.ToString().Equals("1"))
                        //{
                        //    //oi_addr.BLOCK = 
                        //    oi_addr.STREET = detail.OWNERCORPSTREET;
                        //    oi_addr.STREET_NO = detail.OWNERCORPSTREETNO;
                        //    oi_addr.BUILDINGNAME = detail.OWNERCORPBUILDING;
                        //    oi_addr.FLOOR = detail.OWNERCORPFLOOR;
                        //    oi_addr.FLAT = detail.OWNERCORPFLAT;
                        //    oi_addr.DISTRICT = detail.OWNERCORPDISTRICT;
                        //    oi_addr.REGION = detail.OWNERCORPAREA;

                        //    //oi.NAME_CHINESE = 
                        //    //oi.NAME_ENGLISH = 
                        //    //oi.EMAIL = 
                        //    //oi.CONTACT_NO = 
                        //    //oi.FAX_NO = 
                        //    //oi.ID_TYPE = 
                        //    //oi.ID_NUMBER = 
                        //    //oi.ID_ISSUE_COUNTRY = 
                        //    //oi.CONTACT_PERSON_TITLE = 
                        //    //oi.FIRST_NAME = 
                        //    //oi.LAST_NAME = 
                        //    //oi.MOBILE = 
                        //    //oi.SIGNATURE_DATE = 
                        //    //oi.PRC_NAME = 
                        //    //oi.PRC_CONTACT_NO = 
                        //    //oi.PBP_NAME = 
                        //    //oi.PBP_CONTACT_NO = 
                        //    //oi.PAW_SAME_AS = 

                        // FK
                        db.B_SV_ADDRESS.Add(oi_addr);
                        db.SaveChanges();

                        oi.SV_ADDRESS_ID = oi_addr.UUID;
                        db.B_SV_PERSON_CONTACT.Add(oi);
                        db.SaveChanges();
                        //}
                        #endregion

                        #region signboard & address
                        B_SV_ADDRESS signboard_addr = new B_SV_ADDRESS();
                        //signboard_addr.FLAT = detail.UNAUTHSIGNBOARDFLAT;
                        //signboard_addr.FLOOR = detail.UNAUTHSIGNBOARDFLOOR;
                        //signboard_addr.BUILDINGNAME = detail.BUILDING;
                        //signboard_addr.STREET_NO = detail.STREETNO;
                        //signboard_addr.STREET = detail.STREETROADVILLAGE;
                        //signboard_addr.DISTRICT = detail.UNAUTHSIGNBOARDDISTRICT;
                        //signboard_addr.REGION = detail.UNAUTHSIGNBOARDAREA;
                        ////signboard_addr.FULL_ADDRESS = detail.;

                        db.B_SV_ADDRESS.Add(signboard_addr);
                        db.SaveChanges();

                        B_SV_SIGNBOARD sv_signboard = new B_SV_SIGNBOARD();
                        //sv_signboard.LOCATION_OF_SIGNBOARD = detail.UNAUTHSIGNBOARDDETAILEDLOCATION;
                        ////sv_signboard.RVD_NO = ;
                        ////sv_signboard.FACADE = ;
                        ////sv_signboard.TYPE = ;
                        ////sv_signboard.BTM_FLOOR = ;
                        ////sv_signboard.TOP_FLOOR = ;
                        ////sv_signboard.A_M2 = ;
                        ////sv_signboard.P_M = ;
                        ////sv_signboard.H_M = ;
                        ////sv_signboard.H2_M = ;
                        ////sv_signboard.LED = ;
                        ////sv_signboard.BUILDING_PORTION = ;
                        ////sv_signboard.STATUS = ;
                        ////sv_signboard.DESCRIPTION = ;
                        ////sv_signboard.S24_ORDER_NO = ;
                        ////sv_signboard.S24_ORDER_TYPE = ;

                        // FK
                        sv_signboard.LOCATION_ADDRESS_ID = signboard_addr.UUID;
                        sv_signboard.OWNER_ID = owner.UUID;

                        db.B_SV_SIGNBOARD.Add(sv_signboard);
                        db.SaveChanges();


                        #endregion

                        #region sv_record
                        B_SV_RECORD sv_record = new B_SV_RECORD();
                        //sv_record.TO_USER_ID;
                        //sv_record.PO_USER_ID;
                        //sv_record.SPO_USER_ID;

                        //sv_record.AREA_CODE;
                        //sv_record.RECOMMENDATION;
                        //sv_record.NO_OF_SIGNBOARD_VALIDATED;
                        //sv_record.NO_OF_SIGNBOARD_INVOLVED;
                        //sv_record.LSO_VS_OPERATION_YEAR;
                        //sv_record.PREVIOUS_SUBMISSION_NUMBER;
                        //sv_record.NO_OF_SUBMISSION_ERECTION;
                        //sv_record.NO_OF_SUBMISSION_REMOVAL;

                        //sv_record.VALIDITY_AP;
                        //sv_record.SIGNATURE_AP;
                        //sv_record.ITEM_STATED;
                        //sv_record.VALIDITY_PRC;
                        //sv_record.SIGNATURE_AS;
                        //sv_record.INFO_SIGNBOARD_OWNER_PROVIDED;
                        //sv_record.OTHER_IRREGULARITIES;

                        sv_record.RECEIVED_DATE = master.CREATEDDATE;
                        sv_record.REFERENCE_NO = sv_submission.REFERENCE_NO;
                        //sv_record.LANGUAGE_CODE = SignboardConstant.LANG_CHINESE;
                        //sv_record.WITH_ALTERATION = detail.ASWORKOPTION.ToString();
                        //sv_record.INSPECTION_DATE = detail.INSPECTIONDATE;
                        //sv_record.PROPOSED_ALTERATION_COMM_DATE;
                        sv_record.ACTUAL_ALTERATION_COMM_DATE = detail.COMMENCEDATE;
                        sv_record.ACTUAL_ALTERATION_COMP_DATE = detail.COMPLETEDATE;
                        //sv_record.VALIDATION_EXPIRY_DATE;
                        //sv_record.SIGNBOARD_REMOVAL;
                        //sv_record.SIGNBOARD_REMOVAL_DISCOV_DATE;

                        //sv_record.S_CHK_VS_NO = ;
                        //sv_record.S_CHK_INSP_DATE = ;
                        //sv_record.S_CHK_WORK_DATE = ;
                        //sv_record.S_CHK_SIGNBOARD = ;
                        //sv_record.S_CHK_SIG = ;
                        //sv_record.S_CHK_SIG_DATE = ;
                        //sv_record.S_CHK_MW_ITEM_NO = ;
                        //sv_record.S_CHK_SUPPORT_DOC = ;
                        //sv_record.S_CHK_SBO_PWA_AP =;
                        //sv_record.S_CHK_OTHERS = ;

                        //sv_record.P_CHK_APP_AP_MW_ITEM =;
                        //sv_record.P_CHK_VAL_AP = ; 
                        //sv_record.P_CHK_SIG_AP = ;
                        //sv_record.P_CHK_VAL_RSE = ;
                        //sv_record.P_CHK_SIG_RSE = ;
                        //sv_record.P_CHK_VAL_RI = ;
                        //sv_record.P_CHK_SIG_RI = ;
                        //sv_record.P_CHK_VAL_PRC = ;
                        //sv_record.P_CHK_SIG_AS = ;
                        //sv_record.P_CHK_CAP_AS_MW_ITEM = ;

                        //sv_record.ACK_LETTERISS_DATE;

                        sv_record.STATUS_CODE = SignboardConstant.SV_RECORD_STATUS_CODE_DATA_ENTRY_DRAFT;

                        // FK
                        sv_record.SV_SUBMISSION_ID = sv_submission.UUID;
                        //sv_record.SV_SIGNBOARD_ID = sv_signboard.UUID;
                        sv_record.PAW_ID = paw.UUID;
                        sv_record.OI_ID = oi.UUID;

                        db.B_SV_RECORD.Add(sv_record);
                        db.SaveChanges();
                        #endregion

                        #region professional
                        B_SV_APPOINTED_PROFESSIONAL ap = new B_SV_APPOINTED_PROFESSIONAL();
                        ap.CERTIFICATION_NO = detail.APCRNUM1 + "/" + detail.APCRNUM2;
                        ap.CHINESE_NAME = detail.APNAME;
                        //ap.ENGLISH_NAME = detail.APCFMENGNAME;
                        //ap.AS_CHINESE_NAME;
                        //ap.AS_ENGLISH_NAME;
                        //ap.STATUS_CODE;
                        ap.EXPIRY_DATE = detail.APCREXPIREDDATE;
                        ap.SIGNATURE_DATE = detail.APSIGNDATE;
                        //ap.CONTACT_NO;
                        //ap.FAX_NO;
                        ap.IDENTIFY_FLAG = SignboardConstant.PBP_CODE_AP;

                        // FK
                        ap.SV_RECORD_ID = sv_record.UUID;
                        db.B_SV_APPOINTED_PROFESSIONAL.Add(ap);
                        db.SaveChanges();


                        B_SV_APPOINTED_PROFESSIONAL prc = new B_SV_APPOINTED_PROFESSIONAL();
                        prc.CERTIFICATION_NO = detail.PRCCRNUM1 + "/" + detail.PRCCRNUM2;
                        prc.CHINESE_NAME = detail.PRCCHINAME;
                        prc.ENGLISH_NAME = detail.PRCENGNAME1 + " " + detail.PRCENGNAME2;
                        prc.AS_CHINESE_NAME = detail.PRCASCHINAME;
                        prc.AS_ENGLISH_NAME = detail.PRCASENGNAME;
                        //prc.STATUS_CODE;
                        prc.EXPIRY_DATE = detail.PRCCREXPIREDDATE;
                        prc.SIGNATURE_DATE = detail.PRCSIGNDATE;
                        prc.CONTACT_NO = detail.PRCTEL;
                        prc.FAX_NO = detail.PRCFAX;
                        prc.IDENTIFY_FLAG = SignboardConstant.PRC_CODE;

                        // FK
                        prc.SV_RECORD_ID = sv_record.UUID;
                        db.B_SV_APPOINTED_PROFESSIONAL.Add(prc);
                        db.SaveChanges();

                        B_SV_APPOINTED_PROFESSIONAL rse = new B_SV_APPOINTED_PROFESSIONAL();
                        if (!detail.RSENOTREQUIREDOPTION.ToString().Equals("1")) // 1: NOT REQUIRE
                        {
                            rse.CERTIFICATION_NO = "RSE " + detail.RSECRNUM1 + "/" + detail.RSECRNUM2;
                            rse.CHINESE_NAME = detail.RSENAME;
                            //rse.ENGLISH_NAME = detail.RSECFMENGNAME;
                            //rse.AS_CHINESE_NAME = ;
                            //rse.AS_ENGLISH_NAME;
                            //rse.STATUS_CODE;
                            rse.EXPIRY_DATE = detail.RSECREXPIREDDATE;
                            rse.SIGNATURE_DATE = detail.RSESIGNDATE;
                            //rse.CONTACT_NO;
                            //rse.FAX_NO;
                            rse.IDENTIFY_FLAG = SignboardConstant.PBP_CODE_RSE;
                        }
                        // FK
                        rse.SV_RECORD_ID = sv_record.UUID;
                        db.B_SV_APPOINTED_PROFESSIONAL.Add(rse);
                        db.SaveChanges();

                        B_SV_APPOINTED_PROFESSIONAL rge = new B_SV_APPOINTED_PROFESSIONAL();
                        rge.IDENTIFY_FLAG = SignboardConstant.PBP_CODE_RGE;
                        rge.SV_RECORD_ID = sv_record.UUID;
                        db.B_SV_APPOINTED_PROFESSIONAL.Add(rge);
                        db.SaveChanges();

                        #endregion

                        #region mw items
                        if (!detail.COMPLETEDWORKSAMEASSC01OPTION.ToString().Equals("1")) // 1: TRUE -> SAME
                        {
                            var count = 0;
                            foreach (var item in items)
                            {
                                B_SV_RECORD_ITEM mwitem = new B_SV_RECORD_ITEM();
                                mwitem.MW_ITEM_CODE = item.MWITEMNO;
                                mwitem.LOCATION_DESCRIPTION = item.MWDESC;
                                mwitem.CLASS_CODE = SignboardCommentService.getClassCode(item.MWITEMNO);
                                mwitem.ORDERING = new decimal(count);

                                // FK
                                mwitem.SV_RECORD_ID = sv_record.UUID;
                                db.B_SV_RECORD_ITEM.Add(mwitem);
                                db.SaveChanges();
                                count++;
                            }
                        }
                        #endregion

                        #region validation items
                        //var counter = 0;
                        //// 1(a)
                        //if (detail.INSPECTEDUNAUTHSIGNBOARDITEM1A != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM1A.ToString().Equals("1"))
                        //{
                        //    for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM1AQTY; i++)
                        //    {
                        //        B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                        //        validation.VALIDATION_ITEM = "1(a)";
                        //        validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                        //        validation.ORDERING = new decimal(counter);

                        //        // FK
                        //        validation.SV_RECORD_ID = sv_record.UUID;

                        //        db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                        //        db.SaveChanges();
                        //        counter++;
                        //    }
                        //}
                        //// 1(b)
                        //if (detail.INSPECTEDUNAUTHSIGNBOARDITEM1B != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM1B.ToString().Equals("1"))
                        //{
                        //    for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM1BQTY; i++)
                        //    {
                        //        B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                        //        validation.VALIDATION_ITEM = "1(b)";
                        //        validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                        //        validation.ORDERING = new decimal(counter);

                        //        // FK
                        //        validation.SV_RECORD_ID = sv_record.UUID;
                        //        db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                        //        db.SaveChanges();
                        //        counter++;
                        //    }
                        //}
                        //// 1(c)
                        //if (detail.INSPECTEDUNAUTHSIGNBOARDITEM1C != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM1C.ToString().Equals("1"))
                        //{
                        //    for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM1CQTY; i++)
                        //    {
                        //        B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                        //        validation.VALIDATION_ITEM = "1(c)";
                        //        validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                        //        validation.ORDERING = new decimal(counter);


                        //        // FK
                        //        validation.SV_RECORD_ID = sv_record.UUID;
                        //        db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                        //        db.SaveChanges();
                        //        counter++;
                        //    }
                        //}
                        //// 2(a)
                        //if (detail.INSPECTEDUNAUTHSIGNBOARDITEM2A != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM2A.ToString().Equals("1"))
                        //{
                        //    for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM2AQTY; i++)
                        //    {
                        //        B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                        //        validation.VALIDATION_ITEM = "2(a)";
                        //        validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                        //        validation.ORDERING = new decimal(counter);

                        //        // FK
                        //        validation.SV_RECORD_ID = sv_record.UUID;
                        //        db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                        //        db.SaveChanges();
                        //        counter++;
                        //    }
                        //}
                        //// 2(b)
                        //if (detail.INSPECTEDUNAUTHSIGNBOARDITEM2B != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM2B.ToString().Equals("1"))
                        //{
                        //    for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM2BQTY; i++)
                        //    {
                        //        B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                        //        validation.VALIDATION_ITEM = "2(b)";
                        //        validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                        //        validation.ORDERING = new decimal(counter);

                        //        // FK
                        //        validation.SV_RECORD_ID = sv_record.UUID;
                        //        db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                        //        db.SaveChanges();
                        //        counter++;
                        //    }
                        //}
                        //// 2(c)
                        //if (detail.INSPECTEDUNAUTHSIGNBOARDITEM2C != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM2C.ToString().Equals("1"))
                        //{
                        //    for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM2CQTY; i++)
                        //    {
                        //        B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                        //        validation.VALIDATION_ITEM = "2(c)";
                        //        validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                        //        validation.ORDERING = new decimal(counter);

                        //        // FK
                        //        validation.SV_RECORD_ID = sv_record.UUID;
                        //        db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                        //        db.SaveChanges();
                        //        counter++;
                        //    }
                        //}
                        //// 3
                        //if (detail.INSPECTEDUNAUTHSIGNBOARDITEM3 != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM3.ToString().Equals("1"))
                        //{
                        //    for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM3QTY; i++)
                        //    {
                        //        B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                        //        validation.VALIDATION_ITEM = "3";
                        //        validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                        //        validation.ORDERING = new decimal(counter);

                        //        // FK
                        //        validation.SV_RECORD_ID = sv_record.UUID;
                        //        db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                        //        db.SaveChanges();
                        //        counter++;
                        //    }
                        //}
                        //// 4(a)
                        //if (detail.INSPECTEDUNAUTHSIGNBOARDITEM4A != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM4A.ToString().Equals("1"))
                        //{
                        //    for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM4AQTY; i++)
                        //    {
                        //        B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                        //        validation.VALIDATION_ITEM = "4(a)";
                        //        validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                        //        validation.ORDERING = new decimal(counter);

                        //        // FK
                        //        validation.SV_RECORD_ID = sv_record.UUID;
                        //        db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                        //        db.SaveChanges();
                        //        counter++;
                        //    }
                        //}
                        //// 4(b)
                        //if (detail.INSPECTEDUNAUTHSIGNBOARDITEM4B != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM4B.ToString().Equals("1"))
                        //{
                        //    for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM4BQTY; i++)
                        //    {
                        //        B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                        //        validation.VALIDATION_ITEM = "4(b)";
                        //        validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                        //        validation.ORDERING = new decimal(counter);

                        //        // FK
                        //        validation.SV_RECORD_ID = sv_record.UUID;
                        //        db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                        //        db.SaveChanges();
                        //        counter++;
                        //    }
                        //}
                        //// 5
                        //if (detail.INSPECTEDUNAUTHSIGNBOARDITEM5 != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM5.ToString().Equals("1"))
                        //{
                        //    for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM5QTY; i++)
                        //    {
                        //        B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                        //        validation.VALIDATION_ITEM = "5";
                        //        validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                        //        validation.ORDERING = new decimal(counter);

                        //        // FK
                        //        validation.SV_RECORD_ID = sv_record.UUID;
                        //        db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                        //        db.SaveChanges();
                        //        counter++;
                        //    }
                        //}
                        //// 6
                        //if (detail.INSPECTEDUNAUTHSIGNBOARDITEM6 != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM6.ToString().Equals("1"))
                        //{
                        //    for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM6QTY; i++)
                        //    {
                        //        B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                        //        validation.VALIDATION_ITEM = "6";
                        //        validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                        //        validation.ORDERING = new decimal(counter);

                        //        // FK
                        //        validation.SV_RECORD_ID = sv_record.UUID;
                        //        db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                        //        db.SaveChanges();
                        //        counter++;
                        //    }
                        //}
                        #endregion

                        #region file upload
                        // form
                        B_SV_SCANNED_DOCUMENT sd = db.B_SV_SCANNED_DOCUMENT.Find(sv_submission.SV_SCANNED_DOCUMENT_ID);
                        string fileName = master.FILENAME;
                        string fileExtension = master.FILEEXTENSION;
                        //string DSN = ss.GetNumber();
                        string DSN = sd.DSN_NUMBER;
                        string directory = ApplicationConstant.SMMFilePath + "scan" + getSvDocRelativeFilePath(DSN);
                        string relativePath = getSvDocRelativeFilePath(DSN) + fileName + fileExtension;
                        string fullPath = ApplicationConstant.SMMFilePath + "scan" + relativePath;
                        if (!Directory.Exists(directory))
                        {
                            System.IO.Directory.CreateDirectory(directory);
                        }
                        File.WriteAllBytes(fullPath, master.FILECONTENT);

                        sd.FILE_PATH = fullPath;
                        sd.RELATIVE_FILE_PATH = relativePath;
                        sd.RECORD_ID = sv_submission.REFERENCE_NO;
                        sd.DSN_NUMBER = DSN;
                        sd.RECORD_TYPE = SignboardConstant.SCAN_DOC_RECORD_TYPE_VALIDATION;
                        sd.DOCUMENT_TYPE = "Form";
                        //sd.FOLDER_TYPE = SignboardConstant.SCANNED_DOC_FOLDER_TYPE_PRIVATE_SCU;
                        sd.SUBMIT_TYPE = SignboardConstant.SV_Validation;

                        db.SaveChanges();


                        foreach (var file in files)
                        {
                            string file_name = file.FILENAME;
                            string file_extension = file.FILEEXTENSION;
                            string DSN_no = ss.GetNumber();
                            string file_directory = ApplicationConstant.SMMFilePath + "scan" + getSvDocRelativeFilePath(DSN_no);
                            string relative_path = getSvDocRelativeFilePath(DSN_no) + file_name + file_extension;
                            string full_path = ApplicationConstant.SMMFilePath + "scan" + relative_path;
                            if (!Directory.Exists(file_directory))
                            {
                                System.IO.Directory.CreateDirectory(file_directory);
                            }
                            File.WriteAllBytes(full_path, file.FILECONTENT);

                            B_SV_SCANNED_DOCUMENT doc = new B_SV_SCANNED_DOCUMENT();
                            //doc.AS_THUMBNAIL = "N";
                            doc.FILE_PATH = full_path;
                            sd.RELATIVE_FILE_PATH = relative_path;
                            doc.RECORD_ID = sv_submission.REFERENCE_NO;
                            doc.DSN_NUMBER = DSN_no;
                            doc.RECORD_TYPE = SignboardConstant.SCAN_DOC_RECORD_TYPE_VALIDATION;
                            doc.DOCUMENT_TYPE = file.DOCTYPE;
                            doc.FOLDER_TYPE = file.ATTTYPE;
                            doc.SUBMIT_TYPE = SignboardConstant.SV_Validation;


                            db.B_SV_SCANNED_DOCUMENT.Add(doc);
                            db.SaveChanges();
                        }
                        #endregion

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }

        public void dataEntry_SC02C(string EFSS_ID, SCUR_Models scurModel)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                B_EFSS_FORM_MASTER master = db.B_EFSS_FORM_MASTER.Find(EFSS_ID);
                B_EFSCU_TBL_SC02C detail = db.B_EFSCU_TBL_SC02C.Find(master.FORMCONTENTID);
                List<B_EFSCU_TBL_SC02C_ITEM> items = db.B_EFSCU_TBL_SC02C_ITEM.Where(x => x.SC02CID == detail.ID).ToList();
                List<B_EFSS_FORM_ATTACHMENTS> files = db.B_EFSS_FORM_ATTACHMENTS.Where(x => x.RECVFORMID == master.ID).ToList();

                B_SV_SUBMISSION sv_submission = db.B_SV_SUBMISSION.Where(x => x.REFERENCE_NO == scurModel.SubmissionNo && x.FORM_CODE == scurModel.FormCode)
                    .Include(x => x.B_SV_SCANNED_DOCUMENT)
                    .FirstOrDefault();

                SignboardCommonDAOService ss = new SignboardCommonDAOService();

                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region signboard owner & address
                        B_SV_ADDRESS owner_addr = new B_SV_ADDRESS();
                        //owner_addr.FLAT = detail.SOCORRFLAT;
                        //owner_addr.FLOOR = detail.SOCORRFLOOR;
                        //owner_addr.BUILDINGNAME = detail.SOCORRBUILDING;
                        //owner_addr.STREET = detail.SOCORRSTREET;
                        //owner_addr.STREET_NO = detail.SOCORRSTREETNO;
                        //owner_addr.DISTRICT = detail.SOCORRDISTRICT;
                        //owner_addr.REGION = detail.SOCORRAREA;
                        ////owner_addr.FULL_ADDRESS = ;
                        //owner_addr.FILE_REFERENCE_NO = master.FOURPLUSTWO;
                        ////owner_addr.BCIS_DISTRICT = ;

                        db.B_SV_ADDRESS.Add(owner_addr);
                        db.SaveChanges();

                        B_SV_PERSON_CONTACT owner = new B_SV_PERSON_CONTACT();
                        //owner.NAME_CHINESE = detail.SOCHINAMEOFBUSINESS;
                        //owner.NAME_ENGLISH = detail.SOENGNAMEOFBUSINESS1 + " " + detail.SOENGNAMEOFBUSINESS2;
                        //owner.EMAIL = detail.EMAIL;
                        //owner.CONTACT_NO = detail.SOTEL;
                        //owner.FAX_NO = detail.SOFAX;
                        //owner.ID_TYPE = "3"; // BR #
                        //owner.ID_NUMBER = detail.SOBRN;
                        ////owner.ID_ISSUE_COUNTRY = ;
                        ////owner.CONTACT_PERSON_TITLE = ;
                        ////owner.FIRST_NAME = ;
                        ////owner.LAST_NAME = ;
                        ////owner.MOBILE = ;
                        ////owner.SIGNATURE_DATE = ;
                        ////owner.PRC_NAME = ;
                        ////owner.PRC_CONTACT_NO = ;
                        ////owner.PBP_NAME;
                        ////owner.PBP_CONTACT_NO = ;
                        ////owner.PAW_SAME_AS;

                        // FK
                        owner.SV_ADDRESS_ID = owner_addr.UUID;
                        db.B_SV_PERSON_CONTACT.Add(owner);
                        db.SaveChanges();

                        #endregion

                        #region paw & address
                        B_SV_ADDRESS paw_addr = new B_SV_ADDRESS();
                        ////paw_addr.FLAT = 
                        ////paw_addr.FLOOR = 
                        ////paw_addr.BUILDINGNAME = 
                        ////paw_addr.STREET = 
                        ////paw_addr.STREET_NO =
                        ////paw_addr.DISTRICT = 
                        ////paw_addr.REGION = 


                        db.B_SV_ADDRESS.Add(paw_addr);
                        db.SaveChanges();

                        B_SV_PERSON_CONTACT paw = new B_SV_PERSON_CONTACT();
                        //paw.NAME_CHINESE = detail.ARRNAME;
                        ////paw.NAME_ENGLISH = 
                        ////paw.EMAIL = 
                        ////paw.CONTACT_NO = 
                        ////paw.FAX_NO = 
                        ////paw.ID_TYPE = 
                        ////paw.ID_NUMBER = 
                        ////paw.ID_ISSUE_COUNTRY
                        ////paw.CONTACT_PERSON_TITLE = 
                        ////paw.FIRST_NAME = 
                        ////paw.LAST_NAME = 
                        ////paw.MOBILE = 
                        ////paw.SIGNATURE_DATE = 
                        ////paw.PRC_NAME = 
                        ////paw.PRC_CONTACT_NO = 
                        ////paw.PBP_NAME = 
                        ////paw.PBP_CONTACT_NO = 
                        //string paw_same_as = "";
                        //if (detail.ARRISAPOPTION.ToString().Equals("1"))
                        //{
                        //    paw_same_as = "PBP";
                        //}
                        //else if (detail.ARRISRGBCOPTION.ToString().Equals("1"))
                        //{
                        //    paw_same_as = "PBP";
                        //}
                        //else if (detail.ARRISRMWCTYPECOPTION.ToString().Equals("1"))
                        //{
                        //    paw_same_as = "PRC";
                        //}
                        //else if (detail.ARRISRSEOPTION.ToString().Equals("1"))
                        //{
                        //    paw_same_as = "PBP";
                        //}
                        //else if (detail.ARRISSOOPTION.ToString().Equals("1"))
                        //{
                        //    paw_same_as = "SB_OWNER";
                        //}
                        //paw.PAW_SAME_AS = paw_same_as;


                        // FK
                        paw.SV_ADDRESS_ID = paw_addr.UUID;
                        db.B_SV_PERSON_CONTACT.Add(paw);
                        db.SaveChanges();

                        #endregion

                        #region oi & address
                        B_SV_ADDRESS oi_addr = new B_SV_ADDRESS();
                        B_SV_PERSON_CONTACT oi = new B_SV_PERSON_CONTACT();
                        //if (detail.OWNERCORPFORMEDOPTION.ToString().Equals("1"))
                        //{
                        //    //oi_addr.BLOCK = 
                        //    oi_addr.STREET = detail.OWNERCORPSTREET;
                        //    oi_addr.STREET_NO = detail.OWNERCORPSTREETNO;
                        //    oi_addr.BUILDINGNAME = detail.OWNERCORPBUILDING;
                        //    oi_addr.FLOOR = detail.OWNERCORPFLOOR;
                        //    oi_addr.FLAT = detail.OWNERCORPFLAT;
                        //    oi_addr.DISTRICT = detail.OWNERCORPDISTRICT;
                        //    oi_addr.REGION = detail.OWNERCORPAREA;
                        db.B_SV_ADDRESS.Add(oi_addr);
                        db.SaveChanges();


                        //    //oi.NAME_CHINESE = 
                        //    //oi.NAME_ENGLISH = 
                        //    //oi.EMAIL = 
                        //    //oi.CONTACT_NO = 
                        //    //oi.FAX_NO = 
                        //    //oi.ID_TYPE = 
                        //    //oi.ID_NUMBER = 
                        //    //oi.ID_ISSUE_COUNTRY = 
                        //    //oi.CONTACT_PERSON_TITLE = 
                        //    //oi.FIRST_NAME = 
                        //    //oi.LAST_NAME = 
                        //    //oi.MOBILE = 
                        //    //oi.SIGNATURE_DATE = 
                        //    //oi.PRC_NAME = 
                        //    //oi.PRC_CONTACT_NO = 
                        //    //oi.PBP_NAME = 
                        //    //oi.PBP_CONTACT_NO = 
                        //    //oi.PAW_SAME_AS = 

                        // FK
                        oi.SV_ADDRESS_ID = oi_addr.UUID;

                        db.B_SV_PERSON_CONTACT.Add(oi);
                        db.SaveChanges();
                        //}
                        #endregion

                        #region signboard & address
                        B_SV_ADDRESS signboard_addr = new B_SV_ADDRESS();
                        //signboard_addr.FLAT = detail.UNAUTHSIGNBOARDFLAT;
                        //signboard_addr.FLOOR = detail.UNAUTHSIGNBOARDFLOOR;
                        //signboard_addr.BUILDINGNAME = detail.BUILDING;
                        //signboard_addr.STREET_NO = detail.STREETNO;
                        //signboard_addr.STREET = detail.STREETROADVILLAGE;
                        //signboard_addr.DISTRICT = detail.UNAUTHSIGNBOARDDISTRICT;
                        //signboard_addr.REGION = detail.UNAUTHSIGNBOARDAREA;
                        ////signboard_addr.FULL_ADDRESS = detail.;

                        db.B_SV_ADDRESS.Add(signboard_addr);
                        db.SaveChanges();

                        B_SV_SIGNBOARD sv_signboard = new B_SV_SIGNBOARD();
                        //sv_signboard.LOCATION_OF_SIGNBOARD = detail.UNAUTHSIGNBOARDDETAILEDLOCATION;
                        ////sv_signboard.RVD_NO = ;
                        ////sv_signboard.FACADE = ;
                        ////sv_signboard.TYPE = ;
                        ////sv_signboard.BTM_FLOOR = ;
                        ////sv_signboard.TOP_FLOOR = ;
                        ////sv_signboard.A_M2 = ;
                        ////sv_signboard.P_M = ;
                        ////sv_signboard.H_M = ;
                        ////sv_signboard.H2_M = ;
                        ////sv_signboard.LED = ;
                        ////sv_signboard.BUILDING_PORTION = ;
                        ////sv_signboard.STATUS = ;
                        ////sv_signboard.DESCRIPTION = ;
                        ////sv_signboard.S24_ORDER_NO = ;
                        ////sv_signboard.S24_ORDER_TYPE = ;

                        // FK
                        sv_signboard.LOCATION_ADDRESS_ID = signboard_addr.UUID;
                        sv_signboard.OWNER_ID = owner.UUID;

                        db.B_SV_SIGNBOARD.Add(sv_signboard);
                        db.SaveChanges();


                        #endregion

                        #region sv_record
                        B_SV_RECORD sv_record = new B_SV_RECORD();
                        //sv_record.TO_USER_ID;
                        //sv_record.PO_USER_ID;
                        //sv_record.SPO_USER_ID;

                        //sv_record.AREA_CODE;
                        //sv_record.RECOMMENDATION;
                        //sv_record.NO_OF_SIGNBOARD_VALIDATED;
                        //sv_record.NO_OF_SIGNBOARD_INVOLVED;
                        //sv_record.LSO_VS_OPERATION_YEAR;
                        //sv_record.PREVIOUS_SUBMISSION_NUMBER;
                        //sv_record.NO_OF_SUBMISSION_ERECTION;
                        //sv_record.NO_OF_SUBMISSION_REMOVAL;

                        //sv_record.VALIDITY_AP;
                        //sv_record.SIGNATURE_AP;
                        //sv_record.ITEM_STATED;
                        //sv_record.VALIDITY_PRC;
                        //sv_record.SIGNATURE_AS;
                        //sv_record.INFO_SIGNBOARD_OWNER_PROVIDED;
                        //sv_record.OTHER_IRREGULARITIES;

                        sv_record.RECEIVED_DATE = master.CREATEDDATE;
                        sv_record.REFERENCE_NO = sv_submission.REFERENCE_NO;
                        //sv_record.LANGUAGE_CODE = SignboardConstant.LANG_CHINESE;
                        //sv_record.WITH_ALTERATION = detail.ASWORKOPTION.ToString();
                        //sv_record.INSPECTION_DATE = detail.INSPECTIONDATE;
                        //sv_record.PROPOSED_ALTERATION_COMM_DATE;
                        sv_record.ACTUAL_ALTERATION_COMM_DATE = detail.COMMENCEDATE;
                        sv_record.ACTUAL_ALTERATION_COMP_DATE = detail.COMPLETEDATE;
                        //sv_record.VALIDATION_EXPIRY_DATE;
                        //sv_record.SIGNBOARD_REMOVAL;
                        //sv_record.SIGNBOARD_REMOVAL_DISCOV_DATE;

                        //sv_record.S_CHK_VS_NO = ;
                        //sv_record.S_CHK_INSP_DATE = ;
                        //sv_record.S_CHK_WORK_DATE = ;
                        //sv_record.S_CHK_SIGNBOARD = ;
                        //sv_record.S_CHK_SIG = ;
                        //sv_record.S_CHK_SIG_DATE = ;
                        //sv_record.S_CHK_MW_ITEM_NO = ;
                        //sv_record.S_CHK_SUPPORT_DOC = ;
                        //sv_record.S_CHK_SBO_PWA_AP =;
                        //sv_record.S_CHK_OTHERS = ;

                        //sv_record.P_CHK_APP_AP_MW_ITEM =;
                        //sv_record.P_CHK_VAL_AP = ; 
                        //sv_record.P_CHK_SIG_AP = ;
                        //sv_record.P_CHK_VAL_RSE = ;
                        //sv_record.P_CHK_SIG_RSE = ;
                        //sv_record.P_CHK_VAL_RI = ;
                        //sv_record.P_CHK_SIG_RI = ;
                        //sv_record.P_CHK_VAL_PRC = ;
                        //sv_record.P_CHK_SIG_AS = ;
                        //sv_record.P_CHK_CAP_AS_MW_ITEM = ;

                        //sv_record.ACK_LETTERISS_DATE;

                        sv_record.STATUS_CODE = SignboardConstant.SV_RECORD_STATUS_CODE_DATA_ENTRY_DRAFT;

                        // FK
                        sv_record.SV_SUBMISSION_ID = sv_submission.UUID;
                        sv_record.SV_SIGNBOARD_ID = sv_signboard.UUID;
                        sv_record.PAW_ID = paw.UUID;
                        sv_record.OI_ID = oi.UUID;

                        db.B_SV_RECORD.Add(sv_record);
                        db.SaveChanges();
                        #endregion

                        #region professional
                        B_SV_APPOINTED_PROFESSIONAL prc = new B_SV_APPOINTED_PROFESSIONAL();
                        prc.CERTIFICATION_NO = detail.PRCCRNUM1 + "/" + detail.PRCCRNUM2;
                        prc.CHINESE_NAME = detail.PRCCHINAME;
                        prc.ENGLISH_NAME = detail.PRCENGNAME1 + " " + detail.PRCENGNAME2;
                        prc.AS_CHINESE_NAME = detail.PRCASCHINAME;
                        prc.AS_ENGLISH_NAME = detail.PRCASENGNAME;
                        //prc.STATUS_CODE;
                        prc.EXPIRY_DATE = detail.PRCCREXPIREDDATE;
                        prc.SIGNATURE_DATE = detail.PRCSIGNDATE;
                        prc.CONTACT_NO = detail.PRCTEL;
                        prc.FAX_NO = detail.PRCFAX;
                        prc.IDENTIFY_FLAG = SignboardConstant.PRC_CODE;

                        // FK
                        prc.SV_RECORD_ID = sv_record.UUID;
                        db.B_SV_APPOINTED_PROFESSIONAL.Add(prc);
                        db.SaveChanges();

                        B_SV_APPOINTED_PROFESSIONAL ap = new B_SV_APPOINTED_PROFESSIONAL();
                        B_SV_APPOINTED_PROFESSIONAL rse = new B_SV_APPOINTED_PROFESSIONAL();
                        B_SV_APPOINTED_PROFESSIONAL rge = new B_SV_APPOINTED_PROFESSIONAL();

                        if(!detail.APISRCOPTION.ToString().Equals("1")) // 1: is PRC -> skip this
                        {
                            if ((detail.APISAPOPTION.ToString().Equals("1"))) // 1: is AP
                            {
                                ap.CERTIFICATION_NO = detail.APCRNUM1 + "/" + detail.APCRNUM2;
                                ap.CHINESE_NAME = detail.APCHINAME;
                                ap.ENGLISH_NAME = detail.APENGNAME;
                                //ap.AS_CHINESE_NAME;
                                //ap.AS_ENGLISH_NAME;
                                //ap.STATUS_CODE;
                                ap.EXPIRY_DATE = detail.APCREXPIREDDATE;
                                ap.SIGNATURE_DATE = detail.APSIGNDATE;
                                //ap.CONTACT_NO;
                                //ap.FAX_NO;
                            }
                            else if (detail.APISRSEOPTION.ToString().Equals("1")) // 1: is RSE
                            {
                                rse.CERTIFICATION_NO = detail.APCRNUM1 + "/" + detail.APCRNUM2;
                                rse.CHINESE_NAME = detail.APCHINAME;
                                rse.ENGLISH_NAME = detail.APENGNAME;
                                //ap.AS_CHINESE_NAME;
                                //ap.AS_ENGLISH_NAME;
                                //ap.STATUS_CODE;
                                rse.EXPIRY_DATE = detail.APCREXPIREDDATE;
                                rse.SIGNATURE_DATE = detail.APSIGNDATE;
                                //ap.CONTACT_NO;
                                //ap.FAX_NO;
                            }
                            else if(detail.APISRIOPTION.ToString().Equals("1")) // 1: is RI (RGE)
                            {
                                rge.CERTIFICATION_NO = detail.APCRNUM1 + "/" + detail.APCRNUM2;
                                rge.CHINESE_NAME = detail.APCHINAME;
                                rge.ENGLISH_NAME = detail.APENGNAME;
                                //ap.AS_CHINESE_NAME;
                                //ap.AS_ENGLISH_NAME;
                                //ap.STATUS_CODE;
                                rge.EXPIRY_DATE = detail.APCREXPIREDDATE;
                                rge.SIGNATURE_DATE = detail.APSIGNDATE;
                                //ap.CONTACT_NO;
                                //ap.FAX_NO;
                            }
                            ap.IDENTIFY_FLAG = SignboardConstant.PBP_CODE_AP;
                            rse.IDENTIFY_FLAG = SignboardConstant.PBP_CODE_RSE;
                            rge.IDENTIFY_FLAG = SignboardConstant.PBP_CODE_RGE;

                            // FK
                            ap.SV_RECORD_ID = sv_record.UUID;
                            rse.SV_RECORD_ID = sv_record.UUID;
                            rge.SV_RECORD_ID = sv_record.UUID;
                            db.B_SV_APPOINTED_PROFESSIONAL.Add(ap);
                            db.B_SV_APPOINTED_PROFESSIONAL.Add(rse);
                            db.B_SV_APPOINTED_PROFESSIONAL.Add(rge);
                            db.SaveChanges();
                        }

                        

                        #endregion

                        #region mw items
                        if (!detail.COMPLETEDWORKSAMEASSC02OPTION.ToString().Equals("1")) // 1: TRUE -> SAME
                        {
                            var count = 0;
                            foreach (var item in items)
                            {
                                B_SV_RECORD_ITEM mwitem = new B_SV_RECORD_ITEM();
                                mwitem.MW_ITEM_CODE = item.MWITEMNO;
                                mwitem.LOCATION_DESCRIPTION = item.MWDESC;
                                mwitem.CLASS_CODE = SignboardCommentService.getClassCode(item.MWITEMNO);
                                mwitem.ORDERING = new decimal(count);

                                // FK
                                mwitem.SV_RECORD_ID = sv_record.UUID;
                                db.B_SV_RECORD_ITEM.Add(mwitem);
                                db.SaveChanges();
                                count++;
                            }
                        }
                        #endregion

                        #region validation items
                        //var counter = 0;
                        //// 1(a)
                        //if (detail.INSPECTEDUNAUTHSIGNBOARDITEM1A != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM1A.ToString().Equals("1"))
                        //{
                        //    for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM1AQTY; i++)
                        //    {
                        //        B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                        //        validation.VALIDATION_ITEM = "1(a)";
                        //        validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                        //        validation.ORDERING = new decimal(counter);

                        //        // FK
                        //        validation.SV_RECORD_ID = sv_record.UUID;

                        //        db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                        //        db.SaveChanges();
                        //        counter++;
                        //    }
                        //}
                        //// 1(b)
                        //if (detail.INSPECTEDUNAUTHSIGNBOARDITEM1B != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM1B.ToString().Equals("1"))
                        //{
                        //    for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM1BQTY; i++)
                        //    {
                        //        B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                        //        validation.VALIDATION_ITEM = "1(b)";
                        //        validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                        //        validation.ORDERING = new decimal(counter);

                        //        // FK
                        //        validation.SV_RECORD_ID = sv_record.UUID;
                        //        db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                        //        db.SaveChanges();
                        //        counter++;
                        //    }
                        //}
                        //// 1(c)
                        //if (detail.INSPECTEDUNAUTHSIGNBOARDITEM1C != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM1C.ToString().Equals("1"))
                        //{
                        //    for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM1CQTY; i++)
                        //    {
                        //        B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                        //        validation.VALIDATION_ITEM = "1(c)";
                        //        validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                        //        validation.ORDERING = new decimal(counter);


                        //        // FK
                        //        validation.SV_RECORD_ID = sv_record.UUID;
                        //        db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                        //        db.SaveChanges();
                        //        counter++;
                        //    }
                        //}
                        //// 2(a)
                        //if (detail.INSPECTEDUNAUTHSIGNBOARDITEM2A != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM2A.ToString().Equals("1"))
                        //{
                        //    for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM2AQTY; i++)
                        //    {
                        //        B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                        //        validation.VALIDATION_ITEM = "2(a)";
                        //        validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                        //        validation.ORDERING = new decimal(counter);

                        //        // FK
                        //        validation.SV_RECORD_ID = sv_record.UUID;
                        //        db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                        //        db.SaveChanges();
                        //        counter++;
                        //    }
                        //}
                        //// 2(b)
                        //if (detail.INSPECTEDUNAUTHSIGNBOARDITEM2B != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM2B.ToString().Equals("1"))
                        //{
                        //    for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM2BQTY; i++)
                        //    {
                        //        B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                        //        validation.VALIDATION_ITEM = "2(b)";
                        //        validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                        //        validation.ORDERING = new decimal(counter);

                        //        // FK
                        //        validation.SV_RECORD_ID = sv_record.UUID;
                        //        db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                        //        db.SaveChanges();
                        //        counter++;
                        //    }
                        //}
                        //// 2(c)
                        //if (detail.INSPECTEDUNAUTHSIGNBOARDITEM2C != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM2C.ToString().Equals("1"))
                        //{
                        //    for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM2CQTY; i++)
                        //    {
                        //        B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                        //        validation.VALIDATION_ITEM = "2(c)";
                        //        validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                        //        validation.ORDERING = new decimal(counter);

                        //        // FK
                        //        validation.SV_RECORD_ID = sv_record.UUID;
                        //        db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                        //        db.SaveChanges();
                        //        counter++;
                        //    }
                        //}
                        //// 3
                        //if (detail.INSPECTEDUNAUTHSIGNBOARDITEM3 != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM3.ToString().Equals("1"))
                        //{
                        //    for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM3QTY; i++)
                        //    {
                        //        B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                        //        validation.VALIDATION_ITEM = "3";
                        //        validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                        //        validation.ORDERING = new decimal(counter);

                        //        // FK
                        //        validation.SV_RECORD_ID = sv_record.UUID;
                        //        db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                        //        db.SaveChanges();
                        //        counter++;
                        //    }
                        //}
                        //// 4(a)
                        //if (detail.INSPECTEDUNAUTHSIGNBOARDITEM4A != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM4A.ToString().Equals("1"))
                        //{
                        //    for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM4AQTY; i++)
                        //    {
                        //        B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                        //        validation.VALIDATION_ITEM = "4(a)";
                        //        validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                        //        validation.ORDERING = new decimal(counter);

                        //        // FK
                        //        validation.SV_RECORD_ID = sv_record.UUID;
                        //        db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                        //        db.SaveChanges();
                        //        counter++;
                        //    }
                        //}
                        //// 4(b)
                        //if (detail.INSPECTEDUNAUTHSIGNBOARDITEM4B != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM4B.ToString().Equals("1"))
                        //{
                        //    for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM4BQTY; i++)
                        //    {
                        //        B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                        //        validation.VALIDATION_ITEM = "4(b)";
                        //        validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                        //        validation.ORDERING = new decimal(counter);

                        //        // FK
                        //        validation.SV_RECORD_ID = sv_record.UUID;
                        //        db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                        //        db.SaveChanges();
                        //        counter++;
                        //    }
                        //}
                        //// 5
                        //if (detail.INSPECTEDUNAUTHSIGNBOARDITEM5 != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM5.ToString().Equals("1"))
                        //{
                        //    for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM5QTY; i++)
                        //    {
                        //        B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                        //        validation.VALIDATION_ITEM = "5";
                        //        validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                        //        validation.ORDERING = new decimal(counter);

                        //        // FK
                        //        validation.SV_RECORD_ID = sv_record.UUID;
                        //        db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                        //        db.SaveChanges();
                        //        counter++;
                        //    }
                        //}
                        //// 6
                        //if (detail.INSPECTEDUNAUTHSIGNBOARDITEM6 != null && detail.INSPECTEDUNAUTHSIGNBOARDITEM6.ToString().Equals("1"))
                        //{
                        //    for (int i = 0; i < (int)detail.INSPECTEDUNAUTHSIGNBOARDITEM6QTY; i++)
                        //    {
                        //        B_SV_RECORD_VALIDATION_ITEM validation = new B_SV_RECORD_VALIDATION_ITEM();
                        //        validation.VALIDATION_ITEM = "6";
                        //        validation.CORRESPONDING_ITEM = getCorrespondingMwItem(validation.VALIDATION_ITEM);
                        //        validation.ORDERING = new decimal(counter);

                        //        // FK
                        //        validation.SV_RECORD_ID = sv_record.UUID;
                        //        db.B_SV_RECORD_VALIDATION_ITEM.Add(validation);
                        //        db.SaveChanges();
                        //        counter++;
                        //    }
                        //}
                        #endregion

                        #region file upload
                        // form
                        B_SV_SCANNED_DOCUMENT sd = db.B_SV_SCANNED_DOCUMENT.Find(sv_submission.SV_SCANNED_DOCUMENT_ID);
                        string fileName = master.FILENAME;
                        string fileExtension = master.FILEEXTENSION;
                        //string DSN = ss.GetNumber();
                        string DSN = sd.DSN_NUMBER;
                        string directory = ApplicationConstant.SMMFilePath + "scan" + getSvDocRelativeFilePath(DSN);
                        string relativePath = getSvDocRelativeFilePath(DSN) + fileName + fileExtension;
                        string fullPath = ApplicationConstant.SMMFilePath + "scan" + relativePath;
                        if (!Directory.Exists(directory))
                        {
                            System.IO.Directory.CreateDirectory(directory);
                        }
                        File.WriteAllBytes(fullPath, master.FILECONTENT);

                        sd.FILE_PATH = fullPath;
                        sd.RELATIVE_FILE_PATH = relativePath;
                        sd.RECORD_ID = sv_submission.REFERENCE_NO;
                        sd.DSN_NUMBER = DSN;
                        sd.RECORD_TYPE = SignboardConstant.SCAN_DOC_RECORD_TYPE_VALIDATION;
                        sd.DOCUMENT_TYPE = "Form";
                        //sd.FOLDER_TYPE = SignboardConstant.SCANNED_DOC_FOLDER_TYPE_PRIVATE_SCU;
                        sd.SUBMIT_TYPE = SignboardConstant.SV_Validation;

                        db.SaveChanges();


                        foreach (var file in files)
                        {
                            string file_name = file.FILENAME;
                            string file_extension = file.FILEEXTENSION;
                            string DSN_no = ss.GetNumber();
                            string file_directory = ApplicationConstant.SMMFilePath + "scan" + getSvDocRelativeFilePath(DSN_no);
                            string relative_path = getSvDocRelativeFilePath(DSN_no) + file_name + file_extension;
                            string full_path = ApplicationConstant.SMMFilePath + "scan" + relative_path;
                            if (!Directory.Exists(file_directory))
                            {
                                System.IO.Directory.CreateDirectory(file_directory);
                            }
                            File.WriteAllBytes(full_path, file.FILECONTENT);

                            B_SV_SCANNED_DOCUMENT doc = new B_SV_SCANNED_DOCUMENT();
                            //doc.AS_THUMBNAIL = "N";
                            doc.FILE_PATH = full_path;
                            sd.RELATIVE_FILE_PATH = relative_path;
                            doc.RECORD_ID = sv_submission.REFERENCE_NO;
                            doc.DSN_NUMBER = DSN_no;
                            doc.RECORD_TYPE = SignboardConstant.SCAN_DOC_RECORD_TYPE_VALIDATION;
                            doc.DOCUMENT_TYPE = file.DOCTYPE;
                            doc.FOLDER_TYPE = file.ATTTYPE;
                            doc.SUBMIT_TYPE = SignboardConstant.SV_Validation;


                            db.B_SV_SCANNED_DOCUMENT.Add(doc);
                            db.SaveChanges();
                        }
                        #endregion

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }

        public string getCorrespondingMwItem(string code)
        {
            using(EntitiesSignboard db = new EntitiesSignboard())
            {
                var result = db.B_S_SYSTEM_VALUE.Include(x => x.B_S_SYSTEM_TYPE)
                    .Where(x => x.B_S_SYSTEM_TYPE.TYPE == SignboardConstant.SYSTEM_TYPE_VALIDATION_ITEM && x.CODE == code)
                    .FirstOrDefault();
                return result != null ? result.DESCRIPTION : code;
            }
        }

        public string getSvDocRelativeFilePath(string DSN)
        {
            DateTime now = System.DateTime.Now;
            string fileSeparator = Char.ToString(ApplicationConstant.FileSeparator);
            string relativePath = fileSeparator + now.Year + fileSeparator + now.Month;
            relativePath += fileSeparator + DSN + fileSeparator;

            return relativePath; // eg. "\2014\11\D0000001723\"
        }
        
    }
}