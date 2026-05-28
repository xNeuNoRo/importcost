using ImportCostPro.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Persistence.EntityConfigurations
{
    public class OrderProductConfiguration : IEntityTypeConfiguration<OrderProduct>
    {
        public void Configure(EntityTypeBuilder<OrderProduct> builder)
        {
            builder.ToTable("OrderProducts");
            builder.HasKey(x => x.Id);

            #region Properties configurations

            builder.Property(x => x.Quantity).IsRequired().HasPrecision(10, 2);
            builder.Property(x => x.UnitFobPrice).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.TargetProfitMargin).IsRequired().HasPrecision(5, 2);

            #endregion

            #region Indexes

            // Solo puede haber 1 producto sin repetirse por orden,
            // por eso creamos este indice compuesto.
            builder.HasIndex(x => new { x.ImportOrderId, x.ProductId }).IsUnique();

            #endregion

            #region Relationships

            builder
                .HasOne(x => x.ImportOrder)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(x => x.ImportOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion
        }
    }
}
