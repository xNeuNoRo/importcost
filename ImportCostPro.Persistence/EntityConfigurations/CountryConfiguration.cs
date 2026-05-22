using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Persistence.EntityConfigurations
{
    public class CountryEntityConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasKey(x => x.ID);

            builder.ToTable("Countries");

            #region Properties configurations
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(x => x.IsoCode)
                .IsRequired()
                .HasMaxLength(3);
            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
            
            #endregion
        }
    }
}