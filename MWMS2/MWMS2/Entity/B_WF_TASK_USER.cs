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
    
    public partial class B_WF_TASK_USER
    {
        public string UUID { get; set; }
        public string WF_TASK_ID { get; set; }
        public Nullable<System.DateTime> START_TIME { get; set; }
        public Nullable<System.DateTime> ACTION_TIME { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string STATUS { get; set; }
        public string SYS_POST_ID { get; set; }
    
        public virtual B_WF_TASK B_WF_TASK { get; set; }
    }
}
