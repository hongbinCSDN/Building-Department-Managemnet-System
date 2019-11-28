using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Services.ProcessingDAO.DAO;
using MWMS2.Areas.MWProcessing.Models;
using System.Text;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;

namespace MWMS2.Services.ProcessingDAO.Services
{
    public class ProcessingTSKGSBLService
    {
        private ProcessingTSKGSDAOService _DA;
        protected ProcessingTSKGSDAOService DA
        {
            get
            {
                return _DA ?? (_DA = new ProcessingTSKGSDAOService());
            }
        }
        private ProcessingSystemValueDAOService _SVDA;
        protected ProcessingSystemValueDAOService SVDA
        {
            get
            {
                return _SVDA ?? (_SVDA = new ProcessingSystemValueDAOService());
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

        private string Search_whereq(Fn03TSK_GSModel model)
        {
            StringBuilder whereq = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(model.SubmissionType))
            {
                whereq.Append(@" and ((record.SUBMIT_TYPE = :submitType) or (record.SUBMIT_TYPE='ICC' and record.ICC_TYPE =:submitType)) ");
                model.QueryParameters.Add("submitType", model.SubmissionType);
            }

            if (!string.IsNullOrWhiteSpace(model.RefNo))
            {
                whereq.Append(" and lower(referenceNumber.reference_no) like lower(:mwReferenceNo) ");
                model.QueryParameters.Add("mwReferenceNo", "%" + model.RefNo + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Status))
            {
                if (ProcessingConstant.ENQUIRY_STATUS_IN_PROGRESS_DISPALY.Equals(model.Status))
                {
                    whereq.Append(" and WT.TASK_CODE != :statusId ");
                    model.QueryParameters.Add("statusId", ProcessingConstant.WF_GO_TASK_END);
                }
                else if (ProcessingConstant.ENQUIRY_STATUS_CLOSED.Equals(model.Status))
                {
                    whereq.Append(" and WT.TASK_CODE = :statusId ");
                    model.QueryParameters.Add("statusId", ProcessingConstant.WF_GO_TASK_END);
                }
               
            }
            if (!string.IsNullOrWhiteSpace(model.Source))
            {
                whereq.Append(@" and record.source = :source ");
                model.QueryParameters.Add("source", model.Source);
            }
            if (!string.IsNullOrWhiteSpace(model.ReceiveDate))
            {
                whereq.Append(@" and TO_DATE(TO_CHAR(record.receive_Date, 'DD/MM/YYYY'),'DD/MM/YYYY') = TO_DATE(:receiveDate, 'DD/MM/YYYY') ");
                model.QueryParameters.Add("receiveDate", model.ReceiveDate);
            }
            if (!string.IsNullOrWhiteSpace(model.ReferralDate))
            {
                whereq.Append(@" and To_DATE(TO_CHAR(record.referral_Date,'DD/MM/YYYY'),'DD/MM/YYYY') = TO_CHAR(:referralDate, 'DD/MM/YYYY') ");
                model.QueryParameters.Add("referralDate", model.ReferralDate);
            }
            if (!string.IsNullOrWhiteSpace(model.Progress))
            {
                whereq.Append(@" and record.progress = :progress ");
                model.QueryParameters.Add("progress", model.Progress);
            }
            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                whereq.Append(@" and ( (lower(record.english_Name) like :name) or (lower(record.chinese_Name) like :name)) ");
                if (model.NameType == "W")
                {
                    model.QueryParameters.Add("name", "%" + model.Name.ToLower() + "%");
                }
                else
                {
                    model.QueryParameters.Add("name", model.Name.ToLower() + "%");
                }
            }
            if (!string.IsNullOrWhiteSpace(model.Keyword))
            {
                whereq.Append(@" and upper(key_word) like :keyWord ");
                model.QueryParameters.Add("keyWord", "%" + model.Keyword.ToUpper() + "%");
            }
            return whereq.ToString();
        }
        public Fn03TSK_GSModel Search(Fn03TSK_GSModel model)
        {
            model.QueryWhere = Search_whereq(model);
            model = DA.Search(model);
            //CreateDisplayList(model);
            return model;
        }

        private void CreateDisplayList(Fn03TSK_GSModel model)
        {
            for (int i = 0; i < model.Data.Count(); i++)
            {
                AddColumnDSNOrICCNo(model, i);

                AddColumnFinalAndInterimReplyDate(model, i);

            }
        }

        private void AddColumnFinalAndInterimReplyDate(DisplayGrid model, int currDate)
        {
            if (model.Data[currDate]["RECEIVE_DATE"] != null && !string.IsNullOrWhiteSpace(model.Data[currDate]["RECEIVE_DATE"].ToString()))
            {
                DateTime finalReplyDueDate = Convert.ToDateTime(model.Data[currDate]["RECEIVE_DATE"]).AddDays(30);
                model.Data[currDate].Add("FinalReplyDueDate", finalReplyDueDate.ToString("dd/MM/yyyy"));
                model.Data[currDate].Add("FinalReplyRemainingDays", (DateTime.Now - finalReplyDueDate).Days);

                model.Data[currDate].Add("InterimReplyRemainingDays", (DateTime.Now - Convert.ToDateTime(model.Data[currDate]["RECEIVE_DATE"])).Days + 10);
            }
            else
            {
                model.Data[currDate].Add("FinalReplyDueDate", "");
                model.Data[currDate].Add("FinalReplyRemainingDays", "0");
                model.Data[currDate].Add("InterimReplyRemainingDays", "0");
            }
        }

        private void AddColumnDSNOrICCNo(DisplayGrid model, int currDate)
        {
            if (model.Data[currDate]["SUBMIT_TYPE"].ToString() == ProcessingConstant.SOURCE_ICC)
            {
                model.Data[currDate].Add("DSN_ICCNO", model.Data[currDate]["ICC_NUMBER"].ToString());
            }
            else
            {
                P_S_SYSTEM_VALUE sv = SVDA.GetSSystemValueByCode(ProcessingConstant.DSN_GENERAL_ENTRY_COMPLETED);
                List<P_MW_DSN> mwDSNList = MWDSNDA.GetMwDsnListByRecordIdAndStatusID(model.Data[currDate]["REFERENCE_NO"].ToString(), sv.UUID);
                P_MW_DSN mwDSN = null;

                if (mwDSNList.Count() > 0)
                {
                    mwDSN = mwDSNList[0];
                }
                else
                {
                    sv = SVDA.GetSSystemValueByCode(ProcessingConstant.DSN_DOCUMENT_SORTING);
                    mwDSNList = MWDSNDA.GetMwDsnListByRecordIdAndStatusID(model.Data[currDate]["REFERENCE_NO"].ToString(), sv.UUID);
                }

                if (mwDSN != null)
                {
                    model.Data[currDate].Add("DSN_ICCNO", mwDSN.DSN);
                }
                else
                {
                    model.Data[currDate].Add("DSN_ICCNO", "");
                }
            }
        }

    }
}