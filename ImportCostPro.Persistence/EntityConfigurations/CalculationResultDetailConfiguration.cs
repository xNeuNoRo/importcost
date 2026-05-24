using ImportCostPro.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Persistence.EntityConfigurations
{
    public class CalculationResultDetailConfiguration
        : IEntityTypeConfiguration<CalculationResultDetail>
    {
        public void Configure(EntityTypeBuilder<CalculationResultDetail> builder)
        {
            builder.ToTable("CalculationResultDetails");

            // Primary Key (PK)
            builder.HasKey(x => x.Id);

            #region Properties Configurations

            builder.Property(x => x.Quantity).IsRequired().HasPrecision(10, 2);
            builder.Property(x => x.OriginalUnitPriceFob).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.LocalTotalFob).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.AllocatedFreight).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.AllocatedInsurance).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.LocalTotalCif).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.CustomsDutyAmount).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.ExciseTaxAmount).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.CustomsServiceAmount).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.ItbisAmount).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.AllocatedLocalExpenses).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.LocalTotalLandedCost).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.UnitLandedCost).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.ProfitMarginRate).IsRequired().HasPrecision(5, 2);
            builder.Property(x => x.SuggestedRetailPrice).IsRequired().HasPrecision(18, 2);

            #endregion

            #region Relationships

            builder
                .HasOne(x => x.CalculationResult)
                .WithMany(c => c.Details)
                .HasForeignKey(x => x.CalculationResultId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion
        }
    }
}
