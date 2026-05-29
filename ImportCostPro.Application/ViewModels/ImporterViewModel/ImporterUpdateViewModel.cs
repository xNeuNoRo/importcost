using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.Importer
{
    public class ImporterUpdateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="La razon social o nombre legal es requerida")]
        [StringLength(150, ErrorMessage ="La Razon social no puede exceder los 150 caracteres")]
        [Display(Name = "Razon Social")]
        public string LegalName { get; set; } = null!;

        
        [Required(ErrorMessage ="El documento de intedidad fisca; (RNC/Tax Id) es requerido")]
        [StringLength(150, ErrorMessage ="El RNC no puede exceder los 20")]
        [Display(Name = "RNC")]
        public string TaxId { get; set; } = null!;

        [Required(ErrorMessage ="El pais es requerido")]
        [Display(Name = "Pais")]
        public int CountryId { get; set;}

        [StringLength(20, ErrorMessage ="El numero de telefono no puede exceder los 20 caracteres")]
        [Display(Name = "Telefono")]
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
        public bool IsActive { get; set; }

    }
}