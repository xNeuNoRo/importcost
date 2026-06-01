using ImportCostPro.Application.DTOs.Currency.Responses;
using ImportCostPro.Application.ViewModels.DashboardViewModels;
using ImportCostPro.Persistence.Enums;
using ImportCostPro.Persistence.Interfaces.Providers;
using ImportCostPro.Persistence.Interfaces.Repositories;
using Mapster;

namespace ImportCostPro.Application.Services
{
    public class DashboardService
    {
        private readonly IImportOrderRepository _importOrderRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ICalculationResultRepository _calculationResultRepository;
        private readonly IExchangeRateRepository _exchangeRateRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public DashboardService(
            IImportOrderRepository importOrderRepository,
            ISupplierRepository supplierRepository,
            ICalculationResultRepository calculationResultRepository,
            IExchangeRateRepository exchangeRateRepository,
            ICurrencyRepository currencyRepository,
            IDateTimeProvider dateTimeProvider)
        {
            _importOrderRepository = importOrderRepository;
            _supplierRepository = supplierRepository;
            _calculationResultRepository = calculationResultRepository;
            _exchangeRateRepository = exchangeRateRepository;
            _currencyRepository = currencyRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        /// <summary>
        /// Obtiene las estadísticas consolidadas para el panel de control.
        /// Realiza consultas optimizadas y ligeras a la base de datos.
        /// </summary>
        public async Task<DashboardViewModel> GetStatisticsAsync()
        {
            var now = _dateTimeProvider.UtcNow;
            
            // Consultas asíncronas independientes
            var openOrdersCount = await _importOrderRepository.CountByStatusAsync(OrderStatus.Open);
            var activeSuppliersCount = await _supplierRepository.CountAsync(s => s.IsActive);
            var monthlyTotal = await _calculationResultRepository.GetMonthlyTotalImportCostAsync(now.Month, now.Year);
            var pendingRatesCount = await _exchangeRateRepository.GetActiveCurrenciesMissingRateCountAsync(now);
            var missingCurrencies = await _exchangeRateRepository.GetActiveCurrenciesMissingRateAsync(now);
            
            var localCurrency = await _currencyRepository.GetLocalCurrencyAsync();

            return new DashboardViewModel
            {
                OpenOrdersCount = openOrdersCount,
                ActiveSuppliersCount = activeSuppliersCount,
                MonthlyLiquidatedTotal = monthlyTotal,
                PendingExchangeRatesCount = pendingRatesCount,
                MissingCurrencies = missingCurrencies.Adapt<List<CurrencyResponse>>(),
                CurrencySymbol = localCurrency?.Symbol ?? "RD$"
            };
        }
    }
}
