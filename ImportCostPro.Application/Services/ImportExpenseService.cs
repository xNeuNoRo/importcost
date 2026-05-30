using ImportCostPro.Application.DTOs.ImportExpense.Requests;
using ImportCostPro.Application.DTOs.ImportExpense.Responses;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Extensions;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Enums;
using ImportCostPro.Persistence.Interfaces.Repositories;

namespace ImportCostPro.Application.Services
{
    public class ImportExpenseService
    {
        private readonly IImportExpenseRepository _importExpenseRepository;
        private readonly IImportOrderRepository _importOrderRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IExchangeRateRepository _exchangeRateRepository;

        public ImportExpenseService(
            IImportExpenseRepository importExpenseRepository,
            IImportOrderRepository importOrderRepository,
            ICurrencyRepository currencyRepository,
            IExchangeRateRepository exchangeRateRepository
        )
        {
            _importExpenseRepository = importExpenseRepository;
            _importOrderRepository = importOrderRepository;
            _currencyRepository = currencyRepository;
            _exchangeRateRepository = exchangeRateRepository;
        }

        public async Task<IEnumerable<ImportExpenseResponse>> GetExpensesByOrderIdAsync(
            int importOrderId
        )
        {
            var expenses = await _importExpenseRepository.GetExpensesByOrderIdAsync(importOrderId);
            return expenses.Select(e => e.ToResponse());
        }

        public async Task<ImportExpenseResponse?> GetByIdAsync(int id)
        {
            var expense = await _importExpenseRepository.GetByIdWithCurrencyAsync(id);
            if (expense == null)
            {
                return null;
            }
            return expense.ToResponse();
        }

        public async Task<ImportExpenseResponse> CreateAsync(CreateImportExpenseRequest request)
        {
            string normalizedDescription = request.Description.Trim();

            var orderStatus = await _importOrderRepository.GetStatusByIdAsync(
                request.ImportOrderId
            );
            if (orderStatus == null)
            {
                throw new BusinessException(
                    $"La orden de importación con ID {request.ImportOrderId} no fue encontrada."
                );
            }

            if (
                orderStatus == OrderStatus.Calculated
                || orderStatus == OrderStatus.Closed
                || orderStatus == OrderStatus.Canceled
            )
            {
                throw new BusinessException(
                    "No se puede registrar este gasto porque la orden ya fue calculada, cerrada o cancelada."
                );
            }

            var currency = await _currencyRepository.GetByIdAsync(request.CurrencyId);
            if (currency == null)
            {
                throw new ValidationBusinessException(
                    nameof(request.CurrencyId),
                    "La moneda seleccionada debe existir en el sistema."
                );
            }

            if (!currency.IsActive)
            {
                throw new ValidationBusinessException(
                    nameof(request.CurrencyId),
                    "La moneda seleccionada debe estar activa."
                );
            }

            // Validar tasa de cambio si la moneda no es la local
            if (!currency.IsLocalCurrency)
            {
                var localCurrency = await _currencyRepository.GetLocalCurrencyAsync();
                if (localCurrency == null)
                {
                    throw new BusinessException(
                        "No se ha configurado una moneda local en el sistema."
                    );
                }

                var rate = await _exchangeRateRepository.GetLatestActiveRateAsync(
                    currency.Id,
                    localCurrency.Id,
                    request.ExpenseDate
                );

                if (rate == null)
                {
                    throw new BusinessException(
                        "No existe una tasa de cambio activa desde la moneda del gasto hacia la moneda local para la fecha del gasto."
                    );
                }
            }

            if (
                request.ExpenseType == ExpenseType.InternationalFreight
                && await _importExpenseRepository.HasExpenseTypeAsync(
                    request.ImportOrderId,
                    ExpenseType.InternationalFreight
                )
            )
            {
                throw new ValidationBusinessException(
                    nameof(request.ExpenseType),
                    "Ya existe un gasto de flete internacional registrado para esta orden."
                );
            }

            if (
                request.ExpenseType == ExpenseType.InternationalInsurance
                && await _importExpenseRepository.HasExpenseTypeAsync(
                    request.ImportOrderId,
                    ExpenseType.InternationalInsurance
                )
            )
            {
                throw new ValidationBusinessException(
                    nameof(request.ExpenseType),
                    "Ya existe un gasto de seguro internacional registrado para esta orden."
                );
            }

            var entity = new ImportExpense
            {
                ImportOrderId = request.ImportOrderId,
                CurrencyId = request.CurrencyId,
                Description = normalizedDescription,
                ExpenseType = request.ExpenseType,
                DistributionBase = request.DistributionBase,
                OriginalAmount = request.OriginalAmount,
                ExpenseDate = request.ExpenseDate.Date,
            };

            await _importExpenseRepository.AddAsync(entity);

            var responseEntity = await _importExpenseRepository.GetByIdWithCurrencyAsync(entity.Id);
            return responseEntity!.ToResponse();
        }

