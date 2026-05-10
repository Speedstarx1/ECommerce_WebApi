using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityTypeConfigurations
{
    public class CartItemEntityTypeConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("cart_items");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Id)
                .HasColumnName("id")
                .HasColumnType("uuid");

            builder.Property(ci => ci.CartId)
                .HasColumnName("cart_id")
                .IsRequired();

            builder.Property(ci => ci.ProductId)
                .HasColumnName("product_id")
                .IsRequired();

            builder.HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(ci => ci.Quantity)
                .HasColumnName("quantity")
                .IsRequired();

            builder.Property(ci => ci.UnitPrice)
                .HasColumnName("unit_price")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(ci => ci.CreatedBy)
                .HasColumnName("created_by");

            builder.Property(ci => ci.UpdatedBy)
                .HasColumnName("modified_by");

            builder.Property(ci => ci.CreatedDate)
                .HasColumnName("created_date");

            builder.Property(ci => ci.UpdatedDate)
                .HasColumnName("modified_date");

            builder.Property(ci => ci.IsDeleted)
                .HasColumnName("is_deleted")
                .HasDefaultValue(false);
        }
    }
}