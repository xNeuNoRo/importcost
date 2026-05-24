using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Persistence.Interfaces.Repositories
{
    public interface ICalculationResultRepository
    {
        /// <summary>
        /// Registra un nuevo resultado de cálculo en la bd.
        /// </summary>
        Task AddAsync(CalculationResult entity);

        /// <summary>
        /// Recupera un cálculo específico por su id.
        /// </summary>
        Task<CalculationResult?> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene el histórico completo de cálculos y simulaciones asociados a una orden específica,
        /// ordenados desde el más reciente al más antiguo.
        /// </summary>
        Task<IEnumerable<CalculationResult>> GetHistoryByOrderIdAsync(int importOrderId);

        /// <summary>
        /// Recupera el último cálculo oficial realizado para una orden de importación (el vigente),
        /// incluyendo de forma optimizada todo el desglose de productos (Details) para el cierre logístico.
        /// </summary>
        Task<CalculationResult?> GetLatestCalculatedResultWithDetailsAsync(int importOrderId);
    }
}
