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
    
    public partial class P_MW_VERIFICATION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public P_MW_VERIFICATION()
        {
            this.P_MW_RECORD_ITEM_CHECKLIST = new HashSet<P_MW_RECORD_ITEM_CHECKLIST>();
        }
    
        public string UUID { get; set; }
        public string MW_RECORD_ID { get; set; }
        public string STATUS_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string SPO_ROLLBACK { get; set; }
        public string PO_ROLLBACK { get; set; }
        public string HANDLING_UNIT { get; set; }
    
        public virtual P_MW_RECORD P_MW_RECORD { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<P_MW_RECORD_ITEM_CHECKLIST> P_MW_RECORD_ITEM_CHECKLIST { get; set; }
    }
}
