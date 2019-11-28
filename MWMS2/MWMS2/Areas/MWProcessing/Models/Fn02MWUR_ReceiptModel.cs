using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn02MWUR_ReceiptModel:DisplayGrid
    {
        public String DSN { get; set; }
        public Dictionary<string, string> OutstandingList { get; set; }

        public Dictionary<string, string> ConfirmReceiptList { get; set; }
    }
}