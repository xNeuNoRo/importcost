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
        private readonly ICalculationResultRepository _calculationResultRepository;
        private readonly IOrderProductRepository _orderProductRepository;

        public ImportOrderService(
            IImportOrderRepository importOrderRepository,
            IImporterRepository importerRepository,
            ISupplierRepository supplierRepository,
            ICountryRepository countryRepository,
            ICurrencyRepository currencyRepository,
            ICalculationResultRepository calculationResultRepository,
            IOrderProductRepository orderProductRepository)
        {
            _importOrderRepository = importOrderRepository;
            _importerRepository = importerRepository;
            _supplierRepository = supplierRepository;
            _countryRepository = countryRepository;
            _currencyRepository = currencyRepository;
            _calculationResultRepository = calculationResultRepository;
            _orderProductRepository = orderProductRepository;
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
            string normalizedOrderNumber = request.OrderNumber.Trim();

            await EnsureRelationsExistAsync(
                request.ImporterId,
                request.SupplierId,
                request.OriginCountryId,
                request.CurrencyId,
                isCreation: true
            );

            if (await _importOrderRepository.ExistsByOrderNumberAsync(normalizedOrderNumber))
            {
                throw new ValidationBusinessException(
                    nameof(request.OrderNumber),
                    "Ya existe una orden de importación registrada con este número."
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
            string normalizedOrderNumber = request.OrderNumber.Trim();

            var entity = await _importOrderRepository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                throw new BusinessException(
                    $"La orden de importación con ID {request.Id} no fue encontrada en el sistema."
                );
            }

            if (entity.Status == OrderStatus.Closed || entity.Status == OrderStatus.Canceled)
            {
                throw new BusinessException(
                    "No se puede editar esta orden porque está cerrada o cancelada."
                );
            }

            if (entity.Status == OrderStatus.Calculated)
            {
                if (
                    entity.ImporterId != request.ImporterId
                    || entity.SupplierId != request.SupplierId
                    || entity.OriginCountryId != request.OriginCountryId
                    || entity.CurrencyId != request.CurrencyId
                    || entity.OrderDate != request.OrderDate.Date
                    || entity.TransportMode != request.TransportMode
                )
                {
                    throw new BusinessException(
                        "No se pueden modificar estos datos porque la orden ya tiene una Liquidación de Costos oficial."
                    );
                }
            }

            await EnsureRelationsExistAsync(
                request.ImporterId,
                request.SupplierId,
                request.OriginCountryId,
                request.CurrencyId,
                isCreation: false
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
                    "Ya existe una orden de importación registrada con este número."
                );
            }

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
            if (!Enum.IsDefined(typeof(OrderStatus), newStatus))
            {
                throw new ValidationBusinessException(
                    nameof(newStatus),
                    "El estado de la orden de importación especificado no es válido."
                );
            }

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

            if (newStatus == OrderStatus.Closed)
            {
                if (currentStatus != OrderStatus.Calculated)
                {
                    throw new BusinessException(
                        "Solo se puede cerrar una orden que se encuentra en estado Liquidada."
                    );
                }

                var calculation =
                    await _calculationResultRepository.GetLatestCalculatedResultWithDetailsAsync(
                        id
                    );

                if (calculation == null)
                {
                    throw new BusinessException(
                        "No se puede cerrar esta orden porque no tiene una Liquidación de Costos oficial guardada."
                    );
                }

                var products = await _orderProductRepository.GetProductsByOrderIdAsync(id);
                if (products == null || !products.Any())
                {
                    throw new BusinessException(
                        "No se puede cerrar esta orden porque no tiene productos registrados."
                    );
                }
                if (calculation.TotalImportCost <= 0)
                {
                    throw new BusinessException(
                        "No se puede cerrar esta orden porque el costo total de importación de la Liquidación de Costos oficial es 0."
                    );
                }
            }

            if (newStatus == OrderStatus.Calculated && currentStatus != OrderStatus.Open)
            {
                throw new BusinessException(
                    "Solo se puede liquidar una orden que se encuentra en estado Abierta."
                );
            }

            if (newStatus == OrderStatus.Open)
            {
                throw new BusinessException(
                    "No se puede reabrir una orden que ya fue calculada o anulada."
                );
            }

            var success = await _importOrderRepository.UpdateStatusAsync(id, newStatus);
            if (!success)
            {
                throw new BusinessException(
                    $"No se pudo actualizar el estado de la orden de importación con ID {id}."
                );
            }

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _importOrderRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new BusinessException($"La orden de importación con ID {id} no existe.");
            }

            var hasCalculation =
                await _calculationResultRepository.GetLatestCalculatedResultWithDetailsAsync(id);
            if (hasCalculation != null)
            {
                throw new BusinessException(
                    "No se puede eliminar esta orden porque ya tiene una Liquidación de Costos oficial."
                );
            }

            return await _importOrderRepository.DeleteAsync(id);
        }

        private async Task EnsureRelationsExistAsync(
            int importerId,
            int supplierId,
            int originCountryId,
            int currencyId,
            bool isCreation
        )
        {
            var importer = await _importerRepository.GetByIdAsync(importerId);
            if (importer == null || (isCreation && !importer.IsActive))
            {
                throw new ValidationBusinessException(
                    nameof(CreateImportOrderRequest.ImporterId),
                    "El importador seleccionado debe existir y estar activo."
                );
            }

            var supplier = await _supplierRepository.GetByIdAsync(supplierId);
            if (supplier == null || (isCreation && !supplier.IsActive))
            {
                throw new ValidationBusinessException(
                    nameof(CreateImportOrderRequest.SupplierId),
                    "El proveedor seleccionado debe existir y estar activo."
                );
            }

            var country = await _countryRepository.GetByIdAsync(originCountryId);
            if (country == null || (isCreation && !country.IsActive))
            {
                throw new ValidationBusinessException(
                    nameof(CreateImportOrderRequest.OriginCountryId),
                    "El país seleccionado debe existir y estar activo."
                );
            }

            var currency = await _currencyRepository.GetByIdAsync(currencyId);
            if (currency == null || (isCreation && !currency.IsActive))
            {
                throw new ValidationBusinessException(
                    nameof(CreateImportOrderRequest.CurrencyId),
                    "La moneda seleccionada debe existir y estar activa."
                );
            }
        }
    }
}
