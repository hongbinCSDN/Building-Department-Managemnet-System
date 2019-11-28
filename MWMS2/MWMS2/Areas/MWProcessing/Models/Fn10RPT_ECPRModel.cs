using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Models;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn10RPT_ECPRModel:DisplayGrid
    {
        public string ReportType { get; set; }
        public string Status { get; set; }
        public string ReceiptChannel { get; set; }
        public string RelatedRefNo { get; set; }
        public string ReceivedDateFrom { get; set; }
        public string ReceivedDateTo { get; set; }
    }
}