using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.OrderProductViewModels
{
    public class CreateOrderProductViewModel
    {
        [Required(ErrorMessage = "La orden de importación seleccionada no es válida.")]
        [Range(1, int.MaxValue, ErrorMessage = "La orden de importación seleccionada no es válida.")]
        [Display(Name = "Orden de Importación")]
        public int ImportOrderId { get; set; }

        [Required(ErrorMessage = "El producto seleccionado no es válido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El producto seleccionado no es válido.")]
        [Display(Name = "Producto")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "La cantidad del producto debe ser mayor a 0.")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "La cantidad del producto debe ser mayor a 0.")]
        [Display(Name = "Cantidad")]
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = "El precio FOB unitario del producto debe ser mayor a 0.")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "El precio FOB unitario del producto debe ser mayor a 0.")]
        [Display(Name = "Precio FOB Unitario")]
        public decimal UnitFobPrice { get; set; }

        [Required(ErrorMessage = "El margen de ganancia esperado debe ser mayor o igual que 0 y menor que 100%.")]
        [Range(0, 99.99, ErrorMessage = "El margen de ganancia esperado debe ser mayor o igual que 0 y menor que 100%.")]
        [Display(Name = "Margen de Ganancia (%)")]
        public decimal TargetProfitMargin { get; set; }
    }
}
