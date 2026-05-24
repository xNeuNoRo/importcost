using ImportCostPro.Persistence.Contexts;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Enums;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Persistence.Repositories
{
    public class ImportExpenseRepository
        : GenericRepository<ImportExpense>,
            IImportExpenseRepository
    {
        public ImportExpenseRepository(AppDbContext context)
            : base(context) { }

        public async Task<IEnumerable<ImportExpense>> GetExpensesByOrderIdAsync(int importOrderId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(e => e.Currency)
                .Where(e => e.ImportOrderId == importOrderId)
                .ToListAsync();
        }

        public async Task<bool> HasExpenseTypeAsync(
            int importOrderId,
            ExpenseType expenseType,
            int? excludeExpenseId = null
        )
        {
            var query = _dbSet.Where(e =>
                e.ImportOrderId == importOrderId && e.ExpenseType == expenseType
            );

            if (excludeExpenseId.HasValue)
            {
                query = query.Where(e => e.Id != excludeExpenseId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
