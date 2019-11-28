using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Entity;

namespace MWMS2.Services.ProcessingDAO.DAO
{
    public class P_MW_FILEREF_DAOService
    {
        public P_MW_FILEREF GetFileRefByMWRecord(string mw)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_FILEREF.Where(m => m.MW_RECORD_ID == mw).FirstOrDefault();
            }
        }
        public P_MW_FILEREF GetFileRefByMWRecord(string mw, EntitiesMWProcessing db)
        {
            return db.P_MW_FILEREF.Where(m => m.MW_RECORD_ID == mw).FirstOrDefault();
        }

        public List<P_MW_FILEREF> GetFileRefsByMwRefNo(EntitiesMWProcessing db ,string mwRefNo)
        {
            return db.P_MW_FILEREF.Where(m => m.MW_RECORD_ID == mwRefNo).ToList();
        }

        public List<P_MW_FILEREF> GetFileRefsByMwRefNo(string mwRefNo)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_FILEREF.Where(m => m.MW_RECORD_ID == mwRefNo).ToList();
            }
                
        }

    }
}