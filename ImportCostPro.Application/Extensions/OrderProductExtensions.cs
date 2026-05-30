using ImportCostPro.Application.DTOs.OrderProduct.Responses;
using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Application.Extensions
{
    public static class OrderProductExtensions
    {
        public static OrderProductResponse ToResponse(this OrderProduct entity)
        {
            return new OrderProductResponse
            {
                Id = entity.Id,
                ImportOrderId = entity.ImportOrderId,
                ProductId = entity.ProductId,
                ProductReferenceCode = entity.Product?.ReferenceCode ?? string.Empty,
                ProductName = entity.Product?.Name ?? string.Empty,
                UnitOfMeasure = entity.Product?.UnitOfMeasure ?? default,
                Quantity = entity.Quantity,
                UnitFobPrice = entity.UnitFobPrice,
                SubtotalFob = entity.Quantity * entity.UnitFobPrice,
                TargetProfitMargin = entity.TargetProfitMargin,
            };
        }
    }
}
