using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Application.DTOs.ImportExpense.Requests
{
    public class UpdateImportExpenseRequest
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public DistributionBase DistributionBase { get; set; }
        public decimal OriginalAmount { get; set; }
        public DateTime ExpenseDate { get; set; }
    }
}
