using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.LandedCostViewModels
{
    public class CalculationResultDetailViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        [Display(Name = "Cód. Producto")]
        public string ProductReferenceCode { get; set; } = null!;

        [Display(Name = "Producto")]
        public string ProductName { get; set; } = null!;

        [Display(Name = "Cantidad")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Quantity { get; set; }

        [Display(Name = "FOB Unitario")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal OriginalUnitPriceFob { get; set; }

        [Display(Name = "FOB Total")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal OriginalTotalFob { get; set; }

        [Display(Name = "FOB Local")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal LocalTotalFob { get; set; }

        [Display(Name = "Flete Prorrateado")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal AllocatedFreight { get; set; }

        [Display(Name = "Seguro Prorrateado")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal AllocatedInsurance { get; set; }

        [Display(Name = "CIF Local")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal LocalTotalCif { get; set; }

        [Display(Name = "Arancel")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal CustomsDutyAmount { get; set; }

        [Display(Name = "Selectivo")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal ExciseTaxAmount { get; set; }

        [Display(Name = "Tasa Servicio")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal CustomsServiceAmount { get; set; }

        [Display(Name = "ITBIS")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal ItbisAmount { get; set; }

        [Display(Name = "Gastos Locales")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal AllocatedLocalExpenses { get; set; }

        [Display(Name = "Costo Total")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal LocalTotalLandedCost { get; set; }

        [Display(Name = "Costo Unitario")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal UnitLandedCost { get; set; }

        [Display(Name = "Margen (%)")]
        [DisplayFormat(DataFormatString = "{0:N2}%")]
        public decimal ProfitMarginRate { get; set; }

        [Display(Name = "Precio Sugerido")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal SuggestedRetailPrice { get; set; }
    }
}
