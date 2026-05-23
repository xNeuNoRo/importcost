using ImportCostPro.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Persistence.EntityConfigurations
{
    public class ExchangeRateConfiguration : IEntityTypeConfiguration<ExchangeRate>
    {
        public void Configure(EntityTypeBuilder<ExchangeRate> builder)
        {
            builder.ToTable("ExchangeRates");

            // Primary Key (PK)
            builder.HasKey(e => e.Id);

            #region Properties configurations

            builder.Property(e => e.RateValue).IsRequired().HasPrecision(18, 4);

            builder.Property(e => e.EffectiveDate).IsRequired().HasColumnType("date");

            builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);

            builder
                .Property(e => e.IsUsedInOfficialCalculation)
                .IsRequired()
                .HasDefaultValue(false);

            #endregion

            #region Relationships

            builder
                .HasOne(e => e.FromCurrency)
                .WithMany()
                .HasForeignKey(e => e.FromCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(e => e.ToCurrency)
                .WithMany()
                .HasForeignKey(e => e.ToCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region Indexes

            // Para evitar tener más de una tasa activa para la misma combinación de monedas y fecha,
            // esto evita a nivel de bd que bajo concurrencia se pudiese crear más de una tasa activa
            // para la misma combinación de monedas y fecha.
            builder
                .HasIndex(e => new
                {
                    e.FromCurrencyId,
                    e.ToCurrencyId,
                    e.EffectiveDate,
                })
                .IsUnique()
                .HasFilter("[IsActive] = 1");

            #endregion
        }
    }
}
