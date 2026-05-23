using ImportCostPro.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Persistence.EntityConfigurations
{
    public class SuppliersConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Suppliers");

            #region  Properties configurations;

            builder.Property(x => x.Name).IsRequired().HasMaxLength(150);

            builder.Property(x => x.PhoneNumber).HasMaxLength(20);

            builder.Property(x => x.Email).IsRequired().HasMaxLength(100);

            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            #endregion

            #region  Relationships

            builder
                .HasOne(x => x.OriginCountry)
                .WithMany()
                .HasForeignKey(x => x.OriginCountryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.DefaultCurrency)
                .WithMany()
                .HasForeignKey(x => x.DefaultCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Indexes

            // El nombre no puede ser dup
            builder.HasIndex(x => x.Name).IsUnique();

            #endregion
        }
    }
}
