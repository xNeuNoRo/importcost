using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.CountryViewModels
{
    public class CountryCreateViewModel
    {
        [Required(ErrorMessage = "El nombre del país es requerido.")]
        [Display(Name = "Nombre del País")]
        [MaxLength(150, ErrorMessage = "El nombre del país no debe exceder los 150 caracteres.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "El código ISO del país es requerido.")]
        [Display(Name = "Código ISO")]
        [StringLength(3, MinimumLength = 2, ErrorMessage = "El código ISO del país debe tener entre 2 y 3 caracteres.")]
        public string IsoCode {get;  set;} = null!;
        }
        }
