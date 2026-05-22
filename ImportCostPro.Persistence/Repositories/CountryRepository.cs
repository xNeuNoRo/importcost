using ImportCostPro.Persistence.Contexts;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Persistence.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly AppDbContext _context;

        public CountryRepository(AppDbContext context)
        {
            _context = context;
        }

        /// Obtiene todos los países.
        public async Task<IEnumerable<Country>> GetAllAsync()
        {
            return await _context.Countries.ToListAsync();
        }

        /// Obtiene un país por su ID.
        public async Task<Country?> GetByIdAsync(Guid id)
        {
            return await _context.Countries.FirstOrDefaultAsync(c => c.Id == id);
        }

        /// Obtiene un país por su código ISO.
        public async Task<Country?> GetByIsoCodeAsync(string isoCode)
        {
            return await _context.Countries.FirstOrDefaultAsync(c => c.IsoCode == isoCode);
        }

        /// Agrega un nuevo país.
        public async Task AddAsync(Country country)
        {
            await _context.Countries.AddAsync(country);
            await _context.SaveChangesAsync();
        }

        /// Actualiza un país existente.
        public async Task UpdateAsync(Country country)
        {
            _context.Countries.Update(country);
            await _context.SaveChangesAsync();
        }

        /// Verifica si existe un país con el código ISO especificado.
        public async Task<bool> ExistsByIsoCodeAsync(string isoCode)
        {
            return await _context.Countries.AnyAsync(c => c.IsoCode == isoCode);
        }

        /// Verifica si existe un país con el ID especificado.
        public async Task<bool> ExistsByIdAsync(Guid id)
        {
            return await _context.Countries.AnyAsync(c => c.Id == id);
        }

        /// Elimina un país por su ID.
        public async Task DeleteAsync(Guid id)
        {
            var country = await GetByIdAsync(id);
            if (country != null)
            {
                _context.Countries.Remove(country);
                await _context.SaveChangesAsync();
            }
        }
    }
}
