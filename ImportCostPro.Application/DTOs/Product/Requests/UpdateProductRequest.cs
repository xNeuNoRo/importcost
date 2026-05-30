using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Application.DTOs.Product.Requests
{
    /// <summary>
    /// Contrato de entrada para la actualización de un producto existente.
    /// </summary>
    public class UpdateProductRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ReferenceCode { get; set; } = null!;
        public string? Description { get; set; }
        public decimal UnitWeight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public int DefaultOriginCountryId { get; set; }
        public int TariffCategoryId { get; set; }
    }
}
