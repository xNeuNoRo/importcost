using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.LandedCostViewModels
{
    public class CalculationResultDetailViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        [Display(Name = "Referencia")]
        public string ProductReferenceCode { get; set; } = null!;

        [Display(Name = "Descripción del Producto")]
        public string ProductName { get; set; } = null!;

        [Display(Name = "Cantidad")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Quantity { get; set; }

        [Display(Name = "FOB Unitario (Origen)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal OriginalUnitPriceFob { get; set; }

        [Display(Name = "Valor FOB Total (Origen)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal OriginalTotalFob { get; set; }

        [Display(Name = "Valor FOB Total (Local)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal LocalTotalFob { get; set; }

        [Display(Name = "Flete Prorrateado")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal AllocatedFreight { get; set; }

        [Display(Name = "Seguro Prorrateado")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal AllocatedInsurance { get; set; }

        [Display(Name = "Valor CIF Local")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal LocalTotalCif { get; set; }

        [Display(Name = "Monto Arancel")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal CustomsDutyAmount { get; set; }

        [Display(Name = "Impuesto Selectivo (ISC)")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal ExciseTaxAmount { get; set; }

        [Display(Name = "Tasa de Servicio")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal CustomsServiceAmount { get; set; }

        [Display(Name = "ITBIS Liquidado")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal ItbisAmount { get; set; }

        [Display(Name = "Gastos Locales Asignados")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal AllocatedLocalExpenses { get; set; }

        [Display(Name = "Costo Total de Importación")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal LocalTotalLandedCost { get; set; }

        [Display(Name = "Costo Unitario en Almacén")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal UnitLandedCost { get; set; }

        [Display(Name = "Margen de Beneficio (%)")]
        [DisplayFormat(DataFormatString = "{0:N2}%")]
        public decimal ProfitMarginRate { get; set; }

        [Display(Name = "Precio de Venta Sugerido")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal SuggestedRetailPrice { get; set; }
    }
}
