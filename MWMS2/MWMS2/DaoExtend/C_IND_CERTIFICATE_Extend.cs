﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MWMS2.Entity
{
    using MWMS2.Utility;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(C_IND_CERTIFICATE_Extend))] public partial class C_IND_CERTIFICATE
    {
        public C_INTERVIEW_CANDIDATES C_INTERVIEW_CANDIDATES { get; set; }
        private int _TIMEDURATION = -1;
        public int TIMEDURATION
        {
            get
            {
                return C_INTERVIEW_CANDIDATES == null ? 0 : _TIMEDURATION < 0 ? (C_INTERVIEW_CANDIDATES.END_DATE - C_INTERVIEW_CANDIDATES.START_DATE).Value.Minutes : _TIMEDURATION;
            }
            set { _TIMEDURATION = value; }
        }

        [Display(Name = "Date of Expiry")]
        public string EXPIRY_DATE_DISPLAY
        {
            get
            {
                if (REMOVAL_DATE != null) return "-";
                else return EXPIRY_DATE.ToString();
            }
        }


        [Display(Name = "Registration Status")]
        public string REGISTRATION_STATUS_DISPLAY
        {
            get
            {
                DateTime now = DateTime.Now;
                string v = "";
                if (this.EXPIRY_DATE == null
                    || (this.EXPIRY_DATE != null && this.RESTORE_DATE != null && this.REMOVAL_DATE == null
                    && CommonUtil.getDayDiff(this.RESTORE_DATE, this.EXPIRY_DATE) > 0))
                { v = "Application in progress"; }
                else if ((this.REMOVAL_DATE != null
                    && CommonUtil.getDayDiff(this.REMOVAL_DATE, now) > 0
                    && CommonUtil.getDayDiff(this.EXPIRY_DATE, now) >= 0
                    && this.RETENTION_DATE == null)
                    || (this.REMOVAL_DATE == null
                    && (CommonUtil.getDayDiff(this.EXPIRY_DATE, now) >= 0
                    || (CommonUtil.getDayDiff(this.EXPIRY_DATE, now) < 0
                    && CommonUtil.getDayDiff(this.RETENTION_DATE, CommonUtil.D20040831) > 0)))
                    )
                { v = "Currently on the register"; }
                else if ((CommonUtil.getDayDiff(this.EXPIRY_DATE, now) < 0
                    && this.REMOVAL_DATE == null
                    && this.RETENTION_DATE == null)
                    || (CommonUtil.getDayDiff(this.EXPIRY_DATE, now) < 0
                    && this.REMOVAL_DATE == null
                    && CommonUtil.getDayDiff(this.RETENTION_DATE, CommonUtil.D20040831) < 0)
                    || (this.REMOVAL_DATE != null
                    && CommonUtil.getDayDiff(this.REMOVAL_DATE, this.EXPIRY_DATE) < 0)
                    || (this.REMOVAL_DATE != null
                    && CommonUtil.getDayDiff(this.REMOVAL_DATE, now) < 0))
                { v = "Registration Expired/Removed"; }
                else if (CommonUtil.getDayDiff(this.EXPIRY_DATE, now) < 0 && this.RETENTION_DATE == null)
                { v = "Registration Expired/Removed"; }
                else if (this.REMOVAL_DATE != null
                    && (CommonUtil.getDayDiff(this.EXPIRY_DATE, now) >= 0
                    || (CommonUtil.getDayDiff(this.EXPIRY_DATE, now) < 0
                    && CommonUtil.getDayDiff(this.RETENTION_DATE, CommonUtil.D20040831) > 0)))
                {
                    if (CommonUtil.getDayDiff(this.REMOVAL_DATE, now) > 0) { v = "Currently on the register"; }
                    else v = "Registration Expired/Removed";
                }
                return v;
            }
        }


        

    }

    public partial class C_IND_CERTIFICATE_Extend
    {

    }
}