using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Entity
{
    [MetadataType(typeof(P_MW_RECORD_AL_FOLLOW_UP_OFFENCES_Extend))]
    public partial class P_MW_RECORD_AL_FOLLOW_UP_OFFENCES
    {
        public bool IsRectified
        {
            get { return IS_RECTIFIED == "Y"; }
            set { IS_RECTIFIED = value ? "Y" : "N"; }
        }

    }
    public class P_MW_RECORD_AL_FOLLOW_UP_OFFENCES_Extend
    {
    }
}