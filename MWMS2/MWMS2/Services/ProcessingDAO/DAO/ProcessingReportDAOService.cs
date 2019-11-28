using Microsoft.Ajax.Utilities;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MWMS2.Services.ProcessingDAO.DAO
{
    public class ProcessingReportDAOService
    {

        public Fn10RPT_PPJLModel GetPBPJobList(Fn10RPT_PPJLModel model)
        {
            model.Search();
            return model;
        }

        public Fn10RPT_MWMSWCCModel GetIncompletedRecord(Fn10RPT_MWMSWCCModel model)
        {
            string sql = @"SELECT DISTINCT t0.S_FORM_TYPE_CODE,
                                    t1.REFERENCE_NO,
                                    t0.COMMENCEMENT_DATE,
                                    CASE
                                      WHEN ( ( Extract(MONTH FROM SYSDATE) + ( Extract(YEAR FROM SYSDATE) * 12 ) ) - ( Extract(MONTH FROM t0.COMMENCEMENT_DATE) + ( Extract(YEAR FROM t0.COMMENCEMENT_DATE) * 12 ) ) + CASE
                                                                                                                                                                                                                         WHEN Extract(DAY FROM SYSDATE) - Extract(DAY FROM t0.COMMENCEMENT_DATE) > 0 THEN 1
                                                                                                                                                                                                                         ELSE 0
                                                                                                                                                                                                                       END > 12 ) THEN 13
                                      ELSE ( Extract(MONTH FROM SYSDATE) + ( Extract(YEAR FROM SYSDATE) * 12 ) ) - ( Extract(MONTH FROM t0.COMMENCEMENT_DATE) + ( Extract(YEAR FROM t0.COMMENCEMENT_DATE) * 12 ) ) + CASE
                                                                                                                                                                                                                       WHEN Extract(DAY FROM SYSDATE) - Extract(DAY FROM t0.COMMENCEMENT_DATE) > 0 THEN 1
                                                                                                                                                                                                                       ELSE 0
                                                                                                                                                                                                                     END
                                    END                                                      AS MONTH_DIFF,
                                    t4.RECEIVED_DATE,
                                    (SELECT Count(subR.UUID)
                                     FROM   P_MW_RECORD subR
                                     WHERE  subR.S_FORM_TYPE_CODE IN ( :MW0204CodeList )
                                            AND subR.REFERENCE_NUMBER = t0.REFERENCE_NUMBER) AS MW0204_COUNTER,
                                    (SELECT Count(subR.UUID)
                                     FROM   P_MW_RECORD subR,
                                            P_MW_SUMMARY_MW_ITEM_CHECKLIST C
                                     WHERE  subR.S_FORM_TYPE_CODE IN ( :MW0204CodeList )
                                            AND subR.REFERENCE_NUMBER = t0.REFERENCE_NUMBER
                                            AND subR.UUID = c.MW_RECORD_ID
                                            AND C.SPO_ENDORSEMENT_DATE IS NOT NULL
                                            AND C.RECOMMEDATION_APPLICATION = :ResultAck)    AS MW0204_OK_COUNTER,
                                    (SELECT Count(subR.UUID)
                                     FROM   P_MW_RECORD subR,
                                            P_MW_SUMMARY_MW_ITEM_CHECKLIST C
                                     WHERE  subR.S_FORM_TYPE_CODE IN ( :MW0204CodeList )
                                            AND subR.REFERENCE_NUMBER = t0.REFERENCE_NUMBER
                                            AND subR.UUID = c.MW_RECORD_ID
                                            AND C.SPO_ENDORSEMENT_DATE IS NOT NULL
                                            AND C.RECOMMEDATION_APPLICATION <> :ResultAck)   AS MW0204_NOT_OK_COUNTER
                    FROM   P_MW_RECORD t0
                           INNER JOIN P_MW_REFERENCE_NO t1
                                   ON t0.REFERENCE_NUMBER = t1.UUID
                           INNER JOIN P_MW_SUMMARY_MW_ITEM_CHECKLIST t2
                                   ON t0.UUID = t2.MW_RECORD_ID
                           INNER JOIN P_MW_APPOINTED_PROFESSIONAL t3PRC
                                   ON t0.UUID = t3PRC.MW_RECORD_ID
                           INNER JOIN P_MW_APPOINTED_PROFESSIONAL t3PBP
                                   ON t0.UUID = t3PBP.MW_RECORD_ID";

            model.Query = sql;
            model.Rpp = -1;
            model.Search();
            return model;
        }

        #region Fn10RPT_SLR
        public Fn10RPT_SLRModel Fn10RPT_SLRSearch(Fn10RPT_SLRModel model)
        {
            string search_q = @"SELECT R.MW_DSN MW_DSN,
                                       RN.REFERENCE_NO REFERENCE_NO,
                                       To_char(TASK.CREATED_DATE, 'yyyy-MM-dd') TASK_DATE,
                                       To_char(TASK.CREATED_DATE, 'HH24:MI:SS') TASK_TIME,
                                       TASK.TASK_CODE TASK_CODE,
                                       U.BD_PORTAL_LOGIN BD_PORTAL_LOGIN
                                FROM   P_MW_RECORD R
                                       INNER JOIN P_MW_REFERENCE_NO RN
                                               ON R.REFERENCE_NUMBER = RN.UUID AND R.IS_DATA_ENTRY = 'Y'
                                       INNER JOIN P_WF_INFO INFO
                                               ON INFO.RECORD_ID = R.MW_DSN And RECORD_TYPE = 'WF_TYPE_SUBMISSION'
                                       INNER JOIN P_WF_TASK TASK
                                               ON TASK.P_WF_INFO_ID = INFO.UUID 
                                               And TASK.TASK_CODE != 'WF_GO_TASK_END' 
                                               And TASK.STATUS = 'WF_STATUS_OPEN'
                                       INNER JOIN P_WF_TASK_USER TUSER
                                               ON TUSER.P_WF_TASK_ID = TASK.UUID
                                       LEft JOIN SYS_POST U
                                               ON TUSER.USER_ID = U.uuid
                                WHERE  1 = 1 ";

            model.Query = search_q;
            model.Search();
            return model;
        }
        #endregion

        #region Fn10RPT_SFMW
        public Fn10RPT_SFMWModel SearchSFMW(Fn10RPT_SFMWModel model)
        {
            model.Query = @"SELECT *
                            FROM   P_MW_ACK_LETTER
                            WHERE  Form_No IN ( 'MW01', 'MW03' ) ";

            //model.Query = @"SELECT *
            //                FROM   P_MW_ACK_LETTER ack
            //                       JOIN(SELECT *
            //                            FROM   (SELECT Count(MW_NO) cout,
            //                                           MW_NO
            //                                    FROM   P_MW_ACK_LETTER
            //                                    WHERE  Form_NO IN ( 'MW01', 'MW02', 'MW03', 'MW04' )
            //                                    GROUP  BY MW_NO)
            //                            WHERE  cout = 1) noCom
            //                         ON noCom.MW_NO = ack.MW_NO
            //                WHERE  Form_NO IN ( 'MW01', 'MW03' ) ";

            model.Search();
            return model;
        }
        #endregion

        #region ECPR
        public Fn10RPT_ECPRModel SearchEnquiryAndComplaintProgress(Fn10RPT_ECPRModel model)
        {
            model.Query = @"SELECT record.*,refNo.REference_no
                                FROM   P_Mw_General_Record record
                                inner join P_MW_REFERENCE_NO refNo
                                    on refNo.uuid=record.reference_number
                                WHERE  ( ( record.submit_Type = :submitType )
                                          OR ( record.submit_Type = 'ICC'
                                               AND record.icc_Type = :submitType ) ) ";
            model.QueryParameters.Add(":submitType", model.ReportType);
            model.QueryWhere = SearchEnquiryAndComplaintProgress_where(model);
            model.Sort = " record.receive_Date ";
            model.Search();
            return model;
        }
        public string SearchEnquiryAndComplaintProgress_where(Fn10RPT_ECPRModel model)
        {
            StringBuilder where_q = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(model.RelatedRefNo))
            {
                where_q.Append(@" AND refNo.reference_No LIKE :mwReferenceNo ");
                model.QueryParameters.Add(":mwReferenceNo", model.RelatedRefNo + "%");
            }
            if (!string.IsNullOrWhiteSpace(model.Status))
            {
                where_q.Append(@" AND record.status = :statusId ");
                model.QueryParameters.Add(":statusId", model.Status);
            }
            if (!string.IsNullOrWhiteSpace(model.ReceiptChannel))
            {
                where_q.Append(@" AND record.channel = :channel ");
                model.QueryParameters.Add(":channel", model.ReceiptChannel);
            }
            if (!string.IsNullOrWhiteSpace(model.ReceivedDateFrom))
            {
                where_q.Append(@" AND To_date(to_char(record.receive_Date,'dd/MM/yyyy'), 'dd/MM/yyyy') >= to_date(:receiveFromDate,'dd/MM/yyyy') ");
                model.QueryParameters.Add(":receiveFromDate", model.ReceivedDateFrom);
            }
            if (!string.IsNullOrWhiteSpace(model.ReceivedDateTo))
            {
                where_q.Append(@" AND To_date(to_char(record.receive_Date,'dd/MM/yyyy'), 'dd/MM/yyyy') <= to_date(:receiveToDate,'dd/MM/yyyy') ");
                model.QueryParameters.Add(":receiveToDate", model.ReceivedDateTo);
            }
            return where_q.ToString();
        }
        #endregion

        #region Non Complete Class II with item 2.15 and 2.34 Report
        public Fn10RPT_NCCWIModel SearchNCCWI(Fn10RPT_NCCWIModel model)
        {
            //model.Query = @"SELECT MW_DSN,
            //                       S_FORM_TYPE_CODE,
            //                       REFERENCE_NO,
            //                       RECEIVED_DATE,
            //                       COMMENCEMENT_SUBMISSION_DATE,
            //                       P_get_item_code_by_record_id2(mwrecord.uuid) ITEMCODE,
            //                       mwAddress.ADDRESS
            //                FROM   P_mw_record mwrecord
            //                       INNER JOIN P_MW_REFERENCE_NO refNo
            //                               ON refNo.uuid = mwrecord.REFERENCE_NUMBER
            //                       INNER JOIN P_MW_FORM mwForm
            //                               ON mwForm.mw_record_id = mwrecord.uuid
            //                       INNER JOIN (SELECT UUID,
            //                                          DISTRICT ADDRESS
            //                                   FROM   P_MW_ADDRESS) mwAddress
            //                               ON mwAddress.uuid = mwrecord.LOCATION_ADDRESS_ID
            //                       INNER JOIN P_MW_RECORD_ITEM items
            //                               ON items.mw_record_id = mwrecord.uuid
            //                WHERE  S_FORM_TYPE_CODE = 'MW03'
            //                       AND items.MW_ITEM_CODE IN ( '2.15', '2.34' )
            //                ";
            model.Query = @"SELECT R.UUID,
                                   R.LOCATION_ADDRESS_ID,
                                   R.MW_DSN,
                                   R.S_FORM_TYPE_CODE,
                                   RN.REFERENCE_NO,
                                   A.CHINESE_UNIT_NO||' '||A.DISPLAY_FLOOR||' '||A.DISPLAY_BUILDINGNAME
                                   ||' '||A.DISPLAY_STREET_NO||' '||A.DISPLAY_STREET  ADDRESS,
                                   F.RECEIVED_DATE,
                                   R.COMMENCEMENT_SUBMISSION_DATE,
                                   RI.MW_ITEM_CODE ITEMCODE
                            FROM   P_MW_RECORD R
                                   INNER JOIN P_MW_REFERENCE_NO RN
                                           ON R.REFERENCE_NUMBER = RN.UUID
                                   INNER JOIN P_MW_ADDRESS A
                                           ON R.LOCATION_ADDRESS_ID = A.uuid
                                   INNER JOIN P_MW_FORM F
                                           ON R.uuid = F.mw_record_id
                                   INNER JOIN (SELECT MW_RECORD_ID,  LISTAGG(MW_ITEM_CODE, ',')
                                                      within group(order by MW_ITEM_CODE) as MW_ITEM_CODE
                                               FROM   P_MW_RECORD_ITEM
                                               WHERE  MW_ITEM_CODE IN ( '2.15', '2.34' )
                                               GROUP  BY MW_RECORD_ID,
                                                         MW_ITEM_CODE)RI
                                           ON R.uuid = RI.MW_RECORD_ID
                            WHERE  R.S_FORM_TYPE_CODE = 'MW03' ";
            model.Search();
            return model;
        }


        #endregion
    }
}