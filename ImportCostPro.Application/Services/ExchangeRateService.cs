using ImportCostPro.Application.DTOs.ExchangeRate.Requests;
using ImportCostPro.Application.DTOs.ExchangeRate.Responses;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Extensions;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;

namespace ImportCostPro.Application.Services
{
    public class ExchangeRateService
    {
        private readonly IExchangeRateRepository _exchangeRateRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly Persistence.Interfaces.Providers.IDateTimeProvider _dateTimeProvider;

        public ExchangeRateService(
            IExchangeRateRepository exchangeRateRepository,
            ICurrencyRepository currencyRepository,
            Persistence.Interfaces.Providers.IDateTimeProvider dateTimeProvider
        )
        {
            _exchangeRateRepository = exchangeRateRepository;
            _currencyRepository = currencyRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IEnumerable<ExchangeRateResponse>> GetAllAsync()
        {
            var rates = await _exchangeRateRepository.GetAllWithCurrenciesAsync();
            return rates.Select(r => r.ToResponse());
        }

        public async Task<ExchangeRateResponse?> GetByIdAsync(int id)
        {
            var rate = await _exchangeRateRepository.GetByIdWithCurrenciesAsync(id);
            if (rate == null)
            {
                return null;
            }
            return rate.ToResponse();
        }

        public async Task<ExchangeRateResponse> CreateAsync(CreateExchangeRateRequest request)
        {
            int normalizedFromCurrencyId = request.FromCurrencyId;
            int normalizedToCurrencyId = request.ToCurrencyId;
            decimal normalizedRateValue = request.RateValue;
            DateTime normalizedEffectiveDate = request.EffectiveDate.Date;

            var fromCurrency = await _currencyRepository.GetByIdAsync(normalizedFromCurrencyId);
            if (fromCurrency == null || !fromCurrency.IsActive)
            {
                throw new ValidationBusinessException(
                    nameof(request.FromCurrencyId),
                    "La moneda origen debe existir y estar activa en el mantenimiento de monedas."
                );
            }

            var toCurrency = await _currencyRepository.GetByIdAsync(normalizedToCurrencyId);
            if (toCurrency == null || !toCurrency.IsActive)
            {
                throw new ValidationBusinessException(
                    nameof(request.ToCurrencyId),
                    "La moneda destino debe existir y estar activa en el mantenimiento de monedas."
                );
            }

            bool duplicate = await _exchangeRateRepository.ExistsActiveDuplicateAsync(
                normalizedFromCurrencyId,
                normalizedToCurrencyId,
                normalizedEffectiveDate
            );

            if (duplicate)
            {
                throw new ValidationBusinessException(
                    nameof(request.FromCurrencyId),
                    "Ya existe una tasa de cambio activa para esta moneda origen, moneda destino y fecha de vigencia."
                );
            }

            var entity = ExchangeRate.Create(
                normalizedFromCurrencyId,
                normalizedToCurrencyId,
                normalizedRateValue,
                normalizedEffectiveDate
            );

            await _exchangeRateRepository.AddAsync(entity);

            var responseRate = await _exchangeRateRepository.GetByIdWithCurrenciesAsync(entity.Id);
            return responseRate!.ToResponse();
        }

        public async Task<ExchangeRateResponse> UpdateAsync(UpdateExchangeRateRequest request)
        {
            int normalizedFromCurrencyId = request.FromCurrencyId;
            int normalizedToCurrencyId = request.ToCurrencyId;
            decimal normalizedRateValue = request.RateValue;
            DateTime normalizedEffectiveDate = request.EffectiveDate.Date;

            var entity = await _exchangeRateRepository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                throw new BusinessException(
                    $"La tasa de cambio con ID {request.Id} no fue encontrada en el sistema."
                );
            }

            if (await _exchangeRateRepository.IsExchangeRateReferencedAsync(request.Id))
            {
                throw new BusinessException(
                    "No se puede modificar esta tasa de cambio porque ya fue utilizada en un cálculo oficial de landed cost."
                );
            }

            var fromCurrency = await _currencyRepository.GetByIdAsync(normalizedFromCurrencyId);
            if (fromCurrency == null)
            {
                throw new ValidationBusinessException(
                    nameof(request.FromCurrencyId),
                    "La moneda origen debe existir en el mantenimiento de monedas."
                );
            }

            var toCurrency = await _currencyRepository.GetByIdAsync(normalizedToCurrencyId);
            if (toCurrency == null)
            {
                throw new ValidationBusinessException(
                    nameof(request.ToCurrencyId),
                    "La moneda destino debe existir en el mantenimiento de monedas."
                );
            }

            bool duplicate = await _exchangeRateRepository.ExistsActiveDuplicateAsync(
                normalizedFromCurrencyId,
                normalizedToCurrencyId,
                normalizedEffectiveDate,
                request.Id
            );

            if (duplicate)
            {
                throw new ValidationBusinessException(
                    nameof(request.FromCurrencyId),
                    "Ya existe una tasa de cambio activa para esta moneda origen, moneda destino y fecha de vigencia."
                );
            }

            entity.UpdateDetails(
                normalizedFromCurrencyId,
                normalizedToCurrencyId,
                normalizedRateValue,
                normalizedEffectiveDate
            );

            await _exchangeRateRepository.UpdateAsync(entity);

            var responseRate = await _exchangeRateRepository.GetByIdWithCurrenciesAsync(entity.Id);
            return responseRate!.ToResponse();
        }

        public async Task<bool> ToggleStatusAsync(int id)
        {
            var entity = await _exchangeRateRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new BusinessException($"La tasa de cambio con ID {id} no fue encontrada.");
            }

            entity.IsActive = !entity.IsActive;
            await _exchangeRateRepository.UpdateAsync(entity);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _exchangeRateRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new BusinessException($"La tasa de cambio con ID {id} no fue encontrada.");
            }

            if (await _exchangeRateRepository.IsExchangeRateReferencedAsync(id))
            {
                throw new BusinessException(
                    "No se puede eliminar esta tasa de cambio porque ya fue utilizada en un cálculo oficial de landed cost."
                );
            }

            await _exchangeRateRepository.DeleteAsync(id);

            return true;
        }
    }
}
