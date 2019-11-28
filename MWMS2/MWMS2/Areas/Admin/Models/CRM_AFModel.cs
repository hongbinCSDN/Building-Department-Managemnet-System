using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Models;
using MWMS2.Utility;

namespace MWMS2.Areas.Admin.Models
{
    public class CRM_AFModel: DisplayGrid, IValidatableObject
    {
        public string CODE { get; set; }
        public string RegType { get; set; }
        public IEnumerable<SelectListItem> RegTypeList { set; get; } = SystemListUtil.RetrieveRegType();
        public string EngDesc { get; set; }
        public string ChiDesc { get; set; }
        public Nullable<decimal> Ord { get; set; }
        public bool IsActive { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            yield return ValidationUtil.Validate_Length(this, "CODE");
            yield return ValidationUtil.Validate_Length(this, "EngDesc");


            yield return ValidationUtil.Validate_Mandatory(this, "CODE");
            yield return ValidationUtil.Validate_Mandatory(this, "EngDesc");
            yield return ValidationUtil.Validate_Mandatory(this, "Ord");
        }
    }
}