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
    
    public partial class P_MW_COMPLAINT_CHECKLIST
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public P_MW_COMPLAINT_CHECKLIST()
        {
            this.P_MW_COMPLAINT_CHECKLIST_COMMENT = new HashSet<P_MW_COMPLAINT_CHECKLIST_COMMENT>();
            this.P_MW_COMPLAINT_CHECKLIST_SECTION = new HashSet<P_MW_COMPLAINT_CHECKLIST_SECTION>();
        }
    
        public string UUID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
        public string RECORD_ID { get; set; }
        public string RE_ASSIGNMENT_REQUIRED { get; set; }
        public string REFERRAL_TO_REQUIRED { get; set; }
        public Nullable<System.DateTime> REFERRAL_TO_DUE_DATE { get; set; }
        public string STRUCTURAL_COMMENT_REQUIRED { get; set; }
        public Nullable<System.DateTime> STRUCTURAL_DUE_DATE { get; set; }
        public string INTERIM_REPLY_REQUIRED { get; set; }
        public string INTERIRM_REPLY_BY { get; set; }
        public string INTERIRM_REPLY_COMPLETED { get; set; }
        public string AUDIT_REQUIRED { get; set; }
        public string ASSIGN_OFFICER { get; set; }
        public string SITE_INSPECTION_RECORD_EXIST { get; set; }
        public string SITE_INSPECTION_RECORD { get; set; }
        public string PHOTOS_ATTACHED { get; set; }
        public string SITE_INSPECTION_REQUIRED { get; set; }
        public string REPLY_REQUIRED { get; set; }
        public string REPLY_BY { get; set; }
        public string REPLY_COMPLETED { get; set; }
        public string RECOMMENT_ON_ACTION_TAKEN { get; set; }
        public string STATUS { get; set; }
        public string DRAFT_REPLY_TEMPLATE { get; set; }
        public string DRAFT_INTERIRM_REPLY { get; set; }
        public string FLOW_STATUS { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<P_MW_COMPLAINT_CHECKLIST_COMMENT> P_MW_COMPLAINT_CHECKLIST_COMMENT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<P_MW_COMPLAINT_CHECKLIST_SECTION> P_MW_COMPLAINT_CHECKLIST_SECTION { get; set; }
    }
}
