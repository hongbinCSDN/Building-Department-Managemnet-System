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
    
    public partial class C_IND_QUALIFICATION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public C_IND_QUALIFICATION()
        {
            this.C_IND_QUALIFICATION_DETAIL = new HashSet<C_IND_QUALIFICATION_DETAIL>();
        }
    
        public string UUID { get; set; }
        public string MASTER_ID { get; set; }
        public string PRB_ID { get; set; }
        public string CATEGORY_ID { get; set; }
        public Nullable<System.DateTime> REGISTRATION_DATE { get; set; }
        public Nullable<System.DateTime> EXPIRY_DATE { get; set; }
        public string REGISTRATION_NUMBER { get; set; }
        public string PRB_EXPIRY_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
        public string QUALIFICATION_TYPE { get; set; }
    
        public virtual C_IND_APPLICATION C_IND_APPLICATION { get; set; }
        public virtual C_S_SYSTEM_VALUE C_S_SYSTEM_VALUE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<C_IND_QUALIFICATION_DETAIL> C_IND_QUALIFICATION_DETAIL { get; set; }
        public virtual C_S_CATEGORY_CODE C_S_CATEGORY_CODE { get; set; }
        public virtual C_S_SYSTEM_VALUE C_S_SYSTEM_VALUE1 { get; set; }
    }
}