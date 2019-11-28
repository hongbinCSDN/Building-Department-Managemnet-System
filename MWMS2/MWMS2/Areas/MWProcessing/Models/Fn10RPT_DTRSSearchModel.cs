using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn10RPT_DTRSSearchModel : DisplayGrid
    {
        public string DSN { get; set; }
        public string RefNo { get; set; }
        public string DocumentType { get; set; }
        public string PeriodFromDate { get; set; }
        public string PeriodToDate { get; set; }
        public List<SelectListItem> GetDocumentTypeList()
        {
            return new List<SelectListItem>
            {
                 new SelectListItem{Text="- All -",Value=""}
                ,new SelectListItem{Text="Incoming",Value="Incoming"}
                ,new SelectListItem{Text = "Outgoing",Value="Outgoing"}
            };
        }
        public List<SelectListItem> GetRelatedRefNoList()
        {
            return new List<SelectListItem>
            {
                new SelectListItem{Text = "- All -",Value=""}
                ,new SelectListItem{Text = "MW",Value="MW"}
                ,new SelectListItem{Text = "VS",Value="VS"}
                ,new SelectListItem{Text="Enq",Value="Enq"}
                ,new SelectListItem{Text="Com",Value="Com"}
            };
        }
    }
}