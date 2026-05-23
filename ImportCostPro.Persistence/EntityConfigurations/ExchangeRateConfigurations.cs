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

            builder.HasKey(e => e.Id);

            builder.Property(e => e.RateValue)
                .HasColumnType("decimal(18,4)")
                .IsRequired();

            builder.Property(e => e.EffectiveDate)
                .IsRequired();

            builder.Property(e => e.IsActive)
                .HasDefaultValue(true);

            // Relaciones con la entidad Currency
            builder.HasOne(e => e.OriginCurrency)
                .WithMany()
                .HasForeignKey(e => e.OriginCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.DestinationCurrency)
                .WithMany()
                .HasForeignKey(e => e.DestinationCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
