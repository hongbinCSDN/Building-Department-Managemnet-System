using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Admin.Models
{
    public class PEM1103NumValidatedStructuresModel
    {
        public string UUID { get; set; }
        public string CurrentYearStartDate { get; set; } = new DateTime(DateTime.Now.Year,1,1).ToString("yyyy-MM-dd");
        public string CurrentDate { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
        public string Number { get; set; }
    }
}