using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.TaxConfig
{
    public class TaxConfigViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Tasa General del ITBIS;")]
        public decimal GeneralItbisRate { get; set; }

        [Display(Name = "Servicio Aduanero(%)")]
        public decimal CustomsServiceRate { get; set; }

        [Display(Name = "Ultima Actualizacion")]
        public DateTime? UpdatedAt { get; set; }
    }
}