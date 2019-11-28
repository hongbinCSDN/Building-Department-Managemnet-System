using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class RvStreetNameDAOService
    {
        public B_RV_STREET_NAME findBySmTableId(int? smTableId)
        {
            B_RV_STREET_NAME result = new B_RV_STREET_NAME();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                result = db.B_RV_STREET_NAME.Where
                       (o => o.SM_TABLE_ID == smTableId).FirstOrDefault();
            }
            return result;
        }
    }
}