using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Services
{
    public class ProcessingVerficationBLService
    {
        //ProcessingVerficationDAOService
        private ProcessingVerificationDAOService DAOService;
        protected ProcessingVerificationDAOService DA
        {
            get { return DAOService ?? (DAOService = new ProcessingVerificationDAOService()); }
        }

        private SYS_UNIT_DAOService _sysUnitService;
        protected SYS_UNIT_DAOService sysUnitService
        {
            get { return _sysUnitService ?? (_sysUnitService = new SYS_UNIT_DAOService()); }
        }

        public string Search_whereq(Fn04VRF_VRFModel model)
        {
            StringBuilder whereq = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(model.RefNo))
            {
                whereq.Append(@" and REFERENCE_NO=:refNo ");
                model.QueryParameters.Add("refNo", model.RefNo);
            }

            if (!string.IsNullOrWhiteSpace(model.SubmissionDateFrom) && !string.IsNullOrWhiteSpace(model.SubmissionDateTo))
            {
                whereq.Append(" and To_date(to_char(commencement_submission_date,'dd/MM/yyyy'), 'dd/MM/yyyy')>= to_date(:submissionDateFrom,'dd/MM/yyyy') ");
                model.QueryParameters.Add("submissionDateFrom", model.SubmissionDateFrom);
                whereq.Append(" and To_date(to_char(commencement_submission_date,'dd/MM/yyyy'), 'dd/MM/yyyy')<= to_date(:submissionDateTo,'dd/MM/yyyy') ");
                model.QueryParameters.Add("submissionDateTo", model.SubmissionDateTo.Trim());
            }
            else if (!string.IsNullOrWhiteSpace(model.SubmissionDateFrom))
            {
                whereq.Append(" and To_date(to_char(commencement_submission_date,'dd/MM/yyyy'), 'dd/MM/yyyy')>= to_date(:submissionDateFrom,'dd/MM/yyyy') ");
                model.QueryParameters.Add("submissionDateFrom", model.SubmissionDateFrom.Trim());
            }
            else if (!string.IsNullOrWhiteSpace(model.SubmissionDateTo))
            {
                whereq.Append(" and To_date(to_char(commencement_submission_date,'dd/MM/yyyy'), 'dd/MM/yyyy')<= to_date(:submissionDateTo,'dd/MM/yyyy') ");
                model.QueryParameters.Add("submissionDateTo", model.SubmissionDateTo.Trim());
            }

            if (!string.IsNullOrWhiteSpace(model.FormNo))
            {
                whereq.Append(" and S_FORM_TYPE_CODE=:formNo ");
                model.QueryParameters.Add("formNo", model.FormNo.Trim());
            }

            if (!string.IsNullOrEmpty(model.Status))
            {
                if (model.Status == "Open")
                {
                    whereq.Append(" AND PROGRESS = 'Open'  ");
                }
                else 
                {
                    whereq.Append(" AND PROGRESS = 'In progress'  ");
                }
            }

            if (!string.IsNullOrWhiteSpace(model.SubmissionType))
            {
                whereq.Append(" AND MW_PROGRESS_STATUS_CODE=:SubmissionType ");
                model.QueryParameters.Add("SubmissionType", model.SubmissionType);
            }

            if (!string.IsNullOrWhiteSpace(model.MWItem))
            {
                whereq.Append(" AND ITEMS like :items ");
                model.QueryParameters.Add("items", model.MWItem + "%");
            }

            //Get user unit 
            SYS_UNIT sysUnit = sysUnitService.GetSYS_UNITByUuid(SessionUtil.LoginPost.SYS_UNIT_ID) ?? new SYS_UNIT();

            string handlingUnit = ProcessingConstant.UNIT_SU.Equals(sysUnit.CODE) ? ProcessingConstant.HANDLING_UNIT_SMM : ProcessingConstant.HANDLING_UNIT_PEM;
            model.QueryParameters.Add("handlingUnit", handlingUnit);

            return whereq.ToString();
        }

        public Fn04VRF_VRFModel Search(Fn04VRF_VRFModel model)
        {
            model.QueryWhere = Search_whereq(model);
            return DA.Search(model);
        }

    }
}