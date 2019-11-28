using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services.Signborad.SignboardDAO;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.WorkFlow
{
    public class WorkFlowManagementService
    {
        public B_WF_INFO getWfInfoByKeyNumber(EntitiesSignboard db, string flowType, string keyNumber)
        {
            //using (EntitiesSignboard db = new EntitiesSignboard())
           // {
                return  db.B_WF_INFO.
                    Where(x => x.RECORD_TYPE == flowType && x.RECORD_ID == keyNumber).FirstOrDefault();

           // }
        }
        public B_WF_TASK getCurrentWfTask(EntitiesSignboard db, string flowType, string keyNumber)
        {
           // using (EntitiesSignboard db = new EntitiesSignboard())
           // {
                var query = from wfinfo in db.B_WF_INFO
                            join wftask in db.B_WF_TASK
                            on wfinfo.UUID equals wftask.WF_INFO_ID
                            where wfinfo.RECORD_TYPE == flowType
                            && wfinfo.RECORD_ID == keyNumber
                            && wftask.STATUS == SignboardConstant.WF_STATUS_OPEN
                            select wftask;
                return query.FirstOrDefault();
          //  }

        }
        public bool getAuditPercent(double probability)
        {
            if (probability > 100)
            {
                probability = 100;
            }
            Random r = new Random();
            int p = r.Next(2);
            double d = r.NextDouble();
            probability = probability / 100;
            /*   */
            if (d < probability)
                return true;
            else
            {
                if (d > probability)
                    return false;
                else
                {
                    //when random equaling probability,
                    //It has 50% chance to return 0.
                    if (p == 1)
                        return false;
                    else
                        return true;
                }
            }
        }



        public bool needValidationAudit(
             String flowType, String keyNumber, String currentStage, String direction)
        {
            bool result = true;
            double percent = 50;
           
            try
            {
                using (EntitiesSignboard db = new EntitiesSignboard())
                {
                    var q = db.B_S_AUDIT_CHECK_PERCENTAGE.Where(x => x.YEAR == DateTime.Now.Year);
                    if (q.Any())
                    {
                        percent = (double)q.FirstOrDefault().PERCENTAGE;
                      
                    }
                    else
                    {

                    }
                    result = getAuditPercent(percent);
                }



            }
            catch (Exception ex)
            {
            }
            return result;
        }


        private string getNextStage(EntitiesSignboard db,
                 string flowType, string keyNumber, string currentStage, string direction)
        {
           // using (EntitiesSignboard db = new EntitiesSignboard())
          //  {
                B_S_PARAMETER b_S_PARAMETER = db.B_S_PARAMETER.FirstOrDefault();
                bool needSPOVal = ApplicationConstant.DB_CHECKED.Equals(b_S_PARAMETER.SPO_VALIDATION);
                bool needSPOAud = ApplicationConstant.DB_CHECKED.Equals(b_S_PARAMETER.SPO_AUDIT);

            #region Audit
       
            bool needAudit = needValidationAudit(
                      flowType, keyNumber, currentStage, direction);

                string result = SignboardConstant.WF_GO_END;

                if (SignboardConstant.WF_GO_END.Equals(direction))
                {
                    return SignboardConstant.WF_MAP_END;
                }

                if (SignboardConstant.WF_MAP_VALIDATION_ISSUE_LETTER_TO.Equals(currentStage))
                {
                    return SignboardConstant.WF_MAP_VALIDATION_TO;
                }

                if (SignboardConstant.WF_MAP_VALIDATION_TO.Equals(currentStage))
                {
                    return SignboardConstant.WF_MAP_VALIDATION_PO;
                }

                if (SignboardConstant.WF_MAP_VALIDATION_PO.Equals(currentStage))
                {

                    if (SignboardConstant.WF_GO_BACK.Equals(direction))
                    {
                        return SignboardConstant.WF_MAP_VALIDATION_TO;
                    }


                    if (SignboardConstant.WF_GO_SPO.Equals(direction) || needSPOVal)
                    {
                        return SignboardConstant.WF_MAP_VALIDATION_SPO;
                    }


                    if (needAudit)
                    {
                        return SignboardConstant.WF_MAP_AUDIT_TO;
                    }
                    else
                    {
                        return SignboardConstant.WF_MAP_END;
                    }
                }
                if (SignboardConstant.WF_MAP_VALIDATION_SPO.Equals(currentStage))
                {
                    if (SignboardConstant.WF_GO_BACK.Equals(direction))
                    {
                        return SignboardConstant.WF_MAP_VALIDATION_PO;
                    }

                    if (needAudit)
                    {
                        return SignboardConstant.WF_MAP_AUDIT_TO;
                    }
                    else
                    {
                        return SignboardConstant.WF_MAP_END;
                    }
                }

                if (SignboardConstant.WF_MAP_AUDIT_TO.Equals(currentStage))
                {
                    return SignboardConstant.WF_MAP_AUDIT_PO;
                }

                if (SignboardConstant.WF_MAP_AUDIT_PO.Equals(currentStage))
                {

                    if (SignboardConstant.WF_GO_BACK.Equals(direction))
                    {
                        return SignboardConstant.WF_MAP_AUDIT_TO;
                    }

                    if (SignboardConstant.WF_GO_SPO.Equals(direction) || needSPOAud)
                    {
                        return SignboardConstant.WF_MAP_AUDIT_SPO;
                    }
                }
                if (SignboardConstant.WF_MAP_AUDIT_SPO.Equals(currentStage))
                {
                    if (SignboardConstant.WF_GO_BACK.Equals(direction))
                    {
                        return SignboardConstant.WF_MAP_AUDIT_PO;
                    }
                }


                if (SignboardConstant.WF_MAP_S24_SPO_APPROVE.Equals(currentStage))
                {
                    return SignboardConstant.WF_MAP_S24_PO;
                }

                if (SignboardConstant.WF_MAP_S24_PO.Equals(currentStage))
                {
                    return SignboardConstant.WF_MAP_S24_SPO_COMPILE;
                }

                if (SignboardConstant.WF_MAP_S24_SPO_COMPILE.Equals(currentStage))
                {
                    if (SignboardConstant.WF_GO_BACK.Equals(direction))
                    {
                        return SignboardConstant.WF_MAP_S24_PO;
                    }
                }

                if (SignboardConstant.WF_MAP_GC_SPO_APPROVE.Equals(currentStage))
                {
                    return SignboardConstant.WF_MAP_GC_PO;
                }

                if (SignboardConstant.WF_MAP_GC_PO.Equals(currentStage))
                {
                    return SignboardConstant.WF_MAP_GC_SPO_COMPILE;
                }

                if (SignboardConstant.WF_MAP_GC_SPO_COMPILE.Equals(currentStage))
                {
                    if (SignboardConstant.WF_GO_BACK.Equals(direction))
                    {
                        return SignboardConstant.WF_MAP_GC_PO;
                    }

                    if (SignboardConstant.WF_GO_NEXT.Equals(direction))
                    {
                        return SignboardConstant.WF_MAP_GC_PO_COMPLI;
                    }

                }
                return result;
            //}
            #endregion



        }

        public List<B_WF_TASK_USER> getCurrentWfTaskUser(EntitiesSignboard db, String flowType, String keyNumber)
        {

           // using (EntitiesSignboard db = new EntitiesSignboard())
            //{
                var TUquery = db.B_WF_TASK_USER.Where(x => x.STATUS == SignboardConstant.WF_STATUS_OPEN)
                                             .Where(y=>y.B_WF_TASK.STATUS == SignboardConstant.WF_STATUS_OPEN)
                                             .Where(z=>z.B_WF_TASK.B_WF_INFO.RECORD_TYPE==flowType
                                             && z.B_WF_TASK.B_WF_INFO.RECORD_ID==keyNumber)
                                             .Include(x => x.B_WF_TASK)
                                             .Include(x => x.B_WF_TASK.B_WF_INFO);

                return TUquery.ToList();

          //  }
               

                 //           String queryStr =
                 //                 " select u from WfInfo s, WfTask t, WfTaskUser u  " +
                 //                 " WHERE  t.wfInfo = s and t = u.wfTask and " +
                 //                 " s.recordType = :recordType and" +
                 //                 " s.recordId = :recordId and " +
                 //                 " t.status = :openStatus and " +
                 //                 " u.status = :openStatus  ";
                 //      Query query = session.createQuery(queryStr);


		
	}

        public List<string> getValidationUserList(EntitiesSignboard db,
                string flowType, string keyNumber, string wfMapStatus)
        {
            List<string> listString = new List<string>();
         //   using (EntitiesSignboard db = new EntitiesSignboard())
         //   {
                var query = db.B_SV_RECORD.Find(keyNumber);
                string toUser = query.TO_USER_ID;
                string poUser = query.PO_USER_ID;
                string spoUser = query.SPO_USER_ID;

                if (SignboardConstant.WF_MAP_VALIDATION_TO.Equals(wfMapStatus) ||
                   SignboardConstant.WF_MAP_AUDIT_TO.Equals(wfMapStatus) ||
                   SignboardConstant.WF_MAP_VALIDATION_ISSUE_LETTER_TO.Equals(wfMapStatus))
                {
                    listString.Add(toUser);
                }
                if (SignboardConstant.WF_MAP_VALIDATION_PO.Equals(wfMapStatus) ||
                   SignboardConstant.WF_MAP_AUDIT_PO.Equals(wfMapStatus))
                {
                    listString.Add(poUser);
                }
                if (SignboardConstant.WF_MAP_VALIDATION_SPO.Equals(wfMapStatus) ||
                   SignboardConstant.WF_MAP_AUDIT_SPO.Equals(wfMapStatus))
                {
                    listString.Add(spoUser);
                }
               

             //   listString.Add(SystemParameterConstant.WFUserUUID);

          //  }

            return listString;

        }
     




        public void saveOrUpdateToBD(EntitiesSignboard db, B_WF_INFO wfInfo, List<B_WF_TASK> wfTaskList, List<B_WF_TASK_USER> wfTaskUserList,string nextStage,string recordNumber )
        {

                if (wfInfo.UUID == null)
                {
                    db.B_WF_INFO.Add(wfInfo);
                }
                else
                {
                     db.Entry(wfInfo).State = EntityState.Modified;
                }
              
                for (int i = 0; i < wfTaskList.Count(); i++)
                {
                    B_WF_TASK wfTask = wfTaskList[i];
                    if (wfTask.UUID == null)
                    {
                        db.B_WF_TASK.Add(wfTask);
                    }
                    else
                    {

                          db.Entry(wfTask).State = EntityState.Modified;
                    }
                }
                for (int i = 0; i < wfTaskUserList.Count(); i++)
                {
                    B_WF_TASK_USER wfTaskUser = wfTaskUserList[i];
                    if (wfTaskUser.UUID == null)
                    {
                        db.B_WF_TASK_USER.Add(wfTaskUser);
                    }
                    else
                    {
                        db.Entry(wfTaskUser).State = EntityState.Modified;
                    }
                }

          

                var query = db.B_SV_RECORD.Where(x => x.UUID == recordNumber).FirstOrDefault();
                query.WF_STATUS = nextStage;
       
                //return new ServiceResult() { Result = ServiceResult.RESULT_FAILURE, Message = { ex.Message } };
            
        }
        public SYS_POST getAdminUserUUID()
        {
            SYS_POST admin = new SYS_POST();
            using (EntitiesAuth auth = new EntitiesAuth())
            {
                admin = auth.SYS_POST.Where(x => x.CODE == ApplicationConstant.ADMIN).FirstOrDefault();
            }

            return admin;
        }
        public string goNextWorkFlow(EntitiesSignboard db,
             string flowType, string keyNumber, string direction)
        {
               
                    B_WF_INFO wfInfo = new B_WF_INFO();
                    B_WF_TASK wfTask = new B_WF_TASK();
                    List<B_WF_TASK_USER> wfTaskUserList = new List<B_WF_TASK_USER>();

                    wfInfo = getWfInfoByKeyNumber(db, flowType, keyNumber);
                    wfTask = getCurrentWfTask(db, flowType, keyNumber);
                    wfTaskUserList = getCurrentWfTaskUser(db, flowType, keyNumber);
                    string currentStage = wfTask.TASK_CODE;
                    string nextStage = getNextStage(db, flowType,
                                keyNumber, currentStage, direction);

                    B_WF_INFO wfInfoUpdated = new B_WF_INFO();
                    List<B_WF_TASK> wfTaskUpdatedList = new List<B_WF_TASK>();
                    List<B_WF_TASK_USER> wfTaskUserUpdatedList = new List<B_WF_TASK_USER>();
                    DateTime currentDate = DateTime.Now;
                    if (SignboardConstant.WF_GO_END.Equals(nextStage))
                    {

                        wfInfo.CURRENT_STATUS = SignboardConstant.WF_STATUS_CLOSE;
                        wfInfoUpdated = wfInfo;
                        wfTask.STATUS = SignboardConstant.WF_STATUS_CLOSE;

                        wfTask.END_TIME = currentDate;

                        wfTaskUpdatedList.Add(wfTask);

                        for (int i = 0; i < wfTaskUserList.Count(); i++)
                        {
                            B_WF_TASK_USER wfTaskUser = wfTaskUserList[i];

                            //if (wfTaskUser.SYS_POST_ID.Equals(SystemParameterConstant.WFUserUUID))
                            if (wfTaskUser.SYS_POST_ID.Equals(SessionUtil.LoginPost.UUID))
                            {
                                wfTaskUser.STATUS = SignboardConstant.WF_STATUS_DONE;
                                wfTaskUser.ACTION_TIME = currentDate;
                            }
                            else
                            {
                                wfTaskUser.STATUS = SignboardConstant.WF_STATUS_CLOSE;
                            }

                        }
                        wfTaskUserUpdatedList.AddRange(wfTaskUserList);
                    }
                    else
                    {

                        wfInfoUpdated = wfInfo;

                        wfTask.STATUS = SignboardConstant.WF_STATUS_CLOSE;
                        wfTask.END_TIME = currentDate;

                        wfTaskUpdatedList.Add(wfTask);



                        B_WF_TASK wfTaskNew = new B_WF_TASK();

                        wfTaskNew.B_WF_INFO = wfInfo;

                        wfTaskNew.START_TIME = currentDate;
                        wfTaskNew.TASK_CODE = nextStage;
                        wfTaskNew.STATUS = SignboardConstant.WF_STATUS_OPEN;
                        wfTaskUpdatedList.Add(wfTaskNew);


                        for (int i = 0; i < wfTaskUserList.Count(); i++)
                        {
                            B_WF_TASK_USER wfTaskUser = wfTaskUserList[i];

                            // if (wfTaskUser.SYS_POST_ID.Equals(SystemParameterConstant.WFUserUUID))
                            if (wfTaskUser.SYS_POST_ID.Equals(SessionUtil.LoginPost.UUID))
                            {
                                wfTaskUser.STATUS = SignboardConstant.WF_STATUS_DONE;
                                wfTaskUser.ACTION_TIME = currentDate;
                            }
                            else
                            {
                                wfTaskUser.STATUS = SignboardConstant.WF_STATUS_CLOSE;
                            }

                        }
                        wfTaskUserUpdatedList.AddRange(wfTaskUserList);

                        List<string> jobAssignUser = new List<string>();
                        jobAssignUser.AddRange(getValidationUserList(db, flowType, keyNumber, nextStage));


                         //jobAssignUser.Add(getAdminUserUUID().UUID);
                         SignboardCommonDAOService dao = new SignboardCommonDAOService();
                        jobAssignUser.AddRange(dao.getSignboardAdminList());

                        for (int i = 0; i < jobAssignUser.Count(); i++)
                        {
                            string sUserAccount = jobAssignUser[i];
                            B_WF_TASK_USER wfTaskUser = new B_WF_TASK_USER();
                            wfTaskUser.B_WF_TASK = wfTaskNew;
                            wfTaskUser.SYS_POST_ID = sUserAccount;
                            wfTaskUser.STATUS = SignboardConstant.WF_STATUS_OPEN;
                            wfTaskUser.START_TIME = currentDate;


                            wfTaskUserUpdatedList.Add(wfTaskUser);

                        }
                    }
                    saveOrUpdateToBD(db, wfInfoUpdated, wfTaskUpdatedList, wfTaskUserUpdatedList, nextStage, keyNumber);
                
            
            return nextStage;



           // }




        }
        public void assignJobForValidation(string flowType, B_SV_RECORD KeyNumber)
        {
            //B_SV_RECORD svRecord = new B_SV_RECORD();
            //SvRecordDAOService svRecordDAOService = new SvRecordDAOService();
            //B_SV_RECORD record = svRecordDAOService.findById(KeyNumber);
            //    db.B_SV_RECORD.Where
            //       (o => o.UUID == KeyNumber).Include(o => o.B_SV_SUBMISSION).FirstOrDefault();
            assigmentUserToSVRecord(KeyNumber);
        }
        public void assigmentUserToSVRecord(B_SV_RECORD svMyRecord)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
             {
                SvRecordDAOService svRecordDAOService = new SvRecordDAOService();
                SvSubmissionDAOService svSubmissionDAOService = new SvSubmissionDAOService();
                string batchNumber = (from svs in db.B_SV_SUBMISSION
                                      where svs.UUID == svMyRecord.SV_SUBMISSION_ID
                                      select svs.BATCH_NO).FirstOrDefault();
                string svRecordUUID = svMyRecord.UUID;
                bool assigned = false;
                try
                {
                    if (!string.IsNullOrEmpty(batchNumber))
                    {
                        List<B_SV_RECORD> svRecordList = svRecordDAOService.getSVRecordByBatchNumber(batchNumber, svRecordUUID);
                        if (svRecordList != null)
                        {
                            for (int i = 0; i < svRecordList.Count(); i++)
                            {
                                B_SV_RECORD svRecord = svRecordList[i];
                                if (svRecord.SPO_ASSIGNMENT_DATE != null)
                                {
                                    svMyRecord.PO_USER_ID = svRecord.PO_USER_ID;
                                    svMyRecord.TO_USER_ID = svRecord.TO_USER_ID;
                                    svMyRecord.SPO_USER_ID = svRecord.SPO_USER_ID;
                                    assigned = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!assigned)
                    {
                        WorkFlowDAOService workFlowDAOService = new WorkFlowDAOService();
                        SParameterDAOService sParameter = new SParameterDAOService();
                        SScuTeamManagerService sScuTeamManager = new SScuTeamManagerService();
                        List<B_S_PARAMETER> SParameter = sParameter.FindByProperty();
                        int MAX_NUMBER_OF_JOB = Convert.ToInt32(SParameter[0].NUMBER_OF_JOB_ASSIGN);
                        B_WF_ASSIGNMENT wfAssignment = workFlowDAOService.getCurrentWfAssignment(SignboardConstant.WF_MAP_VALIDATION_TO);
                        List<B_S_SCU_TEAM> toUserList = sScuTeamManager.getWFUserList(SignboardConstant.S_USER_ACCOUNT_RANK_TO);

                        B_S_SCU_TEAM assignedTO = new B_S_SCU_TEAM();
                        B_S_SCU_TEAM assignedPO = new B_S_SCU_TEAM();
                        B_S_SCU_TEAM assignedSPO = new B_S_SCU_TEAM();


                        if (wfAssignment == null)
                        {
                            assignedTO = toUserList[0];
                            wfAssignment = new B_WF_ASSIGNMENT();
                            wfAssignment.NUMBER_OF_WORK = 1;
                            wfAssignment.CURRENT_USER_ID = assignedTO.CHILD_SYS_POST_ID;
                            wfAssignment.WF_MAP_CODE = SignboardConstant.WF_MAP_VALIDATION_TO;

                        }
                        else
                        {
                            string currentUserID = wfAssignment.CURRENT_USER_ID;
                            int currentNoOfJob = Convert.ToInt32(wfAssignment.NUMBER_OF_WORK);

                            string nextUserID = "";
                            for (int i = 0; i < toUserList.Count(); i++)
                            {
                                B_S_SCU_TEAM acc = toUserList[i];
                                if (currentUserID.Equals(acc.CHILD_SYS_POST_ID))
                                {
                                    if (i == toUserList.Count() - 1)
                                    {
                                        nextUserID = toUserList[0].CHILD_SYS_POST_ID;
                                    }
                                    else
                                    {
                                        nextUserID = toUserList[i+1].CHILD_SYS_POST_ID;
                                    }
                                    break;
                                }
                            }
                            if (nextUserID.Equals(""))
                            {
                                nextUserID = toUserList[0].CHILD_SYS_POST_ID;
                                wfAssignment.NUMBER_OF_WORK = 1;
                                wfAssignment.CURRENT_USER_ID = nextUserID;
                            }
                            else
                            {
                                if (currentNoOfJob < MAX_NUMBER_OF_JOB)
                                {
                                    wfAssignment.NUMBER_OF_WORK= wfAssignment.NUMBER_OF_WORK+1;
                                }
                                else
                                {
                                    wfAssignment.NUMBER_OF_WORK = 1;
                                    wfAssignment.CURRENT_USER_ID = nextUserID;
                                }
                            }

                        }

                        assignedTO = db.B_S_SCU_TEAM.Where(x => x.CHILD_SYS_POST_ID == wfAssignment.CURRENT_USER_ID).FirstOrDefault();
                        assignedPO = sScuTeamManager.getParents(assignedTO.CHILD_SYS_POST_ID); // TO's child --> PO
                        assignedSPO = sScuTeamManager.getParents(assignedPO.SYS_POST_ID); // PO
                        svMyRecord.TO_USER_ID = assignedTO.CHILD_SYS_POST_ID != null ? assignedTO.CHILD_SYS_POST_ID : null;
                        svMyRecord.PO_USER_ID = assignedPO.SYS_POST_ID != null ? assignedPO.SYS_POST_ID : null;
                        svMyRecord.SPO_USER_ID = assignedSPO.SYS_POST_ID != null ? assignedSPO.SYS_POST_ID : null;
                        
                        //    svMyRecord.setSUserAccountByToUserId(assignedTO);
                        //    svMyRecord.setSUserAccountByPoUserId(assignedPO);
                        //    svMyRecord.setSUserAccountBySpoUserId(assignedSPO);
                        //    setTracking(request, wfAssignment);
                        //    workFlowDAOManager.saveWfAssignment(wfAssignment, session);
                }
                
                db.SaveChanges();
                }
            catch (Exception e)
            {
                throw e;
            }
   
            }
        }
        

        public List<string> getUserList(EntitiesSignboard db, string flowType, string keyNumber, string wfMapStatus)
        {
            List<string> result = new List<string>();

            if(flowType.Equals(SignboardConstant.WF_TYPE_S24))
            {
                result.AddRange(getS24UserList(db, flowType, keyNumber, wfMapStatus));
            }
            if(flowType.Equals(SignboardConstant.WF_TYPE_GC))
            {
                result.AddRange(getGCUserList(db, flowType, keyNumber, wfMapStatus));
            }
            else
            {
                result.AddRange(getValidationUserList(db, flowType, keyNumber, wfMapStatus));
            }

            SignboardCommonDAOService dao = new SignboardCommonDAOService();
            List<string> admins = dao.getSignboardAdminList();

            result.AddRange(admins);

            return result;
        }

        public List<string> getS24UserList(EntitiesSignboard db, string flowType, string keyNumber, string wfMapStatus)
        {
            List<string> result = new List<string>();
            B_SV_24_ORDER sv24Order = db.B_SV_24_ORDER.Find(keyNumber);
            if(wfMapStatus.Equals(SignboardConstant.WF_MAP_S24_PO))
            {
                result.Add(sv24Order.TO_USER_ID);
                result.Add(sv24Order.PO_USER_ID);
            }
            if(wfMapStatus.Equals(SignboardConstant.WF_MAP_S24_SPO_APPROVE) || wfMapStatus.Equals(SignboardConstant.WF_MAP_S24_SPO_COMPILE))
            {
                result.Add(sv24Order.SPO_USER_ID);
            }

            return result;
        }
        public List<string> getGCUserList(EntitiesSignboard db, string flowType, string keyNumber, string wfMapStatus)
        {
            List<string> result = new List<string>();
            B_SV_GC svGc = db.B_SV_GC.Find(keyNumber);
            if(wfMapStatus.Equals(SignboardConstant.WF_MAP_GC_PO) || wfMapStatus.Equals(SignboardConstant.WF_MAP_GC_PO_COMPLI))
            {
                result.Add(svGc.TO_USER_ID);
                result.Add(svGc.PO_USER_ID);
            }
            if(wfMapStatus.Equals(SignboardConstant.WF_MAP_GC_SPO_APPROVE) || wfMapStatus.Equals(SignboardConstant.WF_MAP_GC_SPO_COMPILE))
            {
                result.Add(svGc.SPO_USER_ID);
            }
            return result;
        }


        public string startWorkFlow(EntitiesSignboard db,  string flowType, string keyNumber)
        {
          //  using(EntitiesSignboard db = new EntitiesSignboard())
         //   {
                string result = "";
                try
                {
                    B_WF_INFO wfInfo = new B_WF_INFO();
                    B_WF_TASK wfTask = new B_WF_TASK();
                    List<B_WF_TASK_USER> wfTaskUserList = new List<B_WF_TASK_USER>();

                    wfInfo.RECORD_ID = keyNumber;
                    wfInfo.RECORD_TYPE = flowType;
                    wfInfo.CURRENT_STATUS = SignboardConstant.WF_STATUS_OPEN;


                    List<string> jobAssignUser = new List<string>();

                    string startWork = "";
                    if (flowType.Equals(SignboardConstant.WF_TYPE_S24))
                    {
                        startWork = SignboardConstant.WF_MAP_S24_SPO_APPROVE;
                    }
                    else if (flowType.Equals(SignboardConstant.WF_TYPE_GC))
                    {
                        startWork = SignboardConstant.WF_MAP_GC_SPO_APPROVE;
                    }
                    else
                    {
                        startWork = SignboardConstant.WF_MAP_VALIDATION_START;
                    }

                    jobAssignUser.AddRange(getUserList(db, flowType, keyNumber, startWork));

                    wfTask.B_WF_INFO = wfInfo;
                    wfTask.START_TIME = System.DateTime.Now;
                    wfTask.TASK_CODE = startWork;
                    wfTask.STATUS = SignboardConstant.WF_STATUS_OPEN;



                    for (int i = 0; i < jobAssignUser.Count(); i++)
                    {
                        string sUserAccount = jobAssignUser[i];
                        B_WF_TASK_USER wfTaskUser = new B_WF_TASK_USER();
                        wfTaskUser.B_WF_TASK = wfTask; // new
                        wfTaskUser.SYS_POST_ID = sUserAccount;
                        wfTaskUser.STATUS = SignboardConstant.WF_STATUS_OPEN;
                        wfTaskUser.START_TIME = System.DateTime.Now;
                        //db.B_WF_TASK_USER.Add(wfTaskUser);
                        wfTaskUserList.Add(wfTaskUser);
                    }
                    List<B_WF_TASK> wfTaskList = new List<B_WF_TASK>();
                    wfTaskList.Add(wfTask);

                    saveOrUpdateToBD(db, wfInfo, wfTaskList, wfTaskUserList, startWork, keyNumber);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error :" + ex.Message);
                    throw ex;
                }
                return result;
           // }
        }

    }
}