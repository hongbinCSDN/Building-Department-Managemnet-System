using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class ProcessingIncomingDAOService
    {

        public int UpdateDsn(P_MW_DSN model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_MW_DSN record = db.P_MW_DSN.Where(w => w.UUID == model.UUID).FirstOrDefault();
                record.RECORD_ID = model.RECORD_ID;

                return db.SaveChanges();
            }
        }
    }
}