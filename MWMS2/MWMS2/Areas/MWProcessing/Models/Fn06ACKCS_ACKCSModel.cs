using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Models;
using MWMS2.Utility;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn06ACKCS_ACKCSModel : DisplayGrid
    {
        public string MWNo { get; set; }
        public string MWClass { get; set; }
        public string SubmissionDateFrom { get; set; }
        public string SubmissionDateTo { get; set; }
        public string SubmissionType { get; set; }
        public string Status { get; set; }

        public List<SelectListItem> Statuses
        {
            get
            {
                return SystemListUtil.GetACKNStatus();
            }
        }

        public List<SelectListItem> MWClasses
        {
            get
            {
                return SystemListUtil.GetACKNClasses();
            }
        }

        public List<SelectListItem> SubmissionTypes
        {
            get
            {
                return SystemListUtil.GetACKNSubmissionType();
            }
        }
    }
}