using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Persistence.Interfaces.Repositories
{
    public interface ICurrencyRepository : IGenericRepository<Currency>
    {
        /// <summary>
        /// Verifica si existe una moneda con el código ISO especificado.
        /// </summary>
        Task<bool> ExistsByIsoCodeAsync(string isoCode);

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
