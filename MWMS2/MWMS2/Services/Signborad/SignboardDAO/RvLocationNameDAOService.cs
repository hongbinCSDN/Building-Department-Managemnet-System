using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class RvLocationNameDAOService
    {
        public B_RV_LOCATION_NAME findByLcTableId(int? lcTableId)
        {
            B_RV_LOCATION_NAME result = new B_RV_LOCATION_NAME();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                result = db.B_RV_LOCATION_NAME.Where
                       (o => o.LC_TABLE_ID == lcTableId).FirstOrDefault();
            }
            return result;
        }
    }
}