using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.TariffCategoryViewModels
{
    public class TariffCategoryViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Código Arancelario")]
        public string Code { get; set; } = null!;

        [Display(Name = "Nombre o Descripción")]
        public string Description { get; set; } = null!;

        [Display(Name = "Arancel (%)")]
        public decimal CustomsDutyRate { get; set; }

        [Display(Name = "Aplica ITBIS")]
        public bool AppliesItbis { get; set; }

        [Display(Name = "Aplica Selectivo")]
        public bool AppliesExciseTax { get; set; }

        [Display(Name = "Impuesto Selectivo (%)")]
        public decimal ExciseTaxRate { get; set; }

        [Display(Name = "Estado")]
        public bool IsActive { get; set; }
    }
}
