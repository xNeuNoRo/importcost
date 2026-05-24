using ImportCostPro.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Persistence.EntityConfigurations
{
    public class ImportExpenseConfiguration : IEntityTypeConfiguration<ImportExpense>
    {
        public void Configure(EntityTypeBuilder<ImportExpense> builder)
        {
            builder.ToTable("ImportExpenses");

            // Primary Key (PK)
            builder.HasKey(x => x.Id);

            #region Properties configurations

            builder.Property(x => x.Description).IsRequired().HasMaxLength(150);
            builder.Property(x => x.ExpenseType).IsRequired();
            builder.Property(x => x.DistributionBase).IsRequired();
            builder.Property(x => x.OriginalAmount).IsRequired().HasPrecision(18, 2);

            #endregion

            #region Relationships

            builder
                .HasOne(x => x.ImportOrder)
                .WithMany()
                .HasForeignKey(x => x.ImportOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Currency)
                .WithMany()
                .HasForeignKey(x => x.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion
        }
    }
}
