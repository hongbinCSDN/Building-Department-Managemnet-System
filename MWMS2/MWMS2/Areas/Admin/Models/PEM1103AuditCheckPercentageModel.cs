using MWMS2.Entity;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Admin.Models
{
    public class PEM1103AuditCheckPercentageModel
    {
        public IEnumerable<SelectListItem> YearList
        {
            get { return SystemListUtil.RetrieveYearList(); }
        }
        public P_S_AUDIT_CHECK_PERCENTAGE AuditCheckPercentage { get; set; }

    }
}