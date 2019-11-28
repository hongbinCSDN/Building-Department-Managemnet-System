using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Entity
{
    [MetadataType(typeof(P_MW_FORM_Extend))]
    public partial class P_MW_FORM
    {
        public bool IsINVOLVE_SIGNBOARD
        {
            get { return INVOLVE_SIGNBOARD == "Y"; }
            set { INVOLVE_SIGNBOARD = value ? "Y" : "N"; }
        }

        public bool IsFORM06_A_4_AP
        {
            get { return FORM06_A_4_AP == "Y"; }
            set { FORM06_A_4_AP = value ? "Y" : "N"; }
        }

        public bool IsFORM06_A_4_RI
        {
            get { return FORM06_A_4_RI == "Y"; }
            set { FORM06_A_4_RI = value ? "Y" : "N"; }
        }

        public bool IsFORM06_A_4_RSE
        {
            get { return FORM06_A_4_RSE == "Y"; }
            set { FORM06_A_4_RSE = value ? "Y" : "N"; }
        }

        public bool IsFORM06_A_4_RGBC
        {
            get { return FORM06_A_4_RGBC == "Y"; }
            set { FORM06_A_4_RGBC = value ? "Y" : "N"; }
        }

        public bool IsFORM06_A_4_RMWC
        {
            get { return FORM06_A_4_RMWC == "Y"; }
            set { FORM06_A_4_RMWC = value ? "Y" : "N"; }
        }

        public bool IsFORM06_A_5_COMPLETED_MENTION
        {
            get { return FORM06_A_5_COMPLETED_MENTION == "Y"; }
            set { FORM06_A_5_COMPLETED_MENTION = value ? "Y" : "N"; }
        }

        public bool IsFORM06_A_5_IDENTICAL
        {
            get { return FORM06_A_5_IDENTICAL == "Y"; }
            set { FORM06_A_5_IDENTICAL = value ? "Y" : "N"; }
        }

        public bool IsFORM06_A_1_INVOLVE
        {
            get { return FORM06_A_1_INVOLVE == "Y"; }
            set { FORM06_A_1_INVOLVE = value ? "Y" : "N"; }
        }

        public bool IsFORM06_B_1_AP
        {
            get { return FORM06_B_1_AP == "Y"; }
            set { FORM06_B_1_AP = value ? "Y" : "N"; }
        }

        public bool IsFORM06_B_1_RI
        {
            get { return FORM06_B_1_RI == "Y"; }
            set { FORM06_B_1_RI = value ? "Y" : "N"; }
        }

        public bool IsFORM06_B_1_RSE
        {
            get { return FORM06_B_1_RSE == "Y"; }
            set { FORM06_B_1_RSE = value ? "Y" : "N"; }
        }

        public bool IsFORM06_B_1_RGBC
        {
            get { return FORM06_B_1_RGBC == "Y"; }
            set { FORM06_B_1_RGBC = value ? "Y" : "N"; }
        }

        public bool IsFORM06_B_1_RMWC
        {
            get { return FORM06_B_1_RMWC == "Y"; }
            set { FORM06_B_1_RMWC = value ? "Y" : "N"; }
        }

        public bool IsFORM07_A_S27
        {
            get { return FORM07_A_S27 == "Y"; }
            set { FORM07_A_S27 = value ? "Y" : "N"; }
        }

        public bool IsFORM09_A_AP
        {
            get { return FORM09_A_AP == "Y"; }
            set { FORM09_A_AP = value ? "Y" : "N"; }
        }

        public bool IsFORM09_A_RI
        {
            get { return FORM09_A_RI == "Y"; }
            set { FORM09_A_RI = value ? "Y" : "N"; }
        }

        public bool IsFORM09_A_RSE
        {
            get { return FORM09_A_RSE == "Y"; }
            set { FORM09_A_RSE = value ? "Y" : "N"; }
        }

        public bool IsFORM09_A_RGE
        {
            get { return FORM09_A_RGE == "Y"; }
            set { FORM09_A_RGE = value ? "Y" : "N"; }
        }

        public bool IsFORM09_REASON_ILL
        {
            get { return FORM09_REASON_ILL == "Y"; }
            set { FORM09_REASON_ILL = value ? "Y" : "N"; }
        }

        public bool IsFORM09_REASON_ABSENCE
        {
            get { return FORM09_REASON_ABSENCE == "Y"; }
            set { FORM09_REASON_ABSENCE = value ? "Y" : "N"; }
        }

        public bool IsFORM09_FURTHER_NOTICE
        {
            get { return FORM09_FURTHER_NOTICE == "Y"; }
            set { FORM09_FURTHER_NOTICE = value ? "Y" : "N"; }
        }

        public bool IsFORM11_E_SAME_DATE
        {
            get { return FORM11_E_SAME_DATE == "Y"; }
            set { FORM11_E_SAME_DATE = value ? "Y" : "N"; }
        }

        public bool IsFORM11_E_NEW_DATE
        {
            get { return FORM11_E_NEW_DATE == "Y"; }
            set { FORM11_E_NEW_DATE = value ? "Y" : "N"; }
        }

        public bool IsFORM31_A_AP
        {
            get { return FORM31_A_AP == "Y"; }
            set { FORM31_A_AP = value ? "Y" : "N"; }
        }

        public bool IsFORM31_A_RI
        {
            get { return FORM31_A_RI == "Y"; }
            set { FORM31_A_RI = value ? "Y" : "N"; }
        }

        public bool IsFORM31_A_RSE
        {
            get { return FORM31_A_RSE == "Y"; }
            set { FORM31_A_RSE = value ? "Y" : "N"; }
        }

        public bool IsFORM31_A_RGE
        {
            get { return FORM31_A_RGE == "Y"; }
            set { FORM31_A_RGE = value ? "Y" : "N"; }
        }

        public bool IsFORM33_DEC_1
        {
            get { return FORM33_DEC_1 == "Y"; }
            set { FORM33_DEC_1 = value ? "Y" : "N"; }
        }
        public bool IsFORM33_DEC_2
        {
            get { return FORM33_DEC_2 == "Y"; }
            set { FORM33_DEC_2 = value ? "Y" : "N"; }
        }

        public bool IsFORM33_DEC_3
        {
            get { return FORM33_DEC_3 == "Y"; }
            set { FORM33_DEC_3 = value ? "Y" : "N"; }
        }

        public bool IsFORM33_DEC_4
        {
            get { return FORM33_DEC_4 == "Y"; }
            set { FORM33_DEC_4 = value ? "Y" : "N"; }
        }

        public bool IsFORM33_DEC_5
        {
            get { return FORM33_DEC_5 == "Y"; }
            set { FORM33_DEC_5 = value ? "Y" : "N"; }
        }

        public bool IsFORM33_DEC_6
        {
            get { return FORM33_DEC_6 == "Y"; }
            set { FORM33_DEC_6 = value ? "Y" : "N"; }
        }

        public bool IsFORM33_DEC_7
        {
            get { return FORM33_DEC_7 == "Y"; }
            set { FORM33_DEC_7 = value ? "Y" : "N"; }
        }

        public bool IsFORM33_DEC_8
        {
            get { return FORM33_DEC_8 == "Y"; }
            set { FORM33_DEC_8 = value ? "Y" : "N"; }
        }

        public bool IsFORM33_DEC_9
        {
            get { return FORM33_DEC_9 == "Y"; }
            set { FORM33_DEC_9 = value ? "Y" : "N"; }
        }

        public bool IsFORM33_DEC_10
        {
            get { return FORM33_DEC_10 == "Y"; }
            set { FORM33_DEC_10 = value ? "Y" : "N"; }
        }

        public bool IsFORM33_DEC_11
        {
            get { return FORM33_DEC_11 == "Y"; }
            set { FORM33_DEC_11 = value ? "Y" : "N"; }
        }

        public bool IsFORM33_DEC_12
        {
            get { return FORM33_DEC_12 == "Y"; }
            set { FORM33_DEC_12 = value ? "Y" : "N"; }
        }

        public bool IsFORM33_AP
        {
            get { return FORM33_AP == "Y"; }
            set { FORM33_AP = value ? "Y" : "N"; }
        }

        public bool IsFORM33_PRC
        {
            get { return FORM33_PRC == "Y"; }
            set { FORM33_PRC = value ? "Y" : "N"; }
        }


    }
    public class P_MW_FORM_Extend
    {
    }
}