using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class RvStreetLocationDAOService
    {
        public B_RV_STREET_LOCATION findBySlTableId(int? slTableId)
        {
            B_RV_STREET_LOCATION result = new B_RV_STREET_LOCATION();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                result = db.B_RV_STREET_LOCATION.Where
                       (o => o.SL_TABLE_ID == slTableId).FirstOrDefault();
            }
            return result;
        }
    }
}