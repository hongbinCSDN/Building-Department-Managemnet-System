using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Models
{

    public class Fn01LM_ModModel : IValidatableObject
    {
        public Fn01LM_ModModel()
        {
            WorkTypeList = new List<SelectListItem>()
            {
                new SelectListItem{Value = "B" , Text="Building works"},
                new SelectListItem{Value = "S" , Text="Street works"}
            };

            HandingStaffList = new List<SelectListItem>()
            {
                new SelectListItem{Value = "BS1" , Text="BS1"},
                new SelectListItem{Value = "SE4" , Text="SE4"},
                new SelectListItem{Value = "BS6" , Text="BS6"}
            };
        }


        public P_MW_MODIFICATION P_MW_MODIFICATION { get; set; }
        public P_MOD_BD106 P_MOD_BD106 { get; set; }
        public List<P_MOD_BD106_ANNUAL_INSP> P_MOD_BD106_ANNUAL_INSP { get; set; }

        public List<P_MW_MODIFICATION_RELATED_MWNO> listMwNo { get; set; }

        public List<P_MOD_BD106_ITEM_View> listBD106Item { get; set; }

        public List<string> listMwRefNo { get; set; }

        public List<string> ListSiteInspectDate { get; set; }
        public List<string> ListSiteInspectComp { get; set; }

        public string WorkType { get; set; }

        public string HandingStaff { get; set; }
        public List<SelectListItem> WorkTypeList { set; get; }
        public List<SelectListItem> HandingStaffList { set; get; }
        public List<SelectListItem> GetResultSiteInspection()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text="In Order"
                    ,Value="In Order"
                }
                ,new SelectListItem()
                {
                    Text="Not In Order"
                    ,Value="Not In Order"
                }
            };
        }
        public List<SelectListItem> GetBD106Status()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text="Accept"
                    ,Value="Accept"
                }
                ,new SelectListItem()
                {
                    Text="Not Accept"
                    ,Value="Not Accept"
                }
            };
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.UUID");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.ADDRESS");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.APPLICANT_CAPACITY");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.APPLICANT_NAME");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.COUNTER");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.DESC_OF_MODI");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.DSN");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.EMAIL");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.FILE_TYPE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.FORM_NO");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.IS_BUILDING_WORKS");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.IS_STREET_WORKS");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.LANGUAGE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.LOC_OF_SUBJECT");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.LOT_NO");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.NATURE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.RECEIVED_DATE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.REFERENCE_NO");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.REGULATIONS");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.RRM_SYN_STATUS");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.SUPPORTING_DOCUMENT");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.UNABLE_TO_COMPLY_REASON");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.VALIDITY");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.CREATED_BY");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.CREATED_DATE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.MODIFIED_BY");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.MODIFIED_DATE");
            yield return ValidationUtil.Validate_Length(this, "P_MW_MODIFICATION.HANDING_STAFF");

            yield return ValidationUtil.Validate_Mandatory(this, "P_MW_MODIFICATION.DSN");

            yield return ValidationUtil.Validate_Email(this, "P_MW_MODIFICATION.EMAIL");

        }
    }

    public class P_MOD_BD106_ITEM_View
    {
        public string UUID { get; set; }
        public string CODE { get; set; }
        public string DESCRIPTION { get; set; }
        public string P_MOD_BD106_ID { get; set; }
        public string S_SYSTEM_VALUE_UUID { get; set; }
        public bool IsChecked
        {
            get { return (IS_CHECKED != null && IS_CHECKED.ToUpper() == "TRUE"); }
            set { IS_CHECKED = value.ToString(); }
        }
        private string IS_CHECKED { get; set; }
    }

    public class Fn01LM_SearchModModel : DisplayGrid, IValidatableObject
    {
        public string REFERENCE_NO { get; set; }
        public Nullable<System.DateTime> ReceivedDateFrom { get; set; }
        public Nullable<System.DateTime> ReceivedDateTo { get; set; }
        public string LOT_NO { get; set; }
        public string ADDRESS { get; set; }
        public string APPLICANT_CAPACITY { get; set; }
        public string APPLICANT_NAME { get; set; }
        public string HANDING_STAFF { get; set; }

        public List<SelectListItem> PoLookUpList { get; set; }
        public Dictionary<string, string> PoUserId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Mandatory(this, "ReceivedDateFrom");
            yield return ValidationUtil.Validate_Mandatory(this, "ReceivedDateTo");
        }
    }

    public class Fn01LM_PrintModModel
    {
        public string UUID { get; set; }
        public string PERMIT_NO { get; set; }
        public string REF_NO { get; set; }
        public string RECEIVED_DATE { get; set; }
        public string ISSUE_DATE { get; set; }
        public string ANNUAL_DATE { get; set; }
        public string ADDRESS { get; set; }
        public string ModEngSpoName { get; set; }
        public string ModChiSpoName { get; set; }
        public string ModEngPosition { get; set; }
        public string ModChiPosition { get; set; }
    }
}