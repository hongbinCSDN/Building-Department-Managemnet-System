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
    
    public partial class W_WL_FILE
    {
        public string UUID { get; set; }
        public string WL_UUID { get; set; }
        public byte[] LETTER_FILE { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public string STATUS_DESCRIPTION { get; set; }
        public string FILE_NAME { get; set; }
        public string FILE_PATH { get; set; }
    
        public virtual W_WL W_WL { get; set; }
    }
}
