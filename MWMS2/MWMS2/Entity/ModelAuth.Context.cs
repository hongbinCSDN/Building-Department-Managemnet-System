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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class EntitiesAuth : EntityFilter
    {
        public EntitiesAuth()
            : base("name=EntitiesAuth")
        {
            Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<SYS_FUNC> SYS_FUNC { get; set; }
        public virtual DbSet<SYS_POST> SYS_POST { get; set; }
        public virtual DbSet<SYS_POST_AREA> SYS_POST_AREA { get; set; }
        public virtual DbSet<SYS_POST_ROLE> SYS_POST_ROLE { get; set; }
        public virtual DbSet<SYS_RANK> SYS_RANK { get; set; }
        public virtual DbSet<SYS_ROLE> SYS_ROLE { get; set; }
        public virtual DbSet<SYS_ROLE_FUNC> SYS_ROLE_FUNC { get; set; }
        public virtual DbSet<SYS_SECTION> SYS_SECTION { get; set; }
        public virtual DbSet<SYS_UNIT> SYS_UNIT { get; set; }
        public virtual DbSet<SYS_USER> SYS_USER { get; set; }
        public virtual DbSet<SYS_LOG> SYS_LOG { get; set; }
    }
}
