using ImportCostPro.Application.DTOs.ImportOrder.Responses;
using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Application.Extensions
{
    public static class ImportOrderExtensions
    {
        public static ImportOrderResponse ToResponse(this ImportOrder entity)
        {
            return new ImportOrderResponse
            {
                Id = entity.Id,
                OrderNumber = entity.OrderNumber,
                OrderDate = entity.OrderDate,
                TransportMode = entity.TransportMode,
                Status = entity.Status,
                ImporterId = entity.ImporterId,
                ImporterName = entity.Importer?.LegalName ?? string.Empty,
                SupplierId = entity.SupplierId,
                SupplierName = entity.Supplier?.Name ?? string.Empty,
                OriginCountryId = entity.OriginCountryId,
                OriginCountryName = entity.OriginCountry?.Name ?? string.Empty,
                CurrencyId = entity.CurrencyId,
                CurrencyIsoCode = entity.Currency?.IsoCode ?? string.Empty,
            };
        }
    }
}
