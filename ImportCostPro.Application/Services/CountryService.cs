using ImportCostPro.Application.DTOs.Country.Requests;
using ImportCostPro.Application.DTOs.Country.Responses;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Mapster;

namespace ImportCostPro.Application.Services
{
    public class CountryService
    {
        private readonly ICountryRepository _countryRepository;

        public CountryService(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<IEnumerable<CountryResponse>> GetAllAsync()
        {
            var countries = await _countryRepository.GetAllAsync();
            return countries.Adapt<IEnumerable<CountryResponse>>().ToList();
        }

        public async Task<CountryResponse?> GetByIdAsync(int id)
        {
            var country = await _countryRepository.GetByIdAsync(id);
            if (country == null)
                return null;

            return country.Adapt<CountryResponse>();
        }

        public async Task<CountryResponse> CreateAsync(CreateCountryRequest request)
        {
            string normalizedName = request.Name.Trim();
            string normalizedIsoCode = request.IsoCode.Trim().ToUpperInvariant();

            if (await _countryRepository.ExistsByIsoCodeAsync(normalizedIsoCode))
            {
                throw new ValidationBusinessException(
                    nameof(request.IsoCode),
                    $"Ya existe un país registrado con este código ISO."
                );
            }

            if (await _countryRepository.ExistsByNameAsync(normalizedName))
            {
                throw new ValidationBusinessException(
                    nameof(request.Name),
                    $"Ya existe un país registrado con este nombre."
                );
            }

            var country = request.Adapt<Country>();
            country.Name = normalizedName;
            country.IsoCode = normalizedIsoCode;
            country.IsActive = true;

            await _countryRepository.AddAsync(country);

            return country.Adapt<CountryResponse>();
        }

        public async Task<CountryResponse> UpdateAsync(UpdateCountryRequest request)
        {
            string normalizedName = request.Name.Trim();
            string normalizedIsoCode = request.IsoCode.Trim().ToUpperInvariant();

            var existingCountry = await _countryRepository.GetByIdAsync(request.Id);
            if (existingCountry == null)
            {
                throw new BusinessException($"No se encontró un país con el ID '{request.Id}'.");
            }

            if (await _countryRepository.ExistsByIsoCodeAsync(normalizedIsoCode, excludeId: request.Id))
            {
                throw new ValidationBusinessException(
                    nameof(request.IsoCode),
                    $"Ya existe un país registrado con este código ISO."
                );
            }

            if (await _countryRepository.ExistsByNameAsync(normalizedName, excludeId: request.Id))
            {
                throw new ValidationBusinessException(
                    nameof(request.Name),
                    $"Ya existe un país registrado con este nombre."
                );
            }

            existingCountry = request.Adapt(existingCountry);
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
                throw new BusinessException($"No se encontró un país con el ID '{id}'.");
            }

            if (await _countryRepository.IsCountryReferencedAsync(id))
            {
                throw new BusinessException(
                    $"No se puede eliminar el país '{existingCountry.Name}' porque está siendo referenciado por otras entidades."
                );
            }

            await _countryRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> ToggleStatusAsync(int id)
        {
            var existingCountry = await _countryRepository.GetByIdAsync(id);
            if (existingCountry == null)
            {
                throw new BusinessException($"No se encontró un país con el ID '{id}'.");
            }

            existingCountry.IsActive = !existingCountry.IsActive;

            await _countryRepository.UpdateAsync(existingCountry);
            return true;
        }
    }
}
