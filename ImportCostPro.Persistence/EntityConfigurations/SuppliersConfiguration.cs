using ImportCostPro.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImportCostPro.Persistence.EntityConfigurations
{
    public class SuppliersConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Suppliers");
            
            #region  Properties configurations;

            builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150)
            .IsUnicode(false);

            builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20)
            .IsUnicode(false);

            builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(100)
            .IsUnicode(false);

            builder.Property(x => x.Address)
            .HasMaxLength(250)
            .IsUnicode(false);

            #endregion

            #region  Relationships 
            builder.HasOne(x => x.Country)
            .WithMany()
            .HasForeignKey(x => x.OriginCountryId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Currency)
            .WithMany()
            .HasForeignKey(x => x.DefaultCurrencyId)
            .OnDelete(DeleteBehavior.Restrict);
        
            #endregion

        }
    }
}


