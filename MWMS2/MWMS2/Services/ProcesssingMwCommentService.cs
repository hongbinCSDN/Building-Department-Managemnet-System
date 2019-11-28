using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Entity;

namespace MWMS2.Services
{
    public class ProcessingMwCommentService
    {
        public List<P_MW_COMMENT> getMwCommentsByRecordIdAndRecordType(string recordId, string recordType)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_COMMENT.Where(m => m.RECORD_ID == recordId && m.RECORD_TYPE == recordType).ToList();
            }
        }
        public P_MW_COMMENT getMwCommentsByUUID(string uuid)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_MW_COMMENT.Where(m => m.UUID == uuid).FirstOrDefault();
            }
        }
    }
}