using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;

namespace MWMS2.Areas.Signboard.Models
{
    public class ValidationDisplayModel : DisplayGrid
    {

        public string MODIFIED_DATE { get; set; }


        public string TaskType { get; set; }
        public string NextCheckingType { get; set; }
        public string SelectedValidationMWItem { get; set; }
        public string TargetValidationUUID { get; set; }
        public string TargetDSNUUID { get; set; }
        public B_SV_VALIDATION B_SV_VALIDATION { get; set; }
        public List<B_SV_SCANNED_DOCUMENT> SubmittedDocList { get; set; }

        public Dictionary<string, string> SubmittedDocListFolderType { get; set; }

        public List<SelectListItem> FolderType { get { return SystemListUtil.GetFolderType(); } set { } }
        public List<B_SV_RECORD_VALIDATION_ITEM> ValidationItemList { get; set; }
        public List<B_SV_RECORD_ITEM> SVRecordItemList { get; set; }
        public List<B_SV_PHOTO_LIBRARY> SVPLList { get; set; }


        public B_SV_APPOINTED_PROFESSIONAL AP_AP { get; set; }
        public B_SV_APPOINTED_PROFESSIONAL AP_RSE { get; set; }
        public B_SV_APPOINTED_PROFESSIONAL AP_RGE { get; set; }
        public B_SV_APPOINTED_PROFESSIONAL AP_PRC { get; set; }
        public string TO_POST { get; set; }
        public string PO_POST { get; set; }
        public string SPO_POST { get; set; }

        public string PBP_AP_RESULT { get; set; }
        public string PBP_RSE_RESULT { get; set; }
        public string PBP_RGE_RESULT { get; set; }


        public string PRC_RESULT { get; set; }
        public string PRC_AS_RESULT { get; set; }
        public string PBP_PRC_RESULT_TO { get; set; }

        public B_SV_RECORD_FORM_CHECKLIST B_SV_RECORD_FORM_CHECKLIST { get; set; }

        public List<string> Rmk { get; set; }
        public List<SelectListItem> GetValidation
        {
            get
            {
                return SystemListUtil.GetSystemValueBySystemType(SignboardConstant.SV_Validation, 0);

            }
        }

        public List<SelectListItem> GetMWItemList
        {
            get
            {
                return SystemListUtil.GetSystemValueBySystemType(SignboardConstant.SYSTEM_TYPE_ITEM_NO, 6);
            }

        }
        public string NewMWItemUUID { get; set; }
        public string NewMWItem { get; set; }
        public string NewMWItemDes { get; set; }
        public string NewMWItemOrder { get; set; }

        public string PhotoLibUUID { get; set; }
        public string PhotoLibDes { get; set; }
        public string PhotoLibUrl { get; set; }

        public Dictionary<string, List<B_S_MW_ITEM_CHECKLIST_ITEM>> b_S_MW_ITEM_CHECKLIST_ITEMsList { get; set; }
        public Dictionary<string, List<B_SV_RECORD_ITEM_CHECKLIST_ITEM>> svRecordChecklistItemList { get; set; }
        // public List<List<B_S_MW_ITEM_CHECKLIST_ITEM>> b_S_MW_ITEM_CHECKLIST_ITEMsList { get; set; }

        //public string[] TagertMWItemRemarks { get; set; }
        //public string[] TagertMWItemItemDetails { get; set; }
        //public string[] TagertMWItemUUID { get; set; }
        //public string[] TagertCHECKLISTID { get; set; }
        public Dictionary<string, string>[] TargetMWItem { get; set; }
        public Dictionary<string, string> TargetMWItemResult { get; set; }
        public Dictionary<string, string> TargetMWItemPOResult { get; set; }
        public string TargetMWItemSAResult { get; set; }
        public string TargetMWItemSCCResult { get; set; }

        public string SaveMode { get; set; }

        public string EditMode { get; set; }
        public string VIEW_MODE = SignboardConstant.VIEW_MODE;

        public List<B_SV_SIGNBOARD_RELATION> SignboardRelationList { get; set; }
        public List<string> NewRelatedSignboard { get; set; }


        public B_SV_ADDRESS B_SV_ADDRESS { get; set; }

        public string advisoryLetter { get; set; }
        public List<SelectListItem> advisoryLetterList { get; set; }

        public string letterUuid { get; set; }

        public string currUser { get; set; }
    }


    public class ValidationPrintModel
    {
        // Audit form
        public string FORM_CODE { get; set; }
        public string SUBMISSION_NO { get; set; }

        // SL-SC01_CH_01
        public string PAW_NAME_C { get; set; }
        public string FLOOR { get; set; }
        public string FLAT { get; set; }
        public string BLOCK { get; set; }
        public string BUILDINGNAME { get; set; }
        public string STREET { get; set; }
        public string STREET_NO { get; set; }
        public string DISTRICT { get; set; }
        public string REGION { get; set; }
        //public string SUBMISSION_NO { get; set; }
        public string LOCATION_OF_SIGNBOARD { get; set; }
        public string SIGNBOARD_DESCRIPTION { get; set; }
        public string RECEIVED_DATE_C { get; set; }
        public string SPO_CH_NAME { get; set; }
        public string PBP_NAME_C { get; set; }
        public string PRC_NAME_C { get; set; }
        public string SIGNBOARD_OWNER_NAME_C { get; set; }
        public string NOTIFY_DATE_C { get; set; }

        // SL-SC01_EN_01
        public string NOTIFY_DATE { get; set; }
        public string PAW_NAME_E { get; set; }
        public string SPO_ENG_NAME { get; set; }
        public string RECEIVED_DATE { get; set; }
        public string PBP_NAME_E { get; set; }
        public string PRC_NAME_E { get; set; }
        public string SIGNBOARD_OWNER_NAME_E { get; set; }
        
    }

}