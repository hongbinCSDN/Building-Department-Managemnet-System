using MWMS2.Constant;
using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class P_MW_RECORD_DAOService
    {
        public P_MW_RECORD getFinalMwRecordByRefNo(P_MW_REFERENCE_NO mwReferenceNo)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_RECORD.Where(w => w.REFERENCE_NUMBER == mwReferenceNo.UUID && w.IS_DATA_ENTRY == ProcessingConstant.MW_NOT_DATA_ENTRY).FirstOrDefault();
            }
        }

        public P_MW_RECORD GetFinalMwRecordByRefNoUuid(string REFERENCE_NUMBER)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_RECORD.Where(w => w.REFERENCE_NUMBER == REFERENCE_NUMBER && w.IS_DATA_ENTRY == ProcessingConstant.MW_NOT_DATA_ENTRY).FirstOrDefault();
            }
        }

        public P_MW_RECORD GetFinalMwRecordByRefNo(string REFERENCE_NUMBER, EntitiesMWProcessing db)
        {
            //return db.P_MW_RECORD.Where(w => w.REFERENCE_NUMBER == REFERENCE_NUMBER && w.IS_DATA_ENTRY == ProcessingConstant.MW_NOT_DATA_ENTRY).FirstOrDefault();
            return db.P_MW_RECORD.Where(w => w.P_MW_REFERENCE_NO.REFERENCE_NO == REFERENCE_NUMBER && w.IS_DATA_ENTRY == ProcessingConstant.MW_NOT_DATA_ENTRY).FirstOrDefault();
        }

        public P_MW_RECORD GetFinalMwRecordByRefNo(string REFERENCE_NUMBER)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_RECORD.Where(w => w.P_MW_REFERENCE_NO.REFERENCE_NO == REFERENCE_NUMBER && w.IS_DATA_ENTRY == ProcessingConstant.MW_NOT_DATA_ENTRY).FirstOrDefault();
            }
                
        }

        public P_MW_RECORD GetP_MW_RECORDByUuid(string uuid, EntitiesMWProcessing db)
        {
            return db.P_MW_RECORD.Where(w => w.UUID == uuid).Include(o => o.P_MW_VERIFICATION).FirstOrDefault();
        }

        public P_MW_RECORD GetP_MW_RECORDByUuid(string uuid)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_RECORD.Where(w => w.UUID == uuid).Include(o => o.P_MW_VERIFICATION).FirstOrDefault();
            }

        }

        public List<P_MW_RECORD> GetP_MW_RECORDsByRefNo(EntitiesMWProcessing db, string refNo)
        {
            List<P_MW_RECORD> result = new List<P_MW_RECORD>();
            var reference = db.P_MW_REFERENCE_NO.Where(w => w.REFERENCE_NO == refNo).FirstOrDefault();

            if (reference != null)
            {
                result = reference.P_MW_RECORD.ToList();
            }

            return result;
        }

        public List<P_MW_RECORD> GetP_MW_RECORDsByRefNo(string refNo)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                List<P_MW_RECORD> result = new List<P_MW_RECORD>();
                var reference = db.P_MW_REFERENCE_NO.Where(w => w.REFERENCE_NO == refNo).FirstOrDefault();

                if (reference != null)
                {
                    result = reference.P_MW_RECORD.ToList();
                }

                return result;
            }

        }
    }
}