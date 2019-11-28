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
    public class Fn10RPT_AFCModel : DisplayGrid, IValidatableObject
    {

        public static string SORT_RECEIVED_DATE = "Received Date";
        public static string SORT_SUBMISSION = "Submission No.";


        public string HandlingOfficer { get; set; }
        public string SortBy { get; set; }

        [Display(Name = "Period From")]
        public DateTime DateFrom { get; set; }

        [Display(Name = "Period To")]
        public DateTime DateTo { get; set; }

        public List<SelectListItem> HandlingOfficerList { get; set; } = new List<SelectListItem>
        {            new SelectListItem{ Value = "", Text = "-All-" }
            
        };

        public List<SelectListItem> SortByList { get; set; } = new List<SelectListItem>
        {
            new SelectListItem{ Value = SORT_RECEIVED_DATE, Text = "Received Date" },
            new SelectListItem{ Value = SORT_SUBMISSION,Text = "Submission No." },
        };

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Mandatory(this, "DateFrom");
            yield return ValidationUtil.Validate_Mandatory(this, "DateTo");
        }
    }
}