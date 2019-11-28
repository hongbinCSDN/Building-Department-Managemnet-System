using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{ 
    public class ProcessingAscBLService
    {
        private ProcessingAscDAOService DAOService;
        protected ProcessingAscDAOService DA
        {
            get { return DAOService ?? (DAOService = new ProcessingAscDAOService()); }
        }

    }
}