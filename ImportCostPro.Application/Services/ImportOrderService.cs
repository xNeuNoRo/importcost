using ImportCostPro.Application.DTOs.ImportOrder.Requests;
using ImportCostPro.Application.DTOs.ImportOrder.Responses;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Extensions;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Enums;
using ImportCostPro.Persistence.Interfaces.Repositories;

namespace ImportCostPro.Application.Services
{
    public class ImportOrderService
    {
        private readonly IImportOrderRepository _importOrderRepository;
        private readonly IImporterRepository _importerRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly ICurrencyRepository _currencyRepository;

        public ImportOrderService(
            IImportOrderRepository importOrderRepository,
            IImporterRepository importerRepository,
            ISupplierRepository supplierRepository,
            ICountryRepository countryRepository,
            ICurrencyRepository currencyRepository
        )
        {
            _importOrderRepository = importOrderRepository;
            _importerRepository = importerRepository;
            _supplierRepository = supplierRepository;
            _countryRepository = countryRepository;
            _currencyRepository = currencyRepository;
        }

        public async Task<IEnumerable<ImportOrderResponse>> GetAllAsync()
        {
            var orders = await _importOrderRepository.GetAllWithRelationsAsync();
            return orders.Select(o => o.ToResponse());
        }

        public async Task<ImportOrderResponse?> GetByIdAsync(int id)
        {
            var order = await _importOrderRepository.GetByIdWithRelationsAsync(id);
            if (order == null)
            {
                return null;
            }
            return order.ToResponse();
        }

        public async Task<ImportOrderResponse> CreateAsync(CreateImportOrderRequest request)
        {
            string normalizedOrderNumber = request.OrderNumber?.Trim() ?? string.Empty;

            ValidateBasicRules(normalizedOrderNumber, nameof(request.OrderNumber));

            await EnsureRelationsExistAsync(
                request.ImporterId,
                request.SupplierId,
                request.OriginCountryId,
                request.CurrencyId
            );

            if (await _importOrderRepository.ExistsByOrderNumberAsync(normalizedOrderNumber))
            {
                throw new ValidationBusinessException(
                    nameof(request.OrderNumber),
                    $"El número de orden '{normalizedOrderNumber}' ya se encuentra registrado."
                );
            }

            var entity = ImportOrder.Create(
                normalizedOrderNumber,
                request.OrderDate,
                request.TransportMode,
                request.ImporterId,
                request.SupplierId,
                request.OriginCountryId,
                request.CurrencyId
            );

            await _importOrderRepository.AddAsync(entity);

            var responseOrder = await _importOrderRepository.GetByIdWithRelationsAsync(entity.Id);
            return responseOrder!.ToResponse();
        }

        public async Task<ImportOrderResponse> UpdateAsync(UpdateImportOrderRequest request)
        {
            string normalizedOrderNumber = request.OrderNumber?.Trim() ?? string.Empty;

            ValidateBasicRules(normalizedOrderNumber, nameof(request.OrderNumber));

            var currentStatus = await _importOrderRepository.GetStatusByIdAsync(request.Id);
            if (currentStatus == null)
            {
                throw new BusinessException(
                    $"La orden de importación con ID {request.Id} no fue encontrada en el sistema."
                );
            }

            if (currentStatus == OrderStatus.Closed || currentStatus == OrderStatus.Canceled)
            {
                throw new BusinessException(
                    "No se permite editar una orden que se encuentra en estado Closed o Canceled."
                );
            }

            await EnsureRelationsExistAsync(
                request.ImporterId,
                request.SupplierId,
                request.OriginCountryId,
                request.CurrencyId
            );

            if (
                await _importOrderRepository.ExistsByOrderNumberAsync(
                    normalizedOrderNumber,
                    request.Id
                )
            )
            {
                throw new ValidationBusinessException(
                    nameof(request.OrderNumber),
                    $"El número de orden '{normalizedOrderNumber}' ya está en uso por otra importación."
                );
            }

            var entity = await _importOrderRepository.GetByIdAsync(request.Id);

            entity!.UpdateDetails(
                normalizedOrderNumber,
                request.OrderDate,
                request.TransportMode,
                request.ImporterId,
                request.SupplierId,
                request.OriginCountryId,
                request.CurrencyId
            );

            await _importOrderRepository.UpdateAsync(entity);

            var responseOrder = await _importOrderRepository.GetByIdWithRelationsAsync(entity.Id);
            return responseOrder!.ToResponse();
        }

        public async Task<bool> ChangeStatusAsync(int id, OrderStatus newStatus)
        {
            var currentStatus = await _importOrderRepository.GetStatusByIdAsync(id);
            if (currentStatus == null)
            {
                throw new BusinessException(
                    $"La orden de importación con ID {id} no fue encontrada."
                );
            }

            if (currentStatus == OrderStatus.Closed || currentStatus == OrderStatus.Canceled)
            {
                throw new BusinessException(
                    "No se puede cambiar el estado de una orden que ya ha sido cerrada o cancelada definitivamente."
                );
            }

            return await _importOrderRepository.UpdateStatusAsync(id, newStatus);
        }

        private static void ValidateBasicRules(string orderNumber, string orderNumberPropertyName)
        {
            if (string.IsNullOrWhiteSpace(orderNumber))
            {
                throw new ValidationBusinessException(
                    orderNumberPropertyName,
                    "El número de la orden de importación es obligatorio."
                );
            }
        }

        private async Task EnsureRelationsExistAsync(
            int importerId,
            int supplierId,
            int originCountryId,
            int currencyId
        )
        {
            if (!await _importerRepository.ExistsByIdAsync(importerId))
            {
                throw new ValidationBusinessException(
                    nameof(CreateImportOrderRequest.ImporterId),
                    $"El importador con ID {importerId} no existe."
                );
            }

            if (!await _supplierRepository.ExistsByIdAsync(supplierId))
            {
                throw new ValidationBusinessException(
                    nameof(CreateImportOrderRequest.SupplierId),
                    $"El proveedor con ID {supplierId} no existe."
                );
            }

            if (!await _countryRepository.ExistsByIdAsync(originCountryId))
            {
                throw new ValidationBusinessException(
                    nameof(CreateImportOrderRequest.OriginCountryId),
                    $"El país de origen con ID {originCountryId} no existe."
                );
            }

            if (!await _currencyRepository.ExistsByIdAsync(currencyId))
            {
                throw new ValidationBusinessException(
                    nameof(CreateImportOrderRequest.CurrencyId),
                    $"La moneda con ID {currencyId} no existe."
                );
            }
        }
    }
}
