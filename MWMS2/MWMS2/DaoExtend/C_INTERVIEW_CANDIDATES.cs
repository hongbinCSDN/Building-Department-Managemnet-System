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

    [MetadataType(typeof(C_INTERVIEW_CANDIDATES_Extend))] public partial class C_INTERVIEW_CANDIDATES
    {
        public string START_DATE_DISPLAY_TIME
        {
            get
            {
                 return this.START_DATE?.ToString("hh:mm tt");
               
            }
        }
        private string _INTERVIEW_TYPE_DISPLAY;
        public string INTERVIEW_TYPE_DISPLAY
        {
            get
            {
                if (_INTERVIEW_TYPE_DISPLAY != null) return _INTERVIEW_TYPE_DISPLAY;
                String interviewType = this.INTERVIEW_TYPE;
                String result = "";
                if (!String.IsNullOrEmpty(interviewType))
                {
                    if ("I".Equals(interviewType))
                    {
                        result = "Interview";
                    }
                    if ("A".Equals(interviewType))
                    {
                        result = "Assessment";
                    }
                }

                return result;
            }
            set
            {
                _INTERVIEW_TYPE_DISPLAY = value;
            }
        }

        //public string Duration
        //{
        //    get
        //    {
        //        DateTime? startDate = this.START_DATE;
        //        DateTime? endDate = this.END_DATE;

        //       //Duration TimeDuration = (endDate - startDate).TotalMinutes;

        //        //return TimeDuration;
        //    }
        //}
    }

    public partial class C_INTERVIEW_CANDIDATES_Extend
    {

    }
}