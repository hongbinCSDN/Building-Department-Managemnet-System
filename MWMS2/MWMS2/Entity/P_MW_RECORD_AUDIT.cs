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
    
    public partial class P_MW_RECORD_AUDIT
    {
        public string UUID { get; set; }
        public string MW_RECORD_ID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
        public string ACTION_DESC { get; set; }
        public string ASSIGNMENT { get; set; }
        public string AUDIT_STATUS { get; set; }
        public string FORM_TYPE { get; set; }
        public string DSN { get; set; }
        public string CONFIRMED { get; set; }
        public string AUDIT_MW_STATUS { get; set; }
        public string OTHER_HANDLING_OFFICER { get; set; }
        public string AUDIT_DESCRIPTION { get; set; }
        public string ORDER_NOTICE_LETTER_REF_NO { get; set; }
        public string AUDIT_RESULT { get; set; }
        public Nullable<System.DateTime> REFERRAL_DATE { get; set; }
        public Nullable<System.DateTime> REPLY_DATE { get; set; }
        public string AUDIT_REMARK { get; set; }
    
        public virtual P_MW_RECORD P_MW_RECORD { get; set; }
    }
}
