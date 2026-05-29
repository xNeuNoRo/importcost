using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Projections;

namespace ImportCostPro.Persistence.Interfaces.Repositories
{
    public interface IOrderProductRepository : IGenericRepository<OrderProduct>
    {
        /// <summary>
        /// Obtiene un producto asociado a una orden de importación por su ID, incluyendo los detalles del producto.
        /// </summary>
        Task<OrderProduct?> GetByIdWithProductAsync(int id);

        /// <summary>
        /// Obtiene los productos asociados a una orden de importación, incluyendo los detalles del producto.
        /// </summary>
        Task<IEnumerable<OrderProduct>> GetProductsByOrderIdAsync(int importOrderId);

        /// <summary>
        /// Verifica si un producto ya está asociado a una orden de importación específica para evitar duplicados.
        /// </summary>
        Task<bool> IsProductAlreadyInOrderAsync(int importOrderId, int productId);

        /// <summary>
        /// Calcula los totales necesarios para la promediación de costos de una orden de importación,
        /// incluyendo el total FOB original, el peso total, el volumen total y la cantidad total de mercancía.
        /// </summary>
        Task<OrderProrationTotalsProjection?> GetProrationTotalsAsync(int importOrderId);
    }
}
