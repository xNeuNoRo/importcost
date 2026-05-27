namespace ImportCostPro.Application.DTOs.OrderProduct.Requests
{
    public class CreateOrderProductRequest
    {
        public int ImportOrderId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitFobPrice { get; set; }
        public decimal TargetProfitMargin { get; set; }
    }
}
