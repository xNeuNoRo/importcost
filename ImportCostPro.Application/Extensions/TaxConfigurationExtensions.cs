using ImportCostPro.Application.DTOs.TaxConfiguration.Responses;
using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Application.Extensions
{
    public static class TaxConfigurationExtensions
    {
        public static TaxConfigurationResponse ToResponse(this TaxConfiguration entity)
        {
            return new TaxConfigurationResponse
            {
                Id = entity.Id,
                ItbisPercentage = entity.GeneralItbisRate,
                CustomsServiceRatePercentage = entity.CustomsServiceRate,
            };
        }
    }
}
