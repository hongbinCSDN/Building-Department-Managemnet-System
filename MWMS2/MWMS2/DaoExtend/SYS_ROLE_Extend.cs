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
    using System.Linq;
    using System.Reflection;

    [MetadataType(typeof(SYS_ROLE_Extend))] public partial class SYS_ROLE
    {
        public Dictionary<string, object> ToDictionary()
        {
            PropertyInfo[] i = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            return this.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(o=>o.PropertyType.Namespace != "System.Collections.Generic")
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(this, null));
        }
    }

    public partial class SYS_ROLE_Extend
    {

    }
}