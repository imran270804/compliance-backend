using Microsoft.EntityFrameworkCore;
using ComplianceCalendar.Models;

namespace ComplianceCalendar.Data
{
    public class AdminContext : DbContext
    {
        public AdminContext(DbContextOptions<AdminContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<AssignedTo> AssignedTo { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("employees");


            modelBuilder.Entity<Department>().ToTable("departments");
            modelBuilder.Entity<AssignedTo>().ToTable("assignedto");

            modelBuilder.Entity<Department>()
                .HasKey(de => de.Id);

            modelBuilder.Entity<AssignedTo>()
                .HasOne(at => at.Employees)
                .WithMany()
                .HasForeignKey(at => at.EmpId);

            modelBuilder.Entity<AssignedTo>()
                .HasOne(at => at.Departments)
                .WithMany()
                .HasForeignKey(uf => uf.DepartmentId);

            modelBuilder.Entity<AssignedTo>()
                .HasKey(at => at.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
