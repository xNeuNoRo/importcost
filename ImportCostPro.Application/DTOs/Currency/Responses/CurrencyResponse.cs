namespace ImportCostPro.Application.DTOs.Currency.Responses
{
    /// <summary>
    /// Contrato unificado de salida plano para la visualización de divisas.
    /// </summary>
    public class CurrencyResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string IsoCode { get; set; } = null!;
        public string Symbol { get; set; } = null!;
        public bool IsLocalCurrency { get; set; }
        public bool IsActive { get; set; }
    }
}
