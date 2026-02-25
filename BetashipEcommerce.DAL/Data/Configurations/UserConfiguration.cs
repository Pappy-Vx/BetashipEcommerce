using BetashipEcommerce.CORE.Customers.ValueObjects;
using BetashipEcommerce.CORE.Identity;
using BetashipEcommerce.CORE.Identity.Enums;
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
    internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", "ecommerce");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .HasConversion(
                    id => id.Value,
                    value => new UserId(value))
                .HasColumnName("UserId");

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);

            builder.OwnsOne(u => u.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(256);
            });

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.CustomerId)
                .HasConversion(
                    id => id != null ? id.Value : (Guid?)null,
                    value => value.HasValue ? new CustomerId(value.Value) : null);

            builder.Property(u => u.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(u => u.IsEmailVerified).IsRequired();
            builder.Property(u => u.EmailVerifiedAt);

            builder.Property(u => u.CreatedAt).IsRequired();
            builder.Property(u => u.LastLoginAt);
            builder.Property(u => u.LastPasswordChangedAt);
            builder.Property(u => u.FailedLoginAttempts).IsRequired();
            builder.Property(u => u.LockedOutUntil);

            // Store roles as JSON array
            builder.Property(u => u.Roles)
                .HasConversion(
                    roles => System.Text.Json.JsonSerializer.Serialize(roles, (System.Text.Json.JsonSerializerOptions?)null),
                    json => System.Text.Json.JsonSerializer.Deserialize<List<UserRole>>(json, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<UserRole>())
                .HasColumnName("Roles")
                .HasColumnType("character varying(500)");

            // Store permissions as JSON array
            builder.Property(u => u.Permissions)
                .HasConversion(
                    perms => System.Text.Json.JsonSerializer.Serialize(perms, (System.Text.Json.JsonSerializerOptions?)null),
                    json => System.Text.Json.JsonSerializer.Deserialize<List<string>>(json, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>())
                .HasColumnName("Permissions")
                .HasColumnType("text");

            // Audit fields
            //builder.Property(u => u.CreatedBy);
            //builder.Property(u => u.CreatedByUsername).HasMaxLength(100);
            //builder.Property(u => u.UpdatedBy);
            //builder.Property(u => u.UpdatedByUsername).HasMaxLength(100);
            //builder.Property(u => u.IsDeleted).IsRequired().HasDefaultValue(false);
            //builder.Property(u => u.DeletedAt);
            //builder.Property(u => u.DeletedBy);
            //builder.Property(u => u.DeletedByUsername).HasMaxLength(100);

            // User sessions
            builder.OwnsMany(u => u.Sessions, sessions =>
            {
                sessions.ToTable("UserSessions", "ecommerce");
                sessions.WithOwner().HasForeignKey("UserId");
                sessions.HasKey("Id");

                sessions.Property(s => s.IpAddress)
                    .IsRequired()
                    .HasMaxLength(50);

                sessions.Property(s => s.UserAgent)
                    .IsRequired()
                    .HasMaxLength(500);

                sessions.Property(s => s.LoginAt).IsRequired();
                sessions.Property(s => s.LogoutAt);
            });

            // Indexes
            builder.HasIndex(u => u.Username)
                .IsUnique()
                .HasDatabaseName("IX_Users_Username");

            builder.HasIndex(u => u.Status)
                .HasDatabaseName("IX_Users_Status");

            builder.Ignore(u => u.DomainEvents);
        }
    }
}
