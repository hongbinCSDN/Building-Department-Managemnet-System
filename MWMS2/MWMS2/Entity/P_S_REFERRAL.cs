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
    
    public partial class P_S_REFERRAL
    {
        public string UUID { get; set; }
        public string EMAIL_ADDRESS { get; set; }
        public string RECEVIER_NAME { get; set; }
        public string REPLY_NAME { get; set; }
        public string REPLAY_ADDRESS { get; set; }
        public string CASE_ID { get; set; }
        public string IS_ACTIVE { get; set; }
        public string S_TEMPLATE_ID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
    
        public virtual P_S_TEMPLATE P_S_TEMPLATE { get; set; }
    }
}