using System.ComponentModel.DataAnnotations;
using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Application.ViewModels.ImportOrderViewModels
{
    public class UpdateImportOrderViewModel
    {
        [Required(ErrorMessage = "El ID de la orden no es válido.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El número de orden es requerido.")]
        [MaxLength(30, ErrorMessage = "El número de orden debe tener un máximo de 30 caracteres.")]
        [Display(Name = "Número de Orden")]
        public string OrderNumber { get; set; } = null!;

        [Required(ErrorMessage = "El importador seleccionado no es válido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El importador seleccionado no es válido.")]
        [Display(Name = "Importador")]
        public int ImporterId { get; set; }

        [Required(ErrorMessage = "El proveedor seleccionado no es válido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El proveedor seleccionado no es válido.")]
        [Display(Name = "Proveedor")]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "El país seleccionado no es válido.")]
        [Range(1, int.MaxValue, ErrorMessage = "El país seleccionado no es válido.")]
        [Display(Name = "País de Origen")]
        public int OriginCountryId { get; set; }

        [Required(ErrorMessage = "La moneda seleccionada no es válida.")]
        [Range(1, int.MaxValue, ErrorMessage = "La moneda seleccionada no es válida.")]
        [Display(Name = "Moneda")]
        public int CurrencyId { get; set; }

        [Required(ErrorMessage = "La modalidad de transporte seleccionada no es válida.")]
        [EnumDataType(typeof(TransportMode), ErrorMessage = "La modalidad de transporte seleccionada no es válida.")]
        [Display(Name = "Modalidad de Transporte")]
        public TransportMode TransportMode { get; set; }

        [Required(ErrorMessage = "La fecha de la orden es requerida.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de la Orden")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Estado")]
        public OrderStatus Status { get; set; }
    }
}
