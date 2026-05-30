namespace ImportCostPro.Application.DTOs.Importer.Requests
{
    /// <summary>
    /// Contrato de entrada para la actualización de un importador existente.
    /// </summary>
    public class UpdateImporterRequest
    {
        public int Id { get; set; }
        public string LegalName { get; set; } = null!;
        public string TaxId { get; set; } = null!;
        public int CountryId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
    }
}