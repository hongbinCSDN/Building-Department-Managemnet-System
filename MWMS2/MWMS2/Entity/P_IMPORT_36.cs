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
    
    public partial class P_IMPORT_36
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public P_IMPORT_36()
        {
            this.P_IMPORT_36_ITEM = new HashSet<P_IMPORT_36_ITEM>();
        }
    
        public string UUID { get; set; }
        public string FILENAME { get; set; }
        public string FILE_PATH { get; set; }
        public string STATUS { get; set; }
        public Nullable<System.DateTime> CREATED_DT { get; set; }
        public string CREATED_POST { get; set; }
        public string CREATED_NAME { get; set; }
        public string CREATED_SECTION { get; set; }
        public Nullable<System.DateTime> LAST_MODIFIED_DT { get; set; }
        public string LAST_MODIFIED_POST { get; set; }
        public string LAST_MODIFIED_NAME { get; set; }
        public string LAST_MODIFIED_SECTION { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<P_IMPORT_36_ITEM> P_IMPORT_36_ITEM { get; set; }
    }
}