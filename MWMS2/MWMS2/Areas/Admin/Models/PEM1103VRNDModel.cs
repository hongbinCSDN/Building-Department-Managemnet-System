using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Admin.Models
{
    public class PEM1103VRNDModel
    {
    }

    public class PEM1103VRNDSearchModel : DisplayGrid
    {

        public Dictionary<string, decimal> DN { get; set; }
        public Dictionary<string, string> DNcompare { get; set; }

        public Dictionary<string, decimal> CL1Value { get; set; }
        public Dictionary<string, string> CL1Compare { get; set; }

        public Dictionary<string, decimal> CL2Value { get; set; }
        public Dictionary<string, string> CL2Compare { get; set; }

        public Dictionary<string, decimal> RefusalValue { get; set; }
        public Dictionary<string, string> RefusalCompare { get; set; }

        public IEnumerable<SelectListItem> DNCompareList
        {
            get
            {
                return (new List<SelectListItem>()
                {
                    new SelectListItem{Value = "LARGER_THAN_OR_EQUAL" , Text=">="},
                    new SelectListItem{Value = "SMALLER_THAN_OR_EQUAL" , Text="<="}
                });
            }
        }

        public IEnumerable<SelectListItem> CLCompareList
        {
            get
            {
                return (new List<SelectListItem>()
                {
                    new SelectListItem{Value = "SMALLER_THAN" , Text="<"},
                    new SelectListItem{Value = "SMALLER_THAN_OR_EQUAL" , Text="<="}
                });
            }
        }

        public IEnumerable<SelectListItem> RefusalCompareList
        {
            get
            {
                return (new List<SelectListItem>()
                {
                    new SelectListItem{Value = "SMALLER_THAN_OR_EQUAL" , Text="X <="},
                    new SelectListItem{Value = "NA" , Text="N/A"},
                    new SelectListItem{Value = "LARGER_THAN" , Text="X >"}
                });
            }
        }


    }
}