using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Models
{
    public class Fn08ADM_ReportModel : DisplayGrid
    {
        public string UserAccount { get; set; }
        public IEnumerable<SelectListItem> UserAccountList { get { return SystemListUtil.UserAccountName(); } }
        public string VettingOfficer { get; set; }
        public IEnumerable<SelectListItem> VettingOfficerList { get { return SystemListUtil.GetVetCIHROfficer(); } }
        public string CaseStatus { get; set; }
        public IEnumerable<SelectListItem> CaseStatusList { get { return SystemListUtil.CaseStatusList(); } }
        public IEnumerable<SelectListItem> arrCategoryList { get { return SystemListUtil.arrCategoryList(); } }
        public string arrTypeOfApplication { get; set; }
        public IEnumerable<SelectListItem> arrTypeOfApplicationList { get { return SystemListUtil.arrTypeOfApplicationList(); } }
        public string arrInterviewResult { get; set; }
        public IEnumerable<SelectListItem> arrInterviewResultList { get { return SystemListUtil.arrInterviewResultList(); } }
        public DateTime? fr_date { get; set; }
        public DateTime? to_date { get; set; }
        public string fileReference { get; set; }

        public C_S_SYSTEM_VALUE C_S_SYSTEM_VALUE { get; set; }

        //public DateTime? received_fr_date { get {
        //        //DateTime now = DateTime.Today;
        //        DateTime year = DateTime.Now.AddMonths(-12);
        //        //TimeSpan Date = now - year;
        //        return year; } }
        public DateTime? received_fr_date { get; set; }
        //public DateTime? received_to_date { get { return DateTime.Today; } }
        public DateTime? received_to_date { get; set; }
        public DateTime? due_fr_date { get; set; }
        public DateTime? due_to_date { get; set; }
        public DateTime? interview_fr_date { get; set; }
        public DateTime? interview_to_date { get; set; }
        public DateTime? result_fr_date { get; set; }
        public DateTime? result_to_date { get; set; }
        public string TypeOfLetter { get; set; }
        public IEnumerable<SelectListItem> LetterTypeList { get { return SystemListUtil.LetterTypeResultList(); } }
        public string AllTypeOfApp { get; set; }
        public bool CategoryAll { get; set; }
        public bool CategoryIP { get; set; }
        public bool CategoryGBC { get; set; }
        public bool CategoryMWC { get; set; }
        public bool CategoryMWI { get; set; }
        public Dictionary<string,string> ApplicationTypeList { get; set; }
        public List<string> irItem { get; set; }
        public string TypeOfIntVRes { get; set; }
        //public IEnumerable<SelectListItem> InterViewResultList { get { return SystemListUtil.AdmInterviewResultList(); } }
        public Dictionary<string, string> InterViewResultList { get; set; }

        public string RegType { get; set; }
        //Missing Item
        public bool Professionals { get; set; }
        public bool Contractors { get; set; }
        public bool CRMWC { get; set; }
        public bool IRMWC { get; set; }
    }
}