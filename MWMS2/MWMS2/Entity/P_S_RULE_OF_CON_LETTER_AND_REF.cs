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
    
    public partial class P_S_RULE_OF_CON_LETTER_AND_REF
    {
        public string UUID { get; set; }
        public string FORM_CODE { get; set; }
        public string DESCRIPTION { get; set; }
        public string BMWR { get; set; }
        public Nullable<decimal> DAYS_OF_NOTIFICATION { get; set; }
        public Nullable<decimal> CONDITIONAL_LETTER_VALUE1 { get; set; }
        public string CONDITIONAL_LETTER_COMPARE1 { get; set; }
        public string CONDITIONAL_LETTER_COMPARE2 { get; set; }
        public Nullable<decimal> CONDITIONAL_LETTER_VALUE2 { get; set; }
        public string REFUSAL_COMPARE { get; set; }
        public Nullable<decimal> REFUSAL_VALUE { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
        public string S_FORM_TYPE_CODE { get; set; }
        public string DAYS_OF_NOTIFICATION_COMPARE { get; set; }
    }
}
