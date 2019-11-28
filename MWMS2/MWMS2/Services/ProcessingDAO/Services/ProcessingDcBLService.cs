using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class ProcessingDcBLService
    {
        private ProcessingDcDAOService DAOService;
        protected ProcessingDcDAOService DA
        {
            get { return DAOService ?? (DAOService = new ProcessingDcDAOService()); }
        }
    }
}