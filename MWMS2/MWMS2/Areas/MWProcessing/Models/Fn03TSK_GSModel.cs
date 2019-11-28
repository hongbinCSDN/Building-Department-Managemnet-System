using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Models;
using MWMS2.Utility;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn03TSK_GSModel:DisplayGrid
    {
        public string SubmissionType { get; set; }
        public string RefNo { get; set; }
        public string Status { get; set; }
        public string ReceiveDate { get; set; }
        public string Source { get; set; }
        public string ReferralDate { get; set; }
        public string Name { get; set; }
        public string NameType { get; set; }
        public string Progress { get; set; }
        public string Keyword { get; set; }

        public List<SelectListItem> SubmissionTypes
        {
            get
            {
                return SystemListUtil.GetTSKGSSubmissionType();
            }
        }

        public List<SelectListItem> Statuses
        {
            get
            {
                return SystemListUtil.GetTSKGSStatuses();
            }
        }

        public List<SelectListItem> Sources
        {
            get
            {
                return SystemListUtil.GetTSKGSSource();
            }
        }

        public List<SelectListItem> Progresses
        {
            get
            {
                return SystemListUtil.GetTSKProgress();
            }
        }
    }
}