//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MWMS2.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class C_COMP_CONVICTION
    {
        public string UUID { get; set; }
        public string REGISTRATION_TYPE { get; set; }
        public string ENGLISH_NAME { get; set; }
        public string CHINESE_NAME { get; set; }
        public string BR_NO { get; set; }
        public string BRANCH_NO { get; set; }
        public string COMPANY_TYPE_ID { get; set; }
        public string SITE_DESCRIPTION { get; set; }
        public string CONVICTION_SOURCE_ID { get; set; }
        public string REMARKS { get; set; }
        public string RECORD_TYPE { get; set; }
        public Nullable<System.DateTime> IMPORT_DATE { get; set; }
        public string CR_SECTION { get; set; }
        public Nullable<System.DateTime> CR_OFFENCE_DATE { get; set; }
        public Nullable<System.DateTime> CR_JUDGE_DATE { get; set; }
        public Nullable<decimal> CR_FINE { get; set; }
        public string CR_ACCIDENT { get; set; }
        public string CR_FATAL { get; set; }
        public string CR_REPORT { get; set; }
        public string SRR_ACTION { get; set; }
        public Nullable<System.DateTime> SRR_EFFECTIVE_DATE { get; set; }
        public string SRR_CATEGORY { get; set; }
        public string SRR_SUSPENSION_DETAILS { get; set; }
        public Nullable<System.DateTime> SRR_SUSPENSION_FROM_DATE { get; set; }
        public Nullable<System.DateTime> SRR_SUSPENSION_TO_DATE { get; set; }
        public Nullable<System.DateTime> SRR_APPROVAL_DATE { get; set; }
        public string SRR_REPORT { get; set; }
        public Nullable<System.DateTime> DA_DECISION_DATE { get; set; }
        public string DA_DETAILS { get; set; }
        public string DA_REPORT { get; set; }
        public string MISC_DETAILS { get; set; }
        public Nullable<System.DateTime> MISC_RECEIVING_DATE { get; set; }
        public string MISC_REPORT { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
        public string PROPRI_NAME { get; set; }
        public string REFERENCE { get; set; }
        public string AS_NAME_ENG { get; set; }
        public string AS_NAME_CHN { get; set; }
        public string ID_NO { get; set; }
        public string CATEGORY { get; set; }
        public string CRC_INTERVIEW { get; set; }
        public Nullable<System.DateTime> CRC_INTERVIEW_DATE { get; set; }
        public string RECORD_CLEARED { get; set; }
        public string GBC_SELECT { get; set; }
        public string SC_D_SELECT { get; set; }
        public string SC_F_SELECT { get; set; }
        public string SC_GI_SELECT { get; set; }
        public string SC_SF_SELECT { get; set; }
        public string SC_V_SELECT { get; set; }
        public string MWC_CO_SELECT { get; set; }
    
        public virtual C_S_SYSTEM_VALUE C_S_SYSTEM_VALUE { get; set; }
        public virtual C_S_SYSTEM_VALUE C_S_SYSTEM_VALUE1 { get; set; }
    }
}
