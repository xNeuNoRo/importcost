namespace ImportCostPro.Application.DTOs.ExchangeRate.Requests
{
    public class CreateExchangeRateRequest
    {
        public int FromCurrencyId { get; set; }
        public int ToCurrencyId { get; set; }
        public decimal RateValue { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}