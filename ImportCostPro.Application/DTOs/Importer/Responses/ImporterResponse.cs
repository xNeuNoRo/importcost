namespace ImportCostPro.Application.DTOs.Importer.Responses
{
    /// <summary>
    /// Contrato unificado de salida para las respuestas del módulo de importadores.
    /// </summary>
    public class ImporterResponse
    {
        public int Id { get; set; }
        public string LegalName { get; set; } = null!;
        public string TaxId { get; set; } = null!;
        public int CountryId { get; set; }
        public string CountryName { get; set; } = null!;
        public string CountryIsoCode { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
    }
}