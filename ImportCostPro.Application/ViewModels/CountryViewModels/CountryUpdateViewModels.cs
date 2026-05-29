using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.CountryViewModels
{
    public class CountryUpdateViewModel
    {
        public int Id { get; set;}

        [Required(ErrorMessage = "El Nombre del Pais es requerido")]
        [Display(Name = "Nombre del Pais")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "El Codigo ISO es requerido")]
        [Display(Name = "Codigo ISO")]
        public string IsoCode {get;  set;} = null!;

        [Required(ErrorMessage = "El Estado es requerido")]
        [Display(Name = "Estado")]
        public bool IsActive { get; set;}



    }
}
