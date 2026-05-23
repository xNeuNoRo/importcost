using ImportCostPro.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Persistence.EntityConfigurations
{
    public class CurrencyEntityConfiguration : IEntityTypeConfiguration<Currency>
    {
        public void Configure(EntityTypeBuilder<Currency> builder)
        {
            // Primary key (PK)
            builder.HasKey(x => x.Id);

            builder.ToTable("Currencies");

            #region Properties configurations

            builder.Property(x => x.Name).IsRequired().HasMaxLength(150);

            builder.Property(x => x.IsoCode).IsRequired().HasMaxLength(3);

            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.Property(x => x.Symbol).IsRequired().HasMaxLength(10);

            builder.Property(x => x.IsLocalCurrency).IsRequired().HasDefaultValue(false);

            #endregion

            #region Indexes

            // Aseguramos unicidad del codigo ISO
            builder.HasIndex(x => x.IsoCode).IsUnique();
            
            // Solo puede haber una moneda local, por ende le dicemos a efc que 
            // cree un indice unico sobre IsLocalCurrency pero solo para los registros donde IsLocalCurrency = true
            builder.HasIndex(x => x.IsLocalCurrency).IsUnique().HasFilter("[IsLocalCurrency] = 1");

            #endregion
        }
    }
}
