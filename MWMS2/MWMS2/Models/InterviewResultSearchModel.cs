using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Utility;

namespace MWMS2.Models
{
    // Common model for Prof, MWC(W), GBC and MWC
    public class InterviewResultSearchModel : DisplayGrid
    {
        public string Year { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Group { get; set; }
        public string Type { get; set; }
        public string SurnName { get; set; }
        public string GivenName { get; set; }
        public string FileRef { get; set; }
        public string InterviewNo{ get; set; } 
        public string HKID { get; set; }
        public string PassportNo { get; set; }
        public string UUID { get; set; }
        public string ImportType { get; set; }

        public IEnumerable<SelectListItem> showGroupList
        {
            get { return SystemListUtil.GetGroupList(); }
        }

        public IEnumerable<SelectListItem> showTypeList
        {
            get { return SystemListUtil.RetrieveCategoryListByRegType(RegistrationConstant.REGISTRATION_TYPE_CGA); }
        }

        public IEnumerable<SelectListItem> yearList {
            get { return SystemListUtil.RetrieveYearList(); }
        }
    }

}