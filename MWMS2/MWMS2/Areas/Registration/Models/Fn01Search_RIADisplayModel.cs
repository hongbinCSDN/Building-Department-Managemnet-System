using MWMS2.Entity;
using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Areas.Registration.Models
{

    public class Fn01Search_RIADisplayModel
    {
        public C_IND_QUALIFICATION C_IND_QUALIFICATION { get; set; }
        public C_IND_APPLICATION C_IND_APPLICATION { get; set; }
        public C_IND_CERTIFICATE C_IND_CERTIFICATE { get; set; }
        [Display(Name = "Key Personnel")]
        public ApplicantDisplayListModel C_APPLICANT { get; set; } = new ApplicantDisplayListModel();

        [Display(Name = "Interest of Building Safety Services")]
        public List<C_S_SYSTEM_VALUE> C_S_SYSTEM_VALUEs_BS { get; set; } = new List<C_S_SYSTEM_VALUE>();

        [Display(Name = "Registration Status")]
        public C_S_SYSTEM_VALUE C_S_SYSTEM_VALUE_AppStatus { get; set; } = new C_S_SYSTEM_VALUE();

        [Display(Name = "(English)")]
        public C_ADDRESS C_ADDRESS_English { get; set; } = new C_ADDRESS();
        [Display(Name = "(Chinese)")]
        public C_ADDRESS C_ADDRESS_Chinese { get; set; } = new C_ADDRESS();

        [Display(Name = "Category")]
        public C_S_CATEGORY_CODE C_S_CATEGORY_CODE { get; set; }

        public List<C_IND_QUALIFICATION> C_IND_QUALIFICATIONs { get; set; }

        public string getDecryptHKID()
        {
            return EncryptDecryptUtil.getDecryptHKID(C_APPLICANT.HKID_PASSPORT_DISPLAY);
        }
    }
}