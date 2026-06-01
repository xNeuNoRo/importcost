using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.TaxConfigurationViewModels
{
    public class SaveTaxConfigurationViewModel
    {
        [Required(ErrorMessage = "El porcentaje general de ITBIS es requerido.")]
        [Range(0, 100, ErrorMessage = "El porcentaje general de ITBIS debe estar entre 0 y 100.")]
        [Display(Name = "Porcentaje general de ITBIS")]
        public decimal ItbisPercentage { get; set; }

        [Required(ErrorMessage = "El porcentaje de tasa de servicio aduanal es requerido.")]
        [Range(0, 100, ErrorMessage = "El porcentaje de tasa de servicio aduanal debe estar entre 0 y 100.")]
        [Display(Name = "Porcentaje de tasa de servicio aduanal")]
        public decimal CustomsServiceRatePercentage { get; set; }
    }
}
