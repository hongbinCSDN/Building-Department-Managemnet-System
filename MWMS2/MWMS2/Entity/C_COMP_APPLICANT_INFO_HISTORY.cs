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
    
    public partial class C_COMP_APPLICANT_INFO_HISTORY
    {
        public string UUID { get; set; }
        public string COMPANY_APPLICANTS_ID { get; set; }
        public string ROLE_NAME { get; set; }
        public string REMARKS { get; set; }
        public Nullable<System.DateTime> ACCEPT_DATE { get; set; }
        public Nullable<System.DateTime> REMOVAL_DATE { get; set; }
        public string APPLICATION_STATUS_ID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
    
        public virtual C_COMP_APPLICANT_INFO C_COMP_APPLICANT_INFO { get; set; }
        public virtual C_S_SYSTEM_VALUE C_S_SYSTEM_VALUE { get; set; }
    }
}
