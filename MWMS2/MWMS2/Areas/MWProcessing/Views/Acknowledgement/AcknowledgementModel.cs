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
    public class AcknowledgementModel : IValidatableObject
    {
        public string R_UUID { get; set; }
        public string V_UUID { get; set; }
        public string HandlingUnit { get; set; }
        public P_MW_VERIFICATION P_MW_VERIFICATION { get; set; }
        public P_MW_RECORD P_MW_RECORD { get; set; }
        public P_MW_REFERENCE_NO P_MW_REFERENCE_NO { get; set; }

        public P_MW_DSN P_MW_DSN { get; set; }

        public List<P_MW_APPOINTED_PROFESSIONAL> P_MW_APPOINTED_PROFESSIONALs { get; set; }

        public List<P_MW_SCANNED_DOCUMENT> P_MW_SCANNED_DOCUMENTs { get; set; }
        public List<P_MW_SCANNED_DOCUMENT> P_MW_SCANNED_DOCUMENTsIC { get; set; }
        public List<P_MW_SCANNED_DOCUMENT> P_MW_SCANNED_DOCUMENTsNIC { get; set; }

        public P_MW_RECORD_PSAC P_MW_RECORD_PSAC { get; set; }
        public P_MW_RECORD_SAC P_MW_RECORD_SAC { get; set; }
        public P_MW_RECORD_SAC P_MW_RECORD_SAC_WL { get; set; }

        public bool IsSacWL { get; set; }

        public P_MW_RECORD_PSAC_IMAGE P_MW_RECORD_PSAC_IMAGE { get; set; }
        public List<P_MW_RECORD_PSAC_IMAGE> P_MW_RECORD_PSAC_IMAGEs { get; set; }
        public List<P_MW_RECORD_PSAC_IMAGE> P_MW_RECORD_PSAC_IMAGEsPlan { get; set; }
        public List<P_MW_RECORD_PSAC_IMAGE> P_MW_RECORD_PSAC_IMAGEsPhoto { get; set; }

        public List<P_MW_RECORD_ITEM> P_MW_RECORD_ITEMs { get; set; }
        public List<P_MW_RECORD_ITEM> FilterP_MW_RECORD_ITEMs { get; set; }
        public P_MW_RECORD_FORM_CHECKLIST P_MW_RECORD_FORM_CHECKLIST { get; set; }
        public P_MW_RECORD_FORM_CHECKLIST_PO P_MW_RECORD_FORM_CHECKLIST_PO { get; set; }

        public List<P_MW_FORM_09> P_MW_FORM_09s { get; set; }

        public List<P_MW_RECORD_ITEM_CHECKLIST_ITEM> P_MW_RECORD_ITEM_CHECKLIST_ITEMs { get; set; }

        public List<RecordItemCheckListItem> RecordItemCheckListItems { get; set; }

        public List<P_S_MW_ITEM_CHECKBOX> P_S_MW_ITEM_CHECKBOXs { get; set; }
        public List<P_S_MW_ITEM_NATURE> P_S_MW_ITEM_NATUREs { get; set; }

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

        public List<SelectListItem> OffenseList { get; set; }

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

        public bool IsSummary { get; set; }
        public bool IsSubmit { get; set; }

        public DateTime ActiveDate
        {
            get { return Convert.ToDateTime(ProcessingConstant.ACTIVE_DATE); }
        }

        public P_MW_APPOINTED_PROFESSIONAL AppointedAP { get; set; }
        public P_MW_APPOINTED_PROFESSIONAL AppointedRSE { get; set; }
        public P_MW_APPOINTED_PROFESSIONAL AppointedRGE { get; set; }
        public P_MW_APPOINTED_PROFESSIONAL AppointedPRC { get; set; }
        public P_MW_APPOINTED_PROFESSIONAL AppointedOther0 { get; set; }
        public P_MW_APPOINTED_PROFESSIONAL AppointedOther1 { get; set; }

        public List<SpecifiedMWRecordFormCheckItem> SpecifiedMWRecordFormCheckItemList { get; set; }

        public int MwItemVersion
        {
            get { return 1; }
        }
        public bool IsReadonly { get; set; }
        public bool IsSPO { get; set; }
        public bool IsSAC { get; set; }
        public string TaskUserID { get; set; }

        public P_MW_RECORD_AL_FOLLOW_UP P_MW_RECORD_AL_FOLLOW_UP { get; set; }

        public List<P_MW_RECORD_AL_FOLLOW_UP_OFFENCES> P_MW_RECORD_AL_FOLLOW_UP_OFFENCESs { get; set; }

        public P_MW_RECORD_WL_FOLLOW_UP P_MW_RECORD_WL_FOLLOW_UP { get; set; }
        public P_MW_RECORD_REFERRED_TO_LSS_EBD P_MW_RECORD_REFERRED_TO_LSS_EBD { get; set; }
    }

    public class SpecifiedMWRecordFormCheckItem
    {
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string ToChecking { get; set; }
        public string ToRemarks { get; set; }
        public string PoAgreement { get; set; }
        public string PoRemarks { get; set; }
        public string MwNumber { get; set; }
        public bool Signature { get; set; }
        public string SignatureUuid { get; set; }
        public string ColumnName { get; set; }
        public string Form09UUID { get; set; }
    }
}