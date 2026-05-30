using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.Importer
{
    public class ImporterCreateViewModel
    {
        [Required(ErrorMessage = "El nombre o razón social es requerido.")]
        [MaxLength(150, ErrorMessage = "El nombre o razón social debe tener un máximo de 150 caracteres.")]
        [Display(Name = "Razón Social")]
        public string LegalName { get; set; } = null!;

        [Required(ErrorMessage = "El RNC o identificación fiscal es requerido.")]
        [MaxLength(20, ErrorMessage = "El RNC o identificación fiscal debe tener un máximo de 20 caracteres.")]
        [Display(Name = "RNC / Identificación Fiscal")]
        public string TaxId { get; set; } = null!;

        [Required(ErrorMessage = "El país seleccionado no es válido.")]
        [Display(Name = "País")]
        public int CountryId { get; set; }

        [MaxLength(20, ErrorMessage = "El teléfono debe tener un máximo de 20 caracteres.")]
        [Display(Name = "Teléfono")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "El correo electrónico debe tener un formato válido.")]
        [MaxLength(100, ErrorMessage = "El correo electrónico debe tener un máximo de 100 caracteres.")]
        [Display(Name = "Correo Electrónico")]
        public string? Email { get; set; }

        [MaxLength(250, ErrorMessage = "La dirección debe tener un máximo de 250 caracteres.")]
        [Display(Name = "Dirección")]
        public string? Address { get; set; }

        [Display(Name = "Estado")]
        public bool IsActive { get; set; } = true;
    }
}