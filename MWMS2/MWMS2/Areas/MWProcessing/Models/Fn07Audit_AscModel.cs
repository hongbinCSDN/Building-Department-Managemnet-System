using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn07Audit_AscModel
    {
    }

    public class Fn07Audit_AscSearchPart1Model : DisplayGrid, IValidatableObject
    {
        public Nullable<System.DateTime> AuditAssignedDateFrom { get; set; }
        public Nullable<System.DateTime> AuditAssignedDateTo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Length(this, "AuditAssignedDateFrom");
            yield return ValidationUtil.Validate_Length(this, "AuditAssignedDateTo");
        }
    }

    public class Fn07Audit_AscSearchPart2Model : DisplayGrid, IValidatableObject
    {
        public string ReferenceNoType { get; set; }
        public string ReferenceNo { get; set; }
        public string ItemNo { get; set; }
        public string Class { get; set; }
        public string Status { get; set; }
        public string MwcoLocation { get; set; }
        public string Street { get; set; }
        public string StreetNo { get; set; }
        public string Block { get; set; }
        public string Floor { get; set; }
        public string Unit { get; set; }
        public string AppointedPerson { get; set; }
        public string ApEnglishName { get; set; }
        public string ApChineseName { get; set; }
        public string ApRnCertificate { get; set; }
        public string PrContractor { get; set; }
        public string PrcEnglishName { get; set; }
        public string PrcChineseName { get; set; }
        public string PrcRnCertificate { get; set; }

        public Nullable<System.DateTime> ReceivedDateFrom { get; set; }
        public Nullable<System.DateTime> ReceivedDateTo { get; set; }
        public Nullable<System.DateTime> CommencementDateFrom { get; set; }
        public Nullable<System.DateTime> CommencementDateTo { get; set; }
        public Nullable<System.DateTime> CompletionDateFrom { get; set; }
        public Nullable<System.DateTime> CompletionDateTo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Length(this, "ReceivedDateFrom");
            yield return ValidationUtil.Validate_Length(this, "ReceivedDateTo");
            yield return ValidationUtil.Validate_Length(this, "CommencementDateFrom");
            yield return ValidationUtil.Validate_Length(this, "CommencementDateTo");
            yield return ValidationUtil.Validate_Length(this, "CompletionDateFrom");
            yield return ValidationUtil.Validate_Length(this, "CompletionDateTo");
        }
    }
}