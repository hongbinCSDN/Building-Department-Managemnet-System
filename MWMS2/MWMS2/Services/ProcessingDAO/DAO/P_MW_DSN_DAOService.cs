using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class P_MW_DSN_DAOService
    {
        public P_MW_DSN GetP_MW_DSNByDsn(string DSN)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                db.Configuration.LazyLoadingEnabled = false;
                return db.P_MW_DSN.Where(w => w.DSN == DSN).FirstOrDefault();
            }
        }

        public P_MW_DSN GetP_MW_DSNByDsn(string DSN, EntitiesMWProcessing db)
        {
            return db.P_MW_DSN.Where(w => w.DSN == DSN).FirstOrDefault();
        }


        public int AddP_MW_DSN(P_MW_DSN model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                db.P_MW_DSN.Add(model);
                return db.SaveChanges();
            }
        }

        public int UpdateP_MW_DSN(P_MW_DSN model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                return db.SaveChanges();
            }
        }
        public List<P_MW_DSN> GetMwDsnListByRecordIdAndStatusID(string recordID, string svUUID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_DSN.Where(m => m.RECORD_ID == recordID && m.SCANNED_STATUS_ID == svUUID).ToList();
            }
        }

        public List<P_MW_DSN> GetMwDsnListByRecordId(string recordID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_DSN.Where(m => m.RECORD_ID == recordID).ToList();
            }
        }

        public P_MW_DSN GetParentMwDsnByRecordId(string recordID)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_DSN.Where(m => m.RECORD_ID == recordID).OrderBy(o=>o.CREATED_DATE).FirstOrDefault();
            }
        }

    }
}