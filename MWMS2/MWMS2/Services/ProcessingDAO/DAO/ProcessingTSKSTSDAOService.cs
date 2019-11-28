using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;

namespace MWMS2.Services.ProcessingDAO.DAO
{
    public class ProcessingTSKSTSDAOService
    {
        private const string SearchGRBySubmitType = @" SELECT GR.*
                                            FROM   P_MW_GENERAL_RECORD GR
                                                   LEFT JOIN P_MW_COMPLAINT_CHECKLIST CHECKLIST
                                                          ON CHECKLIST.RECORD_ID = GR.UUID
                                                   JOIN P_MW_REFERENCE_NO REF_NO
                                                     ON REF_NO.UUID = GR.REFERENCE_NUMBER
         
                                            WHERE  ( ( GR.SUBMIT_TYPE = :SUBMIT_TYPE )
                                                      OR ( GR.SUBMIT_TYPE = 'ICC'
                                                           AND GR.ICC_TYPE = :SUBMIT_TYPE ) )
                                                   AND ( GR.FORM_STATUS IN ( 'GENERAL RECORD NEW', 'GENERAL RECORD DRAFT' )
                                                          OR ( GR.FORM_STATUS = 'GENERAL RECORD COMPLETED'
                                                               AND CHECKLIST.FLOW_STATUS != 'DONE' ) ) ";

        public Fn03TSK_STSModel SearchEnquiryList(Fn03TSK_STSModel model)
        {
            model.Query = SearchGRBySubmitType;
            model.QueryParameters.Add("SUBMIT_TYPE", ProcessingConstant.SUBMIT_TYPE_ENQ);
            model.Search();

            return model;
        }

        public Fn03TSK_STSModel SearchComplaintList(Fn03TSK_STSModel model)
        {
            model.Query = SearchGRBySubmitType;
            model.QueryParameters.Add("SUBMIT_TYPE", ProcessingConstant.SUBMIT_TYPE_COM);
            model.Search();

            return model;
        }

        public Fn03TSK_STSModel SearchAuditList(Fn03TSK_STSModel model)
        {
            string search_q = @"SELECT AU.DSN,
                                       REF_NO.REFERENCE_NO,
                                       RECORD.S_FORM_TYPE_CODE,
                                       To_char(AU.MODIFIED_DATE, 'DD/MM/YYYY') ASSIGN_DATE,
                                       AU.AUDIT_STATUS,
                                       AU.UUID,
                                       RECORD.UUID                             AS RECROD_ID
                                FROM   P_MW_RECORD_AUDIT AU
                                       JOIN P_MW_RECORD RECORD
                                         ON RECORD.UUID = AU.MW_RECORD_ID
                                       JOIN P_MW_REFERENCE_NO REF_NO
                                         ON REF_NO.UUID = RECORD.REFERENCE_NUMBER
                                WHERE  au.audit_status in (:status) ";

            List<string> statuses = new List<string>()
            {
                ProcessingConstant.STATUS_OPEN
                ,ProcessingConstant.STATUS_IN_PROGRESS
                ,ProcessingConstant.STATUS_PO_COMPLETED
            };

            model.Query = search_q;
            model.QueryParameters.Add("status", statuses);
            model.Search();

            return model;
        }

        //public Fn03TSK_STSModel SearchSubmissionList(Fn03TSK_STSModel model, string statusCode)
        //{
        //    string search_q = @"SELECT V.UUID             AS V_UUID,
        //                               RN.REFERENCE_NO,
        //                               R.S_FORM_TYPE_CODE,
        //                               F.RECEIVED_DATE,
        //                               R.MODIFIED_DATE,
        //                               R.UUID             AS R_UUID,
        //                               R.MW_DSN,
        //                               FCHECK.UUID        AS CHECKLIST_UUID,
        //                               POCHECK.UUID       AS POCHECKLIST_UUID,
        //                               SCHECK.UUID        AS SUMCHECKLIST_UUID,
        //                               SCHECK.CREATED_BY  AS SUMCHECKLIST_CREATED_BY,
        //                               SCHECK.MODIFIED_BY AS SUMCHECKLIST_MODIFIED_BY,
        //                               V.PO_ROLLBACK
        //                        FROM   P_MW_VERIFICATION V
        //                               INNER JOIN P_MW_RECORD R
        //                                       ON V.MW_RECORD_ID = R.UUID
        //                                          AND V.STATUS_CODE = :STATUS_CODE
        //                               INNER JOIN P_MW_FORM F
        //                                       ON R.UUID = F.MW_RECORD_ID
        //                               INNER JOIN P_MW_REFERENCE_NO RN
        //                                       ON R.REFERENCE_NUMBER = RN.UUID
        //                               LEFT OUTER JOIN P_MW_RECORD_FORM_CHECKLIST FCHECK
        //                                            ON R.UUID = FCHECK.MW_RECORD_ID
        //                               LEFT OUTER JOIN P_MW_RECORD_FORM_CHECKLIST_PO POCHECK
        //                                            ON FCHECK.UUID = POCHECK.MW_RECORD_FORM_CHECKLIST_ID
        //                               LEFT OUTER JOIN P_MW_SUMMARY_MW_ITEM_CHECKLIST SCHECK
        //                                            ON R.UUID = SCHECK.MW_RECORD_ID
        //                        WHERE  1 = 1 ";
        //    if(statusCode==)
        //    model.Query = search_q;
        //    model.QueryParameters.Add("STATUS_CODE", statusCode);

        //}
    }
}