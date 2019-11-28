using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class SYS_UNIT_DAOService
    {
        public SYS_UNIT GetSYS_UNITByUuid(string uuid)
        {
            using (EntitiesAuth db = new EntitiesAuth())
            {
                return db.SYS_UNIT.Where(w => w.UUID == uuid).FirstOrDefault();
            }
        }
    }
}