using ImportCostPro.Application.DTOs.Supplier.Responses;
using ImportCostPro.Persistence.Entities;
using Mapster;

namespace ImportCostPro.Application.Extensions
{
    public static class SupplierExtensions
    {
        public static SupplierResponse ToResponse(
            this Supplier supplier,
            Country? country = null,
            Currency? currency = null
        )
        {
            var dto = supplier.Adapt<SupplierResponse>();


            var finalCountry = country ?? supplier.OriginCountry;
            var finalCurrency = currency ?? supplier.DefaultCurrency;

            dto.CountryName = finalCountry?.Name ?? string.Empty;
            dto.CurrencyName = finalCurrency?.Name ?? string.Empty;
            dto.CurrencyIsoCode = finalCurrency?.IsoCode ?? string.Empty;
            dto.CurrencySymbol = finalCurrency?.Symbol ?? string.Empty;

            return dto;
        }
    }
}