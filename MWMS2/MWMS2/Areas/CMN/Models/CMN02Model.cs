using MWMS2.Entity;
using MWMS2.Filter;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.CMN.Models
{

    public class CMN02Model : DisplayGrid
    {
        public string Keyword { get; set; }
        public List<QSTable> QSList { get; set; } = new List<QSTable>();
        public List<SYS_QUICK_SEARCH> QSTable { get; set; } = new List<SYS_QUICK_SEARCH>();




        public string Search_ChiName { get; set; }
        public string Search_EngName { get; set; }
        public DateTime? Search_ExpiryDateFrom { get; set; }
        public DateTime? Search_ExpiryDateTo { get; set; }
        public string Search_RegistrationNo { get; set; }
        public string Search_ServicesBS { get; set; }
        public string Search_ChiCompName { get; set; }
        public string Search_EngCompName { get; set; }
        public string Search_ASEngName { get; set; }
        public string Search_ASChiName { get; set; }
        public string Search_TelBS { get; set; }
        // public List<ProcessingMWTable> ProcessingMWList { get; set; } = new List<ProcessingMWTable>();


    }

    //public class ProcessingMWTable
    //{
    //    public string KEYWORD { get; set; }
    //    public string KEYWORD_TYPE { get; set; }
    //    public string SUBMISSION_NATURE { get; set; }
    //    public string MW_DSN { get; set; }
    //    public DateTime? FIRST_RECEIVED_DATE { get; set; }
    //    public string S_FORM_TYPE_CODE { get; set; }
    //    public string REFERENCE_NO { get; set; }
    //    public string CERTIFICATION_NO_AP { get; set; }

    //    public string CERTIFICATION_NO_PRC { get; set; }
    //    public string LOCATION_OF_MINOR_WORK { get; set; }
    //    public string STREE_NAME { get; set; }
    //    public string STREET_NO { get; set; }
    //    public string BUILDING_NAME { get; set; }
    //    public string FLOOR { get; set; }
    //    public string FLAT { get; set; }
    //    public string SITE_AUDIT_RELATED { get; set; }
    //    public string PRE_SITE_AUDIT_RELATED { get; set; }
    //    public string AUDIT_RELATED { get; set; }
    //    public string VERIFICATION_SPO { get; set; }
    //    public DateTime? COMMENCEMENT_DATE { get; set; }
    //    public DateTime? COMPLETION_DATE { get; set; }
    //    public string NAME_ENGLISH_OWNER { get; set; }
    //    public string CONTACT_NO_OWNER { get; set; }
    //    public string NAME_ENGLISH_OI { get; set; }
    //    public string CONTACT_NO_OI { get; set; }
    //    public string FILEREF_FOUR_TWO { get; set; }
    
    //}


    public class QSTable
    {
      
        public string KEYWORD { get; set; }
        public string RECORD_UUID { get; set; }
        public string RECORD_TYPE { get; set; }
        public string REGISTRATION_NO { get; set; }
        public string APPLICANT_SURNAME { get; set; }
        public string APPLICANT_GIVEN_NAME { get; set; }
        public string APPLICANT_CHINESE_NAME { get; set; }
        public string COMP_NAME { get; set; }
        public string COMP_CHI_NAME { get; set; }
        public string AS_SURNAME { get; set; }
        public string AS_GIVEN_NAME { get; set; }
        public string AS_CHI_NAME { get; set; }
        public string HKID { get; set; }
        public string PASSPORT { get; set; }
        public string UKEY { get; set; }
        public string KEYWORD_TYPE { get; set; }
        public string COMP_IND_TYPE { get; set; }
        public DateTime? EXPIRY_DATE { get; set; }
    }
}