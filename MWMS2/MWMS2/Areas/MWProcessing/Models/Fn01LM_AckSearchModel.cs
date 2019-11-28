using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn01LM_AckSearchModel : DisplayGrid, IValidatableObject
    {
        //public string Counter { get; set; }
        //public string Nature { get; set; }
        //public string PBBW { get; set; }
        //public string OrderRelated { get; set; }
        //public string SSP { get; set; }
        //public string Language { get; set; }
        //public string FileType { get; set; }
        //public string Barcode { get; set; }
        public string Repeated { get; set; }
        public string SearchDSN { get; set; }
        //public string SearchReceivedDate { get; set; }
        public P_MW_ACK_LETTER P_MW_ACK_LETTER { get; set; }
        public P_MW_DSN P_MW_DSN { get; set; }
        public V_CRM_INFO PBP { get; set; }
        public V_CRM_INFO PRC { get; set; }

        public string LANGUAGE_RADIO_ENGLISH = ProcessingConstant.LANGUAGE_RADIO_ENGLISH;
        public string LANGUAGE_RADIO_CHINESE = ProcessingConstant.LANGUAGE_RADIO_CHINESE;
        public string DOC_TYPE_PDF = ProcessingConstant.DOC_TYPE_PDF;
        public string DOC_TYPE_DOCX = ProcessingConstant.DOC_TYPE_DOCX;

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

    public class Fn01LM_AckPrintModel
    {
        public string FormOn { get; set; }
        public string mwno { get; set; }
        public string enqno { get; set; }
        public string pbpName { get; set; }
        public string pbpContact { get; set; }
        public string pbpAddress { get; set; }
        public string prcName { get; set; }
        public string prcContact { get; set; }
        public string prcAddress { get; set; }
        private string _address { get; set; }
        public string address
        {
            get { return _address.ToUpper(); }
            set { _address = string.IsNullOrEmpty(value) ? "" : value.ToUpper(); }
        }
        public string unit { get; set; }
        public string floor { get; set; }
        public string building { get; set; }
        public string streetNo { get; set; }
        public string street { get; set; }
        public string ryear { get; set; }
        public string rmonth { get; set; }
        public string rday { get; set; }
        public string paw { get; set; }
        public string pawContact { get; set; }
        public string lyear { get; set; }
        public string lmonth { get; set; }
        public string lday { get; set; }
        public string letterDate { get; set; }
        public string spoPosition { get; set; }
        public string spoName { get; set; }

        public string PBPNo { get; set; }
        public string PRCNo { get; set; }

        public string Language { get; set; }
        public string ReceiveYear { get; set; }
        public string ReceiveMonth { get; set; }
        public string ReceiveDay { get; set; }
    }
}