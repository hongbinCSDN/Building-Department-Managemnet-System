using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.DAO;

namespace MWMS2.Services.ProcessingDAO.Services
{
    public class ProcessingTSKSTSBLService
    {
        private ProcessingTSKSTSDAOService _DA;
        protected ProcessingTSKSTSDAOService DA
        {
            get
            {
                return _DA ?? (_DA = new ProcessingTSKSTSDAOService());
            }
        }

        private ProcessingSystemValueDAOService _SV_DA;
        protected ProcessingSystemValueDAOService SV_DA
        {
            get
            {
                return _SV_DA ?? (_SV_DA = new ProcessingSystemValueDAOService());
            }
        }

        private P_MW_DSN_DAOService _MWDSNDA;
        protected P_MW_DSN_DAOService MWDSNDA
        {
            get
            {
                return _MWDSNDA ?? (_MWDSNDA = new P_MW_DSN_DAOService());
            }
        }

        public Fn03TSK_STSModel SearchComplaintList(Fn03TSK_STSModel model)
        {
            model = DA.SearchComplaintList(model);
            for (int i = 0; i < model.Data.Count; i++)
            {
                AddColumnDSNOrICCNo(model, i);
                AddColumnFinalAndInterimReplyDate(model, i);
                AddColumnStatus(model, i);
            }

            return model;
        }

        public Fn03TSK_STSModel SearchEnquiryList(Fn03TSK_STSModel model)
        {
            model = DA.SearchEnquiryList(model);
            for (int i = 0; i < model.Data.Count; i++)
            {
                AddColumnDSNOrICCNo(model, i);
                AddColumnFinalAndInterimReplyDate(model, i);
                AddColumnStatus(model, i);
            }
            return model;
        }

        public Fn03TSK_STSModel SearchAuditList(Fn03TSK_STSModel model)
        {
            return DA.SearchAuditList(model);
        }

        //public Fn03TSK_STSModel SearchSubmissionList(Fn03TSK_STSModel model)
        //{
            
        //}

        private void AddColumnDSNOrICCNo(DisplayGrid model, int currData)
        {
            P_S_SYSTEM_VALUE dsnGeneralEntryCompleted = SV_DA.GetSSystemValueByCode(ProcessingConstant.DSN_GENERAL_ENTRY_COMPLETED);
            P_S_SYSTEM_VALUE dsnDocSorting = SV_DA.GetSSystemValueByCode(ProcessingConstant.DSN_DOCUMENT_SORTING);

            if (model.Data[currData]["SUBMIT_TYPE"].ToString() == ProcessingConstant.DB_SOURCE_ICC)
            {
                model.Data[currData].Add("DSN_ICCNO", model.Data[currData]["ICC_NUMBER"].ToString());
            }
            else
            {
                P_MW_DSN mwDSN = null;
                List<P_MW_DSN> mwDSNList = MWDSNDA.GetMwDsnListByRecordIdAndStatusID(model.Data[currData]["REFERENCE_NO"].ToString(), dsnGeneralEntryCompleted.UUID);

                if (mwDSNList.Count() <= 0)
                {
                    //sv = sSystemValueService.getSSystemValueByCode(ApplicationConstant.DSN_DOCUMENT_SORTING);
                    mwDSNList = MWDSNDA.GetMwDsnListByRecordIdAndStatusID(model.Data[currData]["REFERENCE_NO"].ToString(), dsnDocSorting.UUID);
                }

                if (mwDSNList.Count() > 0)
                {
                    mwDSN = mwDSNList[0];
                }

                if (mwDSN != null)
                {
                    model.Data[currData].Add("DSN_ICCNO", mwDSN.DSN);
                }
            }
        }

        private void AddColumnFinalAndInterimReplyDate(DisplayGrid model, int currData)
        {
            if (model.Data[currData]["RECEIVE_DATE"] != null && !string.IsNullOrWhiteSpace(model.Data[currData]["RECEIVE_DATE"].ToString()))
            {
                DateTime finalReplyDueDate = Convert.ToDateTime(model.Data[currData]["RECEIVE_DATE"]).AddDays(30);
                model.Data[currData].Add("FinalReplyDueDate", finalReplyDueDate.ToString("dd/MM/yyyy"));
                model.Data[currData].Add("FinalReplyRemainingDays", (DateTime.Now - finalReplyDueDate).Days);

                model.Data[currData].Add("InterimReplyRemainingDays", (DateTime.Now - Convert.ToDateTime(model.Data[currData]["RECEIVE_DATE"])).Days + 10);
            }
            else
            {
                model.Data[currData].Add("FinalReplyDueDate", "");
                model.Data[currData].Add("FinalReplyRemainingDays", "0");
                model.Data[currData].Add("InterimReplyRemainingDays", "0");
            }
        }

        private void AddColumnStatus(DisplayGrid model, int currData)
        {
            if (!string.IsNullOrWhiteSpace(model.Data[currData]["FORM_STATUS"].ToString()))
            {
                if (model.Data[currData]["FORM_STATUS"].ToString() == ProcessingConstant.GENERAL_RECORD_NEW)
                {
                    model.Data[currData].Add("STATUS", ProcessingConstant.STATUS_OPEN);
                }

                if (model.Data[currData]["FORM_STATUS"].ToString() == ProcessingConstant.GENERAL_RECORD_DRAFT
                    || model.Data[currData]["FORM_STATUS"].ToString() == ProcessingConstant.GENERAL_RECORD_COMPLETED)
                {
                    model.Data[currData].Add("STATUS", ProcessingConstant.STATUS_IN_PROGRESS);
                }
            }

            //if (model.Data[currData]["COMMENCEMENT_DATE"] != null)
            //{
            //    int compareResult = CompareDate(DateTime.Now, Convert.ToDateTime(model.Data[currData]["COMMENCEMENT_DATE"]));
            //    if (((compareResult == 2) || (compareResult == 1)) &&
            //          (model.Data[currData]["COMPLETION_DATE"] == null)
            //       )
            //    {
            //        model.Data[currData]["STATUS"] = ProcessingConstant.MW_STATUS_IN_PROGRESS;
            //    }
            //}
            //if (model.Data[currData]["COMPLETION_DATE"] != null)
            //{
            //    int compareResult = CompareDate(DateTime.Now, Convert.ToDateTime(model.Data[currData]["COMPLETION_DATE"]));
            //    if (compareResult == 2 || compareResult == 1)
            //    {
            //        model.Data[currData]["STATUS"] = ProcessingConstant.MW_STATUS_COMPLETED;
            //    }
            //}
        }
    }

}