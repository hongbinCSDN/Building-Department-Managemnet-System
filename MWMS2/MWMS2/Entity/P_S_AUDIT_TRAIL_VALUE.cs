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
    
    public partial class P_S_AUDIT_TRAIL_VALUE
    {
        public string UUID { get; set; }
        public string AUDIT_TRAIL_ID { get; set; }
        public string OLD_STATUS { get; set; }
        public string NEW_STATUS { get; set; }
        public string EMAIL_SENDER { get; set; }
        public string EMAIL_RECIPIENT { get; set; }
        public string EMAIL_CC_RECIPIENT { get; set; }
        public string EMAIL_SUBJECT { get; set; }
        public string EMAIL_CONTENT { get; set; }
        public string S_TEMPLATE_ID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
    
        public virtual P_S_AUDIT_TRAIL P_S_AUDIT_TRAIL { get; set; }
        public virtual P_S_TEMPLATE P_S_TEMPLATE { get; set; }
    }
}
