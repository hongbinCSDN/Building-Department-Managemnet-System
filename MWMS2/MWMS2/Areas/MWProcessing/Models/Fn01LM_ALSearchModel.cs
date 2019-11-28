using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn01LM_ALSearchModel : DisplayGrid ,IValidatableObject
    {
        public P_S_TO_DETAILS P_S_TO_DETAILS { get; set; }
        public P_MW_ACK_LETTER P_MW_ACK_LETTER { get; set; }
        public List<SelectListItem> SelectListPOPosts
        {
            get
            {
                return SystemListUtil.GetActiveSToDetails();
            }
        }

        public string SearchDSN { get; set; }

        public bool IsSearchDSN { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Mandatory(this, "P_MW_ACK_LETTER.DSN");

            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.DSN");
            yield return ValidationUtil.Validate_Length(this, "P_S_TO_DETAILS.PO_POST");
            yield return ValidationUtil.Validate_Length(this, "P_S_TO_DETAILS.PO_NAME_CHI");
        }
    }
}