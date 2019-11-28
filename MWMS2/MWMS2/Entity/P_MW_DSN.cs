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
    
    public partial class P_MW_DSN
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public P_MW_DSN()
        {
            this.P_MW_GENERAL_RECORD_LETTER_INFO = new HashSet<P_MW_GENERAL_RECORD_LETTER_INFO>();
            this.P_MW_RECORD_LETTER_INFO = new HashSet<P_MW_RECORD_LETTER_INFO>();
            this.P_MW_DSN1 = new HashSet<P_MW_DSN>();
            this.P_MW_RECORD_PSAC_IMAGE = new HashSet<P_MW_RECORD_PSAC_IMAGE>();
            this.P_MW_SCANNED_DOCUMENT = new HashSet<P_MW_SCANNED_DOCUMENT>();
        }
    
        public string UUID { get; set; }
        public string RECORD_TYPE { get; set; }
        public string RECORD_ID { get; set; }
        public string DSN { get; set; }
        public Nullable<System.DateTime> MWU_RECEIVED_DATE { get; set; }
        public Nullable<System.DateTime> RD_RECEIVED_DATE { get; set; }
        public string FORM_CODE { get; set; }
        public string SCANNED_STATUS_ID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
        public Nullable<System.DateTime> RD_DELIVERED_DATE { get; set; }
        public Nullable<System.DateTime> REGISTRY_DELIVERED_DATE { get; set; }
        public string SUBMIT_TYPE { get; set; }
        public string OUTSTANDING_REMOVED { get; set; }
        public string RE_ASSIGN { get; set; }
        public string ITEM_SEQUENCE_NO { get; set; }
        public string SSP_SUBMITTED { get; set; }
        public Nullable<System.DateTime> ISSUED_DATE { get; set; }
        public string MW_DSN_PARENT_KEY { get; set; }
        public string SUBMIT_FLOW { get; set; }
        public string NATURE { get; set; }
        public string FROM_IMPORT { get; set; }
        public string SUBMISSION_NATURE { get; set; }
    
        public virtual P_S_SYSTEM_VALUE P_S_SYSTEM_VALUE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<P_MW_GENERAL_RECORD_LETTER_INFO> P_MW_GENERAL_RECORD_LETTER_INFO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<P_MW_RECORD_LETTER_INFO> P_MW_RECORD_LETTER_INFO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<P_MW_DSN> P_MW_DSN1 { get; set; }
        public virtual P_MW_DSN P_MW_DSN2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<P_MW_RECORD_PSAC_IMAGE> P_MW_RECORD_PSAC_IMAGE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<P_MW_SCANNED_DOCUMENT> P_MW_SCANNED_DOCUMENT { get; set; }
        public virtual P_S_SYSTEM_VALUE P_S_SYSTEM_VALUE1 { get; set; }
    }
}
