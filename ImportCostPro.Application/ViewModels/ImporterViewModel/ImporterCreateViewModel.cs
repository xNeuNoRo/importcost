using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.Importer
{
    public class ImporterCreateViewModel
    {
        [Required(ErrorMessage = "La razón social o nombre legal es requerida.")]
        [StringLength(150, ErrorMessage = "La razón social no puede exceder los 150 caracteres.")]
        [Display(Name = "Razón Social")]
        public string LegalName { get; set; } = null!;

        [Required(ErrorMessage = "El documento de identidad fiscal (RNC/Tax ID) es requerido.")]
        [StringLength(20, ErrorMessage = "El RNC/Tax ID no puede exceder los 20 caracteres.")]
        [Display(Name = "RNC / Tax ID")]
        public string TaxId { get; set; } = null!;

        [Required(ErrorMessage = "El país es requerido.")]
        [Display(Name = "País")]
        public int CountryId { get; set; }

        [StringLength(20, ErrorMessage = "El número de teléfono no puede exceder los 20 caracteres.")]
        [Display(Name = "Teléfono")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede exceder los 100 caracteres.")]
        [Display(Name = "Correo Electrónico")]
        public string? Email { get; set; }

        [StringLength(250, ErrorMessage = "La dirección no puede exceder los 250 caracteres.")]
        [Display(Name = "Dirección")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "El estado es requerido.")]
        [Display(Name = "Estado")]
        public bool IsActive { get; set; } = true;
    }
}