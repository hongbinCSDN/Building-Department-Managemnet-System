using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Models;
namespace MWMS2.Areas.Registration.Models
{
    public class Fn04MCA_MRASearchModel : DisplayGrid
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Room { get; set; }
       

    }
}