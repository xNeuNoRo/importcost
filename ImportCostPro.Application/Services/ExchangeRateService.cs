using ImportCostPro.Application.DTOs.ExchangeRate.Requests;
using ImportCostPro.Application.DTOs.ExchangeRate.Responses;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Extensions;
using ImportCostPro.Persistence.Interfaces.Repositories;

namespace ImportCostPro.Application.Services
{
    public class ExchangeRateService
    {
        private readonly IExchangeRateRepository _exchangeRateRepository;

        public ExchangeRateService(IExchangeRateRepository exchangeRateRepository)
        {
            _exchangeRateRepository = exchangeRateRepository;
        }

        public async Task<IEnumerable<ExchangeRateResponse>> GetAllAsync()
        {
            var rates = await _exchangeRateRepository.GetAllWithCurrenciesAsync();
            return rates.Select(r => r.ToResponse());
        }

        public async Task<ExchangeRateResponse> GetByIdAsync(int id)
        {
            var rate = await _exchangeRateRepository.GetByIdAsync(id)
                ?? throw new ValidationBusinessException($"La tasa de cambio con ID {id} no fue encontrada.", nameof(id));

            return rate.ToResponse();
        }

        public async Task<ExchangeRateResponse> CreateAsync(CreateExchangeRateRequest request)
        {
            ValidateBasicRules(request.FromCurrencyId, request.ToCurrencyId, request.RateValue);

            bool duplicate = await _exchangeRateRepository.ExistsActiveDuplicateAsync(
                request.FromCurrencyId, request.ToCurrencyId, request.EffectiveDate);

            if (duplicate)
                throw new ValidationBusinessException("Ya existe una tasa de cambio activa para la misma moneda origen, destino y fecha de vigencia.", nameof(request.FromCurrencyId));

            var entity = request.ToEntity();
            await _exchangeRateRepository.AddAsync(entity);

            return entity.ToResponse();
        }

        public async Task<ExchangeRateResponse> UpdateAsync(UpdateExchangeRateRequest request)
        {
            ValidateBasicRules(request.FromCurrencyId, request.ToCurrencyId, request.RateValue);

            var entity = await _exchangeRateRepository.GetByIdAsync(request.Id)
                ?? throw new ValidationBusinessException($"La tasa de cambio con ID {request.Id} no fue encontrada.", nameof(request.Id));

            bool duplicate = await _exchangeRateRepository.ExistsActiveDuplicateAsync(
                request.FromCurrencyId, request.ToCurrencyId, request.EffectiveDate, request.Id);

            if (duplicate)
                throw new ValidationBusinessException("Ya existe una tasa de cambio activa para la misma moneda origen, destino y fecha de vigencia.", nameof(request.FromCurrencyId));

            entity.FromCurrencyId = request.FromCurrencyId;
            entity.ToCurrencyId = request.ToCurrencyId;
            entity.RateValue = request.RateValue;
            entity.EffectiveDate = request.EffectiveDate;

            await _exchangeRateRepository.UpdateAsync(entity);

            return entity.ToResponse();
        }

        public async Task ToggleActiveAsync(int id)
        {
            var entity = await _exchangeRateRepository.GetByIdAsync(id)
                ?? throw new ValidationBusinessException($"La tasa de cambio con ID {id} no fue encontrada.", nameof(id));

            entity.IsActive = !entity.IsActive;
            await _exchangeRateRepository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _exchangeRateRepository.GetByIdAsync(id)
                ?? throw new ValidationBusinessException($"La tasa de cambio con ID {id} no fue encontrada.", nameof(id));

            await _exchangeRateRepository.DeleteAsync(id);
        }

        private static void ValidateBasicRules(int fromCurrencyId, int toCurrencyId, decimal rateValue)
        {
            if (fromCurrencyId == toCurrencyId)
                throw new ValidationBusinessException("La moneda origen y la moneda destino no pueden ser iguales.", nameof(fromCurrencyId));

            if (rateValue <= 0)
                throw new ValidationBusinessException("El valor de la tasa de cambio debe ser mayor a 0.", nameof(rateValue));
        }
    }
}