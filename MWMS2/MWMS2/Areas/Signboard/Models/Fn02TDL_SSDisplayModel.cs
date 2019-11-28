using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Services.Signborad.SignboardDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Signboard.Models
{
    public class Fn02TDL_SSDisplayModel
    {
        public bool ShowPrivacy { get; set; }
        public List<SvValidationDisplayModel> ValidationList { get; set; }
        public string SubmissionNo { get; set; }
        public string Description { get; set; }
        public string Facade { get; set; }
        public string Type { get; set; }
        public string BtmFloor { get; set; }
        public string TopFloor { get; set; }
        public string Area { get; set; }
        public string Projection { get; set; }
        public string Height { get; set; }
        public string Clearance { get; set; }
        public string Led { get; set; }
        public string BuildingPortion { get; set; }
        public string Thumbnail { get; set; }
        public string SignboardFlat { get; set; }
        public string SignboardFloor { get; set; }
        public string SignboardBlock { get; set; }
        public string SignboardBuilding { get; set; }
        public string SignboardStreetNo { get; set; }
        public string SignboardStreet { get; set; }
        public string SignboardDistrict { get; set; }
        public string SignboardRegion { get; set; }
        public string SignboardRvdNo { get; set; }
        public string PawChiName { get; set; }
        public string PawEngName { get; set; }
        public string PawIdNo { get; set; }
        public string PawIdType { get; set; }
        public string PawIdIssueCountry { get; set; }
        public string PawFlat { get; set; }
        public string PawFloor { get; set; }
        public string PawBlock { get; set; }
        public string PawBuilding { get; set; }
        public string PawStreetNo { get; set; }
        public string PawStreet { get; set; }
        public string PawDistrict { get; set; }
        public string PawRegion { get; set; }
        public string PawEmail { get; set; }
        public string PawContact { get; set; }
        public string PawFax { get; set; }
        public string OwnerChiName { get; set; }
        public string OwnerEngName { get; set; }
        public string OwnerIdNo { get; set; }
        public string OwnerIdType { get; set; }
        public string OwnerIdIssueCountry { get; set; }
        public string OwnerFlat { get; set; }
        public string OwnerFloor { get; set; }
        public string OwnerBlock { get; set; }
        public string OwnerBuilding { get; set; }
        public string OwnerStreetNo { get; set; }
        public string OwnerStreet { get; set; }
        public string OwnerDistrict { get; set; }
        public string OwnerRegion { get; set; }
        public string OwnerEmail { get; set; }
        public string OwnerContact { get; set; }
        public string OwnerFax { get; set; }
        public string OiChiName { get; set; }
        public string OiEngName { get; set; }
        public string OiIdNo { get; set; }
        public string OiIdType { get; set; }
        public string OiIdIssueCountry { get; set; }
        public string OiFlat { get; set; }
        public string OiFloor { get; set; }
        public string OiBlock { get; set; }
        public string OiBuilding { get; set; }
        public string OiStreetNo { get; set; }
        public string OiStreet { get; set; }
        public string OiDistrict { get; set; }
        public string OiRegion { get; set; }
        public string OiEmail { get; set; }
        public string OiContact { get; set; }
        public string OiFax { get; set; }
        public string OiPrcName { get; set; }
        public string OiPrcContact { get; set; }
        public string OiPbpName { get; set; }
        public string OiPbpContact { get; set; }
        public List<B_SV_SCANNED_DOCUMENT> DocList { get; set; }
        public List<B_SV_PHOTO_LIBRARY> PhotoList { get; set; }
        public List<string> SignboardRelationList  { get; set; }
        
        // Optional
        public List<B_SV_24_ORDER> SvOrderList  { get; set; }
        public List<B_SV_GC> SvGcList { get; set; }
        public List<B_SV_COMPLAIN> SvComplainList { get; set; }

        public string Status { get; internal set; }

        public string SIGNBOARD_THUMBNAIL_WIDTH = SignboardConstant.SIGNBOARD_THUMBNAIL_WIDTH;
    }
}