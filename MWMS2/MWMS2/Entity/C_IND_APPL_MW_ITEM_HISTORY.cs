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
    
    public partial class C_IND_APPL_MW_ITEM_HISTORY
    {
        public string UUID { get; set; }
        public System.DateTime REGISTRATION_DATE { get; set; }
        public string MASTER_ID { get; set; }
        public string ITEM_TYPE_ID { get; set; }
        public string ITEM_CLASS_ID { get; set; }
        public string ITEM_DETAILS_ID { get; set; }
        public string SUPPORTED_BY_ID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
    
        public virtual C_S_SYSTEM_VALUE C_S_SYSTEM_VALUE { get; set; }
        public virtual C_S_SYSTEM_VALUE C_S_SYSTEM_VALUE1 { get; set; }
        public virtual C_S_SYSTEM_VALUE C_S_SYSTEM_VALUE2 { get; set; }
        public virtual C_IND_APPLICATION C_IND_APPLICATION { get; set; }
    }
}
