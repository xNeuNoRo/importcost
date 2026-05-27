using ImportCostPro.Application.DTOs.LandedCost.Responses;
using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Application.Extensions
{
    public static class LandedCostExtensions
    {
        public static LandedCostCalculationResponse ToResponse(this CalculationResult entity)
        {
            return new LandedCostCalculationResponse
            {
                Id = entity.Id,
                ImportOrderId = entity.ImportOrderId,
                OrderNumber = entity.ImportOrder?.OrderNumber ?? string.Empty,
                LocalCurrencyUsedId = entity.LocalCurrencyUsedId,
                LocalCurrencyIsoCode = entity.LocalCurrencyUsed?.IsoCode ?? string.Empty,
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
                Details = entity.Details.Select(d => d.ToDetailResponse()).ToList(),
            };
        }

        public static CalculationResultDetailResponse ToDetailResponse(
            this CalculationResultDetail entity
        )
        {
            return new CalculationResultDetailResponse
            {
                Id = entity.Id,
                ProductId = entity.ProductId,
                ProductReferenceCode = entity.Product?.ReferenceCode ?? string.Empty,
                ProductName = entity.Product?.Name ?? string.Empty,
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
