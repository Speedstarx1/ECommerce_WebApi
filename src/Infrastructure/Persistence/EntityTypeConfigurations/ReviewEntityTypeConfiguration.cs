using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityTypeConfigurations
{
    public class ReviewEntityTypeConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("reviews");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .HasColumnName("id")
                .HasColumnType("uuid");

            builder.Property(r => r.CustomerId)
                .HasColumnName("customer_id")
                .IsRequired();

            builder.Property(r => r.ProductId)
                .HasColumnName("product_id")
                .IsRequired();

            builder.Property(r => r.Rating)
                .HasColumnName("rating")
                .IsRequired();

            builder.Property(r => r.Comment)
                .HasColumnName("comment")
                .HasColumnType("text");

            builder.HasOne(r => r.Customer)
                .WithMany()
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // One review per customer per product
            builder.HasIndex(r => new { r.CustomerId, r.ProductId })
                .IsUnique();

            builder.Property(r => r.CreatedBy)
                .HasColumnName("created_by");

            builder.Property(r => r.UpdatedBy)
                .HasColumnName("modified_by");

            builder.Property(r => r.CreatedDate)
                .HasColumnName("created_date");

            builder.Property(r => r.UpdatedDate)
                .HasColumnName("modified_date");

            builder.Property(r => r.IsDeleted)
                .HasColumnName("is_deleted")
                .HasDefaultValue(false);
        }
    }
}