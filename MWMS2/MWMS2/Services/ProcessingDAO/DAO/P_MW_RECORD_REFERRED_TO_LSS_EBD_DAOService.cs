using MWMS2.Constant;
using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class P_MW_RECORD_REFERRED_TO_LSS_EBD_DAOService
    {
        public P_MW_RECORD_REFERRED_TO_LSS_EBD GetP_MW_RECORD_REFERRED_TO_LSS_EBDByRecordID(string recordID, string handlingUnit)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_RECORD_REFERRED_TO_LSS_EBD.Where(w => w.MW_RECORD_ID == recordID && (handlingUnit == ProcessingConstant.HANDLING_UNIT_SMM ? (w.HANDLING_UNIT == handlingUnit) : ((w.HANDLING_UNIT == handlingUnit) || (w.HANDLING_UNIT == null)))).FirstOrDefault();
            }
        }
    }
}