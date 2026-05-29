using ImportCostPro.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Persistence.EntityConfigurations
{
    public class TariffCategoryConfiguration : IEntityTypeConfiguration<TariffCategory>
    {
        public void Configure(EntityTypeBuilder<TariffCategory> builder)
        {
            builder.ToTable("TariffCategories");

            // Primary key (PK)
            builder.HasKey(x => x.Id);

            #region Properties configurations

            builder.Property(x => x.Code).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(500);
            builder.Property(x => x.CustomsDutyRate).IsRequired().HasPrecision(5, 2);
            builder
                .Property(x => x.ExciseTaxRate)
                .IsRequired()
                .HasPrecision(5, 2)
                .HasDefaultValue(0m);
            builder.Property(x => x.AppliesItbis).IsRequired().HasDefaultValue(true);
            builder.Property(x => x.AppliesExciseTax).IsRequired().HasDefaultValue(false);
            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            #endregion

            #region Indexes

            builder.HasIndex(x => x.Code).IsUnique();

            #endregion
        }
    }
}
