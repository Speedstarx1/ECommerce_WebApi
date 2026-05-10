using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.EntityTypeConfigurations
{
    public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("orders");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .HasColumnName("id")
                .HasColumnType("uuid");

            builder.Property(o => o.OrderNumber)
                .HasColumnName("order_number")
                .HasColumnType("varchar(20)")
                .IsRequired();

            builder.HasIndex(o => o.OrderNumber)
                .IsUnique();

            builder.Property(o => o.CustomerId)
                .HasColumnName("customer_id")
                .IsRequired();

            builder.HasOne(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(o => o.Status)
                .HasColumnName("status")
                .HasColumnType("varchar(50)")
                .HasConversion<EnumToStringConverter<OrderStatus>>()
                .IsRequired();

            builder.Property(o => o.TotalAmount)
                .HasColumnName("total_amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(o => o.ShippingAddress)
                .HasColumnName("shipping_address")
                .HasColumnType("varchar(500)")
                .IsRequired();

            builder.Property(o => o.PaystackReference)
                .HasColumnName("paystack_reference")
                .HasColumnType("varchar(100)");

            builder.Property(o => o.PaidAt)
                .HasColumnName("paid_at");

            builder.HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(o => o.CreatedBy)
                .HasColumnName("created_by");

            builder.Property(o => o.UpdatedBy)
                .HasColumnName("modified_by");

            builder.Property(o => o.CreatedDate)
                .HasColumnName("created_date");

            builder.Property(o => o.UpdatedDate)
                .HasColumnName("modified_date");

            builder.Property(o => o.IsDeleted)
                .HasColumnName("is_deleted")
                .HasDefaultValue(false);
        }
    }
}








