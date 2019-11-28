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
    public class Fn06AS_DBDisplayModel : DisplayGrid
    {
        public C_APPLICANT C_APPLICANT { get; set; }
        public C_AS_CONSENT C_AS_CONSENT { get; set; }
        public C_COMP_APPLICANT_INFO C_COMP_APPLICANT_INFO { get; set; }
        public C_ADDRESS ChinessAddess { get; set; }
        public C_ADDRESS EnglishAddress { get; set; }
        public string EngFullName { get; set; }
        public IEnumerable<SelectListItem> ConsentList
        {
            get { return SystemListUtil.GetSVConsentListByRegType(RegistrationConstant.SYSTEM_TYPE_AS_DROPDOWN); }
        }
        public IEnumerable<SelectListItem> ClassList
        {
            get { return SystemListUtil.GetSVClassListByRegType(RegistrationConstant.SYSTEM_TYPE_AS_CLASS_DROPDOWN); }
        }
        public string Email1{get; set;}
        public string Email2 { get; set; }
        public string ID{ get;set;}
        public bool SaveSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public string FilePath { get; set; }
        public string STATUS { get; set; }
        public string ClassOfMW { get; set; }
        public string ConsentStatus { get; set; }
    }
}