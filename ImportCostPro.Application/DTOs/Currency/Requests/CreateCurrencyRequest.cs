namespace ImportCostPro.Application.DTOs.Currency.Requests
{
    /// <summary>
    /// Contrato de entrada plano para el registro de una nueva divisa en el ERP.
    /// </summary>
    public class CreateCurrencyRequest
    {
        public string Name { get; set; } = null!;
        public string IsoCode { get; set; } = null!; // Ej: "USD", "DOP"
        public string Symbol { get; set; } = null!; // Ej: "$", "RD$"
        public bool IsLocalCurrency { get; set; }
    }
}
