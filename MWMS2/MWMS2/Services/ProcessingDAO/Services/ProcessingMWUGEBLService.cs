using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.DAO;
using MWMS2.Utility;

namespace MWMS2.Services.ProcessingDAO.Services
{
    public class ProcessingMWUGEBLService
    {
        private ProcessingMWUGEDAOService _DA;
        protected ProcessingMWUGEDAOService DA
        {
            get
            {
                return _DA ?? (_DA = new ProcessingMWUGEDAOService());
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

        private string Search_where(Fn02MWUR_GEModel model)
        {
            StringBuilder where_q = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(model.DSN))
            {
                where_q.Append(" and DSN=:DSN ");
                model.QueryParameters.Add("DSN", model.DSN.Trim());
            }
            if (!string.IsNullOrWhiteSpace(model.RefNo))
            {
                where_q.Append(" and record_id=:RecordId  ");
                model.QueryParameters.Add("RecordId", model.RefNo.Trim());
            }
            if (!string.IsNullOrWhiteSpace(model.ReceiveFormDate) && !string.IsNullOrWhiteSpace(model.ReceiveToDate))
            {
                where_q.Append("  AND To_Date(To_Char(MWU_RECEIVED_DATE,'dd/MM/yyyy'),'dd/MM/yyyy') between To_Date( :ReceivedDateFrom ,'dd/MM/yyyy') and To_Date( :ReceivedDateTo ,'dd/MM/yyyy') ");
                model.QueryParameters.Add("ReceivedDateFrom", model.ReceiveFormDate.Trim());
                model.QueryParameters.Add("ReceivedDateTo", model.ReceiveToDate.Trim());
            }

            // Format search criteria: Status
            if (!string.IsNullOrWhiteSpace(model.Status))
            {
                if (ProcessingConstant.DSN_SCANNED.Equals(model.Status))
                {
                    where_q.Append("  and sv.code=:Status ");
                    model.QueryParameters.Add("Status", ProcessingConstant.DSN_SCANNED);
                } else if (ProcessingConstant.DSN_CONFIRMED.Equals(model.Status))
                {
                    where_q.Append("  and sv.code=:Status ");
                    model.QueryParameters.Add("Status", ProcessingConstant.DSN_CONFIRMED);
                }
                else if ((ProcessingConstant.DSN_GENERAL_ENTRY + "," +
                    ProcessingConstant.DSN_WILL_SCAN).Equals(model.Status))
                {
                    List<String> statusList = new List<string>();
                    statusList.Add(ProcessingConstant.DSN_GENERAL_ENTRY);
                    statusList.Add(ProcessingConstant.DSN_GENERAL_ENTRY_WILL_SCAN);
                    where_q.Append("  and sv.code in (:Status) ");
                    model.QueryParameters.Add("Status", statusList);
                }
            }
            else
            {
                List<String> statusList = new List<string>();
                statusList.Add(ProcessingConstant.DSN_SCANNED);
                statusList.Add(ProcessingConstant.DSN_CONFIRMED);
                statusList.Add(ProcessingConstant.DSN_GENERAL_ENTRY);
                statusList.Add(ProcessingConstant.DSN_GENERAL_ENTRY_WILL_SCAN);
                where_q.Append("  and sv.code in (:Status) ");
                model.QueryParameters.Add("Status", statusList);
            }
            return where_q.ToString();
        }
        public Fn02MWUR_GEModel Search(Fn02MWUR_GEModel model)
        {
            model.QueryWhere = Search_where(model);
            DA.Search(model);
            return model;
        }

        public Fn02MWUR_GEModel Enquiry(string dsn)
        {
            Fn02MWUR_GEModel model = new Fn02MWUR_GEModel();
            model = DA.Enquiry(dsn);
            model = DA.GetMWGeneralRecord(model);
            return model;
        }

        private string SearchScanned_where(Fn02MWUR_GEModel model)
        {
            StringBuilder where_q = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(model.Enquiry.UUID))
            {
                where_q.Append(" AND DSN_ID=:DSNID ");
                model.QueryParameters.Add("DSNID", model.Enquiry.UUID.Trim());
            }
            return where_q.ToString();
        }
        public Fn02MWUR_GEModel GetScannedDoc(Fn02MWUR_GEModel model)
        {
            model.QueryWhere = SearchScanned_where(model);
            DA.GetScannedDoc(model);
            return model;
        }

        public ServiceResult Submit1(Fn02MWUR_GEModel model)
        {
            if (DA.GetMWGeneralRecord(model).P_MW_GENERAL_RECORD != null)
            {
                return new ServiceResult()
                {
                    Result = ServiceResult.RESULT_SUCCESS
                };
            }

            return DA.Submit1(model);
        }

        public ServiceResult CheckedBeforeSubmit2(Fn02MWUR_GEModel model)
        {
            model = DA.GetMWGeneralRecord(model);
            if (model.P_MW_GENERAL_RECORD != null)
            {
                return new ServiceResult()
                {
                    Result = ServiceResult.RESULT_SUCCESS
                };
            }
            return new ServiceResult()
            {
                Result = ServiceResult.RESULT_FAILURE
            };
        }

        public Fn02MWUR_GEModel Submit2(Fn02MWUR_GEModel model)
        {
            return DA.GetMWGeneralRecord(model);
        }

