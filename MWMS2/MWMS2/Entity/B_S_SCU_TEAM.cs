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
    
    public partial class B_S_SCU_TEAM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public B_S_SCU_TEAM()
        {
            this.B_S_SCU_TEAM1 = new HashSet<B_S_SCU_TEAM>();
        }
    
        public string UUID { get; set; }
        public string USER_ACCOUNT_ID { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public Nullable<decimal> LEFT { get; set; }
        public Nullable<decimal> RIGHT { get; set; }
        public string POSITION { get; set; }
        public string PARENT_ID { get; set; }
        public Nullable<decimal> ON_LEVEL { get; set; }
        public Nullable<decimal> ORDERING { get; set; }
        public string SYS_POST_ID { get; set; }
        public string CHILD_SYS_POST_ID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<B_S_SCU_TEAM> B_S_SCU_TEAM1 { get; set; }
        public virtual B_S_SCU_TEAM B_S_SCU_TEAM2 { get; set; }
    }
}