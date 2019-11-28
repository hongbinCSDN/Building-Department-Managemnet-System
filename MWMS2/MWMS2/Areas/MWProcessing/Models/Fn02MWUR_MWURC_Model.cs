using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static MWMS2.Utility.ValidationUtil;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn02MWUR_MWURC_Model : DisplayGrid, IValidatableObject
    {
        public string DocType { get; set; }
        public string FormNo { get; set; }
        public bool VsForMW01_MW03 { get; set; }
        public string TypeOfRefNo { get; set; }
        public string InputRefNo { get; set; }
        public string ToBePrintDSN { get; set; }
        public string ToBePrintSubmissionNo { get; set; }
        public string ToBePrintFormNo { get; set; }
        public List<P_MW_DSN> MwDsnList { get; set; } = new List<P_MW_DSN>();
        public List<DisplaySubmissionObj> submissionList { get; set; } = new List<DisplaySubmissionObj>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_FormNo(FormNo, "FormNo", DocType);
            yield return Validate_InputRefNo(FormNo, "InputRefNo", InputRefNo);
        }

        public ValidationResult Validate_InputRefNo(string formNo, string propName, string refNo)
        {
            MWPNewSubmissionBLService mWPNewSubmissionBLService = new MWPNewSubmissionBLService();
            return mWPNewSubmissionBLService.Validate_InputRefNo(formNo, propName, this);
        }
    }

    // display obj
    public class DisplaySubmissionObj
    {
        public string RecDate { get; set; }
        public string Time { get; set; }
        public string Dsn { get; set; }
        public string RefNo { get; set; }
        public string FormNo { get; set; }
    }
}