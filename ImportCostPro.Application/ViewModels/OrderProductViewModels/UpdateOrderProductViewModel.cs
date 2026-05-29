using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.OrderProductViewModels
{
    public class UpdateOrderProductViewModel
    {
        [Required(ErrorMessage = "El ID del detalle de producto no es válido.")]
        public int Id { get; set; }

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
