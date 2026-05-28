using ImportCostPro.Persistence.Contexts;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Persistence.Repositories
{
    public class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(AppDbContext context)
            : base(context) { }

        public async Task<IEnumerable<Supplier>> GetAllWithRelationsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.OriginCountry)
                .Include(s => s.DefaultCurrency)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Supplier?> GetByIdWithRelationsAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.OriginCountry)
                .Include(s => s.DefaultCurrency)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            string cleanName = name.Trim();

            var query = _dbSet.Where(s => s.Name == cleanName);

            if (excludeId.HasValue)
            {
                query = query.Where(s => s.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<Supplier>> GetByCurrencyAndCountryAsync(
            int defaultCurrencyId,
            int originCountryId
        )
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.OriginCountry)
                .Include(s => s.DefaultCurrency)
                .Where(s =>
                    s.DefaultCurrencyId == defaultCurrencyId && s.OriginCountryId == originCountryId
                )
                .ToListAsync();
        }

        public async Task<bool> IsSupplierReferencedAsync(int supplierId)
        {
            return await _context.Set<ImportOrder>().AnyAsync(o => o.SupplierId == supplierId);
        }
    }
}
