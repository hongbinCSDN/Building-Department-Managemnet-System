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
    
    public partial class P_S_AUDIT_TRAIL
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public P_S_AUDIT_TRAIL()
        {
            this.P_S_AUDIT_TRAIL1 = new HashSet<P_S_AUDIT_TRAIL>();
            this.P_S_AUDIT_TRAIL_VALUE = new HashSet<P_S_AUDIT_TRAIL_VALUE>();
        }
    
        public string UUID { get; set; }
        public System.DateTime ACTION_DATE { get; set; }
        public string ACTION { get; set; }
        public string TRIGGERED_BY { get; set; }
        public string REF_AUDIT_TRAIL_ID { get; set; }
        public string MW_ID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
    
        public virtual P_MW_RECORD P_MW_RECORD { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<P_S_AUDIT_TRAIL> P_S_AUDIT_TRAIL1 { get; set; }
        public virtual P_S_AUDIT_TRAIL P_S_AUDIT_TRAIL2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<P_S_AUDIT_TRAIL_VALUE> P_S_AUDIT_TRAIL_VALUE { get; set; }
    }
}
