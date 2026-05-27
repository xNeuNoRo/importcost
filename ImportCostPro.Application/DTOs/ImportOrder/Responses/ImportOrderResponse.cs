using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Application.DTOs.ImportOrder.Responses
{
    public class ImportOrderResponse
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = null!;

        public int ImporterId { get; set; }
        public string ImporterName { get; set; } = null!;

        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = null!;

        public int OriginCountryId { get; set; }
        public string OriginCountryName { get; set; } = null!;

        public int CurrencyId { get; set; }
        public string CurrencyIsoCode { get; set; } = null!;

        public OrderStatus Status { get; set; }
        public TransportMode TransportMode { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
