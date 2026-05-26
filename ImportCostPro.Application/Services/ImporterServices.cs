
using ImportCostPro.Application.DTOs.Importer.Requests;
using ImportCostPro.Application.DTOs.Importer.Responses;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Mapster;



namespace ImportCostPro.Application.Services
{
    public class ImporterServices
    {
        private readonly IImporterRepository _importerRepository;

        private readonly ICountryRepository _countryRepository;

        public ImporterServices (
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

        public async Task <ImporterResponse?> GetByIdAsync(int id)
        {
            var importer = await _importerRepository.GetByIdWithCountryAsync(id);

            if (importer == null)
            return null;

            var response = importer.Adapt<ImporterResponse>();

            if(importer.Country != null)
            {
                response.CountryName = importer.Country.Name;
                response.CountryIsoCode = importer.Country.IsoCode;
            }
            return response;
        }
        public async Task<ImporterResponse> CreateAsync(CreateImporterRequest request)
        {
            string normalizedLegalName = request.LegalName?.Trim() ?? string.Empty;
            string normalizedTaxID = request.TaxId?.Trim().ToUpperInvariant() ?? string.Empty;

            var countryExits = await _countryRepository.GetByIdAsync(request.CountryId);
            if(countryExits == null)
            {
                throw new ValidationBusinessException(
                    nameof(request.CountryId),
                    "El pais seleccionado no es valido o no existe."
                );
            }
            if (await _importerRepository.ExistsTaxIdAsync(normalizedTaxID))
            {
                throw new ValidationBusinessException(
                    nameof(request.TaxId),
                    "El RNC ingresado ya pertenece a otro importador"
                );
            }

            var importer = request.Adapt<Importer>();
            importer.LegalName = normalizedLegalName;
            importer.TaxId =  normalizedTaxID;
            importer.IsActive = true;

            return importer.Adapt<ImporterResponse>();
            }

        
        public async Task<ImporterResponse> UpdateAsync(UpdateImporterRequest request)
        {
            string normalizedLegalName = request.LegalName?.Trim() ?? string.Empty;
            string normalizedTaxID = request.TaxId?.Trim().ToUpperInvariant() ?? string.Empty;

            var existingImporter = await _importerRepository.GetByIdAsync(request.Id);
            if(existingImporter == null)
            {
                throw new ValidationBusinessException(
                    nameof(request.Id),
                    "Importador no encontrado"
                );
            }

            var countryExists = await _countryRepository.GetByIdAsync(request.CountryId);
            if(countryExists == null)
            {
                throw new ValidationBusinessException(
                    nameof(request.CountryId),
                    "El pais seleccionado no existe o no es valido"
                );
            }
            
            if( await _importerRepository.ExistsTaxIdAsync(normalizedLegalName, excludeId: request.Id))
            {
                throw new ValidationBusinessException(
                    nameof(request.TaxId),
                    "El RNC ingresado ya esta siendo utilizado"
                );
            }

            existingImporter = request.Adapt(existingImporter);
            existingImporter.LegalName = normalizedLegalName;
            existingImporter.TaxId = normalizedTaxID;

            await _importerRepository.UpdateAsync(existingImporter);
            return existingImporter.Adapt<ImporterResponse>();
        }
        
        public async Task<bool> DeleteAsync(int id)
        {
            var existingImporter = await _importerRepository.GetByIdAsync(id);
            if(existingImporter == null)
            {
                throw new ValidationBusinessException(
                    nameof(id),
                    "Importador no encontrado"
                );
            }

            if( await _importerRepository.IsImporterReferencedAsync(id))
            {
                throw new BusinessException("No se puede eliminar: el importador posee ordenes de importacion activas");
            }

            await _importerRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> ToogleStatusAsync(int id)
        {
            var existingImporter = await _importerRepository.GetByIdAsync(id);
            if (existingImporter == null)
            {
                throw new ValidationBusinessException(
                    nameof(id),
                    "Importador no encontrado"
                );
            }

            existingImporter.IsActive = !existingImporter.IsActive;
            
            await _importerRepository.UpdateAsync(existingImporter);
            return true;
        }
    }
}
        