        public ServiceResult SubmitEntry(Fn02MWUR_GEModel model)
        {
            return DA.SaveEntry(model, true);
        }

        public ServiceResult SaveEntry(Fn02MWUR_GEModel model)
        {
            return DA.SaveEntry(model, false);
        }

        public Fn02MWUR_GEModel EnquiryForm(string id)
        {
            Fn02MWUR_GEModel model = new Fn02MWUR_GEModel();
            model.P_MW_GENERAL_RECORD = new P_MW_GENERAL_RECORD();
            model.P_MW_GENERAL_RECORD.UUID = id;
            DA.EnquiryForm(model);
            
            return model;
        }
        public Fn02MWUR_GEModel GetCheklist(Fn02MWUR_GEModel model)
        {
            return DA.GetCheklist(model);
        }
        public ServiceResult SaveEnquiryForm(Fn02MWUR_GEModel model)
        {
                return DA.SaveEnquiryForm(model);
        }

        public ServiceResult SaveCheckListForm(Fn02MWUR_GEModel model)
        {
            return DA.SaveCheckListForm(model, ProcessingConstant.COMPLAINT_CHECKLIST_DRAFT, null);
        }
        public ServiceResult SubmitCheckListForm(Fn02MWUR_GEModel model)
        {
            return DA.SaveCheckListForm(model, ProcessingConstant.COMPLAINT_CHECKLIST_PO_SUBMIT, null);
        }
        public ServiceResult SaveComplaintCheckListForm(Fn02MWUR_GEModel model)
        {
            return DA.SaveComplaintCheckListForm(model, ProcessingConstant.COMPLAINT_CHECKLIST_DRAFT, null);
        }

        public ServiceResult SubmitComplaintCheckListForm(Fn02MWUR_GEModel model)
        {
            return DA.SaveComplaintCheckListForm(model, ProcessingConstant.COMPLAINT_CHECKLIST_PO_SUBMIT, null);

            //ServiceResult serviceResult = new ServiceResult();

            //using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            //{
            //    using(DbContextTransaction tran = db.Database.BeginTransaction())
            //    {
            //        try
            //        {
            //            P_MW_GENERAL_RECORD mwGeneralRecord = db.P_MW_GENERAL_RECORD.Where(m => m.UUID == model.P_MW_GENERAL_RECORD.UUID).FirstOrDefault();

            //            if (!model.IsSPO)
            //            {

            //            }

            //            if (model.IsSubmit)
            //            {
            //                //work flow
            //                string postId = SessionUtil.LoginPost.UUID;
            //                string refNo = mwGeneralRecord.P_MW_REFERENCE_NO.REFERENCE_NO;
            //                P_WF_TASKTOUSER working = db.P_WF_TASKTOUSER.Where(o => o.POST_CODE == postId).Where(o => o.MW_NUMBER == refNo).FirstOrDefault();
            //                if (working != null)//status == ProcessingConstant.COMPLAINT_CHECKLIST_PO_SUBMIT)
            //                {
            //                    //this.setSubmittedOkey(true);
            //                    mwGeneralRecord.FORM_STATUS = ProcessingConstant.GENERAL_RECORD_COMPLETED;
            //                    ProcessingWorkFlowManagementService.Instance.ToNext(db
            //                        , ProcessingWorkFlowManagementService.WF_TYPE_ENQ_COM
            //                        , mwGeneralRecord.UUID
            //                        , working.ACTIVITY//ProcessingWorkFlowManagementService.WF_ENQUIRY_TASK_ACKN_PO
            //                        , SessionUtil.LoginPost.UUID);

            //                }
            //            }
                        

            //            db.SaveChanges();
            //            tran.Commit();

            //            serviceResult.Result = ServiceResult.RESULT_SUCCESS;
            //        }
            //        catch(Exception e)
            //        {
            //            tran.Rollback();
            //            AuditLogService.logDebug(e);
            //            serviceResult.Result = ServiceResult.RESULT_FAILURE;
            //            serviceResult.Message = new List<string>() { e.Message };
            //        }

            //    }
            //}
            //return serviceResult;
             
        }

        public Fn02MWUR_GEModel ViewAndAddComment(string id)
        {
            Fn02MWUR_GEModel model = new Fn02MWUR_GEModel();
            model.P_MW_COMMENT = new P_MW_COMMENT();
            model.P_MW_GENERAL_RECORD = MWGeneralRecordService.MWGeneralRecordByUUID(id);
            model.P_MW_COMMENTs = DA_MwComment.getMwCommentsByRecordIdAndRecordType(id, ProcessingConstant.MW_GENERAL_RECORD);
            return model;
            //return DA.ViewAndAddComment(model);
        }

        public ServiceResult EditComment(string id)
        {
            P_MW_COMMENT comment = DA_MwComment.getMwCommentsByUUID(id);
            return new ServiceResult()
            {
                Result = ServiceResult.RESULT_SUCCESS
                ,
                Data = comment
            };
        }

        public ServiceResult SaveOrEditComment(Fn02MWUR_GEModel model)
        {
            return DA.SaveOrEditComment(model);
        }

        public ServiceResult GeneralMWNumber()
        {
            return DA.GeneralMWNumber();
        }
    }

}