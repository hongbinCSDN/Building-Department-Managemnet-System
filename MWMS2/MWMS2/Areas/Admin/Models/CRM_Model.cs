using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;

namespace MWMS2.Areas.Admin.Models
{
    public class CRM_Model : EditFormModel
    {
        private const string V = "Company Type";

        public string Year { get; set; }

        public List<C_S_PUBLIC_HOLIDAY> Publicholidays { get; set; }

        /**
        // public C_S_PUBLIC_HOLIDAY[] PHList{ get; set; }
        public Dictionary<string, string> HolidayChiList { get; set; }
        public Dictionary<string, string> HolidayEngList { get; set; }
        public Dictionary<string, string> HolidayDateList { get; set; }
        **/



        public string ImportType { get; set; }

        public List<SelectListItem> getYear()
        {
            return SystemListUtil.GetYearList();
        }
        public string SystemType { get; set; }
        public string DisplayType
        {
            get
            {
                string tmp = "";
                if (SystemType != null)
                {
                    switch (SystemType)
                    {
                        case Constant.RegistrationConstant.SYSTEM_TYPE_COMPANY_TYPE:
                            tmp = V;
                            break;
                        case Constant.RegistrationConstant.SYSTEM_TYPE_COMMITTEE_TYPE:
                            tmp = "Committee Type";
                            break;
                        case Constant.RegistrationConstant.SYSTEM_TYPE_CONVICTION_SOURCE:
                            tmp = "Conviction Source";
                            break;
                        case Constant.RegistrationConstant.SYSTEM_TYPE_PANEL_TYPE:
                            tmp = "Panel Type";
                            break;
                        case Constant.RegistrationConstant.SYSTEM_TYPE_BUILDING_SAFETY_CODE:
                            tmp = "Building Safety Code";
                            break;
                        case Constant.RegistrationConstant.SYSTEM_TYPE_MWTYPE:
                            tmp = "New Minor Works Type";
                            break;
                        case Constant.RegistrationConstant.SYSTEM_TYPE_CATEGORY_GROUP:
                            tmp = "Category Group";
                            break;
                        case Constant.RegistrationConstant.SYSTEM_TYPE_HTML_NOTES:
                            tmp = "HTML Notes";
                            break;
                        case Constant.RegistrationConstant.SYSTEM_TYPE_INTERVIEW_RESULT:
                            tmp = "Interview Result";
                            break;
                        case Constant.RegistrationConstant.SYSTEM_TYPE_PANEL_ROLE:
                            tmp = "Panel Role";
                            break;
                        case Constant.RegistrationConstant.SYSTEM_TYPE_COMMITTEE_ROLE:
                            tmp = "Committee Role";
                            break;
                        case Constant.RegistrationConstant.SYSTEM_TYPE_NON_BUILDING_WORKS_RELATED:
                            tmp = "Non-building Works Related Labour Safety Offences";
                            break;
                        case Constant.RegistrationConstant.SYSTEM_TYPE_SECRETARY:
                            tmp = "Secretary";
                            break;
                        case Constant.RegistrationConstant.SYSTEM_TYPE_ASSISTANT:
                            tmp = "Assistant";
                            break;
                        case Constant.RegistrationConstant.SYSTEM_TYPE_VETTING_OFFICER:
                            tmp = "Vetting Officer";
                            break;
                        case Constant.RegistrationConstant.SYSTEM_TYPE_MWCLASS:
                            tmp = "Minor Works Class";
                            break;
                        case Constant.RegistrationConstant.SYSTEM_TYPE_TITLE:
                            tmp = "Title";
                            break;
                        case Constant.RegistrationConstant.SYSTEM_TYPE_SOCIETY_NAME:
                            tmp = "Society Name";
                            break;
                        case Constant.RegistrationConstant.SYSTEM_TYPE_PROF_REGISTRATION_BOARD:
                            tmp = "Professional Registration Board";
                            break;
                        case Constant.RegistrationConstant.SYSTEM_TYPE_PRACTICE_NOTE:
                            tmp = "Practice Notes";
                            break;
                        case Constant.RegistrationConstant.SYSTEM_TYPE_PERIOD_OF_VALIDITY:
                            tmp = "Period of Validity";
                            break;
                        default:
                            tmp = SystemType;
                            break;
                    }
                }
                return tmp;
            }
            set { SystemType = value; }
        }

        public string CODE { get; set; }
        public string RegType { get; set; }
        public IEnumerable<SelectListItem> RegTypeList { set; get; } = SystemListUtil.RetrieveRegType();
        public string EngDesc { get; set; }
        public string ChiDesc { get; set; }
        public Nullable<decimal> Ord { get; set; }
        public bool IsActive { get; set; }

    }
}