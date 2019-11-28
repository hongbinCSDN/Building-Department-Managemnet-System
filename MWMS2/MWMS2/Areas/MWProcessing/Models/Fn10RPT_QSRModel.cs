using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn10RPT_QSRModel : DisplayGrid
    {
        public string ClassCode { get; set; }

        public List<SelectListItem> ClassCodeList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem{ Value = "", Text = "- ALL -" }
            ,new SelectListItem{Value = "CLASS_I",Text="Class I"}
            ,new SelectListItem{Value = "CLASS_II",Text = "Class II"}
            ,new SelectListItem{Value = "CLASS_III",Text = "Class III"}
        };
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }

}