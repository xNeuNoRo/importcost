using ImportCostPro.Application.DTOs.TariffCategory.Requests;
using ImportCostPro.Application.DTOs.TariffCategory.Responses;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Extensions;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;

namespace ImportCostPro.Application.Services
{
    public class TariffCategoryService
    {
        private readonly ITariffCategoryRepository _tariffCategoryRepository;

        public TariffCategoryService(ITariffCategoryRepository tariffCategoryRepository)
        {
            _tariffCategoryRepository = tariffCategoryRepository;
        }

        public async Task<IEnumerable<TariffCategoryResponse>> GetAllAsync()
        {
            var categories = await _tariffCategoryRepository.GetAllAsync();
            return categories.Select(c => c.ToResponse());
        }

        public async Task<TariffCategoryResponse?> GetByIdAsync(int id)
        {
            var category = await _tariffCategoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return null;
            }
            return category.ToResponse();
        }

        public async Task<TariffCategoryResponse> CreateAsync(CreateTariffCategoryRequest request)
        {
            string normalizedCode = request.Code?.Trim().ToUpperInvariant() ?? string.Empty;
            string normalizedDescription = request.Description?.Trim() ?? string.Empty;
            decimal normalizedRate = request.CustomsDutyRate;

            ValidateBasicRules(
                normalizedCode,
                normalizedDescription,
                normalizedRate,
                nameof(request.Code),
                nameof(request.Description),
                nameof(request.CustomsDutyRate)
            );

            bool duplicate = await _tariffCategoryRepository.ExistsByCodeAsync(normalizedCode);
            if (duplicate)
            {
                throw new ValidationBusinessException(
                    nameof(request.Code),
                    $"Ya existe una categoría arancelaria registrada con el código '{normalizedCode}'."
                );
            }

            var entity = TariffCategory.Create(
                normalizedCode,
                normalizedDescription,
                normalizedRate
            );

            await _tariffCategoryRepository.AddAsync(entity);

            return entity.ToResponse();
        }

        public async Task<TariffCategoryResponse> UpdateAsync(UpdateTariffCategoryRequest request)
        {
            string normalizedCode = request.Code?.Trim().ToUpperInvariant() ?? string.Empty;
            string normalizedDescription = request.Description?.Trim() ?? string.Empty;
            decimal normalizedRate = request.CustomsDutyRate;

            ValidateBasicRules(
                normalizedCode,
                normalizedDescription,
                normalizedRate,
                nameof(request.Code),
                nameof(request.Description),
                nameof(request.CustomsDutyRate)
            );

            var entity = await _tariffCategoryRepository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                throw new BusinessException(
                    $"La categoría arancelaria con ID {request.Id} no fue encontrada en el sistema."
                );
            }

            if (await _tariffCategoryRepository.IsTariffCategoryReferencedAsync(request.Id))
            {
                throw new BusinessException(
                    "No se puede modificar una categoría arancelaria que ya se encuentra asociada a productos en el catálogo."
                );
            }

            bool duplicate = await _tariffCategoryRepository.ExistsByCodeAsync(
                normalizedCode,
                request.Id
            );
            if (duplicate)
            {
                throw new ValidationBusinessException(
                    nameof(request.Code),
                    $"Ya existe otra categoría arancelaria registrada con el código '{normalizedCode}'."
                );
            }

            entity.UpdateDetails(normalizedCode, normalizedDescription, normalizedRate);

            await _tariffCategoryRepository.UpdateAsync(entity);

            return entity.ToResponse();
        }

        public async Task<bool> ToggleStatusAsync(int id)
        {
            var entity = await _tariffCategoryRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new BusinessException(
                    $"La categoría arancelaria con ID {id} no fue encontrada."
                );
            }

            entity.IsActive = !entity.IsActive;
            await _tariffCategoryRepository.UpdateAsync(entity);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _tariffCategoryRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new BusinessException(
                    $"La categoría arancelaria con ID {id} no fue encontrada."
                );
            }

            if (await _tariffCategoryRepository.IsTariffCategoryReferencedAsync(id))
            {
                throw new BusinessException(
                    "Está prohibido eliminar físicamente una categoría arancelaria que cuenta con histórico de asociación de productos."
                );
            }

            await _tariffCategoryRepository.DeleteAsync(id);

            return true;
        }

        private static void ValidateBasicRules(
            string code,
            string description,
            decimal customsDutyRate,
            string codePropertyName,
            string descriptionPropertyName,
            string ratePropertyName
        )
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ValidationBusinessException(
                    codePropertyName,
                    "El código arancelario oficial de aduanas no puede estar vacío."
                );
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ValidationBusinessException(
                    descriptionPropertyName,
                    "La descripción de la mercancía no puede estar vacía."
                );
            }

            if (customsDutyRate < 0 || customsDutyRate > 100)
            {
                throw new ValidationBusinessException(
                    ratePropertyName,
                    "El porcentaje de arancel aduanero (Gravamen) debe estar configurado entre 0 y 100%."
                );
            }
        }
    }
}
