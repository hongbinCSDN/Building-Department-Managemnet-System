using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MWMS2.Utility;

namespace MWMS2.Areas.Signboard.Models
{
    public class Fn03SRC_AASearchModel : DisplayGrid
    {
        public string RefNo { get; set; }
        public DateTime? PeriodDateFrom { get; set; }
        public DateTime? PeriodDateTo { get; set; }
        public string Status { get; set; }
        public List<SelectListItem> StatusList { get; set; } = SystemListUtil.GetStatusList(0);
    }
}