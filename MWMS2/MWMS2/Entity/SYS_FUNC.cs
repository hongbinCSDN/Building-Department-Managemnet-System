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
    
    public partial class SYS_FUNC
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SYS_FUNC()
        {
            this.SYS_FUNC1 = new HashSet<SYS_FUNC>();
            this.SYS_ROLE_FUNC = new HashSet<SYS_ROLE_FUNC>();
        }
    
        public string UUID { get; set; }
        public string PARENT_ID { get; set; }
        public string CODE { get; set; }
        public string DESCRIPTION { get; set; }
        public string URL { get; set; }
        public Nullable<decimal> SEQ { get; set; }
        public string USE_TYPE { get; set; }
        public string ABLE_SHOW { get; set; }
        public string ABLE_LIST { get; set; }
        public string ABLE_VIEW_DETAILS { get; set; }
        public string ABLE_CREATE { get; set; }
        public string ABLE_EDIT { get; set; }
        public string ABLE_DELETE { get; set; }
        public string IS_ACTIVE { get; set; }
        public Nullable<System.DateTime> CREATED_DT { get; set; }
        public string CREATED_POST { get; set; }
        public string CREATED_NAME { get; set; }
        public string CREATED_SECTION { get; set; }
        public Nullable<System.DateTime> LAST_MODIFIED_DT { get; set; }
        public string LAST_MODIFIED_POST { get; set; }
        public string LAST_MODIFIED_NAME { get; set; }
        public string LAST_MODIFIED_SECTION { get; set; }
        public string ICON { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SYS_FUNC> SYS_FUNC1 { get; set; }
        public virtual SYS_FUNC SYS_FUNC2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SYS_ROLE_FUNC> SYS_ROLE_FUNC { get; set; }
    }
}