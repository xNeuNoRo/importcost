using System.ComponentModel.DataAnnotations;
using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Application.ViewModels.ProductViewModels
{
    public class ProductUpdateViewModel
    {        
        [Required(ErrorMessage = "El ID del producto no es válido.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es requerido.")]
        [MaxLength(150, ErrorMessage = "El nombre del producto debe tener un máximo de 150 caracteres.")]
        [Display(Name = "Nombre del Producto")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "El código o referencia es requerido.")]
        [MaxLength(50, ErrorMessage = "El código o referencia debe tener un máximo de 50 caracteres.")]
        [Display(Name = "Código o Referencia")]
        public string ReferenceCode { get; set; } = null!;

        [MaxLength(250, ErrorMessage = "La descripción debe tener un máximo de 250 caracteres.")]
        [Display(Name = "Descripción")]
        public string? Description { get; set; } 

        [Required(ErrorMessage = "El peso unitario es requerido.")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "El peso unitario debe ser mayor que 0.")]
        [Display(Name = "Peso Unitario (kg)")]
        public decimal UnitWeight { get; set; } 

        [Display(Name = "Largo (cm)")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "El largo debe ser mayor que 0.")]
        public decimal? Length { get; set; } 

        [Display(Name = "Ancho (cm)")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "El ancho debe ser mayor que 0.")]
        public decimal? Width { get; set; }

        [Display(Name = "Alto (cm)")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "El alto debe ser mayor que 0.")]
        public decimal? Height { get; set; }

        [Required(ErrorMessage = "La unidad de medida es requerida.")]
        [EnumDataType(typeof(UnitOfMeasure), ErrorMessage = "La unidad de medida seleccionada no es válida.")]
        [Display(Name = "Unidad de Medida")]
        public UnitOfMeasure UnitOfMeasure { get; set; }

        [Required(ErrorMessage = "El país de origen por defecto es requerido.")]
        [Display(Name = "País de Origen por Defecto")]
        public int DefaultOriginCountryId { get; set; }

        [Required(ErrorMessage = "La categoría arancelaria es requerida.")]
        [Display(Name = "Categoría Arancelaria")]
        public int TariffCategoryId { get; set; }

        [Display(Name = "Estado")]
        public bool IsActive { get; set; }
    }
}
