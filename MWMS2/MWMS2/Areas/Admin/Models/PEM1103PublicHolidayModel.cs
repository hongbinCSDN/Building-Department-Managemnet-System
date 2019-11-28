using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Admin.Models
{
    public class PEM1103PublicHolidayModel
    {
        public string Year { get; set; }
        public IEnumerable<SelectListItem> YearList
        {
            get { return SystemListUtil.RetrieveYearList(); }
        }
        public List<Holiday> holidays { get; set; }
    }

    public class Holiday
    {
        public string UUID { get; set; }
        public string Date { get; set; }
        public string HolidayName_Desc { get; set; }
    }
}