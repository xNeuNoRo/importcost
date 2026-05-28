using ImportCostPro.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Persistence.EntityConfigurations
{
    public class CalculationResultConfiguration : IEntityTypeConfiguration<CalculationResult>
    {
        public void Configure(EntityTypeBuilder<CalculationResult> builder)
        {
            builder.ToTable("CalculationResults");

            // Primary Key (PK)
            builder.HasKey(x => x.Id);

            #region Properties Configurations

            builder.Property(x => x.CalculationDate).IsRequired();
            builder.Property(x => x.ExchangeRateUsed).IsRequired().HasPrecision(18, 4);
            builder.Property(x => x.TotalOriginalFob).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.TotalLocalFob).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.TotalFreight).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.TotalInsurance).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.TotalCif).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.TotalTariff).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.TotalExciseTax).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.TotalCustomsService).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.TotalItbis).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.TotalLocalExpenses).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.TotalImportCost).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.TotalImportedQuantity).IsRequired().HasPrecision(10, 2);
            builder.Property(x => x.HistoricalItbisRate).IsRequired().HasPrecision(5, 2);
            builder.Property(x => x.HistoricalCustomsServiceRate).IsRequired().HasPrecision(5, 2);

            #endregion

            #region Relationships


            builder
                .HasOne(x => x.ImportOrder)
                .WithMany(o => o.CalculationResults)
                .HasForeignKey(x => x.ImportOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.LocalCurrencyUsed)
                .WithMany()
                .HasForeignKey(x => x.LocalCurrencyUsedId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion
        }
    }
}
