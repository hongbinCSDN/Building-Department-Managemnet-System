using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn01LM_PcsaModel
    {
    }

    public class Fn01LM_PcsaSearchModel:DisplayGrid, IValidatableObject
    {
        public string DSN { get; set; }
        public string RefNo { get; set; }
        public Nullable<System.DateTime> SelectionDateFrom { get; set; }
        public Nullable<System.DateTime> SelectionDateTo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Length(this, "SelectionDateFrom");
            yield return ValidationUtil.Validate_Length(this, "SelectionDateTo");
        }
        public bool IsGeneral { get; set; }
    }

    public class PsacDetailModel
    {
        public P_MW_DSN P_MW_DSN { get; set; }
        public V_CRM_INFO PbpInfo { get; set; }
        public V_CRM_INFO PrcInfo { get; set; }
        public P_MW_ACK_LETTER P_MW_ACK_LETTER { get; set; }
        public P_MW_ACK_LETTER_PREAUDIT P_MW_ACK_LETTER_PREAUDIT { get; set; }
    }
}