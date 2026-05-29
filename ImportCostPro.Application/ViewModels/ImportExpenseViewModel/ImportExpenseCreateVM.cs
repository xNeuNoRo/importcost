using System.ComponentModel.DataAnnotations;
using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Application.ViewModels.ImportExpense
{
    public class ImportExpenseCreateVM
    {
        [Required(ErrorMessage = "La orden de importacion es requerida.")]
        [Display(Name = "Orden de importación")]
        public int ImportOrderId { get; set; }

        [Required(ErrorMessage = "La moneda es requerida")]
        [Display(Name = "Moneda")]
        public int CurrencyId {get; set;}

        [Required(ErrorMessage = "La descripción del gasto es requerida.")]
        [StringLength(250, ErrorMessage = "La descripción no puede exceder los 250 caracteres.")]
        [Display(Name = "Descripción del Gasto")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "El tipo de gasto es requerido.")]
        [Display(Name = "Tipo de Gasto")]
        public ExpenseType ExpenseType { get; set; }

        [Required(ErrorMessage = "La base de distribución es requerida.")]
        [Display(Name = "Base de Distribución")]
        public DistributionBase DistributionBase { get; set; }

        [Required(ErrorMessage = "El monto original es requerido.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
        [Display(Name = "Monto Original")]
        public decimal OriginalAmount { get; set; }
    }
}
