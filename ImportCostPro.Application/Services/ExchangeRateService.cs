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

        public ExchangeRateService(
            IExchangeRateRepository exchangeRateRepository,
            ICurrencyRepository currencyRepository
        )
        {
            _exchangeRateRepository = exchangeRateRepository;
            _currencyRepository = currencyRepository;
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

            ValidateBasicRules(
                normalizedFromCurrencyId,
                normalizedToCurrencyId,
                normalizedRateValue,
                nameof(request.FromCurrencyId),
                nameof(request.RateValue)
            );

            if (!await _currencyRepository.ExistsByIdAsync(normalizedFromCurrencyId))
            {
                throw new ValidationBusinessException(
                    nameof(request.FromCurrencyId),
                    $"La moneda de origen con ID {normalizedFromCurrencyId} no existe."
                );
            }

            if (!await _currencyRepository.ExistsByIdAsync(normalizedToCurrencyId))
            {
                throw new ValidationBusinessException(
                    nameof(request.ToCurrencyId),
                    $"La moneda de destino con ID {normalizedToCurrencyId} no existe."
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
                    "Ya existe una tasa de cambio activa para la misma moneda origen, destino y fecha de vigencia."
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

            ValidateBasicRules(
                normalizedFromCurrencyId,
                normalizedToCurrencyId,
                normalizedRateValue,
                nameof(request.FromCurrencyId),
                nameof(request.RateValue)
            );

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
                    "No se puede modificar una tasa de cambio que ya ha sido utilizada en un histórico de importación."
                );
            }

            if (!await _currencyRepository.ExistsByIdAsync(normalizedFromCurrencyId))
            {
                throw new ValidationBusinessException(
                    nameof(request.FromCurrencyId),
                    $"La moneda de origen con ID {normalizedFromCurrencyId} no existe."
                );
            }

            if (!await _currencyRepository.ExistsByIdAsync(normalizedToCurrencyId))
            {
                throw new ValidationBusinessException(
                    nameof(request.ToCurrencyId),
                    $"La moneda de destino con ID {normalizedToCurrencyId} no existe."
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
                    "Ya existe una tasa de cambio activa para la misma moneda origen, destino y fecha de vigencia."
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
                    "Está prohibido eliminar físicamente una tasa que cuenta con histórico de prorrateo."
                );
            }

            await _exchangeRateRepository.DeleteAsync(id);

            return true;
        }

        private static void ValidateBasicRules(
            int fromCurrencyId,
            int toCurrencyId,
            decimal rateValue,
            string fromCurrencyPropertyName,
            string rateValuePropertyName
        )
        {
            if (fromCurrencyId == toCurrencyId)
            {
                throw new ValidationBusinessException(
                    fromCurrencyPropertyName,
                    "La moneda origen y la moneda destino no pueden ser iguales."
                );
            }

            if (rateValue <= 0)
            {
                throw new ValidationBusinessException(
                    rateValuePropertyName,
                    "El valor de la tasa de cambio debe ser mayor a 0."
                );
            }
        }
    }
}
