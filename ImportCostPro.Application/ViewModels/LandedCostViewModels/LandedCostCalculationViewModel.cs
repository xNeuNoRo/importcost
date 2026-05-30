using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.LandedCostViewModels
{
    public class LandedCostCalculationViewModel
    {
        public int Id { get; set; }
        public int ImportOrderId { get; set; }

        [Display(Name = "Número de Orden")]
        public string OrderNumber { get; set; } = null!;

        [Display(Name = "Fecha de Liquidación")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CalculationDate { get; set; }

        [Display(Name = "Tasa de Cambio (Local)")]
        [DisplayFormat(DataFormatString = "{0:N4}")]
        public decimal ExchangeRateUsed { get; set; }

        [Display(Name = "Divisa Local")]
        public string LocalCurrencyIsoCode { get; set; } = null!;

        [Display(Name = "Valor FOB Total (Local)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalLocalFob { get; set; }

        [Display(Name = "Gastos de Flete Total")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalFreight { get; set; }

        [Display(Name = "Gastos de Seguro Total")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalInsurance { get; set; }

        [Display(Name = "Valor CIF Total Liquidado")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalCif { get; set; }

        [Display(Name = "Total Aranceles")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalTariff { get; set; }

        [Display(Name = "Total Impuesto Selectivo (ISC)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalExciseTax { get; set; }

        [Display(Name = "Total Tasa Servicio Aduanal")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalCustomsService { get; set; }

        [Display(Name = "Total ITBIS Liquidado")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalItbis { get; set; }

        [Display(Name = "Total Otros Gastos Locales")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalLocalExpenses { get; set; }

        [Display(Name = "Costo de Importación Consolidado")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalImportCost { get; set; }

        public List<CalculationResultDetailViewModel> Details { get; set; } = new();
    }
}
