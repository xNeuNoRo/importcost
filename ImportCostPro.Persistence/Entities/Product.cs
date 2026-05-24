using ImportCostPro.Persistence.Common;
using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Persistence.Entities
{
    public class Product : ActivatableBaseEntity
    {
        public required string Name { get; set; }
        public required string ReferenceCode { get; set; }
        public int DefaultOriginCountryId { get; set; }
        public int TariffCategoryId { get; set; }
        public required decimal UnitWeight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }

        public required UnitOfMeasure UnitOfMeasure { get; set; }

        public string? Description { get; set; }

        // => Navigation Properties
        public Country DefaultOriginCountry { get; set; } = null!;
        public TariffCategory TariffCategory { get; set; } = null!;
    }
}
