using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn10RPT_TSRModel
    {
    }

    public class Fn10RPT_TSRSearchModel : DisplayGrid, IValidatableObject
    {
        public string ReportType { get; set; }
        public Nullable<System.DateTime> DateFrom { get; set; }
        public Nullable<System.DateTime> DateTo { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Mandatory(this, "DateFrom");
            yield return ValidationUtil.Validate_Mandatory(this, "DateTo");
        }
    }

    public class Fn10RPT_TSRGenerateModel : DisplayGrid
    {
        public List<TypeOfForm> TypeOfForms { get; set; }
    }
    public class TypeOfForm
    {
        public string Form { get; set; }
        public string Aug { get; set; }
        public string Total { get; set; }
    }
}