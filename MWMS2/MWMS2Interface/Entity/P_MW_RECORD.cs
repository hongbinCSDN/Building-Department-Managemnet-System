//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MWMS2Interface.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class P_MW_RECORD
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public P_MW_RECORD()
        {
            this.P_MW_RECORD_ADDRESS_INFO = new HashSet<P_MW_RECORD_ADDRESS_INFO>();
            this.P_MW_RECORD_ITEM = new HashSet<P_MW_RECORD_ITEM>();
        }
    
        public string UUID { get; set; }
        public string REFERENCE_NUMBER { get; set; }
        public string LOCATION_ADDRESS_ID { get; set; }
        public Nullable<System.DateTime> COMMENCEMENT_DATE { get; set; }
        public Nullable<System.DateTime> COMPLETION_DATE { get; set; }
        public Nullable<System.DateTime> COMMENCEMENT_SUBMISSION_DATE { get; set; }
        public string STATUS_CODE { get; set; }
        public string SIGNBOARD_PERFROMER_ID { get; set; }
        public string S_FORM_TYPE_CODE { get; set; }
        public string PLAN_NO { get; set; }
        public string LANGUAGE_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
        public string CLASS_CODE { get; set; }
        public string MW_PROGRESS_STATUS_CODE { get; set; }
        public string IS_DATA_ENTRY { get; set; }
        public Nullable<System.DateTime> COMPLETION_SUBMISSION_DATE { get; set; }
        public string LOCATION_OF_MINOR_WORK { get; set; }
        public string FILE_REFERENCE { get; set; }
        public string OWNER_ID { get; set; }
        public Nullable<System.DateTime> COMMENCEMENT_ACKNOWLEDGE_DATE { get; set; }
        public Nullable<System.DateTime> COMMENCEMENT_CONDINTIONAL_DATE { get; set; }
        public Nullable<System.DateTime> COMMENCEMENT_REFUSE_DATE { get; set; }
        public Nullable<System.DateTime> COMPLETION_ACKNOWLEDGE_DATE { get; set; }
        public Nullable<System.DateTime> COMPLETION_CONDINTIONAL_DATE { get; set; }
        public Nullable<System.DateTime> COMPLETION_REFUSE_DATE { get; set; }
        public string SUBMIT_TYPE { get; set; }
        public Nullable<System.DateTime> FIRST_RECEIVED_DATE { get; set; }
        public Nullable<System.DateTime> LAST_RECEIVED_DATE { get; set; }
        public string VERIFICATION_SPO { get; set; }
        public string SPO_COMPLETE { get; set; }
        public string MW_DSN { get; set; }
        public string PERMIT_NO { get; set; }
        public string RRM_SYN_STATUS { get; set; }
        public Nullable<System.DateTime> INSPECTION_DATE { get; set; }
        public string FORM_VERSION { get; set; }
        public string OI_ID { get; set; }
        public string MW05_ITEM36 { get; set; }
        public string NO_OF_PREMISES { get; set; }
        public string FILEREF_FOUR { get; set; }
        public string FILEREF_TWO { get; set; }
        public string AUDIT_RELATED { get; set; }
        public string IMPORT_FORM_TYPE { get; set; }
        public string EFSS_REF_NO { get; set; }
        public string SITE_AUDIT_RELATED { get; set; }
        public string PRE_SITE_AUDIT_RELATED { get; set; }
        public string VERIFICATION_SPO_SMM { get; set; }
        public string SPO_COMPLETE_SMM { get; set; }
        public string SITE_AUDIT_RELATED_MW { get; set; }
        public string SITE_AUDIT_RELATED_SB { get; set; }
    
        public virtual P_MW_ADDRESS P_MW_ADDRESS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<P_MW_RECORD_ADDRESS_INFO> P_MW_RECORD_ADDRESS_INFO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<P_MW_RECORD_ITEM> P_MW_RECORD_ITEM { get; set; }
    }
}
