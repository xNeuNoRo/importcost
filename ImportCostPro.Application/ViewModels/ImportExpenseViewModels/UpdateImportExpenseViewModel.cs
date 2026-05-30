using System.ComponentModel.DataAnnotations;
using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Application.ViewModels.ImportExpenseViewModels
{
    public class UpdateImportExpenseViewModel
    {
        [Required(ErrorMessage = "El ID del gasto logístico no es válido.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "La descripción del gasto de importación es obligatoria.")]
        [MaxLength(250, ErrorMessage = "La descripción del gasto no debe exceder los 250 caracteres.")]
        [Display(Name = "Descripción del Gasto")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "La base de distribución seleccionada no es válida.")]
        [EnumDataType(typeof(DistributionBase), ErrorMessage = "La base de distribución seleccionada no es válida.")]
        [Display(Name = "Base de Distribución")]
        public DistributionBase DistributionBase { get; set; }

        [Required(ErrorMessage = "El monto original del gasto logístico debe ser mayor a 0.")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "El monto original del gasto logístico debe ser mayor a 0.")]
        [Display(Name = "Monto Original")]
        public decimal OriginalAmount { get; set; }

        [Required(ErrorMessage = "La fecha del gasto es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha del Gasto")]
        public DateTime ExpenseDate { get; set; }
    }
}
