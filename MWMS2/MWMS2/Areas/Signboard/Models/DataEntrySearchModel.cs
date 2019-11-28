using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Models
{
    public class DataEntrySearchModel : DisplayGrid
    {
        public B_SV_SUBMISSION svSubmission { get; set; }
        public List<B_S_SYSTEM_VALUE> SignboardTypeList { get; set; }
        public List<string> selectedDSNList { get;set; }
        public string selectedDSN { get; set; }
        public string BactchNum{ get; set; }
        public string RegType { get; set; }
        public string searchFileRefNo { set; get; }
        public DateTime? searchReceivedDateFrom { get; set; }
        public DateTime? searchReceivedDateTo { get; set; }
        public string Status { get; set; }
        public IEnumerable<SelectListItem> StatusList
        {
            get { return SystemListUtil.GetSMMStatusList(); }
        }
    }
}