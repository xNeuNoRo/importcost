namespace ImportCostPro.Application.DTOs.Currency.Requests
{
    public class CreateCurrencyRequest
    {
        public string Name { get; set; } = null!;
        public string IsoCode { get; set; } = null!;
        public string Symbol { get; set; } = null!;
        public bool IsLocalCurrency { get; set; }
        public bool IsActive { get; set; }
    }
}