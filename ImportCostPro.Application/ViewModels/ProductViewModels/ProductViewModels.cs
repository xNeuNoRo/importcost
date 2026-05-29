using System.ComponentModel.DataAnnotations;
using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.WebApp.Models.Currency
{
    public class ProductViewModels
    {
        public int Id { get; set; }

        [Display(Name = "Nombre del producto")]
        public string Name { get; set; } = null!;

        [Display(Name = "Codigo de referencia")]
        public string ReferenceCode { get; set; } = null!;

        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        [Display(Name = "Peso unitario")]
        public decimal UnitWeight { get; set;}

        [Display(Name = "Largo")]
        public decimal? Length { get; set;}

        [Display(Name = "Ancho")]
        public decimal? Width { get; set;}

        [Display(Name = "Alto")]
        public decimal? Height { get; set;}

        [Display(Name = "Unidad de medida")]
        public UnitOfMeasure UnitOfMeasure { get; set;}

        [Display(Name = "País de origen por defecto")]
        public int DefaultOriginCountryId { get; set; }

        [Display(Name = "Categoría arancelaria")]
        public int TariffCategoryId { get; set; }

        [Display(Name = "Estado")]
        public bool IsActive { get; set; }
    }   
}
