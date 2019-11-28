using MWMS2.Entity;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Models
{
    public class LeaveFormDisplayModel : IValidatableObject
    {
        public string RegType { get; set; }
        public C_COMP_APPLICANT_INFO C_COMP_APPLICANT_INFO { get; set; }
        public C_COMP_APPLICATION C_COMP_APPLICATION { get; set; }
        public C_APPLICANT C_APPLICANT { get; set; }
        public C_LEAVE_FORM C_LEAVE_FORM { get; set; }
        public C_IND_CERTIFICATE C_IND_CERTIFICATE { get; set; }
        public C_IND_APPLICATION C_IND_APPLICATION { get; set; }
        public string ChiName { get; set; }
        public string ASName { get; set; }
        public string RoleType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Remark { get; set; }
        public DateTime? CancelStartDate { get; set; }
        public DateTime? CancelEndDate { get; set; }
        public string CancelRemark { get; set; }
        public string mode { get; set; }
        public IEnumerable<SelectListItem> Nature
        {
            get { return SystemListUtil.GetLeaveFormNature("L"); }
        }
        public string SelectedNatureType { set; get; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (RegType =="CGC" || RegType == "CMW") { 
                if(mode == "C")
                {
                    yield return ValidationUtil.Validate_Mandatory(this, "CancelStartDate");
                    yield return ValidationUtil.Validate_Mandatory(this, "CancelEndDate");
                    yield return ValidationUtil.Validate_Mandatory(this, "CancelRemark");
                }
                else
                {
                    yield return ValidationUtil.Validate_Mandatory(this, "StartDate");
                    yield return ValidationUtil.Validate_Mandatory(this, "EndDate");
                    yield return ValidationUtil.Validate_Mandatory(this, "Remark");
                }
            }
            else
            {
                if (mode == "C")
                {
                    yield return ValidationUtil.Validate_Mandatory(this, "CancelIndStartDate");
                    yield return ValidationUtil.Validate_Mandatory(this, "CancelIndEndDate");
                    yield return ValidationUtil.Validate_Mandatory(this, "CancelIndRemark");
                }
                else
                {
                    yield return ValidationUtil.Validate_Mandatory(this, "IndStartDate");
                    yield return ValidationUtil.Validate_Mandatory(this, "IndEndDate");
                    yield return ValidationUtil.Validate_Mandatory(this, "IndRemark");
                }
            }

        }
        public bool SaveSuccess { get; set; }
        public List<C_LEAVE_FORM> LeaveFormList { get; set; }
        public string ErrorMessage { get; set; }
        public string IndRemark { get; set; }
        public DateTime? IndStartDate { get; set; }
        public DateTime? IndEndDate { get; set; }
        public string CancelIndRemark { get; set; }
        public DateTime? CancelIndStartDate { get; set; }
        public DateTime? CancelIndEndDate { get; set; }
    }
}