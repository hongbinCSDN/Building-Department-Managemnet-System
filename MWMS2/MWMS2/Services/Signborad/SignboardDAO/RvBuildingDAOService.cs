using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class RvBuildingDAOService
    {
        public B_RV_BUILDING findByBgTableId(int? bgTableId)
        {
            B_RV_BUILDING result = new B_RV_BUILDING();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                result = db.B_RV_BUILDING.Where
                       (o => o.BG_TABLE_ID == bgTableId).FirstOrDefault();
            }
            return result;
        }
    }
}