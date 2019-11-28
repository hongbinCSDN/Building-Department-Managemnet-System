using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Models;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn10RPT_MWMSWCCModel : DisplayGrid
    {
        public string CommFromDate { get; set; }
        public string CommToDate { get; set; }
        public string ReceivedFromDate { get; set; }
        public string ReceivedToDate { get; set; }
        public bool FormTypeMW01 { get; set; }
        public bool FormTypeMW03 { get; set; }
        public List<Checkboxlist> StatusFormMW0204
        {
            get;
            set;
        }
        public string PBPReg { get; set; }
        public string PRCReg { get; set; }
    }

    public class Checkboxlist
    {
        public Checkboxlist()
        {
            IsChecked = false;
        }

        public string UUID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsChecked { get; set; }
    }
}