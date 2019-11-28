using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Admin.Models
{
    public class PEM_UnitModel:DisplayGrid,IValidatableObject
    {
        public string SectionCode { get; set; }
        public string UnitCode { get; set; }
        public string Description { get; set; }
        public string Fax { get; set; }

        public SYS_UNIT SYS_UNIT { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Mandatory(this, "SYS_UNIT.CODE");

            yield return ValidationUtil.Validate_Length(this, "SYS_UNIT.CODE");
            yield return ValidationUtil.Validate_Length(this, "SYS_UNIT.DESCRIPTION");
            yield return ValidationUtil.Validate_Length(this, "SYS_UNIT.FAX");
        }
    }
}