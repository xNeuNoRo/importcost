namespace ImportCostPro.Application.DTOs.Country.Requests
{
    /// <summary>
    /// Contrato de entrada para la modificación de un país existente.
    /// </summary>
    public class UpdateCountryRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string IsoCode { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}