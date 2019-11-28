using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Entity;

namespace MWMS2.Services
{
    public class MWGeneralRecordService
    {
        public static P_MW_GENERAL_RECORD MWGeneralRecordByCaseNo(string mwReferenceNo)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_MW_GENERAL_RECORD p_MW_GENERAL_RECORD = db.P_MW_GENERAL_RECORD.Where(m => m.REFERENCE_NUMBER == mwReferenceNo).FirstOrDefault();
                return p_MW_GENERAL_RECORD;
            }
        }
        public static P_MW_GENERAL_RECORD MWGeneralRecordByUUID(string uuid)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                P_MW_GENERAL_RECORD p_MW_GENERAL_RECORD = db.P_MW_GENERAL_RECORD.Where(m => m.UUID == uuid).FirstOrDefault();
                return p_MW_GENERAL_RECORD;
            }
        }

        public P_MW_GENERAL_RECORD MWGeneralRecordByUUID(EntitiesMWProcessing db, string uuid)
        {
            return db.P_MW_GENERAL_RECORD.Where(m => m.UUID == uuid).FirstOrDefault();
        }
    }
}