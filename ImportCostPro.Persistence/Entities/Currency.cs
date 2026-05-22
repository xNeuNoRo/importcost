using ImportCostPro.Persistence.Common;

namespace ImportCostPro.Persistence.Entities
{
    public class Currency : ActivatableBaseEntity
    {
        public required string Name { get; set; }
        public required string IsoCode { get; set; }
        public required string Symbol { get; set; }
        // Indica si esta moneda es la moneda local del sistema
        public required bool IsLocalCurrency { get; set; } = false;

    }
}
