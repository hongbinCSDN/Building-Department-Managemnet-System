using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class ProcessingPcsaDAOService : BaseDAOService
    {
        private const string searchPcsSql = @"SELECT L.DSN,
                                                       To_char(L.RECEIVED_DATE, 'yyyy-MM-dd')   AS RECEIVED_DATE,
                                                       L.MW_NO,
                                                       L.FORM_NO,
                                                       L.ADDRESS,
                                                       A.HANDLING_OFFICER,
                                                       L.PRC_NO,
                                                       L.ITEM_DISPLAY,
                                                       To_char(A.INSPECTION_DATE, 'yyyy-MM-dd') AS INSPECTION_DATE,
                                                       A.PA_RESULT,
                                                       A.PA_REMARK,
                                                       To_char(A.SELECTION_DATE, 'yyyy-MM-dd')  AS SELECTION_DATE,
                                                       A.UUID
                                                FROM   P_MW_ACK_LETTER L,
                                                       P_MW_ACK_LETTER_PREAUDIT A
                                                WHERE  L.UUID = A.MW_ACK_LETTER_ID ";



        public string GetSearchCriteria(Fn01LM_PcsaSearchModel model)
        {
            string whereQ = "";

            if (model.SelectionDateFrom != null)
            {
                whereQ += "\r\n\t" + "AND to_date(A.SELECTION_DATE)" + " >= :SelectionDateFrom";
                model.QueryParameters.Add("SelectionDateFrom", model.SelectionDateFrom);
            }
            if (model.SelectionDateTo != null)
            {
                whereQ += "\r\n\t" + "AND to_date(A.SELECTION_DATE)" + " <= :SelectionDateTo";
                model.QueryParameters.Add("SelectionDateTo", model.SelectionDateTo);
            }

            if (model.IsGeneral)
            {
                whereQ = "";
            }

            if (!string.IsNullOrWhiteSpace(model.DSN))
            {
                whereQ += "\r\n\t" + "AND L.DSN = :DSN";
                model.QueryParameters.Add("DSN", model.DSN.Trim());
            }
            if (!string.IsNullOrWhiteSpace(model.RefNo))
            {
                whereQ += "\r\n\t" + "AND L.MW_NO = :RefNo";
                model.QueryParameters.Add("RefNo", model.RefNo.Trim());
            }

           

            return whereQ;
        }

        /// <summary>
        /// Search Pcs
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public DisplayGrid SearchPcsa(Fn01LM_PcsaSearchModel model)
        {
            model.Query = searchPcsSql;
            model.QueryWhere = GetSearchCriteria(model);
            model.Search();
            return model;
        }

        public string ExportPcsa(Fn01LM_PcsaSearchModel model)
        {
            model.Query = searchPcsSql;
            model.QueryWhere = GetSearchCriteria(model);
            return model.Export("Pre-commencement Site Audit List");
        }

        public string GetRandomPsac(Fn01LM_PcsaSearchModel model)
        {
            string sql = @"SELECT UUID
                            FROM   (SELECT R.UUID
                                    FROM   (SELECT *
                                            FROM   P_MW_ACK_LETTER
                                            WHERE  RECEIVED_DATE IS NOT NULL
                                                   AND UUID NOT IN (SELECT MW_ACK_LETTER_ID
                                                                    FROM   P_MW_ACK_LETTER_PREAUDIT)) R
                                    WHERE  To_date(R.RECEIVED_DATE) >= To_date(:SelectionDateFrom,'dd/MM/yyyy')
                                           AND To_date(R.RECEIVED_DATE) <= To_date(:SelectionDateTo,'dd/MM/yyyy')
                                           AND R.NATURE IN ( 'SUBMISSION','E-SUBMISSION' )
                                           AND R.FORM_NO IN ( 'MW01','MW03' )
                                           AND R.AUDIT_RELATED = 'Y'
                                    ORDER  BY DBMS_RANDOM.value()) A
                            WHERE  ROWNUM <= 1    ";

            OracleParameter[] oracleParameters = new OracleParameter[]
            {
                new OracleParameter(":SelectionDateFrom",model.SelectionDateFrom==null?"01/01/0001":model.SelectionDateFrom.Value.ToString("dd/MM/yyyy")),
                new OracleParameter(":SelectionDateTo",model.SelectionDateTo==null?"01/01/0001":model.SelectionDateTo.Value.ToString("dd/MM/yyyy"))
            };

            return GetObjectData<string>(sql, oracleParameters).FirstOrDefault();


        }

        public int UpdatePsac(EntitiesMWProcessing db , P_MW_ACK_LETTER_PREAUDIT model)
        {
            P_MW_ACK_LETTER_PREAUDIT record = db.P_MW_ACK_LETTER_PREAUDIT.Where(w => w.UUID == model.UUID).FirstOrDefault();

            if(record != null)
            {
                record.HANDLING_OFFICER = model.HANDLING_OFFICER;
                record.INSPECTION_DATE = model.INSPECTION_DATE;
                record.PA_RESULT = model.PA_RESULT;
                record.PA_REMARK = model.PA_REMARK;

                record.MODIFIED_BY = SessionUtil.LoginPost.CODE;
                record.MODIFIED_DATE = DateTime.Now;
            }

            return db.SaveChanges();
        }
    }
}