using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Persistence.EntityConfigurations
{
    public class ImportOrderConfiguration : IEntityTypeConfiguration<ImportOrder>
    {
        public void Configure(EntityTypeBuilder<ImportOrder> builder)
        {
            builder.ToTable("ImportOrders");

            // Primary key (PK)
            builder.HasKey(x => x.Id);

            #region Properties configurations

            builder.Property(x => x.OrderNumber).IsRequired().HasMaxLength(30);
            builder.Property(x => x.OrderDate).IsRequired().HasColumnType("date");
            builder.Property(x => x.TransportMode).IsRequired();
            builder
                .Property(x => x.Status)
                .IsRequired()
                .HasDefaultValue(OrderStatus.Open)
                .HasSentinel((OrderStatus)(-1)); // Le decimos q el valor vacio o no asignado es -1, que no es un valor valido del enum
            ;

            #endregion

            #region Indexes

            builder.HasIndex(x => x.OrderNumber).IsUnique();

            #endregion

            #region Relationships

            builder
                .HasOne(x => x.Importer)
                .WithMany()
                .HasForeignKey(x => x.ImporterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Supplier)
                .WithMany()
                .HasForeignKey(x => x.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.OriginCountry)
                .WithMany()
                .HasForeignKey(x => x.OriginCountryId)
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
