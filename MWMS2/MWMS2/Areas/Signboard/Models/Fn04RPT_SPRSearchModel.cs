using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Signboard.Models
{
    public class Fn04RPT_SPRSearchModel : DisplayGrid
    {
        public DateTime? PeriodFromDate { get;set;}
        public DateTime? PeriodToDate { get; set; }
        public string SubNo { get; set; }
        public List<string> ResultList { get; set; } = new List<string>();
        public List<string> CountList { get; set; } = new List<string>();
        public List<string> involvedList { get; set; } = new List<string>();

    }
}