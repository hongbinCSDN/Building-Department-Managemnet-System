using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Areas.MWProcessing.Models;
using Oracle.ManagedDataAccess.Client;
using MWMS2.Entity;
using System.Data.Entity;
using MWMS2.Models;
using MWMS2.Constant;
using MWMS2.Services;
using MWMS2.Utility;

namespace MWMS2.Services.ProcessingDAO.DAO
{
    public class ProcessingMWUGEDAOService : ProcessingSystemValueDAOService
    {
        private ProcessingMWDSNDAOService _DA_MW_DSN;
        protected ProcessingMWDSNDAOService DA_MW_DSN
        {
            get
            {
                return _DA_MW_DSN ?? (_DA_MW_DSN = new ProcessingMWDSNDAOService());
            }
        }

        private ProcessingMwCommentService _DA_MwComment;
        protected ProcessingMwCommentService DA_MwComment
        {
            get
            {
                return _DA_MwComment ?? (new ProcessingMwCommentService());
            }
        }

        private const string search_q = @"select * from P_MW_DSN mw
                                            inner join P_S_SYSTEM_VALUE sv
                                            on mw.SCANNED_STATUS_ID=sv.uuid
                                            inner join (select uuid,to_char(modified_date,'HH24:MI:SS') as MODIFIED_TIME from P_MW_DSN) md_time
                                            on md_time.uuid=mw.uuid
                                                where 1=1 ";

        private const string searchEnquiry_q = @"SELECT    mwdsn.uuid,
                                                           mwdsn.dsn,
                                                           CATEGORY_CODE,
                                                           mwref.REFERENCE_NO
                                                    FROM   p_mw_dsn mwdsn
                                                           LEFT JOIN P_MW_REFERENCE_NO mwref
                                                                  ON mwref.REFERENCE_NO = mwdsn.record_id
                                                    WHERE  1 = 1 AND mwdsn.dsn =:DSN ";

        private const string searchScannedDoc_q = @"select * from P_MW_SCANNED_DOCUMENT
                                                        where 1=1 ";

        public Fn02MWUR_GEModel Search(Fn02MWUR_GEModel model)
        {
            model.Query = search_q;
            model.Search();
            return model;
        }

        public Fn02MWUR_GEModel Enquiry(string dsn)
        {
            BaseDAOService baseDAO = new BaseDAOService();
            OracleParameter[] param = new OracleParameter[]
            {
                new OracleParameter("DSN",dsn)
            };
            Fn02MWUR_GEEnquiryModel enquiry = baseDAO.GetObjectData<Fn02MWUR_GEEnquiryModel>(searchEnquiry_q, param).FirstOrDefault();
            Fn02MWUR_GEModel model = new Fn02MWUR_GEModel();
            if (enquiry != null)
            {
                model.DSNID = enquiry.UUID;
                model.Enquiry = enquiry;
            }


            return model;
        }

        public Fn02MWUR_GEModel GetScannedDoc(Fn02MWUR_GEModel model)
        {
            model.Query = searchScannedDoc_q;
            model.Search();
            return model;
        }

