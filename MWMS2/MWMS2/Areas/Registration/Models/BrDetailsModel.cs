using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Models
{

    public class BrDetailsModel : DisplayGrid
    {
        public string BR_NO { get; set; }
        public string RegType { get; set; }
    }
}