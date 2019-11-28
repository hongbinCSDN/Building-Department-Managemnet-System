using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Admin.Models
{
    public class PEM1111UploadPDFHistoryModel : DisplayGrid
    {
        public string UUID { get; set; }
        public string CreateDate { get; set; }
        public string UploadBy { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Status { get; set; }

        public IEnumerable<SelectListItem> StatusList
        {
            get
            {
                return (new List<SelectListItem>()
                {
                    new SelectListItem{ Value = "" , Text="- All -"},
                    new SelectListItem{ Value = "SUCCESS" , Text="Success Case"},
                    new SelectListItem{ Value = "UNSUCCESS" , Text="Unsuccess Case"},
                });
            }
        }
    }
}