using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class P_MW_ACK_LETTER_PREAUDIT_DAOService
    {
        public P_MW_ACK_LETTER_PREAUDIT GetP_MW_ACK_LETTER_PREAUDITByUuid(string uuid)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_ACK_LETTER_PREAUDIT.Where(w => w.UUID == uuid).FirstOrDefault();
            }
        }

        public int AddP_MW_ACK_LETTER_PREAUDIT(P_MW_ACK_LETTER_PREAUDIT model)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                db.P_MW_ACK_LETTER_PREAUDIT.Add(model);
                return db.SaveChanges();
            }
        }

        public int RemoveP_MW_ACK_LETTER_PREAUDIT(EntitiesMWProcessing db, string uuid)
        {
            P_MW_ACK_LETTER_PREAUDIT record = db.P_MW_ACK_LETTER_PREAUDIT.Where(w => w.UUID == uuid).FirstOrDefault();
            if (record != null)
            {
                db.P_MW_ACK_LETTER_PREAUDIT.Remove(record);
            }
            return db.SaveChanges();
        }
    }
}