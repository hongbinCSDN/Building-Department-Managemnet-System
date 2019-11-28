using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class ProcessingPebBLService
    {
        private ProcessingPebDAOService DAOService;
        protected ProcessingPebDAOService DA
        {
            get { return DAOService ?? (DAOService = new ProcessingPebDAOService()); }
        }
    }
}