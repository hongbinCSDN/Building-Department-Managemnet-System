using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class CheckboxModel
    {
        public CheckboxModel()
        {
            IsChecked = false;
        }

        public string UUID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
        public bool IsChecked { get; set; }
    }
}