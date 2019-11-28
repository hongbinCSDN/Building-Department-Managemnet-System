using MWMS2.Entity;

using MWMS2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWMS2.Areas.Registration.Models
{

    public class Fn01Search_CNVDisplayModel
    {
        public C_COMP_CONVICTION C_COMP_CONVICTION { get;set;}
        public C_IND_CONVICTION C_IND_CONVICTION { get; set; }

        public bool CrReport
        {
            get { return C_COMP_CONVICTION != null && "Y".Equals(C_COMP_CONVICTION.CR_REPORT); }
            set { C_COMP_CONVICTION.CR_REPORT = value ? "Y" : "N"; }
        }

        public bool SrrReport
        {
            get { return C_COMP_CONVICTION != null && "Y".Equals(C_COMP_CONVICTION.SRR_REPORT); }
            set { C_COMP_CONVICTION.SRR_REPORT = value ? "Y" : "N"; }
        }

        public bool DaReport
        {
            get { return C_COMP_CONVICTION != null && "Y".Equals(C_COMP_CONVICTION.DA_REPORT); }
            set { C_COMP_CONVICTION.DA_REPORT = value ? "Y" : "N"; }
        }

        public bool MiscReport
        {
            get { return C_COMP_CONVICTION != null && "Y".Equals(C_COMP_CONVICTION.MISC_REPORT); }
            set { C_COMP_CONVICTION.MISC_REPORT = value ? "Y" : "N"; }
        }

        public bool Ind_CrReport
        {
            get { return C_IND_CONVICTION != null && "Y".Equals(C_IND_CONVICTION.CR_REPORT); }
            set { C_IND_CONVICTION.CR_REPORT = value ? "Y" : "N"; }
        }

        public bool Ind_SrrReport
        {
            get { return C_IND_CONVICTION != null && "Y".Equals(C_IND_CONVICTION.SRR_REPORT); }
            set { C_IND_CONVICTION.SRR_REPORT = value ? "Y" : "N"; }
        }

        public bool Ind_DaReport
        {
            get { return C_IND_CONVICTION != null && "Y".Equals(C_IND_CONVICTION.DA_REPORT); }
            set { C_IND_CONVICTION.DA_REPORT = value ? "Y" : "N"; }
        }

        public bool Ind_MiscReport
        {
            get { return C_IND_CONVICTION != null && "Y".Equals(C_IND_CONVICTION.MISC_REPORT); }
            set { C_IND_CONVICTION.MISC_REPORT = value ? "Y" : "N"; }
        }
        public string getDecryptHKID()
        {
            return EncryptDecryptUtil.getDecryptHKID(C_IND_CONVICTION.HKID);
        }


        public string getDecryptPassportNo()
        {
            return EncryptDecryptUtil.getDecryptHKID(C_IND_CONVICTION.PASSPORT_NO);
        }


        public List<SelectListItem> getConvictionSource()
        {
            return SystemListUtil.GetConvictionSourceList();
        }

        public List<SelectListItem> getGender()
        {
            return SystemListUtil.RetrieveGender();
        }

        public List<SelectListItem> getTypeList()
        {
            return SystemListUtil.RetrieveCompInd();
        }

        public List<SelectListItem> getCompanyType()
        {
            return SystemListUtil.GetCompanyTypeList();
        }

        public List<SelectListItem> getYesNo()
        {
            return SystemListUtil.RetrieveYesNo();
        }
    }
}