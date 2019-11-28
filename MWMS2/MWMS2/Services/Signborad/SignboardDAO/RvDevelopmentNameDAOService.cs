using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class RvDevelopmentNameDAOService
    {
        public B_RV_DEVELOPMENT_NAME findByDvTableId(int? dvTableId)
        {
            B_RV_DEVELOPMENT_NAME result = new B_RV_DEVELOPMENT_NAME();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                result = db.B_RV_DEVELOPMENT_NAME.Where
                       (o => o.DV_TABLE_ID == dvTableId).FirstOrDefault();
            }
            return result;
        }
    }
}