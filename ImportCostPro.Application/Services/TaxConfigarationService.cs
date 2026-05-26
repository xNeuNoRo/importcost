using ImportCostPro.Application.DTOs.TaxConfiguration.Requests;
using ImportCostPro.Application.DTOs.TaxConfiguration.Responses;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Extensions;
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
            return config.ToResponse();
        }

        public async Task<TaxConfigurationResponse> SaveConfigurationAsync(SaveTaxConfigurationRequest request)
        {
            if (request.ItbisPercentage < 0 || request.ItbisPercentage > 100)
            {
                throw new ValidationBusinessException("El porcentaje de ITBIS debe estar entre 0 y 100.", nameof(request.ItbisPercentage));
            }

            if (request.CustomsServiceRatePercentage < 0 || request.CustomsServiceRatePercentage > 100)
            {
                throw new ValidationBusinessException("El porcentaje de servicio aduanal debe estar entre 0 y 100.", nameof(request.CustomsServiceRatePercentage));
            }

            var currentConfig = await _taxConfigurationRepository.GetSingleConfigurationAsync();

            if (currentConfig == null)
            {
                var newConfig = request.ToEntity();
                await _taxConfigurationRepository.AddAsync(newConfig);
                return newConfig.ToResponse();
            }

            request.UpdateEntity(currentConfig);
            await _taxConfigurationRepository.UpdateAsync(currentConfig);

            return currentConfig.ToResponse();
        }
    }
}