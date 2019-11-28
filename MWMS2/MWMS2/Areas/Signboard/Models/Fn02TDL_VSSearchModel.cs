using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MWMS2.Utility;
using MWMS2.Constant;
using MWMS2.Entity;

namespace MWMS2.Areas.Signboard.Models
{
    public class Fn02TDL_VSSearchModel : DisplayGrid
    {
        public string RefNo { get; set; }
        public string Status { get; set; }
        public string Class { get; set; }
        public DateTime? ReceivedDateFrom { get; set; }
        public DateTime? ReceivedDateTo { get; set; }
        public string ItemNo { get; set; }
        public string RelatedOrderNo { get; set; }
        public string Alternation { get; set; }
        public string OrderNo { get; set; }
        public string Recommend { get; set; }
        public string Street { get; set; }
        public string StreetNo { get; set; }
        public string Building { get; set; }
        public string District { get; set; }
        public string Region { get; set; }
        public string Paw { get; set; }
        public string RelatedParty { get; set; }
        public string EnglishName { get; set; }
        public string ChineseName { get; set; }
        public string CertOfRegNo { get; set; }
        public string HandlingOfficer { get; set; }
        public DateTime? InspectDateFrom { get; set; }
        public DateTime? InspectDateTo { get; set; }
        public DateTime? CompletionDateFrom { get; set; }
        public DateTime? CompletionDateTo { get; set; }
        public DateTime? ValidationExpiryDateFrom { get; set; }
        public DateTime? ValidationExpiryDateTo { get; set; }
        public DateTime? ExpiryDateFrom { get; set; }
        public DateTime? ExpiryDateTo { get; set; }
        public bool SignboardRemoved { get; set; }
        public List<SelectListItem> StatusList { get; set; } = SystemListUtil.GetStatusList(1);
        public List<SelectListItem> ClassList { get; set; } = SystemListUtil.GetSystemValueBySystemType("Class", 3);
        public List<SelectListItem> ItemNoList { get; set; } = SystemListUtil.GetSystemValueBySystemType("Item No", 5);
        public List<SelectListItem> AlternationList { get; set; } = SystemListUtil.GetPoolingList();
        public List<SelectListItem> RecommendationList { get; set; } = SystemListUtil.GetSystemValueBySystemType("LetterResult", 2);
        public List<SelectListItem> RegionList { get; set; } = SystemListUtil.GetSystemValueBySystemType("Region", 2);
         public List<SelectListItem> HandlingOfficerList { get; set; } = SystemListUtil.GetSUserAccountList(SystemParameterConstant.SecurityLevel[0]);
        public List<SelectListItem> RelatedPartyList { get; set; } = SystemListUtil.GetRelatedPartyList();

        public List<B_SV_SCANNED_DOCUMENT> DocList { get; set; }

        public string SIGNBOARD_THUMBNAIL_WIDTH = SignboardConstant.SIGNBOARD_THUMBNAIL_WIDTH;
    }
}