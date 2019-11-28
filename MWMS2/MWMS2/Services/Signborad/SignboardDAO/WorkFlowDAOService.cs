using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class WorkFlowDAOService
    {
        public List<List<object>> getWFTaskUser(string flowType, string keyNumber, string taskType)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();

            string queryString = ""
                + " \r\n select distinct u.sys_post_id, su.BD_PORTAL_LOGIN"
                + " \r\n from b_Wf_Info s, b_Wf_Task t, b_Wf_Task_User u, sys_post su"
                + " \r\n WHERE t.wf_info_id = s.uuid and t.uuid = u.wf_task_id"
                + " \r\n\t AND s.record_Type = :recordType"
                + " \r\n\t AND s.record_Id = :recordId"
                + " \r\n\t AND t.task_Code = :taskType"
                + " \r\n\t and u.status = :taskUserStatus"
                + " \r\n\t and u.sys_post_id = su.uuid"
                + " \r\n order by su.BD_PORTAL_LOGIN";

            queryParameters.Add("recordType", flowType);
            queryParameters.Add("recordId", keyNumber);
            queryParameters.Add("taskType", taskType);
            queryParameters.Add("taskUserStatus", SignboardConstant.WF_STATUS_DONE);


            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryString, queryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return data;
        }

        public List<List<object>> getWFTaskUserStatusOpen(string flowType, string keyNumber, string taskType)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();

            string queryString = ""
                + " \r\n select distinct u.sys_post_id, su.BD_PORTAL_LOGIN"
                + " \r\n from b_Wf_Info s, b_Wf_Task t, b_Wf_Task_User u, sys_post su"
                + " \r\n WHERE t.wf_info_id = s.uuid and t.uuid = u.wf_task_id"
                + " \r\n\t AND s.record_Type = :recordType"
                + " \r\n\t AND s.record_Id = :recordId"
                + " \r\n\t AND t.task_Code = :taskType"
                + " \r\n\t and u.status = :taskUserStatus"
                + " \r\n\t and u.sys_post_id = su.uuid"
                + " \r\n order by su.BD_PORTAL_LOGIN";

            queryParameters.Add("recordType", flowType);
            queryParameters.Add("recordId", keyNumber);
            queryParameters.Add("taskType", taskType);
            queryParameters.Add("taskUserStatus", SignboardConstant.WF_STATUS_OPEN);


            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                using (DbConnection conn = db.Database.Connection)
                {
                    DbDataReader dr = CommonUtil.GetDataReader(conn, queryString, queryParameters);
                    data = CommonUtil.convertToList(dr);
                    conn.Close();
                }
            }
            return data;
        }

        public B_WF_ASSIGNMENT getCurrentWfAssignment(string wfCode)
        {
            B_WF_ASSIGNMENT result = new B_WF_ASSIGNMENT();
            List<B_WF_ASSIGNMENT> resultList = new List<B_WF_ASSIGNMENT>();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                resultList = (from wfa in db.B_WF_ASSIGNMENT
                              where wfa.WF_MAP_CODE == wfCode
                              select wfa).ToList();
            }
            if (resultList.Count() != 0)
            {
                result = resultList[0];
            }
            return result;
        }
    }
}