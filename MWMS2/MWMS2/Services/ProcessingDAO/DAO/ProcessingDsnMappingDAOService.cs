using MWMS2.Areas.MWProcessing.Models;
using MWMS2.Constant;
using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class ProcessingDsnMappingDAOService
    {
        public int UpdateMwDSN(P_MW_DSN model, EntitiesMWProcessing db)
        {
            P_MW_DSN record = db.P_MW_DSN.Where(w => w.UUID == model.UUID).FirstOrDefault();
            record.RECORD_ID = model.RECORD_ID;

            //Update StatusID
            record.SCANNED_STATUS_ID = model.SCANNED_STATUS_ID;

            return db.SaveChanges();
        }

        private string[] SystemValueCodes = { ProcessingConstant.DSN_RD_DELIVERED, ProcessingConstant.DSN_RD_RE_SENT, ProcessingConstant.DSN_REGISTRY_RECEIPT_COUNTED, ProcessingConstant.DSN_REGISTRY_RECEIVED, ProcessingConstant.DSN_RE_SCANNED, ProcessingConstant.DSN_CHECKLIST_SECTION_NEW, ProcessingConstant.DSN_CHECKLIST_PHOTO_NEW, ProcessingConstant.MWU_RD_INCOMING_NEW, };

        public List<P_MW_DSN> DsnMappingSearch(string DSN, DateTime? FromDate, DateTime? ToDate)
        {

            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return (from d in db.P_MW_DSN
                        join sv in db.P_S_SYSTEM_VALUE
                        on d.SCANNED_STATUS_ID equals sv.UUID
                        where (string.IsNullOrEmpty(DSN) ? true : d.DSN == DSN)
                        && SystemValueCodes.Contains(sv.CODE)
                        && (FromDate == null ? true : d.RD_DELIVERED_DATE >= FromDate.Value)
                        && (ToDate == null ? true : d.RD_DELIVERED_DATE <= ToDate.Value)
                        select d).ToList();
            }

        }

        public List<P_MW_DSN> DsnMappingSearch(string DSN, DateTime? FromDate, DateTime? ToDate, EntitiesMWProcessing db)
        {
            return (from d in db.P_MW_DSN
                    join sv in db.P_S_SYSTEM_VALUE
                    on d.SCANNED_STATUS_ID equals sv.UUID
                    where (string.IsNullOrEmpty(DSN) ? true : d.DSN == DSN)
                    && SystemValueCodes.Contains(sv.CODE)
                    && (FromDate == null ? true : d.RD_DELIVERED_DATE >= FromDate.Value)
                    && (ToDate == null ? true : d.RD_DELIVERED_DATE <= ToDate.Value)
                    select d).ToList();
        }
    }
}