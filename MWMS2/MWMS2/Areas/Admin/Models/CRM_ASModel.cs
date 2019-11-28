
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MWMS2.Areas.Admin.Models
{
    public class CRM_ASModel : DisplayGrid, IValidatableObject
    {
        public string CODE { get; set; }
        public string EngDesc { get; set; }
        public string ChiDesc { get; set; }
        public Nullable<decimal> Ord { get; set; }
        public bool IsActive { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Mandatory(this, "CODE");
            yield return ValidationUtil.Validate_Mandatory(this, "EngDesc");
            yield return ValidationUtil.Validate_Mandatory(this, "Ord");
            yield return ValidationUtil.Validate_Length(this, "CODE");
            yield return ValidationUtil.Validate_Length(this, "EngDesc");

        }
    }
}