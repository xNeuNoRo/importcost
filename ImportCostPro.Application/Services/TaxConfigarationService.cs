using ImportCostPro.Application.DTOs.TaxConfiguration.Requests;
using ImportCostPro.Application.DTOs.TaxConfiguration.Responses;
using ImportCostPro.Application.Exceptions;
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

            return config.ToResponse()!;
        }

        public async Task<TaxConfigurationResponse> SaveConfigurationAsync(
            SaveTaxConfigurationRequest request
        )
        {
            decimal normalizedItbis = request.ItbisPercentage;
            decimal normalizedCustoms = request.CustomsServiceRatePercentage;

            if (normalizedItbis < 0 || normalizedItbis > 100)
            {
                throw new ValidationBusinessException(
                    nameof(request.ItbisPercentage),
                    "El porcentaje de ITBIS debe estar entre 0 y 100."
                );
            }

            if (normalizedCustoms < 0 || normalizedCustoms > 100)
            {
                throw new ValidationBusinessException(
                    nameof(request.CustomsServiceRatePercentage),
                    "El porcentaje de servicio aduanal debe estar entre 0 y 100."
                );
            }

            var currentConfig = await _taxConfigurationRepository.GetSingleConfigurationAsync();

            if (currentConfig == null)
            {
                var newConfig = TaxConfiguration.Create(normalizedItbis, normalizedCustoms);

                await _taxConfigurationRepository.AddAsync(newConfig);
                return newConfig.ToResponse()!;
            }

            currentConfig.UpdateRates(normalizedItbis, normalizedCustoms);

            await _taxConfigurationRepository.UpdateAsync(currentConfig);

            return currentConfig.ToResponse()!;
        }
    }
}
