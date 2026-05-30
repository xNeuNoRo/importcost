using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Persistence.Interfaces.Repositories
{
    // Esta interfaz no hereda directamente de GenericRepository
    // para restringir las operaciones disponibles por seguridad
    public interface IImportOrderRepository
    {
        // SOLO USAREMOS EL ADDASYNC, UPDATEASYNC EL GETBYIDASYNC DE GENERICREPOSITORY

        /// <summary>
        /// Registra una nueva orden de importación en el sistema.
        /// </summary>
        Task AddAsync(ImportOrder entity);

        /// <summary>
        /// Actualiza una orden de importación existente en el sistema.
        /// </summary>
        Task UpdateAsync(ImportOrder entity);

        /// <summary>
        /// Elimina físicamente una orden de importación del sistema.
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Recupera una orden básica por su identificador único para validaciones previas de estado.
        /// </summary>
        Task<ImportOrder?> GetByIdAsync(int id);

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

        /// <summary>
        /// Recupera una orden de importación con toda su información relacionada optimizada para el proceso de cálculo,
        /// incluyendo productos, gastos y datos maestros necesarios para el motor de promediacion.
        /// </summary>
        Task<ImportOrder?> GetAggregateForCalculationAsync(int id);

        /// <summary>
        /// Cuenta las órdenes que se encuentran en un estado específico.
        /// </summary>
        Task<int> CountByStatusAsync(OrderStatus status);
    }
}
