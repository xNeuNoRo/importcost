using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.WebApp.Models.Currency
{
    public class CurrencyEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la moneda es requerido.")]
        [Display(Name = "Nombre de la moneda")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "El código ISO es requerido.")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "El código ISO debe tener exactamente 3 caracteres.")]
        [Display(Name = "Código ISO")]
        public string IsoCode { get; set; } = null!;

        [Required(ErrorMessage = "El símbolo es requerido.")]
        [Display(Name = "Símbolo")]
        public string Symbol { get; set; } = null!;

        [Required(ErrorMessage = "El campo Es moneda local es requerido.")]
        [Display(Name = "Es moneda local")]
        public bool IsLocalCurrency { get; set; }

        [Required(ErrorMessage = "El estado es requerido.")]
        [Display(Name = "Estado")]
        public bool IsActive { get; set; }
    }
}