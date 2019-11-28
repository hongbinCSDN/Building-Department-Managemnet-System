using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Signboard.Models
{
    public class Fn04RPT_MSSearchModel : DisplayGrid
    {
        public DateTime PeriodDateFrom { get; set; }
        public DateTime PeriodDateTo { get; set; }
        public List<DateTime> MonthList { get; set; }
        public List<string> ClassIAcknowledgedList { get; set; } = new List<string>();
        public List<string> ClassIRefusedList { get; set; } = new List<string>();
        public List<string> ClassIIAcknowledgedList { get; set; } = new List<string>();
        public List<string> ClassIIRefusedList { get; set; } = new List<string>();
        public List<string> ClassIIIAcknowledgedList { get; set; } = new List<string>();
        public List<string> ClassIIIRefusedList { get; set; } = new List<string>();
        public List<string> ClassIConditionalList { get; set; } = new List<string>();
        public List<string> ClassIIConditionalList { get; set; } = new List<string>();
        public List<string> ClassIIIConditionalList { get; set; } = new List<string>();
        public List<string> ClassIAuditList { get; set; } = new List<string>();
        public List<string> ClassIIAuditList { get; set; } = new List<string>();
        public List<string> ClassIIIAuditList { get; set; } = new List<string>();
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