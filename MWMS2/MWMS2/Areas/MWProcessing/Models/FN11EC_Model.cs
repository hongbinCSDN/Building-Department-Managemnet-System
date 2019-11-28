using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Models;
using MWMS2.Services;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static MWMS2.Utility.ValidationUtil;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn11EC_Model : DisplayGrid, IValidatableObject
    {
        public string RegNo { get; set; }
        public string PbpPrcAs { get; set; }
        public List<string> ItemCodes { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //throw new NotImplementedException();
            yield return null;
        }

        public string ChiCompName { get; set; }
        public string EngCompName { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? RemovalDate { get; set; }

        public List<V_CRM_INFO> V_CRM_INFOs { get; set; } = new List<V_CRM_INFO>();

        public V_CRM_INFO V_CRM_INFO { get; set; }

        public List<SelectListItem> PbpPrcAsList { get; set; } = new List<SelectListItem>();

        public bool IsCompany
        {
            get { return !string.IsNullOrEmpty(RegNo) && (RegNo.Contains("MWC ") || RegNo.Contains("GBC ")); }
        }

        public bool IsMwcw
        {
            get { return !string.IsNullOrEmpty(RegNo) && RegNo.Contains("MWC(W)"); }
        }

        public bool IsPopup { get; set; }

        public string ValidItem { get; set; }

        public string InvalidItem { get; set; }
    }
}