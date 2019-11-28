using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Admin.Models
{
    public class PEMBookMarkSearchModel :DisplayGrid 
    {
        public string UUID { get; set; }
        public string Street {get;set;}
        public string StreetNo { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string Unit { get; set; }
    }
}