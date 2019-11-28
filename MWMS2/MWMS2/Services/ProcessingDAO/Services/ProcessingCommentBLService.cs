using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class ProcessingCommentBLService
    {
        private ProcessingCommentDAOService DaoService;
        protected ProcessingCommentDAOService DA
        {
            get { return DaoService ?? (DaoService = new ProcessingCommentDAOService()); }
        }

        private P_MW_RECORD_DAOService _RecordDaoService;
        protected P_MW_RECORD_DAOService RecordDaoService
        {
            get { return _RecordDaoService ?? (_RecordDaoService = new P_MW_RECORD_DAOService()); }
        }

        private MWGeneralRecordService _generalRecordDaoService;
        protected MWGeneralRecordService generalRecordDaoService
        {
            get { return _generalRecordDaoService ?? (_generalRecordDaoService = new MWGeneralRecordService()); }
        }

        public void GetCommentModel(CommentModel model)
        {
            model.P_MW_COMMENTs = DA.GetP_MW_COMMENTs(model.RECORD_ID);
            if (model.P_MW_COMMENTs == null)
            {
                model.P_MW_COMMENTs = new List<Entity.P_MW_COMMENT>();
            }
        }

        public ServiceResult AddP_MW_COMMENT(P_MW_COMMENT model)
        {
            ServiceResult serviceResult = new ServiceResult();

            try
            {
                serviceResult.Result = DA.AddP_MW_COMMENT(model) > 0 ? ServiceResult.RESULT_SUCCESS : ServiceResult.RESULT_FAILURE;
            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.Message.Add(e.Message);
            }

            return serviceResult;
        }

        public ServiceResult Update(P_MW_COMMENT model)
        {
            ServiceResult serviceResult = new ServiceResult();

            try
            {
                serviceResult.Result = DA.UpdateP_MW_COMMENT(model) > 0 ? ServiceResult.RESULT_SUCCESS : ServiceResult.RESULT_FAILURE;
            }
            catch (Exception e)
            {
                serviceResult.Result = ServiceResult.RESULT_FAILURE;
                serviceResult.Message.Add(e.Message);
            }

            return serviceResult;
        }

        public ServiceResult RollbackAndComment(CommentModel model)
        {
            ServiceResult serviceResult = new ServiceResult();
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                using (DbContextTransaction tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        P_MW_COMMENT mwComent = new P_MW_COMMENT()
                        {
                            RECORD_ID = model.RECORD_ID,
                            COMMENT_AREA = model.COMMENT_AREA
                        };

                        DA.AddP_MW_COMMENT(mwComent, db);

                        if (model.SubmissionType == "Verification" || model.SubmissionType == "Acknowledgement")
                        {
                            rollbackDataEntry(db, model);
                        }
                        else if (model.SubmissionType == "Enquiry" || model.SubmissionType == "Complaint")
                        {
                            rollbackGeneralEntry(db, model);
                        }



                        db.SaveChanges();
                        tran.Commit();

                        serviceResult.Result = ServiceResult.RESULT_SUCCESS;
                    }
                    catch (Exception e)
                    {
                        tran.Rollback();

                        serviceResult.Result = ServiceResult.RESULT_FAILURE;
                        serviceResult.Message.Add(e.Message);
                    }
                }
            }


            return serviceResult;
        }

        private void rollbackDataEntry(EntitiesMWProcessing db, CommentModel model)
        {

            P_MW_RECORD record = RecordDaoService.GetP_MW_RECORDByUuid(model.RECORD_ID, db);

            P_MW_VERIFICATION ver = db.P_MW_VERIFICATION.Where(w => w.UUID == model.V_UUID).FirstOrDefault();
            if (ver == null) throw new Exception("Verification not exists.");

            ver.STATUS_CODE = model.IsSPO ? ProcessingConstant.MW_ACKN_STATUS_OPEN : ProcessingConstant.MW_VERT_STATUS_ROLLBACK;

            string taskCode = "";

            if (model.IsSPO)
            {
                taskCode = ProcessingConstant.HANDLING_UNIT_SMM.Equals(ver.HANDLING_UNIT) ? ProcessingWorkFlowManagementService.WF_SUBMISSION_TASK_ACKN_SPO_SMM : ProcessingWorkFlowManagementService.WF_SUBMISSION_TASK_ACKN_SPO;
            }
            else
            {
                taskCode = ProcessingConstant.HANDLING_UNIT_SMM.Equals(ver.HANDLING_UNIT) ? ProcessingWorkFlowManagementService.WF_SUBMISSION_TASK_ACKN_PO_SMM : ProcessingWorkFlowManagementService.WF_SUBMISSION_TASK_ACKN_PO;
            }

            ProcessingWorkFlowManagementService.Instance.ToBack(db
                , ProcessingWorkFlowManagementService.WF_TYPE_SUBMISSION
                , record.MW_DSN
                , taskCode
                , SessionUtil.LoginPost.UUID);

            db.SaveChanges();
        }

        private void rollbackGeneralEntry(EntitiesMWProcessing db, CommentModel model)
        {
            P_MW_GENERAL_RECORD record = generalRecordDaoService.MWGeneralRecordByUUID(db, model.RECORD_ID);

            string taskCode = "";

            if (model.SubmissionType == "Enquiry")
            {
                taskCode = ProcessingWorkFlowManagementService.WF_ENQUIRY_TASK_ACKN_SPO;
            }
            else if (model.SubmissionType == "Complaint")
            {
                taskCode = ProcessingWorkFlowManagementService.WF_COMPLAINT_TASK_ACKN_SPO;
            }

            ProcessingWorkFlowManagementService.Instance.ToBack(db
                , ProcessingWorkFlowManagementService.WF_TYPE_ENQ_COM
                , record.UUID
                , taskCode
                , SessionUtil.LoginPost.UUID);

            db.SaveChanges();
        }

    }
}