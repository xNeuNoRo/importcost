using ImportCostPro.Application.DTOs.TaxConfiguration.Requests;
using ImportCostPro.Application.DTOs.TaxConfiguration.Responses;
using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Application.Extensions
{
    public static class TaxConfigurationExtensions
    {
        public static TaxConfigurationResponse ToResponse(this TaxConfiguration entity)
        {
            if (entity == null) return null;

            return new TaxConfigurationResponse
            {
                Id = entity.Id,
                ItbisPercentage = entity.GeneralItbisRate,
                CustomsServiceRatePercentage = entity.CustomsServiceRate
            };
        }

        public static TaxConfiguration ToEntity(this SaveTaxConfigurationRequest request)
        {
            if (request == null) return null;

            return new TaxConfiguration
            {
                GeneralItbisRate = request.ItbisPercentage,
                CustomsServiceRate = request.CustomsServiceRatePercentage
            };
        }

        public static void UpdateEntity(this SaveTaxConfigurationRequest request, TaxConfiguration entity)
        {
            if (request == null || entity == null) return;

            entity.GeneralItbisRate = request.ItbisPercentage;
            entity.CustomsServiceRate = request.CustomsServiceRatePercentage;
        }
    }
}