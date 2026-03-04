using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.MenuItemName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.UnitPrice)
                .HasColumnType("decimal(10,2)");

            builder.Ignore(x => x.Subtotal);
        }
    }
}
