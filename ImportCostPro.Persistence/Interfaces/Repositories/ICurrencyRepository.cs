using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Persistence.Interfaces.Repositories
{
    public interface ICurrencyRepository : IGenericRepository<Currency>
    {
        /// <summary>
        /// Obtiene la moneda local del sistema, que se utiliza como referencia para los cálculos de costos y reportes financieros.
        /// </summary>
        Task<Currency?> GetLocalCurrencyAsync();

        /// <summary>
        /// Verifica si existe una moneda con el código ISO especificado.
        /// </summary>
        Task<bool> ExistsByIsoCodeAsync(string isoCode, int? excludeId = null);

        /// <summary>
        /// Verifica si ya existe una moneda marcada como moneda local.
        /// </summary>
        Task<bool> AnyLocalCurrencyAsync(int? excludingId = null);

        /// <summary>
        /// Verifica si la moneda está referenciada por otras entidades del sistema
        /// (tasas, proveedores, órdenes, gastos, resultados de landed cost, etc.).
        /// </summary>
        Task<bool> IsCurrencyReferencedAsync(int currencyId);
    }
}
