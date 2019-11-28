using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Models
{
    public class Fn04RPT_SVSBHSearchModel : DisplayGrid
    {
        public DateTime? SVSBBYPeriodFromDate { get; set; }
        public DateTime? SVSBBYPeriodToDate { get; set; }
        public DateTime? PeriodFromDate { get; set; }
        public DateTime? PeriodToDate { get; set; }
        public string District { get; set; }
        public string LSO { get; set; }
        public string SeniorOfficer { get; set; }
        public string Officer { get; set; }
        public IEnumerable<SelectListItem> DistrictList
        {
            get { return SystemListUtil.GetDistrictList(); }
        }
        public IEnumerable<SelectListItem> SeniorOfficerList
        {
            get { return SystemListUtil.GetSeniorOfficerList(); }
        }
        public IEnumerable<SelectListItem> LSOList
        {
            get { return SystemListUtil.GetLSOList(); }
        }
        public IEnumerable<SelectListItem> OfficerList
        {
            get { return SystemListUtil.GetOfficerList(); }
        }
    }
}