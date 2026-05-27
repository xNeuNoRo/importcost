using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Application.DTOs.OrderProduct.Responses
{
    public class OrderProductResponse
    {
        public int Id { get; set; }
        public int ImportOrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductReferenceCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitFobPrice { get; set; }
        public decimal SubtotalFob { get; set; }
        public decimal TargetProfitMargin { get; set; }
    }
}
