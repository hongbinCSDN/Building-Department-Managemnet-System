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
    
    public partial class P_MW_ACK_LETTER_FREEZE_DETAIL
    {
        public string UUID { get; set; }
        public string MASTER_ID { get; set; }
        public string MW_ACK_LETTER_UUID { get; set; }
        public string MW_RECORD_UUID { get; set; }
        public string TYPE { get; set; }
        public string VALUE { get; set; }
        public Nullable<System.DateTime> ITEM_PERIOD_FROM { get; set; }
        public Nullable<System.DateTime> ITEM_PERIOD_TO { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
    
        public virtual P_MW_ACK_LETTER_FREEZE_MASTER P_MW_ACK_LETTER_FREEZE_MASTER { get; set; }
    }
}