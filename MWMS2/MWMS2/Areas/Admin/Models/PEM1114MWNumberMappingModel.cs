using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Admin.Models
{
    public class PEM1114MWNumberMappingModel : DisplayGrid
    {
        public string Reference_No { get; set; }
        public List<string> UserIDList { get; set; }


    }
}