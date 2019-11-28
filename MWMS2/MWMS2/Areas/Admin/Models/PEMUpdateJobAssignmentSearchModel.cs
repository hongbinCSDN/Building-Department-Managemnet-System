using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Admin.Models
{
    public class PEMUpdateJobAssignmentSearchModel : DisplayGrid
    {
        // display model
        public SYS_POST SYS_POST { get; set; }
        public string UUID { get; set; }
        public string RefNo { get; set; }
        public string Type { get; set; }
        public string Task { get; set; }
        public string User { get; set; }
        public string NewHandler { get; set; }
        public IEnumerable<SelectListItem> NewHandlerList { get; set; } = SystemListUtil.RetrieveNewHandlerPEM();

        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }

        // search model
        public string SubmissionNo { get; set; }
        public bool WildcardSearch { get; set; }

        public string FileRef { get; set; }
        public string RecordType { get; set; }
        public string CurrentHandleUser { get; set; }
        public string NewHandleUser { get; set; }

        public string wftuUuid { get; set; }
    }
}