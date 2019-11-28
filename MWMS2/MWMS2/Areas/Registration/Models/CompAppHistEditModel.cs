using MWMS2.App_Start;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Models
{

    public class CompAppHistEditModel : EditFormModel, IValidatableObject
    {
        public C_COMP_APPLICANT_INFO_MASTER C_COMP_APPLICANT_INFO_MASTER { get; set; }
        public string SelectedTypeApprove0 { get; set; }
        public string SelectedTypeApprove1 { get; set; }
        public string SelectedTypeApprove2 { get; set; }
        public string SelectedTypeApprove3 { get; set; }
        public string SelectedTypeApprove4 { get; set; }
        public string SelectedTypeApprove5 { get; set; }
        public string SelectedTypeApprove6 { get; set; }

        public string SelectedTypeApply0 { get; set; }
        public string SelectedTypeApply1 { get; set; }
        public string SelectedTypeApply2 { get; set; }
        public string SelectedTypeApply3 { get; set; }
        public string SelectedTypeApply4 { get; set; }
        public string SelectedTypeApply5 { get; set; }
        public string SelectedTypeApply6 { get; set; }

        public string AppFormId { get; set; }
        public DateTime? AppDate { get; set; }
        public string AppStatus { get; set; }
        public bool EFORM { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            return new List<ValidationResult>();
        }
    }
}