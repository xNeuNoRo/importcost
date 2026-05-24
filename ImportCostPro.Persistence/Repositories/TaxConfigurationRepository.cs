using ImportCostPro.Persistence.Contexts;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Persistence.Repositories
{
    public class TaxConfigurationRepository
        : GenericRepository<TaxConfiguration>, // Reutilizamos el crud para redirigir las ops. de
            // update y add y asi ahorramos logica repetida.
            ITaxConfigurationRepository
    {
        public TaxConfigurationRepository(AppDbContext context)
            : base(context) { }

        public async Task<TaxConfiguration?> GetSingleConfigurationAsync()
        {
            return await _dbSet.AsNoTracking().SingleOrDefaultAsync();
        }

        // Redirigimos las operaciones de actualización y creación
        // al GenericRepository para evitar duplicación de lógica.

        public new async Task UpdateAsync(TaxConfiguration config)
        {
            await base.UpdateAsync(config);
        }

        public new async Task AddAsync(TaxConfiguration config)
        {
            await base.AddAsync(config);
        }
    }
}
