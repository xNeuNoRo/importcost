using ImportCostPro.Application.DTOs.Product.Responses;
using ImportCostPro.Persistence.Entities;
using Mapster;

namespace ImportCostPro.Application.Extensions
{
    public static class ProductExtensions
    {
        /// <summary>
        /// Extensión arquitectónica para aplanar la entidad Product a su DTO de salida
        /// centralizando el cálculo de volumen y la resolución semántica de maestros.
        /// </summary>
        public static ProductResponse ToResponse(
            this Product product,
            Country? country = null,
            TariffCategory? tariffCategory = null
        )
        {
            // Mapeamos la entidad a su DTO de salida
            var dto = product.Adapt<ProductResponse>();

            // Resolvemos los datos de las tablas cruzadas
            var finalCountry = country ?? product.DefaultOriginCountry;
            var finalTariff = tariffCategory ?? product.TariffCategory;

            // Mapeamos los datos cruzados de otras tablas y calculados
            dto.DefaultOriginCountryName = finalCountry?.Name ?? "N/A";
            dto.TariffCategoryCode = finalTariff?.Code ?? "N/A";
            dto.TariffCategoryDescription = finalTariff?.Description ?? "N/A";
            dto.CustomsDutyRate = finalTariff?.CustomsDutyRate ?? 0m;
            dto.VolumeCubicMeters =
                (product.Length ?? 0m) * (product.Width ?? 0m) * (product.Height ?? 0m);

            return dto;
        }
    }
}
