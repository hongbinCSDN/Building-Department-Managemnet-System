using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class SvRecordValidationItemDAOService
    {
        public List<B_SV_RECORD_VALIDATION_ITEM> getSvRecordValidationItems(string UUID)
        {
            List<B_SV_RECORD_VALIDATION_ITEM> svRecordItemList = new List<B_SV_RECORD_VALIDATION_ITEM>();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                svRecordItemList = (from svrvi in db.B_SV_RECORD_VALIDATION_ITEM
                                    where svrvi.UUID == UUID
                                    select svrvi).ToList();
            }
            return svRecordItemList;
        }
        public void deleteSvRecordValidationItems(string svRecordUUID)
        {
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                db.B_SV_RECORD_VALIDATION_ITEM.RemoveRange(db.B_SV_RECORD_VALIDATION_ITEM.Where(c => c.SV_RECORD_ID == svRecordUUID));
                db.SaveChanges();
            }
        }
    }
}