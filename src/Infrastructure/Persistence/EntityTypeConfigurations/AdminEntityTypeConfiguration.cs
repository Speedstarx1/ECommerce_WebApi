using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections;

namespace Infrastructure.Persistence.EntityTypeConfigurations
{
    public class AdminEntityTypeConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.Property(s => s.RefNumber)
                .HasColumnName("reference_number")
                .HasColumnType("varchar(20)")
                .IsRequired();

            builder.HasIndex(s => s.RefNumber)
                .IsUnique();

            
        }
    }
}