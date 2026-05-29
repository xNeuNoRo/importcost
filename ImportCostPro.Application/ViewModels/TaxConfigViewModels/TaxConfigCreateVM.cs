using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.TaxConfig
{
    public class TaxConfigCreateVM
    {
        [Required(ErrorMessage = "La tasa general del ITBIS es requerida.")]
        [Range(0, 100, ErrorMessage = "La tasa general del ITBIS debe estar entre 0 y 100.")]
        [Display(Name = "Tasa General del ITBIS;")]

        public decimal GeneralItbisRate { get; set; }

        [Required(ErrorMessage = "La tasa de servicio aduanero es requerida.")]
        [Range(0, 100, ErrorMessage ="La tasa debe estar entre 0 y 100")]
        [Display(Name = "Servicio Aduanero(%)")]
        public decimal CustomsServiceRate { get; set; }
    
    }
}