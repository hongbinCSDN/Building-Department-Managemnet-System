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
    
    public partial class P_MW_RECORD_FORM_CHECKLIST
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public P_MW_RECORD_FORM_CHECKLIST()
        {
            this.P_MW_RECORD_FORM_CHECKLIST_PO = new HashSet<P_MW_RECORD_FORM_CHECKLIST_PO>();
        }
    
        public string UUID { get; set; }
        public string MW_RECORD_ID { get; set; }
        public string FORM_CODE { get; set; }
        public string STATUS_ID { get; set; }
        public string INFO_NOT { get; set; }
        public string INFO_NOT_RMK { get; set; }
        public string INFO_DATE { get; set; }
        public string INFO_DATE_RMK { get; set; }
        public string INFO_OTHER { get; set; }
        public string INFO_OTHER_RMK { get; set; }
        public string PBP_AP_NAME { get; set; }
        public string PBP_AP_NAME_RMK { get; set; }
        public string PBP_AP_VALID { get; set; }
        public string PBP_AP_VALID_RMK { get; set; }
        public string PBP_AP_SIGN { get; set; }
        public string PBP_AP_SIGN_RMK { get; set; }
        public string PBP_AP_SSP { get; set; }
        public string PBP_AP_SSP_RMK { get; set; }
        public string PBP_RSE_NAME { get; set; }
        public string PBP_RSE_NAME_RMK { get; set; }
        public string PBP_RSE_VALID { get; set; }
        public string PBP_RSE_VALID_RMK { get; set; }
        public string PBP_RSE_SIGN { get; set; }
        public string PBP_RSE_SIGN_RMK { get; set; }
        public string PBP_RGE_NAME { get; set; }
        public string PBP_RGE_NAME_RMK { get; set; }
        public string PBP_RGE_VALID { get; set; }
        public string PBP_RGE_VALID_RMK { get; set; }
        public string PBP_RGE_SIGN { get; set; }
        public string PBP_RGE_SIGN_RMK { get; set; }
        public string PBP_OTHER { get; set; }
        public string PBP_OTHER_RMK { get; set; }
        public string PRC_NAME { get; set; }
        public string PRC_NAME_RMK { get; set; }
        public string PRC_AS_NAME { get; set; }
        public string PRC_AS_NAME_RMK { get; set; }
        public string PRC_AP_VALID { get; set; }
        public string PRC_AP_VALID_RMK { get; set; }
        public string PRC_CAP { get; set; }
        public string PRC_CAP_RMK { get; set; }
        public string PRC_VALID { get; set; }
        public string PRC_VALID_RMK { get; set; }
        public string PRC_AS_VALID { get; set; }
        public string PRC_AS_VALID_RMK { get; set; }
        public string PRC_AS_SIGN { get; set; }
        public string PRC_AS_SIGN_RMK { get; set; }
        public string PRC_AS_OTHER { get; set; }
        public string PRC_AS_OTHER_RMK { get; set; }
        public string PRC_OTHER { get; set; }
        public string PRC_OTHER_RMK { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
        public string INFO_INS_DATE { get; set; }
        public string INFO_INS_DATE_RMK { get; set; }
        public string INFO_OWNER_NAME { get; set; }
        public string INFO_OWNER_NAME_RMK { get; set; }
        public string SSP_STATUS_ID { get; set; }
        public string PBP_AP_SSP_SPO { get; set; }
        public string INFO_NOT_PRC_CLASS1 { get; set; }
        public string INFO_NOT_PRC_CLASS1_RMK { get; set; }
        public string INFO_NOT_PRC_CLASS2 { get; set; }
        public string INFO_NOT_PRC_CLASS2_RMK { get; set; }
        public string AP_DETAIL_VALID { get; set; }
        public string AP_DETAIL_VALID_RMK { get; set; }
        public string RSE_DETAIL_VALID { get; set; }
        public string RSE_DETAIL_VALID_RMK { get; set; }
        public string RGE_DETAIL_VALID { get; set; }
        public string RGE_DETAIL_VALID_RMK { get; set; }
        public string PRC_DETAIL_VALID { get; set; }
        public string PRC_DETAIL_VALID_RMK { get; set; }
        public string DATE_VALID { get; set; }
        public string DATE_VALID_RMK { get; set; }
        public string AP_VALID_MSG { get; set; }
        public string RSE_VALID_MSG { get; set; }
        public string RGE_VALID_MSG { get; set; }
        public string PRC_VALID_MSG { get; set; }
        public string DATE_VALID_MSG { get; set; }
        public string MW_VERIFICATION_ID { get; set; }
        public string PBP_NEW_RGE_VALID { get; set; }
        public string PBP_NEW_RGE_VALID_RMK { get; set; }
        public string PBP_NEW_RGE_SIGN { get; set; }
        public string PBP_NEW_RGE_SIGN_RMK { get; set; }
        public string PBP_NEW_AP_VALID { get; set; }
        public string PBP_NEW_AP_VALID_RMK { get; set; }
        public string PBP_NEW_AP_SIGN { get; set; }
        public string PBP_NEW_AP_SIGN_RMK { get; set; }
        public string PBP_NEW_RSE_VALID { get; set; }
        public string PBP_NEW_RSE_VALID_RMK { get; set; }
        public string PBP_NEW_RSE_SIGN { get; set; }
        public string PBP_NEW_RSE_SIGN_RMK { get; set; }
        public string MW_NO_VALID { get; set; }
        public string MW_NO_VALID_RMK { get; set; }
        public string SIGNBOARD_DETAIL_VALID { get; set; }
        public string SIGNBOARD_DETAIL_VALID_RMK { get; set; }
        public string MW_NO_VALID_MSG { get; set; }
        public string SIGNBOARD_DETAIL_VALID_MSG { get; set; }
        public string PRC_OTHER_AS_LIST { get; set; }
        public string PBP_NEW_NAME { get; set; }
        public string PBP_NEW_NAME_RMK { get; set; }
        public string FORM6_AP_CAP { get; set; }
        public string FORM6_AP_CAP_RMK { get; set; }
        public string FORM6_AP_AS_VALID { get; set; }
        public string FORM6_AP_AS_VALID_RMK { get; set; }
        public string FORM6_AP_AS_SIGN { get; set; }
        public string FORM6_AP_AS_SIGN_RMK { get; set; }
        public string FORM6_AP_AS_OTHER { get; set; }
        public string FORM6_AP_AS_OTHER_RMK { get; set; }
        public string FORM6_AP_OTHER_AS_LIST { get; set; }
        public string FORM6_AP_AS_NAME { get; set; }
        public string FORM6_AP_AS_NAME_RMK { get; set; }
        public string PBP_AP_DEC1 { get; set; }
        public string PBP_AP_DEC1_RMK { get; set; }
        public string PBP_AP_DEC5 { get; set; }
        public string PBP_AP_DEC5_RMK { get; set; }
        public string PBP_AP_DEC6 { get; set; }
        public string PBP_AP_DEC6_RMK { get; set; }
        public string PBP_AP_DEC7 { get; set; }
        public string PBP_AP_DEC7_RMK { get; set; }
        public string PBP_AP_DEC8 { get; set; }
        public string PBP_AP_DEC8_RMK { get; set; }
        public string PBP_AP_DEC_S483 { get; set; }
        public string PBP_AP_DEC_S483_RMK { get; set; }
        public string PBP_AP_SIGNATURE_DATE { get; set; }
        public string PBP_AP_SIGNATURE_DATE_RMK { get; set; }
        public string PBP_RSE_DEC1 { get; set; }
        public string PBP_RSE_DEC1_RMK { get; set; }
        public string PBP_RSE_DEC3 { get; set; }
        public string PBP_RSE_DEC3_RMK { get; set; }
        public string PBP_RSE_DEC4 { get; set; }
        public string PBP_RSE_DEC4_RMK { get; set; }
        public string PBP_RSE_SIGNATURE_DATE { get; set; }
        public string PBP_RSE_SIGNATURE_DATE_RMK { get; set; }
        public string PBP_RGE_DEC1 { get; set; }
        public string PBP_RGE_DEC1_RMK { get; set; }
        public string PBP_RGE_DEC3 { get; set; }
        public string PBP_RGE_DEC3_RMK { get; set; }
        public string PBP_RGE_DEC4 { get; set; }
        public string PBP_RGE_DEC4_RMK { get; set; }
        public string PBP_RGE_SIGNATURE_DATE { get; set; }
        public string PBP_RGE_SIGNATURE_DATE_RMK { get; set; }
        public string PRC_DEC1 { get; set; }
        public string PRC_DEC1_RMK { get; set; }
        public string PRC_DEC2 { get; set; }
        public string PRC_DEC2_RMK { get; set; }
        public string PRC_DEC3 { get; set; }
        public string PRC_DEC3_RMK { get; set; }
        public string PRC_DEC4 { get; set; }
        public string PRC_DEC4_RMK { get; set; }
        public string PRC_DEC5 { get; set; }
        public string PRC_DEC5_RMK { get; set; }
        public string PRC_DEC6 { get; set; }
        public string PRC_DEC6_RMK { get; set; }
        public string PRC_DEC7 { get; set; }
        public string PRC_DEC7_RMK { get; set; }
        public string PRC_DEC12 { get; set; }
        public string PRC_DEC12_RMK { get; set; }
        public string PRC_DEC13 { get; set; }
        public string PRC_DEC13_RMK { get; set; }
        public string PRC_INVOLVE_CLASS2 { get; set; }
        public string PRC_INVOLVE_CLASS2_RMK { get; set; }
        public string PRC_INVOLVE_CLASS3 { get; set; }
        public string PRC_INVOLVE_CLASS3_RMK { get; set; }
        public string PRC_DEC_S33 { get; set; }
        public string PRC_DEC_S33_RMK { get; set; }
        public string PRC_DEC_S34 { get; set; }
        public string PRC_DEC_S34_RMK { get; set; }
        public string PRC_DEC_S36 { get; set; }
        public string PRC_DEC_S36_RMK { get; set; }
        public string PRC_DEC_S37 { get; set; }
        public string PRC_DEC_S37_RMK { get; set; }
        public string PRC_SIGNATURE_DATE { get; set; }
        public string PRC_SIGNATURE_DATE_RMK { get; set; }
        public string APPLICANT_DETAIL_VALID { get; set; }
        public string APPLICANT_DETAIL_VALID_RMK { get; set; }
        public string APPLICANT_DETAIL_MSG { get; set; }
        public string APPLICANT_SIGN_VALID { get; set; }
        public string APPLICANT_SIGN_VALID_RMK { get; set; }
        public string FORM09_REASON { get; set; }
        public string FORM09_REASON_RMK { get; set; }
        public string FORM09_CERT_NO { get; set; }
        public string FORM09_CERT_NO_RMK { get; set; }
        public string FORM09_ACTING_PERIOD { get; set; }
        public string FORM09_ACTING_PERIOD_RMK { get; set; }
        public string PBP_NEW_SIGNATURE_DATE { get; set; }
        public string PBP_NEW_SIGNATURE_DATE_RMK { get; set; }
        public string FORM05_WORK_RELATED { get; set; }
        public string FORM05_WORK_RELATED_RMK { get; set; }
        public string FORM05_LOCATION_MSG { get; set; }
        public string FORM05_LOCATION_VALID { get; set; }
        public string FORM05_LOCATION_VALID_RMK { get; set; }
        public string FORM32_WORK_ITEMS { get; set; }
        public string FORM32_WORK_ITEMS_RMK { get; set; }
        public string FORM07_DEC_S27 { get; set; }
        public string FORM07_DEC_S27_RMK { get; set; }
        public string FORM07_DEC_S48_2 { get; set; }
        public string FORM07_DEC_S48_2_RMK { get; set; }
        public string FORM07_DEC_S48_4 { get; set; }
        public string FORM07_DEC_S48_4_RMK { get; set; }
        public string PRC_INFO_NAME { get; set; }
        public string PRC_INFO_NAME_MSG { get; set; }
        public string PRC_INFO_AS_NAME { get; set; }
        public string PRC_INFO_AS_NAME_MSG { get; set; }
        public string PRC_INFO_ENGLISH_NAME { get; set; }
        public string PRC_INFO_ENGLISH_NAME_MSG { get; set; }
        public string PRC_INFO_CHINESE_NAME { get; set; }
        public string PRC_INFO_CHINESE_NAME_MSG { get; set; }
        public string PRC_INFO_AS_ENGLISH_NAME { get; set; }
        public string PRC_INFO_AS_ENGLISH_NAME_MSG { get; set; }
        public string PRC_INFO_AS_CHINESE_NAME { get; set; }
        public string PRC_INFO_AS_CHINESE_NAME_MSG { get; set; }
        public string PBP_AP_INFO_NAME { get; set; }
        public string PBP_AP_INFO_NAME_MSG { get; set; }
        public string PBP_AP_INFO_ENGLISH_NAME { get; set; }
        public string PBP_AP_INFO_ENGLISH_NAME_MSG { get; set; }
        public string PBP_AP_INFO_CHINESE_NAME { get; set; }
        public string PBP_AP_INFO_CHINESE_NAME_MSG { get; set; }
        public string PBP_RSE_INFO_NAME { get; set; }
        public string PBP_RSE_INFO_NAME_MSG { get; set; }
        public string PBP_RSE_INFO_ENGLISH_NAME { get; set; }
        public string PBP_RSE_INFO_ENGLISH_NAME_MSG { get; set; }
        public string PBP_RSE_INFO_CHINESE_NAME { get; set; }
        public string PBP_RSE_INFO_CHINESE_NAME_MSG { get; set; }
        public string PBP_RGE_INFO_NAME { get; set; }
        public string PBP_RGE_INFO_NAME_MSG { get; set; }
        public string PBP_RGE_INFO_ENGLISH_NAME { get; set; }
        public string PBP_RGE_INFO_ENGLISH_NAME_MSG { get; set; }
        public string PBP_RGE_INFO_CHINESE_NAME { get; set; }
        public string PBP_RGE_INFO_CHINESE_NAME_MSG { get; set; }
        public string PBP_NEW_INFO_NAME { get; set; }
        public string PBP_NEW_INFO_NAME_MSG { get; set; }
        public string PBP_NEW_INFO_ENGLISH_NAME { get; set; }
        public string PBP_NEW_INFO_ENGLISH_NAME_MSG { get; set; }
        public string PBP_NEW_INFO_CHINESE_NAME { get; set; }
        public string PBP_NEW_INFO_CHINESE_NAME_MSG { get; set; }
        public string RGBC_MWC_INFO_NAME { get; set; }
        public string RGBC_MWC_INFO_NAME_MSG { get; set; }
        public string RGBC_MWC_INFO_ENG_NAME { get; set; }
        public string RGBC_MWC_INFO_ENG_NAME_MSG { get; set; }
        public string RGBC_MWC_INFO_CHI_NAME { get; set; }
        public string RGBC_MWC_INFO_CHI_NAME_MSG { get; set; }
        public string RGBC_MWC_INFO_AS_NAME { get; set; }
        public string RGBC_MWC_INFO_AS_NAME_MSG { get; set; }
        public string RGBC_MWC_INFO_AS_ENG_NAME { get; set; }
        public string RGBC_MWC_INFO_AS_ENG_NAME_MSG { get; set; }
        public string RGBC_MWC_INFO_AS_CHI_NAME { get; set; }
        public string RGBC_MWC_INFO_AS_CHI_NAME_MSG { get; set; }
        public string PBP_AP_SSP_SPO_RMK { get; set; }
        public string INFO_AP_RI { get; set; }
        public string INFO_AP_RI_RMK { get; set; }
        public string HANDLING_UNIT { get; set; }
    
        public virtual P_MW_RECORD P_MW_RECORD { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<P_MW_RECORD_FORM_CHECKLIST_PO> P_MW_RECORD_FORM_CHECKLIST_PO { get; set; }
        public virtual P_S_SYSTEM_VALUE P_S_SYSTEM_VALUE { get; set; }
        public virtual P_S_SYSTEM_VALUE P_S_SYSTEM_VALUE1 { get; set; }
    }
}
