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
    
    public partial class C_QP_COUNT
    {
        public string UUID { get; set; }
        public System.DateTime COUNT_DATE { get; set; }
        public string COUNT_TYPE { get; set; }
        public Nullable<decimal> YES { get; set; }
        public Nullable<decimal> NO { get; set; }
        public Nullable<decimal> NO_INDICATION { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
    }
}