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
    
    public partial class B_SV_RECORD_ITEM_CHECKLIST
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public B_SV_RECORD_ITEM_CHECKLIST()
        {
            this.B_SV_RECORD_ITEM_CHECKLIST_ITEM = new HashSet<B_SV_RECORD_ITEM_CHECKLIST_ITEM>();
        }
    
        public string UUID { get; set; }
        public string SV_RECORD_ITEM_ID { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string SV_VALIDATION_ID { get; set; }
        public string RESULT { get; set; }
        public string RESULT_TO { get; set; }
    
        public virtual B_SV_RECORD_ITEM B_SV_RECORD_ITEM { get; set; }
        public virtual B_SV_VALIDATION B_SV_VALIDATION { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<B_SV_RECORD_ITEM_CHECKLIST_ITEM> B_SV_RECORD_ITEM_CHECKLIST_ITEM { get; set; }
    }
}
