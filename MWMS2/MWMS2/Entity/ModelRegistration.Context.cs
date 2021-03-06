﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    public partial class EntitiesRegistration : EntityFilter
    {
        public EntitiesRegistration()
            : base("name=EntitiesConnection")
        {
            Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C_ADDRESS> C_ADDRESS { get; set; }
        public virtual DbSet<C_ADDRESS_HISTORY> C_ADDRESS_HISTORY { get; set; }
        public virtual DbSet<C_APPLICANT> C_APPLICANT { get; set; }
        public virtual DbSet<C_APPLICANT_APPLICATION_HISTORY> C_APPLICANT_APPLICATION_HISTORY { get; set; }
        public virtual DbSet<C_APPLICANT_HISTORY> C_APPLICANT_HISTORY { get; set; }
        public virtual DbSet<C_BUILDING_SAFETY_INFO> C_BUILDING_SAFETY_INFO { get; set; }
        public virtual DbSet<C_COMMITTEE> C_COMMITTEE { get; set; }
        public virtual DbSet<C_COMMITTEE_COMMITTEE_MEMBER> C_COMMITTEE_COMMITTEE_MEMBER { get; set; }
        public virtual DbSet<C_COMMITTEE_GROUP> C_COMMITTEE_GROUP { get; set; }
        public virtual DbSet<C_COMMITTEE_GROUP_MEMBER> C_COMMITTEE_GROUP_MEMBER { get; set; }
        public virtual DbSet<C_COMMITTEE_MEMBER> C_COMMITTEE_MEMBER { get; set; }
        public virtual DbSet<C_COMMITTEE_MEMBER_INSTITUTE> C_COMMITTEE_MEMBER_INSTITUTE { get; set; }
        public virtual DbSet<C_COMMITTEE_PANEL> C_COMMITTEE_PANEL { get; set; }
        public virtual DbSet<C_COMMITTEE_PANEL_MEMBER> C_COMMITTEE_PANEL_MEMBER { get; set; }
        public virtual DbSet<C_COMP_APPLICANT_INFO> C_COMP_APPLICANT_INFO { get; set; }
        public virtual DbSet<C_COMP_APPLICANT_INFO_DETAIL> C_COMP_APPLICANT_INFO_DETAIL { get; set; }
        public virtual DbSet<C_COMP_APPLICANT_INFO_HISTORY> C_COMP_APPLICANT_INFO_HISTORY { get; set; }
        public virtual DbSet<C_COMP_APPLICANT_INFO_MASTER> C_COMP_APPLICANT_INFO_MASTER { get; set; }
        public virtual DbSet<C_COMP_APPLICANT_MW_ITEM> C_COMP_APPLICANT_MW_ITEM { get; set; }
        public virtual DbSet<C_COMP_APPLICATION> C_COMP_APPLICATION { get; set; }
        public virtual DbSet<C_COMP_APPLICATION_HISTORY> C_COMP_APPLICATION_HISTORY { get; set; }
        public virtual DbSet<C_COMP_APPLICATION_MW_ITEM> C_COMP_APPLICATION_MW_ITEM { get; set; }
        public virtual DbSet<C_COMP_APPLN_MW_ITEM_HISTORY> C_COMP_APPLN_MW_ITEM_HISTORY { get; set; }
        public virtual DbSet<C_COMP_CONVICTION> C_COMP_CONVICTION { get; set; }
        public virtual DbSet<C_COMP_PROCESS_MONITOR> C_COMP_PROCESS_MONITOR { get; set; }
        public virtual DbSet<C_EFSS_APPLICANT> C_EFSS_APPLICANT { get; set; }
        public virtual DbSet<C_EFSS_APPLICANT_MW_CAPA> C_EFSS_APPLICANT_MW_CAPA { get; set; }
        public virtual DbSet<C_EFSS_BA24> C_EFSS_BA24 { get; set; }
        public virtual DbSet<C_EFSS_COMPANY> C_EFSS_COMPANY { get; set; }
        public virtual DbSet<C_EFSS_MWC> C_EFSS_MWC { get; set; }
        public virtual DbSet<C_EFSS_MWCW> C_EFSS_MWCW { get; set; }
        public virtual DbSet<C_EFSS_PROFESSIONAL> C_EFSS_PROFESSIONAL { get; set; }
        public virtual DbSet<C_IND_APPL_MW_ITEM_HISTORY> C_IND_APPL_MW_ITEM_HISTORY { get; set; }
        public virtual DbSet<C_IND_APPLICATION> C_IND_APPLICATION { get; set; }
        public virtual DbSet<C_IND_APPLICATION_MW_ITEM> C_IND_APPLICATION_MW_ITEM { get; set; }
        public virtual DbSet<C_IND_APPLICATION_MW_ITEM_DETAIL> C_IND_APPLICATION_MW_ITEM_DETAIL { get; set; }
        public virtual DbSet<C_IND_APPLICATION_MW_ITEM_MASTER> C_IND_APPLICATION_MW_ITEM_MASTER { get; set; }
        public virtual DbSet<C_IND_CERTIFICATE> C_IND_CERTIFICATE { get; set; }
        public virtual DbSet<C_IND_CERTIFICATE_HISTORY> C_IND_CERTIFICATE_HISTORY { get; set; }
        public virtual DbSet<C_IND_CONVICTION> C_IND_CONVICTION { get; set; }
        public virtual DbSet<C_IND_PROCESS_MONITOR> C_IND_PROCESS_MONITOR { get; set; }
        public virtual DbSet<C_IND_QUALIFICATION> C_IND_QUALIFICATION { get; set; }
        public virtual DbSet<C_IND_QUALIFICATION_DETAIL> C_IND_QUALIFICATION_DETAIL { get; set; }
        public virtual DbSet<C_INTERVIEW_CANDIDATES> C_INTERVIEW_CANDIDATES { get; set; }
        public virtual DbSet<C_INTERVIEW_SCHEDULE> C_INTERVIEW_SCHEDULE { get; set; }
        public virtual DbSet<C_LEAVE_FORM> C_LEAVE_FORM { get; set; }
        public virtual DbSet<C_MEETING> C_MEETING { get; set; }
        public virtual DbSet<C_MEETING_MEMBER> C_MEETING_MEMBER { get; set; }
        public virtual DbSet<C_MEMBER_CATEGORY> C_MEMBER_CATEGORY { get; set; }
        public virtual DbSet<C_NEW_SR_BS> C_NEW_SR_BS { get; set; }
        public virtual DbSet<C_NEW_SR_ITEM_NO> C_NEW_SR_ITEM_NO { get; set; }
        public virtual DbSet<C_NEW_SR_QP> C_NEW_SR_QP { get; set; }
        public virtual DbSet<C_NEW_SR_TYPE> C_NEW_SR_TYPE { get; set; }
        public virtual DbSet<C_OLD_SR_BS> C_OLD_SR_BS { get; set; }
        public virtual DbSet<C_OLD_SR_ITEM_NO> C_OLD_SR_ITEM_NO { get; set; }
        public virtual DbSet<C_OLD_SR_QP> C_OLD_SR_QP { get; set; }
        public virtual DbSet<C_OLD_SR_TYPE> C_OLD_SR_TYPE { get; set; }
        public virtual DbSet<C_QP_CARD_HISTORY> C_QP_CARD_HISTORY { get; set; }
        public virtual DbSet<C_QP_COUNT> C_QP_COUNT { get; set; }
        public virtual DbSet<C_S_AUTHORITY> C_S_AUTHORITY { get; set; }
        public virtual DbSet<C_S_BUILDING_SAFETY_ITEM> C_S_BUILDING_SAFETY_ITEM { get; set; }
        public virtual DbSet<C_S_CATEGORY_CODE> C_S_CATEGORY_CODE { get; set; }
        public virtual DbSet<C_S_CATEGORY_CODE_DETAIL> C_S_CATEGORY_CODE_DETAIL { get; set; }
        public virtual DbSet<C_S_EXPORT_LETTER> C_S_EXPORT_LETTER { get; set; }
        public virtual DbSet<C_S_HTML_NOTES> C_S_HTML_NOTES { get; set; }
        public virtual DbSet<C_S_LOG> C_S_LOG { get; set; }
        public virtual DbSet<C_S_PUBLIC_HOLIDAY> C_S_PUBLIC_HOLIDAY { get; set; }
        public virtual DbSet<C_S_ROOM> C_S_ROOM { get; set; }
        public virtual DbSet<C_S_SEARCH_LEVEL> C_S_SEARCH_LEVEL { get; set; }
        public virtual DbSet<C_S_SYSTEM_TYPE> C_S_SYSTEM_TYPE { get; set; }
        public virtual DbSet<C_S_SYSTEM_VALUE> C_S_SYSTEM_VALUE { get; set; }
        public virtual DbSet<C_S_USER_GROUP_CONV_INFO> C_S_USER_GROUP_CONV_INFO { get; set; }
        public virtual DbSet<C_SEARCH_REGISTRATION_BS> C_SEARCH_REGISTRATION_BS { get; set; }
        public virtual DbSet<C_SEARCH_REGISTRATION_ITEM_NO> C_SEARCH_REGISTRATION_ITEM_NO { get; set; }
        public virtual DbSet<C_SEARCH_REGISTRATION_QP> C_SEARCH_REGISTRATION_QP { get; set; }
        public virtual DbSet<C_SEARCH_REGISTRATION_TYPE> C_SEARCH_REGISTRATION_TYPE { get; set; }
        public virtual DbSet<C_AS_CONSENT> C_AS_CONSENT { get; set; }
        public virtual DbSet<C_BATCH_UPLOAD_QP_EXPERIENCE> C_BATCH_UPLOAD_QP_EXPERIENCE { get; set; }
        public virtual DbSet<C_VQPEXPORT> C_VQPEXPORT { get; set; }
        public virtual DbSet<C_POOLING> C_POOLING { get; set; }
        public virtual DbSet<C_MW_IND_CAPA_DETAIL> C_MW_IND_CAPA_DETAIL { get; set; }
        public virtual DbSet<C_MW_IND_CAPA_DETAIL_ITEM> C_MW_IND_CAPA_DETAIL_ITEM { get; set; }
        public virtual DbSet<C_S_MW_IND_CAPA> C_S_MW_IND_CAPA { get; set; }
        public virtual DbSet<C_S_MW_IND_CAPA_MAIN> C_S_MW_IND_CAPA_MAIN { get; set; }
        public virtual DbSet<C_MW_IND_CAPA_FINAL> C_MW_IND_CAPA_FINAL { get; set; }
        public virtual DbSet<C_MW_IND_CAPA_FINAL_ITEM> C_MW_IND_CAPA_FINAL_ITEM { get; set; }
        public virtual DbSet<C_APPLICANT_SCORING> C_APPLICANT_SCORING { get; set; }
        public virtual DbSet<C_APPLICANT_SCORING_COURSE> C_APPLICANT_SCORING_COURSE { get; set; }
        public virtual DbSet<C_REPORT_RPR> C_REPORT_RPR { get; set; }
    }
}
