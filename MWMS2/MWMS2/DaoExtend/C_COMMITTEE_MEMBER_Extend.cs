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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    [MetadataType(typeof(C_COMMITTEE_MEMBER_Extend))]
    public partial class C_COMMITTEE_MEMBER
    {
    }
    public partial class C_COMMITTEE_MEMBER_Extend
    {


        [Display(Name = "C/o")]
        public string ENGLISH_CARE_OF { get; set; }
        [Display(Name = "C/o (Chn)")]
        public string CHINESE_CARE_OF { get; set; }
        [Display(Name = "Telephone Nos.")]
        public string TELEPHONE_NO1 { get; set; }
        [Display(Name = "Fax Nos.")]
        public string FAX_NO1 { get; set; }
        [Display(Name = "Email")]
        public string EMAIL { get; set; }
        [Display(Name = "Rank")]
        public string RANK { get; set; }
        [Display(Name = "Post")]
        public string POST { get; set; }
        [Display(Name = "Career")]
        public string CAREER { get; set; }



        /*
        public string UUID { get; set; }
        public string APPLICANT_ID { get; set; }
        public string TELEPHONE_NO2 { get; set; }
        public string TELEPHONE_NO3 { get; set; }
        public string FAX_NO2 { get; set; }
        public string ENGLISH_ADDRESS_ID { get; set; }
        public string CHINESE_ADDRESS_ID { get; set; }
        public string REGISTRATION_TYPE { get; set; }
        public string CREATED_BY { get; set; }
        public System.DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public System.DateTime MODIFIED_DATE { get; set; }*/

    }
}
