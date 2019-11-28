using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Models
{
    public class DSNSearchModel : DisplayGrid
    {
        public string DSN { get; set; }
        public P_MW_DSN P_MW_DSN { get; set; }

        public string msg { get; set; }

        public List<SelectListItem> getSubmissionNature()
        {
            return SystemListUtil.GetSubmissionNatureList();
        }
    }
}