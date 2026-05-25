using ImportCostPro.Application.DTOs.Currency.Requests;
using ImportCostPro.Application.DTOs.Currency.Responses;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Mapster;

namespace ImportCostPro.Application.Services
{
    public class CurrencyService
    {
        private readonly ICurrencyRepository _currencyRepository;

        public CurrencyService(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }

        public async Task<IEnumerable<CurrencyResponse>> GetAllAsync()
        {
            var currencies = await _currencyRepository.GetAllAsync();
            // Mapster carrea y resuelve el mapeo pa no matarnos a mano con mapeos manuales
            return currencies.Adapt<IEnumerable<CurrencyResponse>>().ToList();
        }

        public async Task<CurrencyResponse?> GetByIdAsync(int id)
        {
            var currency = await _currencyRepository.GetByIdAsync(id);
            if (currency == null)
                return null;

            // Mapster carrea y resuelve el mapeo pa no matarnos a mano con mapeos manuales
            return currency.Adapt<CurrencyResponse>();
        }

        public async Task<CurrencyResponse> CreateAsync(CreateCurrencyRequest request)
        {
            string normalizedIsoCode = request.IsoCode?.Trim().ToUpperInvariant() ?? string.Empty;
            string normalizedName = request.Name?.Trim() ?? string.Empty;
            string normalizedSymbol = request.Symbol?.Trim() ?? string.Empty;

            if (await _currencyRepository.ExistsByIsoCodeAsync(normalizedIsoCode))
            {
                throw new ValidationBusinessException(
                    nameof(request.IsoCode),
                    $"El código ISO '{normalizedIsoCode}' ya se encuentra registrado para otra moneda."
                );
            }

            if (request.IsLocalCurrency && await _currencyRepository.AnyLocalCurrencyAsync())
            {
                throw new ValidationBusinessException(
                    nameof(request.IsLocalCurrency),
                    "Ya existe una moneda configurada como moneda local principal en el sistema. Solo se permite una."
                );
            }

            // Alabadas sean las extensiones de mapeo automatico papadio
            var currency = request.Adapt<Currency>();

            currency.Name = normalizedName;
            currency.IsoCode = normalizedIsoCode;
            currency.Symbol = normalizedSymbol;
            currency.IsActive = true;

            await _currencyRepository.AddAsync(currency);

            // Mapster carrea y resuelve el mapeo pa no matarnos a mano con mapeos manuales
            return currency.Adapt<CurrencyResponse>();
        }

        public async Task<CurrencyResponse> UpdateAsync(UpdateCurrencyRequest request)
        {
            request.Name = request.Name?.Trim() ?? string.Empty;
            request.IsoCode = request.IsoCode?.Trim().ToUpperInvariant() ?? string.Empty;
            request.Symbol = request.Symbol?.Trim() ?? string.Empty;

            var existingCurrency = await _currencyRepository.GetByIdAsync(request.Id);
            if (existingCurrency == null)
            {
                throw new BusinessException(
                    "La moneda que intenta actualizar ya no existe en el sistema."
                );
            }

            if (existingCurrency.IsoCode != request.IsoCode)
            {
                if (await _currencyRepository.ExistsByIsoCodeAsync(request.IsoCode, request.Id))
                {
                    throw new ValidationBusinessException(
                        nameof(request.IsoCode),
                        $"El código ISO '{request.IsoCode}' ya está siendo utilizado por otra divisa."
                    );
                }

                if (await _currencyRepository.IsCurrencyReferencedAsync(request.Id))
                {
                    throw new ValidationBusinessException(
                        nameof(request.IsoCode),
                        "No se puede alterar el código ISO de esta divisa porque ya cuenta con transacciones u órdenes históricas vinculadas."
                    );
                }
            }

            if (request.IsLocalCurrency && !existingCurrency.IsLocalCurrency)
            {
                if (await _currencyRepository.AnyLocalCurrencyAsync(request.Id))
                {
                    throw new ValidationBusinessException(
                        nameof(request.IsLocalCurrency),
                        "Operación rechazada. Ya existe otra divisa establecida como moneda local base."
                    );
                }
            }

            // Mapster carreando
            request.Adapt(existingCurrency);
            await _currencyRepository.UpdateAsync(existingCurrency);

            return existingCurrency.Adapt<CurrencyResponse>();
        }

        public async Task<bool> ToggleStatusAsync(int id)
        {
            var currency = await _currencyRepository.GetByIdAsync(id);
            if (currency == null)
            {
                throw new BusinessException("La moneda especificada no existe en el catálogo.");
            }

            if (currency.IsLocalCurrency)
            {
                throw new BusinessException(
                    "La moneda local base del sistema no puede ser desactivada."
                );
            }

            currency.IsActive = !currency.IsActive;
            await _currencyRepository.UpdateAsync(currency);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var currency = await _currencyRepository.GetByIdAsync(id);
            if (currency == null)
            {
                throw new BusinessException(
                    "La moneda que intenta eliminar no existe en el sistema."
                );
            }

            if (currency.IsLocalCurrency)
            {
                throw new BusinessException(
                    "Está prohibido eliminar físicamente la moneda local del sistema."
                );
            }

            if (await _currencyRepository.IsCurrencyReferencedAsync(id))
            {
                throw new BusinessException(
                    $"No es posible eliminar la divisa '{currency.Name}' debido a que cuenta con tasas de cambio u órdenes de importación asociadas."
                );
            }

            await _currencyRepository.DeleteAsync(id);
            return true;
        }
    }
}
