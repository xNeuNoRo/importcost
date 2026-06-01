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
            string normalizedIsoCode = request.IsoCode.Trim().ToUpperInvariant();
            string normalizedName = request.Name.Trim();
            string normalizedSymbol = request.Symbol.Trim();

            if (await _currencyRepository.ExistsByIsoCodeAsync(normalizedIsoCode))
            {
                throw new ValidationBusinessException(
                    nameof(request.IsoCode),
                    $"El código ISO '{normalizedIsoCode}' ya se encuentra registrado para otra moneda."
                );
            }

            if (await _currencyRepository.ExistsByNameAsync(normalizedName))
            {
                throw new ValidationBusinessException(
                    nameof(request.Name),
                    $"El nombre de moneda '{normalizedName}' ya se encuentra registrado."
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
            string normalizedIsoCode = request.IsoCode.Trim().ToUpperInvariant();
            string normalizedName = request.Name.Trim();
            string normalizedSymbol = request.Symbol.Trim();

            var existingCurrency = await _currencyRepository.GetByIdAsync(request.Id);
            if (existingCurrency == null)
            {
                throw new BusinessException(
                    "La moneda que intenta actualizar ya no existe en el sistema."
                );
            }

            if (await _currencyRepository.ExistsByNameAsync(normalizedName, request.Id))
            {
                throw new ValidationBusinessException(
                    nameof(request.Name),
                    $"El nombre de moneda '{normalizedName}' ya está siendo utilizado por otra divisa."
                );
            }

            if (existingCurrency.IsoCode != normalizedIsoCode)
            {
                if (await _currencyRepository.ExistsByIsoCodeAsync(normalizedIsoCode, request.Id))
                {
                    throw new ValidationBusinessException(
                        nameof(request.IsoCode),
                        $"El código ISO '{normalizedIsoCode}' ya está siendo utilizado por otra divisa."
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
                        "Ya existe una moneda configurada como moneda local. Solo puede existir una moneda local en el sistema."
                    );
                }
            }

            // No permitimos inactivar la moneda local si tiene dependencias
            if (existingCurrency.IsLocalCurrency && !request.IsActive)
            {
                if (await _currencyRepository.IsCurrencyReferencedAsync(request.Id))
                {
                    throw new ValidationBusinessException(
                        nameof(request.IsActive),
                        "No se puede inactivar la moneda local mientras existan registros que dependan de ella."
                    );
                }
            }

            // Mapster carreando
            request.Adapt(existingCurrency);

            existingCurrency.Name = normalizedName;
            existingCurrency.IsoCode = normalizedIsoCode;
            existingCurrency.Symbol = normalizedSymbol;

            await _currencyRepository.UpdateAsync(existingCurrency);

            return existingCurrency.Adapt<CurrencyResponse>();
        }

        public async Task<CurrencyResponse?> GetLocalCurrencyAsync()
        {
            var currency = await _currencyRepository.GetLocalCurrencyAsync();
            return currency?.Adapt<CurrencyResponse>();
        }

        public async Task<bool> IsCurrencyReferencedAsync(int id)
        {
            return await _currencyRepository.IsCurrencyReferencedAsync(id);
        }

        public async Task<bool> ToggleStatusAsync(int id)
        {
            var currency = await _currencyRepository.GetByIdAsync(id);
            if (currency == null)
            {
                throw new BusinessException("La moneda específica no existe en el catálogo.");
            }

            // Si se intenta INACTIVAR y es moneda local, validar dependencias
            if (currency.IsLocalCurrency && currency.IsActive)
            {
                if (await _currencyRepository.IsCurrencyReferencedAsync(id))
                {
                    throw new BusinessException(
                        "No se puede inactivar la moneda local mientras existan registros que dependan de ella."
                    );
                }
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
                    "No se puede eliminar la moneda local del sistema."
                );
            }

            if (await _currencyRepository.IsCurrencyReferencedAsync(id))
            {
                throw new BusinessException(
                    "No se puede eliminar esta moneda porque está asociada a otros registros del sistema."
                );
            }

            await _currencyRepository.DeleteAsync(id);
            return true;
        }
    }
}
