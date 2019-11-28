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
    public class Fn06CMM_CMMSearchModel : EditFormModel, IValidatableObject
    {
        public Dictionary<string, string> PanelRole { get; set; }
        public List<string> CheckMembers { get; set; }
        public string Panel { get; set; }
        public string Year { get; set; }
        public string Committee { get; set; }
        public C_COMMITTEE C_COMMITTEE { get; set; }
        public int? SearchType { get; set; }
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Display(Name = "Given Name")]
        public string GivenName { get; set; }

        public IEnumerable<SelectListItem> YEAR_List
        {
            get
            {
                List<SelectListItem> r = new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } };
                for (int i = 0; i < 12; i++)
                {
                    r.Add(new SelectListItem() { Text = (DateTime.Now.Year + 1 - i).ToString(), Value = (DateTime.Now.Year + 1 - i).ToString() });
                }
                return r;
            }
        }
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
        public IEnumerable<SelectListItem> getComTypeByParent(string pUUID)
        {
            return
              (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
              .Concat(SystemListUtil.GetSVListByParentUUID(
             pUUID)
             .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID }));

        }
        public IEnumerable<SelectListItem> SYSTEM_TYPE_COMMITTEE_TYPE_List_BY_PARENT
        {
            get
            {

                return (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } });


            }
        }

        public IEnumerable<SelectListItem> SYSTEM_TYPE_COMMITTEE_TYPE_List
        {
            get
            {
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
                             RegistrationConstant.SYSTEM_TYPE_PANEL_ROLE)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }));
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PanelRole != null)
            {
                foreach (string key in PanelRole.Keys)
                {
                    yield return ValidationUtil.Validate_Mandatory(PanelRole[key], "Please select role.", "PanelRole[" + key + "]");


                }
            }

            yield return ValidationUtil.Validate_Mandatory(this, "C_COMMITTEE.C_COMMITTEE_PANEL.PANEL_TYPE_ID");

            yield return ValidationUtil.Validate_Mandatory(this, "C_COMMITTEE.YEAR");

            yield return ValidationUtil.Validate_Mandatory(this, "C_COMMITTEE.COMMITTEE_TYPE_ID");
        }
    }
}