using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Application.DTOs.ImportOrder.Requests
{
    public class CreateImportOrderRequest
    {
        public string OrderNumber { get; set; } = null!;
        public int ImporterId { get; set; }
        public int SupplierId { get; set; }
        public int OriginCountryId { get; set; }
        public int CurrencyId { get; set; }
        public TransportMode TransportMode { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
