using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Models
{
    public class Fn01Search_CTICRSearchModel : DisplayGrid
    {
        public string date_type { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public IEnumerable<SelectListItem> TypeOfCategoryList { set; get; } = SystemListUtil.GetTypeOfCategory();
        public string TypeOfCategorys { get; set; }
        public string StatusListItem { get; set; }
        public IEnumerable<SelectListItem> StatusList
        {
            get { return SystemListUtil.GetStatusListItem(); }
        }
    }
}