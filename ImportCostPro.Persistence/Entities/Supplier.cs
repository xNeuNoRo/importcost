using ImportCostPro.Persistence.Common;

namespace ImportCostPro.Persistence.Entities
{
    public class Supplier : ActivatableBaseEntity
    {
        public required string Name { get; set; }
        public string ? PhoneNumber { get; set; }
        public required string Email { get; set; }
        public string ? Address { get; set; }

        // Pais de Origen y moneda principal de negociacion

        public int OriginCountryId { get; set; }
        public int DefaultCurrencyId { get; set; }

        // => Navigation Properties
        public Country? Country { get; set; }
        public Currency? Currency { get; set; }
        
    }
}
