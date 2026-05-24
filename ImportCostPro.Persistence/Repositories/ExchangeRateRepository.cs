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

        public async Task<IEnumerable<ExchangeRate>> GetAllWithCurrenciesAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(e => e.FromCurrency)
                .Include(e => e.ToCurrency)
                .OrderByDescending(e => e.EffectiveDate)
                .ToListAsync();
        }

        public async Task<ExchangeRate?> GetLatestActiveRateAsync(
            int fromCurrencyId,
            int toCurrencyId,
            DateTime date
        )
        {
            var targetDate = date.Date;

            return await _dbSet
                .AsNoTracking()
                .Where(e =>
                    e.FromCurrencyId == fromCurrencyId
                    && e.ToCurrencyId == toCurrencyId
                    && e.IsActive
                    && e.EffectiveDate <= targetDate
                )
                .OrderByDescending(e => e.EffectiveDate)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsActiveDuplicateAsync(
            int fromCurrencyId,
            int toCurrencyId,
            DateTime effectiveDate,
            int? excludeId = null
        )
        {
            var targetEffectiveDate = effectiveDate.Date;

            var query = _dbSet.Where(e =>
                e.FromCurrencyId == fromCurrencyId
                && e.ToCurrencyId == toCurrencyId
                && e.EffectiveDate == targetEffectiveDate
                && e.IsActive
            );

            if (excludeId.HasValue)
            {
                query = query.Where(e => e.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
