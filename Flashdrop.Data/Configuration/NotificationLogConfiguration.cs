using Flashdrop.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flashdrop.Data.Configuration;

public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
{
    public void Configure(EntityTypeBuilder<NotificationLog> builder)
    {
        builder.HasKey(nl => nl.Id);

        builder.Property(nl => nl.OrderId)
            .IsRequired();

        builder.Property(nl => nl.Channel)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(nl => nl.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(nl => nl.Attempts)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(nl => nl.LastAttemptAt)
            .IsRequired();

        // Indexes for notification auditing and DLQ tracking
        builder.HasIndex(nl => nl.OrderId);
        builder.HasIndex(nl => new { nl.Status, nl.LastAttemptAt })
            .HasDatabaseName("IX_NotificationLog_Status_LastAttemptAt");

        // Relationships
        builder.HasOne(nl => nl.Order)
            .WithMany(o => o.NotificationLogs)
            .HasForeignKey(nl => nl.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
