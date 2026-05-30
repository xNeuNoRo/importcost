using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.CountryViewModels
{
    public class CountryViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre del País")]
        public string Name { get; set; } = null!;

        [Display(Name = "Código ISO")]
        public string IsoCode { get; set; } = null!;

        [Display(Name = "Estado")]
        public bool IsActive { get; set; }
    }
}
