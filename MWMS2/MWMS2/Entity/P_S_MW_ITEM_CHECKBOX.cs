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
    
    public partial class P_S_MW_ITEM_CHECKBOX
    {
        public string UUID { get; set; }
        public string DESCRIPTION { get; set; }
        public string ITEM_NO { get; set; }
        public Nullable<decimal> ORDERING { get; set; }
        public Nullable<decimal> MW_ITEM_VERSION { get; set; }
        public Nullable<System.DateTime> EFFECTIVE_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
    
        public virtual P_MW_ITEM_CHECKBOX P_MW_ITEM_CHECKBOX { get; set; }
    }
}
