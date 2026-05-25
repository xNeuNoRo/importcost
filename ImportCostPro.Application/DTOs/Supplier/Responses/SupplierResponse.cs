namespace ImportCostPro.Application.DTOs.Supplier.Responses
{
    public class SupplierResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int OriginCountryId { get; set; }
        public string CountryName { get; set; } = null!;
        public int DefaultCurrencyId { get; set; }
        public string CurrencyName { get; set; } = null!;
        public string CurrencyIsoCode { get; set; } = null!;
        public string CurrencySymbol { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }
}