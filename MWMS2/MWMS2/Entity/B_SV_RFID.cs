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
    
    public partial class B_SV_RFID
    {
        public string UUID { get; set; }
        public string RFID { get; set; }
        public string REMARK { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string SV_RECORD_ID { get; set; }
        public Nullable<System.DateTime> RFID_DATE { get; set; }
    
        public virtual B_SV_RECORD B_SV_RECORD { get; set; }
    }
}
