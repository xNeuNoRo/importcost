using ImportCostPro.Persistence.Contexts;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Persistence.Repositories
{
    public class CountryRepository : GenericRepository<Country>, ICountryRepository
    {
        public CountryRepository(AppDbContext context)
            : base(context) { }

        /// Obtiene un país por su código ISO.
        public async Task<Country?> GetByIsoCodeAsync(string isoCode)
        {
            string cleanIso = isoCode.Trim();
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.IsoCode == cleanIso);
        }

        /// Verifica si existe un país con el código ISO especificado.
        public async Task<bool> ExistsByIsoCodeAsync(string isoCode, int? excludeId = null)
        {
            string cleanIso = isoCode.Trim();

            var query = _dbSet.Where(c => c.IsoCode == cleanIso);

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> IsCountryReferencedAsync(int countryId)
        {
            bool isUsedInImporters = await _context
                .Set<Importer>()
                .AnyAsync(i => i.CountryId == countryId);
            bool isUsedInSuppliers = await _context
                .Set<Supplier>()
                .AnyAsync(s => s.OriginCountryId == countryId);

            // En products seria algo como esto:
            // bool isUsedInProducts = await _context.Set<Product>().AnyAsync(p => p.OriginCountryId == countryId);

            return isUsedInImporters || isUsedInSuppliers;
        }
    }
}
