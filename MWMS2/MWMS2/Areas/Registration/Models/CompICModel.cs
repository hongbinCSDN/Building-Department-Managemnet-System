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

    public class CompICModel : DisplayGrid
    {
        public bool CannotEditFlag { get; set; }

        public string RegType { get; set; }
        public string Year { get; set; }
        public DateTime? InterviewDate { get; set; }
        public string Group { get; set; }
        public string InterviewType { get; set; }
        public string Type { get; set; }
        public string UUID { get; set; }
        public string FileRef { get; set; }
        public string MeetingNo { get; set; }
        public string ModifiedTime { get; set; }
        public string CandidateNo { get; set; }
        public string Duration { get; set; }
        public string StartTime
        {
            //get
            //{
            //    if ("PM".Equals(C_INTERVIEW_SCHEDULE?.C_S_SYSTEM_VALUE?.CODE)) return "02:15 PM";
            //    return "09:15 AM";
            //}
            //set { }
            get;set;
        }
        public string CurrentDate
        {
            get
            {
                return DateUtil.getEnglishCurrentDate();
            }
            set { }
        }
        public Dictionary<string, Dictionary<string, string>> GenData { get; set; }

        public C_INTERVIEW_SCHEDULE C_INTERVIEW_SCHEDULE { get; set; }
        public C_INTERVIEW_CANDIDATES C_INTERVIEW_CANDIDATES = new C_INTERVIEW_CANDIDATES();

        public IEnumerable<SelectListItem> GroupList
        {
            get { return SystemListUtil.GetGroupList(); }
        }

        public IEnumerable<SelectListItem> TypeICList
        {
            get { return SystemListUtil.GetICType(RegType); }
        }

        public IEnumerable<SelectListItem> TypeICList_IP
        {
            get { return SystemListUtil.GetICType_IP(); }
        }

        public IEnumerable<SelectListItem> TypeICList_MW
        {
            get { return SystemListUtil.GetICType_MW(); }
        }

        public IEnumerable<SelectListItem> showGroupList
        {
            get { return SystemListUtil.GetGroupList(); } //real A-Z
        }

        public IEnumerable<SelectListItem> showTypeList
        {
            get { return SystemListUtil.RetrieveTypeByCommitteeTypeID(RegistrationConstant.REGISTRATION_TYPE_MWCA); }
        }

        public IEnumerable<SelectListItem> yearList
        {
            get { return SystemListUtil.RetrieveYearList(); }
        }

        public IEnumerable<SelectListItem> getInterviewType
        {
            get { return SystemListUtil.GetInterviewTypeList(); }
        }
        public IEnumerable<SelectListItem> getNextYearAndLastTenYear
        {
            get
            {
                return
                    (new List<SelectListItem>() { new SelectListItem() { Text = "- Select -", Value = "" } })
                        .Concat(SystemListUtil.getNextYearAndLastTenYear()
                            .Select(o => new SelectListItem() { Text = o.ToString(), Value = o.ToString() }));
            }
        }

        public string REGISTRATION_TYPE_CGA = RegistrationConstant.REGISTRATION_TYPE_CGA;
        public string REGISTRATION_TYPE_IP = RegistrationConstant.REGISTRATION_TYPE_IP;
        public string REGISTRATION_TYPE_MWCA = RegistrationConstant.REGISTRATION_TYPE_MWCA;
        public string REGISTRATION_TYPE_MWIA = RegistrationConstant.REGISTRATION_TYPE_MWIA;
    }
}