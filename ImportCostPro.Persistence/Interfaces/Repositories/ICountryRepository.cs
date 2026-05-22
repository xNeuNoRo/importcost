using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Persistence.Interfaces.Repositories
{
    public interface ICountryRepository
    {
        /// <summary>
        /// Obtiene todos los países.
        /// </summary>
        Task<IEnumerable<Country>> GetAllAsync();

        /// <summary>
        /// Obtiene un país por su ID.
        /// </summary>
        Task<Country?> GetByIdAsync(Guid id);

        /// <summary>
        /// Obtiene un país por su código ISO.
        /// </summary>
        Task<Country?> GetByIsoCodeAsync(string isoCode);

        /// <summary>
        /// Agrega un nuevo país.
        /// </summary>
        Task AddAsync(Country country);

        /// <summary>
        /// Actualiza un país existente.
        /// </summary>
        Task UpdateAsync(Country country);

        /// <summary>
        /// Verifica si existe un país con el código ISO especificado.
        /// </summary>
        Task<bool> ExistsByIsoCodeAsync(string isoCode);

        /// <summary>
        /// Verifica si existe un país con el ID especificado.
        /// </summary>
        Task<bool> ExistsByIdAsync(Guid id);

        /// <summary>
        /// Elimina un país por su ID.
        /// </summary>
        Task DeleteAsync(Guid id);
    }
}
