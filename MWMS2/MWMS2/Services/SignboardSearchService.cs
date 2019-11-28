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
using MWMS2.Areas.Admin.Models;
using System.IO;
using OfficeOpenXml;
using Microsoft.Office.Interop.Word;
using MWMS2.Services.Signborad.SignboardDAO;

namespace MWMS2.Services
{
    public class SignboardSearchService
    {
        string SearchUJA_q = " \r\n select po.UUID as UUID, rec.REFERENCE_NO as REFERENCE_NO , CASE "
                             + " \r\n\t WHEN info.RECORD_TYPE = 'WF_TYPE_VALIDATION' THEN 'Validation' "
                             + " \r\n\t WHEN info.RECORD_TYPE = 'WF_TYPE_GC' THEN 'Government Contractor' "
                             + " \r\n\t WHEN info.RECORD_TYPE = 'WF_TYPE_S24' THEN 'S24 Order' END AS RECORD_TYPE,"
                             + " \r\n\t CASE WHEN wtf.task_Code = '" + SignboardConstant.WF_MAP_VALIDATION_ISSUE_LETTER_TO + "' THEN '" + SignboardConstant.DISPLAY_WF_MAP_VALIDATION_ISSUE_LETTER_TO + "'"
                             + " \r\n\t WHEN wtf.task_Code = '" + SignboardConstant.WF_MAP_VALIDATION_TO + "' THEN '" + SignboardConstant.DISPLAY_WF_MAP_VALIDATION_TO + "'"
                             + " \r\n\t WHEN wtf.task_Code = '" + SignboardConstant.WF_MAP_VALIDATION_PO + "' THEN '" + SignboardConstant.DISPLAY_WF_MAP_VALIDATION_PO + "'"
                             + " \r\n\t WHEN wtf.task_Code = '" + SignboardConstant.WF_MAP_VALIDATION_SPO + "' THEN '" + SignboardConstant.DISPLAY_WF_MAP_VALIDATION_SPO + "'"
                             + " \r\n\t WHEN wtf.task_Code = '" + SignboardConstant.WF_MAP_AUDIT_TO + "' THEN '" + SignboardConstant.DISPLAY_WF_MAP_AUDIT_TO + "'"
                             + " \r\n\t WHEN wtf.task_Code = '" + SignboardConstant.WF_MAP_AUDIT_PO + "' THEN '" + SignboardConstant.DISPLAY_WF_MAP_AUDIT_PO + "'"
                             + " \r\n\t WHEN wtf.task_Code = '" + SignboardConstant.WF_MAP_AUDIT_SPO + "' THEN '" + SignboardConstant.DISPLAY_WF_MAP_AUDIT_SPO + "'"
                             + " \r\n\t END as TASK "
                             + " \r\n\t , po.code as RECORD_USER, tu.UUID AS WFTU_UUID "
                             + " \r\n FROM B_SV_RECORD rec "
                             + " \r\n\t inner join B_wf_info info on info.record_id = rec.UUID "
                             + " \r\n\t inner join B_wf_task wtf on wtf.WF_INFO_ID = info.UUID "
                             + " \r\n\t inner join B_wf_task_user tu on tu.WF_TASK_ID = wtf.UUID "
                             + " \r\n\t inner join sys_post po on po.UUID = tu.SYS_POST_ID" 
                             + " \r\n where 1=1 ";
       
       
        private string SearchUJA_whereQ(SMMUpdateJobAssignSearchModel model)
        {
            string whereQ = "";

            whereQ += "\r\n\t AND tu.STATUS = :status";
            model.QueryParameters.Add("status", SignboardConstant.WF_STATUS_OPEN);

            if (!string.IsNullOrWhiteSpace(model.SubmissionNo))
            {
                whereQ += "\r\n\t" + "AND UPPER(rec.REFERENCE_NO) LIKE :RefNo";
                model.QueryParameters.Add("RefNo", "%" + model.SubmissionNo.Trim().ToUpper() + "%");
            }

            return whereQ;
        }


