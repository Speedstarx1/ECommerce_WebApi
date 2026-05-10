using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityTypeConfigurations
{
    public class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("order_items");

            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.Id)
                .HasColumnName("id")
                .HasColumnType("uuid");

            builder.Property(oi => oi.ProductName)
                .HasColumnName("product_name")
                .IsRequired();

            builder.Property(oi => oi.OrderId)
                .HasColumnName("order_id")
                .IsRequired();

            builder.Property(oi => oi.ProductId)
                .HasColumnName("product_id")
                .IsRequired();

            builder.HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(oi => oi.Quantity)
                .HasColumnName("quantity")
                .IsRequired();

            builder.Property(oi => oi.UnitPrice)
                .HasColumnName("unit_price")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(oi => oi.CreatedBy)
                .HasColumnName("created_by");

            builder.Property(oi => oi.UpdatedBy)
                .HasColumnName("modified_by");

            builder.Property(oi => oi.CreatedDate)
                .HasColumnName("created_date");

            builder.Property(oi => oi.UpdatedDate)
                .HasColumnName("modified_date");

            builder.Property(oi => oi.IsDeleted)
                .HasColumnName("is_deleted")
                .HasDefaultValue(false);
        }
    }
}





