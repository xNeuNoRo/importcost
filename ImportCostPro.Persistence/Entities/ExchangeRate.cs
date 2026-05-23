using ImportCostPro.Persistence.Common;

namespace ImportCostPro.Persistence.Entities
{
    public class ExchangeRate : ActivatableBaseEntity
    {
        public int OriginCurrencyId { get; set; }
        public int DestinationCurrencyId { get; set; }
        public decimal RateValue { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsUsedInOfficialCalculation { get; set; } // Flag de control para evitar eliminación/edición crítica

        // Propiedades de navegación
        public virtual Currency OriginCurrency { get; set; }
        public virtual Currency DestinationCurrency { get; set; }
    }
}
