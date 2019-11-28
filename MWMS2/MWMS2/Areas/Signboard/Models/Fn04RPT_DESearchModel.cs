using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Signboard.Models
{
    public class Fn04RPT_DESearchModel : DisplayGrid
    {
        public string SearchFileRefNo { get; set; }
        public string SearchBatchNumber { get; set; }
        public string HandlingOfficer { get; set; }
        public string SearchFormCode { get; set; }
        public string SearchStatus { get; set; }
        public string EndorsedBy { get; set; }
        public DateTime? ExpDateFrom { get; set; }
        public DateTime? ExpDateTo { get; set; }
        public string SubNo { get; set; }
        public IEnumerable<SelectListItem> HandlingOfficerList
        {
            get { return SystemListUtil.GetHandlingOfficerList(); }
        }
        public IEnumerable<SelectListItem> SearchFormCodeList
        {
            get { return SystemListUtil.GetSearchFormCodeList(); }
        }
        public IEnumerable<SelectListItem> SearchStatusList
        {
            get { return SystemListUtil.GetSearchStatusList(); }
        }
        public IEnumerable<SelectListItem> EndorsedByList
        {
            get { return SystemListUtil.GetEndorsedByList(); }
        }
        
    }
}