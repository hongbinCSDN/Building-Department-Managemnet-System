using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Models
{
    
    public class AppHistDisplayModel
    {


        public string RegType { get; set; }
        public C_COMP_APPLICANT_INFO C_COMP_APPLICANT_INFO { get; set; }
        public C_COMP_APPLICANT_INFO_MASTER C_COMP_APPLICANT_INFO_MASTER { get; set; }
       // public DisplayGrid APPLICATION_HISTORY { get; set; }
       // public DisplayGrid MW_CAPABILITY { get; set; }

        public string HKID
        {
            get; set;
        }
        public string PASSPORT
        {
            get; set;
        }
        
        public string SURNAME { set; get; }
        public string GIVEN_NAME_ON_ID { set; get; }
        public string STATUS { set; get; }
        
        [Display(Name = "English Name")]
        public string NAME { get { return this.SURNAME + " " + this.GIVEN_NAME_ON_ID; } }

        [Display(Name = "Chinese Name")]
        public string CHINESE_NAME { get; set; }

        public string HKID_PASSPORT_DISPLAY { get { return string.IsNullOrWhiteSpace(HKID) || string.IsNullOrWhiteSpace(PASSPORT) ? HKID + PASSPORT : HKID + "/ " + PASSPORT; } }
        public string TITLE { get; set; } = "";
        public string GENDER { get; set; } = "";
        public string ROLE { get; set; } = "";
        public Nullable<System.DateTime> ACCEPTANCE_DATE { get; set; }
        public Nullable<System.DateTime> REMOVAL_DATE { get; set; }
        public Nullable<System.DateTime> WITHDRAWN_DATE { get; set; }
        public Nullable<System.DateTime> CARD_ISSUE_DATE { get; set; }

        public Nullable<System.DateTime> CARD_EXPIRY_DATE { get; set; }

        public string CARD_SERIAL_NO { get; set; }

        public Nullable<System.DateTime> CARD_RETURN_DATE { get; set; }
        public string REMARK { get; set; } = "";
        public string SIGN { get; set; } = "";
        public string C_APPLICANT_UUID { get; set; }
        public string C_COMP_APPLICANT_INFO_UUID { get; set; }
        
     


        public IEnumerable<SelectListItem> APPLICANT_ROLE_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByRegTypeNType(
                            RegistrationConstant.REGISTRATION_TYPE_CGA
                            , RegistrationConstant.SYSTEM_TYPE_APPLICANT_ROLE)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }));
            }
        }
        public IEnumerable<SelectListItem> APPLICANT_STATUS_ID_List
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
        
        public IEnumerable<SelectListItem> CORRESPONDENCE_LANG_List
        {
            get
            {
                return
                    (new List<SelectListItem>() {
                        new SelectListItem() { Text = "Chinese", Value = "C" },
                        new SelectListItem() { Text = "English", Value = "E" }});
            }
        }
        public IEnumerable<SelectListItem> TITLE_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(
                             RegistrationConstant.SYSTEM_TYPE_TITLE)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }));
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
        public IEnumerable<SelectListItem> APPLICATION_FORM_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByRegTypeNType(
                            RegType
                           // RegistrationConstant.REGISTRATION_TYPE_CGA
                            , RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM)
                            .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID }));
            }
        }
        public IEnumerable<SelectListItem> GetApprovedByList
        {
            get
            {
                return (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByRegTypeNType(RegType,
                            RegistrationConstant.SYSTEM_TYPE_APPROVED_BY_LIST)
                            .Select(o => new SelectListItem() { Text = o.CODE, Value = o.CODE }));
            }
        }
    }
}