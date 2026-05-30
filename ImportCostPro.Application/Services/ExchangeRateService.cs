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

            if (normalizedFromCurrencyId == normalizedToCurrencyId)
            {
                throw new ValidationBusinessException(
                    nameof(request.FromCurrencyId),
                    "La moneda origen no puede ser igual a la moneda destino."
                );
            }

            var fromCurrency = await _currencyRepository.GetByIdAsync(normalizedFromCurrencyId);
            if (fromCurrency == null)
            {
                throw new ValidationBusinessException(
                    nameof(request.FromCurrencyId),
                    "La moneda origen seleccionada no existe."
                );
            }

            if (!fromCurrency.IsActive)
            {
                throw new ValidationBusinessException(
                    nameof(request.FromCurrencyId),
                    "La moneda origen seleccionada se encuentra inactiva."
                );
            }

            var toCurrency = await _currencyRepository.GetByIdAsync(normalizedToCurrencyId);
            if (toCurrency == null)
            {
                throw new ValidationBusinessException(
                    nameof(request.ToCurrencyId),
                    "La moneda destino seleccionada no existe."
                );
            }

            if (!toCurrency.IsActive)
            {
                throw new ValidationBusinessException(
                    nameof(request.ToCurrencyId),
                    "La moneda destino seleccionada se encuentra inactiva."
                );
            }

            bool duplicateExists = await _exchangeRateRepository.ExistsActiveDuplicateAsync(
                normalizedFromCurrencyId,
                normalizedToCurrencyId,
                normalizedEffectiveDate
            );

            if (duplicateExists)
            {
                throw new ValidationBusinessException(
                    nameof(request.EffectiveDate),
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

            if (normalizedFromCurrencyId == normalizedToCurrencyId)
            {
                throw new ValidationBusinessException(
                    nameof(request.FromCurrencyId),
                    "La moneda origen no puede ser igual a la moneda destino."
                );
            }

            var entity = await _exchangeRateRepository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                throw new BusinessException($"La tasa de cambio con ID {request.Id} no fue encontrada.");
            }

            if (await _exchangeRateRepository.IsExchangeRateReferencedAsync(request.Id))
            {
                throw new BusinessException(
                    "No se puede modificar esta tasa de cambio porque ya fue utilizada en una Liquidación de Costos oficial."
                );
            }

            var fromCurrency = await _currencyRepository.GetByIdAsync(normalizedFromCurrencyId);
            if (fromCurrency == null)
            {
                throw new ValidationBusinessException(
                    nameof(request.FromCurrencyId),
                    "La moneda origen seleccionada no existe."
                );
            }

            var toCurrency = await _currencyRepository.GetByIdAsync(normalizedToCurrencyId);
            if (toCurrency == null)
            {
                throw new ValidationBusinessException(
                    nameof(request.ToCurrencyId),
                    "La moneda destino seleccionada no existe."
                );
            }

            bool duplicateExists = await _exchangeRateRepository.ExistsActiveDuplicateAsync(
                normalizedFromCurrencyId,
                normalizedToCurrencyId,
                normalizedEffectiveDate,
                excludeId: request.Id
            );

            if (duplicateExists)
            {
                throw new ValidationBusinessException(
                    nameof(request.EffectiveDate),
                    "Ya existe una tasa de cambio activa para esta moneda origen, moneda destino y fecha de vigencia."
                );
            }

            entity.UpdateDetails(
                normalizedFromCurrencyId,
                normalizedToCurrencyId,
                normalizedRateValue,
                normalizedEffectiveDate
            );

            entity.IsActive = request.IsActive;

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
                    "No se puede eliminar esta tasa de cambio porque ya fue utilizada en una Liquidación de Costos oficial."
                );
            }

            await _exchangeRateRepository.DeleteAsync(id);

            return true;
        }
    }
}
