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

    public class ApplicantDisplayListModel
    {
        public static List<ApplicantDisplayListModel> Load(List<C_COMP_APPLICANT_INFO> v)
        {
            List<ApplicantDisplayListModel> r = new List<ApplicantDisplayListModel>();
            for (int i = 0; i < v.Count; i++)
                r.Add(new ApplicantDisplayListModel().Load(v[i]));
            return r;
        }

        public ApplicantDisplayListModel Load(C_COMP_APPLICANT_INFO v)
        {
            HKID = v?.C_APPLICANT?.HKID;
            PASSPORT = v?.C_APPLICANT?.PASSPORT_NO;
            TITLE = v?.C_APPLICANT?.C_S_SYSTEM_VALUE?.ENGLISH_DESCRIPTION;
            STATUS = v?.C_S_SYSTEM_VALUE1?.ENGLISH_DESCRIPTION;
            SURNAME = v?.C_APPLICANT?.SURNAME;
            GIVEN_NAME_ON_ID = v?.C_APPLICANT?.GIVEN_NAME_ON_ID;
            GENDER = v?.C_APPLICANT?.GENDER;
            ROLE = v?.C_S_SYSTEM_VALUE?.CODE;
            ACCEPTANCE_DATE = v?.ACCEPT_DATE;
            REMOVAL_DATE = v?.REMOVAL_DATE;
            WITHDRAWN_DATE = v?.INTERVIEW_WITHDRAWN_DATE;
            CARD_ISSUE_DATE = v?.CARD_ISSUE_DATE;
            CARD_EXPIRY_DATE = v?.CARD_EXPIRY_DATE;
            CARD_SERIAL_NO = v?.CARD_SERIAL_NO;
            CARD_RETURN_DATE = v?.CARD_RETURN_DATE;
            REMARK = v?.REMARK;
            C_COMP_APPLICANT_INFO_UUID = v?.UUID;
            C_APPLICANT_UUID = v?.C_APPLICANT?.UUID;
            EFORM = v.EFORM;
            FILEUUID = v.UUID;
            FILEEXSIST = false;
            FILEEXSIST = v.FILEEXSIST;
            FILEEXSIST = v.FILE_PATH_NONRESTRICTED == null ? FILEEXSIST : true;
            return this;
        }
        public C_COMP_APPLICANT_INFO toC_COMP_APPLICANT_INFO()
        {
            return null;
        }


        public string RegType { get; set; }
        public C_COMP_APPLICANT_INFO C_COMP_APPLICANT_INFO { get; set; }
        public string C_COMP_APPLICANT_INFO_MASTER_UUID  {get;set;}
        public DisplayGrid APPLICATION_HISTORY { get; set; }
        public DisplayGrid MW_CAPABILITY { get; set; }
        public bool EFORM { get; set; }
        public string FILEUUID { get; set; }
        public bool FILEEXSIST { get; set; }
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
                        new SelectListItem() { Text = "English", Value = "E" },
                        new SelectListItem() { Text = "Chinese", Value = "C" }});
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

    }
}