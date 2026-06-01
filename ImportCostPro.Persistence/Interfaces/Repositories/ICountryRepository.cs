using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Persistence.Interfaces.Repositories
{
    public interface ICountryRepository : IGenericRepository<Country>
    {
        /// <summary>
        /// Obtiene un país por su código ISO.
        /// </summary>
        Task<Country?> GetByIsoCodeAsync(string isoCode);

        /// <summary>
        /// Verifica si existe un país con el código ISO especificado.
        /// </summary>
        Task<bool> ExistsByIsoCodeAsync(string isoCode, int? excludeId = null);

        /// <summary>
        /// Verifica si ya existe un país registrado con el nombre especificado.
        /// </summary>
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);

        /// <summary>
        /// Verifica si el país está referenciado por otras entidades del sistema
        /// </summary>
        Task<bool> IsCountryReferencedAsync(int countryId);
    }
}
