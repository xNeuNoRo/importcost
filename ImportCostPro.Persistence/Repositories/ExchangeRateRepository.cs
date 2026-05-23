using ImportCostPro.Persistence.Contexts;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Persistence.Repositories
{
    public class ExchangeRateRepository : GenericRepository<ExchangeRate>, IExchangeRateRepository
    {
        public ExchangeRateRepository(AppDbContext context) 
            : base(context) { }

        /// <summary>
        /// Obtiene todas las tasas de cambio incluyendo las entidades de moneda de origen y destino,
        /// ordenadas de más reciente a más antiguo por la fecha efectiva.
        /// </summary>
        public async Task<IEnumerable<ExchangeRate>> GetAllWithCurrenciesAsync()
        {
            return await _context.Set<ExchangeRate>()
                .Include(e => e.OriginCurrency)
                .Include(e => e.DestinationCurrency)
                .OrderByDescending(e => e.EffectiveDate)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene la última tasa activa para una combinación de monedas dada en o antes de la fecha indicada.
        /// </summary>
        public async Task<ExchangeRate?> GetLatestActiveRateAsync(int originCurrencyId, int destinationCurrencyId, DateTime date)
        {
            return await _context.Set<ExchangeRate>()
                .Where(e => e.OriginCurrencyId == originCurrencyId &&
                            e.DestinationCurrencyId == destinationCurrencyId &&
                            e.IsActive &&
                            e.EffectiveDate.Date <= date.Date)
                .OrderByDescending(e => e.EffectiveDate)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Comprueba si existe otra tasa activa con la misma combinación de monedas y fecha efectiva,
        /// opcionalmente excluyendo un registro por su Id.
        /// </summary>
        public async Task<bool> ExistsActiveDuplicateAsync(int originCurrencyId, int destinationCurrencyId, DateTime effectiveDate, int? excludeId = null)
        {
            var query = _context.Set<ExchangeRate>()
                .Where(e => e.OriginCurrencyId == originCurrencyId &&
                            e.DestinationCurrencyId == destinationCurrencyId &&
                            e.EffectiveDate.Date == effectiveDate.Date &&
                            e.IsActive);

            if (excludeId.HasValue)
            {
                query = query.Where(e => e.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
