using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportCostPro.Persistence.Contexts;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Persistence.Repositories
{
    public class ImporterRepository : GenericRepository<Importer>, IImporterRepository
    {
        public ImporterRepository(AppDbContext context)
            : base(context) { }

        public async Task<IEnumerable<Importer>> GetAllWithCountryAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(i => i.Country)
                .OrderBy(i => i.LegalName) // Ordenamos por nombre legal para mejor UX
                .ToListAsync();
        }

        public async Task<Importer?> GetByIdWithCountryAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(i => i.Country)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<bool> ExistsTaxIdAsync(string taxId, int? excludeId = null)
        {
            string cleanTaxId = taxId.Trim();

            var query = _dbSet.Where(i => i.TaxId == cleanTaxId);

            if (excludeId.HasValue)
            {
                query = query.Where(i => i.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
