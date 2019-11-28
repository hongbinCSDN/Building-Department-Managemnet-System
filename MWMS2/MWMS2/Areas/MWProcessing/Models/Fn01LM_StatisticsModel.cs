using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn01LM_StatisticsModel : DisplayGrid
    {
        
        public string Month { get; set; }
        public string DisplayMonth
        {
            get
            {
                return SystemListUtil.MonthEnglishName(Month);
            }
        }
        public string Year { get; set; }
        public IEnumerable<SelectListItem> MonthList
        {
            get
            {
                return (new List<SelectListItem>())
                    .Concat(SystemListUtil.getMonth()
                        .Select(o => new SelectListItem()
                        {
                            Text = SystemListUtil.MonthEnglishName(o.ToString()),
                            Value = o.ToString()
                        }
                        ));

            }
        }
        public IEnumerable<SelectListItem> YearList
        {
            get { return SystemListUtil.RetrieveYearList(); }
        }
        private DateTime _currentTime = DateTime.MinValue;
        public DateTime CurrentTime
        {
            get
            {
                return (_currentTime == DateTime.MinValue) ? DateTime.Now : _currentTime;
            }
            set { _currentTime = value; }
        }
        public int AccumulatedTotalSubmission { get; set; }
        public string View { get; set; }
        public List<P_S_SYSTEM_VALUE> SubmissionList { get; set; }
        public SearchSubmissionReceivedModel SSRModel { get; set; }
        public List<Fn01LM_StatisticsIncomingListModel> IncomingModel { get; set; }
        public List<Fn01LM_StatisticsOutgoingListModel> OutgoingModel { get; set; }
        public List<Fn01LM_StatisticsIncomingModel> IncomingWeeklySummaryModel { get; set; }
        public List<Fn01LM_StatisticsOutgoingModel> OutgoingWeeklySummaryModel { get; set; }
        public List<TypeReceivedTableModel> TypeReceivedTableModel { get; set; }
        public List<Fn01LM_StatisticsSDMValidationSchemeModel> SDMValidationSchemeModel { get; set; }
        public Fn01LM_StatisticsSDMParticularItemModel SDMParticularItemModel { get; set; }
        public double TotalSDMParticularItem { get; set; }
        public List<Fn01LM_StatisticsRectificationNote3Model> RectificationNote3Model { get; set; }
        public int TotalCountOfSubmission { get; set; }
        public int TotalCountOfAuditSelected { get; set; }
        public string PercentageOfSubmissionSelected { get; set; }
        public int ReceivedCount { get; set; }
        public int AcknowledgedOverCounter { get; set; }
        public int AcknowledgedByFax { get; set; }
        public int ReturnedByFaxNote { get; set; }
        public int ReturnedOverCounter { get; set; }
        public string ReceivedDateFrom { get; set; }
        public string ReceivedDateTo { get; set; }
        public int WeeklyAverage { get; set; }
        public int ReceivedWeekly { get; set; }
        public int AcknowledgedOverCounterWeekly { get; set; }
        public int AcknowledgedByFaxWeekly { get; set; }
        public int ReturnedByFaxNoteWeekly { get; set; }
        public int ReturnedOverCounterWeekly { get; set; }
        public int C112Value { get; set; }
        public double C112Percent { get; set; }
        public double C113Class1 { get; set; }
        public double C113Class2 { get; set; }
        public double C113Class3 { get; set; }
        public int KtBarCode { get; set; }
        public int PcBarCode { get; set; }
        public int EcBarCode { get; set; }
        public int WKGOBarCode { get; set; }
        public int KtNonBarCode { get; set; }
        public int PcNonBarCode { get; set; }
        public int EcNonBarCode { get; set; }
        public int WKGONonBarCode { get; set; }
        public string Received_Date { get; set; }
        public string ExportReportType { get; set; }
        public string ExportType { get; set; }
    }

    public class SearchSubmissionReceivedModel
    {
        public string ReceivedDateFrom { get; set; }
        public string ReceivedDateTo { get; set; }
        public string LetterDateFrom { get; set; }
        public string LetterDateTo { get; set; }
        public string ReferralDateFrom { get; set; }
        public string ReferralDateTo { get; set; }
        public List<string> NatureList { get; set; }
        public List<string> FormTypeList { get; set; }
        public bool OrderRelatedOnly { get; set; }
        public int LetterDateAndReceivedDateLogic { get; set; }
        public bool VsOnly { get; set; }
    }

    public class Fn01LM_StatisticsIncomingListModel
    {
        public List<Fn01LM_StatisticsIncomingModel> IncomingModelList { get; set; }
    }
    public class Fn01LM_StatisticsOutgoingListModel
    {
        public List<Fn01LM_StatisticsOutgoingModel> OutgoingModelList { get; set; }

    }

    public class  Fn01LM_StatisticsIncomingModel
    {
        public DateTime RECEIVED_DATE { get; set; }
        public int WEEK_DAY { get; set; }
        public int DAY { get; set; }
        public int PC_REC { get; set; }
        public int PC_OD { get; set; }
        public int PC_OS { get; set; }
        public int KT_REC { get; set; }
        public int KT_OD { get; set; }
        public int KT_OS { get; set; }
        public int WKGO_PEC { get; set; }
        public int WKGO_OD { get; set; }
        public int WKGO_OS { get; set; }
        public int ES_REC { get; set; }
        public int ES_OD { get; set; }
        public int ES_OS { get; set; }
        public int TTL_REC { get; set; }
        public int TTL_OD { get; set; }
        public int TTL_OS { get; set; }
        public int DL_REC { get; set; }
        public int DL_OD { get; set; }
        public int DL_OS { get; set; }
        public int OR_REC { get; set; }
        public int OR_OD { get; set; }
        public int OR_OS { get; set; }
        public int AUDIT { get; set; }
        public int ICU { get; set; }
        public int CR { get; set; }
        public string IncomingWeeklySummaryDateRange { get; set; }
    }

    public class GetIncomingOutgoingParameterModel
    {
        public string YearMonth { get; set; }
        public string PCCounter { get; set; }
        public string KTCounter { get; set; }
        public string ECounter { get; set; }
        public string WKGOCounter { get; set; }
        public string RecNatureList { get; set; }
        public string DirectandReviseList { get; set; }
        public string CRNatureList { get; set; }
        public string ICUNatureList { get; set; }
        public string WKGNatureList { get; set; }
        public string LetterDateFrom { get; set; }
        public string LetterDateTo { get; set; }
        public string YearStartDate { get; set; }
        public string MonthYear { get; set; }

    }
    public class TypeReceivedTableModel
    {
        public string FORM_NO { get; set; }
        public int TOTAL_COUNT { get; set; }
        public int AUDIT_COUNT { get; set; }
    }

    public class Fn01LM_StatisticsOutgoingModel
    {
        public DateTime DATERANGE { get; set; }
        public int WEEK_DAY { get; set; }
        public int DAY { get; set; }
        public int PC_COUNTER { get; set; }
        public int KT_COUNTER { get; set; }
        public int WKGO_COUNTER { get; set; }
        public int ES_COUNTER { get; set; }
        public int D_LET { get; set; }
        public int O_REL { get; set; }
        public int ICU { get; set; }
        public int CR { get; set; }
        public string WeeklySummaryDateRange { get; set; }
    }

    public class Fn01LM_StatisticsSDMValidationSchemeModel
    {
        public string Period { get; set; }
        public int NoOfStructiresValidated { get; set; }
    }

    public class Fn01LM_StatisticsSDMParticularItemModel
    {
        public double WindowsSubmission { get; set; }
        public double RenderingSubmission { get; set; }
        public double RepairSubmission { get; set; }
        public double AbovegroudDrainageSubmission { get; set; }
        public double AcSupportingFrameSubmission { get; set; }
        public double DryingRackSubmission { get; set; }
        public double CanopySubmission { get; set; }
        public double SdfSubmission { get; set; }
        public double SignboardRelatedSubmission { get; set; }
    }

    public class Fn01LM_StatisticsRectificationNote3Model
    {
        public string PeriodYear { get; set; }
        public int Count { get; set; }
    }

}