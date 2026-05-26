using ImportCostPro.Persistence.Common;

namespace ImportCostPro.Persistence.Entities
{
    public class ExchangeRate : ActivatableBaseEntity
    {
        public int FromCurrencyId { get; private set; }
        public Currency FromCurrency { get; private set; } = null!;
        public int ToCurrencyId { get; private set; }
        public Currency ToCurrency { get; private set; } = null!;
        public decimal RateValue { get; private set; }
        public DateTime EffectiveDate { get; private set; }
        public bool IsUsedInOfficialCalculation { get; set; } = false; // Flag de control para evitar eliminación/edición crítica

        protected ExchangeRate() { }

        private ExchangeRate(
            int fromCurrencyId,
            int toCurrencyId,
            decimal rateValue,
            DateTime effectiveDate
        )
        {
            FromCurrencyId = fromCurrencyId;
            ToCurrencyId = toCurrencyId;
            RateValue = rateValue;
            EffectiveDate = effectiveDate.Date;
            IsActive = true;
        }

        public static ExchangeRate Create(
            int fromCurrencyId,
            int toCurrencyId,
            decimal rateValue,
            DateTime effectiveDate
        )
        {
            return new ExchangeRate(fromCurrencyId, toCurrencyId, rateValue, effectiveDate);
        }

        public void UpdateDetails(
            int fromCurrencyId,
            int toCurrencyId,
            decimal rateValue,
            DateTime effectiveDate
        )
        {
            FromCurrencyId = fromCurrencyId;
            ToCurrencyId = toCurrencyId;
            RateValue = rateValue;
            EffectiveDate = effectiveDate.Date;
        }
    }
}
