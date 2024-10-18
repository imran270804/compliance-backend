using ComplianceCalendar.Data.DataConfig;
using ComplianceCalendar.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace ComplianceCalendar.Data
{
    public class APIContext : DbContext
    {
        public APIContext(DbContextOptions<APIContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Filings> Filings { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<UserFilings> UserFilings { get; set; }
        public DbSet<Documents> Documents { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<AssignedTo> AssignedToDept { get; set; }
        public DbSet<ActiveDirectory> ActiveDirectory { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Filings>().ToTable("filings");
            modelBuilder.Entity<Employee>().ToTable("employees");
            modelBuilder.Entity<UserFilings>().ToTable("userfilings");
            modelBuilder.Entity<Documents>().ToTable("documents");
            modelBuilder.Entity<Department>().ToTable("departments");
            modelBuilder.Entity<AssignedTo>().ToTable("assignedto");
            modelBuilder.Entity<Roles>().ToTable("roles");
            modelBuilder.Entity<ActiveDirectory>().ToTable("activedirectory");
            modelBuilder.Entity<Notification>().ToTable("notifications");

            modelBuilder.ApplyConfiguration(new FilingsConfig());
            modelBuilder.ApplyConfiguration(new UserFilingsConfig());
            modelBuilder.ApplyConfiguration(new DocumentConfig());
            modelBuilder.ApplyConfiguration(new AssignedToConfig());
            modelBuilder.ApplyConfiguration(new DepartmentConfig());
            modelBuilder.ApplyConfiguration(new NotificationConfig());

            base.OnModelCreating(modelBuilder);
        }
    }
}
