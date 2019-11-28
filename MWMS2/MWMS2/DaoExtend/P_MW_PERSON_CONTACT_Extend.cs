using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Entity
{
    [MetadataType(typeof(P_MW_PERSON_CONTACT_Extend))]
    public partial class P_MW_PERSON_CONTACT
    {

        public bool IS_ADDRESS_SAME_A1
        {
            get { return ADDRESS_SAME_A1 == "Y"; }
            set { ADDRESS_SAME_A1 = value ? "Y" : "N"; }
        }
        public bool IS_ADDRESS_SAME_A4
        {
            get { return ADDRESS_SAME_A4 == "Y"; }
            set { ADDRESS_SAME_A4 = value ? "Y" : "N"; }
        }
    }
    public class P_MW_PERSON_CONTACT_Extend
    {
    }
}