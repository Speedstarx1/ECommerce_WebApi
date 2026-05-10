using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityTypeConfigurations
{
    public class CartEntityTypeConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.ToTable("carts");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasColumnName("id")
                .HasColumnType("uuid");

            builder.Property(c => c.CustomerId)
                .HasColumnName("customer_id")
                .IsRequired();

            builder.HasOne(c => c.Customer)
                .WithOne()
                .HasForeignKey<Cart>(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Items)
                .WithOne(i => i.Cart)
                .HasForeignKey(i => i.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(c => c.CreatedBy)
                .HasColumnName("created_by");

            builder.Property(c => c.UpdatedBy)
                .HasColumnName("modified_by");

            builder.Property(c => c.CreatedDate)
                .HasColumnName("created_date");

            builder.Property(c => c.UpdatedDate)
                .HasColumnName("modified_date");

            builder.Property(c => c.IsDeleted)
                .HasColumnName("is_deleted")
                .HasDefaultValue(false);
        }
    }
}