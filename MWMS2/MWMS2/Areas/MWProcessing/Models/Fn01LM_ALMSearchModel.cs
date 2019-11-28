using MWMS2.Models;
using MWMS2.Services.ProcessingDAO.Services;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn01LM_ALMSearchModel:DisplayGrid ,IValidatableObject
    {
        public string DocumentSN { get; set; }
        public string SERemark { get; set; }

        public string MWItem { get; set; }
        public string PRC { get; set; }
        public string PBP { get; set; }
        public string ReceivedDateFrom { get; set; }
        public string ReceivedDateTo { get; set; }
        public string[] MWNOs { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Mandatory(this, "DocumentSN");
            yield return new ProcessingLetterModuleBLService().Validaton_DSN(DocumentSN, "DocumentSN");
        }
    }
}