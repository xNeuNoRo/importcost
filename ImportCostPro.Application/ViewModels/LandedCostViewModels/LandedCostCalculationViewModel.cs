using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.LandedCostViewModels
{
    public class LandedCostCalculationViewModel
    {
        public int Id { get; set; }
        public int ImportOrderId { get; set; }

        [Display(Name = "Número de Orden")]
        public string OrderNumber { get; set; } = null!;

        [Display(Name = "Moneda Local")]
        public string LocalCurrencyIsoCode { get; set; } = null!;

        [Display(Name = "Tasa de Cambio")]
        [DisplayFormat(DataFormatString = "{0:N4}")]
        public decimal ExchangeRateUsed { get; set; }

        [Display(Name = "Total FOB (Original)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal TotalOriginalFob { get; set; }

        [Display(Name = "Total FOB (Local)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalLocalFob { get; set; }

        [Display(Name = "Total Flete")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalFreight { get; set; }

        [Display(Name = "Total Seguro")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalInsurance { get; set; }

        [Display(Name = "Total CIF")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalCif { get; set; }

        [Display(Name = "Total Aranceles")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalTariff { get; set; }

        [Display(Name = "Total Selectivo")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalExciseTax { get; set; }

        [Display(Name = "Total Tasa Servicio")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalCustomsService { get; set; }

        [Display(Name = "Total ITBIS")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalItbis { get; set; }

        [Display(Name = "Gastos Locales")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalLocalExpenses { get; set; }

        [Display(Name = "Costo Total Importación")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalImportCost { get; set; }

        [Display(Name = "Cantidad Total")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal TotalImportedQuantity { get; set; }

        [Display(Name = "Fecha de Cálculo")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime CalculationDate { get; set; }

        public ICollection<CalculationResultDetailViewModel> Details { get; set; } =
            new List<CalculationResultDetailViewModel>();
    }
}