        public async Task<ImportExpenseResponse> UpdateAsync(UpdateImportExpenseRequest request)
        {
            string normalizedDescription = request.Description.Trim();

            var entity = await _importExpenseRepository.GetByIdAsync(request.Id);
            if (entity == null)
            {
                throw new BusinessException(
                    $"El registro de gasto con ID {request.Id} no fue encontrado."
                );
            }

            var orderStatus = await _importOrderRepository.GetStatusByIdAsync(entity.ImportOrderId);

            if (
                orderStatus == OrderStatus.Calculated
                || orderStatus == OrderStatus.Closed
                || orderStatus == OrderStatus.Canceled
            )
            {
                throw new BusinessException(
                    "No se puede modificar este gasto porque la orden ya fue calculada, cerrada o cancelada."
                );
            }

            // Validar tasa si cambio la fecha o si la moneda es extranjera
            var currency = await _currencyRepository.GetByIdAsync(request.CurrencyId);
            if (currency != null && !currency.IsLocalCurrency)
            {
                var localCurrency = await _currencyRepository.GetLocalCurrencyAsync();
                var rate = await _exchangeRateRepository.GetLatestActiveRateAsync(
                    request.CurrencyId,
                    localCurrency!.Id,
                    request.ExpenseDate
                );

                if (rate == null)
                {
                    throw new BusinessException(
                        "No existe una tasa de cambio activa desde la moneda del gasto hacia la moneda local para la fecha del gasto."
                    );
                }
            }

            // Validar unicidad de flete/seguro si cambió el tipo
            if (request.ExpenseType != entity.ExpenseType)
            {
                if (
                    request.ExpenseType == ExpenseType.InternationalFreight
                    && await _importExpenseRepository.HasExpenseTypeAsync(
                        entity.ImportOrderId,
                        ExpenseType.InternationalFreight
                    )
                )
                {
                    throw new ValidationBusinessException(
                        nameof(request.ExpenseType),
                        "Ya existe un gasto de flete internacional registrado para esta orden."
                    );
                }

                if (
                    request.ExpenseType == ExpenseType.InternationalInsurance
                    && await _importExpenseRepository.HasExpenseTypeAsync(
                        entity.ImportOrderId,
                        ExpenseType.InternationalInsurance
                    )
                )
                {
                    throw new ValidationBusinessException(
                        nameof(request.ExpenseType),
                        "Ya existe un gasto de seguro internacional registrado para esta orden."
                    );
                }
            }

            entity.Description = normalizedDescription;
            entity.ExpenseType = request.ExpenseType;
            entity.CurrencyId = request.CurrencyId;
            entity.DistributionBase = request.DistributionBase;
            entity.OriginalAmount = request.OriginalAmount;
            entity.ExpenseDate = request.ExpenseDate.Date;

            await _importExpenseRepository.UpdateAsync(entity);

            var responseEntity = await _importExpenseRepository.GetByIdWithCurrencyAsync(entity.Id);
            return responseEntity!.ToResponse();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _importExpenseRepository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new BusinessException($"El registro de gasto con ID {id} no fue encontrado.");
            }

            var orderStatus = await _importOrderRepository.GetStatusByIdAsync(entity.ImportOrderId);

            if (
                orderStatus == OrderStatus.Calculated
                || orderStatus == OrderStatus.Closed
                || orderStatus == OrderStatus.Canceled
            )
            {
                throw new BusinessException(
                    "No se puede eliminar este gasto porque la orden ya fue calculada, cerrada o cancelada."
                );
            }

            await _importExpenseRepository.DeleteAsync(id);
            return true;
        }
    }
}
