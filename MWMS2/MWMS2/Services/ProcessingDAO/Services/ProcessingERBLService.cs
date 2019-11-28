using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class ProcessingERBLService
    {
        //ProcessingERDAOService
        private ProcessingERDAOService DAOService;
        protected ProcessingERDAOService DA
        {
            get { return DAOService ?? (DAOService = new ProcessingERDAOService()); }
        }
    }
}