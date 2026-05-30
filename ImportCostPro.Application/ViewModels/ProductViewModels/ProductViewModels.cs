using System.ComponentModel.DataAnnotations;
using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Application.ViewModels.ProductViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre del Producto")]
        public string Name { get; set; } = null!;

        [Display(Name = "Código o Referencia")]
        public string ReferenceCode { get; set; } = null!;

        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        [Display(Name = "Peso Unitario (kg)")]
        public decimal UnitWeight { get; set;}

        [Display(Name = "Largo (cm)")]
        public decimal? Length { get; set;}

        [Display(Name = "Ancho (cm)")]
        public decimal? Width { get; set;}

        [Display(Name = "Alto (cm)")]
        public decimal? Height { get; set;}

        [Display(Name = "Unidad de Medida")]
        public UnitOfMeasure UnitOfMeasure { get; set;}

        [Display(Name = "País de Origen")]
        public string DefaultOriginCountryName { get; set; } = null!;

        [Display(Name = "Categoría Arancelaria")]
        public string TariffCategoryName { get; set; } = null!;

        [Display(Name = "Estado")]
        public bool IsActive { get; set; }
    }   
}
