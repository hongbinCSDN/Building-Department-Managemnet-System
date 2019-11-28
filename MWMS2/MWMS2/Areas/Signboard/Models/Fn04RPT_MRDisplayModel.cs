using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Signboard.Models
{
    public class Fn04RPT_MRDisplayModel : DisplayGrid
    {
        public List<string> ColHeaderList { get; set; }
        public List<string> ReceivedRecordCountList { get; set; }
        public List<string> ProcessingRecordCountList { get; set; }
        public List<string> AcknowledgedRecordCountList { get; set; }
        public List<string> RefusedRecordCountList { get; set; }
        public List<string> ConditionalRecordCountList { get; set; }
        public List<string> AuditRecordCountList { get; set; }
        public List<string> ReceivedRecordCountList02 { get; set; }
        public List<string> ProcessingRecordCountList02 { get; set; }
        public List<string> AcknowledgedRecordCountList02 { get; set; }
        public List<string> RefusedRecordCountList02 { get; set; }
        public List<string> ConditionalRecordCountList02 { get; set; }
        public List<string> AuditRecordCountList02 { get; set; }
        public List<string> ReceivedRecordCountList03 { get; set; }
        public List<string> ProcessingRecordCountList03 { get; set; }
        public List<string> AcknowledgedRecordCountList03 { get; set; }
        public List<string> RefusedRecordCountList03 { get; set; }
        public List<string> ConditionalRecordCountList03 { get; set; }
        public List<string> AuditRecordCountList03 { get; set; }
        public List<string> GcList { get; set; }
        public List<string> S24List { get; set; }
        public List<string> S24IssList { get; set; }
        public List<string> S24ProList { get; set; }
        public List<string> S24GcaList { get; set; }
        public List<string> S24ComList { get; set; }
        public List<string> S24NotYetIssList { get; set; }
        public List<string> S24NotYetExpList { get; set; }
        public List<string> S24AppealLodgedList { get; set; }
        public List<string> S24WithdrawnList { get; set; }
        public List<string> S24ProsecutionToBeList { get; set; }
        public List<string> S24PendingList { get; set; }
        public List<string> ReportToScuList { get; set; }
        public List<string> ReportToScuReceivedList { get; set; }
        public List<string> ReportToScuCloseList { get; set; }
        public List<string> EnquiryToScuList { get; set; }
        public List<string> EnquiryToScuReceivedList { get; set; }
        public List<string> EnquiryToScuCloseList { get; set; }
    }
}