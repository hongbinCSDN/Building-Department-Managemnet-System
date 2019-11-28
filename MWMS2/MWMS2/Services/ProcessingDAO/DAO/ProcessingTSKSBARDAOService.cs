using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Areas.MWProcessing.Models;

namespace MWMS2.Services.ProcessingDAO.DAO
{
    public class ProcessingTSKSBARDAOService
    {
        private const string searchSBAR_q = @"SELECT DISTINCT   D.UUID                                AS RECORD_ID,
                                                                MD.SUBMISSION_NATURE,
                                                                MD.DSN,
                                                                F.RECEIVED_DATE                     AS RECEIVED_DATE,
                                                                D.S_FORM_TYPE_CODE                  AS S_FORM_TYPE_CODE,
                                                                REF_NO.REFERENCE_NO,
                                                                AP.CERTIFICATION_NO                 AS AP,
                                                                PRC.CERTIFICATION_NO                AS PRC,
                                                                D.LOCATION_OF_MINOR_WORK            AS LOCATION_OF_MW,
                                                                ADDRESS.DISPLAY_STREET,
                                                                ADDRESS.DISPLAY_STREET_NO,
                                                                ADDRESS.DISPLAY_BUILDINGNAME,
                                                                ADDRESS.DISPLAY_FLOOR,
                                                                ADDRESS.DISPLAY_FLAT,
                                                                P_Get_item_code_by_record_id2(D.UUID) AS MW_ITEM_CODE,
                                                                D.VERIFICATION_SPO                  AS SSP,
                                                                D.COMMENCEMENT_DATE                 AS COMMENCEMENT_DATE,
                                                                D.COMPLETION_DATE                   AS COMPLETION_DATE,
                                                                P_Get_item_ref_by_record_id(D.UUID)   AS MW_ITEM_REF,
                                                                PAW.NAME_ENGLISH                    AS PAWNAME_ENGLISH,
                                                                CASE
                                                                  WHEN PAW.FAX_NO IS NULL THEN PAW_ADDRESS.ENGLISH_DISPLAY
                                                                END                                 AS PAWCONT,
                                                                OI.NAME_ENGLISH                     AS OINAME_ENGLISH,
                                                                CASE
                                                                  WHEN OI.FAX_NO IS NULL THEN OI_ADDRESS.ENGLISH_DISPLAY
                                                                END                                 AS OICONT,
                                                                SIC.REMARK,
                                                                SIC.SPO_ENDORSEMENT_DATE,
                                                                PRC.MW_NO,
                                                                D.FILEREF_FOUR
                                                                || '/'
                                                                || D.FILEREF_TWO                    AS fileRef,
                                                                CASE
                                                                  WHEN SIC.RECOMMEDATION_APPLICATION = 'O' THEN 'Acknowledged'
                                                                  WHEN SIC.RECOMMEDATION_APPLICATION = 'C' THEN 'Conditional'
                                                                  WHEN SIC.RECOMMEDATION_APPLICATION = 'N' THEN 'Refusal'
                                                                END                                 AS RECOMMEDATION,
                                                                CASE
                                                                  WHEN SIC.GROUNDS_OF_REFUSAL = 'Y' THEN 'Irregularities req site rectification'
                                                                  WHEN SIC.GROUNDS_OF_REFUSAL = 'N' THEN 'Irregularities not req site rectification'
                                                                  WHEN SIC.GROUNDS_OF_REFUSAL = 'W' THEN 'Withdrawn'
                                                                END                                 AS GROUNDS_OF_REFUSAL,
                                                                CASE
                                                                  WHEN SIC.GROUNDS_OF_CONDITIONAL = 'Y' THEN 'Irregularities req site rectification'
                                                                  WHEN SIC.GROUNDS_OF_CONDITIONAL = 'N' THEN 'Irregularities not req site rectification'
                                                                END                                 AS GROUNDS_OF_CONDITIONAL,
                                                                WF.V_TO,
                                                                WF.V_SPO,
                                                                WF.A_PO,
                                                                WF.A_SPO
                                                FROM   P_MW_RECORD D
                                                       INNER JOIN p_mw_record_item mwri
                                                               ON mwri.mw_record_id = d.uuid
                                                       INNER JOIN P_MW_VERIFICATION V
                                                               ON D.UUID = V.MW_RECORD_ID
                                                       INNER JOIN P_MW_REFERENCE_NO REF_NO
                                                               ON D.REFERENCE_NUMBER = REF_NO.UUID
                                                       INNER JOIN P_MW_FORM F
                                                               ON D.UUID = F.MW_RECORD_ID
                                                       INNER JOIN P_MW_RECORD FIN
                                                               ON FIN.IS_DATA_ENTRY = 'N'
                                                                  AND REF_NO.UUID = FIN.REFERENCE_NUMBER
                                                       LEFT OUTER JOIN P_MW_APPOINTED_PROFESSIONAL AP
                                                                    ON AP.IDENTIFY_FLAG = 'AP'
                                                                       AND FIN.UUID = AP.MW_RECORD_ID
                                                       LEFT OUTER JOIN P_MW_APPOINTED_PROFESSIONAL PRC
                                                                    ON PRC.IDENTIFY_FLAG = 'PRC'
                                                                       AND FIN.UUID = PRC.MW_RECORD_ID
                                                       LEFT JOIN P_MW_DSN MD
                                                              ON MD.RECORD_ID = REF_NO.REFERENCE_NO
                                                                 AND MD.FORM_CODE = D.S_FORM_TYPE_CODE
                                                                 AND D.MW_DSN = MD.DSN
                                                       LEFT JOIN P_MW_PERSON_CONTACT PAW
                                                              ON PAW.UUID = D.OWNER_ID
                                                       LEFT JOIN P_MW_ADDRESS PAW_ADDRESS
                                                              ON PAW.MW_ADDRESS_ID = PAW_ADDRESS.UUID
                                                       LEFT JOIN P_MW_PERSON_CONTACT OI
                                                              ON OI.UUID = D.OI_ID
                                                       LEFT JOIN P_MW_ADDRESS OI_ADDRESS
                                                              ON OI.MW_ADDRESS_ID = OI_ADDRESS.UUID
                                                       LEFT JOIN (SELECT Y.*
                                                                  FROM   P_MW_SUMMARY_MW_ITEM_CHECKLIST Y,
                                                                         (SELECT MW_RECORD_id,
                                                                                 Max(MODIFIED_DATE) AS MAXDATE
                                                                          FROM   P_MW_SUMMARY_MW_ITEM_CHECKLIST
                                                                          GROUP  BY MW_RECORD_id) N
                                                                  WHERE  Y.MW_RECORD_id = N.MW_RECORD_id
                                                                         AND Y.MODIFIED_DATE = N.MAXDATE) SIC
                                                              ON SIC.MW_RECORD_ID = D.UUID
                                                       LEFT JOIN P_MW_ADDRESS ADDRESS
                                                              ON D.LOCATION_ADDRESS_ID = ADDRESS.UUID
                                                       LEFT JOIN (SELECT   D,
                                                                           Max(V_TO)  AS V_TO,
                                                                           Max(V_SPO) AS V_SPO,
                                                                           Max(A_PO)  AS A_PO,
                                                                           Max(A_SPO) AS A_SPO
                                                                    FROM   (SELECT D,
                                                                                   CASE
                                                                                     WHEN T = 'Verification-TO' THEN 'AP'
                                                                                     ELSE ''
                                                                                   END AS V_TO,
                                                                                   CASE
                                                                                     WHEN T = 'Verification-SPO' THEN U
                                                                                     ELSE ''
                                                                                   END AS V_SPO,
                                                                                   CASE
                                                                                     WHEN T = 'Acknowledgement-PO' THEN 'SA'
                                                                                     ELSE ''
                                                                                   END AS A_PO,
                                                                                   CASE
                                                                                     WHEN T = 'Acknowledgement-SPO' THEN 'PM'
                                                                                     ELSE ''
                                                                                   END AS A_SPO
                                                                            FROM   (SELECT mwrecord.MW_DSN AS D,
                                                                                           wfwft.TASK_CODE AS T,
                                                                                           u.DSMS_USERNAME AS U
                                                                                    FROM   P_MW_RECORD mwrecord
                                                                                           INNER JOIN P_WF_INFO wfwft_info
                                                                                                   ON wfwft_info.record_id = mwrecord.MW_DSN
                                                                                           INNER JOIN P_WF_TASK wfwft
                                                                                                   ON wfwft.P_WF_INFO_ID = wfwft_info.uuid
                                                                                           INNER JOIN P_WF_TASK_USER wfwftu
                                                                                                   ON wfwftu.P_WF_TASK_ID = wfwft_info.uuid
                                                                                           INNER JOIN SYS_POST u
                                                                                                   ON wfwftu.user_id = u.uuid
                                                                                    WHERE  wfwft.TASK_CODE IN ( :taskNameList )))
                                                                  GROUP  BY D) WF
                                                              ON MD.DSN = WF.D
                                                WHERE  1 = 1
                                                       AND d.STATUS_CODE = :mwRecordStatusCode
                                                       AND V.STATUS_CODE = :mwAckResult
                                                       AND D.AUDIT_RELATED = :auditRelated ";

