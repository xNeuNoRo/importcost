using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImportCostPro.Persistence.Contexts;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Persistence.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context)
            : base(context) { }

        public async Task<IEnumerable<Product>> GetAllWithRelationsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(p => p.DefaultOriginCountry)
                .Include(p => p.TariffCategory)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdWithRelationsAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(p => p.DefaultOriginCountry)
                .Include(p => p.TariffCategory)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> ExistsByReferenceCodeAsync(
            string referenceCode,
            int? excludeId = null
        )
        {
            string cleanReference = referenceCode.Trim();

            var query = _dbSet.Where(p => p.ReferenceCode == cleanReference);

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> IsProductReferencedInOrdersAsync(int productId)
        {
            return await Task.FromResult(false); // Placeholder hasta que implementemos la entidad de Orden de Importación
        }
    }
}
