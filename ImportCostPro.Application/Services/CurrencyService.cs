
using ImportCostPro.Application.DTOs.Currency.Requests;
using ImportCostPro.Application.DTOs.Currency.Responses;
using ImportCostPro.Application.Interfaces;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;

namespace ImportCostPro.Application.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository _currencyRepository;

        public CurrencyService(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }

        public async Task<IEnumerable<CurrencyResponse>> GetAllAsync()
        {
            var currencies = await _currencyRepository.GetAllAsync();
            return currencies.Select(c => new CurrencyResponse
            {
                Id = c.Id,
                Name = c.Name,
                IsoCode = c.IsoCode,
                Symbol = c.Symbol,
                IsLocalCurrency = c.IsLocalCurrency,
                IsActive = c.IsActive
            });
        }

        public async Task<CurrencyResponse?> GetByIdAsync(int id)
        {
            var currency = await _currencyRepository.GetByIdAsync(id);
            if (currency == null) return null;

            return new CurrencyResponse
            {
                Id = currency.Id,
                Name = currency.Name,
                IsoCode = currency.IsoCode,
                Symbol = currency.Symbol,
                IsLocalCurrency = currency.IsLocalCurrency,
                IsActive = currency.IsActive
            };
        }

        public async Task<int> CreateAsync(CreateCurrencyRequest request)
        {
            // El código ISO debe guardarse preferiblemente en mayúscula. [cite: 88]
            request.IsoCode = request.IsoCode.ToUpper();

            if (await _currencyRepository.ExistsByIsoCodeAsync(request.IsoCode))
            {
                throw new InvalidOperationException("Ya existe una moneda registrada con este código ISO.");
            }

            if (request.IsLocalCurrency && await _currencyRepository.AnyLocalCurrencyAsync())
            {
                throw new Exception("Ya existe una moneda configurada como moneda local. Solo puede existir una moneda local en el sistema.");
            }

            var currency = new Currency
            {
                Name = request.Name,
                IsoCode = request.IsoCode,
                Symbol = request.Symbol,
                IsLocalCurrency = request.IsLocalCurrency,
                IsActive = true
            };

            await _currencyRepository.AddAsync(currency);
            return currency.Id;
        }

        public async Task UpdateAsync(UpdateCurrencyRequest request)
        {
            var existingCurrency = await _currencyRepository.GetByIdAsync(request.Id)
                ?? throw new InvalidOperationException("Moneda no encontrada.");

            // El código ISO debe manejarse preferiblemente en mayúscula. [cite: 164]
            request.IsoCode = request.IsoCode.ToUpper();

            if (existingCurrency.IsoCode != request.IsoCode)
            {
                if (await _currencyRepository.ExistsByIsoCodeAsync(request.IsoCode, request.Id))
                {
                    throw new InvalidOperationException("Ya existe una moneda registrada con este código ISO.");
                }

                if (await _currencyRepository.IsCurrencyReferencedAsync(request.Id))
                {
                    throw new InvalidOperationException("No se puede modificar el código ISO de esta moneda porque ya está siendo utilizada en registros del sistema.");
                }
            }

            if (request.IsLocalCurrency && !existingCurrency.IsLocalCurrency)
            {
                // Excluir la propia moneda al comprobar si ya existe una moneda local
                if (await _currencyRepository.AnyLocalCurrencyAsync(request.Id))
                {
                    throw new InvalidOperationException("Ya existe una moneda configurada como moneda local. Solo puede existir una moneda local en el sistema.");
                }
            }

            existingCurrency.Name = request.Name;
            existingCurrency.IsoCode = request.IsoCode;
            existingCurrency.Symbol = request.Symbol;
            existingCurrency.IsLocalCurrency = request.IsLocalCurrency;

            await _currencyRepository.UpdateAsync(existingCurrency);
        }

        public async Task ToggleStatusAsync(int id)
        {
            var currency = await _currencyRepository.GetByIdAsync(id)
                ?? throw new Exception("Moneda no encontrada.");

            if (currency.IsActive && currency.IsLocalCurrency && await _currencyRepository.IsCurrencyReferencedAsync(id))
            {
                throw new InvalidOperationException("No se puede inactivar la moneda local mientras existan registros que dependan de ella.");
            }

            currency.IsActive = !currency.IsActive;
            await _currencyRepository.UpdateAsync(currency);
        }

        public async Task DeleteAsync(int id)
        {
            var currency = await _currencyRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Moneda no encontrada.");

            if (currency.IsLocalCurrency)
            {
                throw new InvalidOperationException("No se puede eliminar la moneda local del sistema.");
            }

            if (await _currencyRepository.IsCurrencyReferencedAsync(id))
            {
                throw new InvalidOperationException("No se puede eliminar esta moneda porque está asociada a otros registros del sistema.");
            }

            await _currencyRepository.DeleteAsync(id);
        }
    }
}