using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class SignboardTDLDAOService
    {

        string Serach_TDL_IL_q = ""
               + "\r\n" + "\t" + "SELECT DISTINCT V.UUID AS VALIDATION_UUID, case when r.wf_status = 'WF_VALIDATION_LETTER_TO' then 'SO/TO Issue Letter' when r.wf_status='WF_VALIDATION_TO' then 'SO/TO Validation' when r.wf_status='WF_VALIDATION_PO' then 'TL Validation' when r.wf_status='WF_VALIDATION_SPO' then 'SPO Validation'   END as Task,R.REFERENCE_NO                                               "
               + "\r\n" + "\t" + ", R.RECEIVED_DATE, U.START_TIME,case when V.VALIDATION_STATUS='OPEN' then 'Open' when V.VALIDATION_STATUS='CLOSE' then 'Close'END as VALIDATION_STATUS, r.wf_status, r.FORM_CODE  , R.RECEIVED_DATE +7 AS TargetDate                              "
               + "\r\n" + "\t" + " FROM B_WF_INFO F  INNER JOIN B_WF_TASK T ON F.UUID = T.WF_INFO_ID INNER JOIN B_WF_TASK_USER U ON T.UUID = U.WF_TASK_ID                                     "
               + "\r\n" + "\t" + "  INNER JOIN B_SV_RECORD R ON F.RECORD_ID = R.UUID                                        "
               + "\r\n" + "\t" + " INNER JOIN B_SV_VALIDATION V ON R.UUID = V.SV_RECORD_ID                                      "
         
                + "\r\n" + "\t" + " where 1=1 ";
        string Serach_TDL_AUDIT_q = ""
               + "\r\n" + "\t" + "SELECT DISTINCT V.UUID AS AUDIT_UUID, case when V.wf_status = 'WF_AUDIT_TO' then 'SO/TO Audit' when V.wf_status='WF_AUDIT_PO' then 'TL Audit' when V.wf_status='WF_AUDIT_SPO' then 'SPO Audit'  END as Task,R.REFERENCE_NO                                               "
               + "\r\n" + "\t" + ", R.RECEIVED_DATE, U.START_TIME,V.AUDIT_STATUS,  V.wf_status, r.FORM_CODE  , R.RECEIVED_DATE +7 AS TargetDate                              "
               + "\r\n" + "\t" + " FROM B_WF_INFO F "
                + "\r\n" + "\t" + "INNER JOIN B_WF_TASK T ON F.UUID = T.WF_INFO_ID                                   "
                + "\r\n" + "\t" + "INNER JOIN B_WF_TASK_USER U ON T.UUID = U.WF_TASK_ID "
               + "\r\n" + "\t" + "INNER JOIN B_SV_RECORD R ON F.RECORD_ID = R.UUID                            "          
              + "\r\n" + "\t" + " INNER JOIN B_SV_AUDIT_RECORD V ON R.UUID = V.SV_RECORD_ID                                      "
               + "\r\n" + "\t" + " where 1=1 ";
        public Fn02TDL_TDLDisplayModel IssueLetterSearch(Fn02TDL_TDLDisplayModel model)
        {
            model.Query = Serach_TDL_IL_q;
            model.QueryWhere = SearchTDL_IL_whereQ(model);
            model.Search();
            return model;

        }

        public Fn02TDL_TDLDisplayModel ValidationSearch(Fn02TDL_TDLDisplayModel model)
        {
            model.Query = Serach_TDL_IL_q;
            model.QueryWhere = SearchTDL_VAD_whereQ(model);
            model.Search();



            return model;
        }
        public Fn02TDL_TDLDisplayModel AuditSearch(Fn02TDL_TDLDisplayModel model)
        {
            model.Query = Serach_TDL_AUDIT_q;
            model.QueryWhere = SearchTDL_AUDIT_whereQ(model);
            model.Search();



            return model;
        }
        public string SearchTDL_AUDIT_whereQ(Fn02TDL_TDLDisplayModel model)
        {
            string whereQ = "";

            whereQ += "\r\n\t" + " AND F.RECORD_TYPE = :RecodeType";
            model.QueryParameters.Add("RecodeType", SignboardConstant.WF_TYPE_VALIDATION);

            whereQ += "\r\n\t" + " AND  T.STATUS = :Status";
            model.QueryParameters.Add("Status", SignboardConstant.WF_STATUS_OPEN);
            whereQ += "\r\n\t" + " AND  U.STATUS = :Status1";
            model.QueryParameters.Add("Status1", SignboardConstant.WF_STATUS_OPEN);

             whereQ += "\r\n\t" + " AND  T.TASK_CODE IN (:taskCode) ";
            List<string> TaskList = new List<string>() { SignboardConstant.WF_MAP_AUDIT_PO, SignboardConstant.WF_MAP_AUDIT_SPO, SignboardConstant.WF_MAP_AUDIT_TO };
            model.QueryParameters.Add("taskCode", TaskList);

            //whereQ += "\r\n\t" + " AND  T.TASK_CODE IN (:taskCode1,:taskCode2,:taskCode3) ";
            //model.QueryParameters.Add("taskCode1", SignboardConstant.WF_MAP_AUDIT_PO);
            //model.QueryParameters.Add("taskCode2", SignboardConstant.WF_MAP_AUDIT_SPO);
            //model.QueryParameters.Add("taskCode3", SignboardConstant.WF_MAP_AUDIT_TO);

            whereQ += "\r\n\t" + " AND  U.SYS_POST_ID = :userID";
            // whereQ += "\r\n\t" + " AND  U.USER_ID  = :userID";
            model.QueryParameters.Add("userID", SessionUtil.LoginPost.UUID);



            return whereQ;

        }
        public string SearchTDL_VAD_whereQ(Fn02TDL_TDLDisplayModel model)
        {
            string whereQ = "";

            whereQ += "\r\n\t" + " AND F.RECORD_TYPE = :RecodeType";
            model.QueryParameters.Add("RecodeType", SignboardConstant.WF_TYPE_VALIDATION);

            whereQ += "\r\n\t" + " AND  T.STATUS = :Status";
            model.QueryParameters.Add("Status", SignboardConstant.WF_STATUS_OPEN);
            whereQ += "\r\n\t" + " AND  U.STATUS = :Status1";
            model.QueryParameters.Add("Status1", SignboardConstant.WF_STATUS_OPEN);

            whereQ += "\r\n\t" + " AND  T.TASK_CODE IN (:taskCode) ";
            List<string> TaskList = new List<string>() { SignboardConstant.WF_MAP_VALIDATION_TO, SignboardConstant.WF_MAP_VALIDATION_PO, SignboardConstant.WF_MAP_VALIDATION_SPO };
            model.QueryParameters.Add("taskCode", TaskList);

            //    whereQ += "\r\n\t" + " AND  U.USER_ID  = :userID";  if (wfTaskUser.SYS_POST_ID.Equals(SessionUtil.LoginPost.UUID))

            whereQ += "\r\n\t" + " AND  U.SYS_POST_ID = :userID";
           
              model.QueryParameters.Add("userID", SessionUtil.LoginPost.UUID);
            // model.QueryParameters.Add("userID", SystemParameterConstant.WFUserUUID);



            return whereQ;

        }
        public string SearchTDL_IL_whereQ(Fn02TDL_TDLDisplayModel model)
        {
            string whereQ = "";
       
            whereQ += "\r\n\t" + " AND F.RECORD_TYPE = :RecodeType";
            model.QueryParameters.Add("RecodeType", SignboardConstant.WF_TYPE_VALIDATION);

            whereQ += "\r\n\t" + " AND  T.STATUS = :Status";
            model.QueryParameters.Add("Status", SignboardConstant.WF_STATUS_OPEN);
            whereQ += "\r\n\t" + " AND  U.STATUS = :Status1";
            model.QueryParameters.Add("Status1", SignboardConstant.WF_STATUS_OPEN);
           
            whereQ += "\r\n\t" + " AND  T.TASK_CODE = :taskCode";
            model.QueryParameters.Add("taskCode", SignboardConstant.WF_MAP_VALIDATION_ISSUE_LETTER_TO);

            //whereQ += "\r\n\t" + " AND  U.USER_ID  = :userID";
            whereQ += "\r\n\t" + " AND  U.SYS_POST_ID = :userID"; 
           // model.QueryParameters.Add("userID", SystemParameterConstant.WFUserUUID);

            model.QueryParameters.Add("userID", SessionUtil.LoginPost.UUID);

            return whereQ;

        }
        public Fn02TDL_TDLDisplayModel GetTDLListCount()
        {
            


            return new Fn02TDL_TDLDisplayModel() { };

        }
    }
}