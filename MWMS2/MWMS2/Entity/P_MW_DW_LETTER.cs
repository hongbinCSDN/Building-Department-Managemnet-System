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
    
    public partial class P_MW_DW_LETTER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public P_MW_DW_LETTER()
        {
            this.P_MW_DW_LETTER_ITEM = new HashSet<P_MW_DW_LETTER_ITEM>();
        }
    
        public string UUID { get; set; }
        public string MW_ACK_LETTER_ID { get; set; }
        public string REMARK { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
        public string SCREENING { get; set; }
        public string PRELIMINARY { get; set; }
    
        public virtual P_MW_ACK_LETTER P_MW_ACK_LETTER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<P_MW_DW_LETTER_ITEM> P_MW_DW_LETTER_ITEM { get; set; }
    }
}
