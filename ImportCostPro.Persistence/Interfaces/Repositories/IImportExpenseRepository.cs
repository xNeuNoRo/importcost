using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Persistence.Interfaces.Repositories
{
    public interface IImportExpenseRepository : IGenericRepository<ImportExpense>
    {
        Task<ImportExpense?> GetByIdWithCurrencyAsync(int id);
        Task<IEnumerable<ImportExpense>> GetExpensesByOrderIdAsync(int importOrderId);
        Task<bool> HasExpenseTypeAsync(
            int importOrderId,
            ExpenseType expenseType,
            int? excludeExpenseId = null
        );
    }
}
