using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Persistence.Interfaces.Repositories
{
    public interface IExchangeRateRepository : IGenericRepository<ExchangeRate>
    {

        /// Obtiene todas las tasas de cambio incluyendo las entidades de moneda relacionadas.
        Task<IEnumerable<ExchangeRate>> GetAllWithCurrenciesAsync();

        /// Recupera la última tasa activa entre dos monedas para una fecha dada.
        Task<ExchangeRate?> GetLatestActiveRateAsync(int originCurrencyId, int destinationCurrencyId, DateTime date);

        /// Determina si existe una tasa activa duplicada para la misma combinación de monedas y fecha de vigencia.
        Task<bool> ExistsActiveDuplicateAsync(int originCurrencyId, int destinationCurrencyId, DateTime effectiveDate, int? excludeId = null);
    }
}
