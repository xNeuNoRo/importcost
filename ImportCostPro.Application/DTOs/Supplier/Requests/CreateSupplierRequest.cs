namespace ImportCostPro.Application.DTOs.Supplier.Requests
{
    public class CreateSupplierRequest
    {
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public int OriginCountryId { get; set; }
        public int DefaultCurrencyId { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
