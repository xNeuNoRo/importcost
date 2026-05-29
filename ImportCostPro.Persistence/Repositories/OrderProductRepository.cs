using ImportCostPro.Persistence.Contexts;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using ImportCostPro.Persistence.Projections;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Persistence.Repositories
{
    public class OrderProductRepository : GenericRepository<OrderProduct>, IOrderProductRepository
    {
        public OrderProductRepository(AppDbContext context)
            : base(context) { }

        public async Task<OrderProduct?> GetByIdWithProductAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(op => op.Product)
                .FirstOrDefaultAsync(op => op.Id == id);
        }

        public async Task<IEnumerable<OrderProduct>> GetProductsByOrderIdAsync(int importOrderId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(op => op.Product)
                    .ThenInclude(p => p.TariffCategory)
                .Where(op => op.ImportOrderId == importOrderId)
                .ToListAsync();
        }

        public async Task<bool> IsProductAlreadyInOrderAsync(int importOrderId, int productId)
        {
            return await _dbSet.AnyAsync(op =>
                op.ImportOrderId == importOrderId && op.ProductId == productId
            );
        }

        public async Task<OrderProrationTotalsProjection?> GetProrationTotalsAsync(
            int importOrderId
        )
        {
            return await _dbSet
                .Where(op => op.ImportOrderId == importOrderId)
                .GroupBy(op => op.ImportOrderId)
                .Select(g => new OrderProrationTotalsProjection(
                    g.Sum(x => x.Quantity * x.UnitFobPrice), // Total FOB
                    g.Sum(x => x.Quantity * x.Product.UnitWeight), // Total Peso
                    g.Sum(x =>
                        (decimal)x.Quantity
                        * (x.Product.Length ?? 0) // El volumen se calcula como la cantidad por el ancho por el largo por el alto, considerando que alguna de las dimensiones pueden ser nulas, en ese caso se toma como 0.
                        * (x.Product.Width ?? 0)
                        * (x.Product.Height ?? 0)
                    ), // Total Volumen
                    g.Sum(x => x.Quantity) // Total Cantidad de mercancia
                ))
                .FirstOrDefaultAsync();
        }
    }
}
