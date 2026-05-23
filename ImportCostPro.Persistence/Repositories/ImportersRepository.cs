using ImportCostPro.Persistence.Contexts;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Persistence.Repositories
{
    public class ImporterRepository : GenericRepository<Importer>, IImportersRepository
    {
        public ImporterRepository(AppDbContext context)
            : base(context) { }

        public async Task<IEnumerable<Importer>> GetAllWithAsync()
        {
            // Incluimos la entidad relacionada Country para obtener el nombre del país junto con los importadores

            return await _dbSet
                .AsNoTracking()
                .Include(i => i.Country)
                .OrderBy(i => i.ComercialName)
                .ToListAsync();
        }

        public async Task<Importer?> GetByIAsync(int id)
        {
            // Incluimos la entidad relacionada Country para obtener el nombre del país junto con el importador específico
            return await _dbSet.Include(i => i.Country).FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<bool> ExistTaxIdAsync(string taxId, int? excludeId = null)
        {
            string cleanTaxId = taxId.Trim();

            if (excludeId.HasValue)
            {
                return await _dbSet.AnyAsync(i => i.RNC == cleanTaxId && i.Id != excludeId.Value);
            }

            return await _dbSet.AnyAsync(i => i.RNC == cleanTaxId);
        }
    }
}
