using ImportCostPro.Application.DTOs.Country.Responses;
using ImportCostPro.Application.DTOs.Country.Requests;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Mapster;

namespace ImportCostPro.Application.Services
{
    public class CountryServices
    {
        private readonly ICountryRepository _countryRepository;

        public CountryServices(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<IEnumerable<CountryResponse>> GetAllAsync()
        {
            var countries = await _countryRepository.GetAllAsync();
            // Mapster carrea y resuelve el mapeo pa no matarnos a mano con mapeos manuales
            return countries.Adapt<IEnumerable<CountryResponse>>().ToList();
        }

        public async Task<CountryResponse?> GetByIdAsync(int id)
        {
            var country = await _countryRepository.GetByIdAsync(id);
            if (country == null)
                return null;

            // Mapster carrea y resuelve el mapeo pa no matarnos a mano con mapeos manuales
            return country.Adapt<CountryResponse>();
        }

        public async Task<CountryResponse> CreateAsync(CreateCountryRequest request)
        {
            string normalizedName = request.Name?.Trim() ?? string.Empty;
            string normalizedIsoCode = request.IsoCode?.Trim().ToUpperInvariant() ?? string.Empty;

            if (await _countryRepository.ExistsByIsoCodeAsync(normalizedIsoCode))
            {
                throw new ValidationBusinessException(
                    nameof(request.IsoCode),
                    $"El código ISO '{normalizedIsoCode}' ya se encuentra registrado para otro país."
                );
            }

            // Usamos Mapster para crear la instancia base limpia de la entidad
            var country = request.Adapt<Country>();

            // Asignamos las propiedades normalizadas de forma correcta
            country.Name = normalizedName;
            country.IsoCode = normalizedIsoCode;
            country.IsActive = true; 

            await _countryRepository.AddAsync(country);
            
            return country.Adapt<CountryResponse>();
        }

        public async Task<CountryResponse> UpdatAsync(UpdateCountryRequest request)
        {
            string normalizedName = request.Name?.Trim() ?? string.Empty;
            string normalizedIsoCode = request.IsoCode?.Trim().ToUpperInvariant() ?? string.Empty;

            var existingCountry = await _countryRepository.GetByIdAsync(request.Id);
            if (existingCountry == null)
            {
                throw new ValidationBusinessException(
                    nameof(request.Id),
                    $"No se encontró un país con el ID '{request.Id}'."
                );
            }

            if (await _countryRepository.ExistsByIsoCodeAsync(normalizedIsoCode, excludeId: request.Id))
            {
                throw new ValidationBusinessException(
                    nameof(request.IsoCode),
                    $"El código ISO '{normalizedIsoCode}' ya se encuentra registrado para otro país."
                );
            }

            // Mapster mapea los cambios sobre la entidad existente
            existingCountry = request.Adapt(existingCountry);

            // Aseguramos valores limpios antes de persistir
            existingCountry.Name = normalizedName;
            existingCountry.IsoCode = normalizedIsoCode;

            await _countryRepository.UpdateAsync(existingCountry);
            return existingCountry.Adapt<CountryResponse>();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingCountry = await _countryRepository.GetByIdAsync(id);
            if (existingCountry == null)
            {
                throw new ValidationBusinessException(
                    nameof(id),
                    $"No se encontró un país con el ID '{id}'."
                );
            }

            if (await _countryRepository.IsCountryReferencedAsync(id))
            {
                throw new BusinessException(
                    $"No se puede eliminar el país '{existingCountry.Name}' porque está siendo referenciado por otras entidades en el sistema."
                );
            }

            // SOLUCIÓN AL ERROR: Le pasamos el 'id' numérico entero como pide tu repositorio genérico
            await _countryRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> ToggleStatusAsync(int id)
        {
            var existingCountry = await _countryRepository.GetByIdAsync(id);
            if (existingCountry == null)
            {
                throw new ValidationBusinessException(
                    nameof(id),
                    $"No se encontró un país con el ID '{id}'."
                );
            }

            existingCountry.IsActive = !existingCountry.IsActive;
            await _countryRepository.UpdateAsync(existingCountry);
            return true;
        }
    }
}