using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Persistence.Interfaces.Repositories
{
    public interface IExchangeRateRepository : IGenericRepository<ExchangeRate>
    {
        /// <summary>
        /// Obtiene una tasa de cambio por su ID incluyendo las entidades de moneda (From/To) relacionadas.
        /// </summary>
        Task<ExchangeRate?> GetByIdWithCurrenciesAsync(int id);

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

        /// <summary>
        /// Determina si una tasa de cambio está marcada como usada o referenciada
        /// mediante el indicador de la propia entidad, para proteger el historial.
        /// </summary>
        Task<bool> IsExchangeRateReferencedAsync(int id);

        /// <summary>
        /// Marca una tasa de cambio como usada o referenciada en el indicador de la propia entidad
        /// para evitar que sea eliminada físicamente.
        /// </summary>
        Task MarkAsUsedAsync(int id);

        /// <summary>
        /// Obtiene la cantidad de monedas extranjeras activas que no tienen una tasa de cambio registrada para hoy.
        /// </summary>
        Task<int> GetActiveCurrenciesMissingRateCountAsync(DateTime date);
    }
}
