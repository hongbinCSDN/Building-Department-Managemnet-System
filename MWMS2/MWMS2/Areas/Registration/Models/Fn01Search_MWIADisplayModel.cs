using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Constant;
namespace MWMS2.Areas.Registration.Models
{

    public class Fn01Search_MWIADisplayModel : EditFormModel, IValidatableObject
    {
        public bool result { get; set; }
        public string ErrMsg { get; set; }
        public C_IND_APPLICATION C_IND_APPLICATION { get; set; } = new C_IND_APPLICATION();
        public C_IND_CERTIFICATE C_IND_CERTIFICATE { get; set; } = new C_IND_CERTIFICATE();
        public C_APPLICANT C_APPLICANT { get; set; } = new C_APPLICANT();
        public C_ADDRESS HOME_ADDRESS_ENG { get; set; } = new C_ADDRESS();
        public C_ADDRESS HOME_ADDRESS_CHI { get; set; } = new C_ADDRESS();
        public C_ADDRESS OFFICE_ADDRESS_ENG { get; set; } = new C_ADDRESS();
        public C_ADDRESS OFFICE_ADDRESS_CHI { get; set; } = new C_ADDRESS();
        public C_ADDRESS BS_ADDRESS_ENG { get; set; } = new C_ADDRESS();
        public C_ADDRESS BS_ADDRESS_CHI { get; set; } = new C_ADDRESS();
        public C_S_SYSTEM_VALUE SV_CATEGORY_C_S_SYSTEM_VALUE { get; set; } = new C_S_SYSTEM_VALUE();
        public C_S_SYSTEM_VALUE APP_STATUS_C_S_SYSTEM_VALUE { get; set; } = new C_S_SYSTEM_VALUE();
        public List<C_S_SYSTEM_VALUE> MWITEMLIST { get; set; } = new List<C_S_SYSTEM_VALUE>();

        public List<C_BUILDING_SAFETY_INFO> C_BUILDING_SAFETY_INFOs { get; set; } = new List<C_BUILDING_SAFETY_INFO>();


        public BuildingSafetyCheckListModel BsModel { get; set; } = new BuildingSafetyCheckListModel();
        public List<BuildingSafetyItem> BsItems { get; set; } = new List<BuildingSafetyItem>();

         
        public string getDecryptHKID()
        {
            if (C_APPLICANT != null)
                return EncryptDecryptUtil.getDecryptHKID(C_APPLICANT.HKID);
            else
                return null;
        }
        public string getDecryptPassportNo()
        {
            if (C_APPLICANT != null)
                return EncryptDecryptUtil.getDecryptHKID(C_APPLICANT.PASSPORT_NO);
            else
                return null;
          
        }
        public List<SelectListItem> getTitle()
        {
            return SystemListUtil.RetrieveTitle();
        }
        public List<SelectListItem> getGender()
        {
            return SystemListUtil.RetrieveGender();
        }
        public List<SelectListItem> getCommunicationLanguage()
        {
            return SystemListUtil.RetrieveCommunicationLanguage();
        }
        public List<SelectListItem> getPNAPByType()
        {
            return SystemListUtil.RetrievePNAPByType();
        }
        public IEnumerable<SelectListItem> getCategory
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(
                        SystemListUtil.GetCatCodeByRegType(RegistrationConstant.REGISTRATION_TYPE_MWIA)
                         .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID }));
            }
        }
        public IEnumerable<SelectListItem> getApplicationStatus
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(
                        SystemListUtil.GetSVListByType(RegistrationConstant.SYSTEM_TYPE_APPLICANT_STATUS)
                         .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }));
            }
        }
        public IEnumerable<SelectListItem> getApplicationForm
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(
                        SystemListUtil.GetSVListByRegTypeNType(RegistrationConstant.REGISTRATION_TYPE_MWIA,RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM)
                         .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID }));
            }
        }
        public IEnumerable<SelectListItem> getPeriodOfValidity
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(
                        SystemListUtil.GetSVListByType( RegistrationConstant.SYSTEM_TYPE_PERIOD_OF_VALIDITY)
                         .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID }));
            }
        }
        public List<SelectListItem> GetYNIOption()
        {
            return SystemListUtil.RetrieveYNIOption();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //yield return ValidationUtil.Validate_Length(this, "C_S_SYSTEM_VALUE.CODE");
            //yield return ValidationUtil.Validate_Length(this, "C_S_SYSTEM_VALUE.ORDERING");
            //yield return ValidationUtil.Validate_Length(this, "C_S_SYSTEM_VALUE.ENGLISH_DESCRIPTION");
            //yield return ValidationUtil.Validate_Length(this, "C_S_SYSTEM_VALUE.CHINESE_DESCRIPTION");
            //yield return ValidationUtil.Validate_Mandatory(this, "C_S_SYSTEM_VALUE.CODE");
            //yield return ValidationUtil.Validate_Mandatory(this, "C_S_SYSTEM_VALUE.ENGLISH_DESCRIPTION");


            yield return ValidationUtil.Validate_Length(this, "C_APPLICANT.CHINESE_NAME");

            yield return ValidationUtil.Validate_Mandatory(this, "C_IND_APPLICATION.FILE_REFERENCE_NO");
            yield return ValidationUtil.Validate_Mandatory(this, "C_APPLICANT.HKID", "C_APPLICANT.PASSPORT_NO");
            //yield return ValidationUtil.Validate_Mandatory(this, "C_APPLICANT.HKID");
            yield return ValidationUtil.Validate_Mandatory(this, "C_APPLICANT.SURNAME");
            yield return ValidationUtil.Validate_Mandatory(this, "C_APPLICANT.GIVEN_NAME_ON_ID");
            yield return ValidationUtil.Validate_Mandatory(this, "C_APPLICANT.CHINESE_NAME");
            yield return ValidationUtil.Validate_Mandatory(this, "C_IND_CERTIFICATE.CATEGORY_ID");
            yield return ValidationUtil.Validate_Mandatory(this, "C_IND_CERTIFICATE.CERTIFICATION_NO");
            yield return ValidationUtil.Validate_Mandatory(this, "C_IND_CERTIFICATE.APPLICATION_FORM_ID");
            yield return ValidationUtil.Validate_Mandatory(this, "C_IND_CERTIFICATE.PERIOD_OF_VALIDITY_ID");
            //yield return ValidationUtil.Validate_Mandatory(this, "C_IND_APPLICATION.DATE_OF_DISPOSAL");
            yield return ValidationUtil.Validate_Mandatory(this, "C_IND_APPLICATION.TELEPHONE_NO1");




        }

        public IEnumerable<SelectListItem> getRegion
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select Region Code -", Value = "" } })
                        .Concat(
                        SystemListUtil.GetSVListByType(RegistrationConstant.SYSTEM_TYPE_REGION_CODE)
                         .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }));
            }
        }



        public int MWItemCount
        {
            get {
                return SystemListUtil.GetMWItemFullListByClass(RegistrationConstant.MW_CLASS_III).Count();
            }
        }
        public string ServiceInMWIS { get; set; }
        public List<SelectListItem> GetMWISOption()
        {
            return SystemListUtil.RetrieveMWISOption();
        }
    }
}