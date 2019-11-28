using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MWMS2.Utility;

namespace MWMS2.Areas.Registration.Models
{
    public class MWCA_Model : ApiController
    {
        SystemListUtil systemListUtil = new SystemListUtil();

        public string PNRC { get; set; }

        //public List<String> retrievePNRCList()
        //{
        //   
        //    return systemListUtil.retrievePNRCList();
        //}
    }
}