        private string getSubordinateTaskResult(Fn04RPT_STSearchModel model)
        {
            string whereQ = "";
            if (model.PeriodDateFrom == null || model.PeriodDateTo == null)
            {
                if (model.PeriodDateFrom == null)
                {
                    model.PeriodDateFrom = new DateTime(1999,1,1);

                }
                if (model.PeriodDateTo == null)
                {
                    model.PeriodDateTo = new DateTime(3000, 12,1);
                }
                whereQ = "\r\n" + "\t" + " AND R.RECEIVED_DATE BETWEEN :DateFrom AND :DateTo ";
                model.QueryParameters.Add("DateFrom", model.PeriodDateFrom);
                model.QueryParameters.Add("DateTo", model.PeriodDateTo);
            }
            string sv_recordQ = ""
                + "\r\n" + "\t" + " SELECT COUNT (DISTINCT R.UUID) as count"
                + "\r\n" + "\t" + " , sua.uuid"
                + "\r\n" + "\t" + " FROM b_WF_INFO F"
                + "\r\n" + "\t" + " INNER JOIN b_WF_TASK T ON F.UUID = T.WF_INFO_ID"
                + "\r\n" + "\t" + " INNER JOIN b_WF_TASK_USER U ON T.UUID = U.WF_TASK_ID"
                //+ "\r\n" + "\t" + " join b_s_user_account sua on u.user_id = sua.uuid"
                + "\r\n" + "\t" + " join sys_post sua on u.SYS_POST_ID = sua.uuid"
                + "\r\n" + "\t" + " INNER JOIN b_sv_record R ON F.RECORD_ID = R.UUID"
                + "\r\n" + "\t" + " INNER JOIN b_SV_VALIDATION V ON R.UUID = V.SV_RECORD_ID"
                + "\r\n" + "\t" + " WHERE T.TASK_CODE in ('"+SignboardConstant.WF_MAP_VALIDATION_TO+"','"+SignboardConstant.WF_MAP_VALIDATION_PO+"','" + SignboardConstant.WF_MAP_VALIDATION_SPO +"') "
                + whereQ
                + "\r\n" + "\t" + " AND U.STATUS = '"+model.Status+"'"
                + "\r\n" + "\t" + " group by sua.uuid"
                ;
            string sv_recordQ2 = ""
                + "\r\n" + "\t" + " SELECT COUNT (DISTINCT R.UUID) as count"
                + "\r\n" + "\t" + " , sua.uuid"
                + "\r\n" + "\t" + " FROM b_WF_INFO F"
                + "\r\n" + "\t" + " INNER JOIN b_WF_TASK T ON F.UUID = T.WF_INFO_ID"
                + "\r\n" + "\t" + " INNER JOIN b_WF_TASK_USER U ON T.UUID = U.WF_TASK_ID"
                //+ "\r\n" + "\t" + " join b_s_user_account sua on u.user_id = sua.uuid"
                + "\r\n" + "\t" + " join sys_post sua on u.SYS_POST_ID = sua.uuid"
                + "\r\n" + "\t" + " INNER JOIN b_sv_record R ON F.RECORD_ID = R.UUID"
                + "\r\n" + "\t" + " INNER JOIN b_SV_VALIDATION V ON R.UUID = V.SV_RECORD_ID"
                + "\r\n" + "\t" + " WHERE T.TASK_CODE in ('" + SignboardConstant.WF_MAP_AUDIT_TO + "','" + SignboardConstant.WF_MAP_AUDIT_PO + "','" + SignboardConstant.WF_MAP_AUDIT_SPO + "') "
                + whereQ
                + "\r\n" + "\t" + " AND U.STATUS = '" + model.Status + "'"
                + "\r\n" + "\t" + " group by sua.uuid"
                ;
            string sv_gc = ""
                + "\r\n" + "\t" + " SELECT COUNT (DISTINCT R.UUID) as count "
                + "\r\n" + "\t" + " , sua.uuid "
                + "\r\n" + "\t" + " FROM b_WF_INFO F "
                + "\r\n" + "\t" + " INNER JOIN b_WF_TASK T ON F.UUID = T.WF_INFO_ID "
                + "\r\n" + "\t" + " INNER JOIN b_WF_TASK_USER U ON T.UUID = U.WF_TASK_ID "
                //+ "\r\n" + "\t" + " join b_s_user_account sua on u.user_id = sua.uuid "
                + "\r\n" + "\t" + " join sys_post sua on u.SYS_POST_ID = sua.uuid"
                + "\r\n" + "\t" + " INNER JOIN b_sv_record R ON F.RECORD_ID = R.UUID "
                + "\r\n" + "\t" + " WHERE T.TASK_CODE in ('" + SignboardConstant.WF_MAP_GC_SPO_APPROVE + "','" + SignboardConstant.WF_MAP_GC_PO + "','" + SignboardConstant.WF_MAP_GC_SPO_COMPILE + "','"+SignboardConstant.WF_MAP_GC_PO_COMPLI+ "','" + SignboardConstant.WF_MAP_GC_END + "') "
                + whereQ
                + "\r\n" + "\t" + " AND U.STATUS = '" + model.Status + "'"
                + "\r\n" + "\t" + " group by sua.uuid"
                ;
            string sv_24_order = ""
                + "\r\n" + "\t" + " SELECT COUNT (DISTINCT R.UUID) as count "
                + "\r\n" + "\t" + " , sua.uuid "
                + "\r\n" + "\t" + " FROM b_WF_INFO F "
                + "\r\n" + "\t" + " INNER JOIN b_WF_TASK T ON F.UUID = T.WF_INFO_ID "
                + "\r\n" + "\t" + " INNER JOIN b_WF_TASK_USER U ON T.UUID = U.WF_TASK_ID "
                //+ "\r\n" + "\t" + " join b_s_user_account sua on u.user_id = sua.uuid "
                + "\r\n" + "\t" + " join sys_post sua on u.SYS_POST_ID = sua.uuid"
                + "\r\n" + "\t" + " INNER JOIN b_sv_record R ON F.RECORD_ID = R.UUID "
                + "\r\n" + "\t" + " WHERE T.TASK_CODE in ('" + SignboardConstant.WF_MAP_S24_SPO_APPROVE + "','" + SignboardConstant.WF_MAP_S24_PO + "','" + SignboardConstant.WF_MAP_S24_SPO_COMPILE + "','" + SignboardConstant.WF_MAP_S24_END +"') "
                + whereQ
                + "\r\n" + "\t" + " AND U.STATUS = '" + model.Status + "'"
                + "\r\n" + "\t" + " group by sua.uuid"
                ;
            return ""
                + "\r\n" + "\t" + "select"
                + "\r\n" + "\t" + " sua.uuid, sua.BD_PORTAL_LOGIN as USERNAME"
                + "\r\n" + "\t" + " , sum(case when val.count is not null then val.count else 0 end) as VAL"
                + "\r\n" + "\t" + " , sum(case when aud.count is not null then aud.count else 0 end) as AUD"
                + "\r\n" + "\t" + " , sum(case when gc.count is not null then gc.count else 0 end) as GC"
                + "\r\n" + "\t" + " , sum(case when s24.count is not null then s24.count else 0 end) as S24ORDER"
                //+ "\r\n" + "\t" + " , sua.RANK"
                + "\r\n" + "\t" + " , sua.SYS_RANK_ID"
                + "\r\n" + "\t" + " FROM b_s_scu_team scu"
                + "\r\n" + "\t" + " LEFT JOIN sys_post sua"
                + "\r\n" + "\t" + " ON sua.uuid = scu.SYS_POST_ID"
                //+ "\r\n" + "\t" + " LEFT JOIN b_s_user_account sua"
                //+ "\r\n" + "\t" + " ON sua.uuid = scu.user_account_id"
                + "\r\n" + "\t" + " left join ("
                + "\r\n" + "\t" + sv_recordQ
                + "\r\n" + "\t" +" ) val"
                + "\r\n" + "\t" +" on"
                + "\r\n" + "\t" +" sua.uuid = val.uuid"
                + "\r\n" + "\t" + " left join ("
                + "\r\n" + "\t" + sv_recordQ2
                + "\r\n" + "\t" +" ) aud"
                + "\r\n" + "\t" +" on"
                + "\r\n" + "\t" +" sua.uuid = aud.uuid"
                + "\r\n" + "\t" +" left join ("
                + "\r\n" + "\t" + sv_gc
                + "\r\n" + "\t" + " ) gc "
                + "\r\n" + "\t" + " on "
                + "\r\n" + "\t" + " sua.uuid = gc.uuid "
                + "\r\n" + "\t" + " left join ( "
                + "\r\n" + "\t" + sv_24_order
                + "\r\n" + "\t" +" ) s24"
                + "\r\n" + "\t" +" on "
                + "\r\n" + "\t" +" sua.uuid = s24.uuid "
                //+ "\r\n" + "\t" +" WHERE sua.security_level = '"+ SignboardConstant.S_USER_ACCOUNT_SECURITY_LEVEL_SCU_INTERNAL + "'"
                + "\r\n" + "\t" +" group by"
                //+ "\r\n" + "\t" +" sua.uuid, sua.username, sua.rank " 
                + "\r\n" + "\t" + " sua.uuid, sua.BD_PORTAL_LOGIN, sua.SYS_RANK_ID "
                ;

        }
        private string generateSubmissionProgressReport(Fn04RPT_SPRSearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.SubNo))
            {
                whereQ += "\r\n\t" + "and lower(svr.reference_no) like :signboardNumber";
                model.QueryParameters.Add("signboardNumber", "%" + model.SubNo.ToLower() + "%");
            }
            if (model.PeriodFromDate != null)
            {
                whereQ += "\r\n\t" + "and svr.received_date >= :receivedDateFrom";
                model.QueryParameters.Add("receivedDateFrom", model.PeriodFromDate);
            }
            if (model.PeriodToDate != null)
            {
                whereQ += "\r\n\t" + "and svr.received_date <= :receivedDateTo";
                model.QueryParameters.Add("receivedDateTo", model.PeriodToDate);
            }
            return ""
                    + "\r\n" + "\t" + " select distinct '' as uuid, "
                    + "\r\n" + "\t" + " received_date as RECEIVEDDATE, reference_no as SUBNO, form_code as FCODE, validation_status as STAGE,"
                    + "\r\n" + "\t" + " spo_endorsement_date as ADATE, max(to1) as TO1, max(po) as PO, max(spo) as spo"
                    + "\r\n" + "\t" + " from ("
                    + "\r\n" + "\t" + " SELECT svr.uuid as uuid,"
                    + "\r\n" + "\t" + " svr.received_date,"
                    + "\r\n" + "\t" + " svr.reference_no,"
                    + "\r\n" + "\t" + " svr.FORM_CODE,"
                    + "\r\n" + "\t" + " svr.wf_status AS validation_status,"
                    + "\r\n" + "\t" + " svv.spo_endorsement_date"
                    //+ "\r\n" + "\t" + " , decode(task_code, 'WF_VALIDATION_TO', username, '') as to1"
                    //+ "\r\n" + "\t" + " , decode(task_code, 'WF_VALIDATION_PO', username, '') as po"
                    //+ "\r\n" + "\t" + " , decode(task_code, 'WF_VALIDATION_SPO', username, '') as spo"
                    + "\r\n" + "\t" + " , decode(task_code, 'WF_VALIDATION_TO', BD_PORTAL_LOGIN, '') as to1"
                    + "\r\n" + "\t" + " , decode(task_code, 'WF_VALIDATION_PO', BD_PORTAL_LOGIN, '') as po"
                    + "\r\n" + "\t" + " , decode(task_code, 'WF_VALIDATION_SPO', BD_PORTAL_LOGIN, '') as spo"
                    + "\r\n" + "\t" + " FROM b_sv_record svr"
                    + "\r\n" + "\t" + " left join ("
                    //+ "\r\n" + "\t" + " SELECT svr1.uuid, wt.task_code, sua.username"
                    + "\r\n" + "\t" + " SELECT svr1.uuid, wt.task_code, sua.BD_PORTAL_LOGIN"
                    + "\r\n" + "\t" + " FROM b_sv_record svr1"
                    + "\r\n" + "\t" + " JOIN b_wf_info wi"
                    + "\r\n" + "\t" + " ON wi.record_id = svr1.uuid"
                    + "\r\n" + "\t" + " JOIN b_wf_task wt"
                    + "\r\n" + "\t" + " ON wt.wf_info_id = wi.uuid"
                    + "\r\n" + "\t" + " JOIN b_wf_task_user wtu"
                    + "\r\n" + "\t" + " ON wt.uuid = wtu.wf_task_id"
                    + "\r\n" + "\t" + " JOIN sys_post sua "
                    + "\r\n" + "\t" + " ON sua.uuid   = wtu.SYS_POST_ID"
                    //+ "\r\n" + "\t" + " JOIN b_s_user_account sua"
                    //+ "\r\n" + "\t" + " ON sua.uuid        = wtu.user_id"
                    + "\r\n" + "\t" + " WHERE wt.task_code in ('" + SignboardConstant.WF_MAP_VALIDATION_TO + "', '" + SignboardConstant.WF_MAP_VALIDATION_PO + "', '" + SignboardConstant.WF_MAP_VALIDATION_SPO + "')"
                    + "\r\n" + "\t" + " AND wtu.status     = '" + SignboardConstant.WF_STATUS_DONE + "'"
                    + "\r\n" + "\t" + " group by"
                    //+ "\r\n" + "\t" + " svr1.uuid, wt.task_code, sua.username"
                    + "\r\n" + "\t" + " svr1.uuid, wt.task_code, sua.BD_PORTAL_LOGIN"
                    + "\r\n" + "\t" + " ) u"
                    + "\r\n" + "\t" + " on"
                    + "\r\n" + "\t" + " svr.uuid = u.uuid"
                    + "\r\n" + "\t" + " left JOIN b_sv_validation svv"
                    + "\r\n" + "\t" + " ON svv.sv_record_id = svr.uuid"
                    + "\r\n" + "\t" + " where 1=1"
                    + "\r\n" + "\t" + whereQ
                    + "\r\n" + "\t" + " ) "
                    + "\r\n" + "\t" + " group by "
                    + "\r\n" + "\t" + " received_date, reference_no, form_code, validation_status, " 
                    + "\r\n" + "\t" + " spo_endorsement_date "
                    ;
        }

        private string generateSubmissionCountReport(Fn04RPT_SPRSearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.SubNo))
            {
                whereQ += "\r\n\t" + "and lower(reference_no) like :signboardNumber";
                model.QueryParameters.Add("signboardNumber", "%" + model.SubNo.ToLower() + "%");
            }
            if (model.PeriodFromDate != null)
            {
                whereQ += "\r\n\t" + "and received_date >= :receivedDateFrom";
                model.QueryParameters.Add("receivedDateFrom", model.PeriodFromDate);
            }
            if (model.PeriodToDate != null)
            {
                whereQ += "\r\n\t" + "and received_date <= :receivedDateTo";
                model.QueryParameters.Add("receivedDateTo", model.PeriodToDate);
            }
            return ""
                    + "\r\n" + "\t" + " select sr.username as USERNAME,CASE WHEN q1.sc01 IS NULL THEN 0 WHEN q1.sc01 IS NOT NULL THEN q1.sc01 END AS SC01,CASE WHEN q2.sc02 IS NULL THEN 0 WHEN q2.sc02 IS NOT NULL THEN q2.sc02 END AS SC02,CASE WHEN q3.sc03 IS NULL THEN 0 WHEN q3.sc03 IS NOT NULL THEN q3.sc03 END AS SC03 "
                    + " from ( "
                    //+ "\r\n" + "\t" +" select sr.username as USERNAME,q1.sc01 as SC01, q2.sc02 as SC02, q3.sc03 as SC03 from ( "
                    + "\r\n" + "\t" +" select username from( "
                    + "\r\n" + "\t" + " SELECT svr1.uuid, wt.task_code,  sua.BD_PORTAL_LOGIN as username, form_code "
                    //+ "\r\n" + "\t" + " SELECT svr1.uuid, wt.task_code,  sua.username as username, form_code "
                    + "\r\n" + "\t" +" FROM (select * from b_sv_record "
                    + "\r\n" + "\t" +" where 1=1 "
                    + "\r\n" + "\t" +  whereQ
                    + "\r\n" + "\t" +" ) svr1 "
                    + "\r\n" + "\t" +" JOIN b_wf_info wi "
                    + "\r\n" + "\t" +" ON wi.record_id = svr1.uuid "
                    + "\r\n" + "\t" +" JOIN b_wf_task wt "
                    + "\r\n" + "\t" +" ON wt.wf_info_id = wi.uuid "
                    + "\r\n" + "\t" +" JOIN b_wf_task_user wtu "
                    + "\r\n" + "\t" +" ON wt.uuid = wtu.wf_task_id "
                    + "\r\n" + "\t" + " JOIN sys_post sua "
                    + "\r\n" + "\t" + " ON sua.uuid   = wtu.SYS_POST_ID"
                    //+ "\r\n" + "\t" +" JOIN b_s_user_account sua "
                    //+ "\r\n" + "\t" +" ON sua.uuid  = wtu.user_id "
                    + "\r\n" + "\t" +" WHERE wt.task_code in ('" + SignboardConstant.WF_MAP_VALIDATION_TO + "', '" + SignboardConstant.WF_MAP_VALIDATION_PO + "', '" + SignboardConstant.WF_MAP_VALIDATION_SPO + "')"
                    + "\r\n" + "\t" +" AND wtu.status = '"+SignboardConstant.WF_STATUS_DONE+"'"
                    + "\r\n" + "\t" + " group by svr1.uuid, wt.task_code, form_code, sua.BD_PORTAL_LOGIN) "
                    //+ "\r\n" + "\t" +" group by svr1.uuid, wt.task_code, form_code, sua.username) "
                    + "\r\n" + "\t" +" group by username "
                    + "\r\n" + "\t" +" )sr "
                    + "\r\n" + "\t" +" left join "
                    + "\r\n" + "\t" +" (select username, count(*) sc01 from ( "
                    + "\r\n" + "\t" + " SELECT svr1.uuid, wt.task_code,  sua.BD_PORTAL_LOGIN as username "
                    //+ "\r\n" + "\t" +" SELECT svr1.uuid, wt.task_code,  sua.username as username "
                    + "\r\n" + "\t" +" FROM (select * from b_sv_record "
                    + "\r\n" + "\t" +" where 1=1 "
                    + "\r\n" + "\t" +  whereQ
                    + "\r\n" + "\t" +" ) svr1 "
                    + "\r\n" + "\t" +" JOIN b_wf_info wi "
                    + "\r\n" + "\t" +" ON wi.record_id = svr1.uuid "
                    + "\r\n" + "\t" +" JOIN b_wf_task wt "
                    + "\r\n" + "\t" +" ON wt.wf_info_id = wi.uuid "
                    + "\r\n" + "\t" +" JOIN b_wf_task_user wtu "
                    + "\r\n" + "\t" +" ON wt.uuid = wtu.wf_task_id "
                    + "\r\n" + "\t" + " JOIN sys_post sua "
                    + "\r\n" + "\t" + " ON sua.uuid   = wtu.SYS_POST_ID"
                    //+ "\r\n" + "\t" +" JOIN b_s_user_account sua "
                    //+ "\r\n" + "\t" +" ON sua.uuid  = wtu.user_id "
                    + "\r\n" + "\t" + " WHERE wt.task_code in ('" + SignboardConstant.WF_MAP_VALIDATION_TO + "', '" + SignboardConstant.WF_MAP_VALIDATION_PO + "', '" + SignboardConstant.WF_MAP_VALIDATION_SPO + "')"
                    + "\r\n" + "\t" +" AND wtu.status = '"+SignboardConstant.WF_STATUS_DONE+"'"
                    + "\r\n" + "\t" +" and form_code='SC01' "
                    + "\r\n" + "\t" +" group by svr1.uuid, wt.task_code, sua.BD_PORTAL_LOGIN "
                    //+ "\r\n" + "\t" + " group by svr1.uuid, wt.task_code, sua.username "
                    + "\r\n" + "\t" +" ) "
                    + "\r\n" + "\t" +" group by username "
                    + "\r\n" + "\t" +" ) q1 "
                    + "\r\n" + "\t" +" on q1.username= sr.username "
                    + "\r\n" + "\t" +" left join "
                    + "\r\n" + "\t" +" (select username, count(*) sc02 from ( "
                    + "\r\n" + "\t" + " SELECT svr1.uuid, wt.task_code,  sua.BD_PORTAL_LOGIN as username "
                    //+ "\r\n" + "\t" +" SELECT svr1.uuid, wt.task_code,  sua.username as username "
                    + "\r\n" + "\t" +" FROM (select * from b_sv_record "
                    + "\r\n" + "\t" +" where 1=1 "
                    + "\r\n" + "\t" + whereQ
                    + "\r\n" + "\t" +" ) svr1 "
                    + "\r\n" + "\t" +" JOIN b_wf_info wi "
                    + "\r\n" + "\t" +" ON wi.record_id = svr1.uuid "
                    + "\r\n" + "\t" +" JOIN b_wf_task wt "
                    + "\r\n" + "\t" +" ON wt.wf_info_id = wi.uuid "
                    + "\r\n" + "\t" +" JOIN b_wf_task_user wtu "
                    + "\r\n" + "\t" +" ON wt.uuid = wtu.wf_task_id "
                    + "\r\n" + "\t" + " JOIN sys_post sua "
                    + "\r\n" + "\t" + " ON sua.uuid   = wtu.SYS_POST_ID"
                    //+ "\r\n" + "\t" +" JOIN b_s_user_account sua "
                    //+ "\r\n" + "\t" +" ON sua.uuid  = wtu.user_id "
                    + "\r\n" + "\t" +" WHERE wt.task_code in ('" + SignboardConstant.WF_MAP_VALIDATION_TO + "', '" + SignboardConstant.WF_MAP_VALIDATION_PO + "', '" + SignboardConstant.WF_MAP_VALIDATION_SPO + "')"
                    + "\r\n" + "\t" +" AND wtu.status = '" + SignboardConstant.WF_STATUS_DONE + "'"
                    + "\r\n" + "\t" +" and form_code='SC02' "
                    + "\r\n" + "\t" +" group by svr1.uuid, wt.task_code, sua.BD_PORTAL_LOGIN "
                    + "\r\n" + "\t" +" ) "
                    + "\r\n" + "\t" +" group by username "
                    + "\r\n" + "\t" +" ) q2 "
                    + "\r\n" + "\t" +" on q2.username= sr.username "
                    + "\r\n" + "\t" +" left join "
                    + "\r\n" + "\t" +" (select username, count(*) sc03 from ( "
                    + "\r\n" + "\t" + " SELECT svr1.uuid, wt.task_code,  sua.BD_PORTAL_LOGIN as username "
                    //+ "\r\n" + "\t" +" SELECT svr1.uuid, wt.task_code,  sua.username as username "
                    + "\r\n" + "\t" +" FROM (select * from b_sv_record where 1=1 "
                    + "\r\n" + "\t" + whereQ
                    + "\r\n" + "\t" + ") svr1 "
                    + "\r\n" + "\t" + "JOIN b_wf_info wi "
                    + "\r\n" + "\t" + "ON wi.record_id = svr1.uuid "
                    + "\r\n" + "\t" + "JOIN b_wf_task wt "
                    + "\r\n" + "\t" + "ON wt.wf_info_id = wi.uuid "
                    + "\r\n" + "\t" + "JOIN b_wf_task_user wtu "
                    + "\r\n" + "\t" + "ON wt.uuid = wtu.wf_task_id "
                    + "\r\n" + "\t" + " JOIN sys_post sua "
                    + "\r\n" + "\t" + " ON sua.uuid   = wtu.SYS_POST_ID"
                    //+ "\r\n" + "\t" + "JOIN B_s_user_account sua "
                    //+ "\r\n" + "\t" + "ON sua.uuid  = wtu.user_id "
                    + "\r\n" + "\t" + "WHERE wt.task_code in ('" + SignboardConstant.WF_MAP_VALIDATION_TO + "', '" + SignboardConstant.WF_MAP_VALIDATION_PO + "', '" + SignboardConstant.WF_MAP_VALIDATION_SPO + "')"
                    + "\r\n" + "\t" + "AND wtu.status = '" + SignboardConstant.WF_STATUS_DONE + "'"
                    + "\r\n" + "\t" + "and form_code='SC03' "
                    + "\r\n" + "\t" + "group by svr1.uuid, wt.task_code, sua.BD_PORTAL_LOGIN "
                    + "\r\n" + "\t" + ") "
                    + "\r\n" + "\t" + "group by username "
                    + "\r\n" + "\t" + ") q3 "
                    + "\r\n" + "\t" + "on q3.username= sr.username "
                    //+"\r\n" + "\t" + "order by sr.username "
                    ;
        }
        private string generateSubmissionInvolvedReport(Fn04RPT_SPRSearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.SubNo))
            {
                whereQ += "\r\n\t" + "and lower(reference_no) like :signboardNumber";
                model.QueryParameters.Add("signboardNumber", "%" + model.SubNo.ToLower() + "%");
            }
            if (model.PeriodFromDate != null)
            {
                whereQ += "\r\n\t" + "and received_date >= :receivedDateFrom";
                model.QueryParameters.Add("receivedDateFrom", model.PeriodFromDate);
            }
            if (model.PeriodToDate != null)
            {
                whereQ += "\r\n\t" + "and received_date <= :receivedDateTo";
                model.QueryParameters.Add("receivedDateTo", model.PeriodToDate);
            }
            return ""
                +"\r\n\t" + " select sr.username as USERNAME, "
                +"\r\n\t" + " 	 CASE WHEN q1.sc01 IS NULL THEN 0 WHEN q1.sc01 IS NOT NULL THEN q1.sc01 END AS SC01,CASE WHEN q2.sc02 IS NULL THEN 0 WHEN q2.sc02 IS NOT NULL THEN q2.sc02 END AS SC02,CASE WHEN q3.sc03 IS NULL THEN 0 WHEN q3.sc03 IS NOT NULL THEN q3.sc03 END AS SC03 "
                +"\r\n\t" + " from ( " 
                +"\r\n\t" + " select username from( "
                + "\r\n\t" + " SELECT svr1.uuid, wt.task_code,  sua.BD_PORTAL_LOGIN as username, form_code "
                //+ "\r\n\t" + " SELECT svr1.uuid, wt.task_code,  sua.username as username, form_code "
                + "\r\n\t" + " FROM (select * from b_sv_record "
                +"\r\n\t" + " where 1=1 "
                +"\r\n\t" + whereQ
                +"\r\n\t" + " ) svr1 " 
                +"\r\n\t" + " JOIN b_wf_info wi " 
                +"\r\n\t" + " ON wi.record_id = svr1.uuid " 
                +"\r\n\t" + " JOIN b_wf_task wt " 
                +"\r\n\t" + " ON wt.wf_info_id = wi.uuid " 
                +"\r\n\t" + " JOIN b_wf_task_user wtu " 
                +"\r\n\t" + " ON wt.uuid = wtu.wf_task_id "
                + "\r\n" + "\t" + " JOIN sys_post sua "
                + "\r\n" + "\t" + " ON sua.uuid   = wtu.SYS_POST_ID"
                //+"\r\n\t" + " JOIN b_s_user_account sua " 
                //+"\r\n\t" + " ON sua.uuid  = wtu.user_id " 
                + "\r\n\t" + " WHERE wt.task_code in ('" + SignboardConstant.WF_MAP_VALIDATION_TO + "', '" + SignboardConstant.WF_MAP_VALIDATION_PO + "', '" + SignboardConstant.WF_MAP_VALIDATION_SPO + "')"
                + "\r\n\t" + " AND (wtu.status = '" + SignboardConstant.WF_STATUS_DONE + "' or wtu.status = '"+ SignboardConstant.WF_STATUS_OPEN + "') "
                //+"\r\n\t" + " group by svr1.uuid, wt.task_code, form_code, sua.username) " 
                + "\r\n\t" + " group by svr1.uuid, wt.task_code, form_code, sua.BD_PORTAL_LOGIN) "
                + "\r\n\t" + " group by username " 
                +"\r\n\t" + " )sr " 
                +"\r\n\t" + " left join " 
                +"\r\n\t" + " (select username, count(*) sc01 from ( "
                + "\r\n\t" + " SELECT svr1.uuid, wt.task_code,  sua.BD_PORTAL_LOGIN as username "
                //+"\r\n\t" + " SELECT svr1.uuid, wt.task_code,  sua.username as username " 
                + "\r\n\t" + " FROM (select * from b_sv_record " 
                +"\r\n\t" + " where 1=1 "
                +"\r\n\t" + whereQ
                +"\r\n\t" +") svr1 "
                +"\r\n\t" +"JOIN b_wf_info wi "
                +"\r\n\t" +"ON wi.record_id = svr1.uuid "
                +"\r\n\t" +"JOIN b_wf_task wt "
                +"\r\n\t" +"ON wt.wf_info_id = wi.uuid "
                +"\r\n\t" +"JOIN b_wf_task_user wtu "
                +"\r\n\t" +"ON wt.uuid = wtu.wf_task_id "
                + "\r\n" + "\t" + " JOIN sys_post sua "
                + "\r\n" + "\t" + " ON sua.uuid   = wtu.SYS_POST_ID"
                //+"\r\n\t" +"JOIN b_s_user_account sua "
                //+"\r\n\t" +"ON sua.uuid  = wtu.user_id "
                + "\r\n\t" + "WHERE wt.task_code in ('" + SignboardConstant.WF_MAP_VALIDATION_TO + "', '" + SignboardConstant.WF_MAP_VALIDATION_PO + "', '" + SignboardConstant.WF_MAP_VALIDATION_SPO + "')"
                + "\r\n\t" + "AND (wtu.status = '" + SignboardConstant.WF_STATUS_DONE + "' or wtu.status = '" + SignboardConstant.WF_STATUS_OPEN + "') "
                + "\r\n\t" +"and form_code='SC01' "
                //+"\r\n\t" +"group by svr1.uuid, wt.task_code, sua.username "
                +"\r\n\t" +"group by svr1.uuid, wt.task_code, sua.BD_PORTAL_LOGIN "
                +"\r\n\t" +") "
                +"\r\n\t" +"group by username "
                +"\r\n\t" +") q1 "
                +"\r\n\t" +"on q1.username= sr.username "
                +"\r\n\t" +"left join "
                +"\r\n\t" +"(select username, count(*) sc02 from ( "
                //+"\r\n\t" +"SELECT svr1.uuid, wt.task_code,  sua.username as username "
                + "\r\n\t" + "SELECT svr1.uuid, wt.task_code,  sua.BD_PORTAL_LOGIN as username "
                + "\r\n\t" +"FROM (select * from b_sv_record "
                +"\r\n\t" + "where 1=1 "
                +"\r\n\t" + whereQ 
                +"\r\n\t" + ") svr1 "
				+"\r\n\t" + "JOIN b_wf_info wi "
				+"\r\n\t" + "ON wi.record_id = svr1.uuid "
				+"\r\n\t" + "JOIN b_wf_task wt "
				+"\r\n\t" + "ON wt.wf_info_id = wi.uuid "
				+"\r\n\t" + "JOIN b_wf_task_user wtu "
				+"\r\n\t" + "ON wt.uuid = wtu.wf_task_id "
                + "\r\n" + "\t" + " JOIN sys_post sua "
                + "\r\n" + "\t" + " ON sua.uuid   = wtu.SYS_POST_ID"
    //          + "\r\n\t" + "JOIN b_s_user_account sua "
				//+"\r\n\t" + "ON sua.uuid  = wtu.user_id "
				+"\r\n\t" + "WHERE wt.task_code in ('" + SignboardConstant.WF_MAP_VALIDATION_TO + "', '" + SignboardConstant.WF_MAP_VALIDATION_PO + "', '" + SignboardConstant.WF_MAP_VALIDATION_SPO + "')"
                + "\r\n\t" + "AND (wtu.status = '" + SignboardConstant.WF_STATUS_DONE + "' or wtu.status = '" + SignboardConstant.WF_STATUS_OPEN + "') "
                + "\r\n\t" + "and form_code='SC02' "
				+"\r\n\t" + "group by svr1.uuid, wt.task_code, sua.BD_PORTAL_LOGIN "
                //+"\r\n\t" + "group by svr1.uuid, wt.task_code, sua.username "
				+"\r\n\t" + ") "
				+"\r\n\t" + "group by username "
				+"\r\n\t" + ") q2 "
				+"\r\n\t" + "on q2.username= sr.username "
				+"\r\n\t" + "left join "
				+"\r\n\t" + "(select username, count(*) sc03 from ( "
				+"\r\n\t" + "SELECT svr1.uuid, wt.task_code,  sua.BD_PORTAL_LOGIN as username "
                //+ "\r\n\t" + "SELECT svr1.uuid, wt.task_code,  sua.username as username "
                +"\r\n\t" + "FROM (select * from b_sv_record where 1=1 "
                +"\r\n\t" + whereQ
                +"\r\n\t" +") svr1 "
                +"\r\n\t" +"JOIN b_wf_info wi "
                +"\r\n\t" +"ON wi.record_id = svr1.uuid "
                +"\r\n\t" +"JOIN b_wf_task wt "
                +"\r\n\t" +"ON wt.wf_info_id = wi.uuid "
                +"\r\n\t" +"JOIN b_wf_task_user wtu "
                +"\r\n\t" +"ON wt.uuid = wtu.wf_task_id "
                + "\r\n" + "\t" + " JOIN sys_post sua "
                + "\r\n" + "\t" + " ON sua.uuid   = wtu.SYS_POST_ID"
                //+"\r\n\t" +"JOIN b_s_user_account sua "
                //+"\r\n\t" +"ON sua.uuid  = wtu.user_id "
                + "\r\n\t" + "WHERE wt.task_code in ('" + SignboardConstant.WF_MAP_VALIDATION_TO + "', '" + SignboardConstant.WF_MAP_VALIDATION_PO + "', '" + SignboardConstant.WF_MAP_VALIDATION_SPO + "')"
                + "\r\n\t" +"AND (wtu.status = '" + SignboardConstant.WF_STATUS_DONE + "' or wtu.status = '" + SignboardConstant.WF_STATUS_OPEN + "') "
                + "\r\n\t" +"and form_code='SC03' "
                //+"\r\n\t" +"group by svr1.uuid, wt.task_code, sua.username "
                +"\r\n\t" +"group by svr1.uuid, wt.task_code, sua.BD_PORTAL_LOGIN "
                +"\r\n\t" +") "
                +"\r\n\t" +"group by username "
                +"\r\n\t" +") q3 "
                +"\r\n\t" +"on q3.username= sr.username "
                //+"\r\n\t" + "order by sr.username "
                ;
        }
        string SearchDE_q = ""
                          + "\r\n" + "\t" + " select svv.uuid as UUID, svr.reference_No as REFNO, "
                          + "\r\n" + "\t" + " svr.form_code as FCODE, svr.received_date as RECDATE, "
                          //+ "\r\n" + "\t" + " svv.validation_result as VALRES, "
                          + "\r\n" + "\t" + " decode(svv.validation_result, "
                          + "\r\n" + "\t" + " 'A', '" + SignboardConstant.RECOMMEND_ACK_STR + "', "
                          + "\r\n" + "\t" + " 'R', '" + SignboardConstant.RECOMMEND_REF_STR + "', "
                          + "\r\n" + "\t" + " 'C', '" + SignboardConstant.RECOMMEND_COND_STR + "', "
                          + "\r\n" + "\t" + " svv.validation_result) as VALRES, "
                          + "\r\n" + "\t" + " svv.signboard_expiry_date as SIGNEXPDATE from b_Sv_Validation svv, b_sv_record svr "
                          + "\r\n" + "\t" + " where svv.sv_record_id= svr.uuid ";

        string SearchPPJL_q = ""
                            + "\r\n" + "\t" + "select distinct"
                            + "\r\n" + "\t" + "sap.CHINESE_NAME as CHINESE_NAME,  "
                            + "\r\n" + "\t" + "sap.ENGLISH_NAME as ENGLISH_NAME,  "
                            + "\r\n" + "\t" + "sap.CERTIFICATION_NO as CERTIFICATION_NO, "
                            + "\r\n" + "\t" + "sr.REFERENCE_NO as REFERENCE_NO,  "
                            //+ "\r\n" + "\t" + "sr.WF_STATUS as WF_STATUS,  "
                            + "\r\n" + "\t" + "addre.FULL_ADDRESS as FULL_ADDRESS, "
                            + "\r\n" + "\t" + "sr.FORM_CODE as FORM_CODE, "
                            + "\r\n" + "\t" + "sr.WF_STATUS as WF_STATUS, "
                            + "\r\n" + "\t" + "sv.VALIDATION_RESULT as VALIDATION_RESULT, "
                            + "\r\n" + "\t" + "sv.SPO_ENDORSEMENT_DATE as SPO_ENDORSEMENT_DATE "
                            + "\r\n" + "\t" + "from B_SV_APPOINTED_Professional sap  "
                            + "\r\n" + "\t" + "left join b_sv_Record sr  on sr.uuid = sap.SV_RECORD_ID "
                            + "\r\n" + "\t" + "left join b_sv_validation sv on sv.SV_RECORD_ID = sr.UUID "
                            + "\r\n" + "\t" + "left join B_SV_SIGNBOARD ss on sr.SV_SIGNBOARD_ID = ss.UUID "
                            + "\r\n" + "\t" + "left join B_SV_ADDRESS addre on ss.LOCATION_ADDRESS_ID = addre.UUID "
                            ;

        string SearchUSR_q = ""
                    + "\r\n" + "\t" + " SELECT uuid, "
                    + "\r\n" + "\t" + " CASE "
                    + "\r\n" + "\t" + " WHEN RECORD_TYPE = 'WF_TYPE_VALIDATION' THEN 'Validation' "
                    + "\r\n" + "\t" + " WHEN RECORD_TYPE = 'WF_TYPE_GC' THEN 'Government Contractor' "
                    + "\r\n" + "\t" + " WHEN RECORD_TYPE = 'WF_TYPE_S24' THEN 'S24 Order'"
                    + "\r\n" + "\t" + " END AS RECORD_TYPE," + " REFERENCE_NO," + " FORM_CODE,"
                    + "\r\n" + "\t" + " TASK_USER_ID AS TASK_USER_ID," + " USER_ID,"
                    //+ "\r\n" + "\t" + " USERNAME,  RECORD_TYPE AS recordType, " + " TC, "
                    + "\r\n" + "\t" + " BD_PORTAL_LOGIN,  RECORD_TYPE AS recordType, " + " TC, "
                    + "\r\n" + "\t" + " ST,TO_CHAR(ST, 'hh24:mi')as TIME  FROM  ( "
                    + "\r\n" + "\t" + " SELECT wi.UUID, "
                    + "\r\n" + "\t" + " wi.RECORD_TYPE, sr.REFERENCE_NO, sr.FORM_CODE, "
                    + "\r\n" + "\t" + " tu.UUID AS TASK_USER_ID, u.UUID AS USER_ID, "
                    //+ "\r\n" + "\t" + " u.USERNAME, t.TASK_CODE AS TC, "
                    + "\r\n" + "\t" + " u.BD_PORTAL_LOGIN, t.TASK_CODE AS TC, "
                    + "\r\n" + "\t" + " tu.START_TIME AS ST "
                    + "\r\n" + "\t" + " FROM B_WF_INFO wi, B_SV_RECORD sr "
                    //+ "\r\n" + "\t" + " ,B_WF_TASK t, B_WF_TASK_USER tu, B_S_USER_ACCOUNT u "
                    + "\r\n" + "\t" + " ,B_WF_TASK t, B_WF_TASK_USER tu, sys_post u "
                    + "\r\n" + "\t" + " WHERE wi.RECORD_ID = sr.UUID "
                    + "\r\n" + "\t" + " AND tu.STATUS = '" + SignboardConstant.WF_STATUS_OPEN + "' "
                    + "\r\n" + "\t" + " AND wi.UUID = t.WF_INFO_ID "
                    + "\r\n" + "\t" + " AND t.UUID = tu.WF_TASK_ID "
                    //+ "\r\n" + "\t" + " AND tu.USER_ID = u.UUID "
                    + "\r\n" + "\t" + " AND tu.SYS_POST_ID = u.UUID "
                    + "\r\n" + "\t" + " UNION ALL "
                    + "\r\n" + "\t" + " SELECT wi.UUID, wi.RECORD_TYPE, "
                    + "\r\n" + "\t" + " sg.GC_NUMBER, '-' AS FORM_CODE,  tu.UUID AS TASK_USER_ID, "
                    //+ "\r\n" + "\t" + " u.UUID AS USER_ID, u.USERNAME, t.TASK_CODE, "
                    + "\r\n" + "\t" + " u.UUID AS USER_ID, u.BD_PORTAL_LOGIN, t.TASK_CODE, "
                    + "\r\n" + "\t" + " tu.START_TIME  "
                    + "\r\n" + "\t" + " FROM B_WF_INFO wi, B_SV_GC sg"
                    //+ "\r\n" + "\t" + " ,B_WF_TASK t, B_WF_TASK_USER tu, B_S_USER_ACCOUNT u "
                    + "\r\n" + "\t" + " ,B_WF_TASK t, B_WF_TASK_USER tu, sys_post u "
                    + "\r\n" + "\t" + " WHERE wi.RECORD_ID = sg.UUID "
                    + "\r\n" + "\t" + " AND tu.STATUS = '" + SignboardConstant.WF_STATUS_OPEN + "' "
                    + "\r\n" + "\t" + " AND wi.UUID = t.WF_INFO_ID "
                    + "\r\n" + "\t" + " AND t.UUID = tu.WF_TASK_ID "
                    //+ "\r\n" + "\t" + " AND tu.USER_ID = u.UUID "
                    + "\r\n" + "\t" + " AND tu.SYS_POST_ID = u.UUID "
                    + "\r\n" + "\t" + " UNION ALL "
                    + "\r\n" + "\t" + " SELECT wi.UUID, "
                    + "\r\n" + "\t" + " wi.RECORD_TYPE, "
                    + "\r\n" + "\t" + " s24.ORDER_NO,  '-' AS FORM_CODE, tu.UUID AS TASK_USER_ID, "
                    //+ "\r\n" + "\t" + " u.UUID AS USER_ID,  u.USERNAME,  t.TASK_CODE, "
                    + "\r\n" + "\t" + " u.UUID AS USER_ID,  u.BD_PORTAL_LOGIN,  t.TASK_CODE, "
                    + "\r\n" + "\t" + " tu.START_TIME "
                    + "\r\n" + "\t" + " FROM B_WF_INFO wi, B_SV_24_ORDER s24 "
                    //+ "\r\n" + "\t" + " ,B_WF_TASK t, B_WF_TASK_USER tu, B_S_USER_ACCOUNT u "
                    + "\r\n" + "\t" + " ,B_WF_TASK t, B_WF_TASK_USER tu, sys_post u "
                    + "\r\n" + "\t" + " WHERE wi.RECORD_ID = s24.UUID "
                    + "\r\n" + "\t" + " AND tu.STATUS = '" + SignboardConstant.WF_STATUS_OPEN + "' "
                    + "\r\n" + "\t" + " AND wi.UUID = t.WF_INFO_ID "
                    + "\r\n" + "\t" + " AND t.UUID = tu.WF_TASK_ID "
                    //+ "\r\n" + "\t" + " AND tu.USER_ID = u.UUID "
                    + "\r\n" + "\t" + " AND tu.SYS_POST_ID = u.UUID "
                    + "\r\n" + "\t" + " ) WHERE 1=1 ";

        string SearchCUR_SD_q = ""
                    + "\r\n" + "\t" + " SELECT reference_no as REF_NO, '' as remarks FROM ("
                    + "\r\n" + "\t" + " SELECT distinct reference_no from b_sv_submission"
                    + "\r\n" + "\t" + " UNION ALL"
                    + "\r\n" + "\t" + " SELECT distinct gc_number from b_sv_gc"
                    + "\r\n" + "\t" + " UNION ALL"
                    + "\r\n" + "\t" + " SELECT distinct order_no from b_sv_24_order"
                    + "\r\n" + "\t" + " UNION ALL"
                    + "\r\n" + "\t" + " SELECT distinct complain_number from b_sv_complain"
                    + "\r\n" + "\t" + " UNION ALL"
                    + "\r\n" + "\t" + " SELECT distinct new_item_number from b_sv_signboard)";

        string SearchMR_q = ""
                    + "\r\n" + "\t" + " select count(*) from sv_record svr ";

        public Fn04RPT_PPJLSearchModel SearchPPJL(Fn04RPT_PPJLSearchModel model)
        {
            model.Query = SearchPPJL_q;

            model.QueryWhere = SearchPPJL_whereQ(model);
            model.Search();
            for (int i = 0; i < model.Data.Count(); i++)
            {
                var STATUS = DateUtil.getWFStatusDisplay(model.Data[i]["WF_STATUS"].ToString());
                model.Data[i]["WF_STATUS"] = STATUS;
                var VR = DateUtil.getValidationResultDisplay(model.Data[i]["VALIDATION_RESULT"].ToString());
                model.Data[i]["VALIDATION_RESULT"] = VR;
            }
            return model;
        }
        private string SearchPPJL_whereQ(Fn04RPT_PPJLSearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.searchPbpRegNo)
                    || !string.IsNullOrWhiteSpace(model.searchPbpEnglishName)
                    || !string.IsNullOrWhiteSpace(model.searchPbpChineseName))
            {
                whereQ += "\r\n\t" + "where 1=1";
            }
            if (!string.IsNullOrWhiteSpace(model.searchPbpChineseName))
            {
                //sqlStr = sqlStr
                //        + "and lower(sap.chineseName) like :pbpChineseName ";
                whereQ += "\r\n\t" + "and sap.CHINESE_NAME like :pbpChineseName";
                model.QueryParameters.Add("pbpChineseName", "%" + model.searchPbpChineseName.ToUpper().Trim() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.searchPbpEnglishName))
            {
                //sqlStr = sqlStr
                //        + "and lower(sap.englishName) like :pbpEnglishName ";
                whereQ += "\r\n\t" + "and UPPER(sap.ENGLISH_NAME) like :pbpEnglishName";
                model.QueryParameters.Add("pbpEnglishName", "%" + model.searchPbpEnglishName.ToUpper().Trim() + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.searchPbpRegNo))
            {
                //sqlStr = sqlStr + "and lower(sap.certificationNo) like :pbpRegNo ";
                whereQ += "\r\n\t" + "and UPPER(sap.CERTIFICATION_NO) like :pbpRegNo";
                model.QueryParameters.Add("pbpRegNo", "%" + model.searchPbpRegNo.ToUpper().Trim() + "%");
            }
            //whereQ += "\r\n\t" + " order by sap.CERTIFICATION_NO ";
            return whereQ;
        }
        public string ExportPPJL(Fn04RPT_PPJLSearchModel model)
        {
            model.Query = SearchPPJL_q;
            model.QueryWhere = SearchPPJL_whereQ(model);
            return model.Export("PBP and PRC Job List");
        }

        public Fn04RPT_UPSRSearchModel SearchUPSR(Fn04RPT_UPSRSearchModel model)
        {
            model.Query = SearchUSR_q;

            model.QueryWhere = SearchUSR_whereQ(model);
            model.Sort = model.SearchSortBy;
            model.Search();

            return model;
        }
        private string SearchUSR_whereQ(Fn04RPT_UPSRSearchModel model)
        {
            string whereQ = "";
            string FileRef = "";
            if (model.SearchFileRefNo is null)
            {
                FileRef = "";
            }
            else
            {
                FileRef = model.SearchFileRefNo;
            }
            whereQ += "\r\n\t" + "And lower(reference_no) LIKE :REFERENCE_NO";
            model.QueryParameters.Add("REFERENCE_NO", "%" + FileRef.ToLower() + "%");
            //whereQ += "\r\n\t" + " order by :sortBy ";
            //model.QueryParameters.Add("sortBy", model.SearchSortBy);

            return whereQ;
        }
        public string ExportUPSR(Fn04RPT_UPSRSearchModel model)
        {
            model.Query = SearchUSR_q;
            model.QueryWhere = SearchUSR_whereQ(model);
            
            return model.Export("Un-process Submission Report");
        }
        public Fn04RPT_MRSearchModel IndexMR(Fn04RPT_MRSearchModel model)
        {
            //string currentMonth2 = DateTimeFormatInfo.CurrentInfo.GetMonthName(DateTime.Now.Month).Substring(0, 3);

            string currentMonth = DateTime.Now.ToString("MMM");
            string currentYear = DateTime.Now.Year.ToString();
            DateTime today = DateTime.Now;
            DateTime BefY = today.AddYears(-1);
            int BefM = 12;
            int BefD = DateTime.DaysInMonth(BefY.Year, BefM);
            model.PeriodDateFrom = new DateTime(BefY.Year, BefM, BefD);
            model.PeriodDateTo = DateTime.Now;

            return new Fn04RPT_MRSearchModel()
            {
                PeriodDateFrom = model.PeriodDateFrom,
                PeriodDateTo = model.PeriodDateTo,
                Month = currentMonth,
                Year = currentYear
            };
        }
        public Fn04RPT_MSSearchModel IndexMS(Fn04RPT_MSSearchModel model)
        {
            DateTime today = DateTime.Now;
            DateTime BefY = today.AddYears(-1);
            int BefM = 12;
            int BefD = DateTime.DaysInMonth(BefY.Year, BefM);
            model.PeriodDateFrom = new DateTime(BefY.Year, BefM, BefD);
            model.PeriodDateTo = DateTime.Now;

            return new Fn04RPT_MSSearchModel()
            {
                PeriodDateFrom = model.PeriodDateFrom,
                PeriodDateTo = model.PeriodDateTo
            };
        }
        public Fn04RPT_MSSearchModel ViewMSRecord(Fn04RPT_MSSearchModel model)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                DateTime FromDate = model.PeriodDateFrom;
                DateTime ToDate = model.PeriodDateTo;

                var tempMonthList = DateUtil.getMonthList(FromDate, ToDate);

                List<DateTime> dateList = DateUtil.getMonthList(FromDate, ToDate);

                for (int i = 0; i < dateList.Count(); i++)
                {
                    DateTime startDate = dateList.ElementAt(i);
                    DateTime endDate = startDate.AddMonths(1);

                    var classIAcknowledgedList = (from svr in db.B_SV_RECORD
                                                  join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                  join svri in db.B_SV_RECORD_ITEM on svr.UUID equals svri.SV_RECORD_ID
                                                  where svri.MW_ITEM_CODE.Contains("1.")
                                                  && svv.VALIDATION_RESULT == SignboardConstant.VALIDATION_RESULT_ACKNOWLEDGED
                                                  && svv.SPO_ENDORSEMENT_DATE != null
                                                  && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                  select svr.UUID).Distinct().Count();
                    model.ClassIAcknowledgedList.Add(classIAcknowledgedList.ToString());
                    var classIRefusedList = (from svr in db.B_SV_RECORD
                                             join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                             join svri in db.B_SV_RECORD_ITEM on svr.UUID equals svri.SV_RECORD_ID
                                             where svri.MW_ITEM_CODE.Contains("1.")
                                             && svv.VALIDATION_RESULT == SignboardConstant.VALIDATION_RESULT_REFUSED
                                             && svv.SPO_ENDORSEMENT_DATE != null
                                             && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                             select svr.UUID).Distinct().Count();
                    model.ClassIRefusedList.Add(classIRefusedList.ToString());
                    var classIConditionalList = (from svr in db.B_SV_RECORD
                                                 join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                 join svri in db.B_SV_RECORD_ITEM on svr.UUID equals svri.SV_RECORD_ID
                                                 where svri.MW_ITEM_CODE.Contains("1.")
                                                 && svv.VALIDATION_RESULT == SignboardConstant.VALIDATION_RESULT_CONDITIONAL
                                                 && svv.SPO_ENDORSEMENT_DATE != null
                                                 && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                 select svr.UUID).Distinct().Count();
                    model.ClassIConditionalList.Add(classIConditionalList.ToString());
                    var classIIAcknowledgedList = (from svr in db.B_SV_RECORD
                                                   join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                   join svri in db.B_SV_RECORD_ITEM on svr.UUID equals svri.SV_RECORD_ID
                                                   where svri.MW_ITEM_CODE.Contains("2.")
                                                   && svv.VALIDATION_RESULT == SignboardConstant.VALIDATION_RESULT_ACKNOWLEDGED
                                                   && svv.SPO_ENDORSEMENT_DATE != null
                                                   && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                   select svr.UUID).Distinct().Count();
                    model.ClassIIAcknowledgedList.Add(classIIAcknowledgedList.ToString());
                    var classIIRefusedList = (from svr in db.B_SV_RECORD
                                              join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                              join svri in db.B_SV_RECORD_ITEM on svr.UUID equals svri.SV_RECORD_ID
                                              where svri.MW_ITEM_CODE.Contains("2.")
                                              && svv.VALIDATION_RESULT == SignboardConstant.VALIDATION_RESULT_REFUSED
                                              && svv.SPO_ENDORSEMENT_DATE != null
                                              && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                              select svr.UUID).Distinct().Count();
                    model.ClassIIRefusedList.Add(classIIRefusedList.ToString());
                    var classIIConditionalList = (from svr in db.B_SV_RECORD
                                                  join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                  join svri in db.B_SV_RECORD_ITEM on svr.UUID equals svri.SV_RECORD_ID
                                                  where svri.MW_ITEM_CODE.Contains("2.")
                                                  && svv.VALIDATION_RESULT == SignboardConstant.VALIDATION_RESULT_CONDITIONAL
                                                  && svv.SPO_ENDORSEMENT_DATE != null
                                                  && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                  select svr.UUID).Distinct().Count();
                    model.ClassIIConditionalList.Add(classIIConditionalList.ToString());
                    var classIIIAcknowledgedList = (from svr in db.B_SV_RECORD
                                                    join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                    join svri in db.B_SV_RECORD_ITEM on svr.UUID equals svri.SV_RECORD_ID
                                                    where svri.MW_ITEM_CODE.Contains("3.")
                                                    && svv.VALIDATION_RESULT == SignboardConstant.VALIDATION_RESULT_ACKNOWLEDGED
                                                    && svv.SPO_ENDORSEMENT_DATE != null
                                                    && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                    select svr.UUID).Distinct().Count();
                    model.ClassIIIAcknowledgedList.Add(classIIIAcknowledgedList.ToString());
                    var classIIIRefusedList = (from svr in db.B_SV_RECORD
                                               join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                               join svri in db.B_SV_RECORD_ITEM on svr.UUID equals svri.SV_RECORD_ID
                                               where svri.MW_ITEM_CODE.Contains("3.")
                                               && svv.VALIDATION_RESULT == SignboardConstant.VALIDATION_RESULT_REFUSED
                                               && svv.SPO_ENDORSEMENT_DATE != null
                                               && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                               select svr.UUID).Distinct().Count();
                    model.ClassIIIRefusedList.Add(classIIIRefusedList.ToString());
                    var classIIIConditionalList = (from svr in db.B_SV_RECORD
                                                   join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                   join svri in db.B_SV_RECORD_ITEM on svr.UUID equals svri.SV_RECORD_ID
                                                   where svri.MW_ITEM_CODE.Contains("3.")
                                                   && svv.VALIDATION_RESULT == SignboardConstant.VALIDATION_RESULT_CONDITIONAL
                                                   && svv.SPO_ENDORSEMENT_DATE != null
                                                   && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                   select svr.UUID).Distinct().Count();
                    model.ClassIIIConditionalList.Add(classIIIConditionalList.ToString());
                    var classIAuditList = (from svr in db.B_SV_RECORD
                                           join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                           join svri in db.B_SV_RECORD_ITEM on svr.UUID equals svri.SV_RECORD_ID
                                           where svri.MW_ITEM_CODE.Contains("1.")
                                           && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                           select svr.UUID).Distinct().Count();
                    model.ClassIAuditList.Add(classIAuditList.ToString());
                    var classIIAuditList = (from svr in db.B_SV_RECORD
                                            join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                            join svri in db.B_SV_RECORD_ITEM on svr.UUID equals svri.SV_RECORD_ID
                                            where svri.MW_ITEM_CODE.Contains("2.")
                                            && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                            select svr.UUID).Distinct().Count();
                    model.ClassIIAuditList.Add(classIIAuditList.ToString());
                    var classIIIAuditList = (from svr in db.B_SV_RECORD
                                             join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                             join svri in db.B_SV_RECORD_ITEM on svr.UUID equals svri.SV_RECORD_ID
                                             where svri.MW_ITEM_CODE.Contains("3.")
                                             && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                             select svr.UUID).Distinct().Count();
                    model.ClassIIIAuditList.Add(classIIIAuditList.ToString());
                    var gcList = (from svgc in db.B_SV_GC
                                  where svgc.RECEIVED_DATE >= startDate && svgc.RECEIVED_DATE <= endDate
                                  select svgc.UUID).Distinct().Count();
                    model.GcList.Add(gcList.ToString());
                    //var s24List = (from sv24o in db.B_SV_24_ORDER
                    //               where sv24o.RECEIVED_DATE >= startDate && sv24o.RECEIVED_DATE <= endDate
                    //               select sv24o.UUID).Distinct().Count();
                    //model.S24List.Add(s24List.ToString());
                    var s24NotYetIssList = (from s24 in db.B_SV_24_ORDER
                                            where s24.RECEIVED_DATE >= startDate && s24.RECEIVED_DATE <= endDate
                                            && s24.STATUS_CODE == SignboardConstant.S24_STATUS_NOT_YET_ISSUED
                                            select s24.UUID).Distinct().Count();
                    model.S24NotYetIssList.Add(s24NotYetIssList.ToString());
                    var s24NotYetExpList = (from s24 in db.B_SV_24_ORDER
                                            where s24.RECEIVED_DATE >= startDate && s24.RECEIVED_DATE <= endDate
                                            && s24.STATUS_CODE == SignboardConstant.S24_STATUS_NOT_YET_EXPIRED
                                            select s24.UUID).Distinct().Count();
                    model.S24NotYetExpList.Add(s24NotYetExpList.ToString());
                    var s24AppealLodgedList = (from s24 in db.B_SV_24_ORDER
                                               where s24.RECEIVED_DATE >= startDate && s24.RECEIVED_DATE <= endDate
                                               && s24.STATUS_CODE == SignboardConstant.S24_STATUS_APPEAL_LODGED
                                               select s24.UUID).Distinct().Count();
                    model.S24AppealLodgedList.Add(s24AppealLodgedList.ToString());
                    var s24ProsecutionToBeList = (from s24 in db.B_SV_24_ORDER
                                                  where s24.RECEIVED_DATE >= startDate && s24.RECEIVED_DATE <= endDate
                                                  && s24.STATUS_CODE == SignboardConstant.S24_STATUS_PROSECUTION_TO_BE_INITIATED
                                                  select s24.UUID).Distinct().Count();
                    model.S24ProsecutionToBeList.Add(s24ProsecutionToBeList.ToString());
                    var s24ProList = (from s24 in db.B_SV_24_ORDER
                                      where s24.RECEIVED_DATE >= startDate && s24.RECEIVED_DATE <= endDate
                                      && s24.STATUS_CODE == SignboardConstant.S24_STATUS_PROSECUTION_INITIATED
                                      select s24.UUID).Distinct().Count();
                    model.S24ProList.Add(s24ProList.ToString());
                    var s24GcaList = (from s24 in db.B_SV_24_ORDER
                                      where s24.RECEIVED_DATE >= startDate && s24.RECEIVED_DATE <= endDate
                                      && s24.STATUS_CODE == SignboardConstant.S24_STATUS_GC_ACTION
                                      select s24.UUID).Distinct().Count();
                    model.S24GcaList.Add(s24GcaList.ToString());
                    var s24PendingList = (from s24 in db.B_SV_24_ORDER
                                          where s24.RECEIVED_DATE >= startDate && s24.RECEIVED_DATE <= endDate
                                          && s24.STATUS_CODE == SignboardConstant.S24_STATUS_PENDING_FOR_COMPLIANCE
                                          select s24.UUID).Distinct().Count();
                    model.S24PendingList.Add(s24PendingList.ToString());
                    var s24ComList = (from s24 in db.B_SV_24_ORDER
                                      where s24.RECEIVED_DATE >= startDate && s24.RECEIVED_DATE <= endDate
                                      && s24.STATUS_CODE == SignboardConstant.S24_STATUS_COMPLIED_WITH
                                      select s24.UUID).Distinct().Count();
                    model.S24ComList.Add(s24ComList.ToString());
                    var s24WithdrawnList = (from s24 in db.B_SV_24_ORDER
                                            where s24.RECEIVED_DATE >= startDate && s24.RECEIVED_DATE <= endDate
                                            && s24.STATUS_CODE == SignboardConstant.S24_STATUS_WITHDRAWN
                                            select s24.UUID).Distinct().Count();
                    model.S24WithdrawnList.Add(s24WithdrawnList.ToString());
                    var reportToScuList = (from svc in db.B_SV_COMPLAIN
                                           where svc.COMPLAINT_DATE >= startDate && svc.COMPLAINT_DATE <= endDate
                                           && svc.RECORD_TYPE == SignboardConstant.COMPLAIN
                                           select svc.UUID).Distinct().Count();
                    model.ReportToScuList.Add(reportToScuList.ToString());
                    var reportToScuReceivedList = (from svc in db.B_SV_COMPLAIN
                                                   where svc.COMPLAINT_DATE >= startDate && svc.COMPLAINT_DATE <= endDate
                                                   && svc.RECORD_TYPE == SignboardConstant.COMPLAIN
                                                   && svc.STATUS == SignboardConstant.COMPLAIN_STATUS_OPEN
                                                   select svc.UUID).Distinct().Count();
                    model.ReportToScuReceivedList.Add(reportToScuReceivedList.ToString());
                    var reportToScuCloseList = (from svc in db.B_SV_COMPLAIN
                                                where svc.COMPLAINT_DATE >= startDate && svc.COMPLAINT_DATE <= endDate
                                                && svc.RECORD_TYPE == SignboardConstant.COMPLAIN
                                                && svc.STATUS == SignboardConstant.COMPLAIN_STATUS_CLOSE
                                                select svc.UUID).Distinct().Count();
                    model.ReportToScuCloseList.Add(reportToScuCloseList.ToString());

                    var enquiryToScuList = (from svc in db.B_SV_COMPLAIN
                                            where svc.COMPLAINT_DATE >= startDate && svc.COMPLAINT_DATE <= endDate
                                            && svc.RECORD_TYPE == SignboardConstant.ENQUIRY
                                            select svc.UUID).Distinct().Count();
                    model.EnquiryToScuList.Add(enquiryToScuList.ToString());
                    var enquiryToScuReceivedList = (from svc in db.B_SV_COMPLAIN
                                                    where svc.COMPLAINT_DATE >= startDate && svc.COMPLAINT_DATE <= endDate
                                                    && svc.RECORD_TYPE == SignboardConstant.ENQUIRY
                                                    && svc.STATUS == SignboardConstant.COMPLAIN_STATUS_OPEN
                                                    select svc.UUID).Distinct().Count();
                    model.EnquiryToScuReceivedList.Add(enquiryToScuReceivedList.ToString());
                    var enquiryToScuCloseList = (from svc in db.B_SV_COMPLAIN
                                                 where svc.COMPLAINT_DATE >= startDate && svc.COMPLAINT_DATE <= endDate
                                                 && svc.RECORD_TYPE == SignboardConstant.ENQUIRY
                                                 && svc.STATUS == SignboardConstant.COMPLAIN_STATUS_CLOSE
                                                 select svc.UUID).Distinct().Count();
                    model.EnquiryToScuCloseList.Add(enquiryToScuCloseList.ToString());
                }
                return new Fn04RPT_MSSearchModel()
                {
                    MonthList = tempMonthList,
                    ClassIAcknowledgedList = model.ClassIAcknowledgedList,
                    ClassIRefusedList = model.ClassIRefusedList,
                    ClassIIAcknowledgedList = model.ClassIIAcknowledgedList,
                    ClassIIRefusedList = model.ClassIIRefusedList,
                    ClassIIIAcknowledgedList = model.ClassIIIAcknowledgedList,
                    ClassIIIRefusedList = model.ClassIIIRefusedList,
                    ClassIConditionalList = model.ClassIConditionalList,
                    ClassIIConditionalList = model.ClassIIConditionalList,
                    ClassIIIConditionalList = model.ClassIIIConditionalList,
                    ClassIAuditList = model.ClassIAuditList,
                    ClassIIAuditList = model.ClassIIIAuditList,
                    ClassIIIAuditList = model.ClassIIAuditList,
                    GcList = model.GcList,
                    S24List = model.S24List,
                    S24IssList = model.S24IssList,
                    S24ProList = model.S24ProList,
                    S24GcaList = model.S24GcaList,
                    S24ComList = model.S24ComList,
                    S24NotYetIssList = model.S24NotYetIssList,
                    S24NotYetExpList = model.S24NotYetExpList,
                    S24AppealLodgedList = model.S24AppealLodgedList,
                    S24WithdrawnList = model.S24WithdrawnList,
                    S24ProsecutionToBeList = model.S24ProsecutionToBeList,
                    S24PendingList = model.S24PendingList,
                    ReportToScuList = model.ReportToScuList,
                    ReportToScuReceivedList = model.ReportToScuReceivedList,
                    ReportToScuCloseList = model.ReportToScuCloseList,
                    EnquiryToScuList = model.EnquiryToScuList,
                    EnquiryToScuReceivedList = model.EnquiryToScuReceivedList,
                    EnquiryToScuCloseList = model.EnquiryToScuCloseList
                };
            }
        }

        public Fn04RPT_MRSearchModel ViewMRRecord(Fn04RPT_MRSearchModel model)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {

                DateTime FromDate = new DateTime();
                DateTime ToDate = new DateTime();

                if (model.TypeOfCalendar == "Period")
                {
                    if (model.PeriodDateFrom != null)
                    {
                        FromDate = model.PeriodDateFrom;
                    }
                    if (model.PeriodDateTo != null)
                    {
                        ToDate = model.PeriodDateTo;
                    }
                }
                else
                {
                    int year = int.Parse(model.Year);
                    int month = int.Parse(model.Month);
                    int date = 1;
                    FromDate = new DateTime(year, month, date);
                    ToDate = new DateTime(year, month, date);
                }

                //List<DateTime> dateList = DateUtil.getDateList(FromDate, ToDate);
                List<DateTime> dateList = DateUtil.getMonthList(FromDate, ToDate);
                var tempMonthList = DateUtil.getMonthList(FromDate, ToDate);
                //for (int i = 0; i < dateList.Count(); i=i+2)
                for (int i = 0; i < dateList.Count(); i++)
                {
                    DateTime startDate = dateList.ElementAt(i);
                    DateTime endDate = startDate.AddMonths(1);

                    //DateTime endDates = dateList.ElementAt(i+);
                    //DateTime endDate = DateUtil.getDateLastMoment(dateList.ElementAt(i + 1));

                    #region SC01
                    var receivedRecordCount = (from svr in db.B_SV_RECORD
                                               where svr.FORM_CODE == SignboardConstant.FORM_CODE_SC01
                                               && svr.RECEIVED_DATE >= startDate
                                               && svr.RECEIVED_DATE <= endDate
                                               select svr).Distinct().Count();
                    model.ReceivedRecordCountList.Add(receivedRecordCount.ToString());

                    var processingRecordCount = (from svr in db.B_SV_RECORD
                                                 join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                 where svv.VALIDATION_RESULT == null
                                                 && svr.FORM_CODE == SignboardConstant.FORM_CODE_SC01
                                                 && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                 select svr).Distinct().Count();
                    model.ProcessingRecordCountList.Add(processingRecordCount.ToString());

                    var acknowledgedRecordCount = (from svr in db.B_SV_RECORD
                                                   join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                   where svv.VALIDATION_RESULT == SignboardConstant.FORM_CODE_SC01
                                                   && svv.SPO_ENDORSEMENT_DATE != null
                                                   && svr.FORM_CODE == SignboardConstant.VALIDATION_RESULT_ACKNOWLEDGED
                                                   && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                   select svr).Distinct().Count();
                    model.AcknowledgedRecordCountList.Add(acknowledgedRecordCount.ToString());

                    var refusedRecordCount = (from svr in db.B_SV_RECORD
                                              join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                              where svv.VALIDATION_RESULT == SignboardConstant.FORM_CODE_SC01
                                              && svv.SPO_ENDORSEMENT_DATE != null
                                              && svr.FORM_CODE == SignboardConstant.VALIDATION_RESULT_REFUSED
                                              && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                              select svr).Distinct().Count();
                    model.RefusedRecordCountList.Add(refusedRecordCount.ToString());

                    var conditionalRecordCount = (from svr in db.B_SV_RECORD
                                                  join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                  where svv.VALIDATION_RESULT == SignboardConstant.FORM_CODE_SC01
                                                  && svv.SPO_ENDORSEMENT_DATE != null
                                                  && svr.FORM_CODE == SignboardConstant.VALIDATION_RESULT_CONDITIONAL
                                                  && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                  select svr).Count();
                    model.ConditionalRecordCountList.Add(conditionalRecordCount.ToString());

                    var auditRecordCount = (from svr in db.B_SV_RECORD
                                            join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                            where svr.FORM_CODE == SignboardConstant.FORM_CODE_SC01
                                            && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                            select svr).Distinct().Count();
                    model.AuditRecordCountList.Add(auditRecordCount.ToString());
                    #endregion


                    #region SC01C
                    var receivedRecordCount01C = (from svr in db.B_SV_RECORD
                                               where svr.FORM_CODE == SignboardConstant.FORM_CODE_SC01C
                                               && svr.RECEIVED_DATE >= startDate
                                               && svr.RECEIVED_DATE <= endDate
                                               select svr).Distinct().Count();
                    model.ReceivedRecordCountList01C.Add(receivedRecordCount01C.ToString());

                    var processingRecordCount01C = (from svr in db.B_SV_RECORD
                                                 join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                 where svv.VALIDATION_RESULT == null
                                                 && svr.FORM_CODE == SignboardConstant.FORM_CODE_SC01C
                                                 && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                 select svr).Distinct().Count();
                    model.ProcessingRecordCountList01C.Add(processingRecordCount01C.ToString());

                    var acknowledgedRecordCount01C = (from svr in db.B_SV_RECORD
                                                   join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                   where svv.VALIDATION_RESULT == SignboardConstant.FORM_CODE_SC01C
                                                   && svv.SPO_ENDORSEMENT_DATE != null
                                                   && svr.FORM_CODE == SignboardConstant.VALIDATION_RESULT_ACKNOWLEDGED
                                                   && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                   select svr).Distinct().Count();
                    model.AcknowledgedRecordCountList01C.Add(acknowledgedRecordCount01C.ToString());

                    var refusedRecordCount01C = (from svr in db.B_SV_RECORD
                                              join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                              where svv.VALIDATION_RESULT == SignboardConstant.FORM_CODE_SC01C
                                              && svv.SPO_ENDORSEMENT_DATE != null
                                              && svr.FORM_CODE == SignboardConstant.VALIDATION_RESULT_REFUSED
                                              && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                              select svr).Distinct().Count();
                    model.RefusedRecordCountList01C.Add(refusedRecordCount01C.ToString());

                    var conditionalRecordCount01C = (from svr in db.B_SV_RECORD
                                                  join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                  where svv.VALIDATION_RESULT == SignboardConstant.FORM_CODE_SC01C
                                                  && svv.SPO_ENDORSEMENT_DATE != null
                                                  && svr.FORM_CODE == SignboardConstant.VALIDATION_RESULT_CONDITIONAL
                                                  && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                  select svr).Count();
                    model.ConditionalRecordCountList01C.Add(conditionalRecordCount01C.ToString());

                    var auditRecordCount01C = (from svr in db.B_SV_RECORD
                                            join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                            where svr.FORM_CODE == SignboardConstant.FORM_CODE_SC01C
                                            && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                            select svr).Distinct().Count();
                    model.AuditRecordCountList01C.Add(auditRecordCount01C.ToString());
                    #endregion

                    #region SC02
                    var receivedRecordCount02 = (from sv in db.B_SV_RECORD
                                                 where sv.FORM_CODE == SignboardConstant.FORM_CODE_SC02
                                                 && sv.RECEIVED_DATE >= startDate && sv.RECEIVED_DATE <= endDate
                                                 select sv).Distinct().Count();
                    model.ReceivedRecordCountListSC02.Add(receivedRecordCount02.ToString());
                    var processingRecordCount02 = (from svr in db.B_SV_RECORD
                                                   join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                   where svv.VALIDATION_RESULT == null
                                                   && svr.FORM_CODE == SignboardConstant.FORM_CODE_SC02
                                                   && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                   select svr).Distinct().Count();
                    model.ProcessingRecordCountListSC02.Add(processingRecordCount02.ToString());

                    var acknowledgedRecordCount02 = (from svr in db.B_SV_RECORD
                                                     join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                     where svv.VALIDATION_RESULT == SignboardConstant.FORM_CODE_SC02
                                                     && svv.SPO_ENDORSEMENT_DATE != null
                                                     && svr.FORM_CODE == SignboardConstant.VALIDATION_RESULT_ACKNOWLEDGED
                                                     && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                     select svr).Distinct().Count();

                    model.AcknowledgedRecordCountListSC02.Add(acknowledgedRecordCount02.ToString());

                    var refusedRecordCountList02 = (from svr in db.B_SV_RECORD
                                                    join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                    where svv.VALIDATION_RESULT == SignboardConstant.FORM_CODE_SC02
                                                    && svv.SPO_ENDORSEMENT_DATE != null
                                                    && svr.FORM_CODE == SignboardConstant.VALIDATION_RESULT_REFUSED
                                                    && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                    select svr).Distinct().Count();
                    model.RefusedRecordCountListSC02.Add(refusedRecordCountList02.ToString());

                    var conditionalRecordCountList02 = (from svr in db.B_SV_RECORD
                                                        join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                        where svv.VALIDATION_RESULT == SignboardConstant.FORM_CODE_SC02
                                                        && svv.SPO_ENDORSEMENT_DATE != null
                                                        && svr.FORM_CODE == SignboardConstant.VALIDATION_RESULT_CONDITIONAL
                                                        && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                        select svr).Distinct().Count();
                    model.ConditionalRecordCountListSC02.Add(conditionalRecordCountList02.ToString());

                    var auditRecordCount02 = (from svr in db.B_SV_RECORD
                                              join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                              where svr.FORM_CODE == SignboardConstant.FORM_CODE_SC02
                                              && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                              select svr).Distinct().Count();
                    model.AuditRecordCountListSC02.Add(auditRecordCount02.ToString());

                    #endregion


                    #region SC02C
                    var receivedRecordCount02C = (from sv in db.B_SV_RECORD
                                                 where sv.FORM_CODE == SignboardConstant.FORM_CODE_SC02C
                                                 && sv.RECEIVED_DATE >= startDate && sv.RECEIVED_DATE <= endDate
                                                 select sv).Distinct().Count();
                    model.ReceivedRecordCountListSC02C.Add(receivedRecordCount02C.ToString());
                    var processingRecordCount02C = (from svr in db.B_SV_RECORD
                                                   join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                   where svv.VALIDATION_RESULT == null
                                                   && svr.FORM_CODE == SignboardConstant.FORM_CODE_SC02C
                                                   && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                   select svr).Distinct().Count();
                    model.ProcessingRecordCountListSC02C.Add(processingRecordCount02C.ToString());

                    var acknowledgedRecordCount02C = (from svr in db.B_SV_RECORD
                                                     join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                     where svv.VALIDATION_RESULT == SignboardConstant.FORM_CODE_SC02C
                                                     && svv.SPO_ENDORSEMENT_DATE != null
                                                     && svr.FORM_CODE == SignboardConstant.VALIDATION_RESULT_ACKNOWLEDGED
                                                     && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                     select svr).Distinct().Count();

                    model.AcknowledgedRecordCountListSC02C.Add(acknowledgedRecordCount02C.ToString());

                    var refusedRecordCountList02C = (from svr in db.B_SV_RECORD
                                                    join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                    where svv.VALIDATION_RESULT == SignboardConstant.FORM_CODE_SC02C
                                                    && svv.SPO_ENDORSEMENT_DATE != null
                                                    && svr.FORM_CODE == SignboardConstant.VALIDATION_RESULT_REFUSED
                                                    && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                    select svr).Distinct().Count();
                    model.RefusedRecordCountListSC02C.Add(refusedRecordCountList02C.ToString());

                    var conditionalRecordCountList02C = (from svr in db.B_SV_RECORD
                                                        join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                        where svv.VALIDATION_RESULT == SignboardConstant.FORM_CODE_SC02C
                                                        && svv.SPO_ENDORSEMENT_DATE != null
                                                        && svr.FORM_CODE == SignboardConstant.VALIDATION_RESULT_CONDITIONAL
                                                        && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                        select svr).Distinct().Count();
                    model.ConditionalRecordCountListSC02C.Add(conditionalRecordCountList02C.ToString());

                    var auditRecordCount02C = (from svr in db.B_SV_RECORD
                                              join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                              where svr.FORM_CODE == SignboardConstant.FORM_CODE_SC02C
                                              && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                              select svr).Distinct().Count();
                    model.AuditRecordCountListSC02C.Add(auditRecordCount02C.ToString());

                    #endregion SC02C








                    #region SC03
                    var receivedRecordCount03 = (from sv in db.B_SV_RECORD
                                                 where sv.FORM_CODE == SignboardConstant.FORM_CODE_SC03
                                                 && sv.RECEIVED_DATE >= startDate && sv.RECEIVED_DATE <= endDate
                                                 select sv).Distinct().Count();
                    model.ReceivedRecordCountListSC03.Add(receivedRecordCount03.ToString());

                    var processingRecordCount03 = (from svr in db.B_SV_RECORD
                                                   join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                   where svv.VALIDATION_RESULT == null
                                                   && svr.FORM_CODE == SignboardConstant.FORM_CODE_SC03
                                                   && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                   select svr).Count();
                    model.ProcessingRecordCountListSC03.Add(processingRecordCount02.ToString());

                    var acknowledgedRecordCount03 = (from svr in db.B_SV_RECORD
                                                     join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                     where svv.VALIDATION_RESULT == SignboardConstant.FORM_CODE_SC03
                                                     && svv.SPO_ENDORSEMENT_DATE != null
                                                     && svr.FORM_CODE == SignboardConstant.VALIDATION_RESULT_ACKNOWLEDGED
                                                     && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                     select svr).Distinct().Count();

                    model.AcknowledgedRecordCountListSC03.Add(acknowledgedRecordCount03.ToString());

                    var refusedRecordCountList03 = (from svr in db.B_SV_RECORD
                                                    join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                    where svv.VALIDATION_RESULT == SignboardConstant.FORM_CODE_SC03
                                                    && svv.SPO_ENDORSEMENT_DATE != null
                                                    && svr.FORM_CODE == SignboardConstant.VALIDATION_RESULT_REFUSED
                                                    && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                    select svr).Distinct().Count();
                    model.RefusedRecordCountListSC03.Add(refusedRecordCountList03.ToString());

                    var conditionalRecordCountList03 = (from svr in db.B_SV_RECORD
                                                        join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                                        where svv.VALIDATION_RESULT == SignboardConstant.FORM_CODE_SC03
                                                        && svv.SPO_ENDORSEMENT_DATE != null
                                                        && svr.FORM_CODE == SignboardConstant.VALIDATION_RESULT_CONDITIONAL
                                                        && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                                        select svr).Distinct().Count();
                    model.ConditionalRecordCountListSC03.Add(conditionalRecordCountList03.ToString());

                    var auditRecordCount03 = (from svr in db.B_SV_RECORD
                                              join svv in db.B_SV_VALIDATION on svr.UUID equals svv.SV_RECORD_ID
                                              where svr.FORM_CODE == SignboardConstant.FORM_CODE_SC03
                                              && svr.RECEIVED_DATE >= startDate && svr.RECEIVED_DATE <= endDate
                                              select svr).Distinct().Distinct().Count();
                    model.AuditRecordCountListSC03.Add(auditRecordCount03.ToString());
                    #endregion SC03


                    var MonthlyStatisticGcCount = (from svgc in db.B_SV_GC
                                                   where svgc.RECEIVED_DATE >= startDate
                                                   && svgc.RECEIVED_DATE <= endDate
                                                   select svgc).Distinct().Count();
                    model.GcList.Add(MonthlyStatisticGcCount.ToString());

                    //var MonthlyStatisticS24Count = (from sv24 in db.B_SV_24_ORDER
                    //                                where sv24.RECEIVED_DATE >= startDate
                    //                                && sv24.RECEIVED_DATE <= endDate
                    //                                select sv24).Distinct().Count();
                    //model.S24List.Add(MonthlyStatisticS24Count.ToString());
                    //var MonthlyStatisticS24SubCount = (from sv24 in db.B_SV_24_ORDER
                    //                                   where sv24.RECEIVED_DATE >= startDate
                    //                                   && sv24.RECEIVED_DATE <= endDate
                    //                                   && sv24.STATUS_CODE == SignboardConstant.S24_STATUS_NOT_YET_ISSUED
                    //                                   select sv24).Distinct().Count();
                    //model.S24NotYetIssList.Add(MonthlyStatisticS24SubCount.ToString());
                    //var MonthlyStatisticS24SubCount2 = (from sv24 in db.B_SV_24_ORDER
                    //                                    where sv24.RECEIVED_DATE >= startDate
                    //                                    && sv24.RECEIVED_DATE <= endDate
                    //                                    && sv24.STATUS_CODE == SignboardConstant.S24_STATUS_NOT_YET_EXPIRED
                    //                                    select sv24).Distinct().Count();
                    //model.S24NotYetExpList.Add(MonthlyStatisticS24SubCount2.ToString());
                    //var MonthlyStatisticS24SubCount3 = (from sv24 in db.B_SV_24_ORDER
                    //                                    where sv24.RECEIVED_DATE >= startDate
                    //                                    && sv24.RECEIVED_DATE <= endDate
                    //                                    && sv24.STATUS_CODE == SignboardConstant.S24_STATUS_APPEAL_LODGED
                    //                                    select sv24).Distinct().Count();
                    //model.S24AppealLodgedList.Add(MonthlyStatisticS24SubCount3.ToString());
                    //var MonthlyStatisticS24SubCount4 = (from sv24 in db.B_SV_24_ORDER
                    //                                    where sv24.RECEIVED_DATE >= startDate
                    //                                    && sv24.RECEIVED_DATE <= endDate
                    //                                    && sv24.STATUS_CODE == SignboardConstant.S24_STATUS_PROSECUTION_TO_BE_INITIATED
                    //                                    select sv24).Distinct().Count();
                    //model.S24ProsecutionToBeList.Add(MonthlyStatisticS24SubCount4.ToString());
                    //var MonthlyStatisticS24SubCount5 = (from sv24 in db.B_SV_24_ORDER
                    //                                    where sv24.RECEIVED_DATE >= startDate
                    //                                    && sv24.RECEIVED_DATE <= endDate
                    //                                    && sv24.STATUS_CODE == SignboardConstant.S24_STATUS_PROSECUTION_INITIATED
                    //                                    select sv24).Distinct().Count();
                    //model.S24ProList.Add(MonthlyStatisticS24SubCount5.ToString());
                    //var MonthlyStatisticS24SubCount6 = (from sv24 in db.B_SV_24_ORDER
                    //                                    where sv24.RECEIVED_DATE >= startDate
                    //                                    && sv24.RECEIVED_DATE <= endDate
                    //                                    && sv24.STATUS_CODE == SignboardConstant.S24_STATUS_GC_ACTION
                    //                                    select sv24).Distinct().Count();
                    //model.S24GcaList.Add(MonthlyStatisticS24SubCount6.ToString());
                    //var MonthlyStatisticS24SubCount7 = (from sv24 in db.B_SV_24_ORDER
                    //                                    where sv24.RECEIVED_DATE >= startDate
                    //                                    && sv24.RECEIVED_DATE <= endDate
                    //                                    && sv24.STATUS_CODE == SignboardConstant.S24_STATUS_PENDING_FOR_COMPLIANCE
                    //                                    select sv24).Distinct().Count();
                    //model.S24PendingList.Add(MonthlyStatisticS24SubCount7.ToString());
                    //var MonthlyStatisticS24SubCount8 = (from sv24 in db.B_SV_24_ORDER
                    //                                    where sv24.RECEIVED_DATE >= startDate
                    //                                    && sv24.RECEIVED_DATE <= endDate
                    //                                    && sv24.STATUS_CODE == SignboardConstant.S24_STATUS_COMPLIED_WITH
                    //                                    select sv24).Distinct().Count();
                    //model.S24ComList.Add(MonthlyStatisticS24SubCount8.ToString());
                    //var MonthlyStatisticS24SubCount9 = (from sv24 in db.B_SV_24_ORDER
                    //                                    where sv24.RECEIVED_DATE >= startDate
                    //                                    && sv24.RECEIVED_DATE <= endDate
                    //                                    && sv24.STATUS_CODE == SignboardConstant.S24_STATUS_WITHDRAWN
                    //                                    select sv24).Distinct().Count();
                    //model.S24WithdrawnList.Add(MonthlyStatisticS24SubCount9.ToString());
                    var MonthlyStatisticReportToScuCount = (from svc in db.B_SV_COMPLAIN
                                                            where svc.COMPLAINT_DATE >= startDate
                                                            && svc.COMPLAINT_DATE <= endDate
                                                            && svc.RECORD_TYPE == SignboardConstant.COMPLAIN
                                                            select svc
                                                            ).Distinct().Count();
                    model.ReportToScuList.Add(MonthlyStatisticReportToScuCount.ToString());
                    var MonthlyStatisticReportToScuByStatusCountOpen = (from svc in db.B_SV_COMPLAIN
                                                                        where svc.COMPLAINT_DATE >= startDate
                                                                        && svc.COMPLAINT_DATE <= endDate
                                                                        && svc.RECORD_TYPE == SignboardConstant.COMPLAIN_STATUS_OPEN
                                                                        select svc
                                                                    ).Distinct().Count();
                    model.ReportToScuReceivedList.Add(MonthlyStatisticReportToScuByStatusCountOpen.ToString());
                    var MonthlyStatisticReportToScuByStatusCountClose = (from svc in db.B_SV_COMPLAIN
                                                                         where svc.COMPLAINT_DATE >= startDate
                                                                         && svc.COMPLAINT_DATE <= endDate
                                                                         && svc.RECORD_TYPE == SignboardConstant.COMPLAIN_STATUS_CLOSE
                                                                         select svc).Distinct().Count();
                    model.ReportToScuCloseList.Add(MonthlyStatisticReportToScuByStatusCountClose.ToString());
                    var MonthlyStatisticEnquiryToScuCount = (from svc in db.B_SV_COMPLAIN
                                                             where svc.COMPLAINT_DATE >= startDate
                                                             && svc.COMPLAINT_DATE <= endDate
                                                             && svc.RECORD_TYPE == SignboardConstant.ENQUIRY.ToUpper()
                                                             select svc).Distinct().Count();
                    model.EnquiryToScuList.Add(MonthlyStatisticEnquiryToScuCount.ToString());
                    var MonthlyStatisticEnquiryToScuByStatusCountOpen = (from svc in db.B_SV_COMPLAIN
                                                                         where svc.COMPLAINT_DATE >= startDate
                                                                         && svc.COMPLAINT_DATE <= endDate
                                                                         && svc.RECORD_TYPE.ToUpper() == SignboardConstant.ENQUIRY.ToUpper()
                                                                         && svc.STATUS == SignboardConstant.COMPLAIN_STATUS_OPEN
                                                                         select svc).Distinct().Count();
                    model.EnquiryToScuReceivedList.Add(MonthlyStatisticEnquiryToScuByStatusCountOpen.ToString());
                    var MonthlyStatisticEnquiryToScuByStatusCountClose = (from svc in db.B_SV_COMPLAIN
                                                                          where svc.COMPLAINT_DATE >= startDate
                                                                          && svc.COMPLAINT_DATE <= endDate
                                                                          && svc.RECORD_TYPE.ToUpper() == SignboardConstant.ENQUIRY.ToUpper()
                                                                          && svc.STATUS == SignboardConstant.COMPLAIN_STATUS_OPEN
                                                                          select svc).Distinct().Count();
                    model.EnquiryToScuCloseList.Add(MonthlyStatisticEnquiryToScuByStatusCountClose.ToString());
                }
                return new Fn04RPT_MRSearchModel()
                {
                    getMonthList = tempMonthList,
                    ReceivedRecordCountList = model.ReceivedRecordCountList,
                    ProcessingRecordCountList = model.ProcessingRecordCountList,
                    AcknowledgedRecordCountList = model.AcknowledgedRecordCountList,
                    RefusedRecordCountList = model.RefusedRecordCountList,
                    ConditionalRecordCountList = model.ConditionalRecordCountList,
                    AuditRecordCountList = model.AuditRecordCountList,
                    ReceivedRecordCountList01C = model.ReceivedRecordCountList01C,
                    ProcessingRecordCountList01C = model.ProcessingRecordCountList01C,
                    AcknowledgedRecordCountList01C = model.AcknowledgedRecordCountList01C,
                    RefusedRecordCountList01C = model.RefusedRecordCountList,
                    ConditionalRecordCountList01C = model.ConditionalRecordCountList01C,
                    AuditRecordCountList01C = model.AuditRecordCountList01C,


                    ReceivedRecordCountListSC02 = model.ReceivedRecordCountListSC02,
                    ProcessingRecordCountListSC02 = model.ProcessingRecordCountListSC02,
                    AcknowledgedRecordCountListSC02 = model.AcknowledgedRecordCountListSC02,
                    RefusedRecordCountListSC02 = model.RefusedRecordCountListSC02,
                    ConditionalRecordCountListSC02 = model.ConditionalRecordCountListSC02,
                    AuditRecordCountListSC02 = model.AuditRecordCountListSC02,


                    ReceivedRecordCountListSC02C = model.ReceivedRecordCountListSC02C,
                    ProcessingRecordCountListSC02C = model.ProcessingRecordCountListSC02C,
                    AcknowledgedRecordCountListSC02C = model.AcknowledgedRecordCountListSC02C,
                    RefusedRecordCountListSC02C = model.RefusedRecordCountListSC02C,
                    ConditionalRecordCountListSC02C = model.ConditionalRecordCountListSC02C,
                    AuditRecordCountListSC02C = model.AuditRecordCountListSC02C,


                    ReceivedRecordCountListSC03 = model.ReceivedRecordCountListSC03,
                    ProcessingRecordCountListSC03 = model.ProcessingRecordCountListSC03,
                    AcknowledgedRecordCountListSC03 = model.AcknowledgedRecordCountListSC03,
                    RefusedRecordCountListSC03 = model.RefusedRecordCountListSC03,
                    ConditionalRecordCountListSC03 = model.ConditionalRecordCountListSC03,
                    AuditRecordCountListSC03 = model.AuditRecordCountListSC03,
                    GcList = model.GcList,
                    //S24List = model.S24List,
                    //S24IssList = model.S24IssList,
                    //S24ProList = model.S24ProList,
                    //S24GcaList = model.S24GcaList,
                    //S24ComList = model.S24ComList,
                    //S24NotYetIssList = model.S24NotYetIssList,
                    //S24NotYetExpList = model.S24NotYetExpList,
                    //S24AppealLodgedList = model.S24AppealLodgedList,
                    //S24WithdrawnList = model.S24WithdrawnList,
                    //S24ProsecutionToBeList = model.S24ProsecutionToBeList,
                    //S24PendingList = model.S24PendingList,
                    ReportToScuList = model.ReportToScuList,
                    ReportToScuReceivedList = model.ReportToScuReceivedList,
                    ReportToScuCloseList = model.ReportToScuCloseList,
                    EnquiryToScuList = model.EnquiryToScuList,
                    EnquiryToScuReceivedList = model.EnquiryToScuReceivedList,
                    EnquiryToScuCloseList = model.EnquiryToScuCloseList
                };
            }
        }
        public FileStreamResult ExportMR(Fn04RPT_MRSearchModel searchModel)
        {
            string fileName = "MonthlyRecordsReport";
            Fn04RPT_MRSearchModel exportModel = ViewMRRecord(searchModel);

            MemoryStream stream = new MemoryStream();
            ExcelPackage ep = new ExcelPackage();

            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Export");

            // header
            sheet.Cells["A1"].Value = "Form Code";
            sheet.Cells["A1"].Style.Font.Bold = true;
            sheet.Cells["A1"].Style.Font.Size = 14;
            sheet.Cells["A1"].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            sheet.Cells["B1"].Value = "BD Reply/ Statistic";
            sheet.Cells["B1"].Style.Font.Bold = true;
            sheet.Cells["B1"].Style.Font.Size = 14;
            sheet.Cells["B1"].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            for (int i = 0; i < exportModel.getMonthList.Count; i++)
            {
                string date = exportModel.getMonthList[i].ToString("MMM-yyyy");
                //sheet.Cells[1, i+3].LoadFromText(date);
                sheet.Cells[1, i+3].Value = date;
                sheet.Cells[1, i+3].Style.Font.Bold = true;
                sheet.Cells[1, i+3].Style.Font.Size = 14;
                sheet.Cells[1, i+3].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            }

            string[] ScFormTitle = { "Received", "Processing", "Accept", "Conditional Accept", "Refuse", "Audit" };
            string[] ScuTitle = { "Total", "Received", "Closed" };

            // SC01
            sheet.Cells["A2:A7"].Merge = true;
            sheet.Cells["A2"].Value = "SC01";
            sheet.Cells["A2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells["A2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            for(int y = 0; y < ScFormTitle.Count(); y++)
            {
                sheet.Cells[y+2, 2].LoadFromText(ScFormTitle[y]);
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++ )
            {
                sheet.Cells[2, x].LoadFromText(exportModel.ReceivedRecordCountList[x-3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++ )
            {
                sheet.Cells[3, x].LoadFromText(exportModel.ProcessingRecordCountList[x-3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++ )
            {
                sheet.Cells[4, x].LoadFromText(exportModel.AcknowledgedRecordCountList[x-3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++ )
            {
                sheet.Cells[5, x].LoadFromText(exportModel.ConditionalRecordCountList[x-3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++ )
            {
                sheet.Cells[6, x].LoadFromText(exportModel.RefusedRecordCountList[x-3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++ )
            {
                sheet.Cells[7, x].LoadFromText(exportModel.AuditRecordCountList[x-3].ToString());
            }

            // SC01C
            sheet.Cells["A8:A13"].Merge = true;
            sheet.Cells["A8"].Value = "SC01C";
            sheet.Cells["A8"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells["A8"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            for (int y = 0; y < ScFormTitle.Count(); y++)
            {
                sheet.Cells[y+8, 2].LoadFromText(ScFormTitle[y]);
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[8, x].LoadFromText(exportModel.ReceivedRecordCountList01C[x-3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[9, x].LoadFromText(exportModel.ProcessingRecordCountList01C[x-3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[10, x].LoadFromText(exportModel.AcknowledgedRecordCountList01C[x-3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[11, x].LoadFromText(exportModel.ConditionalRecordCountList01C[x-3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[12, x].LoadFromText(exportModel.RefusedRecordCountList01C[x-3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[13, x].LoadFromText(exportModel.AuditRecordCountList01C[x-3].ToString());
            }
            // SC02
            sheet.Cells["A14:A19"].Merge = true;
            sheet.Cells["A14"].Value = "SC03";
            sheet.Cells["A14"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells["A14"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            for (int y = 0; y < ScFormTitle.Count(); y++)
            {
                sheet.Cells[y + 14, 2].LoadFromText(ScFormTitle[y]);
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[14, x].LoadFromText(exportModel.ReceivedRecordCountListSC02[x-3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[15, x].LoadFromText(exportModel.ProcessingRecordCountListSC02[x-3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[16, x].LoadFromText(exportModel.AcknowledgedRecordCountListSC02[x-3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[17, x].LoadFromText(exportModel.ConditionalRecordCountListSC02[x-3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[18, x].LoadFromText(exportModel.RefusedRecordCountListSC02[x-3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[19, x].LoadFromText(exportModel.AuditRecordCountListSC02[x-3].ToString());
            }

            // SC02
            sheet.Cells["A20:A25"].Merge = true;
            sheet.Cells["A20"].Value = "SC02C";
            sheet.Cells["A20"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells["A20"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            for (int y = 0; y < ScFormTitle.Count(); y++)
            {
                sheet.Cells[y + 20, 2].LoadFromText(ScFormTitle[y]);
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[20, x].LoadFromText(exportModel.ReceivedRecordCountListSC02C[x - 3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[21, x].LoadFromText(exportModel.ProcessingRecordCountListSC02C[x - 3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[22, x].LoadFromText(exportModel.AcknowledgedRecordCountListSC02C[x - 3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[23, x].LoadFromText(exportModel.ConditionalRecordCountListSC02C[x - 3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[24, x].LoadFromText(exportModel.RefusedRecordCountListSC02C[x - 3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[25, x].LoadFromText(exportModel.AuditRecordCountListSC02C[x - 3].ToString());
            }

            // SC03
            sheet.Cells["A26:A31"].Merge = true;
            sheet.Cells["A26"].Value = "SC02C";
            sheet.Cells["A26"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells["A26"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            for (int y = 0; y < ScFormTitle.Count(); y++)
            {
                sheet.Cells[y + 26, 2].LoadFromText(ScFormTitle[y]);
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[26, x].LoadFromText(exportModel.ReceivedRecordCountListSC03[x - 3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[27, x].LoadFromText(exportModel.ProcessingRecordCountListSC03[x - 3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[28, x].LoadFromText(exportModel.AcknowledgedRecordCountListSC03[x - 3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[29, x].LoadFromText(exportModel.ConditionalRecordCountListSC03[x - 3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[30, x].LoadFromText(exportModel.RefusedRecordCountListSC03[x - 3].ToString());
            }
            for (int x = 3; x < exportModel.getMonthList.Count + 3; x++)
            {
                sheet.Cells[31, x].LoadFromText(exportModel.AuditRecordCountListSC03[x - 3].ToString());
            }

            sheet.Cells.AutoFitColumns();
            ep.SaveAs(stream);
            stream.Position = 0;
            FileStreamResult fsr = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            fsr.FileDownloadName = fileName + ".xlsx";  //.xlsx
            return fsr;
        }
        //------------------------------------------------------------------------------
        // Fn01SCUR_SDSearchModel: Seach Function
        public Fn01SCUR_SDSearchModel SearchSCUR_SD(Fn01SCUR_SDSearchModel model)
        {
            model.Query = SearchCUR_SD_q;

            model.QueryWhere = SearchCUR_SD_whereQ(model);

            model.Search();
            return model;
        }
        private string SearchCUR_SD_whereQ(Fn01SCUR_SDSearchModel model)
        {
            string whereQ = "";
            string RefNo = "";

            if (model.FILE_REFERCEN_NO != null) {
                RefNo += model.FILE_REFERCEN_NO;
            }

            whereQ += "\r\n\t" + " where lower(REFERENCE_NO) like :fileRefNo ";
            model.QueryParameters.Add("fileRefNo", "%" + RefNo.ToLower() + "%");
            return whereQ;
        }
        // Fn01SCUR_SDSearchModel: Export Seach Function
        public string ExportCUR_SD(Fn01SCUR_SDSearchModel model)
        {
            model.Query = SearchCUR_SD_q;
            model.QueryWhere = SearchCUR_SD_whereQ(model);
            return model.Export("Scan Document Report");
        }
        //------------------------------------------------------------------------------
        public FileStreamResult ExportMS(Fn04RPT_MSSearchModel searchModel)
        {
            string fileName = "MonthlyStatisticsReport";
            Fn04RPT_MSSearchModel exportModel = ViewMSRecord(searchModel);

            MemoryStream stream = new MemoryStream();
            ExcelPackage ep = new ExcelPackage();

            ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Export");

            // header
            sheet.Cells["A1"].Value = "Type";
            sheet.Cells["A1"].Style.Font.Bold = true;
            sheet.Cells["A1"].Style.Font.Size = 14;
            sheet.Cells["A1"].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            for (int i = 0; i < exportModel.MonthList.Count; i++)
            {
                string date = exportModel.MonthList[i].ToString("MMM-yyyy");
                //sheet.Cells[1, i+3].LoadFromText(date);
                sheet.Cells[1, i + 3].Value = date;
                sheet.Cells[1, i + 3].Style.Font.Bold = true;
                sheet.Cells[1, i + 3].Style.Font.Size = 14;
                sheet.Cells[1, i + 3].Style.Font.Color.SetColor(System.Drawing.Color.Blue);
            }

            string[] MwClassTitle = { "Acknowledged", "Conditional", "Refused", "Audit" };
            string[] ScuTitle = { "Total", "Received", "Closed" };

            // MW Class I
            sheet.Cells["A2:A5"].Merge = true;
            sheet.Cells["A2"].Value = "MW Class I";
            sheet.Cells["A2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells["A2"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            for (int y = 0; y < MwClassTitle.Count(); y++)
            {
                sheet.Cells[y + 2, 2].LoadFromText(MwClassTitle[y]);
            }
            for (int x = 3; x < exportModel.MonthList.Count + 3; x++)
            {
                sheet.Cells[2, x].LoadFromText(exportModel.ClassIAcknowledgedList[x - 3].ToString());
            }
            for (int x = 3; x < exportModel.MonthList.Count + 3; x++)
            {
                sheet.Cells[3, x].LoadFromText(exportModel.ClassIConditionalList[x - 3].ToString());
            }
            for (int x = 3; x < exportModel.MonthList.Count + 3; x++)
            {
                sheet.Cells[4, x].LoadFromText(exportModel.ClassIRefusedList[x - 3].ToString());
            }
            for (int x = 3; x < exportModel.MonthList.Count + 3; x++)
            {
                sheet.Cells[5, x].LoadFromText(exportModel.ClassIAuditList[x - 3].ToString());
            }
            // MW Class II
            sheet.Cells["A6:A9"].Merge = true;
            sheet.Cells["A6"].Value = "MW Class I";
            sheet.Cells["A6"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells["A6"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            for (int y = 0; y < MwClassTitle.Count(); y++)
            {
                sheet.Cells[y + 6, 2].LoadFromText(MwClassTitle[y]);
            }
            for (int x = 3; x < exportModel.MonthList.Count + 3; x++)
            {
                sheet.Cells[6, x].LoadFromText(exportModel.ClassIIAcknowledgedList[x - 3].ToString());
            }
            for (int x = 3; x < exportModel.MonthList.Count + 3; x++)
            {
                sheet.Cells[7, x].LoadFromText(exportModel.ClassIIConditionalList[x - 3].ToString());
            }
            for (int x = 3; x < exportModel.MonthList.Count + 3; x++)
            {
                sheet.Cells[8, x].LoadFromText(exportModel.ClassIIRefusedList[x - 3].ToString());
            }
            for (int x = 3; x < exportModel.MonthList.Count + 3; x++)
            {
                sheet.Cells[9, x].LoadFromText(exportModel.ClassIIAuditList[x - 3].ToString());
            }
            // MW Class III
            sheet.Cells["A10:A13"].Merge = true;
            sheet.Cells["A10"].Value = "MW Class I";
            sheet.Cells["A10"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            sheet.Cells["A10"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

            for (int y = 0; y < MwClassTitle.Count(); y++)
            {
                sheet.Cells[y + 10, 2].LoadFromText(MwClassTitle[y]);
            }
            for (int x = 3; x < exportModel.MonthList.Count + 3; x++)
            {
                sheet.Cells[10, x].LoadFromText(exportModel.ClassIIIAcknowledgedList[x - 3].ToString());
            }
            for (int x = 3; x < exportModel.MonthList.Count + 3; x++)
            {
                sheet.Cells[11, x].LoadFromText(exportModel.ClassIIIConditionalList[x - 3].ToString());
            }
            for (int x = 3; x < exportModel.MonthList.Count + 3; x++)
            {
                sheet.Cells[12, x].LoadFromText(exportModel.ClassIIIRefusedList[x - 3].ToString());
            }
            for (int x = 3; x < exportModel.MonthList.Count + 3; x++)
            {
                sheet.Cells[13, x].LoadFromText(exportModel.ClassIIIAuditList[x - 3].ToString());
            }

            sheet.Cells.AutoFitColumns();
            ep.SaveAs(stream);
            stream.Position = 0;
            FileStreamResult fsr = new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            fsr.FileDownloadName = fileName + ".xlsx";  //.xlsx
            return fsr;
        }
        public Fn04RPT_SPRSearchModel SPReport(Fn04RPT_SPRSearchModel model)
        {
            model.Query = generateSubmissionProgressReport(model);
            model.Search();
            for (int i = 0; i < model.Data.Count(); i++)
            {
                var a = DateUtil.getWFStatusDisplay(model.Data[i]["STAGE"].ToString());
                model.Data[i]["STAGE"] = a;
            }
            return model;
        }
        public Fn04RPT_SPRSearchModel SCReport(Fn04RPT_SPRSearchModel model)
        {
            model.Query = generateSubmissionCountReport(model);
            model.Search();
            for (int i = 0; i < model.Data.Count(); i++)
            {
                var a = DateUtil.getDisplaySetZero(model.Data[i]["SC01"].ToString());
                model.Data[i]["SC01"] = a;
                var b = DateUtil.getDisplaySetZero(model.Data[i]["SC02"].ToString());
                model.Data[i]["SC02"] = b;
                var c = DateUtil.getDisplaySetZero(model.Data[i]["SC03"].ToString());
                model.Data[i]["SC03"] = c;
            }
            return model;
        }
        public Fn04RPT_SPRSearchModel SIReport(Fn04RPT_SPRSearchModel model)
        {
            model.Query = generateSubmissionInvolvedReport(model);
            model.Search();
            for (int i = 0; i < model.Data.Count(); i++)
            {
                var a = DateUtil.getDisplaySetZero(model.Data[i]["SC01"].ToString());
                model.Data[i]["SC01"] = a;
                var b = DateUtil.getDisplaySetZero(model.Data[i]["SC02"].ToString());
                model.Data[i]["SC02"] = b;
                var c = DateUtil.getDisplaySetZero(model.Data[i]["SC03"].ToString());
                model.Data[i]["SC03"] = c;
            }
            return model;
        }
        public Fn04RPT_STSearchModel SearchST(Fn04RPT_STSearchModel model)
        {
            model.Query = getSubordinateTaskResult(model);
            model.Search();
            
            return model;
        }
        public string ExportST(Fn04RPT_STSearchModel model)
        {
            model.Query = getSubordinateTaskResult(model);

            return model.Export("Un-process Submission Report");
        }
        public Fn04RPT_DESearchModel SearchDE(Fn04RPT_DESearchModel model)
        {
            model.Query = SearchDE_q;
            model.QueryWhere = SearchDE_whereQ(model);
            model.Search();
            return model;
        }
        private string SearchDE_whereQ(Fn04RPT_DESearchModel model)
        {
            string whereQ = "";
            if (!string.IsNullOrWhiteSpace(model.SubNo))
            {
                whereQ += "\r\n\t" + " and lower(svr.reference_No) like :fileRefNo ";
                model.QueryParameters.Add("fileRefNo", "%" + model.SubNo.ToLower() + "%");
            }
            return whereQ;
        }

        public SMMUpdateJobAssignSearchModel SearchUJA(SMMUpdateJobAssignSearchModel model)
        {
            model.Query = SearchUJA_q;
            model.QueryWhere = SearchUJA_whereQ(model);
            model.Search();
            return model;
        }

        public string Excel(SMMUpdateJobAssignSearchModel model)
        {
            model.Query = SearchUJA_q;
            model.QueryWhere = SearchUJA_whereQ(model);
            return model.Export("Update Job Assign");
        }

        public SMMUpdateJobAssignSearchModel ViewUJA(string id, string type, string task, string user, string refNo, string wftuUuid) {

            SMMUpdateJobAssignSearchModel model = new SMMUpdateJobAssignSearchModel();

            List<SelectListItem> list = new List<SelectListItem>();
            //list.AddRange(SignboardJobAssignmentDAOService.GetScuTeamByPosition(SignboardConstant.S_USER_ACCOUNT_RANK_SPO));
            //list.AddRange(SignboardJobAssignmentDAOService.GetScuTeamByPosition(SignboardConstant.S_USER_ACCOUNT_RANK_PO));
            //list.AddRange(SignboardJobAssignmentDAOService.GetScuTeamByPosition(SignboardConstant.S_USER_ACCOUNT_RANK_TO));

            using (EntitiesAuth auth = new EntitiesAuth())
            {
                model.SYS_POST = auth.SYS_POST.Where(o => o.UUID == id).FirstOrDefault();
                model.UUID = id;
                model.RefNo = refNo;
                model.Type = type;
                model.Task = task;
                model.User = user;
                model.NewHandler = model.User;
                model.wftuUuid = wftuUuid;
            }

            return model;
        }

        public void SaveForm(SMMUpdateJobAssignSearchModel model)
        {
            using(EntitiesSignboard db = new EntitiesSignboard())
            {
                var query = db.B_WF_TASK_USER.Find(model.wftuUuid);
                query.SYS_POST_ID = model.NewHandler;
                db.SaveChanges();
            }
        }
        
        public string ExportSPReport(Fn04RPT_SPRSearchModel model)
        {
            model.Query = generateSubmissionProgressReport(model);
            //for (int i = 0; i < model.Data.Count(); i++)
            //{
            //    var a = DateUtil.getWFStatusDisplay(model.Data[i]["STAGE"].ToString());
            //    model.Data[i]["STAGE"] = a;
            //}
            return model.Export("Submission Progress Report");
        }
        public string ExportSCReport(Fn04RPT_SPRSearchModel model)
        {
            model.Query = generateSubmissionCountReport(model);

            return model.Export("Submission Count Report");
        }
        public string ExportSIReport(Fn04RPT_SPRSearchModel model)
        {
            model.Query = generateSubmissionInvolvedReport(model);
            //for (int i = 0; i < model.Data.Count(); i++)
            //{
            //    var a = DateUtil.getDisplaySetZero(model.Data[i]["SC01"].ToString());
            //    model.Data[i]["SC01"] = a;
            //    var b = DateUtil.getDisplaySetZero(model.Data[i]["SC02"].ToString());
            //    model.Data[i]["SC02"] = b;
            //    var c = DateUtil.getDisplaySetZero(model.Data[i]["SC03"].ToString());
            //    model.Data[i]["SC03"] = c;
            //}
            return model.Export("Submission Involved Report");
        }

        public DSNSearchModel SearchDSN(string DSN)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {

                var query = from t1 in db.P_MW_DSN
                            where t1.DSN == DSN
                            select t1;

                if (query == null)
                {
                    return null;
                }

                return new DSNSearchModel()
                {
                    P_MW_DSN = query.FirstOrDefault()
                };

            }

        }

        //public ServiceResult SearchDSN(string DSN)
        //{

        //    List<P_MW_DSN> draftList = SessionUtil.DraftList<P_MW_DSN>(ApplicationConstant.DRAFT_KEY_C_COMP_APPLICANT_INFO);
        //    bool exist = draftList.Where(o => o.DSN == DSN).FirstOrDefault() != null;
        //    if (exist)
        //    {
        //        return new ServiceResult()
        //        {
        //            Result = ServiceResult.RESULT_FAILURE,
        //            Message = new List<string>() { "No record found." }
        //        };
        //    }

        //   return new ServiceResult() { Result = ServiceResult.RESULT_SUCCESS };

        // }

     

        public void SaveDSN(DSNSearchModel model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                var query = db.P_MW_DSN.Find(model.P_MW_DSN.UUID);

                query.SUBMISSION_NATURE = model.P_MW_DSN.SUBMISSION_NATURE;
                //query.RECORD_ID = model.P_MW_DSN.RECORD_ID;
                //query.FORM_CODE = model.P_MW_DSN.FORM_CODE;
                //query.SUBMISSION_NATURE = model.P_MW_DSN.SUBMISSION_NATURE;
                //query.MODIFIED_DATE = System.DateTime.Now;
                //query.MODIFIED_BY = SystemParameterConstant.UserName; 

                db.SaveChanges();
            }
        }

    }
}