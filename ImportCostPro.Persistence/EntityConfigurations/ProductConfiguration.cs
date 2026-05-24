using ImportCostPro.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Persistence.EntityConfigurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            builder.HasKey(x => x.Id);

            #region Properties configurations

            builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
            builder.Property(x => x.ReferenceCode).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Description).HasMaxLength(250);
            builder.Property(x => x.UnitWeight).IsRequired().HasPrecision(10, 2);
            builder.Property(x => x.Length).HasPrecision(10, 2);
            builder.Property(x => x.Width).HasPrecision(10, 2);
            builder.Property(x => x.Height).HasPrecision(10, 2);
            builder.Property(x => x.UnitOfMeasure).IsRequired();
            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            #endregion

            #region Indexes

            builder.HasIndex(x => x.ReferenceCode).IsUnique();

            #endregion

            #region Relationships

            builder
                .HasOne(x => x.DefaultOriginCountry)
                .WithMany()
                .HasForeignKey(x => x.DefaultOriginCountryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.TariffCategory)
                .WithMany()
                .HasForeignKey(x => x.TariffCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion
        }
    }
}
