using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Entity
{
    [MetadataType(typeof(P_MW_RECORD_PSAC_Extend))]
    public partial class P_MW_RECORD_PSAC
    {
        public bool IsCHK_MW_NOT_YET_COMM
        {
            get { return CHK_MW_NOT_YET_COMM == "Y"; }
            set { CHK_MW_NOT_YET_COMM = value ? "Y" : "N"; }
        }
        public bool IsCHK_MW_COMM_AND_COMP
        {
            get { return CHK_MW_COMM_AND_COMP == "Y"; }
            set { CHK_MW_COMM_AND_COMP = value ? "Y" : "N"; }
        }
        public bool IsCHK_MW_IN_PROGRESS
        {
            get { return CHK_MW_IN_PROGRESS == "Y"; }
            set { CHK_MW_IN_PROGRESS = value ? "Y" : "N"; }
        }
        public bool IsCHK_INACCESSIBLE
        {
            get { return CHK_INACCESSIBLE == "Y"; }
            set { CHK_INACCESSIBLE = value ? "Y" : "N"; }
        }

    }
    public partial class P_MW_RECORD_PSAC_Extend
    {
    }
}