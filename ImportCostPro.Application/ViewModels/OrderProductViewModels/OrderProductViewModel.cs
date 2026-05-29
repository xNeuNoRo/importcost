using System.ComponentModel.DataAnnotations;
using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Application.ViewModels.OrderProductViewModels
{
    public class OrderProductViewModel
    {
        public int Id { get; set; }
        public int ImportOrderId { get; set; }
        public int ProductId { get; set; }

        [Display(Name = "Cód. Producto")]
        public string ProductReferenceCode { get; set; } = null!;

        [Display(Name = "Producto")]
        public string ProductName { get; set; } = null!;

        [Display(Name = "U.M.")]
        public UnitOfMeasure UnitOfMeasure { get; set; }

        [Display(Name = "Cantidad")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Quantity { get; set; }

        [Display(Name = "FOB Unitario")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal UnitFobPrice { get; set; }

        [Display(Name = "Subtotal FOB")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal SubtotalFob { get; set; }

        [Display(Name = "Margen (%)")]
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public decimal TargetProfitMargin { get; set; }
    }
}
