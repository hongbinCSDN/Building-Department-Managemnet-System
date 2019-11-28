using MWMS2.Constant;
using MWMS2.Models;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Models
{
    public class Fn06AS_DBSearchModel : DisplayGrid
    {
        public string FileRef { get; set; }
        public string RegNo { get; set; }
        public string HKID { get; set; }
        public string PassportNo { get; set; }
        public string HKID_PASSPORT_DISPLAY { get { return string.IsNullOrWhiteSpace(HKID) || string.IsNullOrWhiteSpace(PassportNo) ? HKID + PassportNo : HKID + "/ " + PassportNo; } }
        public string SurName { get; set; }
        public string GivenName { get; set; }
        public string ChiName { get; set; }
        public string Status { get; set; }
        public IEnumerable<SelectListItem> StatusList
        {
            get { return SystemListUtil.GetStatusList(); }
        }
        public bool CAT_GBC { get; set; }
        public bool CAT_SC_D { get; set; }
        public bool CAT_SC_F { get; set; }
        public bool CAT_SC_GI { get; set; }
        public bool CAT_SC_SF { get; set; }
        public bool CAT_SC_V { get; set; }
        public bool CAT_MWC_CLASS_I_II_III { get; set; }
        public bool CAT_MWC_CLASS_II_III { get; set; }
        public bool CAT_MWC_CLASS_III { get; set; }
        public bool ConsetToPublish { get; set; }
        public bool RefusedToPublish { get; set; }
        public bool NotIndicated { get; set; }
    }
}