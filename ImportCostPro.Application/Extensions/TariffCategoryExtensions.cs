using ImportCostPro.Application.DTOs.TariffCategory.Responses;
using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Application.Extensions
{
    public static class TariffCategoryExtensions
    {
        public static TariffCategoryResponse ToResponse(this TariffCategory entity)
        {
            return new TariffCategoryResponse
            {
                Id = entity.Id,
                Code = entity.Code,
                Description = entity.Description,
                CustomsDutyRate = entity.CustomsDutyRate,
                IsActive = entity.IsActive,
            };
        }
    }
}
