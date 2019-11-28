using MWMS2.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MWMS2.Services.ProcessingDAO.DAO
{
    public class ProcessingRDDAOService
    {

        public void createOrUpdateMWDSN(P_MW_DSN dsn)
        {
            ProcessingMWDSNDAOService da = new ProcessingMWDSNDAOService();
            da.createOrUpdateMWDSN(dsn);
        }

    }
}