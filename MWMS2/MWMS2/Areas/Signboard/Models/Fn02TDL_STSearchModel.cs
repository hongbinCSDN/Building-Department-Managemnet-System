using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Models
{
    public class Fn02TDL_STSearchModel : DisplayGrid
    {
        public string SearchClass { get; set; }
        public List<SelectListItem> ChildList { get; set; }
        public int ValidationList { get; set; }
    }
}