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
    
    public partial class P_MW_APPOINTED_PROFESSIONAL
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public P_MW_APPOINTED_PROFESSIONAL()
        {
            this.P_MW_APPOINTED_PROF_HISTORY = new HashSet<P_MW_APPOINTED_PROF_HISTORY>();
            this.P_MW_DECLARARTION = new HashSet<P_MW_DECLARARTION>();
        }
    
        public string UUID { get; set; }
        public string CERTIFICATION_NO { get; set; }
        public string MW_RECORD_ID { get; set; }
        public string PROFESSIONAL_TYPE_ID { get; set; }
        public string CHINESE_NAME { get; set; }
        public string ENGLISH_NAME { get; set; }
        public string FORM_PART { get; set; }
        public string IS_UPDATE_ADDRESS { get; set; }
        public string STATUS_ID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
        public Nullable<System.DateTime> ENDORSEMENT_DATE { get; set; }
        public Nullable<System.DateTime> EFFECT_FROM_DATE { get; set; }
        public Nullable<System.DateTime> EFFECT_TO_DATE { get; set; }
        public string CHINESE_COMPANY_NAME { get; set; }
        public string ENGLISH_COMPANY_NAME { get; set; }
        public string IDENTIFY_FLAG { get; set; }
        public Nullable<decimal> ORDERING { get; set; }
        public Nullable<System.DateTime> COMMENCED_DATE { get; set; }
        public string ISCHECKED { get; set; }
        public Nullable<System.DateTime> COMPLETION_DATE { get; set; }
        public Nullable<System.DateTime> CLASS_COMPLETION_DATE { get; set; }
        public Nullable<System.DateTime> CLASS_ENDORSEMENT_DATE { get; set; }
        public Nullable<System.DateTime> EXPIRY_DATE { get; set; }
        public Nullable<System.DateTime> APPOINTMENT_DATE { get; set; }
        public Nullable<System.DateTime> CLASS1_CEASE_DATE { get; set; }
        public Nullable<System.DateTime> CLASS2_CEASE_DATE { get; set; }
        public string MW_NO { get; set; }
        public string CONTACT_NO { get; set; }
        public string FAX_NO { get; set; }
        public string RECEIVE_SMS { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<P_MW_APPOINTED_PROF_HISTORY> P_MW_APPOINTED_PROF_HISTORY { get; set; }
        public virtual P_MW_RECORD P_MW_RECORD { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<P_MW_DECLARARTION> P_MW_DECLARARTION { get; set; }
    }
}