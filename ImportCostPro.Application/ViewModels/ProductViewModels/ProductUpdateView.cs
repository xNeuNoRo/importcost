using System.ComponentModel.DataAnnotations;
using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.WebApp.Models.Currency
{
    public class ProductUpdateViewModel
    {        
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es requerido.")]
        [StringLength(150, ErrorMessage = "El nombre del producto no puede exceder los 150 caracteres")]
        [Display(Name = "Nombre del producto")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "El Codigo de referencia producto es requerido.")]
        [StringLength(150, ErrorMessage = "El Codigo de referencia no puede exceder los 50 caracteres")]
        [Display(Name = "Codigo de Referencia")]

        public string ReferenceCode { get; set; } = null!;

        [StringLength(250, ErrorMessage = "La Descripcion del producto no puede exceder los 250 caracteres")]
        [Display(Name = "Descripción del producto")]

        public string? Description { get; set; } 

        
        [Required(ErrorMessage = "El Peso del producto es requerido.")]
        [Display(Name = "Peso del producto")]

        public decimal UnitWeight { get; set; } 

        [Display(Name = "Largo del Producto")]
        public decimal? Length { get; set; } 

        [Display(Name = "Largo del Producto")]
        public decimal? Widht { get; set; }

        [Display(Name = "Alto del producto")]
        public decimal? Height { get; set; }

        [Required(ErrorMessage = "La unidad de medida es requerida.")]
        [Display(Name = "Unidad de medidad ")]

        public  UnitOfMeasure UnitOfMeasure { get; set; }

        [Required(ErrorMessage = "El pais de origen por defecto es requerido.")]
        [Display(Name = "Pais de Origen ")]

        public int DefaultOriginCountryId { get; set; }

        [Required(ErrorMessage = "La categoria arancelaria es requerida.")]
        [Display(Name = "Unidad de medidad ")]

        public int TariffCategoryId { get; set; }

        [Required(ErrorMessage = "El estado es requerido.")]
        [Display(Name = "Estado")]

        public bool IsActive { get; set; }




        









        
        

        





    
    }
}
