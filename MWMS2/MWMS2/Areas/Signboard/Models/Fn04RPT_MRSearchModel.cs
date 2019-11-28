using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Models
{
    public class Fn04RPT_MRSearchModel : DisplayGrid
    {
        public string TypeOfCalendar { get; set; }

        public List<DateTime> getMonthList { get; set; }
        public DateTime PeriodDateFrom {get;set;}
        public DateTime PeriodDateTo {get;set;}
        public string Month { get; set; }
        public string Year { get; set; }
        public IEnumerable<SelectListItem> MonthList
        {
            get { return SystemListUtil.GetMonthList(); }
        }
        public IEnumerable<SelectListItem> YearList
        {
            get {
                return SystemListUtil.GetSMMYearList();
            }
        }
        public List<string> ColHeaderList { get; set; } = new List<string>();
        public List<string> ReceivedRecordCountList { get; set; } = new List<string>();
        public List<string> ProcessingRecordCountList { get; set; } = new List<string>();
        public List<string> AcknowledgedRecordCountList { get; set; } = new List<string>();
        public List<string> RefusedRecordCountList { get; set; } = new List<string>();
        public List<string> ConditionalRecordCountList { get; set; } = new List<string>();
        public List<string> AuditRecordCountList { get; set; } = new List<string>();

        public List<string> ReceivedRecordCountList01C { get; set; } = new List<string>();
        public List<string> ProcessingRecordCountList01C { get; set; } = new List<string>();
        public List<string> AcknowledgedRecordCountList01C { get; set; } = new List<string>();
        public List<string> RefusedRecordCountList01C { get; set; } = new List<string>();
        public List<string> ConditionalRecordCountList01C { get; set; } = new List<string>();
        public List<string> AuditRecordCountList01C { get; set; } = new List<string>();

        public List<string> ReceivedRecordCountListSC02 { get; set; } = new List<string>();
        public List<string> ProcessingRecordCountListSC02 { get; set; } = new List<string>();
        public List<string> AcknowledgedRecordCountListSC02 { get; set; } = new List<string>();
        public List<string> RefusedRecordCountListSC02 { get; set; } = new List<string>();
        public List<string> ConditionalRecordCountListSC02 { get; set; } = new List<string>();
        public List<string> AuditRecordCountListSC02 { get; set; } = new List<string>();

        public List<string> ReceivedRecordCountListSC02C { get; set; } = new List<string>();
        public List<string> ProcessingRecordCountListSC02C { get; set; } = new List<string>();
        public List<string> AcknowledgedRecordCountListSC02C { get; set; } = new List<string>();
        public List<string> RefusedRecordCountListSC02C { get; set; } = new List<string>();
        public List<string> ConditionalRecordCountListSC02C { get; set; } = new List<string>();
        public List<string> AuditRecordCountListSC02C { get; set; } = new List<string>();



        public List<string> ReceivedRecordCountListSC03 { get; set; } = new List<string>();
        public List<string> ProcessingRecordCountListSC03 { get; set; } = new List<string>();
        public List<string> AcknowledgedRecordCountListSC03 { get; set; } = new List<string>();
        public List<string> RefusedRecordCountListSC03 { get; set; } = new List<string>();
        public List<string> ConditionalRecordCountListSC03 { get; set; } = new List<string>();
        public List<string> AuditRecordCountListSC03 { get; set; } = new List<string>();
        public List<string> GcList { get; set; } = new List<string>();
        public List<string> S24List { get; set; } = new List<string>();
        public List<string> S24IssList { get; set; } = new List<string>();
        public List<string> S24ProList { get; set; } = new List<string>();
        public List<string> S24GcaList { get; set; } = new List<string>();
        public List<string> S24ComList { get; set; } = new List<string>();
        public List<string> S24NotYetIssList { get; set; } = new List<string>();
        public List<string> S24NotYetExpList { get; set; } = new List<string>();
        public List<string> S24AppealLodgedList { get; set; } = new List<string>();
        public List<string> S24WithdrawnList { get; set; } = new List<string>();
        public List<string> S24ProsecutionToBeList { get; set; } = new List<string>();
        public List<string> S24PendingList { get; set; } = new List<string>();
        public List<string> ReportToScuList { get; set; } = new List<string>();
        public List<string> ReportToScuReceivedList { get; set; } = new List<string>();
        public List<string> ReportToScuCloseList { get; set; } = new List<string>();
        public List<string> EnquiryToScuList { get; set; } = new List<string>();
        public List<string> EnquiryToScuReceivedList { get; set; } = new List<string>();
        public List<string> EnquiryToScuCloseList { get; set; } = new List<string>();
    }
}