using ImportCostPro.Persistence.Common;

namespace ImportCostPro.Persistence.Entities
{
    public class OrderProduct : BaseEntity
    {
        public int ImportOrderId { get; set; }
        public int ProductId { get; set; }
        public required decimal Quantity { get; set; }
        public required decimal UnitFobPrice { get; set; }
        public required decimal TargetProfitMargin { get; set; }

        // => Navigation Properties
        public ImportOrder ImportOrder { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
