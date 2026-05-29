namespace ImportCostPro.Application.DTOs.Country.Responses
{
    /// <summary>
    /// Contrato unificado de salida para las consultas del módulo de países.
    /// </summary>
    public class CountryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string IsoCode { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}