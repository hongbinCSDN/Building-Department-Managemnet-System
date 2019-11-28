using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services.Signborad.SignboardDAO;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Models
{
    public class DataEntryDisplayModel : SignboardRecordCommonDisplayModel
    {
        // public SignboardRecordCommonDisplayModel CommonModel { get; set; }
        public List<B_SV_RECORD_VALIDATION_ITEM> SvRecordValidationItems { get; set; }
        public string FormCode { get; set; }
        public B_SV_RECORD SvRecord { get; set; }
        public B_SV_SUBMISSION SvSubmission { get; set; }
        public B_SV_SIGNBOARD SvSignboard { get; set; }
        public B_SV_PERSON_CONTACT Owner { get; set; }
        public B_SV_PERSON_CONTACT Paw { get; set; }
        public B_SV_PERSON_CONTACT Oi { get; set; }
        public List<B_SV_RECORD_ITEM> ItemList { get; set; }
        public B_SV_APPOINTED_PROFESSIONAL Ap { get; set; } = new B_SV_APPOINTED_PROFESSIONAL();
        public B_SV_APPOINTED_PROFESSIONAL Rse { get; set; } = new B_SV_APPOINTED_PROFESSIONAL();
        public B_SV_APPOINTED_PROFESSIONAL Rge { get; set; } = new B_SV_APPOINTED_PROFESSIONAL();
        public B_SV_APPOINTED_PROFESSIONAL Prc { get; set; } = new B_SV_APPOINTED_PROFESSIONAL();
        public B_SV_ADDRESS OwnerAddress { get; set; }
        public string Rfid { get; set; }
        public DateTime? RfidTime { get; set; }
        public string LanguageCode { get; set; }
        public bool Saved { get; set; }
        public string ErrMsg { get; set; }
        public string FormMode { get; set; }
        public string EditMode { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public List<string> ToList { get; set; }
        public List<string> LetterTemplateList { get; set; }
        public string ExportLetterFlag { get; set; }
        public List<string> LetterTypeList { get; set; }
        public DateTime? EformUpdatedDate { get; set; }

        public List<B_SV_RECORD_ITEM> B_SV_RECORD_ITEMs {
            get {
                List<B_SV_RECORD_ITEM> r = new List<B_SV_RECORD_ITEM>();
                if(B_SV_RECORD_ITEM_MW_ITEM_CODE != null) for(int i = 0; i < B_SV_RECORD_ITEM_MW_ITEM_CODE.Length; i++)
                {
                    if (!string.IsNullOrEmpty(B_SV_RECORD_ITEM_MW_ITEM_CODE[i]))
                        r.Add(new B_SV_RECORD_ITEM()
                    {
                        UUID = "",//B_SV_RECORD_ITEM_UUID[i],
                        MW_ITEM_CODE = B_SV_RECORD_ITEM_MW_ITEM_CODE[i],
                        LOCATION_DESCRIPTION = B_SV_RECORD_ITEM_LOCATION_DESCRIPTION[i],
                        REVISION = B_SV_RECORD_ITEM_REVISION[i],
                        B_SV_RECORD= SvRecord
                    });
                }
                return r;
            }
        }
        public string[] B_SV_RECORD_ITEM_MW_ITEM_CODE { get; set; }
        //public string[] B_SV_RECORD_ITEM_UUID { get; set; }
        public string[] B_SV_RECORD_ITEM_LOCATION_DESCRIPTION { get; set; }
        public string[] B_SV_RECORD_ITEM_REVISION { get; set; }
        //To Dorpdown List
        public string TO_Handling_Officer { get; set; }
        public IEnumerable<SelectListItem> TO_Handling_Officer_List
        {
            get { return SignboardJobAssignmentDAOService.GetScuTeamByPosition(SignboardConstant.S_USER_ACCOUNT_RANK_TO); }
        }
        public List<B_SV_RECORD_VALIDATION_ITEM> B_SV_RECORD_VALIDATION_ITEMs
        {
            get
            {
                List<B_SV_RECORD_VALIDATION_ITEM> r = new List<B_SV_RECORD_VALIDATION_ITEM>();
                if (B_SV_RECORD_VALIDATION_ITEM_DESCRIPTION != null) for (int i = 0; i < B_SV_RECORD_VALIDATION_ITEM_DESCRIPTION.Length; i++)
                {
                    if(!string.IsNullOrEmpty(B_SV_RECORD_VALIDATION_ITEM_CORRESPONDING_ITEM[i]))
                    r.Add(new B_SV_RECORD_VALIDATION_ITEM()
                    {
                        UUID = "",//B_SV_RECORD_ITEM_UUID[i],
                         DESCRIPTION = B_SV_RECORD_VALIDATION_ITEM_DESCRIPTION[i],
                        VALIDATION_ITEM = B_SV_RECORD_VALIDATION_ITEM_VALIDATION_ITEM[i],
                        CORRESPONDING_ITEM = B_SV_RECORD_VALIDATION_ITEM_CORRESPONDING_ITEM[i],
                        B_SV_RECORD = SvRecord
                    });
                }
                return r;
            }
        }
        public string[] B_SV_RECORD_VALIDATION_ITEM_DESCRIPTION { get; set; }
        public string[] B_SV_RECORD_VALIDATION_ITEM_VALIDATION_ITEM { get; set; }
        public string[] B_SV_RECORD_VALIDATION_ITEM_CORRESPONDING_ITEM { get; set; }
        







        // public IEnumerable<SelectListItem> MWITEMs { get; set; }
        // public IEnumerable<SelectListItem> VDITEMs { get; set; }



        public IEnumerable<SelectListItem> TYPEList
        {
            get { return SystemListUtil.GetTYPEList(); }
        }
        public IEnumerable<SelectListItem> LEDList
        {
            get { return SystemListUtil.GetLEDList(); }
        }
        public bool WithAlteration
        {
            get { return SvRecord != null && "Y".Equals(SvRecord.WITH_ALTERATION); }
            set { SvRecord.WITH_ALTERATION = value ? "Y" : "N"; }
        }
        public IEnumerable<SelectListItem> PawSameAsList
        {
            get { return SystemListUtil.GetPawSameAsList(); }
        }
        public DateTime? ValidationExpiryDate { get; set; }
        public bool SignboardRemoval { get; set; }
        public string SaveMode { get; set; }
        public string SelectedLetterType { get; set; }
        public IEnumerable<SelectListItem> SelectedLetterTypeList
        {
            get { return SystemListUtil.GetSystemValueBySystemType(SignboardConstant.SYSTEM_TYPE_LETTER_TYPE, 0); }
            set { }
        }
        public string SelectedLetter { get; set; }
        public List<SelectListItem> LetterList
        {
            get { return SystemListUtil.GetLetter(); }
            set { }
        }
        public List<B_S_LETTER_TEMPLATE> B_S_LETTER_TEMPLATE_List { get; set; }
        //ValidationItemList
        public List<string> jsonSvRecordValidationItems { get; set; }
        public List<string> jsonValidationItemlookUpList { get; set; }
        public List<string> itemlookUpList { get; set; }
        public List<string> jsonSvRecordItems { get; set; }
        public List<string> jsonItemlookUpList { get; set; }
        public List<string> selectedVaCorrItemsR { get; set; }
        public List<string> selectedVaItemsR { get; set; }
        public List<string> selectedVaDescription { get; set; }
        public List<string> svItemResult { get; set; }
        public List<string> selectedVaCorrItemsR2 { get; set; }
        public string RegType { get; set; }
        public IEnumerable<SelectListItem> BcisDistrictList
        {
            get { return SystemListUtil.GetBcisDistrictList(); }
        }
        public DateTime? ReceivedDate { get; set; }
        public IEnumerable<SelectListItem> RecList
        {
            get { return SystemListUtil.GetRecList(); }
        }
        public string validated { get; set; }
        public string involved { get; set; }
    }
    public class DataEntryPrintModel
    {
        //PersonContact Address
        public string FLOOR { get; set; }
        public string FLAT { get; set; }
        public string STREET { get; set; }
        public string BLOCK { get; set; }
        public string STREET_NO { get; set; }
        public string BUILDINGNAME { get; set; }
        public string DISTRICT { get; set; }
        public string REGION { get; set; }
        public string FULL_ADDRESS { get; set; }

        //SUBMISSION
        public string SUBMISSION_NO { get; set; }

        //PAW
        public string PAW_NAME_C { get; set; }
        public string PAW_NAME_E { get; set; }
        public string PAW_CONTACT { get; set; }
        public string PAW_FAX_OR_ADDR { get; set; }

        //PBP
        public string PBP_NAME_C { get; set; }
        public string PBP_NAME_E { get; set; }
        public string PBP_NAME { get; set; }


        //PRC
        public string PRC_NAME_C { get; set; }
        public string PRC_NAME_E { get; set; }
        public string PRC_NAME { get; set; }
        public string PRC_CONTACT { get; set; }
        public string PRC_FAX_OR_ADDR_C { get; set; }
        public string PRC_FAX_OR_ADDR_E { get; set; }

        //AP
        public string AP_NAME_C { get; set; }
        public string AP_NAME_E { get; set; }
        public string AP_CONTACT { get; set; }
        public string AP_FAX_OR_ADDR_C { get; set; }
        public string AP_FAX_OR_ADDR_E { get; set; }

        //RSE
        public string RSE_NAME_E { get; set; }
        public string RSE_NAME_C { get; set; }
        public string RSE_CONTACT { get; set; }
        public string RSE_FAX_OR_ADDR_C { get; set; }
        public string RSE_FAX_OR_ADDR_E { get; set; }

        //RI
        public string RI_NAME_E { get; set; }
        public string RI_NAME_C { get; set; }
        //RGE
        public string RGE_NAME_E { get; set; }
        public string RGE_NAME_C { get; set; }
        public string RGE_CONTACT { get; set; }
        public string RGE_FAX_OR_ADDR_C { get; set; }
        public string RGE_FAX_OR_ADDR_E { get; set; }

        //common
        public string RD_NAME_E { get; set; }
        public string RD_NAME_C { get; set; }
        public string RD_CONTACT { get; set; }
        public string RD_FAX_OR_ADDR_C { get; set; }
        public string RD_FAX_OR_ADDR_E { get; set; }
        //crmPbpPrc
        public string APPOINTED_PERSON_NAME_E { get; set; }
        public string APPOINTED_PERSON_NAME_C { get; set; }
        public string APPOINTED_PERSON_COMPANY_NAME_E { get; set; }
        public string APPOINTED_PERSON_COMPANY_NAME_C { get; set; }
        public string APPOINTED_PERSON_ADDRESS_ROOM_FLAT_BLOCK_E { get; set; }
        public string APPOINTED_PERSON_ADDRESS_C { get; set; }
        public string APPOINTED_PERSON_ADDRESS_ROOM_FLAT_BLOCK_C { get; set; }
        public string APPOINTED_PERSON_BUILDINGNAME_E{ get; set; }
        public string APPOINTED_PERSON_BUILDINGNAME_C{ get; set; }
        public string APPOINTED_PERSON_STREET_E{ get; set; }
        public string APPOINTED_PERSON_STREET_C{ get; set; }
        public string APPOINTED_PERSON_DISTRICT_E{ get; set; }
        public string APPOINTED_PERSON_DISTRICT_C{ get; set; }
        public string APPOINTED_PERSON_REGION_E{ get; set; }
        public string APPOINTED_PERSON_REGION_C { get; set; }

        //SVSIGNBOARD
        public string SIGNBOARD_OWNER_NAME_E { get; set; }
        public string SIGNBOARD_OWNER_NAME_C { get; set; }
        public string LOCATION_OF_SIGNBOARD { get; set; }
        public string SIGNBOARD_DESCRIPTION { get; set; }
        //
        public string NOTIFY_DATE { get; set; }
        public string NOTIFY_DATE_C { get; set; }
        public string RECEIVED_DATE { get; set; }
        public string RECEIVED_DATE_C { get; set; }

        //RDATE
        public string RDATE_Y { get; set; }
        public string RDATE_M { get; set; }
        public string RDATE_M_STR { get; set; }
        public string RDATE_D { get; set; }
        public string enqno { get; set; }

        //DATE
        public string TODAY_Y { get; set; }
        public string TODAY_M { get; set; }
        public string TODAY_M_STR { get; set; }
        public string TODAY_D { get; set; }
        //USER
        public string SPO_CH_NAME { get; set; }
        public string SPO_ENG_NAME { get; set; }

        public string FORM_CODE { get; set; }


        public string LANGCODE { get; set; }
    }
}