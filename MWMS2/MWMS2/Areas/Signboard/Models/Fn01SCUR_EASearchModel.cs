using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Models
{
    public class Fn01SCUR_EASearchModel : DisplayGrid
    {
        public string SCNo { get; set; }
        public string DSN { get; set; }
        public string EfssNo { get; set; }
        public string ErrMsg { get; set; }
        public string Status { get; set; }
        public List<SelectListItem> StatusList = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "- All -", Value = "0" },
            new SelectListItem() { Text = "Submitted", Value = "1" },
            new SelectListItem() { Text = "Not yet submitted", Value = "2" }
        };

        public bool reload { get; set; }
    }
}