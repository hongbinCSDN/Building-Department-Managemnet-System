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
    
    public partial class C_APPLICANT_SCORING
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public C_APPLICANT_SCORING()
        {
            this.C_APPLICANT_SCORING_COURSE = new HashSet<C_APPLICANT_SCORING_COURSE>();
        }
    
        public string UUID { get; set; }
        public string CATEGORY { get; set; }
        public string REGISTRATION_NO { get; set; }
        public string AS_UUID { get; set; }
        public string REMARK { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<C_APPLICANT_SCORING_COURSE> C_APPLICANT_SCORING_COURSE { get; set; }
    }
}
