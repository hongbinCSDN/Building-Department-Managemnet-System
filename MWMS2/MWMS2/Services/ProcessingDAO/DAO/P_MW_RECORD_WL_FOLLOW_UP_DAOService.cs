using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Constant;

namespace MWMS2.Services
{
    public class P_MW_RECORD_WL_FOLLOW_UP_DAOService
    {

        public P_MW_RECORD_WL_FOLLOW_UP GetP_MW_RECORD_WL_FOLLOW_UPByRecordID(string recordID, string handlingUnit)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_RECORD_WL_FOLLOW_UP.Where(w => w.MW_RECORD_ID == recordID && (handlingUnit == ProcessingConstant.HANDLING_UNIT_SMM ? (w.HANDLING_UNIT == handlingUnit) : ((w.HANDLING_UNIT == handlingUnit) || (w.HANDLING_UNIT == null)))).FirstOrDefault();
            }
        }
    }
}