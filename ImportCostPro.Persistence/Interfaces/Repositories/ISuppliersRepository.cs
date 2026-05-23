using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Persistence.Interfaces.Repositories
{
    public interface ISuppliersRepository : IGenericRepository<Supplier>
    {
        // Implementar conseguir el provedor con la moneda y pais de origen
        Task<IEnumerable<Supplier>> ExistsByCurrencyAndOriginCountryAsync(string currency, string originCountry, int? excludeId = null);

        // Metodo que verifica si ya existe un proveedor con el mismo nombre, permitiendo excluir un ID en caso de actualizaciones.
        Task <bool> ExistsByNameAsync(string Name, int? excludeId = null);

    }
}