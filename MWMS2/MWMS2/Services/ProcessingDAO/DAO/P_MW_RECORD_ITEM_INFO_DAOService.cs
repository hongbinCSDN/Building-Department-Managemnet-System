using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class P_MW_RECORD_ITEM_INFO_DAOService
    {
        public List<P_MW_RECORD_ITEM_INFO> GetP_MW_RECORD_ITEM_INFOsByItemCodes(string[] ItemCodes)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return (from x in db.P_MW_RECORD_ITEM_INFO
                        where (ItemCodes.Contains(x.ORIENTAL_ITEM_ID))
                        select x).ToList();
            }
        }
    }
}