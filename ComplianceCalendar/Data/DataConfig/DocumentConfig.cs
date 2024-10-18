using ComplianceCalendar.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace ComplianceCalendar.Data.DataConfig
{
    public class DocumentConfig : IEntityTypeConfiguration<Documents>
    {
        public void Configure(EntityTypeBuilder<Documents> builder)
        {
            builder
                .HasOne(d => d.Filings)
                .WithMany()
                .HasForeignKey(d => d.FilingId);
        }
    }
}
