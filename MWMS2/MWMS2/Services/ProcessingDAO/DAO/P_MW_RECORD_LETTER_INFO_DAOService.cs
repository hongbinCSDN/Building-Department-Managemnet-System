using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class P_MW_RECORD_LETTER_INFO_DAOService
    {
        public int AddP_MW_RECORD_LETTER_INFO(P_MW_RECORD_LETTER_INFO model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                db.P_MW_RECORD_LETTER_INFO.Add(model);
                return db.SaveChanges();
            }
        }

        public P_MW_RECORD_LETTER_INFO GetP_MW_RECORD_LETTER_INFOByDsn(string MW_DSN_ID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                db.Configuration.LazyLoadingEnabled = false;
                return db.P_MW_RECORD_LETTER_INFO.Where(w => w.MW_DSN_ID == MW_DSN_ID).FirstOrDefault();
            }
        }

        public int UpdateP_MW_RECORD_LETTER_INFO(P_MW_RECORD_LETTER_INFO model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                return db.SaveChanges();
            }
        }
    }
}