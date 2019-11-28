using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn10RPT_OSRModel : DisplayGrid
    {
        public string Day { get; set; }
        public string Officer { get; set; }
    }
}