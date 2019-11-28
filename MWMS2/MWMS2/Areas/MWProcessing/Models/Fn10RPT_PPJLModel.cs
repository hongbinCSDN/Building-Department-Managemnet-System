using MWMS2.Constant;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn10RPT_PPJLModel : DisplayGrid
    {
        public string Name_of_PBP_PRC { get; set; }
        public string Registration_NO { get; set; }
        public string Ref_NO { get; set; }
        public string Status { get; set; }
        public string Location_Or_Address_of_Works { get; set; }

        public Fn10RPT_PPJLSearchModel Fn10RPT_PPJLSearchModel { get; set; } = new Fn10RPT_PPJLSearchModel();
    }

    public class Fn10RPT_PPJLSearchModel
    {
        public string Submission_Type { get; set; }
        public List<SelectListItem> Submission_TypeList { get; set; } = new List<SelectListItem>
        {
             new SelectListItem{ Value = "",  Text = "- Select -" }
            ,new SelectListItem{ Value = ProcessingConstant.PREFIX_MW, Text = ProcessingConstant.PREFIX_MW }
            ,new SelectListItem{ Value = ProcessingConstant.PREFIX_VS, Text = ProcessingConstant.PREFIX_VS }
        };

        public string Status { get; set; }
        public List<SelectListItem> StatusList { get; set; } = new List<SelectListItem>
        {
             new SelectListItem{ Value = "",Text = "- Select -" }
            ,new SelectListItem{ Value = ProcessingConstant.RPT_STATUS_IN_PROGRESS,Text = ProcessingConstant.RPT_STATUS_IN_PROGRESS }
            ,new SelectListItem{ Value = ProcessingConstant.RPT_STATUS_COMPLETED, Text = ProcessingConstant.RPT_STATUS_COMPLETED }
        };
        public string Registration_No_of_PBP { get; set; }
        public string English_Name_of_PBP { get; set; }
        public string Chinese_Name_of_PBP { get; set; }
        public string Registration_No_of_PRC { get; set; }
        public string English_Name_of_PRC { get; set; }
        public string Chinese_Name_of_PRC { get; set; }
    }
}