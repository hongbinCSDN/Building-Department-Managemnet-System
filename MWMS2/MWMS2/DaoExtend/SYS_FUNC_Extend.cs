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

    [MetadataType(typeof(SYS_FUNC_Extend))] public partial class SYS_FUNC
    {
        public List<SYS_FUNC> Childs { get; set; } = new List<SYS_FUNC>();
        public SYS_FUNC Parent { get; set; }
        public string[] FunctionSplit { get {
                return string.IsNullOrWhiteSpace(URL) ? null : URL.Replace('/' ,' ').Trim().Split(' ');
            }
        }
        public string URL_TRUN
        {
            get
            {
                return string.IsNullOrWhiteSpace(URL) ? null : "/" + (string.Join("/", FunctionSplit));
            }
        }
    }


    public partial class SYS_FUNC_Extend
    {

    }
}