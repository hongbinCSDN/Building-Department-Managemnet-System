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
    
    public partial class P_MW_MODIFICATION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public P_MW_MODIFICATION()
        {
            this.P_MW_MODIFICATION_RELATED_MWNO = new HashSet<P_MW_MODIFICATION_RELATED_MWNO>();
        }
    
        public string UUID { get; set; }
        public string ADDRESS { get; set; }
        public string APPLICANT_CAPACITY { get; set; }
        public string APPLICANT_NAME { get; set; }
        public string COUNTER { get; set; }
        public string DESC_OF_MODI { get; set; }
        public string DSN { get; set; }
        public string EMAIL { get; set; }
        public string FILE_TYPE { get; set; }
        public string FORM_NO { get; set; }
        public string IS_BUILDING_WORKS { get; set; }
        public string IS_STREET_WORKS { get; set; }
        public string LANGUAGE { get; set; }
        public string LOC_OF_SUBJECT { get; set; }
        public string LOT_NO { get; set; }
        public string NATURE { get; set; }
        public Nullable<System.DateTime> RECEIVED_DATE { get; set; }
        public string REFERENCE_NO { get; set; }
        public string REGULATIONS { get; set; }
        public string RRM_SYN_STATUS { get; set; }
        public string SUPPORTING_DOCUMENT { get; set; }
        public string UNABLE_TO_COMPLY_REASON { get; set; }
        public string VALIDITY { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
        public string HANDING_STAFF { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<P_MW_MODIFICATION_RELATED_MWNO> P_MW_MODIFICATION_RELATED_MWNO { get; set; }
    }
}