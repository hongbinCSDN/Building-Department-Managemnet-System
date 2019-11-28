using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class ProcessingODLBLService
    {
        //ProcessingODLDAOService
        private ProcessingODLDAOService DAOService;
        protected ProcessingODLDAOService DA
        {
            get { return DAOService ?? (DAOService = new ProcessingODLDAOService()); }
        }
    }
}