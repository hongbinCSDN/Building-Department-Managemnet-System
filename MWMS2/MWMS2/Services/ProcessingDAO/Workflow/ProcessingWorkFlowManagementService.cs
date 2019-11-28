using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public enum DIRECTION { NEXT, BACK };
    class ProcessingWorkFlowUserNotFoundException : Exception
    {
        private static ProcessingWorkFlowUserNotFoundException _instance = new ProcessingWorkFlowUserNotFoundException();
        public static ProcessingWorkFlowUserNotFoundException Instance { get { return _instance; } }
    }
    public class ProcessingWorkFlowManagementService
    {
        private ProcessingWorkFlowManagementService() { }
        private static readonly object locker = new object();
        private static ProcessingWorkFlowManagementService instance = null;
        public static ProcessingWorkFlowManagementService Instance { get { if (instance == null) lock (locker) if (instance == null) instance = new ProcessingWorkFlowManagementService(); return instance; } }

        private static SysDAOService sysDAOService = null;
        public static SysDAOService SysDAOService { get { if (sysDAOService == null) sysDAOService = new SysDAOService(); return sysDAOService; } }


        public const string WF_TYPE_SUBMISSION = "WF_TYPE_SUBMISSION";
        public const string WF_TYPE_ENQ_COM = "WF_TYPE_ENQ_COM";

        public const string WF_SUBMISSION_TASK_VERIF_TO = "Verification-TO";
        public const string WF_SUBMISSION_TASK_VERIF_SPO = "Verification-SPO";
        public const string WF_SUBMISSION_TASK_ACKN_PO = "Acknowledgement-PO";
        public const string WF_SUBMISSION_TASK_ACKN_SPO = "Acknowledgement-SPO";

        public const string WF_SUBMISSION_TASK_VERIF_TO_SMM = "Verification-TO-SMM";
        public const string WF_SUBMISSION_TASK_VERIF_SPO_SMM = "Verification-SPO-SMM";
        public const string WF_SUBMISSION_TASK_ACKN_PO_SMM = "Acknowledgement-PO-SMM";
        public const string WF_SUBMISSION_TASK_ACKN_SPO_SMM = "Acknowledgement-SPO-SMM";

        public const string WF_COMPLAINT_TASK_ACKN_PO = "Complaint-PO";
        public const string WF_COMPLAINT_TASK_ACKN_SPO = "Complaint-SPO";
        public const string WF_ENQUIRY_TASK_ACKN_PO = "Enquiry-PO";
        public const string WF_ENQUIRY_TASK_ACKN_SPO = "Enquiry-SPO";


        public const string WF_STATUS_OPEN = "WF_STATUS_OPEN";
        public const string WF_STATUS_CLOSE = "WF_STATUS_CLOSE";
        public const string WF_STATUS_DONE = "WF_STATUS_DONE";

        public const string WF_GO_NEXT = "WF_GO_NEXT";
        public const string WF_GO_BACK = "WF_GO_BACK";
        public const string WF_GO_TASK_END = "WF_GO_TASK_END";
        public const string WF_GO_END = "WF_GO_END";

        public void ToNext(EntitiesMWProcessing db, string recordType, string recordID, string taskCode, string postId)
        { ToFlow(db, recordType, recordID, taskCode, DIRECTION.NEXT, postId); }

        public void ToBack(EntitiesMWProcessing db, string recordType, string recordID, string taskCode, string postId)
        { ToFlow(db, recordType, recordID, taskCode, DIRECTION.BACK, postId); }

        public void StartWorkFlowEnquiry(EntitiesMWProcessing db, P_MW_GENERAL_RECORD generalRecord, string refNumber)
        {
            string submitType = ProcessingConstant.MW_ENQUIRY.Equals(generalRecord.SUBMIT_TYPE) ? WF_ENQUIRY_TASK_ACKN_PO : WF_COMPLAINT_TASK_ACKN_PO;

            // for ICC > Receive general Submission
            if (ProcessingConstant.PREFIX_ICC.Equals(generalRecord.SUBMIT_TYPE))
            {
                submitType = ProcessingConstant.MW_ENQUIRY.Equals(generalRecord.ICC_TYPE) ? WF_ENQUIRY_TASK_ACKN_PO : WF_COMPLAINT_TASK_ACKN_PO;
            }


            GenerateWFUsers(db, refNumber, submitType, false, false);
            try
            {
                P_WF_INFO wfInfo = new P_WF_INFO
                {
                    RECORD_ID = generalRecord.UUID,
                    RECORD_TYPE = WF_TYPE_ENQ_COM,
                    CURRENT_STATUS = WF_STATUS_OPEN
                };
                db.P_WF_INFO.Add(wfInfo);
                List<string> wfTaskList = new List<string>();
                wfTaskList.Add(submitType);
                //wfTaskList.Add(WF_ENQUIRY_TASK_ACKN_PO);
                for (int j = 0; j < wfTaskList.Count(); j++)
                {
                    string wfTaskCode = wfTaskList[j];
                    P_WF_TASK wfTask = new P_WF_TASK();
                    wfTask.START_TIME = System.DateTime.Now;
                    wfTask.TASK_CODE = wfTaskCode;
                    wfTask.STATUS = WF_STATUS_OPEN;
                    wfInfo.P_WF_TASK.Add(wfTask);
                    List<string> jobAssignUser = GetUserList(db, refNumber, wfTaskCode);
                    for (int i = 0; i < jobAssignUser.Count; i++)
                    {
                        string sUserAccount = jobAssignUser[i];
                        wfTask.P_WF_TASK_USER.Add(new P_WF_TASK_USER
                        {
                            USER_ID = sUserAccount,
                            STATUS = WF_STATUS_OPEN,
                            START_TIME = System.DateTime.Now
                        });
                    }
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error :" + ex.Message);
                throw ex;
            }
        }

        public void StartWorkFlowEnquiryToOfficer(EntitiesMWProcessing db, P_MW_GENERAL_RECORD generalRecord, string refNumber, String sysPostCode)
        {
            string submitType = ProcessingConstant.MW_ENQUIRY.Equals(generalRecord.SUBMIT_TYPE) ? WF_ENQUIRY_TASK_ACKN_PO : WF_COMPLAINT_TASK_ACKN_PO;

            // for ICC > Receive general Submission
            if (ProcessingConstant.PREFIX_ICC.Equals(generalRecord.SUBMIT_TYPE))
            {
                submitType = ProcessingConstant.MW_ENQUIRY.Equals(generalRecord.ICC_TYPE) ? WF_ENQUIRY_TASK_ACKN_PO : WF_COMPLAINT_TASK_ACKN_PO;
            }

            try
            {
                P_WF_INFO wfInfo = new P_WF_INFO
                {
                    RECORD_ID = generalRecord.UUID,
                    RECORD_TYPE = WF_TYPE_ENQ_COM,
                    CURRENT_STATUS = WF_STATUS_OPEN
                };
                db.P_WF_INFO.Add(wfInfo);

                List<string> wfTaskList = new List<string>();
                wfTaskList.Add(submitType);

                for (int j = 0; j < wfTaskList.Count(); j++)
                {
                    string wfTaskCode = wfTaskList[j];
                    P_WF_TASK wfTask = new P_WF_TASK();
                    wfTask.START_TIME = System.DateTime.Now;
                    wfTask.TASK_CODE = wfTaskCode;
                    wfTask.STATUS = WF_STATUS_OPEN;
                    wfInfo.P_WF_TASK.Add(wfTask);

                    // get userId by sysPostCode
                    SYS_POST sysPost = SysDAOService.getSysPostByCode(sysPostCode);

                    string userId = sysPost.UUID;
                    wfTask.P_WF_TASK_USER.Add(new P_WF_TASK_USER
                    {
                        USER_ID = userId,
                        STATUS = WF_STATUS_OPEN,
                        START_TIME = System.DateTime.Now
                    });

                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error :" + ex.Message);
                throw ex;
            }
        }

        public void StartWorkFlowSubmission(EntitiesMWProcessing db, string keyNumber, bool SSP,
        bool mwItem, bool signboardItem)
        {
            //dsn == keyNumber
            string mwNo = db.P_MW_DSN.Where(o => o.DSN == keyNumber).Select(o => o.RECORD_ID).FirstOrDefault();
            GenerateWFUsers(db, mwNo, WF_SUBMISSION_TASK_VERIF_TO, SSP, signboardItem);
            if (!mwItem && !signboardItem) mwItem = true;
            try
            {
                P_WF_INFO wfInfo = new P_WF_INFO
                {
                    RECORD_ID = keyNumber,
                    RECORD_TYPE = WF_TYPE_SUBMISSION,
                    CURRENT_STATUS = WF_STATUS_OPEN
                };
                db.P_WF_INFO.Add(wfInfo);
                List<string> wfTaskList = new List<string>();
                if (mwItem)
                {
                    wfTaskList.Add(WF_SUBMISSION_TASK_VERIF_TO);
                    if (SSP) wfTaskList.Add(WF_SUBMISSION_TASK_VERIF_SPO);
                }
                if (signboardItem)
                {
                    wfTaskList.Add(WF_SUBMISSION_TASK_VERIF_TO_SMM);
                    if (SSP) wfTaskList.Add(WF_SUBMISSION_TASK_VERIF_SPO_SMM);
                }
                for (int j = 0; j < wfTaskList.Count(); j++)
                {
                    string wfTaskCode = wfTaskList[j];
                    P_WF_TASK wfTask = new P_WF_TASK();
                    //wfTask.P_WF_INFO = wfInfo;
                    wfTask.START_TIME = System.DateTime.Now;
                    wfTask.TASK_CODE = wfTaskCode;
                    wfTask.STATUS = WF_STATUS_OPEN;
                    db.P_WF_TASK.Add(wfTask);
                    wfInfo.P_WF_TASK.Add(wfTask);
                    List<string> jobAssignUser = GetUserList(db, keyNumber, wfTaskCode);
                    for (int i = 0; i < jobAssignUser.Count(); i++)
                    {
                        string sUserAccount = jobAssignUser[i];
                        db.P_WF_TASK_USER.Add(new P_WF_TASK_USER
                        {
                            P_WF_TASK = wfTask, // new
                            USER_ID = sUserAccount,
                            STATUS = WF_STATUS_OPEN,
                            START_TIME = System.DateTime.Now
                        });
                    }
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error :" + ex.Message);
                throw ex;
            }
        }

        private string LoadStage(string currentStage, DIRECTION direction)
        {
            return
                WF_SUBMISSION_TASK_VERIF_TO == currentStage && direction == DIRECTION.NEXT ? WF_SUBMISSION_TASK_ACKN_PO
                : WF_SUBMISSION_TASK_VERIF_SPO == currentStage && direction == DIRECTION.NEXT ? WF_GO_TASK_END
                : WF_SUBMISSION_TASK_ACKN_PO == currentStage && direction == DIRECTION.NEXT ? WF_SUBMISSION_TASK_ACKN_SPO
                : WF_SUBMISSION_TASK_ACKN_SPO == currentStage && direction == DIRECTION.NEXT ? WF_GO_TASK_END
                : WF_SUBMISSION_TASK_ACKN_SPO == currentStage && direction == DIRECTION.BACK ? WF_SUBMISSION_TASK_ACKN_PO
                : WF_SUBMISSION_TASK_ACKN_PO == currentStage && direction == DIRECTION.BACK ? WF_SUBMISSION_TASK_VERIF_TO

                : WF_SUBMISSION_TASK_VERIF_TO_SMM == currentStage && direction == DIRECTION.NEXT ? WF_SUBMISSION_TASK_ACKN_PO_SMM
                : WF_SUBMISSION_TASK_VERIF_SPO_SMM == currentStage && direction == DIRECTION.NEXT ? WF_GO_TASK_END
                : WF_SUBMISSION_TASK_ACKN_PO_SMM == currentStage && direction == DIRECTION.NEXT ? WF_SUBMISSION_TASK_ACKN_SPO_SMM
                : WF_SUBMISSION_TASK_ACKN_SPO_SMM == currentStage && direction == DIRECTION.NEXT ? WF_GO_TASK_END
                : WF_SUBMISSION_TASK_ACKN_SPO_SMM == currentStage && direction == DIRECTION.BACK ? WF_SUBMISSION_TASK_ACKN_PO_SMM
                : WF_SUBMISSION_TASK_ACKN_PO_SMM == currentStage && direction == DIRECTION.BACK ? WF_SUBMISSION_TASK_VERIF_TO_SMM



                : WF_ENQUIRY_TASK_ACKN_PO == currentStage && direction == DIRECTION.NEXT ? WF_ENQUIRY_TASK_ACKN_SPO
                : WF_ENQUIRY_TASK_ACKN_SPO == currentStage && direction == DIRECTION.NEXT ? WF_GO_TASK_END
                : WF_ENQUIRY_TASK_ACKN_SPO == currentStage && direction == DIRECTION.BACK ? WF_ENQUIRY_TASK_ACKN_PO

                : WF_COMPLAINT_TASK_ACKN_PO == currentStage && direction == DIRECTION.NEXT ? WF_COMPLAINT_TASK_ACKN_SPO
                : WF_COMPLAINT_TASK_ACKN_SPO == currentStage && direction == DIRECTION.NEXT ? WF_GO_TASK_END
                : WF_COMPLAINT_TASK_ACKN_SPO == currentStage && direction == DIRECTION.BACK ? WF_COMPLAINT_TASK_ACKN_PO
                : null;


            //Enq191002063
        }

        private void ToFlow(EntitiesMWProcessing db, string recordType, string recordID, string currentStatus, DIRECTION direction, string postId)
        {
            string mwNo;
            if (recordType == WF_TYPE_ENQ_COM)
            {
                string refUUID = db.P_MW_GENERAL_RECORD.Where(o => o.UUID == recordID).Select(o => o.REFERENCE_NUMBER).FirstOrDefault();
                mwNo = db.P_MW_REFERENCE_NO.Where(o => o.UUID == refUUID).Select(o => o.REFERENCE_NO).FirstOrDefault();
                //mwNo = recordID;
            }
            else mwNo = db.P_MW_DSN.Where(o => o.DSN == recordID).Select(o => o.RECORD_ID).FirstOrDefault();
            string nextStatus = LoadStage(currentStatus, direction);



            P_WF_INFO wfInfo = db.P_WF_INFO.Where(o => o.RECORD_TYPE == recordType).Where(o => o.RECORD_ID == recordID).FirstOrDefault();
            if (wfInfo != null)
            {
                P_WF_TASK wfTask = wfInfo.P_WF_TASK.Where(o => o.STATUS == WF_STATUS_OPEN).Where(o => o.TASK_CODE == currentStatus).FirstOrDefault();
                if (wfTask != null)
                {
                    IEnumerable<P_WF_TASK_USER> wfTaskUser = wfTask.P_WF_TASK_USER.Where(o => o.STATUS == WF_STATUS_OPEN);
                    if (wfTaskUser != null)
                    {
                        DateTime now = DateTime.Now;
                        wfTask.END_TIME = now;
                        wfTask.STATUS = WF_STATUS_CLOSE;
                        P_WF_TASK_USER u1 = wfTaskUser.Where(o => o.USER_ID == postId).FirstOrDefault();
                        List<P_WF_TASK_USER> u2 = wfTaskUser.Where(o => o.USER_ID != postId).ToList();
                        if (u1 != null)
                        {
                            u1.STATUS = WF_STATUS_DONE;
                            u1.ACTION_TIME = now;
                        }
                        u2.Select(o => { o.STATUS = WF_STATUS_CLOSE; return o; }).ToList();

                        if (WF_GO_END == nextStatus)
                        {
                            List<P_WF_TASK> P_WF_TASKCheckListk = wfInfo.P_WF_TASK
                                .Where(o => o.STATUS == WF_SUBMISSION_TASK_ACKN_SPO || o.STATUS == WF_SUBMISSION_TASK_ACKN_SPO_SMM).ToList();
                            bool allClose = true;
                            for (int i = 0; i < P_WF_TASKCheckListk.Count; i++)
                            {
                                if (P_WF_TASKCheckListk[i].STATUS != WF_STATUS_CLOSE) allClose = false;
                            }
                            if (allClose) wfInfo.CURRENT_STATUS = WF_STATUS_CLOSE;

                        }
                        else
                        {
                            P_WF_TASK wfTaskNew = new P_WF_TASK
                            {
                                START_TIME = now,
                                TASK_CODE = nextStatus,
                                STATUS = WF_STATUS_OPEN
                            };
                            wfInfo.P_WF_TASK.Add(wfTaskNew);
                            List<P_WF_TASK_USER> P_WF_TASK_USERNew = db.P_WF_TASKTOUSER
                                 .Where(o => o.ACTIVITY == nextStatus)
                                 .Where(o => o.MW_NUMBER == mwNo)
                                 .ToList()
                                 .Select(o => new P_WF_TASK_USER() { USER_ID = o.POST_CODE, START_TIME = now, STATUS = WF_STATUS_OPEN })
                                 .ToList();
                            foreach (P_WF_TASK_USER o in P_WF_TASK_USERNew) wfTaskNew.P_WF_TASK_USER.Add(o);
                        }
                    }

                }

            }

            db.SaveChanges();
        }



        private void GenerateWFUsers(EntitiesMWProcessing db, string keyNumber, string wfStatus, bool ssp, bool signboard)
        {
            using (EntitiesAuth dbAuth = new EntitiesAuth())
            {
                List<SYS_POST> SYS_POSTs = dbAuth.SYS_POST_ROLE.Where(o => o.SYS_ROLE.CODE == ProcessingConstant.AdministrationGroup).Select(o => o.SYS_POST).ToList();
                List<string> adminPost = SYS_POSTs == null ? null : SYS_POSTs.Select(o => o.UUID).ToList();


                P_WF_TASKTOUSER taskToUser = db.P_WF_TASKTOUSER.Where(o => o.MW_NUMBER == keyNumber && o.ACTIVITY == wfStatus).FirstOrDefault();
                if (taskToUser != null) return;


                P_WF_ROUND_ROBIN P_WF_ROUND_ROBIN = db.P_WF_ROUND_ROBIN.Where(o => o.WF_STATUS == wfStatus).FirstOrDefault();


                if (WF_SUBMISSION_TASK_VERIF_TO == wfStatus)
                {
                    string q1 = ""
                        + "\r\n\t" + " SELECT T1.*                                                          "
                        + "\r\n\t" + " FROM                                                                 "
                        + "\r\n\t" + " SYS_POST T1                                                          "
                        + "\r\n\t" + " INNER JOIN P_S_SCU_TEAM T2 ON T1.UUID = T2.CHILD_SYS_POST_ID         "
                        + "\r\n\t" + " INNER JOIN P_S_SCU_TEAM T3 ON T2.SYS_POST_ID = T3.CHILD_SYS_POST_ID  "
                        + "\r\n\t" + " INNER JOIN SYS_RANK T4 ON T1.SYS_RANK_ID = T4.UUID                   "
                        + "\r\n\t" + " WHERE T1.IS_ACTIVE=  'Y'     AND T4.RANK_GROUP = 'TO'                ";
                    SYS_POST pemSYS_POST, pemSYS_POST_PO, pemSYS_POST_SPO;
                    if (P_WF_ROUND_ROBIN == null)
                    {
                        pemSYS_POST = dbAuth.SYS_POST.SqlQuery(q1).FirstOrDefault();
                        if (pemSYS_POST == null) throw ProcessingWorkFlowUserNotFoundException.Instance;
                        db.P_WF_ROUND_ROBIN.Add(new P_WF_ROUND_ROBIN() { WF_STATUS = wfStatus, CURRENT_POST = pemSYS_POST.UUID });
                    }
                    else
                    {
                        string q2 = q1
                            + "\r\n\t" + " AND T1.CODE > (SELECT CODE FROM SYS_POST WHERE UUID = '" + P_WF_ROUND_ROBIN.CURRENT_POST + "'      )  ";
                        pemSYS_POST = dbAuth.SYS_POST.SqlQuery(q2).FirstOrDefault();
                        if (pemSYS_POST == null)
                        {
                            pemSYS_POST = dbAuth.SYS_POST.SqlQuery(q1).FirstOrDefault();
                            if (pemSYS_POST == null) throw ProcessingWorkFlowUserNotFoundException.Instance;
                        }
                        P_WF_ROUND_ROBIN.CURRENT_POST = pemSYS_POST.UUID;
                    }
                    pemSYS_POST_PO = dbAuth.SYS_POST.SqlQuery("SELECT * FROM SYS_POST WHERE UUID = (SELECT SYS_POST_ID FROM P_S_SCU_TEAM WHERE CHILD_SYS_POST_ID = '" + pemSYS_POST.UUID + "')").FirstOrDefault();
                    pemSYS_POST_SPO = dbAuth.SYS_POST.SqlQuery("SELECT * FROM SYS_POST WHERE UUID = (SELECT SYS_POST_ID FROM P_S_SCU_TEAM WHERE CHILD_SYS_POST_ID = '" + pemSYS_POST_PO.UUID + "')").FirstOrDefault();



                    AddWfToUser(db, WF_SUBMISSION_TASK_VERIF_TO, keyNumber, pemSYS_POST.UUID, adminPost);
                    if (ssp) AddWfToUser(db, WF_SUBMISSION_TASK_VERIF_SPO, keyNumber, pemSYS_POST_SPO.UUID, adminPost);
                    AddWfToUser(db, WF_SUBMISSION_TASK_ACKN_PO, keyNumber, pemSYS_POST_PO.UUID, adminPost);
                    AddWfToUser(db, WF_SUBMISSION_TASK_ACKN_SPO, keyNumber, pemSYS_POST_SPO.UUID, adminPost);

                    int areaId = -1;
                    if (signboard)
                    {
                        string q1SMM = ""
                            + "\r\n\t" + " SELECT T1.*                                                          "
                            + "\r\n\t" + " FROM                                                                 "
                            + "\r\n\t" + " SYS_POST T1                                                          "
                            + "\r\n\t" + " INNER JOIN B_S_SCU_TEAM T2 ON T1.UUID = T2.CHILD_SYS_POST_ID         "
                            + "\r\n\t" + " INNER JOIN B_S_SCU_TEAM T3 ON T2.SYS_POST_ID = T3.CHILD_SYS_POST_ID  "
                            + "\r\n\t" + " INNER JOIN SYS_RANK T4 ON T1.SYS_RANK_ID = T4.UUID                   "
                            + "\r\n\t" + " WHERE T1.IS_ACTIVE=  'Y'     AND T4.RANK_GROUP = 'TO'                ";
                        //string mwNumber = db.P_MW_DSN.Where(o => o.DSN == keyNumber).Select(o => o.RECORD_ID).FirstOrDefault();
                        P_MW_ADDRESS addresses = db.P_MW_RECORD.Where(o => o.P_MW_REFERENCE_NO.REFERENCE_NO == keyNumber).Select(o => o.P_MW_ADDRESS).FirstOrDefault();
                        if (addresses != null && addresses.BLOCK_ID != null)
                        {
                            int blkId = int.Parse(addresses.BLOCK_ID);
                            using (EntitiesAddress addressDB = new EntitiesAddress())
                            {
                                MWMS_BLK blk = addressDB.MWMS_BLK.Where(o => o.ADR_BLK_ID == blkId).FirstOrDefault();
                                if (blk != null) areaId = decimal.ToInt32(blk.AREA_ID.Value);
                            }
                        }


                        P_WF_ROUND_ROBIN smmP_WF_ROUND_ROBIN;
                        SYS_POST smmSYS_POST, smmSYS_POST_PO, smmSYS_POST_SPO;
                        //if (areaId != -1)
                        //{
                        smmP_WF_ROUND_ROBIN = db.P_WF_ROUND_ROBIN
                            .Where(o => o.WF_STATUS == WF_SUBMISSION_TASK_VERIF_TO_SMM)
                            .Where(o => o.AREA == areaId).FirstOrDefault();
                        // }
                        //else
                        // {
                        //     smmP_WF_ROUND_ROBIN = db.P_WF_ROUND_ROBIN
                        //         .Where(o => o.WF_STATUS == WF_SUBMISSION_TASK_VERIF_TO_SMM).FirstOrDefault();
                        // }
                        if (smmP_WF_ROUND_ROBIN == null)
                        {
                            //
                            // string q = ""
                            // + "\r\n\t" + " SELECT T1.*                                                          "
                            // + "\r\n\t" + " FROM                                                                 "
                            // + "\r\n\t" + " SYS_POST T1                                                          "
                            // + "\r\n\t" + " INNER JOIN B_S_SCU_TEAM T2 ON T1.UUID = T2.CHILD_SYS_POST_ID         "
                            // + "\r\n\t" + " INNER JOIN B_S_SCU_TEAM T3 ON T2.SYS_POST_ID = T3.CHILD_SYS_POST_ID  "
                            // + "\r\n\t" + " INNER JOIN SYS_RANK T4 ON T1.SYS_RANK_ID = T4.UUID                   "
                            // + "\r\n\t" + " INNER JOIN SYS_UNIT T5 ON T1.SYS_UNIT_ID = T5.UUID AND T5.CODE = 'SU' "
                            // + "\r\n\t" + " WHERE T1.IS_ACTIVE=  'Y' AND T4.RANK_GROUP = 'TO'                    ";



                            smmSYS_POST = dbAuth.SYS_POST.SqlQuery(q1).FirstOrDefault<SYS_POST>();
                            if (smmSYS_POST == null) throw new Exception("Signbroad Handling officers does not exists");
                            //if (areaId != -1) db.P_WF_ROUND_ROBIN.Add(new P_WF_ROUND_ROBIN() { WF_STATUS = WF_SUBMISSION_TASK_VERIF_TO_SMM, CURRENT_POST = smmSYS_POST.UUID });
                            db.P_WF_ROUND_ROBIN.Add(new P_WF_ROUND_ROBIN() { WF_STATUS = WF_SUBMISSION_TASK_VERIF_TO_SMM, CURRENT_POST = smmSYS_POST.UUID, AREA = areaId });

                        }
                        else
                        {

                            string q2 = q1SMM
                                + "\r\n\t" + " AND T1.CODE > (SELECT CODE FROM SYS_POST WHERE UUID = '" + smmP_WF_ROUND_ROBIN.CURRENT_POST + "'      )  ";
                            smmSYS_POST = dbAuth.SYS_POST.SqlQuery(q2).FirstOrDefault();
                            if (smmSYS_POST == null)
                            {
                                smmSYS_POST = dbAuth.SYS_POST.SqlQuery(q1SMM).FirstOrDefault();
                                if (smmSYS_POST == null) throw ProcessingWorkFlowUserNotFoundException.Instance;
                            }
                            smmP_WF_ROUND_ROBIN.CURRENT_POST = smmSYS_POST.UUID;




                            //string currentPost = P_WF_ROUND_ROBIN.CURRENT_POST;
                            //smmSYS_POST = dbAuth.SYS_POST
                            //   .Where(o => o.SYS_RANK.RANK_GROUP == "TO")
                            //   .Where(o => P_WF_ROUND_ROBIN == null || o.UUID.CompareTo(currentPost) > 0)
                            //   .Include(o => o.SYS_POST2.SYS_POST2)
                            //   .FirstOrDefault();
                            //P_WF_ROUND_ROBIN.CURRENT_POST = smmSYS_POST.UUID;
                        }
                        smmSYS_POST_PO = (dbAuth.SYS_POST.SqlQuery("SELECT * FROM SYS_POST WHERE UUID in (SELECT SYS_POST_ID FROM B_S_SCU_TEAM WHERE CHILD_SYS_POST_ID = '" + smmSYS_POST.UUID + "')").FirstOrDefault<SYS_POST>());
                        smmSYS_POST_SPO = (dbAuth.SYS_POST.SqlQuery("SELECT * FROM SYS_POST WHERE UUID in (SELECT SYS_POST_ID FROM B_S_SCU_TEAM WHERE CHILD_SYS_POST_ID = '" + smmSYS_POST_PO.UUID + "')").FirstOrDefault<SYS_POST>());
                        if (smmSYS_POST != null && smmSYS_POST_PO != null && smmSYS_POST_SPO != null)
                        {
                            AddWfToUser(db, WF_SUBMISSION_TASK_VERIF_TO_SMM, keyNumber, smmSYS_POST.UUID, adminPost);
                            if (ssp) AddWfToUser(db, WF_SUBMISSION_TASK_VERIF_SPO_SMM, keyNumber, smmSYS_POST_SPO.UUID, adminPost);
                            AddWfToUser(db, WF_SUBMISSION_TASK_ACKN_PO_SMM, keyNumber, smmSYS_POST_PO.UUID, adminPost);
                            AddWfToUser(db, WF_SUBMISSION_TASK_ACKN_SPO_SMM, keyNumber, smmSYS_POST_SPO.UUID, adminPost);
                        }
                    }
                }
                else if (WF_ENQUIRY_TASK_ACKN_PO == wfStatus || WF_COMPLAINT_TASK_ACKN_PO == wfStatus)
                {
                    string q1 = ""
                        + "\r\n\t" + " SELECT T1.*                                                          "
                        + "\r\n\t" + " FROM                                                                 "
                        + "\r\n\t" + " SYS_POST T1                                                          "
                        + "\r\n\t" + " INNER JOIN P_S_SCU_TEAM T2 ON T1.UUID = T2.CHILD_SYS_POST_ID         "
                        //+ "\r\n\t" + " INNER JOIN P_S_SCU_TEAM T3 ON T2.SYS_POST_ID = T3.CHILD_SYS_POST_ID  "
                        + "\r\n\t" + " INNER JOIN SYS_RANK T4 ON T1.SYS_RANK_ID = T4.UUID                   "
                        + "\r\n\t" + " WHERE T1.IS_ACTIVE=  'Y'     AND T4.RANK_GROUP = 'PO'                ";
                    SYS_POST pemSYS_POST_PO, pemSYS_POST_SPO;
                    if (P_WF_ROUND_ROBIN == null)
                    {
                        pemSYS_POST_PO = dbAuth.SYS_POST.SqlQuery(q1).FirstOrDefault();
                        db.P_WF_ROUND_ROBIN.Add(new P_WF_ROUND_ROBIN() { WF_STATUS = wfStatus, CURRENT_POST = pemSYS_POST_PO == null ? null : pemSYS_POST_PO.UUID });
                    }
                    else
                    {
                        string q2 = q1
                            + "\r\n\t" + " AND T1.CODE > (SELECT CODE FROM SYS_POST WHERE UUID = '" + P_WF_ROUND_ROBIN.CURRENT_POST + "'      )  ";
                        pemSYS_POST_PO = dbAuth.SYS_POST.SqlQuery(q2).FirstOrDefault();
                        if (pemSYS_POST_PO == null)
                        {
                            pemSYS_POST_PO = dbAuth.SYS_POST.SqlQuery(q1).FirstOrDefault();
                        }
                        P_WF_ROUND_ROBIN.CURRENT_POST = pemSYS_POST_PO == null ? null : pemSYS_POST_PO.UUID;
                    }
                    if (pemSYS_POST_PO != null)
                    {
                        pemSYS_POST_SPO = dbAuth.SYS_POST.SqlQuery("SELECT * FROM SYS_POST WHERE UUID IN (SELECT SYS_POST_ID FROM P_S_SCU_TEAM WHERE CHILD_SYS_POST_ID = '" + pemSYS_POST_PO.UUID + "')").FirstOrDefault();
                        if (wfStatus.StartsWith(ProcessingConstant.MW_ENQUIRY))
                        {
                            AddWfToUser(db, WF_ENQUIRY_TASK_ACKN_PO, keyNumber, pemSYS_POST_PO.UUID, adminPost);
                            AddWfToUser(db, WF_ENQUIRY_TASK_ACKN_SPO, keyNumber, pemSYS_POST_SPO.UUID, adminPost);
                        }
                        else if (wfStatus.StartsWith(ProcessingConstant.MW_COMPLAINT))
                        {
                            AddWfToUser(db, WF_COMPLAINT_TASK_ACKN_PO, keyNumber, pemSYS_POST_PO.UUID, adminPost);
                            AddWfToUser(db, WF_COMPLAINT_TASK_ACKN_SPO, keyNumber, pemSYS_POST_SPO.UUID, adminPost);
                        }

                    }


                }
                db.SaveChanges();
            }
        }
        private void AddWfToUser(EntitiesMWProcessing db, string activity, string mwNumber, string post, List<string> adminPost)
        {
            db.P_WF_TASKTOUSER.Add(new P_WF_TASKTOUSER() { ACTIVITY = activity, POST_CODE = post, MW_NUMBER = mwNumber });
            if (adminPost != null) adminPost.Select(o => db.P_WF_TASKTOUSER.Add(new P_WF_TASKTOUSER() { ACTIVITY = activity, POST_CODE = o, MW_NUMBER = mwNumber }));
        }

        private static List<string> GetUserList(EntitiesMWProcessing db, string keyNumber, string wfMapStatus)
        {
            List<P_WF_TASKTOUSER> taskToUser = null;
            if (wfMapStatus == WF_SUBMISSION_TASK_VERIF_TO
                || wfMapStatus == WF_SUBMISSION_TASK_VERIF_SPO
                || wfMapStatus == WF_SUBMISSION_TASK_ACKN_PO
                || wfMapStatus == WF_SUBMISSION_TASK_ACKN_SPO
               || wfMapStatus == WF_SUBMISSION_TASK_VERIF_TO_SMM
                || wfMapStatus == WF_SUBMISSION_TASK_VERIF_SPO_SMM
                || wfMapStatus == WF_SUBMISSION_TASK_ACKN_PO_SMM
                || wfMapStatus == WF_SUBMISSION_TASK_ACKN_SPO_SMM
                )
            {
                string mwNo = db.P_MW_DSN.Where(o => o.DSN == keyNumber).Select(o => o.RECORD_ID).FirstOrDefault();
                taskToUser = db.P_WF_TASKTOUSER.Where(o => o.MW_NUMBER == mwNo && o.ACTIVITY == wfMapStatus).ToList();

            }
            else if (wfMapStatus == WF_ENQUIRY_TASK_ACKN_PO)
            {
                taskToUser = db.P_WF_TASKTOUSER.Where(o => o.MW_NUMBER == keyNumber && o.ACTIVITY == wfMapStatus).ToList();
            }
            else if (wfMapStatus == WF_COMPLAINT_TASK_ACKN_PO)
            {
                taskToUser = db.P_WF_TASKTOUSER.Where(o => o.MW_NUMBER == keyNumber && o.ACTIVITY == wfMapStatus).ToList();
            }
            return taskToUser.Select(o => o.POST_CODE).ToList();
            //return taskToUser.Count == 0 ? null : taskToUser.Select(o => o.POST_CODE).ToList();
        }

        public P_WF_TASK GetCurrentTaskByRecordID(EntitiesMWProcessing db, string recordID)
        {
            //return db.P_WF_INFO.Where(w => w.RECORD_ID == recordID).FirstOrDefault().P_WF_TASK.Where(w => w.STATUS == ProcessingConstant.WF_STATUS_OPEN).FirstOrDefault();

            // Andy modify on 2019-Nov-04
            P_WF_TASK pWfTask = null;
            P_WF_INFO pWfInfo = db.P_WF_INFO.Where(w => w.RECORD_ID == recordID).FirstOrDefault();
            if (pWfInfo != null)
            {
                pWfTask = db.P_WF_INFO.Where(w => w.RECORD_ID == recordID).FirstOrDefault().P_WF_TASK.Where(w => w.STATUS == ProcessingConstant.WF_STATUS_OPEN).FirstOrDefault();
            }
            return pWfTask;
        }

        public P_WF_TASK GetCurrentTaskByRecordID(string recordID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_WF_TASK pWfTask = null;
                P_WF_INFO pWfInfo = db.P_WF_INFO.Where(w => w.RECORD_ID == recordID).FirstOrDefault();
                if (pWfInfo != null)
                {
                    pWfTask = db.P_WF_INFO.Where(w => w.RECORD_ID == recordID).FirstOrDefault().P_WF_TASK.Where(w => w.STATUS == ProcessingConstant.WF_STATUS_OPEN).FirstOrDefault();
                }
                return pWfTask;
            }
                
        }
    }
}