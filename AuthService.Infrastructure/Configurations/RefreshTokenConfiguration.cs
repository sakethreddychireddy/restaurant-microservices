using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Configurations
{
    public class RefreshTokenConfiguration
        : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Token)
                .IsRequired()
                .HasColumnType("text");

            builder.HasIndex(r => r.Token).IsUnique();
            builder.HasIndex(r => r.UserId);

            // ignore computed properties
            builder.Ignore(r => r.IsExpired);
            builder.Ignore(r => r.IsActive);
        }
    }
}
