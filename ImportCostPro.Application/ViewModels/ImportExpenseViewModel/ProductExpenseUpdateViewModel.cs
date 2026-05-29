using System.ComponentModel.DataAnnotations;
using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Application.ViewModels.ImportExpense
{
    public class ImportExpenseViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "La descripción del gasto es requerida.")]
        [StringLength(250, ErrorMessage = "La descripción no puede exceder los 150 caracteres.")]
        [Display(Name = "Descripción del Gasto")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "La base de distribución es requerida.")]
        [Display(Name = "Base de Distribución")]
        public DistributionBase DistributionBase { get; set; }

        [Required(ErrorMessage = "El monto original es requerido.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
        [Display(Name = "Monto Original")]
        public decimal OriginalAmount { get; set; }
    }
}
    