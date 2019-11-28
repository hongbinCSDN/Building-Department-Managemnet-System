using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Models;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn10RPT_SLRModel : DisplayGrid
    {
        public string SortBy { get; set; }
        public string RefNo { get; set; }
        public string DSN { get; set; }
    }
}