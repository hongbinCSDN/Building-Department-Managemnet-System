using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Models
{   
    public class Fn10RPT_SRSearchModel : DisplayGrid, IValidatableObject
    {
        public string ReportType { get; set; }
        public string WorkClass { get; set; }

        [Display(Name = "Period From")]
        public DateTime DateFrom { get; set; }

        [Display(Name = "Period To")]
        public DateTime DateTo { get; set; }

        public List<SelectListItem> ClassCodeList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem{ Value = "", Text = "- ALL -" }
            ,new SelectListItem{Value = "CLASS_I",Text="Class I"}
            ,new SelectListItem{Value = "CLASS_II",Text = "Class II"}
            ,new SelectListItem{Value = "CLASS_III",Text = "Class III"}
        };

        public List<SelectListItem> ReportTypeList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem{ Value = "", Text = "- ALL -" },
            new SelectListItem{ Value = "MW",Text = "Minor Works" },
            new SelectListItem{ Value = "VS", Text = "Validation Scheme" },
            new SelectListItem{ Value = "Enq", Text = "Enquiry" },
            new SelectListItem{ Value = "Com", Text = "Complaint" },

        };

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Mandatory(this, "DateFrom");
            yield return ValidationUtil.Validate_Mandatory(this, "DateTo");
        }
    }
}