        public Fn03TSK_SBARModel Search(Fn03TSK_SBARModel model)
        {
            model.Query = searchSBAR_q;
            model.Search();
            return model;
        }

        public string Excel(Fn03TSK_SBARModel model)
        {
            string search_q = @"SELECT REF_NO.reference_no,
                                       P_get_item_code_by_record_id2(D.uuid)             ITEM_CODE,
                                       D.FILEREF_FOUR
                                       || '/'
                                       || D.FILEREF_TWO                                  FILE_REF_NO,
                                       ack.NATURE,
                                       ack.FORM_NO,
                                       ack.RECEIVED_DATE,
                                       Substr(ack.PRC_NO, 1, Instr(ack.PRC_NO, ' ') - 1) PRC,
                                       Substr(ack.PBP_NO, 1, Instr(ack.PBP_NO, ' ') - 1) PBP,
                                       ack.SSP,
                                       ack.STREET,
                                       ack.STREET_NO,
                                       ack.BUILDING,
                                       ack.FLOOR,
                                       ack.UNIT,
                                       ack.PAW,
                                       ack.PAW_CONTACT,
                                       ack.IO_MGT,
                                       ack.IO_MGT_CONTACT,
                                       ack.REMARK,
                                       ack.LETTER_DATE,
                                       taskStatus.VERIFICATIONTO,
                                       taskStatus.VERIFICATIONSPO,
                                       taskStatus.ACKNOWLEDGEMENTPO,
                                       taskStatus.ACKNOWLEDGEMENTSPO,
                                       'TBC'                                             OrderBDRef,
                                       'TBC'                                             PreviousRelate,
                                       'TBC'                                             ACKResult,
                                       D.*
                                FROM   P_MW_RECORD D
                                       INNER JOIN p_mw_record_item mwri
                                               ON mwri.mw_record_id = d.uuid
                                       INNER JOIN P_MW_VERIFICATION V
                                               ON D.UUID = V.MW_RECORD_ID
                                       INNER JOIN P_MW_REFERENCE_NO REF_NO
                                               ON D.REFERENCE_NUMBER = REF_NO.UUID
                                       INNER JOIN P_MW_FORM F
                                               ON D.UUID = F.MW_RECORD_ID
                                       INNER JOIN P_MW_RECORD FIN
                                               ON FIN.IS_DATA_ENTRY = 'N'
                                                  AND REF_NO.UUID = FIN.REFERENCE_NUMBER
                                       LEFT OUTER JOIN P_MW_APPOINTED_PROFESSIONAL AP
                                                    ON AP.IDENTIFY_FLAG = 'AP'
                                                       AND FIN.UUID = AP.MW_RECORD_ID
                                       LEFT OUTER JOIN P_MW_APPOINTED_PROFESSIONAL PRC
                                                    ON PRC.IDENTIFY_FLAG = 'PRC'
                                                       AND FIN.UUID = PRC.MW_RECORD_ID
                                       LEFT JOIN P_MW_DSN MD
                                              ON MD.RECORD_ID = REF_NO.REFERENCE_NO
                                                 AND MD.FORM_CODE = D.S_FORM_TYPE_CODE
                                                 AND D.MW_DSN = MD.DSN
                                       LEFT JOIN P_MW_PERSON_CONTACT PAW
                                              ON PAW.UUID = D.OWNER_ID
                                       LEFT JOIN P_MW_ADDRESS PAW_ADDRESS
                                              ON PAW.MW_ADDRESS_ID = PAW_ADDRESS.UUID
                                       LEFT JOIN P_MW_PERSON_CONTACT OI
                                              ON OI.UUID = D.OI_ID
                                       LEFT JOIN P_MW_ADDRESS OI_ADDRESS
                                              ON OI.MW_ADDRESS_ID = OI_ADDRESS.UUID
                                       LEFT JOIN (SELECT Y.*
                                                  FROM   P_MW_SUMMARY_MW_ITEM_CHECKLIST Y,
                                                         (SELECT MW_RECORD_id,
                                                                 Max(MODIFIED_DATE) AS MAXDATE
                                                          FROM   P_MW_SUMMARY_MW_ITEM_CHECKLIST
                                                          GROUP  BY MW_RECORD_id) N
                                                  WHERE  Y.MW_RECORD_id = N.MW_RECORD_id
                                                         AND Y.MODIFIED_DATE = N.MAXDATE) SIC
                                              ON SIC.MW_RECORD_ID = D.UUID
                                       LEFT JOIN P_MW_ADDRESS ADDRESS
                                              ON D.LOCATION_ADDRESS_ID = ADDRESS.UUID
                                       LEFT JOIN (SELECT D,
                                                         Max(V_TO)  AS V_TO,
                                                         Max(V_SPO) AS V_SPO,
                                                         Max(A_PO)  AS A_PO,
                                                         Max(A_SPO) AS A_SPO
                                                  FROM   (SELECT D,
                                                                 CASE
                                                                   WHEN T = 'Verification-TO' THEN 'AP'
                                                                   ELSE ''
                                                                 END AS V_TO,
                                                                 CASE
                                                                   WHEN T = 'Verification-SPO' THEN U
                                                                   ELSE ''
                                                                 END AS V_SPO,
                                                                 CASE
                                                                   WHEN T = 'Acknowledgement-PO' THEN 'SA'
                                                                   ELSE ''
                                                                 END AS A_PO,
                                                                 CASE
                                                                   WHEN T = 'Acknowledgement-SPO' THEN 'PM'
                                                                   ELSE ''
                                                                 END AS A_SPO
                                                          FROM   (SELECT mwrecord.MW_DSN AS D,
                                                                         wfwft.TASK_CODE AS T,
                                                                         u.DSMS_USERNAME AS U
                                                                  FROM   P_MW_RECORD mwrecord
                                                                         INNER JOIN P_WF_INFO wfwft_info
                                                                                 ON wfwft_info.record_id = mwrecord.MW_DSN
                                                                         INNER JOIN P_WF_TASK wfwft
                                                                                 ON wfwft.P_WF_INFO_ID = wfwft_info.uuid
                                                                         INNER JOIN P_WF_TASK_USER wfwftu
                                                                                 ON wfwftu.P_WF_TASK_ID = wfwft_info.uuid
                                                                         INNER JOIN SYS_POST u
                                                                                 ON wfwftu.user_id = u.uuid
                                                                  WHERE  wfwft.TASK_CODE IN ( :taskNameList )))
                                                  GROUP  BY D) WF
                                              ON MD.DSN = WF.D
                                       inner join P_MW_ACK_LETTER ack
                                            on ack.DSN=D.MW_DSN
                                       LEFT JOIN(SELECT *
                                                 FROM   (SELECT info.record_id,
                                                                task.TASK_CODE,
                                                                u.BD_PORTAL_LOGIN
                                                         FROM   P_WF_INFO info
                                                                INNER JOIN P_WF_TASK task
                                                                        ON task.P_WF_INFO_ID = info.uuid
                                                                INNER JOIN P_WF_TASK_USER taskU
                                                                        ON taskU.P_WF_TASK_ID = task.uuid
                                                                INNER JOIN SYS_POST u
                                                                        ON taskU.user_id = u.uuid)
                                                         pivot(sum(TASK_CODE) 
                                                                for TASK_CODE in 
                                                                (  'Verification-TO' VerificationTO
                                                                    ,'Verification-SPO' VerificationSPO
                                                                    ,'Acknowledgement-PO' AcknowledgementPO
                                                                    ,'Acknowledgement-SPO' AcknowledgementSPO))) taskStatus
                                              ON taskStatus.RECORD_ID = D.MW_DSN                                                                                                           WHERE  1 = 1
                                               AND d.STATUS_CODE = :mwRecordStatusCode
                                               AND V.STATUS_CODE = :mwAckResult
                                               AND D.AUDIT_RELATED = :auditRelated ";
            //model.QueryParameters = oracleParameters.ToDictionary(m => m.ParameterName.Trim(':'), m => m.Value);
            model.Search();
            return model.ExportCurrentData("Submission Search_" + DateTime.Now.ToString("yyyy-MM-dd"));
        }

