using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn01SCUR_MMSearchModel : DisplayGrid, IValidatableObject
    {
        public string Counter { get; set; }
        public string Nature { get; set; }
        public string PBBW { get; set; }
        public string OrderRelated { get; set; }
        public string SSP { get; set; }
        public string Language { get; set; }
        public string FileType { get; set; }
        public string Barcode { get; set; }
        public string Repeated { get; set; }
        public string SearchDSN { get; set; }
        //public string SearchReceivedDate { get; set; }
        public P_MW_ACK_LETTER P_MW_ACK_LETTER { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.RECEIVED_DATE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.LETTER_DATE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.DSN");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.MW_NO");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.FORM_NO");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.COMP_DATE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.MW_ITEM");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.PBP_NO");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.PRC_NO");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.COMM_DATE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.ADDRESS");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.PAW");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.IO_MGT");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.IO_MGT_CONTACT");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.REMARK");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.REPEATED");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.LANGUAGE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.FILE_TYPE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.RECEIVED_DATE_FIXED");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.PREVIOUS_RELATED_MW_NO");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.STREET");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.STREET_NO");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.BUILDING");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.FLOOR");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.UNIT");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.AUDIT_RELATED");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.ORDER_RELATED");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.SIGNBOARD_RELATED");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.SDF_RELATED");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.REFERRAL_DATE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.BARCODE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.TO_POST");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.FILEREF_TWO");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.FILEREF_FOUR");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.EMAIL_OF_PAW");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.EMAIL_OF_PRC");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.EMAIL_OF_PBP");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ACK_LETTER.SUPERSEDING_MW_REF_NO");

            yield return ValidationUtil.Validate_Mandatory(this, "P_MW_ACK_LETTER.DSN");

        }
    }


}