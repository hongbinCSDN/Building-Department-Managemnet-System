using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class P_MW_REFERENCE_NO_DAOService
    {
        public P_MW_REFERENCE_NO getMwReferenceNoByMwNo(String mwNo)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_REFERENCE_NO.Where(w => w.REFERENCE_NO == mwNo).FirstOrDefault();
            }
        }

        public P_MW_REFERENCE_NO getMwReferenceNoByMwNo(String mwNo, EntitiesMWProcessing db)
        {
            return db.P_MW_REFERENCE_NO.Where(w => w.REFERENCE_NO == mwNo).FirstOrDefault();
        }

        public int AddP_MW_REFERENCE_NO(P_MW_REFERENCE_NO model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                db.P_MW_REFERENCE_NO.Add(model);
                return db.SaveChanges();
            }
        }

        public int AddP_MW_REFERENCE_NO(P_MW_REFERENCE_NO model, EntitiesMWProcessing db)
        {
                db.P_MW_REFERENCE_NO.Add(model);
                return db.SaveChanges();
        }
    }
}