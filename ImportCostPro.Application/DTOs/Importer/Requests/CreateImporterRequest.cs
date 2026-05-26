namespace ImportCostPro.Application.DTOs.Importer.Requests
{
    /// <summary>
    /// Contrato de entrada para la creación de un nuevo importador.
    /// </summary>
    public class CreateImporterRequest
    {
        public string LegalName { get; set; } = null!;
        public string TaxId { get; set; } = null!;
        public int CountryId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }
}