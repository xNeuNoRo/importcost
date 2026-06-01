namespace ImportCostPro.Application.DTOs.ExchangeRate.Requests
{
    public class UpdateExchangeRateRequest
    {
        public int Id { get; set; }
        public int FromCurrencyId { get; set; }
        public int ToCurrencyId { get; set; }
        public decimal RateValue { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; }
    }
}