using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.EntityTypeConfigurations
{
    public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("products");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("id")
                .HasColumnType("uuid");

            builder.Property(p => p.Name)
                .HasColumnName("name")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(p => p.Description)
                .HasColumnName("description")
                .HasColumnType("text")
                .IsRequired();

            builder.Property(p => p.Price)
                .HasColumnName("price")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.StockQuantity)
                .HasColumnName("stock_quantity")
                .IsRequired();

            builder.Property(p => p.ImageUrl)
                .HasColumnName("image_url");

            builder.Property(p => p.IsAvailable)
                .HasColumnName("is_available")
                .HasDefaultValue(true);

            builder.Property(p => p.CategoryId)
                .HasColumnName("category_id");

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(p => p.CreatedBy)
                .HasColumnName("created_by");

            builder.Property(p => p.UpdatedBy)
                .HasColumnName("modified_by");

            builder.Property(p => p.CreatedDate)
                .HasColumnName("created_date");

            builder.Property(p => p.UpdatedDate)
                .HasColumnName("modified_date");

            builder.Property(p => p.IsDeleted)
                .HasColumnName("is_deleted")
                .HasDefaultValue(false);
        }
    }
}