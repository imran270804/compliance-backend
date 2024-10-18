using ComplianceCalendar.Models;
using Microsoft.EntityFrameworkCore;

namespace ComplianceCalendar.Data
{
    public class ManageUserContext : DbContext
    {
        public ManageUserContext(DbContextOptions<ManageUserContext> dbContextOptions) : base(dbContextOptions) 
        {
            
        }

        public DbSet<Employee> Employee { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Roles> Roles { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Employee>().ToTable("employees");
            modelBuilder.Entity<Department>().ToTable("departments");
            modelBuilder.Entity<Roles>().ToTable("roles");

            modelBuilder.Entity<Department>()
                .HasKey(de => de.Id);
            
            modelBuilder.Entity<Employee>()
                .HasKey(e => e.EmployeeId);
            base.OnModelCreating(modelBuilder);
        }

    }

}


/*namespace ComplianceCalendar.Data
{
    public class APIContext : DbContext
    {
        public APIContext(DbContextOptions<APIContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Filings> Filings { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<UserFilings> UserFilings { get; set; }
        public DbSet<Documents> Documents { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Filings>().ToTable("filings");
            modelBuilder.Entity<Employee>().ToTable("employees");
            modelBuilder.Entity<UserFilings>().ToTable("userfilings");
            modelBuilder.Entity<Documents>().ToTable("documents");
            modelBuilder.Entity<Department>().ToTable("departments");

            modelBuilder.Entity<Filings>()
                .HasOne(f => f.CreatedBy)
                .WithMany()
                .HasForeignKey(f => f.CreatedById);

            modelBuilder.Entity<UserFilings>()
                .HasOne(uf => uf.Filings)
                .WithMany()
                .HasForeignKey(uf => uf.FilingId);

            modelBuilder.Entity<UserFilings>()
                .HasOne(uf => uf.Employee)
                .WithMany()
                .HasForeignKey(uf => uf.EmployeeId);

            modelBuilder.Entity<Documents>()
                .HasOne(d => d.Filings)
                .WithMany()
                .HasForeignKey(d => d.FilingId);

            modelBuilder.Entity<Department>()
                .HasKey(de => de.Id);

            modelBuilder.Entity<Employee>()
                .HasKey(e => e.EmployeeId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
*/