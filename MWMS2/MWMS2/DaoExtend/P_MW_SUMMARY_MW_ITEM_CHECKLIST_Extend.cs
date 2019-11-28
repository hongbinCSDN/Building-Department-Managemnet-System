using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Entity
{
    [MetadataType(typeof(P_MW_SUMMARY_MW_ITEM_CHECKLIST_Extend))]
    public partial class P_MW_SUMMARY_MW_ITEM_CHECKLIST
    {

        public bool IsSL_MWG02_REQUIRED
        {
            get { return SL_MWG02_REQUIRED == "Y"; }
            set { SL_MWG02_REQUIRED = value ? "Y" : "N"; }
        }

        public bool IsCHANGE_PREVIOUS_FORM_STATUS
        {
            get { return CHANGE_PREVIOUS_FORM_STATUS == "Y"; }
            set { CHANGE_PREVIOUS_FORM_STATUS = value ? "Y" : "N"; }
        }

    }
    public partial class P_MW_SUMMARY_MW_ITEM_CHECKLIST_Extend
    {
    }
}