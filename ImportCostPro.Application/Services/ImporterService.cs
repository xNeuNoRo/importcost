using ImportCostPro.Application.DTOs.Importer.Requests;
using ImportCostPro.Application.DTOs.Importer.Responses;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Mapster;

namespace ImportCostPro.Application.Services
{
    public class ImporterService
    {
        private readonly IImporterRepository _importerRepository;
        private readonly ICountryRepository _countryRepository;

        public ImporterService(
            IImporterRepository importerRepository,
            ICountryRepository countryRepository
        )
        {
            _importerRepository = importerRepository;
            _countryRepository = countryRepository;
        }

        public async Task<IEnumerable<ImporterResponse>> GetAllAsync()
        {
            var importers = await _importerRepository.GetAllWithCountryAsync();
            return importers.Adapt<IEnumerable<ImporterResponse>>().ToList();
        }

        public async Task<ImporterResponse?> GetByIdAsync(int id)
        {
            var importer = await _importerRepository.GetByIdWithCountryAsync(id);
            if (importer == null)
                return null;

            return importer.Adapt<ImporterResponse>();
        }

        public async Task<ImporterResponse> CreateAsync(CreateImporterRequest request)
        {
            string normalizedLegalName = request.LegalName.Trim();
            string normalizedTaxID = request.TaxId.Trim().ToUpperInvariant();

            var countryExists = await _countryRepository.GetByIdAsync(request.CountryId);
            if (countryExists == null || !countryExists.IsActive)
            {
                throw new ValidationBusinessException(
                    nameof(request.CountryId),
                    "El país seleccionado no es válido o se encuentra inactivo."
                );
            }

            if (await _importerRepository.ExistsTaxIdAsync(normalizedTaxID))
            {
                throw new ValidationBusinessException(
                    nameof(request.TaxId),
                    "Ya existe un importador registrado con este RNC o identificación fiscal."
                );
            }

            var importer = request.Adapt<Importer>();
            importer.LegalName = normalizedLegalName;
            importer.TaxId = normalizedTaxID;
            importer.IsActive = true;

            await _importerRepository.AddAsync(importer);

            var response = importer.Adapt<ImporterResponse>();
            response.CountryName = countryExists.Name;
            response.CountryIsoCode = countryExists.IsoCode;

            return response;
        }

        public async Task<ImporterResponse> UpdateAsync(UpdateImporterRequest request)
        {
            string normalizedLegalName = request.LegalName.Trim();
            string normalizedTaxID = request.TaxId.Trim().ToUpperInvariant();

            var existingImporter = await _importerRepository.GetByIdAsync(request.Id);
            if (existingImporter == null)
            {
                throw new BusinessException(
                    $"No se encontró un importador con el ID '{request.Id}'."
                );
            }

            if (existingImporter.TaxId != normalizedTaxID)
            {
                bool hasOrders = await _importerRepository.IsImporterReferencedAsync(request.Id);
                if (hasOrders)
                {
                    throw new BusinessException(
                        "No se puede modificar el RNC o identificación fiscal de este importador porque ya tiene órdenes de importación registradas."
                    );
                }
            }

            var countryExists = await _countryRepository.GetByIdAsync(request.CountryId);
            if (countryExists == null)
            {
                throw new ValidationBusinessException(
                    nameof(request.CountryId),
                    "El país seleccionado no existe o no es válido."
                );
            }

            if (await _importerRepository.ExistsTaxIdAsync(normalizedTaxID, excludeId: request.Id))
            {
                throw new ValidationBusinessException(
                    nameof(request.TaxId),
                    "El RNC ingresado ya está siendo utilizado por otro importador."
                );
            }

            existingImporter = request.Adapt(existingImporter);
            existingImporter.LegalName = normalizedLegalName;
            existingImporter.TaxId = normalizedTaxID;

            await _importerRepository.UpdateAsync(existingImporter);

            var response = existingImporter.Adapt<ImporterResponse>();
            response.CountryName = countryExists.Name;
            response.CountryIsoCode = countryExists.IsoCode;

            return response;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingImporter = await _importerRepository.GetByIdAsync(id);
            if (existingImporter == null)
            {
                throw new BusinessException($"No se encontró un importador con el ID '{id}'.");
            }

            if (await _importerRepository.IsImporterReferencedAsync(id))
            {
                throw new BusinessException(
                    $"No se puede eliminar: el importador '{existingImporter.LegalName}' posee órdenes de importación históricas."
                );
            }

            await _importerRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> ToggleStatusAsync(int id)
        {
            var existingImporter = await _importerRepository.GetByIdAsync(id);
            if (existingImporter == null)
            {
                throw new BusinessException($"No se encontró un importador con el ID '{id}'.");
            }

            existingImporter.IsActive = !existingImporter.IsActive;

            await _importerRepository.UpdateAsync(existingImporter);
            return true;
        }
    }
}
