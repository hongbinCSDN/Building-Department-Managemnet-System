using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;

namespace MWMS2.Areas.Registration.Models
{
    public class Fn06CMM_CPSearchModel : EditFormModel,IValidatableObject
    {
        public Dictionary<string, string> PanelRole { get; set; }
        public List<string> CheckMembers { get; set; }
        public string Panel { get; set; }
        public string Year { get; set; }

        public IEnumerable<SelectListItem> SYSTEM_TYPE_PANEL_TYPE_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(
                             RegistrationConstant.SYSTEM_TYPE_PANEL_TYPE)
                            .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID }));
            }
        }
        public IEnumerable<SelectListItem> getNextYearAndLastTenYear
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.getNextYearAndLastTenYear()
                            .Select(o => new SelectListItem() { Text = o.ToString(), Value = o.ToString() }));
            }
        }

        public int? SearchType { get; set; }
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Display(Name = "Given Name")]
        public string GivenName { get; set; }

        //public C_COMMITTEE C_COMMITTEE { get; set; }
        public C_COMMITTEE_PANEL C_COMMITTEE_PANEL { get; set; }
        public DateTime? ExpiryDateTime { get; set; }
        public IEnumerable<SelectListItem> SYSTEM_TYPE_COMMITTEE_TYPE_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(
                             RegistrationConstant.SYSTEM_TYPE_COMMITTEE_TYPE)
                            .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID }));
            }
        }
        public IEnumerable<SelectListItem> PANEL_ROLE_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(
                             RegistrationConstant.SYSTEM_TYPE_PANEL_ROLE)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }));
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            yield return ValidationUtil.Validate_Mandatory(this, "C_COMMITTEE_PANEL.YEAR");
            yield return ValidationUtil.Validate_Mandatory(this, "C_COMMITTEE_PANEL.PANEL_TYPE_ID");




        }
    }
}