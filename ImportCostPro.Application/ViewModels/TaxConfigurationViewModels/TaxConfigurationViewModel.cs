using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.TaxConfigurationViewModels
{
    public class TaxConfigurationViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Porcentaje de ITBIS")]
        [DisplayFormat(DataFormatString = "{0:N2}%")]
        public decimal ItbisPercentage { get; set; }

        [Display(Name = "Servicio Aduanal (%)")]
        [DisplayFormat(DataFormatString = "{0:N2}%")]
        public decimal CustomsServiceRatePercentage { get; set; }
    }
}
