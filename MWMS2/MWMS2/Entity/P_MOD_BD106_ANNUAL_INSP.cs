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
    
    public partial class P_MOD_BD106_ANNUAL_INSP
    {
        public string UUID { get; set; }
        public string MOD_BD106_ID { get; set; }
        public Nullable<System.DateTime> REFERRAL_TO_LSS_DATE { get; set; }
        public Nullable<System.DateTime> SITE_INSPECTION_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
    
        public virtual P_MOD_BD106 P_MOD_BD106 { get; set; }
    }
}
