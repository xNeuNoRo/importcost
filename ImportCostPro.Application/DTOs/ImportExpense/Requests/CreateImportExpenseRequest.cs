using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Application.DTOs.ImportExpense.Requests
{
    public class CreateImportExpenseRequest
    {
        public int ImportOrderId { get; set; }
        public int CurrencyId { get; set; }
        public string Description { get; set; } = null!;
        public ExpenseType ExpenseType { get; set; }
        public DistributionBase DistributionBase { get; set; }
        public decimal OriginalAmount { get; set; }
    }
}
