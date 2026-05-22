using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Persistence.Interfaces.Repositories
{
    public interface IImportersRepository : IGenericRepository<Importer>
    {
        Task<IEnumerable<Importer>> GetAllWithAsync();

        Task<Importer?> GetByIAsync(int id);
     
     // Validar que los Rnc no se repitan entre importadores
        Task<bool> ExistTaxIdAsync(string RNC, int? excludeId = null);
    
     
    }
}
