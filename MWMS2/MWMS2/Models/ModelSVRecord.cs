using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Models
{
    public class ModelSVRecord
    {
        public string UUID { get; set; }
        public string SubmissionUUID { get; set; }
        public string SubmissionNo { get; set; }
        public string FormLanguage { get; set; }
        public string ReceivedDate { get; set; }
        public string FormCode { get; set; }
        public string BatchNumber { get; set; }

        //public List<String> AutoCompletStreetList { get; set; }

        public string SVInfoLocationOfSignboard { get; set; }
        public string SVInfoStreetRoadVillageName { get; set; }
        public string SVInfoStreetNumber { get; set; }
        public string SVInfoBuildingEstate { get; set; }
        public string SVInfoFlat { get; set; }
        public string SVInfoFloor { get; set; }
        public string SVInfoBlock { get; set; }
        public string SVInfoDistrict { get; set; }

        public List<Region> Regions { get; set; }
        [Range(1, 999, ErrorMessage = "Please choose a Region.")]
        public string SVInfoSelectedRegion { get; set; }

        public string SVInfoRVD_No { get; set; }
        public string SVInofRVDBlockID { get; set; }

        public string SVInfoBCISBlockID { get; set; }
        public string SVInfoBCISDistrict { get; set; }
        public string SVInfoBCIS4plus2 { get; set; }
        public string SVInfoS24OrderType    { get; set; }
        public string SVInfoS24OrderNo{ get; set; }


        public List<ValidationItem> ValidationItems { get; set; }
        public List<string> SelectedValidationItems { get; set; }

        public List<string> SelectedValidationItemsCorr { get; set; }
        public List<string> SelectedValidationItemsDescription { get; set; }

        public List<MWItem> MWItems { get; set; }
        public List<string> SelectedMWItems { get; set; }
        public List<string> SelectedMWItemsDescription { get; set; }
        public List<string> SelectedMWItemsRefNo { get; set; }
   

        public string SVOwnerChineseName { get; set; }
        public string SVOwnerEnglishName { get; set; }
        public string SVOwnerIdNumber { get; set; }
        public string SVOwnerIdType { get; set; }
        public string SVOwnerCountryOfIssue { get; set; }
        public string SVOwnerAddressflat { get; set; }
        public string SVOwnerAddressfloor { get; set; }
        public string SVOwnerAddressblock { get; set; }
        public string SVOwnerAddressbuildingnameEst { get; set; }
        public string SVOwnerAddressStreetRoadVillage { get; set; }
        public string SVOwnerAddressStreetNo { get; set; }
        public string SVOwnerAddressDistrict { get; set; }

       // public List<Region> SVOwnerRegions { get; set; }
        [Range(1, 999, ErrorMessage = "Please choose a Region.")]
        public string SVOwnerSelectedRegion { get; set; }

        public string SVOwnerEmailAddress { get; set; }
        public string SVOwnerContactNo { get; set; }
        public string SVOwnerFaxNo { get; set; }

        
        public string SVOwnerCorpChineseName { get; set; }
        public string SVOwnerCorpEnglishName { get; set; }
        public string SVOwnerCorpIdNumber { get; set; }
        public string SVOwnerCorpIdType { get; set; }
        public string SVOwnerCorpCountryOfIssue { get; set; }
        public string SVOwnerCorpAddressflat { get; set; }
        public string SVOwnerCorpAddressfloor { get; set; }
        public string SVOwnerCorpAddressblock { get; set; }
        public string SVOwnerCorpAddressbuildingnameEst { get; set; }
        public string SVOwnerCorpAddressStreetRoadVillage { get; set; }
        public string SVOwnerCorpAddressStreetNo { get; set; }
        public string SVOwnerCorpAddressDistrict { get; set; }

        [Range(1, 999, ErrorMessage = "Please choose a Region.")]
        public string SVOwnerCorpSelectedRegion { get; set; }

        public string SVOwnerCorpEmailAddress { get; set; }
        public string SVOwnerCorpContactNo { get; set; }
        public string SVOwnerCorpFaxNo { get; set; }

        public string SVOwnerCorpPRCAppointedName { get; set; }
        public string SVOwnerCorpPRCAppointedContactNo { get; set; }
        public string SVOwnerCorpPBPAppointedName { get; set; }
        public string SVOwnerCorpPBPAppointedContactNo { get; set; }

        public string SVPAWChineseName { get; set; }
        public string SVPAWEnglishName { get; set; }
        public string SVPAWIdNumber { get; set; }
        public string SVPAWIdType { get; set; }
        public string SVPAWCountryOfIssue { get; set; }
        public string SVPAWAddressflat { get; set; }
        public string SVPAWAddressfloor { get; set; }
        public string SVPAWAddressblock { get; set; }
        public string SVPAWAddressbuildingnameEst { get; set; }
        public string SVPAWAddressStreetRoadVillage { get; set; }
        public string SVPAWAddressStreetNo { get; set; }
        public string SVPAWAddressDistrict { get; set; }

        // public List<Region> SVOwnerRegions { get; set; }
        [Range(1, 999, ErrorMessage = "Please choose a Region.")]
        public string SVPAWSelectedRegion { get; set; }

        public string SVPAWEmailAddress { get; set; }
        public string SVPAWContactNo { get; set; }
        public string SVPAWFaxNo { get; set; }
        public string SVPAWSAMEAS { get; set; }


        public string WSInspectionDate { get; set; }
        public string WSValidationExpiryDate { get; set; }
        public string WSDiscoryDateForSVRemoval { get; set; }



        public string APCertRegNo { get; set; }
        public string APChineseName { get; set; }
        public string APEnglishName { get; set; }
        public string APContactNo { get; set; }
        public string APFaxNo { get; set; }
        public string APExpiryDate { get; set; }
        public string APSignDate{ get; set; }

        public string RSECertRegNo { get; set; }
        public string RSEChineseName { get; set; }
        public string RSEEnglishName { get; set; }
        public string RSEContactNo { get; set; }
        public string RSEFaxNo { get; set; }
        public string RSEExpiryDate { get; set; }
        public string RSESignDate { get; set; }

        //SCO2,SC02C, SC03 form spec
        public string RICertRegNo { get; set; }
        public string RIChineseName { get; set; }
        public string RIEnglishName { get; set; }
        public string RIContactNo { get; set; }
        public string RIFaxNo { get; set; }
        public string RIExpiryDate { get; set; }
        public string RISingDate { get; set; }


        public string PRCCertRegNo { get; set; }
        public string PRCChineseName { get; set; }
        public string PRCEnglishName { get; set; }
        public string PRCContactNo { get; set; }
        public string PRCFaxNo { get; set; }
        public string PRCASChineseName { get; set; }
        public string PRCASEnglishName { get; set; }
        public string PRCExpiryDate { get; set; }
        public string PRCSignDate { get; set; }

        //
        //Data Entry - Validation Information

        public string VIValidityAP { get; set; }
        public string VISignatureAP { get; set; }
        public string VICapacityAS { get; set;  }
        public string VIValidityPRC { get; set; }
        public string VISignatureAS { get; set; }
        public string VIInfoSOProvided { get; set; }
        public string VIOtherIRRMarked { get; set; }
        public string VIRecommendation { get; set; }

        public string GALTOHandlingOffice { get; set; }
        public string GALIOAddress { get; set; }
        public string GALLastAckLetterIssuedDate { get; set; }
        public string GALLetterType { get; set; }
        public string GALLetter { get; set; }
    }
    public class Region
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
    public class ValidationItem
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class MWItem
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

}