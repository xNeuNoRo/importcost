using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Persistence.Interfaces.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        /// <summary>
        /// Obtiene todos los productos del catálogo incluyendo sus relaciones (País y Categoría Arancelaria).
        /// </summary>
        Task<IEnumerable<Product>> GetAllWithRelationsAsync();

        /// <summary>
        /// Obtiene un producto específico por su ID inyectando sus relaciones asociadas.
        /// </summary>
        Task<Product?> GetByIdWithRelationsAsync(int id);

        /// <summary>
        /// Verifica si ya existe un producto con el código de referencia (SKU) especificado,
        /// permitiendo excluir un ID para updates.
        /// </summary>
        Task<bool> ExistsByReferenceCodeAsync(string referenceCode, int? excludeId = null);

        /// <summary>
        /// Determina si el producto está asociado a alguna orden de importación activa o histórica.
        /// </summary>
        Task<bool> IsProductReferencedInOrdersAsync(int productId);
    }
}
