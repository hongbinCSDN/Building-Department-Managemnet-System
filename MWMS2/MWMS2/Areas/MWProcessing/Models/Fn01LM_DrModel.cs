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
    public class Fn01LM_DRSearchModel : DisplayGrid
    {
        public string DSN { get; set; }
        public string FORM_TYPE { get; set; }
        public string CONTRACTOR_REG_NO { get; set; }
        public DateTime? RECEIVED_DATE { get; set; }
        public string HANDING_STAFF_1 { get; set; }
        public string HANDING_STAFF_2 { get; set; }
        public string HANDING_STAFF_3 { get; set; }
        public bool IsToday { get; set; }
        public bool IsMaintenance { get; set; }

        public Irregularities DrIrregularities { get; set; }

        public List<Irregularities> IrregularitiesList { get; set; }

    }
    public class Fn01LM_DRStatModel : DisplayGrid, IValidatableObject
    {
       
        [Display(Name = "From Date")]
        public DateTime? DateFrom { get; set; }

        [Display(Name = "To Date")]
        public DateTime? DateTo { get; set; }
        [Display(Name = "Period Date")]
        public string PeriodDate { get { return DateFrom == null && DateTo == null ? null:"1"; } }

        [Display(Name = "Reg no. of AP/ RI / Contractor")]
        public string RegNo { get; set; }

        public List<Irregularities> IrregularitiesList { get; set; }

        public P_MW_DIRECT_RETURN DR { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Key))
            {
                ValidationResult DateFromNull = ValidationUtil.Validate_Mandatory(this, "DateFrom");
                ValidationResult DateToNull = ValidationUtil.Validate_Mandatory(this, "DateTo");
                ValidationResult RegNoNull = ValidationUtil.Validate_Mandatory(this, "RegNo");
                if (DateFromNull == null && DateToNull != null) yield return DateToNull;
                if (DateToNull == null && DateFromNull != null) yield return DateFromNull;
                if (DateToNull != null && DateFromNull != null || RegNoNull != null)
                {
                    ValidationResult r =  ValidationUtil.Validate_Mandatory(this, "RegNo", "PeriodDate");
                    yield return r;
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    public class Fn01LM_DRSaveModel : IValidatableObject
    {


        public bool IsUpdate { get; set; }
        public string CONTRACTOR_REG_NAME { get; set; }

        public List<Irregularities> IrregularitiesList { get; set; }
        public P_MW_DIRECT_RETURN P_MW_DIRECT_RETURN { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Length(this, "P_MW_DIRECT_RETURN.DSN");
            yield return ValidationUtil.Validate_Length(this, "P_MW_DIRECT_RETURN.FORM_TYPE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_DIRECT_RETURN.CONTRACTOR_REG_NO");
            yield return ValidationUtil.Validate_Length(this, "P_MW_DIRECT_RETURN.RECEIVED_DATE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_DIRECT_RETURN.HANDING_STAFF_1");
            yield return ValidationUtil.Validate_Length(this, "P_MW_DIRECT_RETURN.HANDING_STAFF_2");
            yield return ValidationUtil.Validate_Length(this, "P_MW_DIRECT_RETURN.HANDING_STAFF_3");

            //yield return ValidationUtil.Validate_Mandatory(this, "P_MW_DIRECT_RETURN.DSN");
            yield return ValidationUtil.Validate_Mandatory(this, "P_MW_DIRECT_RETURN.RECEIVED_DATE");
            yield return ValidationUtil.Validate_Mandatory(this, "P_MW_DIRECT_RETURN.FORM_TYPE");
            yield return ValidationUtil.Validate_Mandatory(this, "P_MW_DIRECT_RETURN.CONTRACTOR_REG_NO");

            yield return ValidationUtil.Validate_FormNo(P_MW_DIRECT_RETURN.FORM_TYPE, "P_MW_DIRECT_RETURN.FORM_TYPE", "MWForm");

        }
    }

    public class Irregularities
    {
        public Irregularities()
        {
            Is_Checked = "False";
        }
        public string UUID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
        public bool IsChecked
        {
            get { return Is_Checked.ToUpper() == "TRUE"; }
            set { Is_Checked = value.ToString(); }
        }
        public string Is_Checked { get; set; }

    }
}