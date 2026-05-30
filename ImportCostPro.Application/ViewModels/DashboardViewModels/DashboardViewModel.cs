using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.DashboardViewModels
{
    public class DashboardViewModel
    {
        [Display(Name = "Órdenes Abiertas")]
        public int OpenOrdersCount { get; set; }

        [Display(Name = "Proveedores Externos")]
        public int ActiveSuppliersCount { get; set; }

        [Display(Name = "Inversión Liquidada (Mes)")]
        public decimal MonthlyLandedCostTotal { get; set; }

        [Display(Name = "Tasas por Actualizar")]
        public int PendingExchangeRatesCount { get; set; }
        
        public string CurrencySymbol { get; set; } = "RD$";
    }
}
