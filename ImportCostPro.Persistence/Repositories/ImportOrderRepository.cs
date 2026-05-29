using ImportCostPro.Persistence.Contexts;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Enums;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Persistence.Repositories
{
    public class ImportOrderRepository : GenericRepository<ImportOrder>, IImportOrderRepository
    {
        public ImportOrderRepository(AppDbContext context)
            : base(context) { }

        public new async Task AddAsync(ImportOrder entity)
        {
            await base.AddAsync(entity);
        }

        public new Task UpdateAsync(ImportOrder entity)
        {
            return base.UpdateAsync(entity);
        }

        public new async Task<ImportOrder?> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public new async Task<bool> DeleteAsync(int id)
        {
            var rowsAffected = await _dbSet.Where(o => o.Id == id).ExecuteDeleteAsync();
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<ImportOrder>> GetAllWithRelationsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(o => o.Importer)
                .Include(o => o.Supplier)
                .Include(o => o.OriginCountry)
                .Include(o => o.Currency)
                .Include(o => o.OrderProducts)
                .Include(o => o.CalculationResults)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<ImportOrder?> GetByIdWithRelationsAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(o => o.Importer)
                .Include(o => o.Supplier)
                .Include(o => o.OriginCountry)
                .Include(o => o.Currency)
                .Include(o => o.OrderProducts)
                .Include(o => o.CalculationResults)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<bool> ExistsByOrderNumberAsync(string orderNumber, int? excludeId = null)
        {
            string cleanNumber = orderNumber.Trim();

            var query = _dbSet.Where(o => o.OrderNumber == cleanNumber);

            if (excludeId.HasValue)
            {
                query = query.Where(o => o.Id != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<OrderStatus?> GetStatusByIdAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(o => o.Id == id)
                .Select(o => (OrderStatus?)o.Status)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateStatusAsync(int id, OrderStatus newStatus)
        {
            // Para cambiar el estado de la orden (Abierta => Calculada => Cerrada),
            // no hacemos un SELECT o carga innecesaria. Solo le decimos q modifique el campo de Status.
            // Esto es tres mil veces más eficiente, ya que evitamos cargar toda la orden y sus relaciones a memoria.
            var rowsAffected = await _dbSet
                .Where(o => o.Id == id)
                .ExecuteUpdateAsync(setters => setters.SetProperty(o => o.Status, newStatus));

            // si es mayor a 0, obviamente algo se actualizo
            return rowsAffected > 0;
        }

        public async Task<ImportOrder?> GetAggregateForCalculationAsync(int id)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(o => o.Currency)
                .Include(o => o.Expenses)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                        .ThenInclude(p => p.TariffCategory)
                .Where(o => o.Id == id)
                .FirstOrDefaultAsync();
        }
    }
}
