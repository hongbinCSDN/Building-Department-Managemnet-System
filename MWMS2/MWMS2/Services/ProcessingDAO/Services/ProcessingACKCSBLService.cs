using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Services.ProcessingDAO.DAO;
using MWMS2.Utility;

namespace MWMS2.Services.ProcessingDAO.Services
{
    public class ProcessingACKCSBLService
    {
        private ProcessingACKCSDAOService _DA;
        protected ProcessingACKCSDAOService DA
        {
            get
            {
                return _DA ?? (_DA = new ProcessingACKCSDAOService());
            }
        }

        private SYS_UNIT_DAOService _sysUnitService;
        protected SYS_UNIT_DAOService sysUnitService
        {
            get { return _sysUnitService ?? (_sysUnitService = new SYS_UNIT_DAOService()); }
        }

        public string Search_whereq(Fn06ACKCS_ACKCSModel model)
        {
            StringBuilder whereq = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(model.MWNo))
            {
                whereq.Append(@" and reference_no=:refNo ");
                model.QueryParameters.Add("refNo", model.MWNo.Trim());
            }

            if (!string.IsNullOrWhiteSpace(model.MWClass))
            {
                whereq.Append(@" AND CLASS_CODE=:classCode ");
                model.QueryParameters.Add("classCode", model.MWClass);
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

            if (!string.IsNullOrWhiteSpace(model.SubmissionType))
            {
                whereq.Append(" AND MW_PROGRESS_STATUS_CODE=:SubmissionType ");
                model.QueryParameters.Add("SubmissionType", model.SubmissionType);
            }

            if (!string.IsNullOrEmpty(model.Status))
            {
                if (model.Status == ProcessingConstant.STATUS_OPEN)
                {
                    whereq.Append(" AND PROGRESS = 'Open'  ");
                }
                else
                {
                    whereq.Append(" AND PROGRESS = 'In progress'  ");
                }
            }

            //Get user unit 
            SYS_UNIT sysUnit = sysUnitService.GetSYS_UNITByUuid(SessionUtil.LoginPost.SYS_UNIT_ID) ?? new SYS_UNIT();

            string handlingUnit = ProcessingConstant.UNIT_SU.Equals(sysUnit.CODE) ? ProcessingConstant.HANDLING_UNIT_SMM : ProcessingConstant.HANDLING_UNIT_PEM;
            model.QueryParameters.Add("handlingUnit", handlingUnit);

            return whereq.ToString();
        }

        public Fn06ACKCS_ACKCSModel Search(Fn06ACKCS_ACKCSModel model)
        {
            model.QueryWhere = Search_whereq(model);
            return DA.Search(model);
        }

        public string Excel(Fn06ACKCS_ACKCSModel model)
        {
            model.QueryWhere = Search_whereq(model);
            return DA.Excel(model);
        }
    }
}