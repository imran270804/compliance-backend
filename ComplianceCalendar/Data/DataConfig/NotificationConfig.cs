using ComplianceCalendar.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComplianceCalendar.Data.DataConfig
{
    public class NotificationConfig : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder
                .HasKey(n => n.Id);

            builder
                .HasOne(f => f.Employee)
                .WithMany()
                .HasForeignKey(f => f.EmpId);
        }
    }
}
