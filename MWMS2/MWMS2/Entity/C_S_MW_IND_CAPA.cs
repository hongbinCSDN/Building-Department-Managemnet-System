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
    
    public partial class C_S_MW_IND_CAPA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public C_S_MW_IND_CAPA()
        {
            this.C_MW_IND_CAPA_DETAIL = new HashSet<C_MW_IND_CAPA_DETAIL>();
            this.C_S_MW_IND_CAPA_MAIN = new HashSet<C_S_MW_IND_CAPA_MAIN>();
        }
    
        public string UUID { get; set; }
        public string CHI_DESC_DISPLAY { get; set; }
        public string ITEM_DESC_DISPLAY { get; set; }
        public string ITEM_VALUE { get; set; }
        public string VERSION_CODE { get; set; }
        public string CODE { get; set; }
        public Nullable<decimal> ORDERING { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<C_MW_IND_CAPA_DETAIL> C_MW_IND_CAPA_DETAIL { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<C_S_MW_IND_CAPA_MAIN> C_S_MW_IND_CAPA_MAIN { get; set; }
    }
}
