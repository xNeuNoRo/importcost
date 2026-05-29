using ImportCostPro.Application.DTOs.TaxConfiguration.Requests;
using ImportCostPro.Application.DTOs.TaxConfiguration.Responses;
using ImportCostPro.Application.Extensions;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;

namespace ImportCostPro.Application.Services
{
    public class TaxConfigurationService
    {
        private readonly ITaxConfigurationRepository _taxConfigurationRepository;

        public TaxConfigurationService(ITaxConfigurationRepository taxConfigurationRepository)
        {
            _taxConfigurationRepository = taxConfigurationRepository;
        }

        public async Task<TaxConfigurationResponse> GetConfigurationAsync()
        {
            var config = await _taxConfigurationRepository.GetSingleConfigurationAsync();

            if (config == null)
            {
                return new TaxConfigurationResponse
                {
                    ItbisPercentage = 0,
                    CustomsServiceRatePercentage = 0,
                };
            }

            return config.ToResponse();
        }

        public async Task<TaxConfigurationResponse> SaveConfigurationAsync(
            SaveTaxConfigurationRequest request
        )
        {
            decimal normalizedItbis = request.ItbisPercentage;
            decimal normalizedCustoms = request.CustomsServiceRatePercentage;

            var currentConfig = await _taxConfigurationRepository.GetSingleConfigurationAsync();

            if (currentConfig == null)
            {
                var newConfig = TaxConfiguration.Create(normalizedItbis, normalizedCustoms);

                try
                {
                    await _taxConfigurationRepository.AddAsync(newConfig);
                    return newConfig.ToResponse();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException) // Si arrojo una excepcion de actualizacion,
                // lo mas probable es q sea por una condicion de carrera, asi que intentamos
                // obtener el registro nuevamente para devolverlo
                {
                    currentConfig = await _taxConfigurationRepository.GetSingleConfigurationAsync();

                    if (currentConfig == null)
                    {
                        throw;
                    }
                }
            }

            // Flujo regular de actualización o rescate del Catch de concurrencia
            currentConfig.UpdateRates(normalizedItbis, normalizedCustoms);

            await _taxConfigurationRepository.UpdateAsync(currentConfig);

            return currentConfig.ToResponse();
        }
    }
}
