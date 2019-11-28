using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class ProcessingAcBLService
    {
        private ProcessingAcDAOService DAOService;
        protected ProcessingAcDAOService DA
        {
            get { return DAOService ?? (DAOService = new ProcessingAcDAOService()); }
        }
    }
}