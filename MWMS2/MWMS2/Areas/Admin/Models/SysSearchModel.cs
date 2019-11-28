using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Admin.Models
{
    public class SysSearchModel : DisplayGrid
    {
        [Display(Name = "Role Name")]
        public string SearchRoleCode { get; set; }
        [Display(Name = "Description")]
        public string SearchRoleDesc { get; set; }
    }

    public class SysSearchRoleModuleModel : DisplayGrid, IValidatableObject
    {
        public Dictionary<string, string> FirstModule { get; set; }

        public List<RoleFunc> RoleFuncList { get; set; }

        public List<UserGroup> UserGoupList { get; set; }

        public List<Level> LevelList { get; set; }

        public SYS_ROLE SYS_ROLE { get; set; }

        public string ClassName1 { get; set; }
        public string ClassName2 { get; set; }
        public string ClassName3 { get; set; }

        public string LevelType { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return ValidationUtil.Validate_Length(this, "SYS_ROLE.DESCRIPTION");
        }

    }

    public class RoleFunc
    {
        public string SYS_FUNC_ID { get; set; }
        public string PARENT_ID { get; set; }
        public string CODE { get; set; }
        public string DESCRIPTION { get; set; }
        public string USE_TYPE { get; set; }

        public bool AbleShow
        {
            get { return ABLE_SHOW == "Y"; }
            set { ABLE_SHOW = value ? "Y" : "N"; }
        }
        public string ABLE_SHOW { get; set; }

        public bool CanList
        {
            get { return CAN_LIST == "Y"; }
            set { CAN_LIST = value ? "Y" : "N"; }
        }
        public string CAN_LIST { get; set; }

        public bool CanViewDetails
        {
            get { return CAN_VIEW_DETAILS == "Y"; }
            set { CAN_VIEW_DETAILS = value ? "Y" : "N"; }
        }
        public string CAN_VIEW_DETAILS { get; set; }

        public bool CanCreate
        {
            get { return CAN_CREATE == "Y"; }
            set { CAN_CREATE = value ? "Y" : "N"; }
        }
        public string CAN_CREATE { get; set; }

        public bool CanEdit
        {
            get { return CAN_EDIT == "Y"; }
            set { CAN_EDIT = value ? "Y" : "N"; }
        }
        public string CAN_EDIT { get; set; }

        public bool CanDelete
        {
            get { return CAN_DELETE == "Y"; }
            set { CAN_DELETE = value ? "Y" : "N"; }
        }
        public string CAN_DELETE { get; set; }

        public bool IsChecked
        {
            get { return IS_CHECKED == "Y"; }
            set { IS_CHECKED = value ? "Y" : "N"; }
        }
        public string IS_CHECKED { get; set; }
    }

    public class UserGroup
    {
        public string UUID { get; set; }
        public string ENGLISH_DESCRIPTION { get; set; }
        public string IS_CHECKED { get; set; }
        public bool IsChecked
        {
            get { return IS_CHECKED == "Y"; }
            set { IS_CHECKED = value ? "Y" : "N"; }
        }
    }

    public class Level
    {
        public string UUID { get; set; }
        public string ENGLISH_DESCRIPTION { get; set; }
        public string REGISTRATION_TYPE { get; set; }
        public string IS_CHECKED { get; set; }
        public bool IsChecked
        {
            get { return IS_CHECKED == "Y"; }
            set { IS_CHECKED = value ? "Y" : "N"; }
        }
    }
}