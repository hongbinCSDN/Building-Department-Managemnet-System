using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class P_MW_RECORD_ITEM_DAOService : BaseDAOService
    {
        public List<P_MW_RECORD_ITEM> getMwRecordItemByMwRecordOrdering(P_MW_RECORD mwRecord, bool isItemCodeNullable = true)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_RECORD_ITEM.Where(w => w.MW_RECORD_ID == mwRecord.UUID && (isItemCodeNullable ? true : !string.IsNullOrEmpty(w.MW_ITEM_CODE))).OrderBy(o => o.CLASS_CODE).ThenBy(o => o.ORDERING).ToList();
            }
        }

        public List<PreMwItem> GetPreItemsByRecordID(string MW_RECORD_ID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                string sSql = @"SELECT RI.UUID                 AS UUID,
                                       RI.MW_ITEM_CODE         AS ItemCode,
                                       RI.LOCATION_DESCRIPTION AS Location,
                                       RI.RELEVANT_REFERENCE   AS DescOfVariation,
                                       RI.UUID                 AS FianlItemUUID,
                                       ( CASE
                                           WHEN RII.UUID IS NULL THEN 'N'
                                           ELSE 'Y'
                                         END )                 AS MatchItem
                                FROM   P_MW_RECORD_Item RI
                                       LEFT JOIN P_MW_RECORD_Item_Info RII
                                              ON RI.UUID = RII.ORIENTAL_ITEM_ID
                                WHERE  RI.MW_RECORD_ID = :MW_RECORD_ID
                                       AND STATUS_CODE = 'FINAL' ";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter(":MW_RECORD_ID",MW_RECORD_ID)
                };

                return GetObjectData<PreMwItem>(sSql, oracleParameters).ToList();


            }
        }

        public List<P_MW_RECORD_ITEM> GetFinalP_MW_RECORD_ITEMsByRefNo(string REFERENCE_NO)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                string sSql = @"SELECT RI.*
                                FROM   P_MW_Reference_No RN
                                       JOIN P_MW_RECORD R
                                         ON RN.UUID = R.REFERENCE_NUMBER
                                            AND R.IS_DATA_ENTRY = :IS_DATA_ENTRY
                                       JOIN P_MW_RECORD_ITEM RI
                                         ON R.UUID = RI.MW_RECORD_ID
                                            AND RI.STATUS_CODE = 'FINAL'
                                WHERE  REFERENCE_NO = :REFERENCE_NO ";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter(":IS_DATA_ENTRY",ProcessingConstant.MW_NOT_DATA_ENTRY),
                    new OracleParameter(":REFERENCE_NO",REFERENCE_NO)
                };

                return GetObjectData<P_MW_RECORD_ITEM>(sSql, oracleParameters).ToList();
            }

        }
    }
}