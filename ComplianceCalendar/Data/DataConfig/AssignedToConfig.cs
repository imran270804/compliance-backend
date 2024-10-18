using ComplianceCalendar.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace ComplianceCalendar.Data.DataConfig
{
    public class AssignedToConfig : IEntityTypeConfiguration<AssignedTo>
    {
        public void Configure(EntityTypeBuilder<AssignedTo> builder)
        {
            builder
                .HasKey(at => at.Id);

            builder
                .HasOne(at => at.Employees)
                .WithMany()
                .HasForeignKey(at => at.EmpId);

            builder
                .HasOne(at => at.Departments)
                .WithMany()
                .HasForeignKey(uf => uf.DepartmentId);
        }
    }
}
