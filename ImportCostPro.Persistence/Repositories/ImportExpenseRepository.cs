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

        public new async Task AddAsync(ImportExpense entity)
        {
            await base.AddAsync(entity);
        }

        public new async Task UpdateAsync(ImportExpense entity)
        {
            await base.UpdateAsync(entity);
        }

        public new async Task DeleteAsync(int id)
        {
            await base.DeleteAsync(id);
        }

        public new async Task<ImportExpense?> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

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
