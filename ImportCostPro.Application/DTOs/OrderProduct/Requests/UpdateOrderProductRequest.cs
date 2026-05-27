namespace ImportCostPro.Application.DTOs.OrderProduct.Requests
{
    public class UpdateOrderProductRequest
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitFobPrice { get; set; }
        public decimal TargetProfitMargin { get; set; }
    }
}
