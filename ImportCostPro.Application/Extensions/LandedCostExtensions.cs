using ImportCostPro.Application.DTOs.LandedCost.Responses;
using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Application.Extensions
{
    public static class LandedCostExtensions
    {
        public static LandedCostCalculationResponse ToResponse(
            this CalculationResult entity,
            string orderNumber,
            string localCurrencyIsoCode,
            Dictionary<int, (string Code, string Name)> productsLookup
        )
        {
            return new LandedCostCalculationResponse
            {
                Id = entity.Id,
                ImportOrderId = entity.ImportOrderId,
                OrderNumber = orderNumber,
                LocalCurrencyUsedId = entity.LocalCurrencyUsedId,
                LocalCurrencyIsoCode = localCurrencyIsoCode,
                ExchangeRateUsed = entity.ExchangeRateUsed,
                TotalOriginalFob = entity.TotalOriginalFob,
                TotalLocalFob = entity.TotalLocalFob,
                TotalFreight = entity.TotalFreight,
                TotalInsurance = entity.TotalInsurance,
                TotalCif = entity.TotalCif,
                TotalTariff = entity.TotalTariff,
                TotalExciseTax = entity.TotalExciseTax,
                TotalCustomsService = entity.TotalCustomsService,
                TotalItbis = entity.TotalItbis,
                TotalLocalExpenses = entity.TotalLocalExpenses,
                TotalImportCost = entity.TotalImportCost,
                TotalImportedQuantity = entity.TotalImportedQuantity,
                HistoricalItbisRate = entity.HistoricalItbisRate,
                HistoricalCustomsServiceRate = entity.HistoricalCustomsServiceRate,
                CalculationDate = entity.CalculationDate,
                Details = entity.Details.Select(d => d.ToDetailResponse(productsLookup)).ToList(),
            };
        }

        public static CalculationResultDetailResponse ToDetailResponse(
            this CalculationResultDetail entity,
            Dictionary<int, (string Code, string Name)> productsLookup
        )
        {
            string referenceCode = string.Empty;
            string productName = string.Empty;

            if (productsLookup.TryGetValue(entity.ProductId, out var productData))
            {
                referenceCode = productData.Code;
                productName = productData.Name;
            }

            return new CalculationResultDetailResponse
            {
                Id = entity.Id,
                ProductId = entity.ProductId,
                ProductReferenceCode = referenceCode,
                ProductName = productName,
                Quantity = entity.Quantity,
                OriginalUnitPriceFob = entity.OriginalUnitPriceFob,
                LocalTotalFob = entity.LocalTotalFob,
                AllocatedFreight = entity.AllocatedFreight,
                AllocatedInsurance = entity.AllocatedInsurance,
                LocalTotalCif = entity.LocalTotalCif,
                CustomsDutyAmount = entity.CustomsDutyAmount,
                ExciseTaxAmount = entity.ExciseTaxAmount,
                CustomsServiceAmount = entity.CustomsServiceAmount,
                ItbisAmount = entity.ItbisAmount,
                AllocatedLocalExpenses = entity.AllocatedLocalExpenses,
                LocalTotalLandedCost = entity.LocalTotalLandedCost,
                UnitLandedCost = entity.UnitLandedCost,
                ProfitMarginRate = entity.ProfitMarginRate,
                SuggestedRetailPrice = entity.SuggestedRetailPrice,
            };
        }
    }
}
