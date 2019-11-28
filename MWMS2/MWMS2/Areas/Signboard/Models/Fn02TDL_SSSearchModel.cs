using MWMS2.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MWMS2.Utility;
using MWMS2.Constant;

namespace MWMS2.Areas.Signboard.Models
{
    public class Fn02TDL_SSSearchModel : DisplayGrid
    {
        public string RefNo { get; set; }
        public string MwNo { get; set; }
        public string FileRefNo { get; set; }
        public string ValidationNo { get; set; }
        public string RecordType { get; set; }
        public string RelatedOrderNo { get; set; }
        public DateTime? ExpiryDateFrom { get; set; }
        public DateTime? ExpiryDateTo { get; set; }
        public string Facade { get; set; }
        public string Type { get; set; }
        public string BotFixingAtFloor { get; set; }
        public string TopFixingAtFloor { get; set; }
        public string DisplayArea { get; set; }
        public string Projection { get; set; }
        public string HeightOfSb { get; set; }
        public string ClearanceAbvGround { get; set; }
        public string LedOrTv { get; set; }
        public string BlgPortion { get; set; }
        public string Street { get; set; }
        public string StreetNo { get; set; }
        public string Building { get; set; }
        public string District { get; set; }
        public string Region { get; set; }
        public string BcisAreaCode { get; set; }
        public string SbOwner { get; set; }
        public string Paw { get; set; }
        public bool[] SourceOfInfo { get; set; }
        public List<SelectListItem> RecordTypeList { get; set; } = SystemListUtil.GetRecordTypeList();
        public List<SelectListItem> LedOrTvList { get; set; } = SystemListUtil.GetLedOrTvList();
        public List<SelectListItem> RegionList { get; set; } = SystemListUtil.GetSystemValueBySystemType("Region", 2);
        public List<SelectListItem> BcisAreaCodeList { get; set; } = SystemListUtil.GetBcisAreaCodeList();
        public List<SelectListItem> SourceOfInfoList { get; set; } = SystemListUtil.GetSystemValueBySystemType("SourceOfInformation", 2);

        public string SIGNBOARD_THUMBNAIL_WIDTH = SignboardConstant.SIGNBOARD_THUMBNAIL_WIDTH;
    }
}