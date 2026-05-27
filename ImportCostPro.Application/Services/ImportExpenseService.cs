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

        public ImportExpenseService(
            IImportExpenseRepository importExpenseRepository,
            IImportOrderRepository importOrderRepository,
            ICurrencyRepository currencyRepository
        )
        {
            _importExpenseRepository = importExpenseRepository;
            _importOrderRepository = importOrderRepository;
            _currencyRepository = currencyRepository;
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
            string normalizedDescription = request.Description?.Trim() ?? string.Empty;

            ValidateBasicRules(
                normalizedDescription,
                request.ExpenseType,
                request.DistributionBase,
                request.OriginalAmount
            );

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
                    "Está estrictamente prohibido agregar gastos logísticos a una orden en estado %s, Calculated, Closed o Canceled."
                );
            }

            var currency = await _currencyRepository.GetByIdAsync(request.CurrencyId);
            if (currency == null)
            {
                throw new ValidationBusinessException(
                    nameof(request.CurrencyId),
                    $"La moneda con ID {request.CurrencyId} no existe."
                );
            }

            if (!currency.IsActive)
            {
                throw new ValidationBusinessException(
                    nameof(request.CurrencyId),
                    $"La moneda '{currency.Name}' se encuentra inactiva."
                );
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
            };

            await _importExpenseRepository.AddAsync(entity);

            var responseEntity = await _importExpenseRepository.GetByIdWithCurrencyAsync(entity.Id);
            return responseEntity!.ToResponse();
        }

        public async Task<ImportExpenseResponse> UpdateAsync(UpdateImportExpenseRequest request)
        {
            string normalizedDescription = request.Description?.Trim() ?? string.Empty;

            ValidateBasicRules(
                normalizedDescription,
                ExpenseType.OtherExpenses,
                request.DistributionBase,
                request.OriginalAmount
            );

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
                    "No se permite modificar gastos logísticos de una orden en estado Calculated, Closed o Canceled."
                );
            }

            entity.Description = normalizedDescription;
            entity.DistributionBase = request.DistributionBase;
            entity.OriginalAmount = request.OriginalAmount;

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
                    "No se permite eliminar gastos logísticos de una orden en estado Calculated, Closed o Canceled."
                );
            }

            await _importExpenseRepository.DeleteAsync(id);
            return true;
        }

        private static void ValidateBasicRules(
            string description,
            ExpenseType expenseType,
            DistributionBase distributionBase,
            decimal originalAmount
        )
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ValidationBusinessException(
                    nameof(description),
                    "La descripción del gasto de importación es obligatoria."
                );
            }

            if (!Enum.IsDefined(typeof(ExpenseType), expenseType))
            {
                throw new ValidationBusinessException(
                    nameof(expenseType),
                    "El tipo de gasto seleccionado no es válido."
                );
            }

            if (!Enum.IsDefined(typeof(DistributionBase), distributionBase))
            {
                throw new ValidationBusinessException(
                    nameof(distributionBase),
                    "La base de distribución seleccionada no es válida."
                );
            }

            if (originalAmount <= 0)
            {
                throw new ValidationBusinessException(
                    nameof(originalAmount),
                    "El monto original del gasto logístico debe ser mayor a 0."
                );
            }
        }
    }
}
