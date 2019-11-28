using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Utility;

namespace MWMS2.Areas.Signboard.Models
{
    public class SvValidationDisplayModel
    {
        public string SignboardRefNo { get; set; }
        public string FormCode { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string ValidationResult { get; set; }
    }
}