using Flashdrop.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flashdrop.Data.Configuration;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(om => om.Id);

        builder.Property(om => om.Type)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(om => om.Payload)
            .IsRequired();

        builder.Property(om => om.CreatedAt)
            .IsRequired();

        builder.Property(om => om.ProcessedAt)
            .IsRequired(false);

        builder.Property(om => om.RetryCount)
            .HasDefaultValue(0);

        builder.Property(om => om.OrderId)
            .IsRequired(false);

        // Index for the background publisher to find unprocessed messages
        builder.HasIndex(om => new { om.ProcessedAt, om.RetryCount })
            .HasDatabaseName("IX_OutboxMessage_ProcessedAt_RetryCount");

        // Index for ordering by creation time
        builder.HasIndex(om => om.CreatedAt);

        // Relationships
        builder.HasOne(om => om.Order)
            .WithMany(o => o.OutboxMessages)
            .HasForeignKey(om => om.OrderId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
