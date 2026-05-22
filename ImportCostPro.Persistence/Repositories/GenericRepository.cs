using ImportCostPro.Persistence.Common;
using ImportCostPro.Persistence.Contexts;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Persistence.Repositories
{
    /// <summary>
    /// Repositorio genérico base para operaciones CRUD estándar a lo largo del proyecto.
    /// Asi no se nos queman las manos de tanto escribir cruds xd
    /// </summary>
    /// <typeparam name="T">La entidad que hereda de BaseEntity</typeparam>
    public class GenericRepository<T> : IGenericRepository<T>
        where T : BaseEntity
    {
        // Protected para que si un repositorio en especifico hereda de este,
        // pueda acceder al contexto y al DbSet de la entidad T si es q requiere
        // implementar mas metodos aparte de los estandar del CRUD
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        /// <summary>
        /// Obtiene todos los registros de la entidad T.
        /// </summary>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Obtiene un registro por su ID.
        /// </summary>
        /// <param name="id">EL ID del registro a obtener</param>
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Id == id);
        }

        /// <summary>
        /// Agrega un nuevo registro de la entidad T a la base de datos.
        /// </summary>
        /// <param name="entity">El registro a agregar</param>
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Actualiza un registro de la entidad T en la base de datos.
        /// </summary>
        /// <param name="entity">El registro a actualizar</param>
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina un registro de la entidad T de la base de datos.
        /// </summary>
        /// <param name="id">El ID del registro a eliminar</param>
        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Verifica si existe un registro de la entidad T con el ID especificado.
        /// </summary>
        /// <param name="id">El ID del registro a verificar</param>
        public async Task<bool> ExistsByIdAsync(int id)
        {
            return await _dbSet.AnyAsync(e => e.Id == id);
        }
    }
}
