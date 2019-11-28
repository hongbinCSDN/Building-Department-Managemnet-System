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

namespace MWMS2.Areas.Registration.Models
{

    public class Fn01Search_PADisplayModel : EditFormModel, IValidatableObject
    {
        public bool result { get; set; }
        public string ErrMsg { get; set; }
        public bool TempSave { get; set; }

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

        public List<C_BUILDING_SAFETY_INFO> C_BUILDING_SAFETY_INFOs { get; set; } = new List<C_BUILDING_SAFETY_INFO>();

        public C_IND_QUALIFICATION C_IND_QUALIFICATION { get; set; } = new C_IND_QUALIFICATION();
        public List<PRBLIST> PRBLISTs { get; set; } = new List<PRBLIST>();
        public class PRBLIST
        {
          public C_IND_QUALIFICATION PRB_c_IND_QUALIFICATION { get; set; }
          public C_S_SYSTEM_VALUE  PRB_c_S_SYSTEM_VALUE { get; set; }
          public C_S_CATEGORY_CODE  PRB_c_S_CATEGORY_CODE { get; set; }
          public C_S_SYSTEM_VALUE PRB_Des_c_S_SYSTEM_VALUE { get; set; }
        }
      
        public List<CERTLIST> CERTLISTs { get; set; }
        public class CERTLIST
        {
            public C_IND_CERTIFICATE CERT_c_IND_CERTIFICATE { get; set; }
            public C_S_CATEGORY_CODE CERT_c_S_CATEGORY_CODE { get; set; }
            public C_S_SYSTEM_VALUE CERT_APPFORM_c_S_SYSTEM_VALUE { get; set; }
            public C_S_SYSTEM_VALUE CERT_PERIODVAD_c_S_SYSTEM_VALUE { get; set; }
        }

    

        public string getDecryptHKID()
        {
            if (C_APPLICANT.HKID != null)
                return EncryptDecryptUtil.getDecryptHKID(C_APPLICANT.HKID);
            else
                return null;
        }





        public string getDecryptPassportNo()
        {
            if (C_APPLICANT.PASSPORT_NO != null)
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
        public List<SelectListItem> GetYNIOption()
        {
            return SystemListUtil.RetrieveYNIOption();
        }
        public List<SelectListItem> GetMWISOption()
        {
            return SystemListUtil.RetrieveMWISOption();
        }
        public List<SelectListItem> getYNOption()
        {
            var a = SystemListUtil.RetrieveYNOption();
            a.Reverse();
            a.Last().Selected = true;
            return a;
        }
        public List<SelectListItem> getPRPByType()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "- Select PRB -",
                Value = "",

            });
            selectListItems.AddRange(from sv in SystemListUtil.GetSVListByType(RegistrationConstant.SYSTEM_TYPE_PROF_REGISTRATION_BOARD)
                                     select new SelectListItem()
                                     {
                                         Text = sv.CODE,
                                         Value = sv.UUID
                                     }
                                     );

            return selectListItems;
        }

        public string SelectedCertiUUID { get; set; }
        public C_IND_CERTIFICATE SelectedCertificate { get ; set; }
        public List<SelectListItem> getCertiList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "-",
                Value = "",

            });
            if(CERTLISTs!=null)
                foreach (var item in CERTLISTs)
                {
                    selectListItems.Add(new SelectListItem
                    {
                        Text = item.CERT_c_IND_CERTIFICATE.CERTIFICATION_NO,
                        Value = item.CERT_c_IND_CERTIFICATE.UUID,

                    });
                }
            return selectListItems;

        }
      
        public QualifcationDisplayListModel QualiDisList { get; set; }
     
        public IEnumerable<SelectListItem> RetrieveQCType()
        {
            return (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                         .Concat(SystemListUtil.GetSVListByType(
                             RegistrationConstant.SYSTEM_TYPE_QUALIFICATION_TYPE)
                             .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID }));
        }
        public IEnumerable<SelectListItem> RetrieveQCByType()
        {
            return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.RetrieveCatCodeByType(
                            RegistrationConstant.REGISTRATION_TYPE_IP)
                            .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID }));

      
        }

        public BuildingSafetyCheckListModel BsModel { get; set; } = new BuildingSafetyCheckListModel();
        public List<BuildingSafetyItem> BsItems { get; set; }
        public string ServiceInMWIS { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            
            yield return ValidationUtil.Validate_Length(this, "C_APPLICANT.CHINESE_NAME");
 
            yield return ValidationUtil.Validate_Mandatory(this, "C_IND_APPLICATION.FILE_REFERENCE_NO");
            yield return ValidationUtil.Validate_Mandatory(this, "C_APPLICANT.HKID", "C_APPLICANT.PASSPORT_NO");
            yield return ValidationUtil.Validate_Mandatory(this, "C_APPLICANT.SURNAME");
            yield return ValidationUtil.Validate_Mandatory(this, "C_APPLICANT.GIVEN_NAME_ON_ID");
            yield return ValidationUtil.Validate_Mandatory(this, "C_APPLICANT.CHINESE_NAME");

          
            yield return ValidationUtil.Validate_Mandatory(this, "C_IND_APPLICATION.TELEPHONE_NO1");




        }
    }
}