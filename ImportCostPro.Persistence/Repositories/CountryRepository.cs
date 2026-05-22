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
            return await _context.Countries.FirstOrDefaultAsync(c => c.IsoCode == isoCode);
        }

        /// Verifica si existe un país con el código ISO especificado.
        public async Task<bool> ExistsByIsoCodeAsync(string isoCode)
        {
            return await _context.Countries.AnyAsync(c => c.IsoCode == isoCode);
        }
    }
}
