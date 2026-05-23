using ImportCostPro.Persistence.Common;

namespace ImportCostPro.Persistence.Entities
{
    public class ExchangeRate : ActivatableBaseEntity
    {
        public int FromCurrencyId { get; set; }
        public Currency FromCurrency { get; set; } = null!;
        public int ToCurrencyId { get; set; }
        public Currency ToCurrency { get; set; } = null!;
        public decimal RateValue { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsUsedInOfficialCalculation { get; set; } = false; // Flag de control para evitar eliminación/edición crítica
    }
}
