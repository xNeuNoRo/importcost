using ImportCostPro.Persistence.Common;

namespace ImportCostPro.Persistence.Entities
{
    public class Country : ActivatableBaseEntity
    {
        public required string Name { get; set; }
        public required string IsoCode { get; set; }
    }
}
