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

    public class Fn07CNV_CNVDisplayModel
    {
        public C_COMP_CONVICTION C_COMP_CONVICTION { get;set;}
        public C_IND_CONVICTION C_IND_CONVICTION { get; set; }
        public C_APPLICANT C_APPLICANT { get; set; }

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

        public bool Category1
        {
            get { return C_COMP_CONVICTION != null && "Y".Equals(C_COMP_CONVICTION.GBC_SELECT); }
            set { C_COMP_CONVICTION.GBC_SELECT = value ? "Y" : "N"; }
        }

        public bool Category2
        {
            get { return C_COMP_CONVICTION != null && "Y".Equals(C_COMP_CONVICTION.SC_D_SELECT); }
            set { C_COMP_CONVICTION.SC_D_SELECT = value ? "Y" : "N"; }
        }

        public bool Category3
        {
            get { return C_COMP_CONVICTION != null && "Y".Equals(C_COMP_CONVICTION.SC_F_SELECT); }
            set { C_COMP_CONVICTION.SC_F_SELECT = value ? "Y" : "N"; }
        }

        public bool Category4
        {
            get { return C_COMP_CONVICTION != null && "Y".Equals(C_COMP_CONVICTION.SC_GI_SELECT); }
            set { C_COMP_CONVICTION.SC_GI_SELECT = value ? "Y" : "N"; }
        }

        public bool Category5
        {
            get { return C_COMP_CONVICTION != null && "Y".Equals(C_COMP_CONVICTION.SC_SF_SELECT); }
            set { C_COMP_CONVICTION.SC_SF_SELECT = value ? "Y" : "N"; }
        }

        public bool Category6
        {
            get { return C_COMP_CONVICTION != null && "Y".Equals(C_COMP_CONVICTION.SC_V_SELECT); }
            set { C_COMP_CONVICTION.SC_V_SELECT = value ? "Y" : "N"; }
        }

        public bool Category7
        {
            get { return C_COMP_CONVICTION != null && "Y".Equals(C_COMP_CONVICTION.MWC_CO_SELECT); }
            set { C_COMP_CONVICTION.MWC_CO_SELECT = value ? "Y" : "N"; }
        }


        //public DateTime getModifiedDate()
        //{
        //    return C_IND_CONVICTION.MODIFIED_DATE;

        //}

        public string getDecryptHKID()
        {
            return EncryptDecryptUtil.getDecryptHKID(C_IND_CONVICTION.HKID);
        }

        public string getDecryptHKIDComp()
        {
            return EncryptDecryptUtil.getDecryptHKID(C_COMP_CONVICTION.ID_NO);
        }

        public string getDecryptPassportNo()
        {
            return EncryptDecryptUtil.getDecryptHKID(C_IND_CONVICTION.PASSPORT_NO);
        }
        public IEnumerable<SelectListItem> YesNoListWithNoIndication
        {
            get { return SystemListUtil.RetrieveYesNoExtra(); }
        }

        public List<SelectListItem> getTypeList()
        {
            return SystemListUtil.RetrieveCompInd(); 
        }

        public List<SelectListItem> getGender()
        {
            return SystemListUtil.RetrieveGender();
        }

        public List<SelectListItem> getYesNo()
        {
            return SystemListUtil.RetrieveYesNo();
        }


        public List<SelectListItem> getCompanyType()
        {
            return SystemListUtil.GetCompanyTypeList();
        }

        public List<SelectListItem> getConvictionSource()
        {
            return SystemListUtil.GetConvictionSourceList();
        }


    }
}