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
    public class DirectReturnRosterModel : DisplayGrid, IValidatableObject
    {
        public P_S_DIRECT_RETURN_ROSTER P_S_DIRECT_RETURN_ROSTER { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            yield return ValidationUtil.Validate_Mandatory(this, "P_S_DIRECT_RETURN_ROSTER.ON_DUTY_DATE");
            //yield return ValidationUtil.Validate_Length(this, "P_S_DIRECT_RETURN_ROSTER.OFFICER_TO");
            //yield return ValidationUtil.Validate_Length(this, "P_S_DIRECT_RETURN_ROSTER.OFFICER_PO");

            foreach (ValidationResult r in ValidationUtil.Validate_Properties_Length(this, P_S_DIRECT_RETURN_ROSTER)) yield return r;

        }
    }
}