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
    public class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("categories");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasColumnName("id")
                .HasColumnType("uuid");

            builder.Property(c => c.Name)
                .HasColumnName("name")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.HasIndex(c => c.Name)
                .IsUnique();

            builder.Property(c => c.Description)
                .HasColumnName("description")
                .HasColumnType("text");

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

