using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.ExchageRateViewModels
{
    public class ExchangeRateViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Moneda Origen")]
        public string FromCurrencyName { get; set; } = null!;

        [Display(Name = "ISO Origen")]
        public string FromCurrencyIsoCode { get; set; } = null!;

        [Display(Name = "Moneda Destino")]
        public string ToCurrencyName { get; set; } = null!;

        [Display(Name = "ISO Destino")]
        public string ToCurrencyIsoCode { get; set; } = null!;

        [Display(Name = "Valor de la Tasa")]
        public decimal RateValue { get; set; }

        [Display(Name = "Fecha de Vigencia")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime EffectiveDate { get; set; }

        [Display(Name = "Estado")]
        public bool IsActive { get; set; }
    }
}
