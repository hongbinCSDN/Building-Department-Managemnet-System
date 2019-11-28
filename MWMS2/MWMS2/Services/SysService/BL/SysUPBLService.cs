using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Services
{
    public class SysUPBLService
    {
        //SysUPDAOService
        private SysUPDAOService DAService;
        protected SysUPDAOService DA
        {
            get { return DAService ?? (DAService = new SysUPDAOService()); }
        }
    }
}