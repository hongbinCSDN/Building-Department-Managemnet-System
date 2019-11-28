using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Entity;
using System.Web.Mvc;
using MWMS2.Utility;
using System.ComponentModel.DataAnnotations;

namespace MWMS2.Areas.Admin.Models
{
    
    public class Sys_UMModel : DisplayGrid, IValidatableObject
    {
        [Display(Name = "Post")]
        public SYS_POST SYS_POST { get; set; }
        public string[] SelectedRoles { get; set; }
        public string Role { get; set; }


        [Display(Name = "Confirm DSMS Password")]
        public string Dsms_pw2 { get; set; }

        [Display(Name = "Confirm Password")]
        public string Pw2 { get; set; }
        public IEnumerable<SelectListItem> Status
        {
            get
            {
                return (new List<SelectListItem>()
                {
                    new SelectListItem{Value = "" , Text="- Select -"},
                    new SelectListItem{Value = "Y" , Text="Active"},
                    new SelectListItem{Value = "N" , Text="Inactive"},
                });
            }
        }

        public IEnumerable<SelectListItem> Rank
        {
            get
            {
                return SystemListUtil.GetSysRank();
            }
        }

        public List<SYS_POST_ROLE> SysPostRole { get; set; }




        public List<string> ScuSubordinateMembers { get; set; }
        public List<string> PemSubordinateMembers { get; set; }
        public List<string> ScuResponsibleAreas { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Length(this, "SYS_POST.CODE");
            yield return ValidationUtil.Validate_Mandatory(this, "SYS_POST.CODE");
            yield return ValidationUtil.Validate_Mandatory(this, "SYS_POST.SYS_RANK_ID");
            yield return ValidationUtil.Validate_IsEqual(this, "SYS_POST.DSMS_PW", "Dsms_pw2");
            yield return ValidationUtil.Validate_IsEqual(this, "SYS_POST.PW", "Pw2");


        }
    }
}