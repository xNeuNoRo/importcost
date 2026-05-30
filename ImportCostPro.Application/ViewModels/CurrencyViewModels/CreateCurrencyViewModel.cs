using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.CurrencyViewModels
{
    public class CreateCurrencyViewModel
    {
        [Required(ErrorMessage = "El nombre de la moneda es requerido.")]
        [MaxLength(100, ErrorMessage = "El nombre de la moneda no debe exceder los 100 caracteres.")]
        [Display(Name = "Nombre de la moneda")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "El código ISO de la moneda es requerido.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "El código ISO de la moneda debe tener exactamente 3 caracteres.")]
        [Display(Name = "Código ISO")]
        public string IsoCode { get; set; } = null!;

        [Required(ErrorMessage = "El símbolo de la moneda es requerido.")]
        [MaxLength(10, ErrorMessage = "El símbolo de la moneda no debe exceder los 10 caracteres.")]
        [Display(Name = "Símbolo")]
        public string Symbol { get; set; } = null!;

        [Display(Name = "Es moneda local")]
        public bool IsLocalCurrency { get; set; }
    }
}