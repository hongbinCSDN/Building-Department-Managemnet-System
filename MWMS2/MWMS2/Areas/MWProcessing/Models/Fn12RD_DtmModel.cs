using MWMS2.Entity;
using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn12RD_DtmModel:DisplayGrid
    {
        public string errorMsg { get; set; }

        public string DSN { get; set; }

        public List<P_MW_DSN> DSN_LIST  { get; set; } = new List<P_MW_DSN>();



    }
}