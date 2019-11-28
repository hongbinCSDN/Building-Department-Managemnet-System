using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;

namespace MWMS2.Areas.Registration.Models
{
    public class Fn06CMM_MPDisplayModel: EditFormModel, IValidatableObject
    {
        public bool EditMode
        {
            get
            {
                int dummy;
                return C_COMMITTEE_MEMBER != null
                    && !string.IsNullOrWhiteSpace(C_COMMITTEE_MEMBER.UUID)
                    && !int.TryParse(C_COMMITTEE_MEMBER.UUID, out dummy);
            }
        }
        public C_COMMITTEE_MEMBER C_COMMITTEE_MEMBER { get; set; }
        public Dictionary<string, string> PanelStatus { get; set; }
        public Dictionary<string, DateTime?> PanelEndDate { get; set; }
        public List<string> Institutes { get; set; }
        //public List<C_COMMITTEE_GROUP_MEMBER> C_COMMITTEE_GROUP_MEMBERs = new List<C_COMMITTEE_GROUP_MEMBER>();
        //public List<C_COMMITTEE_PANEL_MEMBER> C_COMMITTEE_PANEL_MEMBERs = new List<C_COMMITTEE_PANEL_MEMBER>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.C_APPLICANT.HKID              ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.C_APPLICANT.PASSPORT_NO       ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.C_APPLICANT.GENDER            ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.C_APPLICANT.SURNAME           ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.C_APPLICANT.GIVEN_NAME_ON_ID  ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.C_APPLICANT.CHINESE_NAME      ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.ENGLISH_CARE_OF               ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.CHINESE_CARE_OF               ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.C_ADDRESS.ADDRESS_LINE1       ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.C_ADDRESS.ADDRESS_LINE2       ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.C_ADDRESS.ADDRESS_LINE3       ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.C_ADDRESS.ADDRESS_LINE4       ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.C_ADDRESS.ADDRESS_LINE5       ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.C_ADDRESS1.ADDRESS_LINE1      ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.C_ADDRESS1.ADDRESS_LINE2      ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.C_ADDRESS1.ADDRESS_LINE3      ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.C_ADDRESS1.ADDRESS_LINE4      ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.C_ADDRESS1.ADDRESS_LINE5      ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.TELEPHONE_NO1                 ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.TELEPHONE_NO2                 ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.TELEPHONE_NO3                 ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.FAX_NO1                       ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.FAX_NO2                       ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.EMAIL                         ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.RANK                          ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.POST                          ");
            yield return ValidationUtil.Validate_Length(this, "C_COMMITTEE_MEMBER.CAREER                        ");


            yield return ValidationUtil.Validate_Mandatory(this
                , "C_COMMITTEE_MEMBER.C_APPLICANT.HKID"
                , "C_COMMITTEE_MEMBER.C_APPLICANT.PASSPORT_NO       ");
            yield return ValidationUtil.Validate_Mandatory(this, "C_COMMITTEE_MEMBER.C_APPLICANT.SURNAME           ");
            yield return ValidationUtil.Validate_Mandatory(this, "C_COMMITTEE_MEMBER.C_APPLICANT.GIVEN_NAME_ON_ID  ");

            yield return ValidationUtil.Validate_Mandatory(this
                , "C_COMMITTEE_MEMBER.C_ADDRESS.ADDRESS_LINE1       "
                , "C_COMMITTEE_MEMBER.C_ADDRESS.ADDRESS_LINE2       "
                , "C_COMMITTEE_MEMBER.C_ADDRESS.ADDRESS_LINE3       "
                , "C_COMMITTEE_MEMBER.C_ADDRESS.ADDRESS_LINE4       "
                , "C_COMMITTEE_MEMBER.C_ADDRESS.ADDRESS_LINE5       ");

            yield return ValidationUtil.Validate_Mandatory(this, "C_COMMITTEE_MEMBER.TELEPHONE_NO1                 ");
        }

        public List<SelectListItem> getTitle()
        {
            return SystemListUtil.RetrieveTitle();
        }
        public IEnumerable<SelectListItem> COMMITTEE_MEMBER_STATUSs
        {
            get
            {
                return
                     SystemListUtil.GetSVListByType(RegistrationConstant.SYSTEM_TYPE_COMMITTEE_MEMBER_STATUS)
                            .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID });
            }
        }
        public IEnumerable<SelectListItem> GENDER_List
        {
            get
            {
                return
                    (new List<SelectListItem>() {new SelectListItem() { Text = "- Select -", Value = "" },
                        new SelectListItem() { Text = "M", Value = "M" },
                        new SelectListItem() { Text = "F", Value = "F" }});
            }
        }
    }
}