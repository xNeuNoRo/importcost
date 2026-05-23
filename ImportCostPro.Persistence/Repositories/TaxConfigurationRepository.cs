using ImportCostPro.Persistence.Contexts;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Persistence.Repositories
{
    public class TaxConfigurationRepository
        : GenericRepository<TaxConfiguration>, // No importa q reutilicemos el crud,
            // solo se expondra por la interfaz los metodos necesarios para esta entidad
            ITaxConfigurationRepository
    {
        public TaxConfigurationRepository(AppDbContext context)
            : base(context) { }

        public async Task<TaxConfiguration?> GetSingleConfigurationAsync()
        {
            return await _dbSet.SingleOrDefaultAsync();
        }
    }
}
