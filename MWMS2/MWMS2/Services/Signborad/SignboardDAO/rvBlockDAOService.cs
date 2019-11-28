using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services.Signborad.SignboardDAO
{
    public class RvBlockDAOService
    {
        public B_RV_BLOCK findByBlockIdAndStreetCode(long blockId, long streetCode)
        {
            List<B_RV_BLOCK> resultList = new List<B_RV_BLOCK>();
            using (EntitiesSignboard db = new EntitiesSignboard())
            {
                resultList = db.B_RV_BLOCK.Where
                       (o => o.BK_TABLE_ID == blockId
                       && o.BK_SL_TABLE_ID == streetCode
                       ).ToList();
            }
            return resultList[0];
        }
    }
}