// Extensions/ExchangeRateExtensions.cs
using ImportCostPro.Application.DTOs.ExchangeRate.Requests;
using ImportCostPro.Application.DTOs.ExchangeRate.Responses;
using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Application.Extensions
{
    public static class ExchangeRateExtensions
    {
        public static ExchangeRateResponse ToResponse(this ExchangeRate entity)
        {
            return new ExchangeRateResponse
            {
                Id = entity.Id,
                FromCurrencyId = entity.FromCurrencyId,
                FromCurrencyIsoCode = entity.FromCurrency?.IsoCode ?? string.Empty,
                ToCurrencyId = entity.ToCurrencyId,
                ToCurrencyIsoCode = entity.ToCurrency?.IsoCode ?? string.Empty,
                RateValue = entity.RateValue,
                EffectiveDate = entity.EffectiveDate,
                IsActive = entity.IsActive
            };
        }

        public static ExchangeRate ToEntity(this CreateExchangeRateRequest request)
        {
            return new ExchangeRate
            {
                FromCurrencyId = request.FromCurrencyId,
                ToCurrencyId = request.ToCurrencyId,
                RateValue = request.RateValue,
                EffectiveDate = request.EffectiveDate,
                IsActive = true
            };
        }
    }
}