namespace ImportCostPro.Application.DTOs.Supplier.Requests
{
    public class UpdateSupplierRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public int OriginCountryId { get; set; }
        public int DefaultCurrencyId { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }
}