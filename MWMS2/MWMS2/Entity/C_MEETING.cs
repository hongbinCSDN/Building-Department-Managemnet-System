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
    
    public partial class C_MEETING
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public C_MEETING()
        {
            this.C_INTERVIEW_SCHEDULE = new HashSet<C_INTERVIEW_SCHEDULE>();
            this.C_MEETING_MEMBER = new HashSet<C_MEETING_MEMBER>();
        }
    
        public string UUID { get; set; }
        public string MEETING_GROUP { get; set; }
        public Nullable<short> YEAR { get; set; }
        public string COMMITTEE_GROUP_ID { get; set; }
        public string COMMITTEE_TYPE_ID { get; set; }
        public string MEETING_NO { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
    
        public virtual C_COMMITTEE_GROUP C_COMMITTEE_GROUP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<C_INTERVIEW_SCHEDULE> C_INTERVIEW_SCHEDULE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<C_MEETING_MEMBER> C_MEETING_MEMBER { get; set; }
        public virtual C_S_SYSTEM_VALUE C_S_SYSTEM_VALUE { get; set; }
    }
}