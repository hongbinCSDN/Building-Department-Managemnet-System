using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn02MWUR_SDModel:DisplayGrid
    {
        public string ScanDSN { get; set; }
        public string DSN { get; set; }
        public DateTime? CompilingDateFrom { get; set; }
        public DateTime? CompilingDateTo { get; set; }
    }
}