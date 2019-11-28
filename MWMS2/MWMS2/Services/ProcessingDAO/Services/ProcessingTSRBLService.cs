using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class ProcessingTSRBLService
    {
        //ProcessingTSRDAOService
        private ProcessingTSRDAOService DAOService;
        protected ProcessingTSRDAOService DA
        {
            get { return DAOService ?? (DAOService = new ProcessingTSRDAOService()); }
        }
    }
}