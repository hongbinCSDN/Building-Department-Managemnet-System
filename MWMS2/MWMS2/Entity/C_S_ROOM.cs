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
    
    public partial class C_S_ROOM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public C_S_ROOM()
        {
            this.C_INTERVIEW_SCHEDULE = new HashSet<C_INTERVIEW_SCHEDULE>();
        }
    
        public string UUID { get; set; }
        public string ROOM { get; set; }
        public string ENGLISH_ADDRESS_ID { get; set; }
        public string CHINESE_ADDRESS_ID { get; set; }
        public string TELEPHONE_NO { get; set; }
        public string FAX_NO { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
    
        public virtual C_ADDRESS C_ADDRESS { get; set; }
        public virtual C_ADDRESS C_ADDRESS1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<C_INTERVIEW_SCHEDULE> C_INTERVIEW_SCHEDULE { get; set; }
    }
}