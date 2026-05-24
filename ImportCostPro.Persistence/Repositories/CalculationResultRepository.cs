using ImportCostPro.Persistence.Contexts;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Persistence.Repositories
{
    public class CalculationResultRepository
        : GenericRepository<CalculationResult>,
            ICalculationResultRepository
    {
        public CalculationResultRepository(AppDbContext context)
            : base(context) { }

        public async Task<IEnumerable<CalculationResult>> GetHistoryByOrderIdAsync(
            int importOrderId
        )
        {
            return await _dbSet
                .AsNoTracking()
                .Where(x => x.ImportOrderId == importOrderId)
                .OrderByDescending(x => x.CalculationDate)
                .ToListAsync();
        }

        public async Task<CalculationResult?> GetLatestCalculatedResultWithDetailsAsync(
            int importOrderId
        )
        {
            return await _dbSet
                .Include(x => x.Details)
                    .ThenInclude(d => d.Product)
                .Where(x => x.ImportOrderId == importOrderId)
                .OrderByDescending(x => x.CalculationDate)
                .FirstOrDefaultAsync();
        }
    }
}
