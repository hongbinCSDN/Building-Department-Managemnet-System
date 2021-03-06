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
    
    public partial class SYS_POST
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SYS_POST()
        {
            this.SYS_POST1 = new HashSet<SYS_POST>();
            this.SYS_POST_ROLE = new HashSet<SYS_POST_ROLE>();
            this.SYS_USER = new HashSet<SYS_USER>();
            this.SYS_POST_AREA = new HashSet<SYS_POST_AREA>();
        }
    
        public string UUID { get; set; }
        public string SYS_RANK_ID { get; set; }
        public string SYS_UNIT_ID { get; set; }
        public string SUPERVISOR_ID { get; set; }
        public string CODE { get; set; }
        public string PHONE { get; set; }
        public string FAX_NO { get; set; }
        public string IS_ACTIVE { get; set; }
        public Nullable<System.DateTime> CREATED_DT { get; set; }
        public string CREATED_POST { get; set; }
        public string CREATED_NAME { get; set; }
        public string CREATED_SECTION { get; set; }
        public Nullable<System.DateTime> LAST_MODIFIED_DT { get; set; }
        public string LAST_MODIFIED_POST { get; set; }
        public string LAST_MODIFIED_NAME { get; set; }
        public string LAST_MODIFIED_SECTION { get; set; }
        public string BD_PORTAL_LOGIN { get; set; }
        public string PW { get; set; }
        public string EMAIL { get; set; }
        public string DSMS_USERNAME { get; set; }
        public string DSMS_PW { get; set; }
        public string RECEIVE_CASE { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
    
        public virtual SYS_RANK SYS_RANK { get; set; }
        public virtual SYS_UNIT SYS_UNIT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SYS_POST> SYS_POST1 { get; set; }
        public virtual SYS_POST SYS_POST2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SYS_POST_ROLE> SYS_POST_ROLE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SYS_USER> SYS_USER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SYS_POST_AREA> SYS_POST_AREA { get; set; }
    }
}
