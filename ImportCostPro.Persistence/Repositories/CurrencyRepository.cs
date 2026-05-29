using ImportCostPro.Persistence.Contexts;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Persistence.Repositories
{
    public class CurrencyRepository : GenericRepository<Currency>, ICurrencyRepository
    {
        public CurrencyRepository(AppDbContext context)
            : base(context) { }

        public async Task<Currency?> GetLocalCurrencyAsync()
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(c => c.IsLocalCurrency && c.IsActive);
        }

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

        public async Task<bool> AnyLocalCurrencyAsync(int? excludingId = null)
        {
            var query = _dbSet.Where(c => c.IsLocalCurrency);

            if (excludingId.HasValue)
            {
                query = query.Where(c => c.Id != excludingId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> IsCurrencyReferencedAsync(int currencyId)
        {
            bool isUsedInRates = await _context
                .Set<ExchangeRate>()
                .AnyAsync(er => er.FromCurrencyId == currencyId || er.ToCurrencyId == currencyId);

            bool isUsedInSuppliers = await _context
                .Set<Supplier>()
                .AnyAsync(s => s.DefaultCurrencyId == currencyId);

            bool isUsedInOrders = await _context
                .Set<ImportOrder>()
                .AnyAsync(io => io.CurrencyId == currencyId);

            bool isUsedInExpenses = await _context
                .Set<ImportExpense>()
                .AnyAsync(ie => ie.CurrencyId == currencyId);

            bool isUsedInCalculations = await _context
                .Set<CalculationResult>()
                .AnyAsync(cr => cr.LocalCurrencyUsedId == currencyId);

            return isUsedInRates
                || isUsedInSuppliers
                || isUsedInOrders
                || isUsedInExpenses
                || isUsedInCalculations;
        }
    }
}
