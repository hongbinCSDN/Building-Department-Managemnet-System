using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Entity;
using Oracle.ManagedDataAccess.Client;

namespace MWMS2.Services.ProcessingDAO.DAO
{
    public class P_MW_GENERAL_RECORD_DAOService : BaseDAOService
    {
        public List<P_MW_GENERAL_RECORD> GetMwGeneralRecordBySubmitType(string submitType)
        {
            string search_q = @"SELECT GR.*
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
            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":SUBMIT_TYPE",submitType)
            };
            return GetObjectData<P_MW_GENERAL_RECORD>(search_q, oracleParameters).ToList();
        }

        public DateTime GetMwGeneralRecordAssigmentDate(String key)
        {
            string search_q = @"SELECT T.CREATETIME
                                FROM   P_DSN_REQUESTID DS,
                                       P_PROCESSTRANSACTION A,
                                       P_WFTASK T
                                WHERE  DS.REQUESTID = A.REQUESTID
                                       AND A.INSTANCEID = T.INSTANCEID
                                       AND ( T.TASKNAME = 'ENQUIRY'
                                              OR T.TASKNAME = 'COMPLAINT HANDLING CHECK_LIST' )
                                       AND DS.DSN = :KEY
                                ORDER  BY T.TASKID ";

            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":KEY",key)
            };
            return GetObjectData<DateTime>(search_q, oracleParameters).FirstOrDefault();
        }
    }
}