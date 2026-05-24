using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Persistence.Interfaces.Repositories
{
    public interface IImportOrderRepository : IGenericRepository<ImportOrder>
    {
        /// <summary>
        /// Obtiene el listado de órdenes con todas sus relaciones.
        /// </summary>
        Task<IEnumerable<ImportOrder>> GetAllWithRelationsAsync();

        /// <summary>
        /// Obtiene los detalles de la orden incluyendo todas sus relaciones.
        /// </summary>
        Task<ImportOrder?> GetByIdWithRelationsAsync(int id);

        /// <summary>
        /// Valida la existencia de un número de orden, permitiendo excluir un registro especifico.
        /// </summary>
        Task<bool> ExistsByOrderNumberAsync(string orderNumber, int? excludeId = null);

        /// <summary>
        /// Obtiene el estado actual de una orden de importación por su ID.
        /// </summary>
        Task<OrderStatus?> GetStatusByIdAsync(int id);

        /// <summary>
        /// Actualiza el estado de una orden de importación.
        /// </summary>
        Task<bool> UpdateStatusAsync(int id, OrderStatus newStatus);
    }
}
