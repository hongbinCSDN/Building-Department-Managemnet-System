using MWMS2.Areas.Signboard.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services.Signborad.SignboardDAO;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MWMS2.Services.Signborad.WorkFlow;

namespace MWMS2.Services.Signborad.SignboardServices
{
    public class SignboardJobAssignmentService
    {
        private object sScuTeamManager;

        public Fn02TDL_JADisplayModel LoadSpoAssignment()
        {
            // check security right
            SignboardJobAssignmentDAOService SignboardJobAssignmentDAOService = new SignboardJobAssignmentDAOService();
            var resultList = PopulateSPOAssigmentList(false);
            var toList = SignboardJobAssignmentDAOService.GetScuTeamByPosition(SignboardConstant.S_USER_ACCOUNT_RANK_TO);
            var poList = SignboardJobAssignmentDAOService.GetScuTeamByPosition(SignboardConstant.S_USER_ACCOUNT_RANK_PO);
            var spoList = SignboardJobAssignmentDAOService.GetScuTeamByPosition(SignboardConstant.S_USER_ACCOUNT_RANK_SPO);
            var userList = PopulateCurrentJobList();
            Dictionary<string, string> toUser = new Dictionary<string, string>();
            Dictionary<string, string> poUser = new Dictionary<string, string>();
            Dictionary<string, string> spoUser = new Dictionary<string, string>();
            Dictionary<string, string> innerUuid = new Dictionary<string, string>();
            if (resultList != null && resultList.Count() > 0)
            {
                foreach(var item in resultList)
                {
                    // item.Uuid = B_SV_RECORD.UUID
                    toUser.Add(item.Uuid, item.ToUserID);
                    poUser.Add(item.Uuid, item.PoUserID);
                    spoUser.Add(item.Uuid, item.SpoUserID);
                    innerUuid.Add(item.Uuid, item.Uuid);
                }
            }            

            return new Fn02TDL_JADisplayModel
            {
                CurrentUserCountList = (userList != null && userList.Count() > 0 ? userList : null),
                ResultList = (resultList != null && resultList.Count() > 0) ? resultList : null,
                ToLookUpList = (toList != null && toList.Count() > 0) ? toList : null,
                PoLookUpList = (poList != null && poList.Count() > 0) ? poList : null,
                SpoLookUpList = (spoList != null && spoList.Count() > 0) ? spoList : null,
                ToUserId = (toUser != null && toUser.Count() > 0) ? toUser : null,
                PoUserId = (poUser != null && poUser.Count() > 0) ? poUser : null,
                SpoUserId = (spoUser != null && spoUser.Count() > 0) ? spoUser : null,
                UuidList = (innerUuid != null && innerUuid.Count() > 0 ? innerUuid : null),
            };
        }

        public List<JobAssignmentModel> PopulateSPOAssigmentList(bool OnlyAutoAssignment)
        {
            SignboardJobAssignmentDAOService SignboardJobAssignmentDAOService = new SignboardJobAssignmentDAOService();
            var objList = SignboardJobAssignmentDAOService.GetSPOAssignmentList();
            List<JobAssignmentModel> resultList = new List<JobAssignmentModel>();
            if (objList != null)
            {
                for(int i = 0; i < objList.Count; i++)
                {
                    JobAssignmentModel result = new JobAssignmentModel();
                    var objInnerList = objList[i];
                    result.Uuid = objInnerList[0].ToString();
                    result.FileRefNo = objInnerList[1].ToString();
                    result.ReceivedDate = objInnerList[2].ToString();
                    result.SignBoardDescription = objInnerList[3].ToString();
                    result.PawName = objInnerList[5].ToString();
                    result.ToUserID = objInnerList[6].ToString();
                    result.PoUserID = objInnerList[7].ToString();
                    result.SpoUserID = objInnerList[8].ToString();
                    result.SpoAssignment = objInnerList[9].ToString();
                    result.FormCode = objInnerList[10].ToString();

                    resultList.Add(result);
                }
            }
            return resultList;
        }

        public List<List<object>> PopulateCurrentJobList()
        {
            List<List<object>> list = new List<List<object>>();
            ReportDAOService ReportDAOService = new ReportDAOService();
            List<List<object>> objList = ReportDAOService.GetSubordinateTaskResult(SignboardConstant.WF_STATUS_OPEN, null, null);

            for(int i = 0; i < objList.Count; i++)
            {
                List<object> listItem = new List<object>();
                listItem = objList[i];
                listItem.Add(ReportDAOService.GetReportToScuJobResult((string)objList[i][0]));

                list.Add(listItem);
            }


            return list;
        }

        public void PopulateSaveJobList(Fn02TDL_JADisplayModel model)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                using (DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        for (var i = 0; i < model.ResultListSize; i++)
                        {
                            SvRecordDAOService SvRecordDAOService = new SvRecordDAOService();
                            B_SV_RECORD result = db.B_SV_RECORD.Find(model.UuidList.Keys.ElementAt(i));
                            string wfStatus = result.WF_STATUS;
                            if (wfStatus == null || !wfStatus.Equals(SignboardConstant.WF_MAP_ASSIGING))
                            {
                                continue;
                            }
                            if (result.SPO_ASSIGNMENT_DATE != null)
                            {
                                continue;
                            }
                            result.TO_USER_ID = model.ToUserId[result.UUID];
                            result.PO_USER_ID = model.PoUserId[result.UUID];
                            result.SPO_USER_ID = model.SpoUserId[result.UUID];
                            db.SaveChanges();

                            if (model.SaveMode.Equals(SignboardConstant.SUBMIT_MODE))
                            {
                                result.ASSIGNED_BY = SystemParameterConstant.UserName;
                                result.SPO_ASSIGNMENT_DATE = System.DateTime.Now;
                                result.WF_STATUS = SignboardConstant.WF_MAP_VALIDATION_START;

                                B_SV_VALIDATION svValidation = db.B_SV_VALIDATION.Where(x => x.SV_RECORD_ID == result.UUID).FirstOrDefault();
                                svValidation.VALIDATION_STATUS = SignboardConstant.SV_VALIDATION_STATUS_CODE_OPEN;
                                db.SaveChanges();
                            }
                            if (model.SaveMode.Equals(SignboardConstant.SUBMIT_MODE))
                            {
                                WorkFlowManagementService WorkFlowManagementService = new WorkFlowManagementService();
                                WorkFlowManagementService.startWorkFlow(db, SignboardConstant.WF_TYPE_VALIDATION, result.UUID);
                                db.SaveChanges();
                            }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error :" + ex.Message);
                    }
                }
            }
        }

    }
}