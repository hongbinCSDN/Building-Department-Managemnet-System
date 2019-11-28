using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Models;
using MWMS2.Utility;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn04VRF_VRFModel : DisplayGrid
    {
        public string RefNo { get; set; }
        public string SubmissionDateFrom { get; set; }
        public string SubmissionDateTo { get; set; }
        public string FormNo { get; set; }
        public string Status { get; set; }
        public string SubmissionType { get; set; }
        public string MWItem { get; set; }
        public List<SelectListItem> FormNos
        {
            get
            {
                return SystemListUtil.GetValidMWFormNos();
            }
        }
        public List<SelectListItem> Statuses
        {
            get
            {
                return SystemListUtil.GetVERTStatus();
            }
        }

        public List<SelectListItem> SubmissionTypes
        {
            get
            {
                return SystemListUtil.GetVERTSubmissionType();
            }
        }
    }
}