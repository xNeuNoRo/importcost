using ImportCostPro.Application.DTOs.OrderProduct.Requests;
using ImportCostPro.Application.DTOs.OrderProduct.Responses;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Extensions;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Enums;
using ImportCostPro.Persistence.Interfaces.Repositories;

namespace ImportCostPro.Application.Services
{
    public class OrderProductService
    {
        private readonly IOrderProductRepository _orderProductRepository;
        private readonly IImportOrderRepository _importOrderRepository;
        private readonly IProductRepository _productRepository;

        public OrderProductService(
            IOrderProductRepository orderProductRepository,
            IImportOrderRepository importOrderRepository,
            IProductRepository productRepository
        )
        {
            _orderProductRepository = orderProductRepository;
            _importOrderRepository = importOrderRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<OrderProductResponse>> GetProductsByOrderIdAsync(
            int importOrderId
        )
        {
            var orderProducts = await _orderProductRepository.GetProductsByOrderIdAsync(
                importOrderId
            );
            return orderProducts.Select(op => op.ToResponse());
        }

        public async Task<OrderProductResponse?> GetByIdAsync(int id)
        {
            var orderProduct = await _orderProductRepository.GetByIdAsync(id);
            if (orderProduct == null)
            {
                return null;
            }
            return orderProduct.ToResponse();
        }

        public async Task<OrderProductResponse> CreateAsync(CreateOrderProductRequest request)
        {
            ValidateBasicRules(
                request.Quantity,
                request.UnitFobPrice,
                request.TargetProfitMargin,
                nameof(request.Quantity),
                nameof(request.UnitFobPrice),
                nameof(request.TargetProfitMargin)
            );

            var orderStatus = await _importOrderRepository.GetStatusByIdAsync(
                request.ImportOrderId
            );
            if (orderStatus == null)
            {
                throw new BusinessException(
                    $"La orden de importación con ID {request.ImportOrderId} no existe."
                );
            }

            if (orderStatus == OrderStatus.Closed || orderStatus == OrderStatus.Canceled)
            {
                throw new BusinessException(
                    "Está prohibido agregar productos a una orden en estado Closed o Canceled."
                );
            }

            if (!await _productRepository.ExistsByIdAsync(request.ProductId))
            {
                throw new ValidationBusinessException(
                    nameof(request.ProductId),
                    $"El producto con ID {request.ProductId} no existe en el catálogo maestro."
                );
            }

            if (
                await _orderProductRepository.IsProductAlreadyInOrderAsync(
                    request.ImportOrderId,
                    request.ProductId
                )
            )
            {
                throw new ValidationBusinessException(
                    nameof(request.ProductId),
                    "Este producto ya se encuentra registrado en la orden de importación. Si desea modificar su cantidad o precio, proceda a editar el registro existente."
                );
            }

            var entity = new OrderProduct
            {
                ImportOrderId = request.ImportOrderId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                UnitFobPrice = request.UnitFobPrice,
                TargetProfitMargin = request.TargetProfitMargin,
            };

            await _orderProductRepository.AddAsync(entity);

            var responseEntity = await _orderProductRepository.GetByIdAsync(entity.Id);
            return responseEntity!.ToResponse();
        }

        public async Task<OrderProductResponse> UpdateAsync(UpdateOrderProductRequest request)
        {
            ValidateBasicRules(
                request.Quantity,
                request.UnitFobPrice,
                request.TargetProfitMargin,
                nameof(request.Quantity),
                nameof(request.UnitFobPrice),
                nameof(request.TargetProfitMargin)
            );

            var entity = await _orderProductRepository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                throw new BusinessException(
                    $"El detalle de producto con ID {request.Id} no fue encontrado."
                );
            }

            var orderStatus = await _importOrderRepository.GetStatusByIdAsync(entity.ImportOrderId);

            if (orderStatus == OrderStatus.Closed || orderStatus == OrderStatus.Canceled)
            {
                throw new BusinessException(
                    "No se permite modificar líneas de productos de una orden que se encuentra en estado Closed o Canceled."
                );
            }

            entity.Quantity = request.Quantity;
            entity.UnitFobPrice = request.UnitFobPrice;
            entity.TargetProfitMargin = request.TargetProfitMargin;

            await _orderProductRepository.UpdateAsync(entity);

            return entity.ToResponse();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _orderProductRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new BusinessException(
                    $"El detalle de producto con ID {id} no fue encontrado."
                );
            }

            var orderStatus = await _importOrderRepository.GetStatusByIdAsync(entity.ImportOrderId);

            if (orderStatus == OrderStatus.Closed || orderStatus == OrderStatus.Canceled)
            {
                throw new BusinessException(
                    "No se permite eliminar líneas de productos de una orden que se encuentra en estado Closed o Canceled."
                );
            }

            await _orderProductRepository.DeleteAsync(id);
            return true;
        }

        private static void ValidateBasicRules(
            decimal quantity,
            decimal unitFobPrice,
            decimal targetProfitMargin,
            string quantityPropName,
            string pricePropName,
            string marginPropName
        )
        {
            if (quantity <= 0)
            {
                throw new ValidationBusinessException(
                    quantityPropName,
                    "La cantidad del producto debe ser mayor a 0."
                );
            }

            if (unitFobPrice <= 0)
            {
                throw new ValidationBusinessException(
                    pricePropName,
                    "El precio FOB unitario del producto debe ser mayor a 0."
                );
            }

            if (targetProfitMargin <= 0 || targetProfitMargin > 100)
            {
                throw new ValidationBusinessException(
                    marginPropName,
                    "El margen de ganancia esperado debe ser configurado entre 0.01% y 100%."
                );
            }
        }
    }
}
