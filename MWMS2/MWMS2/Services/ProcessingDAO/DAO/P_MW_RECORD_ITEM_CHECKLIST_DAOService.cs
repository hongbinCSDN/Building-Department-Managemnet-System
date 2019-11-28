using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Entity;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class P_MW_RECORD_ITEM_CHECKLIST_DAOService:BaseDAOService
    {

        public List<P_MW_RECORD_ITEM_CHECKLIST> GetItemChecklistByItemsAndVerificationID(List<P_MW_RECORD_ITEM> Items, string VerificationID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                string sSql = @"SELECT *
                                FROM   P_MW_RECORD_ITEM_CHECKLIST
                                WHERE  MW_RECORD_ITEM_ID IN ( :MW_RECORD_ITEM_ID )
                                       AND MW_VERIFICATION_ID = :MW_VERIFICATION_ID ";
                string ItemIDs = "";

                foreach(P_MW_RECORD_ITEM i in Items)
                {
                    ItemIDs += ",'" + i.UUID + "'";
                }

                if (!string.IsNullOrEmpty(ItemIDs))
                {
                    ItemIDs = ItemIDs.Substring(1);
                }
                else
                {
                    ItemIDs = "''";

                }

                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter(":MW_RECORD_ITEM_ID",ItemIDs),
                    new OracleParameter(":MW_VERIFICATION_ID",VerificationID)
                };

                return GetObjectData<P_MW_RECORD_ITEM_CHECKLIST>(sSql, oracleParameters).ToList();

               
            }
        }

        public List<PreRecordItemChecklist> GetPreRecordItemChecklists(string MW_RECORD_ID, string MW_VERIFICATION_ID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                string sSql = @"SELECT RI.UUID,
                                       RI.MW_ITEM_CODE,
                                       RI.ORDERING,
                                       RIC.VARIATION_DECLARED
                                FROM   P_MW_RECORD_Item RI
                                       LEFT JOIN P_MW_RECORD_ITEM_CHECKLIST RIC
                                              ON RI.UUID = RIC.MW_RECORD_ITEM_ID
                                                 AND RIC.MW_VERIFICATION_ID = :MW_VERIFICATION_ID
                                WHERE  RI.MW_RECORD_ID = :MW_RECORD_ID ";

                OracleParameter[] oracleParameters = new OracleParameter[]
                {
                    new OracleParameter(":MW_VERIFICATION_ID",MW_VERIFICATION_ID),
                    new OracleParameter(":MW_RECORD_ID",MW_RECORD_ID)
                };

                return GetObjectData<PreRecordItemChecklist>(sSql, oracleParameters).ToList();


            }
        }
    }
}