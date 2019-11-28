using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Utility;

namespace MWMS2.Areas.Admin.Models
{
    public class SMMUpdateJobAssignSearchModel : DisplayGrid
    {
        //displayModel
        public SYS_POST SYS_POST { get; set; }
        public string UUID { get; set; }
        public string RefNo { get; set; }
        public string Type { get; set; }
        public string Task { get; set; }
        public string User { get; set; }
        public string NewHandler { get; set; }
        public IEnumerable<SelectListItem> NewHandlerList { get; set; } = SystemListUtil.RetrieveNewHandler();

        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }

        //SearchModel
        public string SubmissionNo { get; set; }
        public bool WildcardSearch { get; set; }

        public string FileRef { get; set; }
        public string RecordType { get; set; }
        public string CurrentHandleUser { get; set; }
        public string NewHandleUser { get; set; }

        public string wftuUuid { get; set; }

        //public string CODE { get; set; }
        //public string RegType { get; set; }
        //public IEnumerable<SelectListItem> RegTypeList { set; get; } = SystemListUtil.RetrieveRegType();
        //public string EngDesc { get; set; }
        //public string ChiDesc { get; set; }
        //public Nullable<decimal> Ord { get; set; }
        //public bool IsActive { get; set; }

    }
}