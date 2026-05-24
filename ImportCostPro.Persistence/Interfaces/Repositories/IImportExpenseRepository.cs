using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Persistence.Interfaces.Repositories
{
    public interface IImportExpenseRepository
    {
        Task AddAsync(ImportExpense entity);
        Task UpdateAsync(ImportExpense entity);
        Task DeleteAsync(int id);
        Task<ImportExpense?> GetByIdAsync(int id);

        Task<IEnumerable<ImportExpense>> GetExpensesByOrderIdAsync(int importOrderId);
        Task<bool> HasExpenseTypeAsync(
            int importOrderId,
            ExpenseType expenseType,
            int? excludeExpenseId = null
        );
    }
}