        public ServiceResult Submit1(Fn02MWUR_GEModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        P_MW_DSN mwDSN = db.P_MW_DSN.Where(m => m.DSN == model.Enquiry.DSN).FirstOrDefault();

                        // Get mw generate record
                        P_MW_REFERENCE_NO referNo = db.P_MW_REFERENCE_NO.Where(m => m.REFERENCE_NO == mwDSN.RECORD_ID).FirstOrDefault();
                        P_S_SYSTEM_VALUE lang = db.P_S_SYSTEM_VALUE.Where(m => m.CODE == ProcessingConstant.LANG_ENGLISH).FirstOrDefault();
                        P_MW_GENERAL_RECORD mwGeneralRecord = GetMwGeneralRecord(referNo, lang);

                        P_MW_PERSON_CONTACT p_MW_PERSON_CONTACT = new P_MW_PERSON_CONTACT();
                        db.P_MW_PERSON_CONTACT.Add(p_MW_PERSON_CONTACT);
                        db.SaveChanges();
                        mwGeneralRecord.CONTACT_ID = p_MW_PERSON_CONTACT.UUID;
                        mwGeneralRecord.P_MW_PERSON_CONTACT = p_MW_PERSON_CONTACT;

                        P_MW_ADDRESS p_MW_ADDRESS = new P_MW_ADDRESS();
                        db.P_MW_ADDRESS.Add(p_MW_ADDRESS);
                        db.SaveChanges();
                        p_MW_PERSON_CONTACT.MW_ADDRESS_ID = p_MW_ADDRESS.UUID;

                        db.P_MW_GENERAL_RECORD.Add(mwGeneralRecord);

                        // Update Dsn When Create Record
                        P_S_SYSTEM_VALUE status = db.P_S_SYSTEM_VALUE.Where(m => m.CODE == ProcessingConstant.DSN_CONFIRMED).FirstOrDefault();
                        mwDSN = UpdateDSNWhenCreateRecord(mwDSN, status, model.Enquiry.REFERENCE_NO);

                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Console.WriteLine(ex.Message);
                        return new ServiceResult()
                        {
                            Result = ServiceResult.RESULT_FAILURE
                        };
                    }
                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_SUCCESS
                    };
                }
            }
        }

        protected P_MW_GENERAL_RECORD GetMwGeneralRecord(P_MW_REFERENCE_NO referNo, P_S_SYSTEM_VALUE lang)
        {
            P_MW_GENERAL_RECORD mwGeneralRecord = new P_MW_GENERAL_RECORD();
            if (referNo != null)
            {
                mwGeneralRecord.REFERENCE_NUMBER = referNo.UUID;
                mwGeneralRecord.FORM_STATUS = ProcessingConstant.GENERAL_RECORD_NEW;
                //mwGeneralRecord.SUBMIT_TYPE = ProcessingConstant.SUBMIT_TYPE_ENQ;
                if (referNo.REFERENCE_NO.ToUpper().IndexOf(ProcessingConstant.PREFIX_ENQ.ToUpper()) >= 0)
                {
                    mwGeneralRecord.SUBMIT_TYPE = ProcessingConstant.SUBMIT_TYPE_ENQ;
                }
                else if (referNo.REFERENCE_NO.ToUpper().IndexOf(ProcessingConstant.PREFIX_COMP.ToUpper()) >= 0)
                {
                    mwGeneralRecord.SUBMIT_TYPE = ProcessingConstant.SUBMIT_TYPE_COM;
                }

                mwGeneralRecord.LANGUAGE_ID = lang.UUID;
                mwGeneralRecord.STATUS = ProcessingConstant.ENQUIRY_STATUS_OUTSTANDING;

            }
            return mwGeneralRecord;
        }
        protected P_MW_DSN UpdateDSNWhenCreateRecord(P_MW_DSN mwDSN, P_S_SYSTEM_VALUE status, string refNo)
        {
            mwDSN.RECORD_ID = refNo;
            mwDSN.SCANNED_STATUS_ID = status.UUID;
            return mwDSN;
        }

        public Fn02MWUR_GEModel GetMWGeneralRecord(Fn02MWUR_GEModel model)
        {

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_MW_REFERENCE_NO referNo = db.P_MW_REFERENCE_NO.Where(m => m.REFERENCE_NO == model.Enquiry.REFERENCE_NO).FirstOrDefault();
                model.P_MW_GENERAL_RECORD = MWGeneralRecordService.MWGeneralRecordByCaseNo(referNo.UUID);
                if (model.P_MW_GENERAL_RECORD == null)
                {
                    return model;
                }
                model.P_MW_PERSON_CONTACT = db.P_MW_PERSON_CONTACT.Where(m => m.UUID == model.P_MW_GENERAL_RECORD.CONTACT_ID).FirstOrDefault();
                model.P_MW_ADDRESS = model.P_MW_PERSON_CONTACT == null ? null : db.P_MW_ADDRESS.Where(m => m.UUID == model.P_MW_PERSON_CONTACT.MW_ADDRESS_ID).FirstOrDefault();
                model.P_MW_REFERENCE_NO = db.P_MW_REFERENCE_NO.Where(m => m.UUID == model.P_MW_GENERAL_RECORD.REFERENCE_NUMBER).FirstOrDefault();
                model.Language = db.P_S_SYSTEM_VALUE.Where(m => m.UUID == model.P_MW_GENERAL_RECORD.LANGUAGE_ID).FirstOrDefault();

                return model;
            }

        }

        public ServiceResult SaveEntry(Fn02MWUR_GEModel model, bool isSubmit)
        {

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {

                    try
                    {
                        P_MW_DSN p_MW_DSN = db.P_MW_DSN.Where(m => m.DSN == model.DSN).FirstOrDefault();
                        String RefNo = "";
                        if (p_MW_DSN != null)
                        {
                            P_S_SYSTEM_VALUE sv = ProcessingSystemValueService.GetSystemListByCode(ProcessingConstant.GENERAL_ENTRY).FirstOrDefault();
                            p_MW_DSN.SCANNED_STATUS_ID = sv.UUID;
                            RefNo = p_MW_DSN.RECORD_ID;
                        }

                        P_MW_ADDRESS p_MW_ADDRESS = db.P_MW_ADDRESS.Where(m => m.UUID == model.P_MW_ADDRESS.UUID).FirstOrDefault();
                        if (p_MW_ADDRESS != null)
                        {
                            p_MW_ADDRESS.CHINESE_STREET_NAME = model.P_MW_ADDRESS.CHINESE_STREET_NAME;
                            p_MW_ADDRESS.STREET_NO = model.P_MW_ADDRESS.STREET_NO;
                            p_MW_ADDRESS.BUILDING_NAME = model.P_MW_ADDRESS.BUILDING_NAME;
                            p_MW_ADDRESS.FLOOR = model.P_MW_ADDRESS.FLOOR;
                            p_MW_ADDRESS.FLAT = model.P_MW_ADDRESS.FLAT;
                            p_MW_ADDRESS.DISTRICT = model.P_MW_ADDRESS.DISTRICT;
                            p_MW_ADDRESS.REGION = model.P_MW_ADDRESS.REGION;
                        }

                        P_MW_PERSON_CONTACT p_MW_PERSON_CONTACT = db.P_MW_PERSON_CONTACT.Where(m => m.UUID == model.P_MW_PERSON_CONTACT.UUID).FirstOrDefault();
                        if (p_MW_PERSON_CONTACT != null)
                        {
                            p_MW_PERSON_CONTACT.EMAIL = model.P_MW_PERSON_CONTACT.EMAIL;
                            p_MW_PERSON_CONTACT.CONTACT_NO = model.P_MW_PERSON_CONTACT.CONTACT_NO;
                            p_MW_PERSON_CONTACT.FAX_NO = model.P_MW_PERSON_CONTACT.FAX_NO;
                        }

                        P_MW_GENERAL_RECORD p_MW_GENERAL_RECORD = db.P_MW_GENERAL_RECORD.Where(m => m.UUID == model.P_MW_GENERAL_RECORD.UUID).FirstOrDefault();
                        if (p_MW_GENERAL_RECORD != null)
                        {
                            p_MW_GENERAL_RECORD.ADDRESS_AREA = model.P_MW_GENERAL_RECORD.ADDRESS_AREA;
                            p_MW_GENERAL_RECORD.ENGLISH_NAME = model.P_MW_GENERAL_RECORD.ENGLISH_NAME;
                            p_MW_GENERAL_RECORD.CHINESE_NAME = model.P_MW_GENERAL_RECORD.CHINESE_NAME;
                            p_MW_GENERAL_RECORD.CASE_TITLE = model.P_MW_GENERAL_RECORD.CASE_TITLE;
                            p_MW_GENERAL_RECORD.VENUE = model.P_MW_GENERAL_RECORD.VENUE;
                            p_MW_GENERAL_RECORD.CHANNEL = model.P_MW_GENERAL_RECORD.CHANNEL;
                            p_MW_GENERAL_RECORD.SECTION_UNIT_REF = model.P_MW_GENERAL_RECORD.SECTION_UNIT_REF;
                            p_MW_GENERAL_RECORD.RECEIVE_DATE = model.P_MW_GENERAL_RECORD.RECEIVE_DATE;
                            if (model.Language != null)
                            {
                                P_S_SYSTEM_VALUE language = db.P_S_SYSTEM_VALUE.Where(m => m.CODE == model.Language.CODE).FirstOrDefault();
                                p_MW_GENERAL_RECORD.LANGUAGE_ID = language.UUID;
                            }
                            // p_MW_GENERAL_RECORD.FORM_STATUS = isSubmit ? ProcessingConstant.GENERAL_RECORD_COMPLETED : ProcessingConstant.GENERAL_ENTRY_DRAFT;
                            p_MW_GENERAL_RECORD.FORM_STATUS = isSubmit ? ProcessingConstant.GENERAL_RECORD_DRAFT : ProcessingConstant.GENERAL_ENTRY_DRAFT;
                        }

                        // Andy Modify on 2019/10/24
                        if (isSubmit)
                        {
                            P_S_SYSTEM_VALUE svGeneralEntryCompleted = ProcessingSystemValueService.GetSystemValueByTypeAndCode(ProcessingConstant.DSN_STATUS, ProcessingConstant.GENERAL_COMPLETED);
                            if (svGeneralEntryCompleted != null)
                            {
                                // Update status of DSN
                                p_MW_DSN.SCANNED_STATUS_ID = svGeneralEntryCompleted.UUID;
                            }

                            if (String.IsNullOrEmpty(RefNo))
                            {
                                tran.Rollback();
                                return new ServiceResult()
                                {
                                    Result = ServiceResult.RESULT_FAILURE
                                    ,
                                    Message = { "Reference No not found" }
                                };
                            }
                            else
                            {
                                // process work flow
                                ProcessingWorkFlowManagementService.Instance.StartWorkFlowEnquiry(db, p_MW_GENERAL_RECORD, RefNo);
                            }
                        }
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return new ServiceResult()
                        {
                            Result = ServiceResult.RESULT_FAILURE
                            ,
                            Message = { ex.Message }
                        };
                    }

                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_SUCCESS
                    };
                }
            }

        }

        public Fn02MWUR_GEModel EnquiryForm(Fn02MWUR_GEModel model)
        {
            model.P_MW_GENERAL_RECORD = MWGeneralRecordService.MWGeneralRecordByUUID(model.P_MW_GENERAL_RECORD.UUID);
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                model.P_MW_REFERENCE_NO = db.P_MW_REFERENCE_NO.Where(m => m.UUID == model.P_MW_GENERAL_RECORD.REFERENCE_NUMBER).FirstOrDefault();
                model.P_MW_DSN = db.P_MW_DSN.Where(m => m.RECORD_ID == model.P_MW_REFERENCE_NO.REFERENCE_NO).FirstOrDefault();
                model.P_MW_PERSON_CONTACT = db.P_MW_PERSON_CONTACT.Where(m => m.UUID == model.P_MW_GENERAL_RECORD.CONTACT_ID).FirstOrDefault();
                if (model.P_MW_PERSON_CONTACT != null)
                {
                    model.P_MW_ADDRESS = db.P_MW_ADDRESS.Where(m => m.UUID == model.P_MW_PERSON_CONTACT.MW_ADDRESS_ID).FirstOrDefault();
                }
                model.P_MW_REFERENCE_NO = db.P_MW_REFERENCE_NO.Where(m => m.UUID == model.P_MW_GENERAL_RECORD.REFERENCE_NUMBER).FirstOrDefault();

                model.P_MW_ADDRESSES = new List<P_MW_ADDRESS>();
                List<P_MW_GENERAL_RECORD_ADDRESS_INFO> p_MW_GENERAL_RECORD_ADDRESS_INFOs = db.P_MW_GENERAL_RECORD_ADDRESS_INFO.Where(m => m.MW_GENERAL_RECORD_ID == model.P_MW_GENERAL_RECORD.UUID).ToList();
                foreach (var mwGeneralRecordAddressInfo in p_MW_GENERAL_RECORD_ADDRESS_INFOs)
                {
                    model.P_MW_ADDRESSES.Add(mwGeneralRecordAddressInfo.P_MW_ADDRESS);
                }

                model.Language = db.P_S_SYSTEM_VALUE.Where(m => m.UUID == model.P_MW_GENERAL_RECORD.LANGUAGE_ID).FirstOrDefault();

                P_WF_TASK task = ProcessingWorkFlowManagementService.Instance.GetCurrentTaskByRecordID(db, model.P_MW_GENERAL_RECORD.UUID);

                if(task != null)
                {
                    model.IsSPO = task.TASK_CODE.IndexOf(ProcessingConstant.SPO) >= 0;
                    model.IsReadOnly = model.IsSPO;
                }
            }
            return model;
        }

        public Fn02MWUR_GEModel GetCheklist(Fn02MWUR_GEModel model)
        {

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                model.P_MW_COMPLAINT_CHECKLIST = db.P_MW_COMPLAINT_CHECKLIST.Where(m => m.RECORD_ID == model.P_MW_GENERAL_RECORD.UUID).FirstOrDefault();
                return model;
            }


        }

        public ServiceResult SaveEnquiryForm(Fn02MWUR_GEModel model)
        {

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    P_MW_GENERAL_RECORD p_MW_GENERAL_RECORD;
                    try
                    {
                        P_MW_ADDRESS p_MW_ADDRESS = db.P_MW_ADDRESS.Where(m => m.UUID == model.P_MW_ADDRESS.UUID).FirstOrDefault();
                        if (p_MW_ADDRESS != null)
                        {
                            p_MW_ADDRESS.CHINESE_STREET_NAME = model.P_MW_ADDRESS.CHINESE_STREET_NAME;
                            p_MW_ADDRESS.STREET_NO = model.P_MW_ADDRESS.STREET_NO;
                            p_MW_ADDRESS.BUILDING_NAME = model.P_MW_ADDRESS.BUILDING_NAME;
                            p_MW_ADDRESS.FLOOR = model.P_MW_ADDRESS.FLOOR;
                            p_MW_ADDRESS.FLAT = model.P_MW_ADDRESS.FLAT;
                            p_MW_ADDRESS.DISTRICT = model.P_MW_ADDRESS.DISTRICT;
                            p_MW_ADDRESS.REGION = model.P_MW_ADDRESS.REGION;
                        }

                        P_MW_PERSON_CONTACT p_MW_PERSON_CONTACT = db.P_MW_PERSON_CONTACT.Where(m => m.UUID == model.P_MW_PERSON_CONTACT.UUID).FirstOrDefault();
                        if (p_MW_PERSON_CONTACT != null)
                        {
                            p_MW_PERSON_CONTACT.EMAIL = model.P_MW_PERSON_CONTACT.EMAIL;
                            p_MW_PERSON_CONTACT.CONTACT_NO = model.P_MW_PERSON_CONTACT.CONTACT_NO;
                            p_MW_PERSON_CONTACT.FAX_NO = model.P_MW_PERSON_CONTACT.FAX_NO;
                        }

                        p_MW_GENERAL_RECORD = db.P_MW_GENERAL_RECORD.Where(m => m.UUID == model.P_MW_GENERAL_RECORD.UUID).FirstOrDefault();
                        if (p_MW_GENERAL_RECORD != null)
                        {
                            p_MW_GENERAL_RECORD.ENGLISH_NAME = model.P_MW_GENERAL_RECORD.ENGLISH_NAME;
                            p_MW_GENERAL_RECORD.CHINESE_NAME = model.P_MW_GENERAL_RECORD.CHINESE_NAME;
                            p_MW_GENERAL_RECORD.CASE_TITLE = model.P_MW_GENERAL_RECORD.CASE_TITLE;
                            p_MW_GENERAL_RECORD.ADDRESS_AREA = model.P_MW_GENERAL_RECORD.ADDRESS_AREA;
                            p_MW_GENERAL_RECORD.CHANNEL = model.P_MW_GENERAL_RECORD.CHANNEL;
                            p_MW_GENERAL_RECORD.SECTION_UNIT_REF = model.P_MW_GENERAL_RECORD.SECTION_UNIT_REF;
                            p_MW_GENERAL_RECORD.RECEIVE_DATE = model.P_MW_GENERAL_RECORD.RECEIVE_DATE;
                            p_MW_GENERAL_RECORD.VENUE = model.P_MW_GENERAL_RECORD.VENUE;
                            if (model.Language != null)
                            {
                                P_S_SYSTEM_VALUE language = db.P_S_SYSTEM_VALUE.Where(m => m.CODE == model.Language.CODE).FirstOrDefault();
                                p_MW_GENERAL_RECORD.LANGUAGE_ID = language.UUID;
                            }

                            //p_MW_GENERAL_RECORD.FORM_STATUS = model.IsSubmit ? ProcessingConstant.GENERAL_RECORD_COMPLETED : ProcessingConstant.GENERAL_ENTRY_DRAFT;
                        }

                        if (model.P_MW_ADDRESSES != null)
                        {
                            foreach (var item in model.P_MW_ADDRESSES)
                            {
                                // save and update
                                if (string.IsNullOrWhiteSpace(item.UUID))
                                {
                                    db.P_MW_ADDRESS.Add(item);
                                    db.SaveChanges();
                                    P_MW_GENERAL_RECORD_ADDRESS_INFO p_MW_GENERAL_RECORD_ADDRESS_INFO = new P_MW_GENERAL_RECORD_ADDRESS_INFO();
                                    p_MW_GENERAL_RECORD_ADDRESS_INFO.MW_ADDRESS_ID = item.UUID;
                                    p_MW_GENERAL_RECORD_ADDRESS_INFO.MW_GENERAL_RECORD_ID = model.P_MW_GENERAL_RECORD.UUID;
                                    db.P_MW_GENERAL_RECORD_ADDRESS_INFO.Add(p_MW_GENERAL_RECORD_ADDRESS_INFO);
                                }
                                else
                                {
                                    P_MW_ADDRESS address = db.P_MW_ADDRESS.Where(m => m.UUID == item.UUID).FirstOrDefault();
                                    //address.STREET_NO = item.DISPLAY_STREET_NO;
                                    //address.BUILDING_NAME = item.DISPLAY_BUILDINGNAME;
                                    //address.FLOOR = item.DISPLAY_FLOOR;
                                    //address.FLAT = item.DISPLAY_FLAT;
                                    //address.DISTRICT = item.DISPLAY_DISTRICT;
                                    //address.REGION = item.DISPLAY_REGION;
                                    address.STREE_NAME = item.STREE_NAME;
                                    address.STREET_NO = item.STREET_NO;
                                    address.BUILDING_NAME = item.BUILDING_NAME;
                                    address.FLOOR = item.FLOOR;
                                    address.FLAT = item.FLAT;
                                    address.DISTRICT = item.DISTRICT;
                                    address.REGION = item.REGION;
                                }
                            }
                        }

                        //P_MW_REFERENCE_NO referNo =  db.P_MW_REFERENCE_NO.Where(o => o.UUID == p_MW_GENERAL_RECORD.REFERENCE_NUMBER).FirstOrDefault();

                        // Andy modify on 2019-10-24: Handling if submit the general submission
                        //if (model.IsSubmit)
                        //{
                        //    ProcessingWorkFlowManagementService.Instance.ToNext(db
                        //        , ProcessingWorkFlowManagementService.WF_TYPE_ENQ_COM
                        //        , p_MW_GENERAL_RECORD.UUID
                        //        , ProcessingWorkFlowManagementService.WF_ENQUIRY_TASK_ACKN_PO
                        //        , SessionUtil.LoginPost.UUID);
                        //}
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return new ServiceResult()
                        {
                            Result = ServiceResult.RESULT_FAILURE
                            ,
                            Message = { ex.Message }
                        };
                    }

                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_SUCCESS
                    };
                }
            }

        }

        public ServiceResult GeneralMWNumber()
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    P_S_MW_NUMBER newP_S_MW_NUMBER = new P_S_MW_NUMBER();
                    try
                    {
                        string mwNumber = (from m in db.P_S_MW_NUMBER where m.MW_NUMBER.StartsWith("D") select m.MW_NUMBER).Max();
                        int newMWNumber = Convert.ToInt32(mwNumber.Substring(1)) + 1;
                        newP_S_MW_NUMBER.MW_NUMBER = "D" + newMWNumber;
                        db.P_S_MW_NUMBER.Add(newP_S_MW_NUMBER);
                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return new ServiceResult()
                        {
                            Result = ServiceResult.RESULT_FAILURE
                            ,
                            Message = { ex.Message }
                        };
                    }

                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_SUCCESS
                        ,
                        Data = newP_S_MW_NUMBER
                    };
                }
            }
        }

        public ServiceResult SaveCheckListForm(Fn02MWUR_GEModel model, string status, string flowStatus)
        {

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        P_MW_GENERAL_RECORD mwGeneralRecord = db.P_MW_GENERAL_RECORD.Where(m => m.UUID == model.P_MW_GENERAL_RECORD.UUID).FirstOrDefault();
                        if (mwGeneralRecord == null)
                        {
                            return new ServiceResult()
                            {
                                Result = ServiceResult.RESULT_FAILURE
                            };
                            // Error Message
                        }

                        if (!model.IsSPO)
                        {
                            mwGeneralRecord.RECEIVE_DATE = model.P_MW_GENERAL_RECORD.RECEIVE_DATE;
                            mwGeneralRecord.CLOSE_DATE = model.P_MW_GENERAL_RECORD.CLOSE_DATE;
                            mwGeneralRecord.MODIFIED_DATE = DateTime.Now;
                            mwGeneralRecord.STATUS = ProcessingConstant.ENQUIRY_STATUS_INTERIM;
                            if (status == ProcessingConstant.COMPLAINT_CHECKLIST_PO_SUBMIT)
                            {
                                mwGeneralRecord.INTERIM_REPLY_DATE = DateTime.Now;
                            }

                            P_MW_COMPLAINT_CHECKLIST mwComplaintChecklist = db.P_MW_COMPLAINT_CHECKLIST.Where(m => m.RECORD_ID == mwGeneralRecord.UUID).FirstOrDefault();
                            if (mwComplaintChecklist == null)
                            {
                                mwComplaintChecklist = new P_MW_COMPLAINT_CHECKLIST();
                            }
                            mwComplaintChecklist.MODIFIED_DATE = DateTime.Now;
                            mwComplaintChecklist.STATUS = status;

                            if (status == ProcessingConstant.COMPLAINT_CHECKLIST_PO_SUBMIT)
                            {
                                if (mwComplaintChecklist.FLOW_STATUS == ProcessingConstant.FLOW_SITE_PO)
                                {
                                    flowStatus = ProcessingConstant.FLOW_FINAL_CONFIRM_SPO;
                                }
                            }
                            if (flowStatus != null)
                            {
                                mwComplaintChecklist.FLOW_STATUS = flowStatus;
                            }

                            mwComplaintChecklist.RECORD_ID = mwGeneralRecord.UUID;
                            mwComplaintChecklist.RE_ASSIGNMENT_REQUIRED = model.P_MW_COMPLAINT_CHECKLIST.RE_ASSIGNMENT_REQUIRED;
                            mwComplaintChecklist.REFERRAL_TO_REQUIRED = model.P_MW_COMPLAINT_CHECKLIST.REFERRAL_TO_REQUIRED;
                            mwComplaintChecklist.REFERRAL_TO_DUE_DATE = model.P_MW_COMPLAINT_CHECKLIST.REFERRAL_TO_DUE_DATE;
                            mwComplaintChecklist.STRUCTURAL_COMMENT_REQUIRED = model.P_MW_COMPLAINT_CHECKLIST.STRUCTURAL_COMMENT_REQUIRED;
                            mwComplaintChecklist.STRUCTURAL_DUE_DATE = model.P_MW_COMPLAINT_CHECKLIST.STRUCTURAL_DUE_DATE;
                            mwComplaintChecklist.INTERIM_REPLY_REQUIRED = model.P_MW_COMPLAINT_CHECKLIST.INTERIM_REPLY_REQUIRED;
                            mwComplaintChecklist.INTERIRM_REPLY_BY = model.P_MW_COMPLAINT_CHECKLIST.INTERIRM_REPLY_BY;

                            mwComplaintChecklist.INTERIRM_REPLY_COMPLETED = model.P_MW_COMPLAINT_CHECKLIST.INTERIRM_REPLY_COMPLETED;
                            mwComplaintChecklist.AUDIT_REQUIRED = model.P_MW_COMPLAINT_CHECKLIST.AUDIT_REQUIRED;
                            mwComplaintChecklist.SITE_INSPECTION_REQUIRED = model.P_MW_COMPLAINT_CHECKLIST.SITE_INSPECTION_REQUIRED;
                            mwComplaintChecklist.ASSIGN_OFFICER = model.P_MW_COMPLAINT_CHECKLIST.ASSIGN_OFFICER;
                            mwComplaintChecklist.REPLY_REQUIRED = model.P_MW_COMPLAINT_CHECKLIST.REPLY_REQUIRED;
                            mwComplaintChecklist.REPLY_BY = model.P_MW_COMPLAINT_CHECKLIST.REPLY_BY;
                            mwComplaintChecklist.SITE_INSPECTION_RECORD_EXIST = mwComplaintChecklist.SITE_INSPECTION_RECORD_EXIST;

                            mwComplaintChecklist.RECOMMENT_ON_ACTION_TAKEN = model.P_MW_COMPLAINT_CHECKLIST.RECOMMENT_ON_ACTION_TAKEN;
                            mwComplaintChecklist.REPLY_COMPLETED = model.P_MW_COMPLAINT_CHECKLIST.REPLY_COMPLETED;
                            if (string.IsNullOrEmpty(mwComplaintChecklist.UUID))
                            {
                                db.P_MW_COMPLAINT_CHECKLIST.Add(mwComplaintChecklist);
                            }
                            db.SaveChanges();
                            //save sectionList
                            //List<P_MW_DSN> sectionDsnList = new List<P_MW_DSN>();
                            P_S_SYSTEM_VALUE sectionStatus = GetSSystemValueByCode(ProcessingConstant.DSN_CHECKLIST_SECTION_NEW);
                            if (model.P_MW_COMPLAINT_CHECKLIST_SECTIONs != null)
                            {
                                foreach (var section in model.P_MW_COMPLAINT_CHECKLIST_SECTIONs)
                                {
                                    if (!string.IsNullOrWhiteSpace(section.DSN)
                                        || !string.IsNullOrWhiteSpace(section.OFFICER)
                                        || !string.IsNullOrWhiteSpace(section.RESULT)
                                        || !string.IsNullOrWhiteSpace(section.SECTION_UNIT_DIVISION))
                                    {
                                        P_MW_COMPLAINT_CHECKLIST_SECTION newSection;
                                        if (string.IsNullOrWhiteSpace(section.UUID))
                                        {
                                            newSection = new P_MW_COMPLAINT_CHECKLIST_SECTION();
                                        }
                                        else
                                        {
                                            newSection = db.P_MW_COMPLAINT_CHECKLIST_SECTION.Where(m => m.UUID == section.UUID).FirstOrDefault();
                                        }
                                        newSection.SECTION_UNIT_DIVISION = section.SECTION_UNIT_DIVISION;
                                        newSection.OFFICER = section.OFFICER;
                                        newSection.RESULT = section.RESULT;
                                        newSection.DSN = section.DSN;
                                        //newSection.MODIFIED_DATE = DateTime.Now;
                                        newSection.MW_COMPLAINT_CHECKLIST_ID = mwComplaintChecklist.UUID;
                                        if (string.IsNullOrWhiteSpace(section.UUID))
                                        {
                                            db.P_MW_COMPLAINT_CHECKLIST_SECTION.Add(newSection);
                                        }

                                        P_MW_DSN sectionDSN = db.P_MW_DSN.Where(m => m.DSN == section.DSN).FirstOrDefault();
                                        if (sectionDSN == null)
                                        {
                                            sectionDSN = new P_MW_DSN();
                                            sectionDSN.DSN = section.DSN;
                                            sectionDSN.RECORD_ID = mwGeneralRecord.P_MW_REFERENCE_NO.REFERENCE_NO;
                                            sectionDSN.P_S_SYSTEM_VALUE = sectionStatus;
                                            sectionDSN.MODIFIED_DATE = DateTime.Now;
                                            db.P_MW_DSN.Add(sectionDSN);
                                            //sectionDsnList.Add(sectionDSN);
                                        }
                                    }
                                }

                            }

                            //save commentList
                            if (model.P_MW_COMPLAINT_CHECKLIST_COMMENTs != null)
                            {
                                foreach (var comment in model.P_MW_COMPLAINT_CHECKLIST_COMMENTs)
                                {
                                    if (!string.IsNullOrWhiteSpace(comment.INTERNAL_COMMENT)
                                        || !string.IsNullOrWhiteSpace(comment.INTERNAL_COMMENT_DUE_DATE.ToString()))
                                    {
                                        P_MW_COMPLAINT_CHECKLIST_COMMENT newComment;
                                        if (string.IsNullOrWhiteSpace(comment.UUID))
                                        {
                                            newComment = new P_MW_COMPLAINT_CHECKLIST_COMMENT();
                                        }
                                        else
                                        {
                                            newComment = db.P_MW_COMPLAINT_CHECKLIST_COMMENT.Where(m => m.UUID == comment.UUID).FirstOrDefault();
                                        }
                                        newComment.INTERNAL_COMMENT = comment.INTERNAL_COMMENT;
                                        newComment.INTERNAL_COMMENT_DUE_DATE = comment.INTERNAL_COMMENT_DUE_DATE;
                                        newComment.MW_COMPLAINT_CHECKLIST_ID = mwComplaintChecklist.UUID;
                                        newComment.MODIFIED_DATE = DateTime.Now;

                                        if (string.IsNullOrWhiteSpace(comment.UUID))
                                        {
                                            db.P_MW_COMPLAINT_CHECKLIST_COMMENT.Add(newComment);
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }


                            //save photoList
                            P_S_SYSTEM_VALUE photoStatus = GetSSystemValueByCode(ProcessingConstant.DSN_CHECKLIST_PHOTO_NEW);


                            //set MwSubmissionProcessingInfo for submission 
                            P_MW_SUBMISSION_PROCESSING_INFO processingInfo = db.P_MW_SUBMISSION_PROCESSING_INFO.Where(m => m.P_MW_REFERENCE_NO.UUID == mwGeneralRecord.P_MW_REFERENCE_NO.UUID && m.STAGE == flowStatus).FirstOrDefault();
                            if (processingInfo == null)
                            {
                                processingInfo = new P_MW_SUBMISSION_PROCESSING_INFO();
                                processingInfo.MWNO = mwGeneralRecord.P_MW_REFERENCE_NO.UUID;
                                //processingInfo.HANDLE_OFFICER=loginName
                                processingInfo.MWRECORDID = null;
                                processingInfo.STAGE = flowStatus;
                                db.P_MW_SUBMISSION_PROCESSING_INFO.Add(processingInfo);

                                P_MW_RECORD mwRecord = db.P_MW_RECORD.Where(m => m.REFERENCE_NUMBER == mwGeneralRecord.REFERENCE_NUMBER).FirstOrDefault();
                                if (mwRecord != null)
                                {
                                    mwRecord.STATUS_CODE = flowStatus;
                                }
                            }
                        }

                        

                        if (model.IsSubmit)
                        {
                            string postId = SessionUtil.LoginPost.UUID;
                            string refNo = mwGeneralRecord.P_MW_REFERENCE_NO.REFERENCE_NO;
                            P_WF_TASKTOUSER working = db.P_WF_TASKTOUSER.Where(o => o.POST_CODE == postId).Where(o => o.MW_NUMBER == refNo).FirstOrDefault();
                            if (working != null)//status == ProcessingConstant.COMPLAINT_CHECKLIST_PO_SUBMIT)
                            {
                                //this.setSubmittedOkey(true);
                                mwGeneralRecord.FORM_STATUS = ProcessingConstant.GENERAL_RECORD_COMPLETED;
                                ProcessingWorkFlowManagementService.Instance.ToNext(db
                                    , ProcessingWorkFlowManagementService.WF_TYPE_ENQ_COM
                                    , mwGeneralRecord.UUID
                                    , working.ACTIVITY//ProcessingWorkFlowManagementService.WF_ENQUIRY_TASK_ACKN_PO
                                    , SessionUtil.LoginPost.UUID);

                            }
                        }


                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return new ServiceResult()
                        {
                            Result = ServiceResult.RESULT_FAILURE
                            ,
                            Message = { ex.Message }
                        };
                    }

                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_SUCCESS
                    };
                }
            }

        }

        public ServiceResult SaveComplaintCheckListForm(Fn02MWUR_GEModel model, string status, string flowStatus)
        {

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        P_MW_GENERAL_RECORD mwGeneralRecord = db.P_MW_GENERAL_RECORD.Where(m => m.UUID == model.P_MW_GENERAL_RECORD.UUID).FirstOrDefault();
                        if (mwGeneralRecord == null)
                        {
                            return new ServiceResult()
                            {
                                Result = ServiceResult.RESULT_FAILURE
                            };
                            // Error Message
                        }

                        if (!model.IsSPO)
                        {
                            mwGeneralRecord.RECEIVE_DATE = model.P_MW_GENERAL_RECORD.RECEIVE_DATE;
                            mwGeneralRecord.CLOSE_DATE = model.P_MW_GENERAL_RECORD.CLOSE_DATE;
                            mwGeneralRecord.MODIFIED_DATE = DateTime.Now;
                            mwGeneralRecord.STATUS = ProcessingConstant.ENQUIRY_STATUS_INTERIM;
                            if (status == ProcessingConstant.COMPLAINT_CHECKLIST_PO_SUBMIT)
                            {
                                mwGeneralRecord.INTERIM_REPLY_DATE = DateTime.Now;
                            }

                            P_MW_COMPLAINT_CHECKLIST mwComplaintChecklist = db.P_MW_COMPLAINT_CHECKLIST.Where(m => m.RECORD_ID == mwGeneralRecord.UUID).FirstOrDefault();
                            if (mwComplaintChecklist == null)
                            {
                                mwComplaintChecklist = new P_MW_COMPLAINT_CHECKLIST();
                            }
                            mwComplaintChecklist.MODIFIED_DATE = DateTime.Now;
                            mwComplaintChecklist.STATUS = status;

                            if (status == ProcessingConstant.COMPLAINT_CHECKLIST_PO_SUBMIT)
                            {
                                if (mwComplaintChecklist.FLOW_STATUS == ProcessingConstant.FLOW_SITE_PO)
                                {
                                    flowStatus = ProcessingConstant.FLOW_FINAL_CONFIRM_SPO;
                                }
                            }
                            if (flowStatus != null)
                            {
                                mwComplaintChecklist.FLOW_STATUS = flowStatus;
                            }

                            mwComplaintChecklist.RECORD_ID = mwGeneralRecord.UUID;
                            mwComplaintChecklist.RE_ASSIGNMENT_REQUIRED = model.P_MW_COMPLAINT_CHECKLIST.RE_ASSIGNMENT_REQUIRED;
                            mwComplaintChecklist.REFERRAL_TO_REQUIRED = model.P_MW_COMPLAINT_CHECKLIST.REFERRAL_TO_REQUIRED;
                            mwComplaintChecklist.REFERRAL_TO_DUE_DATE = model.P_MW_COMPLAINT_CHECKLIST.REFERRAL_TO_DUE_DATE;
                            mwComplaintChecklist.STRUCTURAL_COMMENT_REQUIRED = model.P_MW_COMPLAINT_CHECKLIST.STRUCTURAL_COMMENT_REQUIRED;
                            //mwComplaintChecklist.STRUCTURAL_DUE_DATE = model.P_MW_COMPLAINT_CHECKLIST.STRUCTURAL_DUE_DATE;
                            mwComplaintChecklist.INTERIM_REPLY_REQUIRED = model.P_MW_COMPLAINT_CHECKLIST.INTERIM_REPLY_REQUIRED;
                            mwComplaintChecklist.INTERIRM_REPLY_BY = model.P_MW_COMPLAINT_CHECKLIST.INTERIRM_REPLY_BY;


                            mwComplaintChecklist.INTERIRM_REPLY_COMPLETED = model.P_MW_COMPLAINT_CHECKLIST.INTERIRM_REPLY_COMPLETED;
                            mwComplaintChecklist.AUDIT_REQUIRED = model.P_MW_COMPLAINT_CHECKLIST.AUDIT_REQUIRED;
                            mwComplaintChecklist.SITE_INSPECTION_RECORD_EXIST = model.P_MW_COMPLAINT_CHECKLIST.SITE_INSPECTION_RECORD_EXIST;
                            mwComplaintChecklist.SITE_INSPECTION_RECORD = model.P_MW_COMPLAINT_CHECKLIST.SITE_INSPECTION_RECORD;
                            //mwComplaintChecklist.ASSIGN_OFFICER = model.P_MW_COMPLAINT_CHECKLIST.ASSIGN_OFFICER;
                            mwComplaintChecklist.REPLY_REQUIRED = model.P_MW_COMPLAINT_CHECKLIST.REPLY_REQUIRED;
                            mwComplaintChecklist.REPLY_BY = model.P_MW_COMPLAINT_CHECKLIST.REPLY_BY;

                            mwComplaintChecklist.RECOMMENT_ON_ACTION_TAKEN = model.P_MW_COMPLAINT_CHECKLIST.RECOMMENT_ON_ACTION_TAKEN;
                            mwComplaintChecklist.REPLY_COMPLETED = model.P_MW_COMPLAINT_CHECKLIST.REPLY_COMPLETED;
                            if (string.IsNullOrEmpty(mwComplaintChecklist.UUID))
                            {
                                db.P_MW_COMPLAINT_CHECKLIST.Add(mwComplaintChecklist);
                            }
                            //db.SaveChanges();
                            ////save sectionList
                            ////List<P_MW_DSN> sectionDsnList = new List<P_MW_DSN>();
                            //P_S_SYSTEM_VALUE sectionStatus = GetSSystemValueByCode(ProcessingConstant.DSN_CHECKLIST_SECTION_NEW);
                            //if (model.P_MW_COMPLAINT_CHECKLIST_SECTIONs != null)
                            //{
                            //    foreach (var section in model.P_MW_COMPLAINT_CHECKLIST_SECTIONs)
                            //    {
                            //        if (!string.IsNullOrWhiteSpace(section.DSN)
                            //            || !string.IsNullOrWhiteSpace(section.OFFICER)
                            //            || !string.IsNullOrWhiteSpace(section.RESULT)
                            //            || !string.IsNullOrWhiteSpace(section.SECTION_UNIT_DIVISION))
                            //        {
                            //            P_MW_COMPLAINT_CHECKLIST_SECTION newSection;
                            //            if (string.IsNullOrWhiteSpace(section.UUID))
                            //            {
                            //                newSection = new P_MW_COMPLAINT_CHECKLIST_SECTION();
                            //            }
                            //            else
                            //            {
                            //                newSection = db.P_MW_COMPLAINT_CHECKLIST_SECTION.Where(m => m.UUID == section.UUID).FirstOrDefault();
                            //            }
                            //            newSection.SECTION_UNIT_DIVISION = section.SECTION_UNIT_DIVISION;
                            //            newSection.OFFICER = section.OFFICER;
                            //            newSection.RESULT = section.RESULT;
                            //            newSection.DSN = section.DSN;
                            //            //newSection.MODIFIED_DATE = DateTime.Now;
                            //            newSection.MW_COMPLAINT_CHECKLIST_ID = mwComplaintChecklist.UUID;
                            //            if (string.IsNullOrWhiteSpace(section.UUID))
                            //            {
                            //                db.P_MW_COMPLAINT_CHECKLIST_SECTION.Add(newSection);
                            //            }

                            //            P_MW_DSN sectionDSN = db.P_MW_DSN.Where(m => m.DSN == section.DSN).FirstOrDefault();
                            //            if (sectionDSN == null)
                            //            {
                            //                sectionDSN = new P_MW_DSN();
                            //                sectionDSN.DSN = section.DSN;
                            //                sectionDSN.RECORD_ID = mwGeneralRecord.P_MW_REFERENCE_NO.REFERENCE_NO;
                            //                sectionDSN.P_S_SYSTEM_VALUE = sectionStatus;
                            //                sectionDSN.MODIFIED_DATE = DateTime.Now;
                            //                db.P_MW_DSN.Add(sectionDSN);
                            //                //sectionDsnList.Add(sectionDSN);
                            //            }
                            //        }
                            //    }

                            //}

                            ////save commentList
                            //if (model.P_MW_COMPLAINT_CHECKLIST_COMMENTs != null)
                            //{
                            //    foreach (var comment in model.P_MW_COMPLAINT_CHECKLIST_COMMENTs)
                            //    {
                            //        if (!string.IsNullOrWhiteSpace(comment.INTERNAL_COMMENT)
                            //            || !string.IsNullOrWhiteSpace(comment.INTERNAL_COMMENT_DUE_DATE.ToString()))
                            //        {
                            //            P_MW_COMPLAINT_CHECKLIST_COMMENT newComment;
                            //            if (string.IsNullOrWhiteSpace(comment.UUID))
                            //            {
                            //                newComment = new P_MW_COMPLAINT_CHECKLIST_COMMENT();
                            //            }
                            //            else
                            //            {
                            //                newComment = db.P_MW_COMPLAINT_CHECKLIST_COMMENT.Where(m => m.UUID == comment.UUID).FirstOrDefault();
                            //            }
                            //            newComment.INTERNAL_COMMENT = comment.INTERNAL_COMMENT;
                            //            newComment.INTERNAL_COMMENT_DUE_DATE = comment.INTERNAL_COMMENT_DUE_DATE;
                            //            newComment.MW_COMPLAINT_CHECKLIST_ID = mwComplaintChecklist.UUID;
                            //            newComment.MODIFIED_DATE = DateTime.Now;

                            //            if (string.IsNullOrWhiteSpace(comment.UUID))
                            //            {
                            //                db.P_MW_COMPLAINT_CHECKLIST_COMMENT.Add(newComment);
                            //                db.SaveChanges();
                            //            }
                            //        }
                            //    }
                            //}


                            ////save photoList
                            //P_S_SYSTEM_VALUE photoStatus = GetSSystemValueByCode(ProcessingConstant.DSN_CHECKLIST_PHOTO_NEW);


                            ////set MwSubmissionProcessingInfo for submission 
                            //P_MW_SUBMISSION_PROCESSING_INFO processingInfo = db.P_MW_SUBMISSION_PROCESSING_INFO.Where(m => m.P_MW_REFERENCE_NO.UUID == mwGeneralRecord.P_MW_REFERENCE_NO.UUID && m.STAGE == flowStatus).FirstOrDefault();
                            //if (processingInfo == null)
                            //{
                            //    processingInfo = new P_MW_SUBMISSION_PROCESSING_INFO();
                            //    processingInfo.MWNO = mwGeneralRecord.P_MW_REFERENCE_NO.UUID;
                            //    //processingInfo.HANDLE_OFFICER=loginName
                            //    processingInfo.MWRECORDID = null;
                            //    processingInfo.STAGE = flowStatus;
                            //    db.P_MW_SUBMISSION_PROCESSING_INFO.Add(processingInfo);

                            //    P_MW_RECORD mwRecord = db.P_MW_RECORD.Where(m => m.REFERENCE_NUMBER == mwGeneralRecord.REFERENCE_NUMBER).FirstOrDefault();
                            //    if (mwRecord != null)
                            //    {
                            //        mwRecord.STATUS_CODE = flowStatus;
                            //    }
                            //}
                        }

                        if (model.IsSubmit)
                        {
                            string postId = SessionUtil.LoginPost.UUID;
                            string refNo = mwGeneralRecord.P_MW_REFERENCE_NO.REFERENCE_NO;
                            P_WF_TASKTOUSER working = db.P_WF_TASKTOUSER.Where(o => o.POST_CODE == postId).Where(o => o.MW_NUMBER == refNo).FirstOrDefault();
                            if (working != null)//status == ProcessingConstant.COMPLAINT_CHECKLIST_PO_SUBMIT)
                            {
                                //this.setSubmittedOkey(true);
                                mwGeneralRecord.FORM_STATUS = ProcessingConstant.GENERAL_RECORD_COMPLETED;
                                ProcessingWorkFlowManagementService.Instance.ToNext(db
                                    , ProcessingWorkFlowManagementService.WF_TYPE_ENQ_COM
                                    , mwGeneralRecord.UUID
                                    , working.ACTIVITY//ProcessingWorkFlowManagementService.WF_ENQUIRY_TASK_ACKN_PO
                                    , SessionUtil.LoginPost.UUID);

                            }
                        }
                        


                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return new ServiceResult()
                        {
                            Result = ServiceResult.RESULT_FAILURE
                            ,
                            Message = { ex.Message }
                        };
                    }

                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_SUCCESS
                    };
                }
            }

        }

        public ServiceResult SaveOrEditComment(Fn02MWUR_GEModel model)
        {
            P_MW_GENERAL_RECORD generalRecord = MWGeneralRecordService.MWGeneralRecordByUUID(model.P_MW_GENERAL_RECORD.UUID);
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(model.P_MW_COMMENT.UUID))
                        {
                            model.P_MW_COMMENT.RECORD_ID = generalRecord.UUID;
                            model.P_MW_COMMENT.RECORD_TYPE = ProcessingConstant.MW_GENERAL_RECORD;
                            db.P_MW_COMMENT.Add(model.P_MW_COMMENT);
                        }
                        else
                        {
                            P_MW_COMMENT comment = db.P_MW_COMMENT.Where(m => m.UUID == model.P_MW_COMMENT.UUID).FirstOrDefault();
                            comment.COMMENT_AREA = model.P_MW_COMMENT.COMMENT_AREA;
                        }

                        db.SaveChanges();
                        tran.Commit();
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return new ServiceResult()
                        {
                            Result = ServiceResult.RESULT_FAILURE
                            ,
                            Message = { ex.Message }
                        };
                    }

                    return new ServiceResult()
                    {
                        Result = ServiceResult.RESULT_SUCCESS
                    };
                }
            }

        }
    }
}