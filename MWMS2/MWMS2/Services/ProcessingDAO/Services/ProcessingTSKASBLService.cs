using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.DAO;

namespace MWMS2.Services.ProcessingDAO.Services
{
    public class ProcessingTSKASBLService
    {
        private ProcessingTSKASDAOService _DA;
        protected ProcessingTSKASDAOService DA
        {
            get
            {
                return _DA ?? (_DA = new ProcessingTSKASDAOService());
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
        public Fn03TSK_ASModel SearchMwRecordByAddress(Fn03TSK_ASModel model)
        {
            model = DA.GetMwRecordByAddress(model);
            for (int i = 0; i < model.Data.Count; i++)
            {
                AddColumnStatus(model, i);
            }
            return model;
        }

        public string ExcelRecord(Fn03TSK_ASModel model)
        {
            return DA.ExcelRecord(model);
        }

        public Fn03TSK_ASModel SearchMWGeneralRecordByAddress(Fn03TSK_ASModel model)
        {
            model = DA.GetMWGeneralRecordByAddress(model);
            for (int i = 0; i < model.Data.Count; i++)
            {
                AddColumnDSNOrICCNo(model, i);
                AddColumnFinalAndInterimReplyDate(model, i);
            }

            return model;
        }

        public string ExcelGR(Fn03TSK_ASModel model)
        {
            return DA.ExcelGR(model);
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

        private void AddColumnDSNOrICCNo(DisplayGrid model, int currData)
        {
            if (model.Data[currData]["SUBMIT_TYPE"].ToString() == ProcessingConstant.SOURCE_ICC)
            {
                model.Data[currData].Add("DSN_ICCNO", model.Data[currData]["ICC_NUMBER"].ToString());
            }
            else
            {
                P_S_SYSTEM_VALUE sv = SVDA.GetSSystemValueByCode(ProcessingConstant.DSN_GENERAL_ENTRY_COMPLETED);
                List<P_MW_DSN> mwDSNList = MWDSNDA.GetMwDsnListByRecordIdAndStatusID(model.Data[currData]["REFERENCE_NO"].ToString(), sv.UUID);
                P_MW_DSN mwDSN = null;

                if (mwDSNList.Count() > 0)
                {
                    mwDSN = mwDSNList[0];
                }
                else
                {
                    sv = SVDA.GetSSystemValueByCode(ProcessingConstant.DSN_DOCUMENT_SORTING);
                    mwDSNList = MWDSNDA.GetMwDsnListByRecordIdAndStatusID(model.Data[currData]["REFERENCE_NO"].ToString(), sv.UUID);
                }

                if (mwDSN != null)
                {
                    model.Data[currData].Add("DSN_ICCNO", mwDSN.DSN);
                }
            }
        }

        private void AddColumnStatus(DisplayGrid model, int currData)
        {
            if (model.Data[currData]["COMMENCEMENT_DATE"] != null)
            {
                int compareResult = CompareDate(DateTime.Now, Convert.ToDateTime(model.Data[currData]["COMMENCEMENT_DATE"]));
                if (((compareResult == 2) || (compareResult == 1)) &&
                      (model.Data[currData]["COMPLETION_DATE"] == null)
                   )
                {
                    model.Data[currData]["STATUS"] = ProcessingConstant.MW_STATUS_IN_PROGRESS;
                }
            }
            if (model.Data[currData]["COMPLETION_DATE"] != null)
            {
                int compareResult = CompareDate(DateTime.Now, Convert.ToDateTime(model.Data[currData]["COMPLETION_DATE"]));
                if (compareResult == 2 || compareResult == 1)
                {
                    model.Data[currData]["STATUS"] = ProcessingConstant.MW_STATUS_COMPLETED;
                }
            }
        }

        private int CompareDate(DateTime d1, DateTime d2)
        {
            short vl = 1;
            int year = d1.Year;
            int month = d1.Month;
            int day = d1.Day;

            int tempYear = d2.Year;
            int tempMonth = d2.Month;
            int tempDay = d2.Day;

            if (year != tempYear)
            {
                if (year > tempYear)
                    vl = 2;
                else
                    vl = 0;
            }
            else
            {
                if (month != tempMonth)
                {
                    if (month > tempMonth)
                        vl = 2;
                    else
                        vl = 0;
                }
                else
                {
                    if (day != tempDay)
                    {
                        if (day > tempDay)
                            vl = 2;
                        else
                            vl = 0;
                    }
                }
            }
            return vl;
        }
    }
}