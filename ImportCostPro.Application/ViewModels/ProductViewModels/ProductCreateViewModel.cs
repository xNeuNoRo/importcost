using System.ComponentModel.DataAnnotations;
using ImportCostPro.Persistence.Repositories;
using ImportCostPro.Persistence.Enums;



namespace ImportCostPro.Application.ViewModelsModels.Product
{
    public class ProductViewModel 
    {
        [Required(ErrorMessage = "El nombre del producto es requerido")]
        [StringLength(150, ErrorMessage = "El nombre del producto no puede exceder los 150 caracteres")]
        [Display(Name = "Nombre del producto")]
        public string Name { get; set; } = null!;
        
        [Required(ErrorMessage = "El codigo de referencia es requerida")]
        [StringLength(150, ErrorMessage = "El codigo de referencia no puede exceder los 50 caracteres")]
        [Display(Name = "Codigo de referencia")]
        public string ReferenceCode { get; set; } = null!;

        [StringLength(250,ErrorMessage ="La descripcion no puede exceder los 250 caracteres")]
        [Display(Name = "Descripción")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "El peso unitario es requerido")]
        [Display(Name = "Peso unitario")]
        public decimal UnitWeight { get; set;}

        [Display(Name = "Largo")]
        public decimal? Length { get; set;}

        [Display(Name = "Ancho")]
        public decimal? Width { get; set;}

        [Display(Name = "Alto")]
        public decimal? Height { get; set;}

        [Required(ErrorMessage = "La unidad de medida es requerida")]
        [Display(Name = "Unidad de medida")]
        public UnitOfMeasure UnitOfMeasure { get; set;}

        [Required(ErrorMessage = "El país de origen por defecto es requerido.")]
        [Display(Name = "País de origen por defecto")]
        public int DefaultOriginCountryId { get; set; }

        [Required(ErrorMessage = "La categoría arancelaria es requerida.")]
        [Display(Name = "Categoría arancelaria")]
        public int TariffCategoryId { get; set; }

        [Required(ErrorMessage = "El estado es requerido.")]
        [Display(Name = "Estado")]
        public bool IsActive { get; set; } = true;




        

    }
}
