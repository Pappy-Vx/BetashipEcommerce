using BetashipEcommerce.CORE.Customers;
using BetashipEcommerce.CORE.Customers.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetashipEcommerce.DAL.Data.Configurations
{
    internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers", "ecommerce");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasConversion(
                    id => id.Value,
                    value => new CustomerId(value))
                .HasColumnName("CustomerId");

            builder.OwnsOne(c => c.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(256);
            });

            builder.Property(c => c.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.OwnsOne(c => c.PhoneNumber, phone =>
            {
                phone.Property(p => p.Value)
                    .HasColumnName("PhoneNumber")
                    .HasMaxLength(20);
            });

            builder.Property(c => c.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(c => c.CreatedAt).IsRequired();
            builder.Property(c => c.UpdatedAt);

            // Customer addresses
            builder.OwnsMany(c => c.Addresses, addresses =>
            {
                addresses.ToTable("CustomerAddresses", "ecommerce");
                addresses.WithOwner().HasForeignKey("CustomerId");
                addresses.HasKey("Id");

                addresses.Property(a => a.Label)
                    .IsRequired()
                    .HasMaxLength(50);

                addresses.Property(a => a.IsDefault)
                    .IsRequired();

                addresses.OwnsOne(a => a.Address, address =>
                {
                    address.Property(ad => ad.Street).HasColumnName("Street").HasMaxLength(200);
                    address.Property(ad => ad.City).HasColumnName("City").HasMaxLength(100);
                    address.Property(ad => ad.State).HasColumnName("State").HasMaxLength(100);
                    address.Property(ad => ad.Country).HasColumnName("Country").HasMaxLength(100);
                    address.Property(ad => ad.PostalCode).HasColumnName("PostalCode").HasMaxLength(20);
                });
            });

            // Indexes
            //builder.HasIndex(c => c.Email.Value)
            //    .IsUnique()
            //    .HasDatabaseName("IX_Customers_Email");

            builder.HasIndex(c => c.Status)
                .HasDatabaseName("IX_Customers_Status");

            builder.Ignore(c => c.DomainEvents);
        }
    }

}
