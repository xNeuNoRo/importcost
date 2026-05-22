using ImportCostPro.Persistence.Contexts;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ImportCostPro.Persistence.Repositories
{
    public class CurrencyRepository : GenericRepository<Currency>, ICurrencyRepository
    {
        public CurrencyRepository(AppDbContext context)
            : base(context) { }

        public async Task<bool> ExistsByIsoCodeAsync(string isoCode)
        {
            return await _context.Currencies.AnyAsync(c => c.IsoCode == isoCode);
        }

        public async Task<bool> AnyLocalCurrencyAsync(int? excludingId = null)
        {
            if (excludingId.HasValue)
            {
                return await _context.Currencies.AnyAsync(c =>
                    c.IsLocalCurrency && c.Id != excludingId.Value
                );
            }

            return await _context.Currencies.AnyAsync(c => c.IsLocalCurrency);
        }

        public async Task<bool> IsCurrencyReferencedAsync(int currencyId)
        {
            // // Inspeccionamos los DbSet del contexto y buscamos entidades que tengan una propiedad int CurrencyId
            // // o una propiedad navigation hacia Currency
            // var contextType = _context.GetType();
            // var properties = contextType.GetProperties();

            // foreach (var prop in properties)
            // {
            //     // Solo DbSet<T>
            //     if (!prop.PropertyType.IsGenericType) continue;
            //     var gen = prop.PropertyType.GetGenericTypeDefinition();
            //     if (gen != typeof(DbSet<>)) continue;

            //     var entityType = prop.PropertyType.GetGenericArguments()[0];

            //     // Construimos una consulta AnyAsync dinámicamente: buscamos si existe alguna entidad con CurrencyId == currencyId
            //     var propCurrencyId = entityType.GetProperty("CurrencyId");
            //     if (propCurrencyId != null && (propCurrencyId.PropertyType == typeof(int) || propCurrencyId.PropertyType == typeof(int?)))
            //     {
            //         // Obtener el DbSet
            //         var dbSet = prop.GetValue(_context);
            //         // Construir query: ((IQueryable)dbSet).Cast<object>().AnyAsync(e => EF.Property<int?>(e, "CurrencyId") == currencyId)
            //         var queryable = dbSet as IQueryable<object>;
            //         if (queryable == null) continue;

            //         // Usar LINQ dinámico con EF.Property via reflection
            //         var methodAnyAsync = typeof(EntityFrameworkQueryableExtensions)
            //             .GetMethods()
            //             .First(m => m.Name == "AnyAsync" && m.GetParameters().Length == 2)
            //             .MakeGenericMethod(entityType);

            //         // Construir lambda: e => EF.Property<int?>(e, "CurrencyId") == currencyId
            //         var parameter = System.Linq.Expressions.Expression.Parameter(entityType, "e");
            //         var efPropertyMethod = typeof(EF).GetMethods().First(m => m.Name == "Property" && m.IsGenericMethod && m.GetParameters().Length == 2)
            //             .MakeGenericMethod(propCurrencyId.PropertyType);
            //         var body = System.Linq.Expressions.Expression.Equal(
            //             System.Linq.Expressions.Expression.Call(efPropertyMethod, parameter, System.Linq.Expressions.Expression.Constant("CurrencyId")),
            //             System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Constant(currencyId), propCurrencyId.PropertyType)
            //         );
            //         var lambdaType = typeof(Func<,>).MakeGenericType(entityType, typeof(bool));
            //         var lambda = System.Linq.Expressions.Expression.Lambda(lambdaType, body, parameter);

            //         var task = methodAnyAsync.Invoke(null, new object[] { prop.GetValue(_context)!, lambda });
            //         if (task is Task<bool> t)
            //         {
            //             if (await t) return true;
            //         }
            //     }

            //     // También verificar propiedades de navegación que apunten a Currency
            //     var navProps = entityType.GetProperties().Where(p => p.PropertyType == typeof(Currency));
            //     if (navProps.Any())
            //     {
            //         var dbSet = prop.GetValue(_context) as IQueryable<object>;
            //         if (dbSet == null) continue;

            //         // Intentamos buscar any e => e.Currency != null && e.Currency.Id == currencyId
            //         var entityParam = System.Linq.Expressions.Expression.Parameter(entityType, "e");
            //         var currencyProp = navProps.First();
            //         var currencyAccess = System.Linq.Expressions.Expression.Property(entityParam, currencyProp);
            //         var idAccess = System.Linq.Expressions.Expression.Property(currencyAccess, "Id");
            //         var body = System.Linq.Expressions.Expression.Equal(idAccess, System.Linq.Expressions.Expression.Constant(currencyId));
            //         var lambdaType = typeof(Func<,>).MakeGenericType(entityType, typeof(bool));
            //         var lambda = System.Linq.Expressions.Expression.Lambda(lambdaType, body, entityParam);

            //         var methodAnyAsync = typeof(EntityFrameworkQueryableExtensions)
            //             .GetMethods()
            //             .First(m => m.Name == "AnyAsync" && m.GetParameters().Length == 2)
            //             .MakeGenericMethod(entityType);

            //         var task = methodAnyAsync.Invoke(null, new object[] { prop.GetValue(_context)!, lambda });
            //         if (task is Task<bool> t)
            //         {
            //             if (await t) return true;
            //         }
            //     }
            // }

            // return false;

            throw new NotImplementedException(
                "Pendiente de implementar hasta que las entidades dependientes (Proveedores, Órdenes, etc.) existan en el AppDbContext."
            );
        }
    }
}
