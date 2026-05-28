using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.CurrencyViewModels   
{
    public class CurrencyViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre de la moneda")]
        public string Name { get; set; } = null!;

        [Display(Name = "Código ISO")]
        public string IsoCode { get; set; } = null!;

        [Display(Name = "Símbolo")]
        public string Symbol { get; set; } = null!;

        [Display(Name = "Moneda local")]
        public bool IsLocalCurrency { get; set; }

        [Display(Name = "Estado")]
        public bool IsActive { get; set; }
    }
}