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
    
    public partial class C_EFSS_APPLICANT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public C_EFSS_APPLICANT()
        {
            this.C_EFSS_APPLICANT_MW_CAPA = new HashSet<C_EFSS_APPLICANT_MW_CAPA>();
        }
    
        public string ID { get; set; }
        public string FORMTYPE { get; set; }
        public string FORM_ID { get; set; }
        public string HKID { get; set; }
        public string PASSPORTNO { get; set; }
        public string SURNAME { get; set; }
        public string GIVENNAME { get; set; }
        public string CHNNAME { get; set; }
        public string SEX { get; set; }
        public string ROLE { get; set; }
        public string COMLANG { get; set; }
        public System.DateTime LASTUPDATE { get; set; }
        public System.DateTime CREATEDATE { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_DATE { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<C_EFSS_APPLICANT_MW_CAPA> C_EFSS_APPLICANT_MW_CAPA { get; set; }
    }
}
