namespace ImportCostPro.Application.DTOs.TaxConfiguration.Requests
{
    /// <summary>
    /// Contrato de entrada unificado para crear o actualizar la configuración global de impuestos.
    /// No requiere ID porque el sistema maneja un registro único (Singleton).
    /// </summary>
    public class SaveTaxConfigurationRequest
    {
        /// <summary>
        /// Porcentaje general de ITBIS (Ej: 18 para 18%)
        /// </summary>
        public decimal ItbisPercentage { get; set; }

        /// <summary>
        /// Porcentaje aplicado sobre el CIF para calcular la tasa de servicio aduanal (Ej: 0.4 para 0.4%)
        /// </summary>
        public decimal CustomsServiceRatePercentage { get; set; }
    }
}