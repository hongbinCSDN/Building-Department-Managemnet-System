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
    
    public partial class P_MOD_BD106_ITEM
    {
        public string UUID { get; set; }
        public string P_MOD_BD106_ID { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string S_SYSTEM_VALUE_UUID { get; set; }
    
        public virtual P_MOD_BD106 P_MOD_BD106 { get; set; }
        public virtual P_S_SYSTEM_VALUE P_S_SYSTEM_VALUE { get; set; }
    }
}