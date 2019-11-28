using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI;
using MWMS2.Utility;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn01LM_OtherSearchModel
    {

    }

    public class Fn01LM_OtherSaveModel : IValidatableObject
    {
        //public string ReceivedDate { get; set; }
        //public string ReturnOverCounter { get; set; }
        //public string DSN { get; set; }
        //public string OrderRelated { get; set; }
        //public string MWNo { get; set; }
        //public string FileReferenceFour { get; set; }
        //public string FileReferenceTwo { get; set; }
        //public string ReceiveDateFrom { get; set; }
        //public string ReceiveDateTo { get; set; }
        //public string REferralDate { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{

        //    yield return ValidationUtil.Validate_Length(this, "ReceivedDate");
        //    yield return ValidationUtil.Validate_Length(this, "ReturnOverCounter");
        //    yield return ValidationUtil.Validate_Length(this, "DSN");
        //    yield return ValidationUtil.Validate_Length(this, "OrderRelated");
        //    yield return ValidationUtil.Validate_Length(this, "MWNo");
        //    yield return ValidationUtil.Validate_Length(this, "FileReferenceFour");
        //    yield return ValidationUtil.Validate_Length(this, "FileReferenceTwo");
        //    yield return ValidationUtil.Validate_Length(this, "ReceiveDateFrom");
        //    yield return ValidationUtil.Validate_Length(this, "ReceiveDateTo");
        //    yield return ValidationUtil.Validate_Length(this, "REferralDate");


        //    //yield return ValidationUtil.Validate_Mandatory(this, "DSN");
        //    //yield return ValidationUtil.Validate_Mandatory(this, "ReceiveDateFrom");
        //    //yield return ValidationUtil.Validate_Mandatory(this, "ReceiveDateTo");

        //}

        //public Fn01LM_OtherDROverCounterModel Fn01LM_OtherDROverCounterModel { get; set; }
        //public Fn01LM_OtherChangeORS Fn01LM_OtherChangeORS { get; set; }
        //public Fn01LM_OtherUpdateFRN Fn01LM_OtherUpdateFRN { get; set; }
        //public Fn01LM_OtherBatchUpdateRD Fn01LM_OtherBatchUpdateRD { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            yield return ValidationUtil.Validate_Length(this, "Fn01LM_OtherDROverCounterModel.ReceivedDate");
            yield return ValidationUtil.Validate_Length(this, "Fn01LM_OtherDROverCounterModel.ReturnOverCounter");
            yield return ValidationUtil.Validate_Length(this, "Fn01LM_OtherChangeORS.DSN");
            yield return ValidationUtil.Validate_Length(this, "Fn01LM_OtherChangeORS.OrderRelated");
            yield return ValidationUtil.Validate_Length(this, "Fn01LM_OtherUpdateFRN.MWNo");
            yield return ValidationUtil.Validate_Length(this, "Fn01LM_OtherUpdateFRN.FileReferenceFour");
            yield return ValidationUtil.Validate_Length(this, "Fn01LM_OtherUpdateFRN.FileReferenceTwo");
            yield return ValidationUtil.Validate_Length(this, "Fn01LM_OtherBatchUpdateRD.ReceiveDateFrom");
            yield return ValidationUtil.Validate_Length(this, "Fn01LM_OtherBatchUpdateRD.ReceiveDateTo");
            yield return ValidationUtil.Validate_Length(this, "Fn01LM_OtherBatchUpdateRD.REferralDate");


            yield return ValidationUtil.Validate_Mandatory(this, "Fn01LM_OtherChangeORS.DSN");
            yield return ValidationUtil.Validate_Mandatory(this, "Fn01LM_OtherBatchUpdateRD.ReceiveDateFrom");
            yield return ValidationUtil.Validate_Mandatory(this, "Fn01LM_OtherBatchUpdateRD.ReceiveDateTo");

        }
    }

    public class Fn01LM_OtherDROverCounterModel : IValidatableObject
    {
        public string ReceivedDate { get; set; }
        public string ReturnOverCounter { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Length(this, "ReceivedDate");
            yield return ValidationUtil.Validate_Length(this, "ReturnOverCounter");

            yield return ValidationUtil.Validate_Mandatory(this, "ReceivedDate");
            yield return ValidationUtil.Validate_Mandatory(this, "ReturnOverCounter");
        }
    }

    public class Fn01LM_OtherChangeORS : IValidatableObject
    {
        public string DSN { get; set; }
        public string OrderRelated { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Length(this, "DSN");
            yield return ValidationUtil.Validate_Length(this, "OrderRelated");

            yield return ValidationUtil.Validate_Mandatory(this, "DSN");
            yield return ValidationUtil.Validate_Mandatory(this, "OrderRelated");
        }
    }

    public class Fn01LM_OtherUpdateFRN : IValidatableObject
    {
        public string MWNo { get; set; }
        public string FileReferenceFour { get; set; }
        public string FileReferenceTwo { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Length(this, "MWNo");
            yield return ValidationUtil.Validate_Length(this, "FileReferenceFour");
            yield return ValidationUtil.Validate_Length(this, "FileReferenceTwo");

            yield return ValidationUtil.Validate_Mandatory(this, "MWNo");
            yield return ValidationUtil.Validate_Mandatory(this, "FileReferenceFour");
            yield return ValidationUtil.Validate_Mandatory(this, "FileReferenceTwo");
        }
    }

    public class Fn01LM_OtherBatchUpdateRD: IValidatableObject
    {
        public string ReceiveDateFrom { get; set; }
        public string ReceiveDateTo { get; set; }
        public string ReferralDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Length(this, "ReceiveDateFrom");
            yield return ValidationUtil.Validate_Length(this, "ReceiveDateTo");
            yield return ValidationUtil.Validate_Length(this, "ReferralDate");

            yield return ValidationUtil.Validate_Mandatory(this, "ReceiveDateFrom");
            yield return ValidationUtil.Validate_Mandatory(this, "ReceiveDateTo");
            yield return ValidationUtil.Validate_Mandatory(this, "ReferralDate");
        }
    }
}