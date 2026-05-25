using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Application.DTOs.Product.Responses
{
    /// <summary>
    /// Contrato unificado de salida para las consultas detalladas del catálogo de productos.
    /// </summary>
    public class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ReferenceCode { get; set; } = null!;
        public string? Description { get; set; }
        public decimal UnitWeight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public decimal VolumeCubicMeters { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public bool IsActive { get; set; }
        public int DefaultOriginCountryId { get; set; }
        public string DefaultOriginCountryName { get; set; } = null!;
        public int TariffCategoryId { get; set; }
        public string TariffCategoryCode { get; set; } = null!;
        public string TariffCategoryDescription { get; set; } = null!;
        public decimal CustomsDutyRate { get; set; }
    }
}
