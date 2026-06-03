using ImportCostPro.Application.DTOs.LandedCost.Requests;
using ImportCostPro.Application.DTOs.LandedCost.Responses;
using ImportCostPro.Application.Exceptions;
using ImportCostPro.Application.Extensions;
using ImportCostPro.Persistence.Entities;
using ImportCostPro.Persistence.Enums;
using ImportCostPro.Persistence.Interfaces.Providers;
using ImportCostPro.Persistence.Interfaces.Repositories;

namespace ImportCostPro.Application.Services
{
    public class LandedCostService
    {
        private readonly IImportOrderRepository _importOrderRepository;
        private readonly ICalculationResultRepository _calculationResultRepository;
        private readonly IExchangeRateRepository _exchangeRateRepository;
        private readonly ITaxConfigurationRepository _taxConfigurationRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public LandedCostService(
            IImportOrderRepository importOrderRepository,
            ICalculationResultRepository calculationResultRepository,
            IExchangeRateRepository exchangeRateRepository,
            ITaxConfigurationRepository taxConfigurationRepository,
            ICurrencyRepository currencyRepository,
            IDateTimeProvider dateTimeProvider
        )
        {
            _importOrderRepository = importOrderRepository;
            _calculationResultRepository = calculationResultRepository;
            _exchangeRateRepository = exchangeRateRepository;
            _taxConfigurationRepository = taxConfigurationRepository;
            _currencyRepository = currencyRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<LandedCostCalculationResponse> ProcessCalculationAsync(
            ProcessCalculationRequest request
        )
        {
            // Buscamos la moneda local
            var localCurrency = await _currencyRepository.GetLocalCurrencyAsync();

            // Si no hay moneda local configurada, no podemos continuar con el cálculo
            if (localCurrency == null)
            {
                throw new BusinessException(
                    "No se encontró una moneda local activa configurada en el sistema. Por favor, confígurela antes de continuar."
                );
            }

            int localCurrencyId = localCurrency.Id;

            // Obtenemos toda la información de la orden necesaria para el cálculo,
            // incluyendo productos, gastos y sus monedas
            var order = await _importOrderRepository.GetAggregateForCalculationAsync(
                request.ImportOrderId
            );
            if (order == null)
            {
                throw new BusinessException(
                    $"La orden de importación con ID {request.ImportOrderId} no existe."
                );
            }

            // Validamos que la orden esté en un estado que permita el cálculo
            if (order.Status == OrderStatus.Closed || order.Status == OrderStatus.Canceled)
            {
                throw new BusinessException(
                    $"No es posible procesar liquidaciones en una orden con estado {order.Status}."
                );
            }

            // Validamos que la orden tenga productos para costear, de lo contrario el cálculo no tiene sentido
            if (!order.OrderProducts.Any())
            {
                throw new BusinessException(
                    "La orden no contiene productos registrados para costear."
                );
            }

            // Validamos que exista una configuracion de impuestos para
            // poder aplicar las tasas correspondientes en el cálculo
            var taxConfig = await _taxConfigurationRepository.GetSingleConfigurationAsync();
            if (taxConfig == null)
            {
                throw new BusinessException(
                    "No se ha encontrado la configuración de impuestos del sistema."
                );
            }

            // Precalculamos las tasas de impuestos
            decimal itbisRate = taxConfig.GeneralItbisRate / 100m;
            decimal customsServiceRate = taxConfig.CustomsServiceRate / 100m;
            decimal exchangeRateUsed = 1.0m;

            // Validar que existan los gastos obligatorios: Flete y Seguro
            bool hasFreight = order.Expenses.Any(e =>
                e.ExpenseType == ExpenseType.InternationalFreight
            );
            bool hasInsurance = order.Expenses.Any(e =>
                e.ExpenseType == ExpenseType.InternationalInsurance
            );

            if (!hasFreight)
            {
                throw new BusinessException(
                    "La orden debe tener un gasto de tipo Flete internacional registrado para realizar la Liquidación de Costos."
                );
            }

            if (!hasInsurance)
            {
                throw new BusinessException(
                    "La orden debe tener un gasto de tipo Seguro internacional registrado para realizar la Liquidación de Costos."
                );
            }

            // Lista para acumular los IDs de las tasas de cambio utilizadas durante el proceso
            var rateIdsToMark = new List<int>();

            // Si la moneda de la orden es diferente a la local,
            // usamos la fecha de la orden para buscar la tasa
            if (order.CurrencyId != localCurrencyId)
            {
                var rate = await _exchangeRateRepository.GetLatestActiveRateAsync(
                    order.CurrencyId,
                    localCurrencyId,
                    order.OrderDate
                );
                if (rate == null)
                {
                    throw new BusinessException(
                        $"No se encontró una tasa de cambio activa para la moneda de la orden ({order.Currency.IsoCode}) hacia la moneda local ({localCurrency.IsoCode}) para la fecha de la orden."
                    );
                }

                rateIdsToMark.Add(rate.Id);
                exchangeRateUsed = rate.RateValue;
            }

            // Cache de tasas de cambio para gastos, usando la fecha de cada gasto
            var expenseRates = new Dictionary<int, decimal>();
            foreach (var expense in order.Expenses)
            {
                if (expense.CurrencyId == localCurrencyId)
                {
                    expenseRates[expense.Id] = 1.0m;
                    continue;
                }

                var rate = await _exchangeRateRepository.GetLatestActiveRateAsync(
                    expense.CurrencyId,
                    localCurrencyId,
                    expense.ExpenseDate
                );

                if (rate == null)
                {
                    throw new BusinessException(
                        $"No existe una tasa de cambio activa desde la moneda del gasto '{expense.Description}' hacia la moneda local para la fecha del gasto."
                    );
                }

                rateIdsToMark.Add(rate.Id);
                expenseRates[expense.Id] = rate.RateValue;
            }

            // Calculamos el costo de importación línea por línea
            var lineItems = order
                .OrderProducts.Select(op => new CalculationLineItem
                {
                    OrderProductId = op.Id,
                    ProductId = op.ProductId,
                    Product = op.Product,
                    Quantity = op.Quantity,
                    OriginalUnitPriceFob = op.UnitFobPrice,
                    LocalTotalFob = op.Quantity * op.UnitFobPrice * exchangeRateUsed,
                    TotalWeight = op.Quantity * op.Product.UnitWeight,
                    TotalVolume =
                        op.Quantity
                        * (op.Product.Length ?? 0m)
                        * (op.Product.Width ?? 0m)
                        * (op.Product.Height ?? 0m),

                    ProfitMarginRate = op.TargetProfitMargin,
                })
                .ToList();

            // Calculamos los totales globales necesarios para el prorrateo de gastos
            decimal globalTotalLocalFob = lineItems.Sum(x => x.LocalTotalFob);
            decimal globalTotalWeight = lineItems.Sum(x => x.TotalWeight);
            decimal globalTotalVolume = lineItems.Sum(x => x.TotalVolume);
            decimal globalTotalQuantity = lineItems.Sum(x => x.Quantity);

            // Validamos bases de distribución según los gastos registrados
            foreach (var expense in order.Expenses)
            {
                switch (expense.DistributionBase)
                {
                    case DistributionBase.FobValue:
                        if (globalTotalLocalFob <= 0)
                            throw new BusinessException(
                                "No se puede realizar la Liquidación de Costos porque existen gastos distribuidos por valor FOB y el FOB total de la orden es 0."
                            );
                        break;
                    case DistributionBase.Weight:
                        if (globalTotalWeight <= 0)
                            throw new BusinessException(
                                "No se puede realizar la Liquidación de Costos porque existen gastos distribuidos por peso y el peso total de la orden es 0 o algún producto no tiene peso configurado."
                            );
                        break;
                    case DistributionBase.Volume:
                        if (globalTotalVolume <= 0)
                            throw new BusinessException(
                                "No se puede realizar la Liquidación de Costos porque existen gastos distribuidos por volumen y el volumen total de la orden es 0 o algún producto no tiene dimensiones configuradas."
                            );
                        break;
                    case DistributionBase.Quantity:
                        if (globalTotalQuantity <= 0)
                            throw new BusinessException(
                                "No se puede realizar la Liquidación de Costos porque existen gastos distribuidos por cantidad y la cantidad total de productos es 0."
                            );

                        break;
                }
            }

            // Prorrateamos cada gasto
            foreach (var expense in order.Expenses)
            {
                decimal localExpenseAmount = expense.OriginalAmount * expenseRates[expense.Id];

                foreach (var line in lineItems)
                {
                    decimal distributionFactor = expense.DistributionBase switch
                    {
                        DistributionBase.FobValue => line.LocalTotalFob / globalTotalLocalFob,
                        DistributionBase.Weight => line.TotalWeight / globalTotalWeight,
                        DistributionBase.Volume => line.TotalVolume / globalTotalVolume,
                        DistributionBase.Quantity => line.Quantity / globalTotalQuantity,
                        _ => throw new BusinessException(
                            "Base de distribución de prorrateo no soportada."
                        ),
                    };

                    decimal allocatedShare = Math.Round(
                        localExpenseAmount * distributionFactor,
                        10, // Mayor precisión interna
                        MidpointRounding.AwayFromZero
                    );

                    if (expense.ExpenseType == ExpenseType.InternationalFreight)
                        line.AllocatedFreight += allocatedShare;
                    else if (expense.ExpenseType == ExpenseType.InternationalInsurance)
                        line.AllocatedInsurance += allocatedShare;
                    else
                        line.AllocatedLocalExpenses += allocatedShare;
                }
            }

            // Redondeo final de gastos prorrateados a 2 decimales para el resultado oficial
            foreach (var line in lineItems)
            {
                line.AllocatedFreight = Math.Round(
                    line.AllocatedFreight,
                    2,
                    MidpointRounding.AwayFromZero
                );
                line.AllocatedInsurance = Math.Round(
                    line.AllocatedInsurance,
                    2,
                    MidpointRounding.AwayFromZero
                );
                line.AllocatedLocalExpenses = Math.Round(
                    line.AllocatedLocalExpenses,
                    2,
                    MidpointRounding.AwayFromZero
                );
            }

            // Detalles de cálculo por línea
            var resultDetails = new List<CalculationResultDetail>();

            foreach (var line in lineItems)
            {
                decimal localTotalCif = Math.Round(
                    line.LocalTotalFob + line.AllocatedFreight + line.AllocatedInsurance,
                    2,
                    MidpointRounding.AwayFromZero
                );

                decimal tariffPercent = line.Product?.TariffCategory?.CustomsDutyRate ?? 0m;
                decimal customsDutyAmount = Math.Round(
                    localTotalCif * (tariffPercent / 100m),
                    2,
                    MidpointRounding.AwayFromZero
                );

                decimal excisePercent = line.Product?.TariffCategory?.ExciseTaxRate ?? 0m;
                bool appliesExcise = line.Product?.TariffCategory?.AppliesExciseTax ?? false;
                decimal exciseTaxAmount = appliesExcise
                    ? Math.Round(
                        localTotalCif * (excisePercent / 100m),
                        2,
                        MidpointRounding.AwayFromZero
                    )
                    : 0m;

                decimal customsServiceAmount = Math.Round(
                    localTotalCif * (customsServiceRate),
                    2,
                    MidpointRounding.AwayFromZero
                );

                decimal itbisBase = Math.Round(
                    localTotalCif + customsDutyAmount + exciseTaxAmount + customsServiceAmount,
                    2,
                    MidpointRounding.AwayFromZero
                );

                bool appliesItbis = line.Product?.TariffCategory?.AppliesItbis ?? true;
                decimal itbisAmount = appliesItbis
                    ? Math.Round(itbisBase * itbisRate, 2, MidpointRounding.AwayFromZero)
                    : 0m;

                decimal localTotalLandedCost = Math.Round(
                    localTotalCif
                        + customsDutyAmount
                        + customsServiceAmount
                        + exciseTaxAmount
                        + itbisAmount
                        + line.AllocatedLocalExpenses,
                    2,
                    MidpointRounding.AwayFromZero
                );

                decimal unitLandedCost = Math.Round(
                    localTotalLandedCost / line.Quantity,
                    2,
                    MidpointRounding.AwayFromZero
                );

                decimal suggestedRetailPrice =
                    line.ProfitMarginRate > 0m
                        ? Math.Round(
                            unitLandedCost / (1m - (line.ProfitMarginRate / 100m)),
                            2,
                            MidpointRounding.AwayFromZero
                        )
                        : unitLandedCost;

                // Agregamos el detalle de cálculo para esta línea al resultado final
                resultDetails.Add(
                    new CalculationResultDetail
                    {
                        ProductId = line.ProductId,
                        Quantity = line.Quantity,
                        OriginalUnitPriceFob = line.OriginalUnitPriceFob,
                        OriginalTotalFob = line.Quantity * line.OriginalUnitPriceFob,
                        LocalTotalFob = line.LocalTotalFob,
                        AllocatedFreight = line.AllocatedFreight,
                        AllocatedInsurance = line.AllocatedInsurance,
                        LocalTotalCif = localTotalCif,
                        CustomsDutyAmount = customsDutyAmount,
                        ExciseTaxAmount = exciseTaxAmount,
                        CustomsServiceAmount = customsServiceAmount,
                        ItbisAmount = itbisAmount,
                        AllocatedLocalExpenses = line.AllocatedLocalExpenses,
                        LocalTotalLandedCost = localTotalLandedCost,
                        UnitLandedCost = unitLandedCost,
                        ProfitMarginRate = line.ProfitMarginRate,
                        SuggestedRetailPrice = suggestedRetailPrice,
                    }
                );
            }

            // Creamos el resultado de cálculo final con toda la información agregada y lo guardamos en la base de datos
            var calculationResult = new CalculationResult
            {
                ImportOrderId = order.Id,
                LocalCurrencyUsedId = localCurrencyId,
                ExchangeRateUsed = exchangeRateUsed,
                TotalOriginalFob = order.OrderProducts.Sum(x => x.Quantity * x.UnitFobPrice),
                TotalLocalFob = resultDetails.Sum(x => x.LocalTotalFob),
                TotalFreight = resultDetails.Sum(x => x.AllocatedFreight),
                TotalInsurance = resultDetails.Sum(x => x.AllocatedInsurance),
                TotalCif = resultDetails.Sum(x => x.LocalTotalCif),
                TotalTariff = resultDetails.Sum(x => x.CustomsDutyAmount),
                TotalExciseTax = resultDetails.Sum(x => x.ExciseTaxAmount),
                TotalCustomsService = resultDetails.Sum(x => x.CustomsServiceAmount),
                TotalItbis = resultDetails.Sum(x => x.ItbisAmount),
                TotalLocalExpenses = resultDetails.Sum(x => x.AllocatedLocalExpenses),
                TotalImportCost = resultDetails.Sum(x => x.LocalTotalLandedCost),
                TotalImportedQuantity = globalTotalQuantity,
                HistoricalItbisRate = taxConfig.GeneralItbisRate,
                HistoricalCustomsServiceRate = taxConfig.CustomsServiceRate,
                CalculationDate = _dateTimeProvider.UtcNow,
                Details = resultDetails,
            };

            await _calculationResultRepository.AddAsync(calculationResult);
            var updated = await _importOrderRepository.UpdateStatusAsync(
                order.Id,
                OrderStatus.Calculated
            );
            if (!updated)
            {
                throw new BusinessException(
                    "Ocurrió un error al actualizar el estado de la orden después de la liquidación."
                );
            }

            foreach (var rateId in rateIdsToMark)
            {
                await _exchangeRateRepository.MarkAsUsedAsync(rateId);
            }

            // Preparamos un diccionario para mapear rápidamente los datos de los productos
            var productsLookup = lineItems.ToDictionary(
                x => x.ProductId,
                x => (Code: x.Product?.ReferenceCode ?? "N/A", Name: x.Product?.Name ?? "N/A")
            );

            // Devolvemos el resultado del cálculo mapeado a un DTO de respuesta
            return calculationResult.ToResponse(
                order.OrderNumber,
                localCurrency.IsoCode,
                localCurrency.Symbol,
                order.Currency?.IsoCode ?? string.Empty,
                productsLookup
            );
        }

        public async Task<LandedCostCalculationResponse?> GetLatestByOrderIdAsync(int orderId)
        {
            var result = await _calculationResultRepository.GetLatestCalculatedResultWithDetailsAsync(orderId);
            if (result == null) return null;

            var productsLookup = result.Details.ToDictionary(
                x => x.ProductId,
                x => (Code: x.Product?.ReferenceCode ?? "N/A", Name: x.Product?.Name ?? "N/A")
            );

            return result.ToResponse(
                result.ImportOrder?.OrderNumber ?? "N/A",
                result.LocalCurrencyUsed?.IsoCode ?? "RD$",
                result.LocalCurrencyUsed?.Symbol ?? "RD$",
                result.ImportOrder?.Currency?.IsoCode ?? string.Empty,
                productsLookup
            );
        }

        /// <summary>
        /// Clase auxiliar para representar cada línea de producto durante el proceso de cálculo de costo de importación,
        /// incluyendo los valores necesarios para el prorrateo de gastos y el cálculo final del costo de importación.
        /// </summary>
        private class CalculationLineItem
        {
            public int OrderProductId { get; set; }
            public int ProductId { get; set; }
            public Product? Product { get; set; }
            public decimal Quantity { get; set; }
            public decimal OriginalUnitPriceFob { get; set; }
            public decimal LocalTotalFob { get; set; }
            public decimal TotalWeight { get; set; }
            public decimal TotalVolume { get; set; }
            public decimal AllocatedFreight { get; set; }
            public decimal AllocatedInsurance { get; set; }
            public decimal AllocatedLocalExpenses { get; set; }
            public decimal ProfitMarginRate { get; set; }
        }
    }
}
