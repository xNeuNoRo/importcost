using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Persistence.Interfaces.Repositories
{
    public interface IImporterRepository : IGenericRepository<Importer>
    {
        /// <summary>
        /// Obtiene todos los importadores incluyendo la información de su país de origen.
        /// </summary>
        Task<IEnumerable<Importer>> GetAllWithCountryAsync();

        /// <summary>
        /// Obtiene un importador por su ID incluyendo la información de su país.
        /// </summary>
        Task<Importer?> GetByIdWithCountryAsync(int id);

        /// <summary>
        /// Verifica si ya existe un TaxId (RNC) registrado, permitiendo excluir un ID en caso de actualizaciones.
        /// </summary>
        Task<bool> ExistsTaxIdAsync(string taxId, int? excludeId = null);

        /// <summary>
        /// Verifica si ya existe un nombre legal o razón social registrado, permitiendo excluir un ID en caso de actualizaciones.
        /// </summary>
        Task<bool> ExistsLegalNameAsync(string legalName, int? excludeId = null);

        /// <summary>
        /// Determina si el importador está siendo referenciado por alguna orden de importación activa o histórica en el sistema.
        /// </summary>
        Task<bool> IsImporterReferencedAsync(int importerId);
    }
}
