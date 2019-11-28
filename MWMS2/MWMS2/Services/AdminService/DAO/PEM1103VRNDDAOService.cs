using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class PEM1103VRNDDAOService
    {
        private const string SearchSql = @"SELECT *
                                           FROM   P_S_RULE_OF_CON_LETTER_AND_REF ";
        public List<P_S_RULE_OF_CON_LETTER_AND_REF> Search()
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_S_RULE_OF_CON_LETTER_AND_REF.ToList();
            }
        }

        public void Search(DisplayGrid displayGrid)
        {
            displayGrid.Query = SearchSql;
            displayGrid.Search();
        }

        public int Update(List<P_S_RULE_OF_CON_LETTER_AND_REF> updateList)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                foreach(P_S_RULE_OF_CON_LETTER_AND_REF item in updateList)
                {
                    P_S_RULE_OF_CON_LETTER_AND_REF record = db.P_S_RULE_OF_CON_LETTER_AND_REF.Where(d => d.UUID == item.UUID).FirstOrDefault();

                    record.DAYS_OF_NOTIFICATION = item.DAYS_OF_NOTIFICATION;
                    record.DAYS_OF_NOTIFICATION_COMPARE = item.DAYS_OF_NOTIFICATION_COMPARE;

                    record.CONDITIONAL_LETTER_VALUE1 = item.CONDITIONAL_LETTER_VALUE1;
                    record.CONDITIONAL_LETTER_COMPARE1 = item.CONDITIONAL_LETTER_COMPARE1;

                    record.CONDITIONAL_LETTER_VALUE2 = item.CONDITIONAL_LETTER_VALUE2;
                    record.CONDITIONAL_LETTER_COMPARE2 = item.CONDITIONAL_LETTER_COMPARE2;

                    record.REFUSAL_VALUE = item.REFUSAL_VALUE;
                    record.REFUSAL_COMPARE = item.REFUSAL_COMPARE;

                }

                return db.SaveChanges();


            }
        }
    }
}