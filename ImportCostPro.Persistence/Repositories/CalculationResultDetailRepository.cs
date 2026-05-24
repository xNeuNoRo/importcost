using ImportCostPro.Persistence.Contexts;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Persistence.Repositories
{
    public class CalculationResultDetailRepository
        : GenericRepository<CalculationResultDetail>,
            ICalculationResultDetailRepository
    {
        public CalculationResultDetailRepository(AppDbContext context)
            : base(context) { }

        public async Task<IEnumerable<CalculationResultDetail>> GetDetailsByCalculationIdAsync(
            int calculationResultId
        )
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.Product)
                .Where(x => x.CalculationResultId == calculationResultId)
                .ToListAsync();
        }

        public async Task<IEnumerable<CalculationResultDetail>> GetCostHistoryByProductIdAsync(
            int productId
        )
        {
            return await _dbSet
                .AsNoTracking()
                .Include(x => x.CalculationResult)
                .Where(x => x.ProductId == productId)
                .OrderByDescending(x => x.CalculationResult.CalculationDate)
                .ToListAsync();
        }
    }
}
