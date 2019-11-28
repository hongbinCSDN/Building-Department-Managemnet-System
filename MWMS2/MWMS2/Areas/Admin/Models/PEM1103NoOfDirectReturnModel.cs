using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Admin.Models
{
    public class PEM1103NoOfDirectReturnModel
    {
        public string Month { get; set; }
        public string Year { get; set; }
        public IEnumerable<SelectListItem> MonthList
        {
            get
            {
                return (new List<SelectListItem>())
                    .Concat(SystemListUtil.getMonth()
                        .Select(o => new SelectListItem()
                        {
                            Text = SystemListUtil.MonthEnglishName(o.ToString()),
                            Value = o.ToString()
                        }
                        ));

            }
        }
        public IEnumerable<SelectListItem> YearList
        {
            get { return SystemListUtil.RetrieveYearList(); }
        }

        public List<CalendarModel> calendarModels { get; set; }
    }

    public class CalendarModel
    {
        public string Date { get; set; }
        public int Number { get; set; }
        public int Counter { get; set; }
        public int Week { get; set; }
    }
}