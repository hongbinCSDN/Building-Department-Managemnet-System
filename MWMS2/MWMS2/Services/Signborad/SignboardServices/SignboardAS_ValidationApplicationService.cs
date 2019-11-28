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
using MWMS2.Services.Signborad.WorkFlow;

namespace MWMS2.Services.Signborad.SignboardServices
{
    public class SignboardAS_ValidationApplicationService : BaseCommonService
    {
        string SearchSRC_VA_q = ""
                    + "\r\n" + "\t" + " select distinct svv.uuid as VALIDATION_UUID, svr.reference_no as REF_NO, svr.received_date as RCV_DATE,"
                    + "\r\n" + "\t" + " CASE WHEN wfu.status = '" + SignboardConstant.WF_STATUS_OPEN + "' THEN 'Open' ELSE 'Close' END as STATUS_A,"
                    + "\r\n" + "\t" + " CASE WHEN wft.task_code = '" + SignboardConstant.WF_MAP_VALIDATION_TO + "' THEN '" + SignboardConstant.DISPLAY_WF_MAP_VALIDATION_TO + "'"
                    + "\r\n" + "\t" + " WHEN wft.task_code = '" + SignboardConstant.WF_MAP_VALIDATION_PO + "' THEN '" + SignboardConstant.DISPLAY_WF_MAP_VALIDATION_PO + "'"
                    + "\r\n" + "\t" + " WHEN wft.task_code = '" + SignboardConstant.WF_MAP_VALIDATION_SPO + "' THEN '" + SignboardConstant.DISPLAY_WF_MAP_VALIDATION_SPO + "' END as TASK_CODE,"
                    + "\r\n" + "\t" + " wft.status as STATUS_B,"
                    + "\r\n" + "\t" + "  svr.FORM_CODE as FORM_CODE"
                    + "\r\n" + "\t" + " from"
                    + "\r\n" + "\t" + " b_sv_validation svv, b_sv_record svr, b_wf_info wfi, b_wf_task wft, b_wf_task_user wfu"
                    + "\r\n" + "\t" + " where svr.uuid = svv.sv_record_id"
                    + "\r\n" + "\t" + " and svr.uuid = wfi.record_id"
                    + "\r\n" + "\t" + " and wfi.uuid = wft.wf_info_id"
                    + "\r\n" + "\t" + " and wft.uuid = wfu.wf_task_id"
                    + "\r\n" + "\t" + " and wft.task_code in ('" + SignboardConstant.WF_MAP_VALIDATION_TO + "','" + SignboardConstant.WF_MAP_VALIDATION_PO + "','" + SignboardConstant.WF_MAP_VALIDATION_SPO + "')";


        public Fn03SRC_VASearchModel SearchSRC_VA(Fn03SRC_VASearchModel model)
        {
            model.Query = SearchSRC_VA_q;
            model.QueryWhere = SearchSRC_VA_whereQ(model);

            model.Search();
            return model;
        }
        private string SearchSRC_VA_whereQ(Fn03SRC_VASearchModel model)
        {
            string whereQ = "";
            string CurrUserId = "";
            string RefNo = "";
            DateTime FromDate = new DateTime();
            DateTime ToDate = new DateTime();
            List<String> StatusList = new List<string>();

            CurrUserId = SessionUtil.LoginPost.UUID;    // Current logged in user's uuid

            if (!string.IsNullOrWhiteSpace(model.RefNo))
            {
                RefNo += model.RefNo;
                whereQ += "\r\n\t" + " and lower(svr.reference_no) like :fileRefNo ";
                model.QueryParameters.Add("fileRefNo", "%" + RefNo.ToLower() + "%");
            }
            if (model.PeriodDateFrom.HasValue)
            {
                FromDate = model.PeriodDateFrom.Value;
                whereQ += "\r\n\t" + " and svr.received_date >= :receivedDateFrom ";
                model.QueryParameters.Add("receivedDateFrom", FromDate);
            }
            if (model.PeriodDateTo.HasValue)
            {
                ToDate = model.PeriodDateTo.Value;
                whereQ += "\r\n\t" + " and svr.received_date <= :receivedDateTo ";
                model.QueryParameters.Add("receivedDateTo", ToDate);
            }
            if (model.Status != null)
            {
                if (model.Status.Equals("Open"))
                {
                    whereQ += "\r\n\t" + " and wfu.status = '" + SignboardConstant.WF_STATUS_OPEN + "'";
                }
                else if (model.Status.Equals("Close"))
                {
                    whereQ += "\r\n\t" + " and wfu.status in ('" + SignboardConstant.WF_STATUS_CLOSE + "', '" + SignboardConstant.WF_STATUS_DONE + "')";
                }
            }

            whereQ += "\r\n\t" + " and wfu.sys_post_id = :userId ";
            model.QueryParameters.Add("userId", CurrUserId);

            return whereQ;
        }
        // Export Seach Function
        public string ExportSRC_VA(Fn03SRC_VASearchModel model)
        {
            model.Query = SearchSRC_VA_q;
            model.QueryWhere = SearchSRC_VA_whereQ(model);
            return model.Export("Validation Application Report");
        }
    }
}