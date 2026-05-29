using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.SupplierViewModels
{
    public class UpdateSupplierViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder los 150 caracteres.")]
        [Display(Name = "Nombre")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Debe seleccionar un país de origen.")]
        [Display(Name = "País de Origen")]
        public int OriginCountryId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una moneda predeterminada.")]
        [Display(Name = "Moneda Predeterminada")]
        public int DefaultCurrencyId { get; set; }

        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [StringLength(150, ErrorMessage = "El correo no puede exceder los 150 caracteres.")]
        [Display(Name = "Correo Electrónico")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres.")]
        [Display(Name = "Teléfono")]
        public string? PhoneNumber { get; set; }

        public bool HasOrders { get; set; }

    }
}