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
    public class Sys_SectionModel:DisplayGrid,IValidatableObject
    {
        public string CODE { get; set; }
        public string DESCRIPTION { get; set; }
        public string FAX { get; set; }
        public SYS_SECTION SYS_SECTION { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Mandatory(this, "SYS_SECTION.CODE");

            yield return ValidationUtil.Validate_Length(this, "SYS_SECTION.DESCRIPTION");
            yield return ValidationUtil.Validate_Length(this, "SYS_SECTION.CODE");
            yield return ValidationUtil.Validate_Length(this, "SYS_SECTION.FAX");
        }
    }

}