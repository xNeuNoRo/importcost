using ImportCostPro.Persistence.Contexts;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Persistence.Repositories
{
    public class TariffCategoryRepository
        : GenericRepository<TariffCategory>,
            ITariffCategoryRepository
    {
        public TariffCategoryRepository(AppDbContext context)
            : base(context) { }

        public async Task<TariffCategory?> GetByCodeAsync(string code)
        {
            string cleanCode = code.Trim();
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(t => t.Code == cleanCode);
        }

        public async Task<bool> ExistsByCodeAsync(string code, int? excludeId = null)
        {
            string cleanCode = code.Trim();

            var query = _dbSet.Where(t => t.Code == cleanCode);

            if (excludeId.HasValue)
            {
                query = query.Where(t => t.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> IsTariffCategoryReferencedAsync(int tariffCategoryId)
        {
            return await _context
                .Set<Product>()
                .AnyAsync(p => p.TariffCategoryId == tariffCategoryId);
        }
    }
}
