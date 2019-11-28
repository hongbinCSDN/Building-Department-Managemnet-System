using MWMS2.Entity;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class SvRecordDAOService : BaseCommonService
    {
        public B_SV_RECORD getLatestSVRecordByRefNo(string signboardNo, string formCode) {

            List<B_SV_RECORD> svRecordList = null;
            B_SV_RECORD result = new B_SV_RECORD();

            B_SV_RECORD MaxDate = new B_SV_RECORD();

            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                MaxDate.CREATED_DATE = (from svr in db.B_SV_RECORD
                           where svr.REFERENCE_NO == signboardNo && svr.FORM_CODE == formCode
                           select svr.CREATED_DATE).Max();


                svRecordList = (from svr in db.B_SV_RECORD
                                  where
                                  svr.REFERENCE_NO == signboardNo
                                  && svr.CREATED_DATE == MaxDate.CREATED_DATE
                                  select svr).ToList();
            }
            if (svRecordList.Count() !=0)
            {
                result = svRecordList[0];
            }
            return result;
        }
        public B_SV_RECORD getSVRecordBySvSubmissionUUID(string UUID)
        {
            List<B_SV_RECORD> svRecordList = null;
            B_SV_RECORD result = new B_SV_RECORD();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                svRecordList =
                    db.B_SV_RECORD.Where(o => o.SV_SUBMISSION_ID == UUID)
                    .Include(o => o.B_SV_RECORD_ITEM)
                    .Include(o => o.B_SV_RECORD_VALIDATION_ITEM).ToList();
            }
            if (svRecordList.Count() != 0)
            {
                result = svRecordList[0];
            }
            return result;
        }
        public List<B_SV_RECORD> getSVRecordByBatchNumber(string batchNumber, string svRecordUUID)
        {
            List<B_SV_RECORD> result = new List<B_SV_RECORD>();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                result = (from svr in db.B_SV_RECORD
                          join svs in db.B_SV_SUBMISSION on svr.SV_SUBMISSION_ID equals svs.UUID
                          where
                          svs.BATCH_NO == batchNumber
                          && svr.UUID == svRecordUUID
                          select svr).ToList();
            }
            return result;
        }
        public B_SV_RECORD findById(string id)
        {
            B_SV_RECORD instance = new B_SV_RECORD();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                instance = db.B_SV_RECORD.Find(id);
            }
            return instance;
        }
        public B_SV_RECORD GetSvReocrdByUUID(string UUID)
        {
            List<B_SV_RECORD> List = null;
            B_SV_RECORD svRecord = new B_SV_RECORD();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                List = (from svr in db.B_SV_RECORD
                          where
                          svr.UUID == UUID
                        select svr).ToList();
            }
            if (List.Count() != 0)
            {
                svRecord = List[0];
            }
            return svRecord;
        }
    }
}