        private static void CreateExcelColumns(Fn03TSK_SBARModel model)
        {
            Dictionary<string, string> col1 = GetExcelColumn("Screened Nature", "NATURE");
            Dictionary<string, string> col2 = GetExcelColumn("dsn", "MW_DSN");
            Dictionary<string, string> col3 = GetExcelColumn("Received Date", "RECEIVED_DATE");
            Dictionary<string, string> col4 = GetExcelColumn("Form type", "FORM_NO");
            Dictionary<string, string> col5 = GetExcelColumn("Ref No.", "REFERENCE_NO");
            Dictionary<string, string> col6 = GetExcelColumn("AP/RI reg. no. ", "PBP");
            Dictionary<string, string> col7 = GetExcelColumn("PRC reg. no. ", "PRC");
            Dictionary<string, string> col8 = GetExcelColumn("Location of Minor Work", "LOCATION_OF_MINOR_WORK");
            Dictionary<string, string> col9 = GetExcelColumn("Street", "STREET");
            Dictionary<string, string> col10 = GetExcelColumn("Street No.", "STREET_NO");
            Dictionary<string, string> col11 = GetExcelColumn("Building / Block", "BUILDING");
            Dictionary<string, string> col12 = GetExcelColumn("Floor", "FLOOR");
            Dictionary<string, string> col13 = GetExcelColumn("Unit", "UNIT");
            Dictionary<string, string> col14 = GetExcelColumn("Item No.", "ITEM_CODE");
            Dictionary<string, string> col15 = GetExcelColumn("SSP", "SSP");
            Dictionary<string, string> col16 = GetExcelColumn("Commencement Date", "COMMENCEMENT_DATE");
            Dictionary<string, string> col17 = GetExcelColumn("Completion Date", "COMPLETION_DATE");
            Dictionary<string, string> col18 = GetExcelColumn("Order/BD Ref.", "ORDERBDREF");
            Dictionary<string, string> col19 = GetExcelColumn("PAW", "PAW");
            Dictionary<string, string> col20 = GetExcelColumn("Contact of PAW", "PAW_CONTACT");
            Dictionary<string, string> col21 = GetExcelColumn("I.O. or Property Management", "IO_MGT");
            Dictionary<string, string> col22 = GetExcelColumn("Contact of I.O. or Property Management", "IO_MGT_CONTACT");
            Dictionary<string, string> col23 = GetExcelColumn("Remark", "REMARK");
            Dictionary<string, string> col24 = GetExcelColumn("Letter Date", "LETTER_DATE");
            Dictionary<string, string> col25 = GetExcelColumn("Previous Related MW No.", "PREVIOUSRELATE");
            Dictionary<string, string> col26 = GetExcelColumn("File Ref No", "FILE_REF_NO");
            Dictionary<string, string> col27 = GetExcelColumn("Acknowledgement Result", "ACKRESULT");
            Dictionary<string, string> col28 = GetExcelColumn("Verification TO", "VERIFICATIONTO");
            Dictionary<string, string> col29 = GetExcelColumn("Verification SPO", "VERIFICATIONSPO");
            Dictionary<string, string> col30 = GetExcelColumn("Acknowledgement PO", "ACKNOWLEDGEMENTPO");
            Dictionary<string, string> col31 = GetExcelColumn("Acknowledgement SPO", "ACKNOWLEDGEMENTSPO");

            model.Columns = new Dictionary<string, string>[]
            {
                col1,col2,col3,col4,col5
                ,col6,col7,col8,col9,col10
                ,col11,col12,col13,col14,col15
                ,col16,col17,col18,col19,col20
                ,col21,col22,col23,col24,col25
                ,col26,col27,col28,col29,col30
                ,col31
            };
        }

        private static Dictionary<string, string> GetExcelColumn(string displayName, string columnName)
        {
            Dictionary<string, string> col = new Dictionary<string, string>();
            col.Add("displayName", displayName);
            col.Add("columnName", columnName);
            return col;
        }
    }
}