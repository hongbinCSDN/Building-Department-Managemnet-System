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
    
    public partial class C_COMMITTEE_GROUP
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public C_COMMITTEE_GROUP()
        {
            this.C_COMMITTEE_GROUP_MEMBER = new HashSet<C_COMMITTEE_GROUP_MEMBER>();
            this.C_MEETING = new HashSet<C_MEETING>();
        }
    
        public string UUID { get; set; }
        public string NAME { get; set; }
        public Nullable<short> YEAR { get; set; }
        public string COMMITTEE_ID { get; set; }
        public string COMMITTEE_TYPE_ID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
        public Nullable<byte> MONTH { get; set; }
    
        public virtual C_COMMITTEE C_COMMITTEE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<C_COMMITTEE_GROUP_MEMBER> C_COMMITTEE_GROUP_MEMBER { get; set; }
        public virtual C_S_SYSTEM_VALUE C_S_SYSTEM_VALUE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<C_MEETING> C_MEETING { get; set; }
    }
}
