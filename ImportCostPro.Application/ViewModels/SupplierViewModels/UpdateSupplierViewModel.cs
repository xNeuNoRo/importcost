using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.SupplierViewModels
{
    public class UpdateSupplierViewModel
    {
        [Required(ErrorMessage = "El ID del proveedor no es válido.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del proveedor es requerido.")]
        [MaxLength(150, ErrorMessage = "El nombre del proveedor debe tener un máximo de 150 caracteres.")]
        [Display(Name = "Nombre del Proveedor")]
        public string Name { get; set; } = null!;

        [EmailAddress(ErrorMessage = "El correo electrónico debe tener un formato válido.")]
        [MaxLength(100, ErrorMessage = "El correo electrónico debe tener un máximo de 100 caracteres.")]
        [Display(Name = "Correo Electrónico")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "El país de origen seleccionado no es válido.")]
        [Display(Name = "País de Origen")]
        public int OriginCountryId { get; set; }

        [Required(ErrorMessage = "La moneda predeterminada seleccionada no es válida.")]
        [Display(Name = "Moneda Predeterminada")]
        public int DefaultCurrencyId { get; set; }

        [MaxLength(20, ErrorMessage = "El teléfono debe tener un máximo de 20 caracteres.")]
        [Display(Name = "Teléfono")]
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        public bool HasOrders { get; set; }
    }
}