using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class VerificaionFormModel : IValidatableObject
    {
        public string CurrentRankOfLoginAc { get; set; }
        public string R_UUID { get; set; }
        public string V_UUID { get; set; }
        public string HandlingUnit { get; set; }
        public P_MW_RECORD P_MW_RECORD { get; set; }
        public P_MW_VERIFICATION P_MW_VERIFICATION { get; set; }
        public P_MW_REFERENCE_NO P_MW_REFERENCE_NO { get; set; }

        public P_MW_DSN P_MW_DSN { get; set; }

        public List<P_MW_APPOINTED_PROFESSIONAL> P_MW_APPOINTED_PROFESSIONALs { get; set; }

        public List<P_MW_SCANNED_DOCUMENT> P_MW_SCANNED_DOCUMENTs { get; set; }
        public List<P_MW_SCANNED_DOCUMENT> P_MW_SCANNED_DOCUMENTsIC { get; set; }
        public List<P_MW_SCANNED_DOCUMENT> P_MW_SCANNED_DOCUMENTsNIC { get; set; }

        public P_MW_RECORD_PSAC P_MW_RECORD_PSAC { get; set; }
        public P_MW_RECORD_SAC P_MW_RECORD_SAC { get; set; }

        public P_MW_RECORD_PSAC_IMAGE P_MW_RECORD_PSAC_IMAGE { get; set; }
        public List<P_MW_RECORD_PSAC_IMAGE> P_MW_RECORD_PSAC_IMAGEs { get; set; }
        public List<P_MW_RECORD_PSAC_IMAGE> P_MW_RECORD_PSAC_IMAGEsPlan { get; set; }
        public List<P_MW_RECORD_PSAC_IMAGE> P_MW_RECORD_PSAC_IMAGEsPhoto { get; set; }

        public List<P_MW_RECORD_ITEM> P_MW_RECORD_ITEMs { get; set; }
        public List<P_MW_RECORD_ITEM> FilterP_MW_RECORD_ITEMs { get; set; }
        public List<P_MW_RECORD_ITEM> FinalP_MW_RECORD_ITEMs { get; set; }
        public P_MW_RECORD_FORM_CHECKLIST P_MW_RECORD_FORM_CHECKLIST { get; set; }

        public List<P_MW_RECORD_ITEM_CHECKLIST_ITEM> P_MW_RECORD_ITEM_CHECKLIST_ITEMs { get; set; }

        public List<RecordItemCheckListItem> RecordItemCheckListItems { get; set; }

        public List<P_S_MW_ITEM_CHECKBOX> P_S_MW_ITEM_CHECKBOXs { get; set; }
        public List<P_S_MW_ITEM_NATURE> P_S_MW_ITEM_NATUREs { get; set; }
        public P_MW_APPOINTED_PROFESSIONAL AppointedAP { get; set; }
        public P_MW_APPOINTED_PROFESSIONAL AppointedRSE { get; set; }
        public P_MW_APPOINTED_PROFESSIONAL AppointedRGE { get; set; }
        public P_MW_APPOINTED_PROFESSIONAL AppointedPRC { get; set; }
        public P_MW_APPOINTED_PROFESSIONAL AppointedOther0 { get; set; }
        public P_MW_APPOINTED_PROFESSIONAL AppointedOther1 { get; set; }
        public P_MW_APPOINTED_PROFESSIONAL AppointedAPForm8 { get; set; }


        public List<SelectListItem> YNList { get; set; } = new List<SelectListItem>()
        {
            new SelectListItem{ Value = "", Text="- Select -" }
            ,new SelectListItem{ Value = "Y", Text="Yes" }
            ,new SelectListItem{ Value = "N", Text="No" }
        };
        public List<SelectListItem> ENEList { get; set; } = new List<SelectListItem>()
        {
            new SelectListItem{ Value = "", Text="- Select -" }
            ,new SelectListItem{ Value = "Y", Text="Erected" }
            ,new SelectListItem{ Value = "N", Text="Not erected" }
        };

        public List<P_MW_ADDRESS> P_MW_ADDRESSes { get; set; }

        public P_MW_ADDRESS P_MW_ADDRESS { get; set; }

        public List<SelectListItem> FolderTypeList { get; set; } = new List<SelectListItem>()
        {
            new SelectListItem{ Value = "Public", Text="Public" }
            ,new SelectListItem{ Value = "Private", Text="Private" }
        };


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.FLAT");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.FLOOR");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.STREET_NO");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.BUILDING_NAME");
            yield return ValidationUtil.Validate_Length(this, "P_MW_ADDRESS.STREE_NAME");
        }

        public P_MW_SUMMARY_MW_ITEM_CHECKLIST P_MW_SUMMARY_MW_ITEM_CHECKLIST { get; set; }

        public List<P_MW_FORM_09> P_MW_FORM_09s { get; set; }

        public P_MW_FORM P_MW_FORM { get; set; }

        public bool IsSummary { get; set; }

        public DateTime ActiveDate
        {
            get { return Convert.ToDateTime(ProcessingConstant.ACTIVE_DATE); }
        }

        public string ProfessionalType { get; set; }
        public string CheckPage { get; set; }
        public string FinalRecordItem { get; set; }
        public P_MW_RECORD FinalRecord { get; set; }
        public List<PreMwItem> PreMwItems { get; set; }
        public List<P_MW_RECORD_ITEM_CHECKLIST> P_MW_RECORD_ITEM_CHECKLISTs { get; set; }
        public List<PreRecordItemChecklist> PreRecordItemChecklists { get; set; }
        public List<RecordItemCheckListItem> FinalRecordItemCheckListItems { get; set; }
        public bool IsReadonly { get; set; }
        public string SUBMISSION_TYPE { get; set; }
        public bool IsSubmit { get; set; }
        public bool IsSAC { get; set; }
        public string TaskUserID { get; set; }
        public List<SYS_POST> POList { get; set; }
    }

    public class RecordItem
    {
        public string UUID { get; set; }
        public string MW_RECORD_ID { get; set; }
        public string MW_ITEM_CODE { get; set; }
        public string MW_VERIFICATION_ID { get; set; }
        public string MW_RECORD_ITEM_CHECKLIST_ID { get; set; }
    }

    public class PreMwItem
    {
        public string UUID { get; set; }
        public string ItemCode { get; set; }
        public string Location { get; set; }
        public string DescOfVariation { get; set; }
        public string MatchItem { get; set; }
        public string Disable { get { return ProcessingConstant.FLAG_Y; } }
        public string FianlItemUUID { get; set; }
        public string REVISED_ITEM_ID { get; set; }
        public string CLASS_CODE { get; set; }
        public Nullable<decimal> ORDERING { get; set; }
        public bool IsGenerate { get; set; }
        public bool IsMatchItem
        {
            get { return MatchItem == "Y"; }
            set { MatchItem = (value) ? "Y" : "N"; }
        }
    }

    public class PreRecordItemChecklist
    {
        public string UUID { get; set; }
        public string MW_ITEM_CODE { get; set; }
        public Nullable<decimal> ORDERING { get; set; }
        public string VARIATION_DECLARED { get; set; }
        public bool? IsVARIATION_DECLARED
        {
            get { return VARIATION_DECLARED == "Y"; }
            set { VARIATION_DECLARED = (value == null) ? null : (value.Value) ? "Y" : "N"; }
        }
    }


    public class RecordItemCheckListItem
    {
        public string S_MW_ITEM_CHECKLIST_ITEM_ID { get; set; }
        public string MW_ITEM_NO { get; set; }
        public string CHECKLIST_NO { get; set; }
        public string DESCRIPTION { get; set; }
        public string CHECKLIST_TYPE { get; set; }
        public string VALUE_TYPE { get; set; }
        public Nullable<decimal> ORDERING { get; set; }
        public string ANSWER { get; set; }
        public string TEXT_ANSWER { get; set; }
        public string REMARKS { get; set; }
        public string PO_AGREEMENT { get; set; }
        public string PO_REMARK { get; set; }
        public string MW_RECORD_ITEM_CHECKLIST_ID { get; set; }
        public string CLASS_CODE { get; set; }
        public Nullable<decimal> CLASS_CODE_ORDERING { get; set; }
        public string ID { get; set; }
        public string SUBMISSION_TYPE { get; set; }
        public string UUID { get; set; }

    }

}