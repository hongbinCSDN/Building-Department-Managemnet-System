using MWMS2.App_Start;
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

    public class CompApplicantEditModel : EditFormModel, IValidatableObject
    {
        public List<HttpPostedFileBase> APPLICANT_FILE { get; set; }
        public C_COMP_APPLICANT_INFO C_COMP_APPLICANT_INFO { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICANT_INFO.C_APPLICANT.HKID              ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICANT_INFO.C_APPLICANT.PASSPORT_NO       ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICANT_INFO.C_APPLICANT.SURNAME           ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICANT_INFO.C_APPLICANT.GIVEN_NAME_ON_ID  ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICANT_INFO.C_APPLICANT.CHINESE_NAME      ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICANT_INFO.RES_ADDRESS_E1                ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICANT_INFO.RES_ADDRESS_E2                ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICANT_INFO.RES_ADDRESS_E3                ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICANT_INFO.RES_ADDRESS_E4                ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICANT_INFO.RES_ADDRESS_E5                ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICANT_INFO.RES_ADDRESS_C1                ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICANT_INFO.RES_ADDRESS_C2                ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICANT_INFO.RES_ADDRESS_C3                ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICANT_INFO.RES_ADDRESS_C4                ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICANT_INFO.RES_ADDRESS_C5                ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICANT_INFO.OFFICE_TEL                    ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICANT_INFO.MOBILE_TEL                    ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICANT_INFO.RES_TEL                       ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICANT_INFO.EMAIL1                        ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICANT_INFO.EMAIL2                        ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICANT_INFO.REMARK                        ");
            yield return ValidationUtil.Validate_Length(this, "C_COMP_APPLICANT_INFO.C_APPLICANT.GENDER            ");


            yield return ValidationUtil.Validate_Mandatory(this, "C_COMP_APPLICANT_INFO.CORRESPONDENCE_LANG_ID             ");
            yield return ValidationUtil.Validate_Mandatory(this, "C_COMP_APPLICANT_INFO.APPLICANT_ROLE_ID                  ");
            yield return ValidationUtil.Validate_Mandatory(this, "C_COMP_APPLICANT_INFO.C_APPLICANT.HKID                   ", "C_COMP_APPLICANT_INFO.C_APPLICANT.PASSPORT_NO       ");
            yield return ValidationUtil.Validate_Mandatory(this, "C_COMP_APPLICANT_INFO.C_APPLICANT.SURNAME                ");
            yield return ValidationUtil.Validate_Mandatory(this, "C_COMP_APPLICANT_INFO.C_APPLICANT.GIVEN_NAME_ON_ID       ");
            yield return ValidationUtil.Validate_Mandatory(this, "C_COMP_APPLICANT_INFO.APPLICANT_STATUS_ID                ");







        }
    }
}