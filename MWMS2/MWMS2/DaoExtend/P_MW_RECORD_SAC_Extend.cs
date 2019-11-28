using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWMS2.Entity
{
    [MetadataType(typeof(P_MW_RECORD_SAC_Extend))]
    public partial class P_MW_RECORD_SAC
    {
        public bool IsCHK_COMMENCEMENT
        {
            get { return CHK_COMMENCEMENT == "Y"; }
            set { CHK_COMMENCEMENT = value ? "Y" : "N"; }
        }
        public bool IsCHK_COMPLETION
        {
            get { return CHK_COMPLETION == "Y"; }
            set { CHK_COMPLETION = value ? "Y" : "N"; }
        }
        public bool IsACTION_TAKEN_CHK_NA
        {
            get { return ACTION_TAKEN_CHK_NA == "Y"; }
            set { ACTION_TAKEN_CHK_NA = value ? "Y" : "N"; }
        }
        public bool IsACTION_TAKEN_CHK_INFORM
        {
            get { return ACTION_TAKEN_CHK_INFORM == "Y"; }
            set { ACTION_TAKEN_CHK_INFORM = value ? "Y" : "N"; }
        }
        public bool IsACTION_TAKEN_CHK_INFORM_PBP
        {
            get { return ACTION_TAKEN_CHK_INFORM_PBP == "Y"; }
            set { ACTION_TAKEN_CHK_INFORM_PBP = value ? "Y" : "N"; }
        }
        public bool IsACTION_TAKEN_CHK_INFORM_PRC
        {
            get { return ACTION_TAKEN_CHK_INFORM_PRC == "Y"; }
            set { ACTION_TAKEN_CHK_INFORM_PRC = value ? "Y" : "N"; }
        }
        public bool IsACTION_TAKEN_CHK_INFORM_PHONE
        {
            get { return ACTION_TAKEN_CHK_INFORM_PHONE == "Y"; }
            set { ACTION_TAKEN_CHK_INFORM_PHONE = value ? "Y" : "N"; }
        }
        public bool IsACTION_TAKEN_CHK_INFORM_LETTER
        {
            get { return ACTION_TAKEN_CHK_INFORM_LETTER == "Y"; }
            set { ACTION_TAKEN_CHK_INFORM_LETTER = value ? "Y" : "N"; }
        }
        public bool IsACTION_TAKEN_CHK_IRR
        {
            get { return ACTION_TAKEN_CHK_IRR == "Y"; }
            set { ACTION_TAKEN_CHK_IRR = value ? "Y" : "N"; }
        }
        public bool IsACTION_TAKEN_CHK_IRR_PBP
        {
            get { return ACTION_TAKEN_CHK_IRR_PBP == "Y"; }
            set { ACTION_TAKEN_CHK_IRR_PBP = value ? "Y" : "N"; }
        }
        public bool IsACTION_TAKEN_CHK_IRR_PRC
        {
            get { return ACTION_TAKEN_CHK_IRR_PRC == "Y"; }
            set { ACTION_TAKEN_CHK_IRR_PRC = value ? "Y" : "N"; }
        }
        public bool IsACTION_TAKEN_CHK_IRR_COMPLETE
        {
            get { return ACTION_TAKEN_CHK_IRR_COMPLETE == "Y"; }
            set { ACTION_TAKEN_CHK_IRR_COMPLETE = value ? "Y" : "N"; }
        }
        public bool IsACTION_TAKEN_CHK_IRR_PARTLY
        {
            get { return ACTION_TAKEN_CHK_IRR_PARTLY == "Y"; }
            set { ACTION_TAKEN_CHK_IRR_PARTLY = value ? "Y" : "N"; }
        }
        public bool IsIRR_NOT_RECTIFIED_CHK_NA
        {
            get { return IRR_NOT_RECTIFIED_CHK_NA == "Y"; }
            set { IRR_NOT_RECTIFIED_CHK_NA = value ? "Y" : "N"; }
        }
        public bool IsIRR_NOT_RECTIFIED_CHK_ID
        {
            get { return IRR_NOT_RECTIFIED_CHK_ID == "Y"; }
            set { IRR_NOT_RECTIFIED_CHK_ID = value ? "Y" : "N"; }
        }
        public bool IsIRR_NOT_RECTIFIED_CHK_NON_ID
        {
            get { return IRR_NOT_RECTIFIED_CHK_NON_ID == "Y"; }
            set { IRR_NOT_RECTIFIED_CHK_NON_ID = value ? "Y" : "N"; }
        }
        public bool IsIRR_NOT_RECTIFIED_CHK_OTHER
        {
            get { return IRR_NOT_RECTIFIED_CHK_OTHER == "Y"; }
            set { IRR_NOT_RECTIFIED_CHK_OTHER = value ? "Y" : "N"; }
        }
        public bool IsIRR_NOT_RECTIFIED_CHK_MW
        {
            get { return IRR_NOT_RECTIFIED_CHK_MW == "Y"; }
            set { IRR_NOT_RECTIFIED_CHK_MW = value ? "Y" : "N"; }
        }
        public bool IsIRR_NOT_RECTIFIED_CHK_MW_DISCIPLINARY
        {
            get { return IRR_NOT_RECTIFIED_CHK_MW_DISCIPLINARY == "Y"; }
            set { IRR_NOT_RECTIFIED_CHK_MW_DISCIPLINARY = value ? "Y" : "N"; }
        }
        public bool IsIRR_NOT_RECTIFIED_CHK_MW_PBP
        {
            get { return IRR_NOT_RECTIFIED_CHK_MW_PBP == "Y"; }
            set { IRR_NOT_RECTIFIED_CHK_MW_PBP = value ? "Y" : "N"; }
        }
        public bool IsIRR_NOT_RECTIFIED_CHK_MW_PRC
        {
            get { return IRR_NOT_RECTIFIED_CHK_MW_PRC == "Y"; }
            set { IRR_NOT_RECTIFIED_CHK_MW_PRC = value ? "Y" : "N"; }
        }
        public bool IsIRR_NOT_RECTIFIED_CHK_MW_PROSECUTION
        {
            get { return IRR_NOT_RECTIFIED_CHK_MW_PROSECUTION == "Y"; }
            set { IRR_NOT_RECTIFIED_CHK_MW_PROSECUTION = value ? "Y" : "N"; }
        }
        public bool IsIRR_NOT_RECTIFIED_CHK_OS_IRR
        {
            get { return IRR_NOT_RECTIFIED_CHK_OS_IRR == "Y"; }
            set { IRR_NOT_RECTIFIED_CHK_OS_IRR = value ? "Y" : "N"; }
        }
        public bool IsIRR_NOT_RECTIFIED_CHK_OS_DISCIPLINARY
        {
            get { return IRR_NOT_RECTIFIED_CHK_OS_DISCIPLINARY == "Y"; }
            set { IRR_NOT_RECTIFIED_CHK_OS_DISCIPLINARY = value ? "Y" : "N"; }
        }
        public bool IsIRR_NOT_RECTIFIED_CHK_OS_PBP
        {
            get { return IRR_NOT_RECTIFIED_CHK_OS_PBP == "Y"; }
            set { IRR_NOT_RECTIFIED_CHK_OS_PBP = value ? "Y" : "N"; }
        }
        public bool IsIRR_NOT_RECTIFIED_CHK_OS_PRC
        {
            get { return IRR_NOT_RECTIFIED_CHK_OS_PRC == "Y"; }
            set { IRR_NOT_RECTIFIED_CHK_OS_PRC = value ? "Y" : "N"; }
        }
        public bool IsIRR_NOT_RECTIFIED_CHK_OS_PROSECUTION
        {
            get { return IRR_NOT_RECTIFIED_CHK_OS_PROSECUTION == "Y"; }
            set { IRR_NOT_RECTIFIED_CHK_OS_PROSECUTION = value ? "Y" : "N"; }
        }

    }
    public partial class P_MW_RECORD_SAC_Extend
    {

    }
}