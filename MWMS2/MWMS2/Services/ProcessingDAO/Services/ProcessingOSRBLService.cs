using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class ProcessingOSRBLService
    {
        //ProcessingOSRDAOService
        private ProcessingOSRDAOService DAOService;
        protected ProcessingOSRDAOService DA
        {
            get { return DAOService ?? (DAOService = new ProcessingOSRDAOService()); }
        }
        private static string baseSql = "SELECT * FROM ";
        public void SubmissionsQ(Fn10RPT_OSRModel model)
        {
            if (model == null) return;
            model.Query = ""
                + "\r\n\t" + " SELECT      DISTINCT                                                                                                                                           "
                + "\r\n\t" + " CASE WHEN V.STATUS_CODE = 'MW_VERT_STATUS_ROLLBACK' THEN 0                                                                                             "
                + "\r\n\t" + " 	WHEN V.STATUS_CODE = 'MW_VERT_STATUS_OPEN' THEN      1                                                                                                "
                + "\r\n\t" + " 	WHEN V.STATUS_CODE = 'MW_ACKN_STATUS_OPEN' THEN                                                                                                       "
                + "\r\n\t" + " 	CASE WHEN V.SPO_ROLLBACK IS NULL THEN 2                                                                                                               "
                + "\r\n\t" + " 	WHEN V.SPO_ROLLBACK = 'Y' THEN 3                                                                                                                      "
                + "\r\n\t" + " 	ELSE 4 END                                                                                                                                            "
                + "\r\n\t" + " ELSE 5 END AS ORD                                                                                                                                      "
                + "\r\n\t" + "                                                                                                                                                        "
                + "\r\n\t" + " , CASE                                                                                                                                                 "
                //+ "\r\n\t" + " WHEN 'Acknowledgement-SPO' = VWF.ACTIVITYID AND V.SPO_ROLLBACK IS NOT NULL THEN 'Resubmitted Acknowledgement'                                          "
                //+ "\r\n\t" + " WHEN 'Acknowledgement-PO' = VWF.ACTIVITYID AND V.SPO_ROLLBACK IS NOT NULL AND V.PO_ROLLBACK IS NOT NULL THEN 'Resubmitted Acknowledgement'             "
                //+ "\r\n\t" + " WHEN 'Acknowledgement-PO' = VWF.ACTIVITYID AND V.SPO_ROLLBACK IS NOT NULL THEN 'Rollbacked Acknowledgement'                                            "
                //+ "\r\n\t" + " WHEN 'Acknowledgement-PO' = VWF.ACTIVITYID AND V.PO_ROLLBACK IS NOT NULL THEN 'Resubmitted Acknowledgement'                                            "
                + "\r\n\t" + " WHEN V.STATUS_CODE = 'MW_VERT_STATUS_OPEN' THEN 'Verification'                                                                                         "
                + "\r\n\t" + " WHEN V.STATUS_CODE = 'MW_VERT_STATUS_ROLLBACK' THEN 'Rollbacked Verification'                                                                          "
                + "\r\n\t" + " WHEN V.STATUS_CODE = 'MW_ACKN_STATUS_OPEN' THEN 'Acknowledgement'                                                                                      "
                + "\r\n\t" + " ELSE NULL END AS TASK                                                                                                                                  "
                + "\r\n\t" + "                                                                                                                                                        "
                + "\r\n\t" + " , RN.REFERENCE_NO                                                                                                                                      "
                + "\r\n\t" + " , R.S_FORM_TYPE_CODE                                                                                                                                   "
                + "\r\n\t" + " , F.RECEIVED_DATE                                                                                                                                      "
                + "\r\n\t" + " , R.MODIFIED_DATE                                                                                                                                      "
                + "\r\n\t" + " , CASE                                                                                                                                                 "
                + "\r\n\t" + " WHEN V.STATUS_CODE = 'MW_VERT_STATUS_OPEN'                                                                                                             "
                + "\r\n\t" + " THEN                                                                                                                                                   "
                + "\r\n\t" + " 	CASE                                                                                                                                                  "
                + "\r\n\t" + " 	WHEN FCHECK.UUID IS NULL                                                                                                                              "
                + "\r\n\t" + "     THEN 'Open' ELSE 'In progress' END                                                                                                                 "
                + "\r\n\t" + " WHEN V.STATUS_CODE = 'MW_ACKN_STATUS_OPEN'                                                                                                             "
                + "\r\n\t" + " THEN                                                                                                                                                   "
                + "\r\n\t" + " 	CASE                                                                                                                                                  "
                + "\r\n\t" + " 	WHEN 'Acknowledgement-SPO' = '123'--VWF.ACTIVITYID                                                                                                           "
                + "\r\n\t" + " 	THEN                                                                                                                                                  "
                + "\r\n\t" + " 		CASE                                                                                                                                              "
                + "\r\n\t" + " 		WHEN SCHECK.MODIFIED_BY = SCHECK.CREATED_BY                                                                                                       "
                + "\r\n\t" + " 		THEN 'Open'                                                                                                                                       "
                + "\r\n\t" + " 		ELSE 'In progress'                                                                                                                                "
                + "\r\n\t" + " 		END                                                                                                                                               "
                + "\r\n\t" + " 	ELSE                                                                                                                                                  "
                + "\r\n\t" + " 		CASE                                                                                                                                              "
                + "\r\n\t" + " 		WHEN POCHECK.UUID IS NULL                                                                                                                         "
                + "\r\n\t" + " 		THEN 'Open'                                                                                                                                       "
                + "\r\n\t" + " 		ELSE 'In progress'                                                                                                                                "
                + "\r\n\t" + " 		END                                                                                                                                               "
                + "\r\n\t" + " 	END                                                                                                                                                   "
                + "\r\n\t" + " ELSE 'Open'                                                                                                                                            "
                + "\r\n\t" + " END AS PROGRESS                                                                                                                                        "
       //         + "\r\n\t" + " , R.UUID AS R_UUID                                                                                                                                     "
               // + "\r\n\t" + " , V.UUID AS V_UUID                                                                                                                                     "
                + "\r\n\t" + " , R.MW_DSN                                                                                                                                             "
                + "\r\n\t" + " --, V.PO_ROLLBACK                                                                                                                                      "
                + "\r\n\t" + " --, CASE WHEN FCHECK.UUID IS NULL THEN 'Open' ELSE 'In progress' END AS VERIFICATION_STATUS                                                            "
                + "\r\n\t" + " --, CASE WHEN SCHECK.MODIFIED_BY = SCHECK.CREATED_BY THEN 'Open'  ELSE 'In progress' END AS ACKSPOSTATUS                                               "
                + "\r\n\t" + " --, CASE WHEN POCHECK.UUID IS NULL THEN 'Open'   ELSE 'In progress' END AS ACKPOSTATUS                                                                 "
                + "\r\n\t" + " FROM P_MW_VERIFICATION V                                                                                                                               "
                + "\r\n\t" + " INNER JOIN  P_MW_RECORD R ON  V.MW_RECORD_ID=R.UUID                                                                                                    "
                + "\r\n\t" + " INNER JOIN  P_MW_FORM F ON R.UUID = F.MW_RECORD_ID                                                                                                     "
                + "\r\n\t" + " INNER JOIN  P_MW_REFERENCE_NO RN ON  R.REFERENCE_NUMBER=RN.UUID                                                                                        "
                + "\r\n\t" + " LEFT OUTER JOIN P_MW_RECORD_FORM_CHECKLIST FCHECK ON R.UUID = FCHECK.MW_RECORD_ID                                                                      "
                + "\r\n\t" + " LEFT OUTER JOIN P_MW_RECORD_FORM_CHECKLIST_PO POCHECK ON  FCHECK.UUID = POCHECK.MW_RECORD_FORM_CHECKLIST_ID                                            "
                + "\r\n\t" + " LEFT OUTER JOIN P_MW_SUMMARY_MW_ITEM_CHECKLIST SCHECK ON R.UUID = SCHECK.MW_RECORD_ID                                                                  "
                //+ "\r\n\t" + " LEFT JOIN V_WF_ACK VWF ON VWF.DSN = R.MW_DSN AND V.STATUS_CODE IN ('MW_VERT_STATUS_OPEN' ,'MW_ACKN_STATUS_OPEN' )                                      "
                + "\r\n\t" + " WHERE (V.STATUS_CODE = 'MW_VERT_STATUS_OPEN'                                                                                                            "
                + "\r\n\t" + " OR V.STATUS_CODE =     'MW_ACKN_STATUS_OPEN'                                                                                                           "
                + "\r\n\t" + " OR V.STATUS_CODE = 'MW_VERT_STATUS_ROLLBACK')                                                                                                           "
                + "\r\n\t" + " AND SYSDATE - F.RECEIVED_DATE >= :dayDiff                                                                                                             "



            //string wf = ""
            + "\r\n\t" + " AND EXISTS (                                                                                   "
            + "\r\n\t" + " 	SELECT 1                                                                                       "
            + "\r\n\t" + " 	FROM P_WF_INFO T1                                                                              "
            + "\r\n\t" + " 	INNER JOIN P_WF_TASK T2 ON T2.P_WF_INFO_ID = T1.UUID                                           "
            + "\r\n\t" + " 	INNER JOIN P_WF_TASK_USER T3 ON T3.P_WF_TASK_ID = T2.UUID                                      "
            + "\r\n\t" + " 	WHERE T1.RECORD_TYPE = 'WF_TYPE_SUBMISSION'                                                    "
            + "\r\n\t" + " 	AND T1.CURRENT_STATUS = 'WF_STATUS_OPEN'                                                       "
            + "\r\n\t" + " 	AND R.MW_DSN = T1.RECORD_ID                                                                    "
           + "\r\n\t" + (string.IsNullOrWhiteSpace(model.Officer)?" ": "	AND T3.USER_ID = :officerUUID                  ")
            + "\r\n\t" + " 	AND (                                                                                          "
            + "\r\n\t" + " 		(V.STATUS_CODE = 'MW_VERT_STATUS_OPEN' AND T2.TASK_CODE = 'Verification-TO')               "
            + "\r\n\t" + " 		OR (V.STATUS_CODE = 'MW_VERT_STATUS_OPEN' AND T2.TASK_CODE = 'Verification-SPO')           "
            + "\r\n\t" + " 		OR (V.STATUS_CODE = 'MW_VERT_STATUS_ROLLBACK' AND T2.TASK_CODE = 'Verification-TO')        "
            + "\r\n\t" + " 		OR (V.STATUS_CODE = 'MW_ACKN_STATUS_OPEN' AND T2.TASK_CODE = 'Acknowledgement-PO')         "
            + "\r\n\t" + " 		OR (V.STATUS_CODE = 'MW_ACKN_STATUS_OPEN' AND T2.TASK_CODE = 'Acknowledgement-SPO')        "
            + "\r\n\t" + " 	)                                                                                              "
            + "\r\n\t" + " )                                                                                               ";

            
            
            model.Sort = "ORD, F.RECEIVED_DATE , RN.REFERENCE_NO";
        }

        public void Search(Fn10RPT_OSRModel model)
        {
            SubmissionsQ(model);
            if (string.IsNullOrEmpty(model.Day)) model.Day = "30";
            model.QueryParameters.Add("dayDiff", model.Day);
            if (!string.IsNullOrWhiteSpace(model.Officer)) model.QueryParameters.Add("officerUUID", model.Officer);
            model.Search();
        }
        public void Excel(Fn10RPT_OSRModel model)
        {
            SubmissionsQ(model);
            if (string.IsNullOrEmpty(model.Day)) model.Day = "30";
            model.QueryParameters.Add("dayDiff", model.Day);
            if (!string.IsNullOrWhiteSpace(model.Officer)) model.QueryParameters.Add("officerUUID", model.Officer);
            model.Export("MW Report");
        }
    }
}