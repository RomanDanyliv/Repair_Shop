﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kursova
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RepairEntities : DbContext
    {
        public RepairEntities()
            : base("name=RepairEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Addresses> Addresses { get; set; }
        public virtual DbSet<Branches> Branches { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Employees> Employees { get; set; }
        public virtual DbSet<Need_Parts> Need_Parts { get; set; }
        public virtual DbSet<Parts_On_Branch> Parts_On_Branch { get; set; }
        public virtual DbSet<Requests> Requests { get; set; }
        public virtual DbSet<Spare_parts> Spare_parts { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Work_Types> Work_Types { get; set; }
    }
}
