using ImportCostPro.Application.DTOs.Supplier.Requests;
using ImportCostPro.Application.DTOs.Supplier.Responses;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Extensions;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Mapster;

namespace ImportCostPro.Application.Services
{
    public class SupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly ICurrencyRepository _currencyRepository;

        public SupplierService(
            ISupplierRepository supplierRepository,
            ICountryRepository countryRepository,
            ICurrencyRepository currencyRepository)
        {
            _supplierRepository = supplierRepository;
            _countryRepository = countryRepository;
            _currencyRepository = currencyRepository;
        }

        public async Task<IEnumerable<SupplierResponse>> GetAllAsync()
        {
            var suppliers = await _supplierRepository.GetAllWithRelationsAsync();
            return suppliers.Select(s => s.ToResponse()).ToList();
        }

        public async Task<SupplierResponse?> GetByIdAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdWithRelationsAsync(id);
            if (supplier == null)
            {
                return null;
            }
            return supplier.ToResponse();
        }

        public async Task<SupplierResponse> CreateAsync(CreateSupplierRequest request)
        {
            request.Name = request.Name?.Trim() ?? string.Empty;

            var country = await _countryRepository.GetByIdAsync(request.OriginCountryId);
            if (country == null || !country.IsActive)
            {
                throw new ValidationBusinessException(
                    nameof(request.OriginCountryId),
                    "El país de origen seleccionado no es válido o se encuentra inactivo."
                );
            }

            var currency = await _currencyRepository.GetByIdAsync(request.DefaultCurrencyId);
            if (currency == null || !currency.IsActive)
            {
                throw new ValidationBusinessException(
                    nameof(request.DefaultCurrencyId),
                    "La moneda predeterminada seleccionada no es válida o se encuentra inactiva."
                );
            }

            bool nameExists = await _supplierRepository.ExistsByNameAsync(request.Name);
            if (nameExists)
            {
                throw new ValidationBusinessException(
                    nameof(request.Name),
                    $"El proveedor con el nombre '{request.Name}' ya está registrado."
                );
            }

            var entity = request.Adapt<Supplier>();
            entity.IsActive = true;

            await _supplierRepository.AddAsync(entity);

            return entity.ToResponse(country, currency);
        }

        public async Task<SupplierResponse> UpdateAsync(UpdateSupplierRequest request)
        {
            request.Name = request.Name?.Trim() ?? string.Empty;

            var entity = await _supplierRepository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                throw new BusinessException(
                    "El proveedor que intenta actualizar ya no existe en el sistema."
                );
            }

            var country = await _countryRepository.GetByIdAsync(request.OriginCountryId);
            var currency = await _currencyRepository.GetByIdAsync(request.DefaultCurrencyId);

            bool nameExists = await _supplierRepository.ExistsByNameAsync(request.Name, excludeId: request.Id);
            if (nameExists)
            {
                throw new ValidationBusinessException(
                    nameof(request.Name),
                    $"El nombre '{request.Name}' ya está registrado en otro proveedor."
                );
            }

            bool isReferenced = await _supplierRepository.IsSupplierReferencedAsync(entity.Id);

            if (isReferenced)
            {
                if (entity.OriginCountryId != request.OriginCountryId)
                {
                    throw new ValidationBusinessException(
                        nameof(request.OriginCountryId),
                        "No se puede modificar el país de origen porque el proveedor tiene órdenes de importación registradas."
                    );
                }

                if (entity.DefaultCurrencyId != request.DefaultCurrencyId)
                {
                    throw new ValidationBusinessException(
                        nameof(request.DefaultCurrencyId),
                        "No se puede modificar la moneda porque el proveedor tiene órdenes de importación registradas."
                    );
                }
            }
            else
            {
                if (entity.OriginCountryId != request.OriginCountryId && (country == null || !country.IsActive))
                {
                    throw new ValidationBusinessException(
                        nameof(request.OriginCountryId),
                        "El país de origen seleccionado no existe o se encuentra inactivo."
                    );
                }

                if (entity.DefaultCurrencyId != request.DefaultCurrencyId && (currency == null || !currency.IsActive))
                {
                    throw new ValidationBusinessException(
                        nameof(request.DefaultCurrencyId),
                        "La moneda predeterminada seleccionada no existe o se encuentra inactiva."
                    );
                }
            }

            request.Adapt(entity);

            await _supplierRepository.UpdateAsync(entity);

            return entity.ToResponse(country, currency);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _supplierRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new BusinessException(
                    "El proveedor que intenta eliminar no existe en el sistema."
                );
            }

            bool isReferenced = await _supplierRepository.IsSupplierReferencedAsync(id);
            if (isReferenced)
            {
                throw new BusinessException(
                    $"No se puede eliminar el proveedor '{entity.Name}' porque cuenta con órdenes de importación asociadas."
                );
            }

            await _supplierRepository.DeleteAsync(id);

            return true;
        }

        public async Task<bool> ToggleStatusAsync(int id)
        {
            var entity = await _supplierRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new BusinessException("El proveedor no existe en el sistema.");
            }

            entity.IsActive = !entity.IsActive;

            await _supplierRepository.UpdateAsync(entity);

            return true;
        }
    }
}