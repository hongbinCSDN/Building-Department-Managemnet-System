using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Models
{
    public class Fn04RPT_UPSRSearchModel : DisplayGrid
    {
       public string SearchFileRefNo { get; set; }
       public string SearchSortBy { get; set; }
       public List<SelectListItem> SortList { get; set; } = SystemListUtil.GetSortList();
    }
}