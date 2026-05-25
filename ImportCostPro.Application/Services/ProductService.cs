using ImportCostPro.Application.DTOs.Product.Requests;
using ImportCostPro.Application.DTOs.Product.Responses;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Extensions;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Mapster;

namespace ImportCostPro.Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly ITariffCategoryRepository _tariffCategoryRepository;

        public ProductService(
            IProductRepository productRepository,
            ICountryRepository countryRepository,
            ITariffCategoryRepository tariffCategoryRepository
        )
        {
            _productRepository = productRepository;
            _countryRepository = countryRepository;
            _tariffCategoryRepository = tariffCategoryRepository;
        }

        public async Task<IEnumerable<ProductResponse>> GetAllAsync()
        {
            // Obtenemos todos los productos con sus relaciones
            var products = await _productRepository.GetAllWithRelationsAsync();

            // Mapeamos cada producto a su DTO de respuesta, incluyendo datos cruzados y calculados
            return products.Select(p => p.ToResponse()).ToList();
        }

        public async Task<ProductResponse?> GetByIdAsync(int id)
        {
            // Obtenemos el producto por ID con sus relaciones y validamos q exista
            var product = await _productRepository.GetByIdWithRelationsAsync(id);
            if (product == null)
            {
                return null;
            }
            return product.ToResponse();
        }

        public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
        {
            // Sanitizamos y normalizamos el código de referencia para evitar duplicados por formato
            request.ReferenceCode = request.ReferenceCode?.Trim().ToUpper() ?? string.Empty;

            // Validamos q sea un pais existente y activo
            var country = await _countryRepository.GetByIdAsync(request.DefaultOriginCountryId);
            if (country == null || !country.IsActive)
            {
                throw new ValidationBusinessException(
                    nameof(request.DefaultOriginCountryId),
                    "El país de origen seleccionado no es válido o se encuentra inactivo."
                );
            }

            // Validamos q sea una categoria existente y activa
            var tariffCategory = await _tariffCategoryRepository.GetByIdAsync(
                request.TariffCategoryId
            );
            if (tariffCategory == null || !tariffCategory.IsActive)
            {
                throw new ValidationBusinessException(
                    nameof(request.TariffCategoryId),
                    "La categoría arancelaria seleccionada no es válida o se encuentra inactiva."
                );
            }

            // Validamos que el código de referencia sea único en el catálogo
            bool referenceCodeExists = await _productRepository.ExistsByReferenceCodeAsync(
                request.ReferenceCode
            );
            if (referenceCodeExists)
            {
                throw new ValidationBusinessException(
                    nameof(request.ReferenceCode),
                    $"El código de referencia '{request.ReferenceCode}' ya está registrado en el catálogo."
                );
            }

            // Mapeamos el DTO de entrada a la entidad, establecemos IsActive en true por defecto
            var entity = request.Adapt<Product>();
            entity.IsActive = true;

            await _productRepository.AddAsync(entity);

            // Mapeamos la entidad creada a su DTO de salida, incluyendo datos cruzados y calculados
            return entity.ToResponse(country, tariffCategory);
        }

        public async Task<ProductResponse> UpdateAsync(UpdateProductRequest request)
        {
            // Sanitizamos y normalizamos el código de referencia para evitar duplicados por formato
            request.ReferenceCode = request.ReferenceCode?.Trim().ToUpper() ?? string.Empty;

            // Validamos q el producto exista
            var entity = await _productRepository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                throw new BusinessException(
                    $"El producto que intenta actualizar ya no existe en el sistema."
                );
            }

            // Validamos q sea un pais existente y activo
            var country = await _countryRepository.GetByIdAsync(request.DefaultOriginCountryId);
            if (country == null || !country.IsActive)
            {
                throw new ValidationBusinessException(
                    nameof(request.DefaultOriginCountryId),
                    "El país de origen seleccionado no existe o se encuentra inactivo."
                );
            }

            // Validamos q sea una categoria existente y activa
            var tariffCategory = await _tariffCategoryRepository.GetByIdAsync(
                request.TariffCategoryId
            );
            if (tariffCategory == null || !tariffCategory.IsActive)
            {
                throw new ValidationBusinessException(
                    nameof(request.TariffCategoryId),
                    "La categoría arancelaria seleccionada no existe o se encuentra inactiva."
                );
            }

            // Validamos q el código de referencia sea único en el catálogo, excluyendo el producto actual
            bool referenceCodeExists = await _productRepository.ExistsByReferenceCodeAsync(
                request.ReferenceCode,
                excludeId: request.Id
            );
            if (referenceCodeExists)
            {
                throw new ValidationBusinessException(
                    nameof(request.ReferenceCode),
                    $"El código de referencia '{request.ReferenceCode}' ya está registrado en otro producto."
                );
            }

            // Mapeamos los cambios del DTO de entrada a la entidad existente
            request.Adapt(entity);

            await _productRepository.UpdateAsync(entity);

            // Mapeamos la entidad actualizada a su DTO de salida, incluyendo datos cruzados y calculados
            return entity.ToResponse(country, tariffCategory);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Validamos q el producto exista
            var entity = await _productRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new BusinessException(
                    $"El producto que intenta eliminar no existe en el sistema."
                );
            }

            // Validamos q el producto no esté referenciado en órdenes de importación activas o históricas
            bool isReferenced = await _productRepository.IsProductReferencedInOrdersAsync(id);
            if (isReferenced)
            {
                throw new BusinessException(
                    $"No se puede eliminar el producto '{entity.Name}' porque cuenta con transacciones logísticas u órdenes asociadas."
                );
            }

            // Procedemos a eliminar el producto de forma permanente
            await _productRepository.DeleteAsync(id);

            return true;
        }

        public async Task<bool> ToggleStatusAsync(int id)
        {
            // Validamos q el producto exista
            var entity = await _productRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new BusinessException($"El producto no existe en el sistema.");
            }

            // Simplemente invertimos el estado actual del producto
            entity.IsActive = !entity.IsActive;

            // Guardamos el cambio de estado en la base de datos
            await _productRepository.UpdateAsync(entity);

            return true;
        }
    }
}
