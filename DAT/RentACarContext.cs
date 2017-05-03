using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using RentACar.Models;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace RentACar.DAT
{
    public class RcDbContext : DbContext
    {
        public RcDbContext() : base("LocalConnection")
        {

        }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<CarEquipment> CarEquipment { get; set; }
        public DbSet<AppParam> AppParams { get; set; }
        public DbSet<AppParamValues> AppParamValues { get; set; }
        public DbSet<Photos> Photos { get; set; }
        public DbSet<Fuel> Fuel { get; set; }
        public DbSet<VehicleLocation> VehicleLocation { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Cascade delete convention is removed for entire context
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            // TPC - mapping (Table-Per-Concrete)               
            modelBuilder.Entity<Vehicle>().Map(u =>
            {
                // This table will have columns with inherited and its own properties. On hover for more information.
                u.MapInheritedProperties();
                u.ToTable("Vehicle");
            });

            // TPC - mapping (Table-Per-Concrete)               
            modelBuilder.Entity<Equipment>().Map(u =>
            {
                // This table will have columns with inherited and its own properties. On hover for more information.
                u.MapInheritedProperties();
                u.ToTable("Equipment");
            });

            // TPC - mapping (Table-Per-Concrete)               
            modelBuilder.Entity<AppParam>().Map(u =>
            {
                // This table will have columns with inherited and its own properties. On hover for more information.
                u.MapInheritedProperties();
                u.ToTable("AppParam");
            });

            // TPC - mapping (Table-Per-Concrete)               
            modelBuilder.Entity<AppParamValues>().Map(u =>
            {
                // This table will have columns with inherited and its own properties. On hover for more information.
                u.MapInheritedProperties();
                u.ToTable("AppParamValue");
            });

            // TPC - mapping (Table-Per-Concrete)               
            modelBuilder.Entity<Photos>().Map(u =>
            {
                // This table will have columns with inherited and its own properties. On hover for more information.
                u.MapInheritedProperties();
                u.ToTable("Photo");
            });

            // TPC - mapping (Table-Per-Concrete)               
            modelBuilder.Entity<Fuel>().Map(u =>
            {
                // This table will have columns with inherited and its own properties. On hover for more information.
                u.MapInheritedProperties();
                u.ToTable("Fuel");
            });

            // TPC - mapping (Table-Per-Concrete)               
            modelBuilder.Entity<VehicleLocation>().Map(u =>
            {
                // This table will have columns with inherited and its own properties. On hover for more information.
                u.MapInheritedProperties();
                u.ToTable("VehicleLocation");
            });

            // TPC - mapping (Table-Per-Concrete)               
            modelBuilder.Entity<Order>().Map(u =>
            {
                // This table will have columns with inherited and its own properties. On hover for more information.
                u.MapInheritedProperties();
                u.ToTable("Order");
            });
        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                // if entry is derived from EntityBaseClass. Some models are not derived like AppParamValues
                // thus they don't have CreatedDate, ModifiedDate columns.
                if (entry.Entity is EntityBaseClass)
                {
                    EntityBaseClass entity = (EntityBaseClass)entry.Entity;

                    if (entry.State == EntityState.Added)
                    {
                        entity.CreatedDate = DateTime.Now;
                        entity.ModifiedDate = DateTime.Now;
                        if (entity.Status == default(int))
                        {
                            // If status wasn't set before
                            entity.Status = EntityStatus.New;
                        }                        
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        if (entity.SetDates == true)
                        {
                            entity.ModifiedDate = DateTime.Now;
                            // entity.Status = EntityStatus.Active;
                        }
                    }
                    else if (entry.State == EntityState.Deleted)
                    {
                        entity.ModifiedDate = DateTime.Now;
                        entity.Status = EntityStatus.Archived;
                    }
                }
                // todo: add logging for each operation. See http://www.entityframeworktutorial.net/change-tracking-in-entity-framework.aspx for code example

            }

            return base.SaveChanges();
        }

        private void SetEntityDates<T, U>(T entity, U entry) where T : EntityBaseClass where U : System.Data.Entity.Infrastructure.DbEntityEntry
        {
            entity = (T)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedDate = DateTime.Now;
                entity.ModifiedDate = DateTime.Now;
                entity.Status = EntityStatus.Active;
            }
            else if (entry.State == EntityState.Modified)
            {
                entity.ModifiedDate = DateTime.Now;
                entity.Status = EntityStatus.Active;
            }
        }
    }
}