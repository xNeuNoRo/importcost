using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Persistence.Interfaces.Repositories
{
    public interface ISuppliersRepository : IGenericRepository<Supplier>
    {
        /// <summary>
        /// Obtiene todos los proveedores con sus entidades de país y moneda inyectadas.
        /// </summary>
        Task<IEnumerable<Supplier>> GetAllWithRelationsAsync();

        /// <summary>
        /// Obtiene un proveedor específico por su ID incluyendo su país y moneda de negociación.
        /// </summary>
        Task<Supplier?> GetByIdWithRelationsAsync(int id);

        /// <summary>
        /// Verifica si ya existe un proveedor con el mismo nombre, permitiendo excluir un ID para actualizaciones.
        /// </summary>
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);

        /// <summary>
        /// Obtiene los proveedores filtrados eficientemente mediante los IDs de País y Moneda.
        /// </summary>
        Task<IEnumerable<Supplier>> GetByCurrencyAndCountryAsync(
            int defaultCurrencyId,
            int originCountryId
        );
    }
}
