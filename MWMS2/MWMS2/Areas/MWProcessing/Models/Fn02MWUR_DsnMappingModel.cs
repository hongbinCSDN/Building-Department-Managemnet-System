using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn02MWUR_DsnMappingModel : DisplayGrid
    {
        //public int Total { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string DSN { get; set; }
        public string ReferenceNo { get; set; }
        public string FormNo { get; set; }
        public string DocType { get; set; }
    }
}