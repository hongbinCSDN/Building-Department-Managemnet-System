using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Admin.Models
{

    public class CRM_DisplayModel : CRM_Model,  IValidatableObject
    {


        public string SystemType { get; set; }
        public string ImportType { get; set; }
        public string File { get; set; }

        public string DisplayType
        {
            get
            {
                string tmp = "";
                if (SystemType != null)
                {
                    switch (SystemType)
                    {   case RegistrationConstant.SYSTEM_TYPE_COMPANY_TYPE:
                            tmp = "Company Type";
                            break;
                        case RegistrationConstant.SYSTEM_TYPE_CONVICTION_SOURCE:
                            tmp = "Conviction Source";
                            break;

                        default:
                            tmp = SystemType;
                            break;
                    }
                }
                return tmp;
            }
            set { SystemType= value; }
        }


        public C_S_SYSTEM_TYPE C_S_SYSTEM_TYPE { get; set; }
        public C_S_SYSTEM_VALUE C_S_SYSTEM_VALUE { get; set; }
        public C_S_AUTHORITY C_S_AUTHORITY { get; set; }
        public C_S_BUILDING_SAFETY_ITEM C_S_BUILDING_SAFETY_ITEM { get; set; }
        public C_S_CATEGORY_CODE C_S_CATEGORY_CODE { get; set; }
        public C_S_HTML_NOTES C_S_HTML_NOTES { get; set; }
        public C_S_ROOM C_S_ROOM { get; set; }
        public C_S_CATEGORY_CODE_DETAIL C_S_CATEGORY_CODE_DETAIL { get; set; }
        public C_S_EXPORT_LETTER C_S_EXPORT_LETTER { get; set; }

        public C_S_EXPORT_LETTER C_S_EXPORT_LETTER_save0 { get; set; }
        public C_S_EXPORT_LETTER C_S_EXPORT_LETTER_save1 { get; set; }
        public C_S_EXPORT_LETTER C_S_EXPORT_LETTER_save2 { get; set; }
        public C_S_EXPORT_LETTER C_S_EXPORT_LETTER_save3 { get; set; }
        public C_ADDRESS C_ADDRESS { get; set; }

        public string BuildingSafetyCode { get; set; }

        public bool Active
        {
            get { return C_S_CATEGORY_CODE != null && "Y".Equals(C_S_CATEGORY_CODE.ACTIVE); }
            set { C_S_CATEGORY_CODE.ACTIVE = value ? "Y" : "N"; }
        }

        public bool IsActive
        {
            get { return C_S_SYSTEM_VALUE != null && "Y".Equals(C_S_SYSTEM_VALUE.IS_ACTIVE); }
            set { C_S_SYSTEM_VALUE.IS_ACTIVE = value ? "Y" : "N"; }
        }

        public IEnumerable<SelectListItem> RegTypeList { set; get; } = Utility.SystemListUtil.RetrieveRegType();

        public List<SelectListItem> getCategoryCode()
        {
            return SystemListUtil.GetCategoryCodeList();
        }

        public List<SelectListItem> getClass()
        {
            return SystemListUtil.Get_C_ClassTypeList();
        }

        public List<SelectListItem> getType()
        {
            return SystemListUtil.GetTypeList();
        }

        public List<SelectListItem> getSyetemValueCode()
        {
            return SystemListUtil.GetSystemValueCodeList();
        }

        public List<SelectListItem> getPanelType()
        {
            return SystemListUtil.GetPanelTypeList();
        }

        public List<SelectListItem> getSystemCategoryCode()
        {
            return SystemListUtil.GetSystemCategoryCodeList();
        }
        //get 4 types
        public List<SelectListItem> getSRegistrationType()
        {
            return SystemListUtil.GetRegistrationTypeList();
        }

        public List<SelectListItem> getCGCSelectionType()
        {
            return SystemListUtil.GetCGCSelectionTypeList();
        }

        public List<SelectListItem> getIPSelectionType()
        {
            return SystemListUtil.GetIPSelectionTypeList();
        }

        public List<SelectListItem> getCMWSelectionType()
        {
            return SystemListUtil.GetCMWSelectionTypeList();
        }

        public List<SelectListItem> getIMWSelectionType()
        {
            return SystemListUtil.GetIMWSelectionTypeList();
        }



        //GetPanelTypeList


        //public string RegType
        //{
        //    get
        //    {
        //        string tmp = "";
        //        if (C_S_SYSTEM_VALUE != null)
        //        {    switch (C_S_SYSTEM_VALUE.REGISTRATION_TYPE)
        //            {
        //                case "CGC":
        //                    tmp = " General Contractor";
        //                    break;
     
        //                case "CMW":
        //                    tmp = " MW Company";
        //                    break;

        //                case "IP":
        //                    tmp = " Professional";
        //                    break;

        //                case "IMW":
        //                    tmp = " MW Individual";
        //                    break;

        //                default:
        //                    tmp = "All";
        //                    break;
        //            }
        //        }
        //        return tmp;     
        //    }
        //    set { C_S_SYSTEM_VALUE.REGISTRATION_TYPE = value; }
           
        //}
        public string RegType { get; set; }




        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {   // GlobalConfig.SYSTEM_COLUMN_META_LIST

            yield return ValidationUtil.Validate_Length(this, "C_S_SYSTEM_VALUE.CODE");
            yield return ValidationUtil.Validate_Length(this, "C_S_SYSTEM_VALUE.ORDERING");
            yield return ValidationUtil.Validate_Length(this, "C_S_SYSTEM_VALUE.ENGLISH_DESCRIPTION");
            yield return ValidationUtil.Validate_Length(this, "C_S_SYSTEM_VALUE.CHINESE_DESCRIPTION");

            yield return ValidationUtil.Validate_Mandatory(this, "C_S_SYSTEM_VALUE.CODE");
            yield return ValidationUtil.Validate_Mandatory(this, "C_S_SYSTEM_VALUE.ENGLISH_DESCRIPTION");



        }
    }
    
}