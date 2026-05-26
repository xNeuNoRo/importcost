namespace ImportCostPro.Application.DTOs.ExchangeRate.Responses
{
    public class ExchangeRateResponse
    {
        public int Id { get; set; }
        public int FromCurrencyId { get; set; }
        public string FromCurrencyIsoCode { get; set; } = null!;
        public int ToCurrencyId { get; set; }
        public string ToCurrencyIsoCode { get; set; } = null!;
        public decimal RateValue { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; }
    }
}