using ImportCostPro.Persistence.Contexts;
using ImportCostPro.Persistence.Interfaces.Providers;
using ImportCostPro.Persistence.Interfaces.Repositories;
using ImportCostPro.Persistence.Providers;
using ImportCostPro.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ImportCostPro.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            // Configuramos el DbContext para usar SQL Server con el connection string definido en appsettings.json
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ImportCostDb"))
            );

            // Registramos el repo generico para que pueda ser inyectado en los otros repositorios
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Registramos cada repo
            services.AddScoped<
                ICalculationResultDetailRepository,
                CalculationResultDetailRepository
            >();
            services.AddScoped<ICalculationResultRepository, CalculationResultRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
            services.AddScoped<IImporterRepository, ImporterRepository>();
            services.AddScoped<IImportExpenseRepository, ImportExpenseRepository>();
            services.AddScoped<IImportOrderRepository, ImportOrderRepository>();
            services.AddScoped<IOrderProductRepository, OrderProductRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<ITariffCategoryRepository, TariffCategoryRepository>();
            services.AddScoped<ITaxConfigurationRepository, TaxConfigurationRepository>();

            // Providers
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            return services;
        }
    }
}
