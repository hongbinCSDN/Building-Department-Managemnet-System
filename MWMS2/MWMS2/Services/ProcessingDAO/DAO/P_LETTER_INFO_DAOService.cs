using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class P_LETTER_INFO_DAOService
    {
        public List<P_LETTER_INFO> GetP_LETTER_INFOLs()
        {
            using (EntitiesMWProcessing db = new EntitiesMWProcessing())
            {
                return db.P_LETTER_INFO.ToList();
            }
        }
    }
}