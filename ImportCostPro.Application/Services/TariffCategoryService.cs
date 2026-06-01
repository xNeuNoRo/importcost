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

        public async Task<bool> IsReferencedAsync(int id)
        {
            return await _tariffCategoryRepository.IsTariffCategoryReferencedAsync(id);
        }

        public async Task<TariffCategoryResponse> CreateAsync(CreateTariffCategoryRequest request)
        {
            string normalizedCode = request.Code.Trim().ToUpperInvariant();
            string normalizedDescription = request.Description.Trim();

            // Validaciones de Reglas de Negocio
            if (request.AppliesExciseTax && request.ExciseTaxRate <= 0)
            {
                throw new ValidationBusinessException(
                    nameof(request.ExciseTaxRate),
                    "Si aplica impuesto selectivo, el porcentaje debe ser mayor que 0."
                );
            }

            if (!request.AppliesExciseTax && request.ExciseTaxRate != 0)
            {
                throw new ValidationBusinessException(
                    nameof(request.ExciseTaxRate),
                    "Si no aplica impuesto selectivo, el porcentaje debe ser 0."
                );
            }

            bool duplicate = await _tariffCategoryRepository.ExistsByCodeAsync(normalizedCode);
            if (duplicate)
            {
                throw new ValidationBusinessException(
                    nameof(request.Code),
                    "Ya existe una categoría arancelaria registrada con este código."
                );
            }

            var entity = TariffCategory.Create(
                normalizedCode,
                normalizedDescription,
                request.CustomsDutyRate,
                request.ExciseTaxRate,
                request.AppliesItbis,
                request.AppliesExciseTax
            );

            await _tariffCategoryRepository.AddAsync(entity);

            return entity.ToResponse();
        }

        public async Task<TariffCategoryResponse> UpdateAsync(UpdateTariffCategoryRequest request)
        {
            string normalizedCode = request.Code.Trim().ToUpperInvariant();
            string normalizedDescription = request.Description.Trim();

            var entity = await _tariffCategoryRepository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                throw new BusinessException(
                    $"La categoría arancelaria con ID {request.Id} no fue encontrada en el sistema."
                );
            }

            // Validaciones de Reglas de Negocio
            if (request.AppliesExciseTax && request.ExciseTaxRate <= 0)
            {
                throw new ValidationBusinessException(
                    nameof(request.ExciseTaxRate),
                    "Si aplica impuesto selectivo, el porcentaje debe ser mayor que 0."
                );
            }

            if (!request.AppliesExciseTax && request.ExciseTaxRate != 0)
            {
                throw new ValidationBusinessException(
                    nameof(request.ExciseTaxRate),
                    "Si no aplica impuesto selectivo, el porcentaje debe ser 0."
                );
            }

            if (await _tariffCategoryRepository.IsTariffCategoryReferencedAsync(request.Id))
            {
                if (
                    entity.Code != normalizedCode
                    || entity.CustomsDutyRate != request.CustomsDutyRate
                    || entity.AppliesItbis != request.AppliesItbis
                    || entity.AppliesExciseTax != request.AppliesExciseTax
                    || entity.ExciseTaxRate != request.ExciseTaxRate
                )
                {
                    throw new BusinessException(
                        "No se pueden modificar campos críticos de esta categoría arancelaria porque ya está asociada a productos en el catálogo."
                    );
                }
            }

            bool duplicate = await _tariffCategoryRepository.ExistsByCodeAsync(
                normalizedCode,
                request.Id
            );
            if (duplicate)
            {
                throw new ValidationBusinessException(
                    nameof(request.Code),
                    "Ya existe una categoría arancelaria registrada con este código."
                );
            }

            entity.UpdateDetails(
                normalizedCode,
                normalizedDescription,
                request.CustomsDutyRate,
                request.ExciseTaxRate,
                request.AppliesItbis,
                request.AppliesExciseTax
            );

            entity.IsActive = request.IsActive;

            await _tariffCategoryRepository.UpdateAsync(entity);

            return entity.ToResponse();
        }

        public async Task<bool> ToggleStatusAsync(int id)
        {
            var entity = await _tariffCategoryRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new BusinessException($"La categoría arancelaria con ID {id} no existe.");
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
                    "No se puede eliminar esta categoría arancelaria porque está asociada a productos registrados."
                );
            }

            await _tariffCategoryRepository.DeleteAsync(id);

            return true;
        }
    }
}
