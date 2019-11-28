using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Models;
using System.Web.Mvc;
using MWMS2.Utility;

namespace MWMS2.Areas.Admin.Models
{
    public class CRM_ARModel: DisplayGrid
    {
        public string CODE { get; set; }
        public string RegType { get; set; }
        public IEnumerable<SelectListItem> RegTypeList { set; get; } = SystemListUtil.RetrieveRegType();
        public string EngDesc { get; set; }
        public string ChiDesc { get; set; }
        public Nullable<decimal> ORDERING { get; set; }
        public bool IsActive { get; set; }

      //  public C_S_SYSTEM_VALUE C_S_SYSTEM_VALUE { get; set; }

    }
}