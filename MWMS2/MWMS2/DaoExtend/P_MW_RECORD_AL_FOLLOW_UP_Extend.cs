using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Entity
{
    [MetadataType(typeof(P_MW_RECORD_AL_FOLLOW_UP_Extend))]
    public partial class P_MW_RECORD_AL_FOLLOW_UP
    {
        public bool IsMinor
        {
            get { return IS_MINOR == "Y"; }
            set { IS_MINOR = value ? "Y" : "N"; }
        }

        public bool IsInvolveMajorOffence
        {
            get { return IS_INVOLVE_MAJOR_OFFENCE == "Y"; }
            set { IS_INVOLVE_MAJOR_OFFENCE = value ? "Y" : "N"; }
        }

    }
    public class P_MW_RECORD_AL_FOLLOW_UP_Extend
    {
    }
}