namespace ImportCostPro.Application.DTOs.Country.Requests
{
    /// <summary>
    /// Contrato de entrada para la creación de un nuevo país.
    /// </summary>
    public class CreateCountryRequest
    {
        public string Name { get; set; } = null!;
        public string IsoCode { get; set; } = null!;
    }
}