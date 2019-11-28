using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn01LM_Search_MOD_Model : DisplayGrid
    {
        public string ModRefNo { get; set; }
        public DateTime? ReceiveDateFrom { get; set; }
        public DateTime? ReceiveDateTo { get; set; }
        public string Address { get; set; }
        public string NoOfApplicant { get; set; }
        public string HandingStaff { get; set; }
    }
}