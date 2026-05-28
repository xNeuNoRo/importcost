using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Persistence.Interfaces.Repositories
{
    public interface ICalculationResultDetailRepository
    {
        /// <summary>
        /// Recupera un detalle específico para auditoría interna del motor de prorrateo.
        /// </summary>
        Task<CalculationResultDetail?> GetByIdAsync(int id);

        /// <summary>
        /// Obtiene el desglose completo de costos de todos los productos pertenecientes
        /// a un cálculo específico mediante su ID de cabecera.
        /// </summary>
        Task<IEnumerable<CalculationResultDetail>> GetDetailsByCalculationIdAsync(
            int calculationResultId
        );

        /// <summary>
        /// Obtiene el histórico de costos unitarios de importación (UnitLandedCost) de un producto
        /// a lo largo de las diferentes órdenes, permitiendo analizar la tendencia de costos en el tiempo.
        /// </summary>
        Task<IEnumerable<CalculationResultDetail>> GetCostHistoryByProductIdAsync(int productId);
    }
}
