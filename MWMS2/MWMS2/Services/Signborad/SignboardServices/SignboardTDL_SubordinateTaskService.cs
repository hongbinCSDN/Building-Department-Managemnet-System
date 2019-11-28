using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardServices
{
    public class SignboardTDL_SubordinateTaskService : BaseCommonService
    {
        string SearchTDL_ST_q = ""
            + "  select *  from "
            + " \r\n\t ((select distinct sua.uuid, sua.bd_portal_login, "
            + " \r\n\t B_GET_TASK_COUNT(sua.uuid, 'V', 'WF_STATUS_OPEN') val, "
            + " \r\n\t B_GET_TASK_COUNT(sua.uuid, 'A', 'WF_STATUS_OPEN') aud "
            + " \r\n\t from b_s_scu_team scu left join sys_post sua on sua.uuid = scu.sys_post_id "
            + " \r\n\t ) "
            + " \r\n\t union "
            + "\r\n\t (select distinct sua.uuid, sua.bd_portal_login, "
            + "\r\n\t B_GET_TASK_COUNT(sua.uuid, 'V', 'WF_STATUS_OPEN') val, "
            + "\r\n\t B_GET_TASK_COUNT(sua.uuid, 'A', 'WF_STATUS_OPEN') aud "
            + "\r\n\t  from b_s_scu_team scu "
            + "\r\n\t left join sys_post sua on sua.uuid = scu.child_sys_post_id "
            + "\r\n\t )) where 1=1  ";

        public Fn02TDL_STSearchModel SearchTDL_ST(Fn02TDL_STSearchModel model)
        {
            model.Query = SearchTDL_ST_q;
            model.QueryWhere = SearchTDL_ST_whereQ(model);

            model.Search();

            return model;
        }

        private string SearchTDL_ST_whereQ(Fn02TDL_STSearchModel model)
        {
            string whereQ = "";

            string currUserUuid = SessionUtil.LoginPost.UUID;
            List<string> list = new List<string>();
            //decimal left = 0;
            //decimal right = 0;
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                B_S_SCU_TEAM scu = db.B_S_SCU_TEAM.Where(x => x.SYS_POST_ID == currUserUuid).FirstOrDefault();

                list.Add(currUserUuid);
                list.AddRange(SystemListUtil.GetSubUser(currUserUuid));
                
                // left = scu.LEFT.Value;
                // right = scu.RIGHT.Value;
            }
            //whereQ += " \r\n where scu.left >= :left and scu.right <= :right";
            //model.QueryParameters.Add("left", left);
            //model.QueryParameters.Add("right", right);

            //if (!string.IsNullOrWhiteSpace(model.SearchClass))
            //{
            //    whereQ += " \r\n\t and sua.uuid = :uuid";
            //    model.QueryParameters.Add("uuid", model.SearchClass);
            //}
            if (list.Count() > 0)
            {
                whereQ += " \r\n\t and uuid IN (:uuidlist)";
                model.QueryParameters.Add("uuidlist", list);

            }
            return whereQ;
        }

        public Fn02TDL_STDisplayModel populateSubordinateTaskSearchForm(string uuid)
        {
            var validationTaskList = populateValidationToDoList(uuid);
            List<TaskModel> validationTaskListModel = new List<TaskModel>();
            if(validationTaskList != null && validationTaskList.Count() > 0)
            {
                for (int i = 0; i < validationTaskList.Count(); i++)
                {
                    TaskModel taskModel = new TaskModel();
                    taskModel.Uuid = (string)validationTaskList[i][0];
                    if(validationTaskList[i][5] != DBNull.Value)
                    {
                        taskModel.Task = (string)validationTaskList[i][5];
                    }
                    else
                    {
                        taskModel.Task = null;
                    }
                    if(validationTaskList[i][1] != DBNull.Value)
                    {
                        taskModel.SubmissionNo = (string)validationTaskList[i][1];
                    }
                    else
                    {
                        taskModel.SubmissionNo = null;
                    }
                    if(validationTaskList[i][6] != DBNull.Value)
                    {
                        taskModel.FormCode = (string)validationTaskList[i][6];
                    }
                    else
                    {
                        taskModel.FormCode = null;
                    }
                    if(validationTaskList[i][2] != DBNull.Value)
                    {
                        taskModel.ReceivedDate = (DateTime)validationTaskList[i][2];
                    }
                    else
                    {
                        taskModel.ReceivedDate = null;
                    }
                    taskModel.AssignmentDate = (DateTime)validationTaskList[i][3];
                    if(validationTaskList[i][4] != DBNull.Value)
                    {
                        taskModel.Status = (string)validationTaskList[i][4];
                    }
                    else
                    {
                        taskModel.Status = null;
                    }
                    
                    validationTaskListModel.Add(taskModel);
                }
            }

            var auditTaskList = populateAuditToDoList(uuid);
            List<TaskModel> auditTaskListModel = new List<TaskModel>();
            if (auditTaskList != null && auditTaskList.Count() > 0)
            {
                for (int i = 0; i < auditTaskList.Count(); i++)
                {
                    TaskModel taskModel = new TaskModel();
                    taskModel.Uuid = (string)auditTaskList[i][0];
                    if(auditTaskList[i][5] != DBNull.Value)
                    {
                        taskModel.Task = (string)auditTaskList[i][5];
                    }
                    else
                    {
                        taskModel.Task = null;
                    }
                    if (auditTaskList[i][1] != DBNull.Value)
                    {
                        taskModel.SubmissionNo = (string)auditTaskList[i][1];
                    }
                    else
                    {
                        taskModel.SubmissionNo = null;
                    }
                    if(auditTaskList[i][6] != DBNull.Value)
                    {
                        taskModel.FormCode = (string)auditTaskList[i][6];
                    }
                    else
                    {
                        taskModel.FormCode = null;
                    }
                    if(auditTaskList[i][2] != DBNull.Value)
                    {
                        taskModel.ReceivedDate = (DateTime)auditTaskList[i][2];
                    }
                    else
                    {
                        taskModel.ReceivedDate = null;
                    }
                    if(auditTaskList[i][3] != DBNull.Value)
                    {
                        taskModel.AssignmentDate = (DateTime)auditTaskList[i][3];
                    }
                    else
                    {
                        taskModel.AssignmentDate = null;
                    }
                    if(auditTaskList[i][4] != DBNull.Value)
                    {
                        taskModel.Status = (string)auditTaskList[i][4];
                    }
                    else
                    {
                        taskModel.Status = null;
                    }


                    auditTaskListModel.Add(taskModel);
                }
            }

            return new Fn02TDL_STDisplayModel
            {
                Uuid = uuid,
                ValidationTaskList = (validationTaskListModel != null && validationTaskListModel.Count() > 0) ? validationTaskListModel : null,
                AuditTaskList = (auditTaskListModel != null && auditTaskListModel.Count() > 0) ? auditTaskListModel : null,
                ValidationTaskListSize = (validationTaskListModel != null && validationTaskListModel.Count() > 0) ? validationTaskList.Count() : 0,
                AuditTaskListSize = (auditTaskListModel != null && auditTaskListModel.Count() > 0) ? auditTaskList.Count() : 0,

            };
        }

        public List<List<object>> populateValidationToDoList(string uuid)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();

            string queryString = ""
                + " \r\n SELECT DISTINCT "
                + " \r\n\t V.UUID AS VALIDATION_UUID, R.REFERENCE_NO, R.RECEIVED_DATE, "
                + " \r\n\t U.START_TIME, V.VALIDATION_STATUS, r.wf_status, r.FORM_CODE"
                + " \r\n\t FROM B_WF_INFO F"
                + " \r\n\t INNER JOIN B_WF_TASK T ON F.UUID = T.WF_INFO_ID"
                + " \r\n\t INNER JOIN B_WF_TASK_USER U ON T.UUID = U.WF_TASK_ID"
                + " \r\n\t INNER JOIN B_SV_RECORD R ON F.RECORD_ID = R.UUID"
                + " \r\n\t INNER JOIN B_SV_VALIDATION V ON R.UUID = V.SV_RECORD_ID"
                + " \r\n\t WHERE 1 = 1"
                + " \r\n\t AND F.RECORD_TYPE = '" + SignboardConstant.WF_TYPE_VALIDATION + "'"
                + " \r\n\t AND T.STATUS = '" + SignboardConstant.WF_STATUS_OPEN + "'"
                + " \r\n\t AND U.STATUS = '" + SignboardConstant.WF_STATUS_OPEN + "'"
                + " \r\n\t AND T.TASK_CODE IN('" + SignboardConstant.WF_MAP_VALIDATION_TO + "', '" + SignboardConstant.WF_MAP_VALIDATION_PO + "', '" + SignboardConstant.WF_MAP_VALIDATION_SPO + "')"
                + " \r\n\t AND U.SYS_POST_ID = :userID";

            queryParameters.Add("userID", uuid);


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

        public List<List<object>> populateAuditToDoList(string uuid)
        {
            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            List<List<object>> data = new List<List<object>>();

            string queryString = ""
                + " \r\n SELECT DISTINCT "
                + " \r\n\t V.UUID AS VALIDATION_UUID, R.REFERENCE_NO, R.RECEIVED_DATE, "
                + " \r\n\t U.START_TIME, V.AUDIT_STATUS, V.wf_status , r.FORM_CODE"
                + " \r\n\t FROM B_WF_INFO F "
                + " \r\n\t  INNER JOIN B_WF_TASK T ON F.UUID = T.WF_INFO_ID"
                + " \r\n\t  INNER JOIN B_WF_TASK_USER U ON T.UUID = U.WF_TASK_ID"
                + " \r\n\t  INNER JOIN B_SV_RECORD R ON F.RECORD_ID = R.UUID"
                + " \r\n\t  INNER JOIN B_SV_AUDIT_RECORD V ON R.UUID = V.SV_RECORD_ID"
                + " \r\n\t  WHERE 1 = 1 "

                + " \r\n\t AND F.RECORD_TYPE = '" + SignboardConstant.WF_TYPE_VALIDATION + "'"
                + " \r\n\t AND T.STATUS = '" + SignboardConstant.WF_STATUS_OPEN + "'"
                + " \r\n\t AND U.STATUS = '" + SignboardConstant.WF_STATUS_OPEN + "'"
                + " \r\n\t AND T.TASK_CODE IN('" + SignboardConstant.WF_MAP_AUDIT_TO + "', '" + SignboardConstant.WF_MAP_AUDIT_PO + "', '" + SignboardConstant.WF_MAP_AUDIT_SPO + "')"
                + " \r\n\t AND U.SYS_POST_ID = :userID";

            queryParameters.Add("userID", uuid);


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
    }
}