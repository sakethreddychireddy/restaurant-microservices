using MenuService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MenuService.Infrastructure.Persistence.Configurations
{
    public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
    {
        public void Configure(EntityTypeBuilder<MenuItem> builder)
        {
            builder.HasKey(mi => mi.Id);
            builder.Property(mi => mi.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(mi => mi.Description)
                .HasMaxLength(500);
            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(500);
            builder.Property(x => x.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder.Property(x => x.Category)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Emoji)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(x => x.Badge)
                .HasMaxLength(50);

            builder.HasIndex(x => x.Category);
            builder.HasIndex(x => x.IsAvailable);
        }
    }
}
