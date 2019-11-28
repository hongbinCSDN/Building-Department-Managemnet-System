using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Entity;

namespace MWMS2.Areas.Signboard.Models
{
    public class Fn04RPT_PPJLSearchModel : DisplayGrid
    {
       public string searchPbpRegNo { get; set; }
       public string searchPbpEnglishName { get; set; }
       public string searchPbpChineseName { get; set; }
       public string numRecords {
            get;set;
        }
    }
}