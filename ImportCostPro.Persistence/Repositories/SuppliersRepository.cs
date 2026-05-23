using ImportCostPro.Persistence.Contexts;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Persistence.Repositories
{
    public class SuppliersRepository : GenericRepository<Supplier>, ISuppliersRepository
    {
        public SuppliersRepository(AppDbContext context)
            : base(context)
        {
            
        }

        public new async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.Country)
                .Include(s => s.Currency)
                .OrderBy(s => s.Name) 
                .ToListAsync();
        }

        public new  async Task<Supplier?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(s => s.Country)
                .Include(s => s.Currency)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task <bool> ExistsByNameAsync(string Name, int? excludeId = null)
        {
            string cleanName = Name.Trim().ToLower(); // evitamos nombres duplicados tanto con mayusculas/minusculas
            if (excludeId.HasValue)
            {
                return await _dbSet.AnyAsync(s => s.Name.ToLower() == cleanName && s.Id != excludeId.Value);
            }

            return await _dbSet.AnyAsync(s => s.Name.ToLower() == cleanName);
        }
        /// <summary>
        ///  Obtenemos los provedores filtrados por moneda y pais especifico.
        /// </summary>
        
        public async Task<IEnumerable<Supplier>> ExistsByCurrencyAndOriginCountryAsync(string currency, string originCountry, int? excludeId = null)
        {
            string cleanCurrency = currency.Trim().ToLower();
            string cleanOriginCountry = originCountry.Trim().ToLower();

            // cargamos el pais y la moneda 

            var query = _dbSet
                .Include(s => s.Country)
                .Include(s => s.Currency)
                .AsNoTracking();

            if (excludeId.HasValue)
            {
                // si estamos editando un proveedor, excluimos el ID del filtro
            return await query.Where
                    (s => s.Id != excludeId.Value && 
                    s.Currency != null && s.Currency.Name.ToLower() == cleanCurrency 
                    && s.Country != null && s.Country.Name.ToLower() == cleanOriginCountry 
                    && s.Id != excludeId.Value)
                    .ToListAsync();
            }
               // en caso de ser consulta limpia de busqueda o filtrado
            return await query.Where
                    (s => s.Currency != null && s.Currency.Name.ToLower() == cleanCurrency 
                    && s.Country != null && s.Country.Name.ToLower() == cleanOriginCountry)
                    .ToListAsync();
        }
    }
}
    