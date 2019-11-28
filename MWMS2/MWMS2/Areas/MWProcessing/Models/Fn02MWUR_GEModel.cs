using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Models;
using MWMS2.Entity;
using System.Web.Mvc;
using MWMS2.Utility;
using System.ComponentModel.DataAnnotations;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn02MWUR_GEModel : DisplayGrid, IValidatableObject
    {
        public string DSNID { get; set; }
        public string DSN { get; set; }
        public string RefNo { get; set; }
        public string ReceiveFormDate { get; set; }
        public string ReceiveToDate { get; set; }
        public Fn02MWUR_GEEnquiryModel Enquiry { get; set; }
        public P_MW_GENERAL_RECORD P_MW_GENERAL_RECORD { get; set; }
        public P_MW_PERSON_CONTACT P_MW_PERSON_CONTACT { get; set; }
        public P_MW_ADDRESS P_MW_ADDRESS { get; set; }
        public List<P_MW_ADDRESS> P_MW_ADDRESSES { get; set; }
        public P_MW_REFERENCE_NO P_MW_REFERENCE_NO { get; set; }
        public P_MW_DSN P_MW_DSN { get; set; }
        public P_S_SYSTEM_VALUE Language { get; set; }
        public P_MW_COMPLAINT_CHECKLIST P_MW_COMPLAINT_CHECKLIST { get; set; }
        public List<P_MW_COMPLAINT_CHECKLIST_SECTION> P_MW_COMPLAINT_CHECKLIST_SECTIONs { get; set; }
        public List<P_MW_COMPLAINT_CHECKLIST_COMMENT> P_MW_COMPLAINT_CHECKLIST_COMMENTs { get; set; }
        public P_MW_COMMENT P_MW_COMMENT { get; set; }
        public List<P_MW_COMMENT> P_MW_COMMENTs { get; set; }
        public string Status { get; set; }
        public List<SelectListItem> DSNStatus
        {
            get
            {
                //return SystemListUtil.GetDSNStatus();
                return SystemListUtil.GetGeneralSubmissionStatusList();
            }
        }
        public bool IsReadOnly { get; set; }
        public bool IsSubmit { get; set; }
        public bool IsSPO { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.REFERENCE_NUMBER");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.ENGLISH_NAME");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.CHINESE_NAME");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.DATE_TIME");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.VENUE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.SECTION_UNIT_REF");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.CONTACT_ID");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.RECEIVE_DATE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.REFERRAL_DATE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.CASE_TITLE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.FINAL_REPLY_DATE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.SUBJECT_OFFICER");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.TO_CC");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.DB_RELATED");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.NATURE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.CLOSE_DATE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.RECORD_TYPE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.FOLLOW_UP");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.DESCRIPTION");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.PROGRESS");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.SUBMIT_TYPE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.STATUS");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.FORM_STATUS");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.FINAL_REPLY_DUE_DATE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.INTERIM_REPLY_DATE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.INTERIM_REPLY_DUE_DATE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.CHANNEL");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.SOURCE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.SUBJECT_MATTER");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.REMARK");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.ICC_NUMBER");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.ICC_TYPE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.ICC_OFFICER_TYPE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.ICC_OFFICER_ID");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.FLOW_STATUS");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.REMARK");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.ADDRESS_AREA");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.KEY_WORD");

            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.NAME_ENGLISH");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.NAME_CHINESE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.EMAIL");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.CONTACT_NO");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.FAX_NO");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.ID_TYPE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.ID_NUMBER");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.ID_ISSUE_COUNTRY");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.MW_ADDRESS_ID");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.CONTACT_PERSON_TITLE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.FIRST_NAME");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.LAST_NAME");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.MOBILE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.ENDORSEMENT_DATE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.SIGNATURE_DATE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.ADDRESS_SAME_A1");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.ADDRESS_SAME_A4");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.PRC");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.PBP");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.PRC_CONTACT_NO");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.PBP_CONTACT_NO");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.OTHER_ID_TYPE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.NAME_ENGLISH2");
            yield return ValidationUtil.Validate_Length(this, "P_MW_GENERAL_RECORD.NAME_CHINESE2");

            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.FLAT");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.FLOOR");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.BLOCK");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.BUILDING_NAME");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.STREET_NO");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.STREE_NAME");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.DISTRICT");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.REGION");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.LOCATION");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.ADDRESS_TYPE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.PERSON_CONTACT_ID");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.BLOCK_ID");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.UNIT_ID");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.STREET_LOCATION_TABLE_ID");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.ENGLISH_STREET_NAME");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.ENGLISH_STREET_TYPE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.ENGLISH_STREET_DIRECTION");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.ENGLISH_ST_TYPE_PRE_INDICATOR");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.ENGLISH_ST_LOCATION_NAME_1");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.ENGLISH_ST_LOCATION_NAME_2");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.ENGLISH_ST_LOCATION_NAME_3");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.CHINESE_STREET_NAME");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.CHINESE_STREET_TYPE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.CHINESE_STREET_DIRECTION");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.CHINESE_ST_TYPE_PRE_INDICATOR");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.CHINESE_ST_LOCATION_NAME_1");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.CHINESE_ST_LOCATION_NAME_2");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.CHINESE_ST_LOCATION_NAME_3");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.BUILDING_NO_NUMERIC");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.BUILDING_NO_ALPHA");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.BUILDING_NO_EXTENSION");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.BLOCK_ID_NUMERIC");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.BLOCK_ID_ALPHA");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.BLOCK_ID_ALPHA_PRE_INDICATOR");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.ENGLISH_BLOCK_DESCRIPTION");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.BLOCK_DESC_PRECEDE_INDICATOR");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.ENGLISH_BUILDING_NAME_LINE_1");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.ENGLISH_BUILDING_NAME_LINE_2");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.ENGLISH_BUILDING_NAME_LINE_3");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.CHINESE_BUILDING_NAME_LINE_1");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.CHINESE_BUILDING_NAME_LINE_2");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.CHINESE_BUILDING_NAME_LINE_3");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.ENGLISH_BLOCK_ADDRESS_2");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.ENGLISH_DEVELOPMENT_NAME");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.CHINESE_DEVELOPMENT_NAME");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.FLOOR_CODE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.ENGLISH_FLOOR_DESCRIPTION");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.CHINESE_FLOOR_DESCRIPTION");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.ENGLISH_UNIT_NO");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.ASSESSMENT_NO");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.ENGLISH_TENEMENT_DESCRIPTION");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.CHINESE_TENEMENT_DESCRIPTION");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.ENGLISH_RRM_ADDRESS");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.ENGLISH_RRM_BUILDING");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.CHINESE_RRM_ADDRESS");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.CHINESE_RRM_BUILDING");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.DISPLAY_STREET");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.DISPLAY_STREET_NO");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.DISPLAY_BUILDINGNAME");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.DISPLAY_FLOOR");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.DISPLAY_FLAT");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.DISPLAY_DISTRICT");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.DISPLAY_REGION");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.ENGLISH_DISPLAY");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.CHINESE_DISPLAY");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.DISPLAY_STREET_CODE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.DISPLAY_BLOCK_ID");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.RRM_SYN_STATUS");


        }
    }

    public class Fn02MWUR_GEEnquiryModel
    {
        public string UUID { get; set; }
        public string DSN { get; set; }
        public string CATEGORY_CODE { get; set; }
        public string REFERENCE_NO { get; set; }
    }

    //public class Fn02MWUR_GEFormModel
    //{
    //    public P_MW_DSN P_MW_DSN { get; set; }
    //    public P_MW_REFERENCE_NO P_MW_REFERENCE_NO { get; set; }
    //    public P_MW_GENERAL_RECORD P_MW_GENERAL_RECORD { get; set; }
    //    public P_MW_PERSON_CONTACT P_MW_PERSON_CONTACT { get; set; }
    //    public P_MW_ADDRESS P_MW_ADDRESS { get; set; }
    //    public List<P_MW_ADDRESS> P_MW_ADDRESSES { get; set; }
    //    public P_S_SYSTEM_VALUE Language { get; set; }
    //    public P_MW_COMPLAINT_CHECKLIST P_MW_COMPLAINT_CHECKLIST { get; set; }
    //    public List<SelectListItem> DSNStatus
    //    {
    //        get
    //        {
    //            return SystemListUtil.GetDSNStatus();
    //        }
    //    }
    //}
}