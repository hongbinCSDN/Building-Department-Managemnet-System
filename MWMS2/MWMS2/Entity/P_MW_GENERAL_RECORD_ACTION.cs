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
    
    public partial class P_MW_GENERAL_RECORD_ACTION
    {
        public string UUID { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string ACTION_NAME { get; set; }
        public Nullable<System.DateTime> ACTION_DATE { get; set; }
        public string DESCRIPTION { get; set; }
        public string ACTION_OFFICER { get; set; }
        public string RECORD_ID { get; set; }
    
        public virtual P_MW_GENERAL_RECORD P_MW_GENERAL_RECORD { get; set; }
    }
}
