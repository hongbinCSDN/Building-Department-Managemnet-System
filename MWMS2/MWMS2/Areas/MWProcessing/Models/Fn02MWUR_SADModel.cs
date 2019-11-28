using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn02MWUR_SADModel : DisplayGrid
    {
     
        
            public string ScanDSN { get; set; }
            public string DSN { get; set; }
            public DateTime? ReceiveDateFrom { get; set; }
            public DateTime? ReceiveDateTo { get; set; }
            public string Status { get; set; }
        public List<SelectListItem> RetrieveStatusCode()
        {
            return SystemListUtil.RetrieveStatusCode();
        }

    }
}