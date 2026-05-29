using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.TariffCategoryViewModels
{
    public class CreateTariffCategoryViewModel
    {
        [Required(ErrorMessage = "El código arancelario es requerido.")]
        [MaxLength(20, ErrorMessage = "El código arancelario debe tener un máximo de 20 caracteres.")]
        [Display(Name = "Código Arancelario")]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "El nombre o descripción es requerido.")]
        [MaxLength(150, ErrorMessage = "El nombre o descripción debe tener un máximo de 150 caracteres.")]
        [Display(Name = "Nombre o Descripción")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "El porcentaje de arancel es requerido.")]
        [Range(0, 100, ErrorMessage = "El porcentaje de arancel debe estar entre 0 y 100.")]
        [Display(Name = "Arancel (%)")]
        public decimal CustomsDutyRate { get; set; }

        [Required(ErrorMessage = "El campo Aplica ITBIS es requerido.")]
        [Display(Name = "Aplica ITBIS")]
        public bool AppliesItbis { get; set; }

        [Required(ErrorMessage = "El campo Aplica Impuesto Selectivo es requerido.")]
        [Display(Name = "Aplica Selectivo")]
        public bool AppliesExciseTax { get; set; }

        [Range(0, 100, ErrorMessage = "El porcentaje de impuesto selectivo debe estar entre 0 y 100.")]
        [Display(Name = "Impuesto Selectivo (%)")]
        public decimal ExciseTaxRate { get; set; }
    }
}
