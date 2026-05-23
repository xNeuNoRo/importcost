using ImportCostPro.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Persistence.EntityConfigurations
{
    public class TaxConfigurationConfiguration : IEntityTypeConfiguration<TaxConfiguration>
    {
        public void Configure(EntityTypeBuilder<TaxConfiguration> builder)
        {
            builder.ToTable("TaxConfigurations");

            // Primary key (PK)
            builder.HasKey(x => x.Id);

            #region Properties configurations

            // Esto es literal un decimal(5,2) en sql server
            // Lo cual nos permite almacenar valores como 18.00,
            // 28.00, etc. para las tasas de impuestos
            builder.Property(x => x.GeneralItbisRate).IsRequired().HasPrecision(5, 2);

            // lo mismo con este porcentaje del servicio aduanero
            builder.Property(x => x.CustomsServiceRate).IsRequired().HasPrecision(5, 2);

            #endregion
        }
    }
}
