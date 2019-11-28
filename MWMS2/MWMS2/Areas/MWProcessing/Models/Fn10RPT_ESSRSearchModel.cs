using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn10RPT_ESSRSearchModel : DisplayGrid
    {
        public DateTime? ScanDateFrom { get; set; }
        public DateTime? ScanDateTo { get; set; }
    }
}