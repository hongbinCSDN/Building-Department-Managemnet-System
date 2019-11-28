using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class ReportDAOService
    {
        public List<List<object>> GetSubordinateTaskResult(string status, DateTime? PeriodDateFrom, DateTime? PeriodDateTo)
        {
            string whereQ = "";
            //if (PeriodDateFrom == null || PeriodDateTo == null)
            //{
            //    if (PeriodDateFrom == null)
            //    {
            //        PeriodDateFrom = new DateTime(1999, 1, 1);

            //    }
            //    if (PeriodDateTo == null)
            //    {
            //        PeriodDateTo = new DateTime(3000, 12, 1);
            //    }
            //    whereQ = "\r\n" + "\t" + " AND R.RECEIVED_DATE BETWEEN " + PeriodDateFrom + " AND " + PeriodDateTo;
            //}
            string sv_recordQ = ""
                + "\r\n" + "\t" + " SELECT COUNT (DISTINCT R.UUID) as count"
                + "\r\n" + "\t" + " , sua.uuid"
                + "\r\n" + "\t" + " FROM b_WF_INFO F"
                + "\r\n" + "\t" + " INNER JOIN b_WF_TASK T ON F.UUID = T.WF_INFO_ID"
                + "\r\n" + "\t" + " INNER JOIN b_WF_TASK_USER U ON T.UUID = U.WF_TASK_ID"
                //+ "\r\n" + "\t" + " join b_s_user_account sua on u.user_id = sua.uuid"
                + "\r\n" + "\t" + " join sys_post sua on u.sys_post_id = sua.uuid"
                + "\r\n" + "\t" + " INNER JOIN b_sv_record R ON F.RECORD_ID = R.UUID"
                + "\r\n" + "\t" + " INNER JOIN b_SV_VALIDATION V ON R.UUID = V.SV_RECORD_ID"
                + "\r\n" + "\t" + " WHERE T.TASK_CODE in ('" + SignboardConstant.WF_MAP_VALIDATION_TO + "','" + SignboardConstant.WF_MAP_VALIDATION_PO + "','" + SignboardConstant.WF_MAP_VALIDATION_SPO + "') "
                + whereQ
             
                + "\r\n" + "\t" + " AND U.STATUS = '" + status + "'"
                //+ "\r\n" + "\t" + " AND T.STATUS = '" + status + "'"
                //+ "\r\n" + "\t" + " AND F.RECORD_TYPE = '" + SignboardConstant.WF_TYPE_VALIDATION + "'"
                + "\r\n" + "\t" + " group by sua.uuid"
                ;
            string sv_recordQ2 = ""
                + "\r\n" + "\t" + " SELECT COUNT (DISTINCT R.UUID) as count"
                + "\r\n" + "\t" + " , sua.uuid"
                + "\r\n" + "\t" + " FROM b_WF_INFO F"
                + "\r\n" + "\t" + " INNER JOIN b_WF_TASK T ON F.UUID = T.WF_INFO_ID"
                + "\r\n" + "\t" + " INNER JOIN b_WF_TASK_USER U ON T.UUID = U.WF_TASK_ID"
                //+ "\r\n" + "\t" + " join b_s_user_account sua on u.user_id = sua.uuid"
                + "\r\n" + "\t" + " join sys_post sua on u.sys_post_id = sua.uuid"
                + "\r\n" + "\t" + " INNER JOIN b_sv_record R ON F.RECORD_ID = R.UUID"
                + "\r\n" + "\t" + " INNER JOIN b_SV_VALIDATION V ON R.UUID = V.SV_RECORD_ID"
                + "\r\n" + "\t" + " WHERE T.TASK_CODE in ('" + SignboardConstant.WF_MAP_AUDIT_TO + "','" + SignboardConstant.WF_MAP_AUDIT_PO + "','" + SignboardConstant.WF_MAP_AUDIT_SPO + "') "
                + whereQ
                + "\r\n" + "\t" + " AND U.STATUS = '" + status + "'"
                + "\r\n" + "\t" + " group by sua.uuid"
                ;
            //string sv_gc = ""
            //    + "\r\n" + "\t" + " SELECT COUNT (DISTINCT R.UUID) as count "
            //    + "\r\n" + "\t" + " , sua.uuid "
            //    + "\r\n" + "\t" + " FROM b_WF_INFO F "
            //    + "\r\n" + "\t" + " INNER JOIN b_WF_TASK T ON F.UUID = T.WF_INFO_ID "
            //    + "\r\n" + "\t" + " INNER JOIN b_WF_TASK_USER U ON T.UUID = U.WF_TASK_ID "
            //    //+ "\r\n" + "\t" + " join b_s_user_account sua on u.user_id = sua.uuid "
            //    + "\r\n" + "\t" + " join sys_post sua on u.sys_post_id = sua.uuid"
            //    + "\r\n" + "\t" + " INNER JOIN b_sv_record R ON F.RECORD_ID = R.UUID "
            //    + "\r\n" + "\t" + " WHERE T.TASK_CODE in ('" + SignboardConstant.WF_MAP_GC_SPO_APPROVE + "','" + SignboardConstant.WF_MAP_GC_PO + "','" + SignboardConstant.WF_MAP_GC_SPO_COMPILE + "','" + SignboardConstant.WF_MAP_GC_PO_COMPLI + "','" + SignboardConstant.WF_MAP_GC_END + "') "
            //    + whereQ
            //    + "\r\n" + "\t" + " AND U.STATUS = '" + status + "'"
            //    + "\r\n" + "\t" + " group by sua.uuid"
            //    ;
            //string sv_24_order = ""
            //    + "\r\n" + "\t" + " SELECT COUNT (DISTINCT R.UUID) as count "
            //    + "\r\n" + "\t" + " , sua.uuid "
            //    + "\r\n" + "\t" + " FROM b_WF_INFO F "
            //    + "\r\n" + "\t" + " INNER JOIN b_WF_TASK T ON F.UUID = T.WF_INFO_ID "
            //    + "\r\n" + "\t" + " INNER JOIN b_WF_TASK_USER U ON T.UUID = U.WF_TASK_ID "
            //    //+ "\r\n" + "\t" + " join b_s_user_account sua on u.user_id = sua.uuid "
            //    + "\r\n" + "\t" + " join sys_post sua on u.sys_post_id = sua.uuid"
            //    + "\r\n" + "\t" + " INNER JOIN B_SV_24_ORDER R ON F.RECORD_ID = R.UUID "
            //    + "\r\n" + "\t" + " WHERE T.TASK_CODE in ('" + SignboardConstant.WF_MAP_S24_SPO_APPROVE + "','" + SignboardConstant.WF_MAP_S24_PO + "','" + SignboardConstant.WF_MAP_S24_SPO_COMPILE + "','" + SignboardConstant.WF_MAP_S24_END + "') "
            //    + whereQ
            //    + "\r\n" + "\t" + " AND U.STATUS = '" + status + "'"
            //    + "\r\n" + "\t" + " group by sua.uuid"
            //    ;
            string queryString =  ""
                + "\r\n" + "\t" + "select"
                // + "\r\n" + "\t" + " sua.uuid, sua.username as USERNAME"
                + "\r\n" + "\t" + " sua.uuid, sua.bd_portal_login as USERNAME, scu.position"
                + "\r\n" + "\t" + " , sum(case when val.count is not null then val.count else 0 end) as VAL"
                + "\r\n" + "\t" + " , sum(case when aud.count is not null then aud.count else 0 end) as AUD"
          //      + "\r\n" + "\t" + " , sum(case when gc.count is not null then gc.count else 0 end) as GC"
           //     + "\r\n" + "\t" + " , sum(case when s24.count is not null then s24.count else 0 end) as S24ORDER"
                //+ "\r\n" + "\t" + " , sua.rank"
                + "\r\n" + "\t" + " FROM b_s_scu_team scu"
                //+ "\r\n" + "\t" + " LEFT JOIN b_s_user_account sua"
                + "\r\n" + "\t" + " LEFT JOIN sys_post sua"
                //+ "\r\n" + "\t" + " ON sua.uuid = scu.user_account_id"
                + "\r\n" + "\t" + " ON sua.uuid = scu.sys_post_id"
                + "\r\n" + "\t" + " left join ("
                + "\r\n" + "\t" + sv_recordQ
                + "\r\n" + "\t" + " ) val"
                + "\r\n" + "\t" + " on"
                + "\r\n" + "\t" + " sua.uuid = val.uuid"
                + "\r\n" + "\t" + " left join ("
                + "\r\n" + "\t" + sv_recordQ2
                + "\r\n" + "\t" + " ) aud"
                + "\r\n" + "\t" + " on"
                + "\r\n" + "\t" + " sua.uuid = aud.uuid"
                //+ "\r\n" + "\t" + " left join ("
                //+ "\r\n" + "\t" + sv_gc
                //+ "\r\n" + "\t" + " ) gc "
                //+ "\r\n" + "\t" + " on "
                //+ "\r\n" + "\t" + " sua.uuid = gc.uuid "
                //+ "\r\n" + "\t" + " left join ( "
                //+ "\r\n" + "\t" + sv_24_order
                //+ "\r\n" + "\t" + " ) s24"
                //+ "\r\n" + "\t" + " on "
                //+ "\r\n" + "\t" + " sua.uuid = s24.uuid "
                //// + "\r\n" + "\t" + " WHERE sua.security_level = '" + SignboardConstant.S_USER_ACCOUNT_SECURITY_LEVEL_SCU_INTERNAL + "'"
                + "\r\n" + "\t" + " WHERE sua.UUID IS NOT null"
                + "\r\n" + "\t" + " group by"
                //+ "\r\n" + "\t" + " sua.uuid, sua.username, sua.rank "
                + "\r\n" + "\t" + " sua.uuid, sua.bd_portal_login, scu.position "
                 + "\r\n" + "\t" + "ORDER BY CASE WHEN scu.position = '" + SignboardConstant.LOOK_UP_NAME_RANK_TO + "' THEN '1'"
                 + "\r\n" + "\t" + "WHEN scu.position = '" + SignboardConstant.LOOK_UP_NAME_RANK_PO + "' THEN '2'"
                 + "\r\n" + "\t" + "WHEN scu.position = '" + SignboardConstant.LOOK_UP_NAME_RANK_SPO + "' THEN '3' END asc, sua.bd_portal_login asc"
                ;


            string sqlTo = @"
	
	
	SELECT scu.child_sys_post_id,sua.BD_PORTAL_LOGIN ,NULL
	, sum(case when val.count is not null then val.count else 0 end) as VAL
 	, sum(case when aud.count is not null then aud.count else 0 end) as AUD
	FROM  b_s_scu_team scu
	 LEFT JOIN sys_post sua  ON sua.UUID = scu.child_sys_post_id  
  	  left join (	
	 SELECT COUNT (DISTINCT R.UUID) as count
	 , sua.uuid
	 FROM b_WF_INFO F
	 INNER JOIN b_WF_TASK T ON F.UUID = T.WF_INFO_ID
	 INNER JOIN b_WF_TASK_USER U ON T.UUID = U.WF_TASK_ID
	 join sys_post sua on u.sys_post_id = sua.uuid
	 INNER JOIN b_sv_record R ON F.RECORD_ID = R.UUID
	 INNER JOIN b_SV_VALIDATION V ON R.UUID = V.SV_RECORD_ID
	 WHERE T.TASK_CODE in ('WF_VALIDATION_TO','WF_VALIDATION_PO','WF_VALIDATION_SPO') 
	 AND U.STATUS = 'WF_STATUS_OPEN'
	 group by sua.uuid
	 ) val
	 on
	 sua.uuid = val.uuid
	 
	 left join (	
	 SELECT COUNT (DISTINCT R.UUID) as count
	 , sua.uuid
	 FROM b_WF_INFO F
	 INNER JOIN b_WF_TASK T ON F.UUID = T.WF_INFO_ID
	 INNER JOIN b_WF_TASK_USER U ON T.UUID = U.WF_TASK_ID
	 join sys_post sua on u.sys_post_id = sua.uuid
	 INNER JOIN b_sv_record R ON F.RECORD_ID = R.UUID
	 INNER JOIN b_SV_VALIDATION V ON R.UUID = V.SV_RECORD_ID
	 WHERE T.TASK_CODE in ('WF_AUDIT_TO','WF_AUDIT_PO','WF_AUDIT_SPO') 
	 AND U.STATUS = 'WF_STATUS_OPEN'
	 group by sua.uuid
	 ) aud
	 on
	 sua.uuid = aud.uuid
	  
	  
  	WHERE  child_sys_post_id NOT IN (SELECT DISTINCT(SYS_POST_ID) FROM B_S_SCU_TEAM)
and sua.BD_PORTAL_LOGIN is not NULL 
   group by
	 scu.child_sys_post_id,sua.BD_PORTAL_LOGIN
";



            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();

            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryString, queryParameters);
                    data = CommonUtil.convertToList(dr);

                    data.AddRange(CommonUtil.convertToList(CommonUtil.GetDataReader(conn, sqlTo, queryParameters)));
                    data=  data.OrderBy(x => x[1]).ToList();
                       
                    conn.Close();
                }
            }
            return data;

        }

        public int GetReportToScuJobResult(string userUuid)
        {
            var count = 0;
            var status = SignboardConstant.COMPLAIN_STATUS_OPEN;
            var report_to_scu = SignboardConstant.COMPLAIN;
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                count = db.B_SV_COMPLAIN.Where(x => x.STATUS == status)
                    .Where(x => x.RECORD_TYPE == report_to_scu)
                    .Where(x => (x.RESPONSIBLE_TO == userUuid || x.RESPONSIBLE_PO == userUuid || x.RESPONSIBLE_SPO == userUuid))
                    .Count();
            }
            return count;
        }
    }
}