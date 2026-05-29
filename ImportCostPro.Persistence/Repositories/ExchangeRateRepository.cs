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

        public async Task<ExchangeRate?> GetByIdWithCurrenciesAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(e => e.FromCurrency)
                .Include(e => e.ToCurrency)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

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

        public async Task<bool> IsExchangeRateReferencedAsync(int id)
        {
            return await _dbSet.AnyAsync(e => e.Id == id && e.IsUsedInOfficialCalculation);
        }

        public async Task MarkAsUsedAsync(int id)
        {
            await _dbSet
                .Where(e => e.Id == id && !e.IsUsedInOfficialCalculation)
                .ExecuteUpdateAsync(setters =>
                    // Solo actualizamos si no está marcado como usado para evitar escrituras innecesarias
                    setters.SetProperty(e => e.IsUsedInOfficialCalculation, _ => true)
                );
        }
    }
}
