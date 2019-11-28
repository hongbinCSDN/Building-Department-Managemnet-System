using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWMS2.Entity;
using MWMS2.Models;

namespace MWMS2.Areas.MWProcessing.Models
{
    public class Fn03TSK_SSModel : DisplayGrid
    {
        public string AddressCarriedOut { get; set; }
        public DateTime? PemCompetionDate { get; set; }
        public string CorrespondenceAddress { get; set; }
        public string CorrespondenceAddressOfSignboard { get; set; }
        public string PreCommencementSiteAudit { get; set; }
        public string MWScannedDocReceivedDate { get; set; }
        public P_MW_RECORD P_MW_RECORD { get; set; }
        public P_MW_REFERENCE_NO P_MW_REFERENCE_NO { get; set; }
        public P_MW_PERSON_CONTACT OWNER { get; set; }
        public P_MW_PERSON_CONTACT SIGNBOARD { get; set; }
        public P_MW_PERSON_CONTACT OI { get; set; }
        public P_MW_ADDRESS OIAddress { get; set; }
        public List<P_MW_RECORD_ITEM> ItemList { get; set; }
        public List<P_MW_RECORD_ITEM_HISTORY> ItemHistoryList { get; set; }
        public MwViewFormPerson Prc { get; set; }
        public List<P_MW_APPOINTED_PROF_HISTORY> ApHistoryList { get; set; }
        public List<P_MW_APPOINTED_PROF_HISTORY> RseHistoryList { get; set; }
        public List<P_MW_APPOINTED_PROF_HISTORY> RgeHistoryList { get; set; }
        public List<P_MW_APPOINTED_PROF_HISTORY> PrcHistoryList { get; set; }
        public List<MwViewFormPerson> ProfessionalList { get; set; }

        //Start modify by dive 20191017
        //public List<DSNRecord> IssuedCorrespondenceList { get; set; }
        //public List<SubDSNRecord> DocList { get; set; }
        public List<P_MW_SCANNED_DOCUMENT> P_MW_SCANNED_DOCUMENTsIC { get; set; }
        public List<P_MW_SCANNED_DOCUMENT> P_MW_SCANNED_DOCUMENTsNIC { get; set; }
        //End modify by dive 20191017

        public List<P_MW_RECORD> ProcessingList { get; set; }
        public List<P_MW_RECORD_ITEM> FinalP_MW_RECORD_ITEMs { get; set; }
    }
    public class Fn03TSK_SSSearchModel : DisplayGrid
    {
        public string CountByDSN { get; set; }
        public string CountByMWNo { get; set; }
        public string DSN { get; set; }
        public string SubmissionNature { get; set; }
        public string PrefixRefNo { get; set; }
        public string RefNo { get; set; }
        public string StatusId { get; set; }
        public string AuditRelated { get; set; }
        public string FormNo { get; set; }
        public string SubmissionType { get; set; }
        public string SubmissionDateForm { get; set; }
        public string SubmissionDateTo { get; set; }
        public string CommenceDateFrom { get; set; }
        public string CommenceDateTo { get; set; }
        public string CompleteDateFrom { get; set; }
        public string CompleteDateTo { get; set; }
        public string LocationOfMW { get; set; }
        public string EfssRefNo { get; set; }
        public string FileRefNo { get; set; }
        public string FileRefFour { get; set; }
        public string FileRefTwo { get; set; }
        public string BlockId { get; set; }
        public string UnitId { get; set; }
        public string ClassCode { get; set; }
        public P_MW_APPOINTED_PROFESSIONAL AP { get; set; }
        public P_MW_APPOINTED_PROFESSIONAL PRC { get; set; }
        public P_MW_ADDRESS MWAddress { get; set; }
        public string ItemInfo { get; set; }
        public MwClassListModel Checkbox_ItemNo_TypeofMWs_Class1 { get; set; }
        public MwClassListModel Checkbox_ItemNo_TypeofMWs_Class2 { get; set; }
        public MwClassListModel Checkbox_ItemNo_TypeofMWs_Class3 { get; set; }
    }
    public class DSNRecord
    {
        public string DSN { get; set; }
        public string EncryptedDsn { get; set; }
        public string ModifiedDateString { get; set; }
        public string LetterType { get; set; }
        public string FormNo { get; set; }
        public string FolderType { get; set; }
        public string FilePath { get; set; }
    }

    public class SubDSNRecord
    {
        public string SubDsn { get; set; }
        public string DocType { get; set; }
        public string FormCode { get; set; }
        public string TotalPage { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string DocTitle { get; set; }
        public string FolderType { get; set; }
        public string EncryptedDsn { get; set; }
        public string Result { get; set; }
        public string ResultDate { get; set; }
        public string ReceivedDate { get; set; }
        public string RefNo { get; set; }

        public string FilePath { get; set; }

        //public P_MW_DSN MWDSN { get; set; }
        //public List<P_MW_SCANNED_DOCUMENT> DocList { get; set; }
        //public string Result { get; set; }
        //public string ResultDate { get; set; }
    }

    public class ItemHistoryList
    {
        public P_MW_RECORD_ITEM Item { get; set; }
        public List<P_MW_RECORD_ITEM_HISTORY> HistoryList { get; set; }
    }

    public class MwViewFormPerson
    {
        public string EffectFromDate { get; set; }
        public string EffectToDate { get; set; }
        public string Class1CeaseDate { get; set; }
        public string Class2CeaseDate { get; set; }
        public string CertificationNo { get; set; }
        public string EnglishName { get; set; }
        public string ChineseName { get; set; }
        public string IdentifyFlag { get; set; }
        public string ApEnglishName { get; set; }
        public string ApChineseName { get; set; }
        public string FaxNo { get; set; }
        public string ContactNo { get; set; }
        public string MwNo { get; set; }
    }

    public class MwScannedDocument
    {
        public P_MW_SCANNED_DOCUMENT ScannedDocument { get; set; }
        public string ReceivedDate { get; set; }
    }
}