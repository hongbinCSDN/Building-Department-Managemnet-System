using MWMS2.App_Start;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Models
{

    public class CompDisplayModel : EditFormModel, IValidatableObject
    {

        public string RegType { get; set; }
        public string RegPrefix { get; set; }
        public bool PoolingSelected { get; set; }
        public List<string> PoolingRefNo { get; set; }
        public List<C_POOLING> ListPoolingRefNo { get; set; }
        //public C_COMP_APPLICANT_INFO_MASTER C_COMP_APPLICANT_INFO_MASTER { get; set; }
        //public C_COMP_APPLICANT_INFO C_COMP_APPLICANT_INFO { get; set; }
        public C_COMP_APPLICATION C_COMP_APPLICATION { get; set; }
        public C_S_SEARCH_LEVEL C_S_SEARCH_LEVEL { get; set; }
        public bool EditMode
        {
            get
            {
                return C_COMP_APPLICATION != null && C_COMP_APPLICATION.UUID != null && C_COMP_APPLICATION.UUID.Length > 10;
            }
        }
        [Display(Name = "Interest of Building Safety Services")]
        public List<C_S_SYSTEM_VALUE> C_S_SYSTEM_VALUEs_BS { get; set; } = new List<C_S_SYSTEM_VALUE>();

        [Display(Name = "Key Personnel")]
        public List<ApplicantDisplayListModel> C_APPLICANTs { get; set; } = new List<ApplicantDisplayListModel>();

        [Display(Name = "(English)")]
        public C_ADDRESS C_ADDRESS_English { get; set; } = new C_ADDRESS();
        [Display(Name = "(Chinese)")]
        public C_ADDRESS C_ADDRESS_Chinese { get; set; } = new C_ADDRESS();

        [Display(Name = "(English)")]
        public C_ADDRESS C_ADDRESS_BS_English { get; set; } = new C_ADDRESS();
        [Display(Name = "(Chinese)")]
        public C_ADDRESS C_ADDRESS_BS_Chinese { get; set; } = new C_ADDRESS();
        public BuildingSafetyCheckListModel BsModel { get; set; }
        public List<BuildingSafetyItem> BsItems { get; set; }

        public string Class1 { get; set; }
        //public string Class1String { get { return (Class1 == null || Class1.Count == 0) ? "" : string.Join(", ", Class1); } }
        public string Class2 { get; set; }
        //public string Class2String { get { return (Class2 == null || Class2.Count == 0) ? "" : string.Join(", ", Class2); } }
        public string Class3 { get; set; }
        //public string Class3String { get { return (Class3 == null || Class3.Count == 0) ? "" : string.Join(", ", Class3); } }
        //new add
        public string ServiceInMWIS { get; set; }
        public List<SelectListItem> GetMWISOption()
        {
            return SystemListUtil.RetrieveMWISOption();
        }
        public string CATEGORY_CODE
        {
            get; set;
        }
        public IEnumerable<SelectListItem> CATEGORY_CODE_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(
                        SystemListUtil.GetCatCodeByRegType(RegType)
                         .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID }));
            }
        }
        public IEnumerable<SelectListItem> APPLICATION_STATUS_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(
                             RegistrationConstant.SYSTEM_TYPE_APPLICANT_STATUS)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }));
            }
        }
        public IEnumerable<SelectListItem> COMPANY_TYPE_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select Company Type -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(
                            RegistrationConstant.SYSTEM_TYPE_COMPANY_TYPE)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }));
            }
        }

        public IEnumerable<SelectListItem> PERIOD_OF_VALIDITY_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(
                            RegistrationConstant.SYSTEM_TYPE_PERIOD_OF_VALIDITY)
                            .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID }));
            }
        }

        public IEnumerable<SelectListItem> APPLICATION_FORM_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByRegTypeNType(
                            RegType
                            , RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM)
                            .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID }));
            }
        }
        public IEnumerable<SelectListItem> PRACTICE_NOTES_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(
                            RegistrationConstant.SYSTEM_TYPE_PRACTICE_NOTE)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }));
            }
        }



        public IEnumerable<SelectListItem> WILLINGNESS_QP_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(
                            RegistrationConstant.SYSTEM_TYPE_QPSERVICES)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.CODE }));
            }
        }

        public IEnumerable<SelectListItem> INTERESTED_FSS_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(
                            RegistrationConstant.SYSTEM_TYPE_FSS_DROPDOWN)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.CODE }));
            }
        }



        public IEnumerable<SelectListItem> BS_REGION_CODE_ID_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(
                            RegistrationConstant.SYSTEM_TYPE_REGION_CODE)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }));
            }
        }

        //public string getValue { get { return  C_S_SEARCH_LEVEL.C_S_SYSTEM_VALUE.UUID; } }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.UUID                        ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.FILE_REFERENCE_NO           ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.BR_NO                       ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.BRANCH_NO                   ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.ENGLISH_COMPANY_NAME        ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.CHINESE_COMPANY_NAME        ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.APPLICATION_STATUS_ID       ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.COMPANY_TYPE_ID             ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.APPLICANT_NAME              ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.ENGLISH_CARE_OF             ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.CHINESE_CARE_OF             ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.C_ADDRESS.ADDRESS_LINE1     ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.C_ADDRESS.ADDRESS_LINE2     ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.C_ADDRESS.ADDRESS_LINE3     ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.C_ADDRESS.ADDRESS_LINE4     ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.C_ADDRESS.ADDRESS_LINE5     ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.C_ADDRESS2.ADDRESS_LINE1    ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.C_ADDRESS2.ADDRESS_LINE2    ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.C_ADDRESS2.ADDRESS_LINE3    ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.C_ADDRESS2.ADDRESS_LINE4    ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.C_ADDRESS2.ADDRESS_LINE5    ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.TELEPHONE_NO1               ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.TELEPHONE_NO2               ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.TELEPHONE_NO3               ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.FAX_NO1                     ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.FAX_NO2                     ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.EMAIL_ADDRESS               ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.OLD_FILE_REFERENCE          ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.PERIOD_OF_VALIDITY_ID       ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.APPLICATION_DATE            ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.APPLICATION_FORM_ID         ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.DUE_DATE                    ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.PRACTICE_NOTES_ID           ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.PROSECUTION_DISCIPLINARY_REC");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.REMARKS                     ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.WILLINGNESS_QP              ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.INTERESTED_FSS              ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.ENGLISH_BS_CARE_OF          ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.CHINESE_BS_CARE_OF          ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.C_ADDRESS1.ADDRESS_LINE1    ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.C_ADDRESS1.ADDRESS_LINE2    ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.C_ADDRESS1.ADDRESS_LINE3    ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.C_ADDRESS1.ADDRESS_LINE4    ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.C_ADDRESS1.ADDRESS_LINE5    ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.C_ADDRESS3.ADDRESS_LINE1    ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.C_ADDRESS3.ADDRESS_LINE2    ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.C_ADDRESS3.ADDRESS_LINE3    ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.C_ADDRESS3.ADDRESS_LINE4    ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.C_ADDRESS3.ADDRESS_LINE5    ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.BS_TELEPHONE_NO1            ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICATION.BS_FAX_NO1                  ");

            yield return ValidationUtil.Validate_Mandatory(this, "C_COMP_APPLICATION.FILE_REFERENCE_NO");
            yield return ValidationUtil.Validate_Mandatory(this, "C_COMP_APPLICATION.BR_NO");
            yield return ValidationUtil.Validate_Mandatory(this, "C_COMP_APPLICATION.ENGLISH_COMPANY_NAME");
            yield return ValidationUtil.Validate_Mandatory(this, "C_COMP_APPLICATION.APPLICATION_STATUS_ID");
            yield return ValidationUtil.Validate_Mandatory(this, "C_COMP_APPLICATION.COMPANY_TYPE_ID");
            yield return ValidationUtil.Validate_Mandatory(this, "C_COMP_APPLICATION.TELEPHONE_NO1");
            yield return ValidationUtil.Validate_Mandatory(this, "C_COMP_APPLICATION.PERIOD_OF_VALIDITY_ID");
            yield return ValidationUtil.Validate_Mandatory(this, "C_COMP_APPLICATION.APPLICATION_FORM_ID");
        }

       

    }
}