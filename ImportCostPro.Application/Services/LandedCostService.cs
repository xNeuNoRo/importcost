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
                    $"No es posible procesar cálculos en una orden con estado {order.Status}."
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

            // Lista para acumular los IDs de las tasas de cambio utilizadas durante el proceso,
            // para luego marcarlas como usadas y evitar su reutilización en futuros cálculos.
            var rateIdsToMark = new List<int>();

            // Si la moneda de la orden es diferente a la local,
            // necesitamos convertir los valores FOB a moneda local usando la tasa de cambio
            if (order.CurrencyId != localCurrencyId)
            {
                var rate = await _exchangeRateRepository.GetLatestActiveRateAsync(
                    order.CurrencyId,
                    localCurrencyId,
                    _dateTimeProvider.UtcNow
                );
                if (rate == null)
                {
                    throw new BusinessException(
                        "No se encontró una tasa de cambio activa para la moneda de la orden."
                    );
                }

                // Acumulamos el ID de la tasa en RAM para marcarla como usada al final del proceso
                rateIdsToMark.Add(rate.Id);
                exchangeRateUsed = rate.RateValue;
            }

            // Para optimizar el rendimiento, especialmente en órdenes con muchos productos y gastos,
            // precalculamos las tasas de cambio de las monedas de los gastos
            // y las almacenamos en un diccionario para acceso rápido durante el prorrateo
            var ratesCache = new Dictionary<int, decimal>();

            // Identificamos las monedas extranjeras utilizadas en los gastos de la orden
            var foreignCurrencyIds = order
                .Expenses.Select(e => e.CurrencyId)
                .Where(id => id != localCurrencyId)
                .Distinct();

            // Precalculamos las tasas de cambio para cada moneda extranjera y las almacenamos en el cache
            foreach (var currencyId in foreignCurrencyIds)
            {
                var rate = await _exchangeRateRepository.GetLatestActiveRateAsync(
                    currencyId,
                    localCurrencyId,
                    _dateTimeProvider.UtcNow
                );
                if (rate == null)
                {
                    throw new BusinessException(
                        "Falta configurar una tasa de cambio activa para una de las monedas de los gastos."
                    );
                }

                // Acumulamos el ID de la tasa en RAM y guardamos el valor en el cache
                rateIdsToMark.Add(rate.Id);
                ratesCache[currencyId] = rate.RateValue;
            }

            // Calculamos el costo de importación línea por línea, aplicando el prorrateo de gastos según la base seleccionada
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

            // Validamos que el total local FOB sea mayor a cero
            // para evitar divisiones por cero en el prorrateo basado en valor FOB
            if (globalTotalLocalFob <= 0)
            {
                throw new BusinessException(
                    "El valor total FOB de las líneas de la orden debe ser mayor a cero."
                );
            }

            // Prorrateamos cada gasto de la orden según la base de distribución seleccionada y lo asignamos a cada línea
            foreach (var expense in order.Expenses)
            {
                // Convertimos el monto del gasto a moneda local si es necesario usando el cache de tasas precalculadas
                decimal localExpenseAmount = expense.OriginalAmount;
                if (expense.CurrencyId != localCurrencyId)
                {
                    localExpenseAmount = expense.OriginalAmount * ratesCache[expense.CurrencyId];
                }

                // Calculamos el factor de distribución para cada línea según la
                // base seleccionada y asignamos la parte correspondiente del gasto a cada línea
                foreach (var line in lineItems)
                {
                    decimal distributionFactor = expense.DistributionBase switch
                    {
                        DistributionBase.FobValue => line.LocalTotalFob / globalTotalLocalFob,

                        DistributionBase.Weight => globalTotalWeight > 0
                            ? line.TotalWeight / globalTotalWeight
                            : (line.LocalTotalFob / globalTotalLocalFob),

                        DistributionBase.Volume => globalTotalVolume > 0
                            ? line.TotalVolume / globalTotalVolume
                            : (line.LocalTotalFob / globalTotalLocalFob),

                        DistributionBase.Quantity => globalTotalQuantity > 0
                            ? line.Quantity / globalTotalQuantity
                            : (line.LocalTotalFob / globalTotalLocalFob),

                        _ => throw new BusinessException(
                            "Base de distribución de prorrateo no soportada."
                        ),
                    };

                    decimal allocatedShare = localExpenseAmount * distributionFactor;

                    if (expense.ExpenseType == ExpenseType.InternationalFreight)
                        line.AllocatedFreight += allocatedShare;
                    else if (expense.ExpenseType == ExpenseType.InternationalInsurance)
                        line.AllocatedInsurance += allocatedShare;
                    else
                        line.AllocatedLocalExpenses += allocatedShare;
                }
            }

            // Creamos una lista para almacenar los detalles de cálculo de cada línea,
            // que luego se asociarán al resultado final
            var resultDetails = new List<CalculationResultDetail>();

            foreach (var line in lineItems)
            {
                // Cálculo del costo CIF local para esta línea,
                // que es la suma del FOB local + gastos internacionales prorrateados
                decimal localTotalCif =
                    line.LocalTotalFob + line.AllocatedFreight + line.AllocatedInsurance;

                // Cálculo de impuestos aduanales para esta línea. El monto del arancel se calcula
                // aplicando el porcentaje de arancel correspondiente a la categoría arancelaria
                // del producto sobre el valor CIF local.
                decimal tariffPercent = line.Product?.TariffCategory?.CustomsDutyRate ?? 0m;
                decimal customsDutyAmount = localTotalCif * (tariffPercent / 100m);

                // Cálculo del impuesto de excise para esta línea, si aplica. El monto del excise se calcula
                // aplicando el porcentaje de excise correspondiente a la categoría arancelaria del producto
                // sobre la suma del valor CIF local + aranceles
                decimal excisePercent = line.Product?.TariffCategory?.ExciseTaxRate ?? 0m;
                decimal exciseTaxAmount =
                    excisePercent > 0
                        ? (localTotalCif + customsDutyAmount) * (excisePercent / 100m)
                        : 0m;

                // Cálculo del monto del servicio aduanal para esta línea, si aplica. El monto del servicio aduanal se calcula
                // aplicando el porcentaje de servicio aduanal configurado en la configuración de impuestos
                // sobre el valor CIF local
                decimal customsServiceAmount = localTotalCif * customsServiceRate;

                // Base imponible acumulada del ITBIS para esta línea,
                // que incluye el valor CIF local + impuestos aduanales + impuestos de excise + servicio aduanal
                decimal itbisBase =
                    localTotalCif + customsDutyAmount + exciseTaxAmount + customsServiceAmount;
                decimal itbisAmount = itbisBase * itbisRate;

                // El costo de importación local total asignado a esta línea es
                // la suma del valor CIF local + impuestos aduanales + impuestos de excise + servicio aduanal + gastos locales prorrateados asignados a esta línea
                decimal localTotalLandedCost =
                    localTotalCif
                    + customsDutyAmount
                    + customsServiceAmount
                    + exciseTaxAmount
                    + itbisAmount
                    + line.AllocatedLocalExpenses;
                decimal unitLandedCost = localTotalLandedCost / line.Quantity;

                // Cálculo del precio de venta sugerido para esta línea,
                // aplicando el margen de ganancia objetivo sobre el costo de importación unitario.
                decimal suggestedRetailPrice =
                    line.ProfitMarginRate == 100m ? unitLandedCost * 2m
                    : line.ProfitMarginRate > 0m
                        ? unitLandedCost / (1m - (line.ProfitMarginRate / 100m))
                    : unitLandedCost;

                // Agregamos el detalle de cálculo para esta línea al resultado final
                resultDetails.Add(
                    new CalculationResultDetail
                    {
                        ProductId = line.ProductId,
                        Quantity = line.Quantity,
                        OriginalUnitPriceFob = line.OriginalUnitPriceFob,
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
                    "Ocurrió un error al actualizar el estado de la orden después del cálculo."
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
