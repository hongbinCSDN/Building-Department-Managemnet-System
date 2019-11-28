using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class P_S_TO_DETAILS_DAOService
    {

        public P_S_TO_DETAILS GetP_S_TO_DETAILSByToPost(string toPost)
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_S_TO_DETAILS.Where(w => w.TO_POST == toPost).FirstOrDefault();
            }
        }
    }
}