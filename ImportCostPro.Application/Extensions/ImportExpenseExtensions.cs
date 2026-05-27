using ImportCostPro.Application.DTOs.ImportExpense.Responses;
using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Application.Extensions
{
    public static class ImportExpenseExtensions
    {
        public static ImportExpenseResponse ToResponse(this ImportExpense entity)
        {
            return new ImportExpenseResponse
            {
                Id = entity.Id,
                ImportOrderId = entity.ImportOrderId,
                CurrencyId = entity.CurrencyId,
                CurrencyIsoCode = entity.Currency?.IsoCode ?? string.Empty,
                Description = entity.Description,
                ExpenseType = entity.ExpenseType,
                DistributionBase = entity.DistributionBase,
                OriginalAmount = entity.OriginalAmount,
            };
        }
    }
}
