using ComplianceCalendar.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace ComplianceCalendar.Data.DataConfig
{
    public class FilingsConfig : IEntityTypeConfiguration<Filings>
    {
        void IEntityTypeConfiguration<Filings>.Configure(EntityTypeBuilder<Filings> builder)
        {
            builder
                .HasOne(f => f.CreatedBy)
                .WithMany()
                .HasForeignKey(f => f.CreatedById);
        }
    }
}
