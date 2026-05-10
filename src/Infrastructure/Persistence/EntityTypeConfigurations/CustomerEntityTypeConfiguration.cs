using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Persistence.EntityTypeConfigurations
{
    public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            // Student-specific properties only (base properties configured in UserEntityTypeConfiguration)
            builder.Property(s => s.RefNumber)
                .HasColumnName("Reference_Number")
                .HasColumnType("varchar(20)")
                .IsRequired();

            builder.HasIndex(s => s.RefNumber)
                .IsUnique();

            
        }
    }
}