using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Entity
{

    [MetadataType(typeof(P_MW_RECORD_FORM_CHECKLIST_Extend))]
    public partial class P_MW_RECORD_FORM_CHECKLIST
    {

        public string CertType { get; set; }
        public string CertNo { get; set; }

    }
    public partial class P_MW_RECORD_FORM_CHECKLIST_Extend
    {
    }
}