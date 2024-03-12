using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Demo.Data.Models;

namespace Demo.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<EquipmentType> EquipmentTypes { get; set; }
        public DbSet<ProblemType> ProblemTypes { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Status> Statuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Application>().ToTable("application");
            modelBuilder.Entity<EquipmentType>().ToTable("equipment");
            modelBuilder.Entity<ProblemType>().ToTable("problem");
            modelBuilder.Entity<Role>().ToTable("role");
            modelBuilder.Entity<Status>().ToTable("workstatus");
            base.OnModelCreating(modelBuilder);
        }
    }
}
