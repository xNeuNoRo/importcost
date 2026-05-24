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

        public async Task<IEnumerable<ImportOrder>> GetAllWithRelationsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(o => o.Importer)
                .Include(o => o.Supplier)
                .Include(o => o.OriginCountry)
                .Include(o => o.Currency)
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
            // Para cambiar el estado de la orden (Abierta -> Calculada -> Cerrada),
            // no hacemos un SELECT innecesario. Instanciamos una entidad ligera, y le decimos al change trackker que solo
            // modificamos el campo de Status. Esto es tres mil veces más eficiente,
            // ya que evitamos cargar toda la orden y sus relaciones a memoria.
            var order = new ImportOrder
            {
                Id = id,
                OrderNumber = string.Empty, // Dummy string reglamentario para cumplir con el 'required' de C#
                OrderDate = DateTime.MinValue,
                TransportMode = TransportMode.Maritime,
                Status = newStatus,
            };

            _context.Entry(order).Property(o => o.Status).IsModified = true;

            // el savechanges nos va a devolver el numero de filas q actualizamos
            var rowsAffected = await _context.SaveChangesAsync();

            // si es mayor a 0, obviamente algo se actualizo
            return rowsAffected > 0;
        }
    }
}
