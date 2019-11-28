using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Models
{
    public class Fn04RPT_STSearchModel : DisplayGrid
    {
        public DateTime? PeriodDateFrom { get; set; }
        public DateTime? PeriodDateTo { get; set; }
        public string Status { get; set; }
        public IEnumerable<SelectListItem> StatusList
        {
            get
            {
                return SystemListUtil.GetSTStatusList();
            }
        }
    }
}