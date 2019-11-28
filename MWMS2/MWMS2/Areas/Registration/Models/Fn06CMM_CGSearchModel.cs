using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Utility;
using MWMS2.Entity;
namespace MWMS2.Areas.Registration.Models
{
    public class Fn06CMM_CGSearchModel : EditFormModel, IValidatableObject
    {
        public string Panel { get; set; }
        public string Year { get; set; }
        public string Committee { get; set; }
        public string CommitteeGroup { get; set; }
        public string Month { get; set; }

        public Dictionary<string, string> PanelRole { get; set; }
        public List<string> CheckMembers { get; set; }


        public C_COMMITTEE_GROUP C_COMMITTEE_GROUP { get; set; }
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

        public IEnumerable<SelectListItem> SYSTEM_TYPE_COMMITTEE_TYPE_List
        {
            get
            {
                List<C_S_SYSTEM_VALUE> tem = SystemListUtil.GetSVListByType(RegistrationConstant.SYSTEM_TYPE_COMMITTEE_TYPE).ToList();
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(
                             RegistrationConstant.SYSTEM_TYPE_COMMITTEE_TYPE)
                            .Where(o => !string.IsNullOrWhiteSpace(o.CODE))
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
                             RegistrationConstant.SYSTEM_TYPE_COMMITTEE_ROLE)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }));
            }
        }
        public IEnumerable<SelectListItem> AToZList
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.getAtoZ()
                            .Select(o => new SelectListItem() { Text = o, Value = o }));
            }
        }
        public IEnumerable<SelectListItem> MonthList
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.getMonth()
                            .Select(o => new SelectListItem() { Text = o.ToString(), Value = o.ToString() }));
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Mandatory(this, "C_COMMITTEE_GROUP.C_COMMITTEE.C_COMMITTEE_PANEL.PANEL_TYPE_ID");
            yield return ValidationUtil.Validate_Mandatory(this, "C_COMMITTEE_GROUP.YEAR");
            yield return ValidationUtil.Validate_Mandatory(this, "C_COMMITTEE_GROUP.COMMITTEE_TYPE_ID");
            yield return ValidationUtil.Validate_Mandatory(this, "C_COMMITTEE_GROUP.NAME");
            yield return ValidationUtil.Validate_Mandatory(this, "C_COMMITTEE_GROUP.MONTH");

            //yield return null;           

            //throw new NotImplementedException();
        }
    }
}