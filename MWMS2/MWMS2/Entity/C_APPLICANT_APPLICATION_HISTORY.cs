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
    
    public partial class C_APPLICANT_APPLICATION_HISTORY
    {
        public string UUID { get; set; }
        public string REGISTRATION_TYPE { get; set; }
        public string MASTER_ID { get; set; }
        public string APPLICANT_ID { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
    
        public virtual C_APPLICANT C_APPLICANT { get; set; }
    }
}
