using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Persistence.Interfaces.Repositories
{
    public interface IExchangeRateRepository : IGenericRepository<ExchangeRate>
    {
        /// <summary>
        /// Obtiene todas las tasas de cambio incluyendo las entidades de moneda (From/To) relacionadas.
        /// </summary>
        Task<IEnumerable<ExchangeRate>> GetAllWithCurrenciesAsync();

        /// <summary>
        /// Recupera la última tasa activa entre dos monedas para una fecha dada.
        /// </summary>
        Task<ExchangeRate?> GetLatestActiveRateAsync(
            int fromCurrencyId,
            int toCurrencyId,
            DateTime date
        );

        /// <summary>
        /// Determina si existe una tasa activa duplicada para la misma combinación de monedas y fecha de vigencia.
        /// </summary>
        Task<bool> ExistsActiveDuplicateAsync(
            int fromCurrencyId,
            int toCurrencyId,
            DateTime effectiveDate,
            int? excludeId = null
        );
    }
}
