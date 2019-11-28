using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class P_MW_ACK_LETTER_DAOService
    {
        public P_MW_ACK_LETTER GetP_MW_ACK_LETTERByDsn(string dsn)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_ACK_LETTER.Where(w => w.DSN == dsn).FirstOrDefault();
            }
        }

        public P_MW_ACK_LETTER GetP_MW_ACK_LETTERByMWNo(string mwno, string formno, EntitiesMWProcessing db)
        {
            return db.P_MW_ACK_LETTER.Where(w => w.MW_NO == mwno && w.FORM_NO == formno).FirstOrDefault();
        }

        public P_MW_ACK_LETTER GetP_MW_ACK_LETTERByUuid(string uuid)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_ACK_LETTER.Where(w => w.UUID == uuid).FirstOrDefault();
            }
            
        }
    }
}