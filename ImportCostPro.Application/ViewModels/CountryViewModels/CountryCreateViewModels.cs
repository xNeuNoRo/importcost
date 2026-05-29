using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.CountryViewModels
{
    public class CountryCreateViewModel 
    {
        [Required(ErrorMessage = "El Nombre del Pais es requerido")]
        [Display(Name = "Nombre del Pais")]
        [StringLength(100, ErrorMessage = "El nombre no puuede exceder los 100 caracteres.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "El Codigo ISO es requerido")]
        [Display(Name = "Codigo ISO")]
        [StringLength(3, MinimumLength = 2, ErrorMessage = "El codigo ISO debe tener 2 y 3 caracteres.")]
        public string IsoCode {get;  set;} = null!;

        [Required(ErrorMessage = "El Estado es requerido")]
        [Display(Name = "Estado")]
        public bool IsActive { get; set;} = true;

        

    }
}