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
    
    public partial class P_IMPORT_36_ITEM
    {
        public string UUID { get; set; }
        public string P_IMPORT_36_UUID { get; set; }
        public string BLOCK_ID { get; set; }
        public string UNIT_ID { get; set; }
        public string NATURE { get; set; }
        public string RECEIVED_DATE { get; set; }
        public string FORM_TYPE { get; set; }
        public string MW_NO { get; set; }
        public string PBP { get; set; }
        public string PRC { get; set; }
        public string WORK_LOCATION { get; set; }
        public string COMM_DATE { get; set; }
        public string COMP_DATE { get; set; }
        public string PAW { get; set; }
        public string PAW_CONTACT { get; set; }
        public string LETTER_DATE { get; set; }
        public string UMW_NOTICE_NO { get; set; }
        public string BD_REF { get; set; }
        public string V_SUBMISSION_CASE_NO { get; set; }
        public string STATUTORY_NOTICE_NO { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
    
        public virtual P_IMPORT_36 P_IMPORT_36 { get; set; }
    }
}
