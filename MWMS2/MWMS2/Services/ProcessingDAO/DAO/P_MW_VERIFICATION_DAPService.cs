using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class P_MW_VERIFICATION_DAPService
    {
        public P_MW_VERIFICATION GetP_MW_VERIFICATIONByUuid(string uuid)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_VERIFICATION.Where(w => w.UUID == uuid).FirstOrDefault();
            }
        }
    }
}