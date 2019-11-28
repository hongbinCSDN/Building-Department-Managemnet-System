using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Areas.MWProcessing.Models;

namespace MWMS2.Services.ProcessingDAO.DAO
{
    public class ProcessingACKCSDAOService
    {
        private const string SearchACKCS_whereq = @"SELECT *
                                                    FROM   (SELECT ( CASE
                                                                       WHEN V.STATUS_CODE = 'MW_VERT_STATUS_ROLLBACK' THEN 0
                                                                       WHEN V.STATUS_CODE = 'MW_VERT_STATUS_OPEN' THEN 1
                                                                       WHEN V.STATUS_CODE = 'MW_ACKN_STATUS_OPEN' THEN
                                                                         CASE
                                                                           WHEN V.SPO_ROLLBACK IS NULL THEN 2
                                                                           WHEN V.SPO_ROLLBACK = 'Y' THEN 3
                                                                           ELSE 4
                                                                         END
                                                                       ELSE 5
                                                                     END ) AS ORD,
                                                                   ( CASE
                                                                       WHEN V.STATUS_CODE = 'MW_VERT_STATUS_OPEN' THEN 'Verification'
                                                                       WHEN V.STATUS_CODE = 'MW_VERT_STATUS_ROLLBACK' THEN 'Rollbacked Verification'
                                                                       WHEN V.STATUS_CODE = 'MW_ACKN_STATUS_OPEN' THEN 'Acknowledgement'
                                                                       ELSE NULL
                                                                     END ) AS TASK,
                                                                   RN.REFERENCE_NO,
                                                                   R.S_FORM_TYPE_CODE,
                                                                   F.RECEIVED_DATE,
                                                                   R.MODIFIED_DATE,
                                                                   ( CASE
                                                                       WHEN V.STATUS_CODE = 'MW_VERT_STATUS_OPEN' THEN
                                                                         CASE
                                                                           WHEN FCHECK.UUID IS NULL THEN 'Open'
                                                                           ELSE 'In progress'
                                                                         END
                                                                       WHEN V.STATUS_CODE = 'MW_ACKN_STATUS_OPEN' THEN
                                                                         CASE
                                                                           WHEN 'Acknowledgement-SPO' = '123'--VWF.ACTIVITYID                                                                                                           
                                                                         THEN
                                                                             CASE
                                                                               WHEN SCHECK.MODIFIED_BY = SCHECK.CREATED_BY THEN 'Open'
                                                                               ELSE 'In progress'
                                                                             END
                                                                           ELSE
                                                                             CASE
                                                                               WHEN POCHECK.UUID IS NULL THEN 'Open'
                                                                               ELSE 'In progress'
                                                                             END
                                                                         END
                                                                       ELSE 'Open'
                                                                     END ) AS PROGRESS,
                                                                   R.UUID  AS R_UUID,
                                                                   V.UUID  AS V_UUID,
                                                                   R.MW_DSN,
                                                                   WT.TASK_CODE,
                                                                   WTU.USER_ID,
                                                                   R.MW_PROGRESS_STATUS_CODE,
                                                                   R.CLASS_CODE,
                                                                   commencement_submission_date
                                                            FROM   P_WF_INFO WI
                                                                   JOIN P_WF_TASK WT
                                                                     ON WI.UUID = WT.P_WF_INFO_ID
                                                                        AND WT.STATUS = 'WF_STATUS_OPEN'
                                                                   JOIN P_WF_TASK_User WTU
                                                                     ON WT.UUID = WTU.P_WF_TASK_ID
                                                                   --AND WTU.USER_ID = :USER_ID
                                                                   JOIN P_MW_RECORD R
                                                                     ON WI.RECORD_ID = R.MW_DSN
                                                                        AND IS_DATA_ENTRY = 'Y'
                                                                   JOIN P_MW_FORM F
                                                                     ON R.UUID = F.MW_RECORD_ID
                                                                   JOIN P_MW_REFERENCE_NO RN
                                                                     ON R.REFERENCE_NUMBER = RN.UUID
                                                                   JOIN P_MW_VERIFICATION V
                                                                     ON R.UUID = V.MW_RECORD_ID
                                                                        AND V.STATUS_CODE IN ( 'MW_ACKN_STATUS_OPEN' )
                                                                        AND Nvl(V.HANDLING_UNIT, 'PEM') = :handlingUnit
                                                                        AND ( CASE
                                                                                WHEN WT.TASK_CODE LIKE '%SMM' THEN 'SMM'
                                                                                ELSE 'PEM'
                                                                              END ) = Nvl(V.HANDLING_UNIT, 'PEM')
                                                                   LEFT JOIN P_MW_RECORD_FORM_CHECKLIST FCHECK
                                                                          ON R.UUID = FCHECK.MW_RECORD_ID
                                                                             AND Nvl(V.HANDLING_UNIT, 'PEM') = FCHECK.HANDLING_UNIT
                                                                   LEFT JOIN P_MW_RECORD_FORM_CHECKLIST_PO POCHECK
                                                                          ON FCHECK.UUID = POCHECK.MW_RECORD_FORM_CHECKLIST_ID
                                                                             AND Nvl(V.HANDLING_UNIT, 'PEM') = POCHECK.HANDLING_UNIT
                                                                   LEFT JOIN P_MW_SUMMARY_MW_ITEM_CHECKLIST SCHECK
                                                                          ON R.UUID = SCHECK.MW_RECORD_ID
                                                                             AND Nvl(V.HANDLING_UNIT, 'PEM') = SCHECK.HANDLING_UNIT
                                                            WHERE  WI.RECORD_TYPE = 'WF_TYPE_SUBMISSION'
                                                                   AND WI.CURRENT_STATUS = 'WF_STATUS_OPEN')
                                                    WHERE  1 = 1 ";
        public Fn06ACKCS_ACKCSModel Search(Fn06ACKCS_ACKCSModel model)
        {
            model.Query = SearchACKCS_whereq;
            model.Sort = "ORD, RECEIVED_DATE , REFERENCE_NO";
            model.Search();
            return model;
        }

        public string Excel(Fn06ACKCS_ACKCSModel model)
        {
            model.Query = SearchACKCS_whereq;
            return model.Export("Acknowledgement - Checklist Summary");
        }
    }
}