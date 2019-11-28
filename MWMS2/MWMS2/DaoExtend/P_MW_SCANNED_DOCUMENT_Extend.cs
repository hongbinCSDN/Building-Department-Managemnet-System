using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Entity
{

    [MetadataType(typeof(P_MW_SCANNED_DOCUMENT_Extend))]
    public partial class P_MW_SCANNED_DOCUMENT
    {

        public string TEMPLATE_NAME { get; set; }
        public string TEMPLATE_NO
        {
            get
            {
                if (!string.IsNullOrEmpty(TEMPLATE_NAME))
                {
                    return TEMPLATE_NAME.Substring(0, 8);
                }
                else
                {
                    return null;
                }
            }
        }

    }
    public partial class P_MW_SCANNED_DOCUMENT_Extend
    {
    }
}