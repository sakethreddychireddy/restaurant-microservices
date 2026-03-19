using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations
{
    public class UserAddressConfiguration
        : IEntityTypeConfiguration<UserAddress>
    {
        public void Configure(EntityTypeBuilder<UserAddress> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Label)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.FullAddress)
                .IsRequired()
                .HasColumnType("text");

            builder.HasIndex(a => a.UserId);
        }
    }
}