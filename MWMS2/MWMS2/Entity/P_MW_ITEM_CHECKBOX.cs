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
    
    public partial class P_MW_ITEM_CHECKBOX
    {
        public string UUID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
        public string CHECK_VALUE { get; set; }
        public string S_MW_ITEM_CHECKBOX_ID { get; set; }
        public string MW_ITEM_CHECKLIST_ITEM_ID { get; set; }
    
        public virtual P_MW_RECORD_ITEM_CHECKLIST_ITEM P_MW_RECORD_ITEM_CHECKLIST_ITEM { get; set; }
        public virtual P_S_MW_ITEM_CHECKBOX P_S_MW_ITEM_CHECKBOX { get; set; }
    }
}
