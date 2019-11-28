using MWMS2.Constant;
using MWMS2.Entity;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Models
{
    public class UpdateAppStatusDisplayModel
    {
        public string CatCode { get; set; }
        public bool SaveSuccess { get; set; }
        public C_COMP_APPLICATION C_COMP_APPLICATION { get; set; } = new C_COMP_APPLICATION();
        public C_IND_APPLICATION C_IND_APPLICATION { get; set; }
        public C_IND_CERTIFICATE C_IND_CERTIFICATE { get; set; } = new C_IND_CERTIFICATE();
        public C_S_CATEGORY_CODE C_S_CATEGORY_CODE { get; set; }
        public C_APPLICANT C_APPLICANT { get; set; }
        [Display(Name = "(English)")]
        public C_ADDRESS C_ADDRESS_English { get; set; } = new C_ADDRESS();
        [Display(Name = "(Chinese)")]
        public C_ADDRESS C_ADDRESS_Chinese { get; set; } = new C_ADDRESS();

        [Display(Name = "(Ind_English)")]
        public C_ADDRESS C_IND_ADDRESS_English { get; set; } = new C_ADDRESS();
        [Display(Name = "(Ind_Chinese)")]
        public C_ADDRESS C_IND_ADDRESS_Chinese { get; set; } = new C_ADDRESS();
        public string DateType { get; set; }
        public IEnumerable<SelectListItem> DateTypeList
        {
            get { return SystemListUtil.GetProcessList(); }
        }
        public IEnumerable<SelectListItem> IndDateTypeList
        {
            get { return SystemListUtil.GetIndProcessList(); }
        }
        public IEnumerable<SelectListItem> ValidityPeriodList {
            get { return SystemListUtil.GetValidityPeriodListByType(); }
        }
        public IEnumerable<SelectListItem> CGAFormList
        {
            get { return SystemListUtil.GetFormList(RegistrationConstant.REGISTRATION_TYPE_CGA); }
        }
        public IEnumerable<SelectListItem> PAFormList
        {
            get { return SystemListUtil.GetFormList(RegistrationConstant.REGISTRATION_TYPE_MWIA); }
        }
        public IEnumerable<SelectListItem> MWCAFormList
        {
            get { return SystemListUtil.GetFormList(RegistrationConstant.REGISTRATION_TYPE_MWCA); }
        }
        public IEnumerable<SelectListItem> MWIAFormList
        {
            get { return SystemListUtil.GetFormList(RegistrationConstant.REGISTRATION_TYPE_MWIA); }
        }
        //Issue Date:
        public string reportIssueDateNewRegistration { get; set; }
        //Last Document Received Date:
        public string reportRcvddateNewRegistration { get; set; }
        //Signature Name:
        public string reportAuthorityUUIDNewRegistration { get; set; }

        //signature name::
        public string newRegAuthorityName { get; set; }
        public string retentionAuthorityName { get; set; }
        public string restorationAuthorityName { get; set; }
        public string removalAuthorityName { get; set; }

        public IEnumerable<SelectListItem> AuthorityList
        {
            get { return SystemListUtil.RetrieveSAuthorityList(); }
        }
        public List<string> missingItem { get; set; }
        public List<string> missingItemRet { get; set; }
        //Missing Item
        public IEnumerable<SelectListItem> MissItemByType
        {
            get { return C_COMP_APPLICATION == null ? SystemListUtil.GetMissItem("") : SystemListUtil.GetMissItem(C_COMP_APPLICATION.MISS_DOCUMENT_TYPE); }
        }
        public IEnumerable<SelectListItem> MissIndItemByType
        {
            get { return C_IND_CERTIFICATE == null ? SystemListUtil.GetMissItem("") : SystemListUtil.GetMissItem(C_IND_CERTIFICATE.MISS_DOCUMENT_TYPE); }

        }
        public DateTime? DateVetting { get; set; }
        public DateTime? IndDateVetting { get; set; }
        public DateTime? dateRetentionApplcationRet { get; set; }
        public DateTime? dateRestorationApplicationRes { get; set; }

        public DateTime? NewRegExtDate { get;set;}
        public DateTime? RetExtDate { get; set; }
        public DateTime? ResExtDate { get; set; }

        public DateTime? IndNewRegExtDate { get; set; }
        public DateTime? IndRetExtDate { get; set; }
        public DateTime? IndResExtDate { get; set; }
        public DateTime? dateIndRetentionApplcationRet { get; set; }
        public DateTime? dateIndRestorationApplicationRes { get; set; }
        public bool isConfirmMail { get; set; }
        //new add for doc
        public List<SelectListItem> getYNOption()
        {
            var a = SystemListUtil.RetrieveYNOption();
            a.Reverse();
            a.Last().Selected = true;
            return a;
        }
        public Nullable<System.DateTime> CARD_APP_DATE { get; set; }
        public List<CERTLIST> CERTLISTs { get; set; } = new List<CERTLIST>();
        public class CERTLIST
        {
            public C_IND_CERTIFICATE CERT_c_IND_CERTIFICATE { get; set; }
            public C_S_CATEGORY_CODE CERT_c_S_CATEGORY_CODE { get; set; }
            public C_S_SYSTEM_VALUE CERT_APPFORM_c_S_SYSTEM_VALUE { get; set; }
            public C_S_SYSTEM_VALUE CERT_PERIODVAD_c_S_SYSTEM_VALUE { get; set; }
        }
        public List<SelectListItem> getCertiList()
        {
            List<SelectListItem> selectListItems = new List<SelectListItem>();
            selectListItems.Add(new SelectListItem
            {
                Text = "-",
                Value = "",

            });
            foreach (var item in CERTLISTs)
            {
                selectListItems.Add(new SelectListItem
                {
                    Text = item.CERT_c_IND_CERTIFICATE.CERTIFICATION_NO,
                    Value = item.CERT_c_IND_CERTIFICATE.UUID,

                });
            }
            return selectListItems;

        }
        public string SelectedCertiUUID { get; set; }
        public C_IND_CERTIFICATE SelectedCertificate { get; set; }

        public C_ADDRESS BS_ADDRESS_ENG { get; set; } = new C_ADDRESS();
        public C_ADDRESS BS_ADDRESS_CHI { get; set; } = new C_ADDRESS();

        public string ProcessPage { get; set; }

        public DateTime? AGFrDate { get; set; }
        public DateTime? AGToDate { get; set; }

        public string AnnualGazetteCtrUUID { get; set; }
        public IEnumerable<SelectListItem> CategoryList
        {

            get
            {
                return SystemListUtil.GetCatList();
            }
        }
    }
}