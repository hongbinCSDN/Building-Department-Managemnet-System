using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class P_MW_FORM_DAOService
    {
        public P_MW_FORM getMwFormByMwRecordandFormCode(P_MW_RECORD mwRecord)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_FORM.Where(w => w.MW_RECORD_ID == mwRecord.UUID).FirstOrDefault();
            }
        }

    }
}