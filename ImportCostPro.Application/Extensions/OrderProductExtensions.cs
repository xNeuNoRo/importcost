using ImportCostPro.Application.DTOs.OrderProduct.Responses;
using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Application.Extensions
{
    public static class OrderProductExtensions
    {
        /// <summary>
        /// Extensión que transforma la entidad OrderProduct a su DTO de respuesta,
        /// calculando dinámicamente los totales de peso y volumen basados en las 
        /// especificaciones físicas del producto vinculado.
        /// </summary>
        public static OrderProductResponse ToResponse(this OrderProduct entity)
        {
            decimal totalWeight = 0;
            decimal totalVolume = 0;

            if (entity.Product != null)
            {
                // Cálculo de Peso Total: Cantidad * Peso Unitario
                totalWeight = entity.Quantity * entity.Product.UnitWeight;

                // Cálculo de Volumen Total: Cantidad * Largo * Ancho * Alto
                // Solo si las tres dimensiones están configuradas
                if (entity.Product.Length.HasValue && entity.Product.Width.HasValue && entity.Product.Height.HasValue)
                {
                    totalVolume = entity.Quantity * entity.Product.Length.Value * entity.Product.Width.Value * entity.Product.Height.Value;
                }
            }

            return new OrderProductResponse
            {
                Id = entity.Id,
                ImportOrderId = entity.ImportOrderId,
                ProductId = entity.ProductId,
                ProductReferenceCode = entity.Product?.ReferenceCode ?? "N/A",
                ProductName = entity.Product?.Name ?? "Producto no cargado",
                UnitOfMeasure = entity.Product?.UnitOfMeasure ?? default,
                Quantity = entity.Quantity,
                UnitFobPrice = entity.UnitFobPrice,
                SubtotalFob = entity.Quantity * entity.UnitFobPrice,
                TotalWeight = totalWeight,
                TotalVolume = totalVolume,
                TargetProfitMargin = entity.TargetProfitMargin,
            };
        }
    }
}
