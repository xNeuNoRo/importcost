using ImportCostPro.Persistence.Common;
using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Persistence.Entities
{
    public class ImportExpense : BaseEntity
    {
        public int ImportOrderId { get; set; }
        public int CurrencyId { get; set; }
        public required string Description { get; set; }
        public required ExpenseType ExpenseType { get; set; }
        public required DistributionBase DistributionBase { get; set; }
        public required decimal OriginalAmount { get; set; }

        // => Navigation Properties
        public ImportOrder ImportOrder { get; set; } = null!;
        public Currency Currency { get; set; } = null!;
    }
}
