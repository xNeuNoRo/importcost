using System.ComponentModel.DataAnnotations;
using ImportCostPro.Application.DTOs.Currency.Responses;

namespace ImportCostPro.Application.ViewModels.DashboardViewModels
{
    public class DashboardViewModel
    {
        [Display(Name = "Órdenes Abiertas")]
        public int OpenOrdersCount { get; set; }

        [Display(Name = "Proveedores Externos")]
        public int ActiveSuppliersCount { get; set; }

        [Display(Name = "Inversión Liquidada (Mes)")]
        public decimal MonthlyLiquidatedTotal { get; set; }

        [Display(Name = "Tasas por Actualizar")]
        public int PendingExchangeRatesCount { get; set; }
        
        public string CurrencySymbol { get; set; } = "RD$";

        public List<CurrencyResponse> MissingCurrencies { get; set; } = new();
    }
}
