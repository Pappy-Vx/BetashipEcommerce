using BetashipEcommerce.CORE.Auditing;
using BetashipEcommerce.CORE.Auditing.ValueObjects;
using BetashipEcommerce.CORE.Identity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.Data.Configurations
{
    internal sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs", "ecommerce");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .HasConversion(
                    id => id.Value,
                    value => new AuditLogId(value))
                .HasColumnName("AuditLogId");

            builder.Property(a => a.UserId)
                .HasConversion(
                    id => id != null ? id.Value : (Guid?)null,
                    value => value.HasValue ? new UserId(value.Value) : null);

            builder.Property(a => a.Username)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Action)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(a => a.EntityType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.EntityId)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.EntityName)
                .HasMaxLength(200);

            builder.Property(a => a.OldValues)
                .HasColumnType("nvarchar(max)");

            builder.Property(a => a.NewValues)
                .HasColumnType("nvarchar(max)");

            builder.Property(a => a.ChangedProperties)
                .HasMaxLength(1000);

            builder.Property(a => a.IpAddress)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.UserAgent)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.Timestamp)
                .IsRequired();

            builder.Property(a => a.AdditionalInfo)
                .HasMaxLength(1000);

            // Indexes for querying
            builder.HasIndex(a => a.UserId)
                .HasDatabaseName("IX_AuditLogs_UserId");

            builder.HasIndex(a => new { a.EntityType, a.EntityId })
                .HasDatabaseName("IX_AuditLogs_Entity");

            builder.HasIndex(a => a.Action)
                .HasDatabaseName("IX_AuditLogs_Action");

            builder.HasIndex(a => a.Timestamp)
                .HasDatabaseName("IX_AuditLogs_Timestamp");
        }
    }
}
