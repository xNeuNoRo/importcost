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
        Task<bool> ExistsByIsoCodeAsync(string isoCode);
    }
}
