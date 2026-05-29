using System.Reflection;
using FluentValidation;
using ImportCostPro.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ImportCostPro.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Registramos los validadores de FluentValidation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Registramos los servicios de aplicación
            services.AddScoped<CountryService>();
            services.AddScoped<CurrencyService>();
            services.AddScoped<ExchangeRateService>();
            services.AddScoped<ImporterService>();
            services.AddScoped<ImportExpenseService>();
            services.AddScoped<ImportOrderService>();
            services.AddScoped<LandedCostService>();
            services.AddScoped<OrderProductService>();
            services.AddScoped<ProductService>();
            services.AddScoped<SupplierService>();
            services.AddScoped<TariffCategoryService>();
            services.AddScoped<TaxConfigurationService>();

            return services;
        }
    }
}
