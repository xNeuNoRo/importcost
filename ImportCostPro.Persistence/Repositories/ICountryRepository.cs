using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Persistence.Repositories
{
    public interface ICountryRepository
    {
        /// Obtiene todos los países.
        Task<IEnumerable<Country>> GetAllAsync();

        /// Obtiene un país por su ID.
        Task<Country?> GetByIdAsync(int id);

        /// Obtiene un país por su código ISO.
        Task<Country?> GetByIsoCodeAsync(string isoCode);

        /// Agrega un nuevo país.
        Task AddAsync(Country country);

        /// Actualiza un país existente.
        Task UpdateAsync(Country country);

        /// Verifica si existe un país con el código ISO especificado.
        Task<bool> ExistsByIsoCodeAsync(string isoCode);

        /// Verifica si existe un país con el ID especificado.
        Task<bool> ExistsByIdAsync(int id);

        /// Elimina un país por su ID.
        Task DeleteAsync(int id);
    }
}
