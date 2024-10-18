using ComplianceCalendar.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace ComplianceCalendar.Data.DataConfig
{
    public class UserFilingsConfig : IEntityTypeConfiguration<UserFilings>
    {
        public void Configure(EntityTypeBuilder<UserFilings> builder)
        {
            builder
                .HasOne(uf => uf.Filings)
                .WithMany()
                .HasForeignKey(uf => uf.FilingId);

            builder
                .HasOne(uf => uf.Employee)
                .WithMany()
                .HasForeignKey(uf => uf.EmployeeId);
        }
    }
}
