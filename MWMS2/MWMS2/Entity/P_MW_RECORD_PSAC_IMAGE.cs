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
    
    public partial class P_MW_RECORD_PSAC_IMAGE
    {
        public string UUID { get; set; }
        public string P_MW_RECORD_PSAC_ID { get; set; }
        public string P_DSN_ID { get; set; }
        public string DSN_SUB { get; set; }
        public string FILE_PATH { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }
        public string DOCUMENT_TYPE { get; set; }
    
        public virtual P_MW_DSN P_MW_DSN { get; set; }
        public virtual P_MW_RECORD_PSAC P_MW_RECORD_PSAC { get; set; }
    }
}
