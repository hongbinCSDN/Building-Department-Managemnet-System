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
    
    public partial class B_SV_SCANNED_DOCUMENT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public B_SV_SCANNED_DOCUMENT()
        {
            this.B_SV_SUBMISSION = new HashSet<B_SV_SUBMISSION>();
        }
    
        public string UUID { get; set; }
        public string RECORD_ID { get; set; }
        public string RECORD_TYPE { get; set; }
        public string DSN_NUMBER { get; set; }
        public string FILE_PATH { get; set; }
        public Nullable<decimal> PAGE_COUNT { get; set; }
        public string DOCUMENT_TYPE { get; set; }
        public string FILE_SIZE_CODE { get; set; }
        public string SUBMIT_TYPE { get; set; }
        public string DOC_DESCRIPTION { get; set; }
        public string RELATIVE_FILE_PATH { get; set; }
        public string STATUS_CODE { get; set; }
        public string FOLDER_TYPE { get; set; }
        public string RRM_SYN_STATUS { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public Nullable<System.DateTime> SCAN_DATE { get; set; }
        public string AS_THUMBNAIL { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<B_SV_SUBMISSION> B_SV_SUBMISSION { get; set; }
    }
}
