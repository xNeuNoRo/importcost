using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Application.DTOs.ImportExpense.Responses
{
    public class ImportExpenseResponse
    {
        public int Id { get; set; }
        public int ImportOrderId { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyIsoCode { get; set; } = null!;
        public string Description { get; set; } = null!;
        public ExpenseType ExpenseType { get; set; }
        public DistributionBase DistributionBase { get; set; }
        public decimal OriginalAmount { get; set; }
    }
}
