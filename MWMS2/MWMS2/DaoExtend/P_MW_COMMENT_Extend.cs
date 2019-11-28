using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Entity
{
    [MetadataType(typeof(P_MW_COMMENT_Extend))]
    public partial class P_MW_COMMENT
    {
        public string HandlingUnit { get; set; }
    }

    public class P_MW_COMMENT_Extend
    {
    }
}