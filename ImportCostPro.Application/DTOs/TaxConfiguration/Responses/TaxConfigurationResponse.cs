namespace ImportCostPro.Application.DTOs.TaxConfiguration.Responses
{
    /// <summary>
    /// Contrato de salida para visualizar la configuración global de impuestos.
    /// </summary>
    public class TaxConfigurationResponse
    {
        public int Id { get; set; }
        
        /// <summary>
        /// Porcentaje general de ITBIS.
        /// </summary>
        public decimal ItbisPercentage { get; set; }

        /// <summary>
        /// Porcentaje de la tasa de servicio aduanal.
        /// </summary>
        public decimal CustomsServiceRatePercentage { get; set; }
    }
}