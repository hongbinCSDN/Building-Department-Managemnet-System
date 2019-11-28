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

    public class CRMCommonDropDownList : DisplayGrid
    {
        public string RegType { get; set; }

        public IList<SelectListItem> CATEGORY_CODE_List_FOR_CHECK_BOX()
        {
            return SystemListUtil.RetrieveQCByType(RegType);
        }



        public IEnumerable<SelectListItem> CATEGORY_CODE_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(
                        SystemListUtil.GetCatCodeByRegType(RegType)
                         .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID }));
            }
        }

   
        public IEnumerable<SelectListItem> APPLICATION_STATUS_List
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
        public IEnumerable<SelectListItem> COMPANY_TYPE_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select Company Type -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(
                            RegistrationConstant.SYSTEM_TYPE_COMPANY_TYPE)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }));
            }
        }

        public IEnumerable<SelectListItem> PERIOD_OF_VALIDITY_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(
                            RegistrationConstant.SYSTEM_TYPE_PERIOD_OF_VALIDITY)
                            .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID }));
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
                            , RegistrationConstant.SYSTEM_TYPE_APPLICATION_FORM)
                            .Select(o => new SelectListItem() { Text = o.CODE, Value = o.UUID }));
            }
        }
        public IEnumerable<SelectListItem> PRACTICE_NOTES_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(
                            RegistrationConstant.SYSTEM_TYPE_PRACTICE_NOTE)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }));
            }
        }



        public IEnumerable<SelectListItem> WILLINGNESS_QP_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- All -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(
                            RegistrationConstant.SYSTEM_TYPE_QPSERVICES)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.CODE }));
            }
        }

        public IEnumerable<SelectListItem> INTERESTED_FSS_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(
                            RegistrationConstant.SYSTEM_TYPE_FSS_DROPDOWN)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.CODE }));
            }
        }
        public IEnumerable<SelectListItem> BS_REGION_CODE_ID_List
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.GetSVListByType(
                            RegistrationConstant.SYSTEM_TYPE_REGION_CODE)
                            .Select(o => new SelectListItem() { Text = o.ENGLISH_DESCRIPTION, Value = o.UUID }));
            }
        }


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