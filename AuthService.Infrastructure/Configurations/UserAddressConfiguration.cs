using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations
{
    public class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
    {
        public void Configure(EntityTypeBuilder<UserAddress> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Label)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.AddressLine1)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.State)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.ZipCode)
                .IsRequired()
                .HasMaxLength(20);

            // computed column — not stored
            builder.Ignore(a => a.FullAddress);

            builder.HasIndex(a => a.UserId);
        }
    }
}