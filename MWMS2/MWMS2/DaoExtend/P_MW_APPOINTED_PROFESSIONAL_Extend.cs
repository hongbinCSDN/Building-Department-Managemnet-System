using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Entity
{
    [MetadataType(typeof(P_MW_APPOINTED_PROFESSIONAL_Extend))]
    public partial class P_MW_APPOINTED_PROFESSIONAL
    {
        public bool IS_CHECKED
        {
            get { return ISCHECKED == "Y"; }
            set { ISCHECKED = value ? "Y" : "N"; }
        }

        public bool IS_RECEIVE_SMS
        {
            get { return RECEIVE_SMS == "Y"; }
            set { RECEIVE_SMS = value ? "Y" : "N"; }
        }
    }

    public class P_MW_APPOINTED_PROFESSIONAL_Extend
    {
    }
}