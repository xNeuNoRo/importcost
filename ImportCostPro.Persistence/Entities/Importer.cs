using ImportCostPro.Persistence.Common;

namespace ImportCostPro.Persistence.Entities
{
    public class Importer : ActivatableBaseEntity
    {
        public required string LegalName { get; set; }

        public required string TaxId { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        // => Navigation Properties

        // El importador pertenece a un Pais
        public int CountryId { get; set; }

        public Country Country { get; set; } = null!;
    }
}
