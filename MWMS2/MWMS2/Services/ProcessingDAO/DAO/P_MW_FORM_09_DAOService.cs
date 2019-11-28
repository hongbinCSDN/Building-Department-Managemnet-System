using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class P_MW_FORM_09_DAOService
    {
        public List<P_MW_FORM_09> getMwForm09ByMwRecord(P_MW_RECORD mwRecord)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_FORM_09.Where(w => w.MW_RECORD_ID == mwRecord.UUID && !string.IsNullOrEmpty(w.MW_NUMBER)).OrderBy(o => o.ORDERING).ToList();
            }
        }

    }
}