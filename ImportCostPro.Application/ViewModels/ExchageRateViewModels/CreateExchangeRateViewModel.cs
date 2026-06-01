using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.ExchageRateViewModels
{
    public class CreateExchangeRateViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "La moneda origen seleccionada no es válida.")]
        [Range(1, int.MaxValue, ErrorMessage = "La moneda origen seleccionada no es válida.")]
        [Display(Name = "Moneda Origen")]
        public int FromCurrencyId { get; set; }

        [Required(ErrorMessage = "La moneda destino seleccionada no es válida.")]
        [Range(1, int.MaxValue, ErrorMessage = "La moneda destino seleccionada no es válida.")]
        [Display(Name = "Moneda Destino")]
        public int ToCurrencyId { get; set; }

        [Required(ErrorMessage = "El valor de la tasa es requerido.")]
        [Range(0.0000000001, double.MaxValue, ErrorMessage = "El valor de la tasa debe ser mayor que 0.")]
        [Display(Name = "Valor de la Tasa")]
        public decimal RateValue { get; set; }

        [Required(ErrorMessage = "La fecha de vigencia es requerida.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Vigencia")]
        public DateTime EffectiveDate { get; set; } = DateTime.Today;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FromCurrencyId == ToCurrencyId && FromCurrencyId > 0)
            {
                yield return new ValidationResult(
                    "La moneda origen no puede ser igual a la moneda destino.",
                    new[] { nameof(FromCurrencyId), nameof(ToCurrencyId) }
                );
            }
        }
    }
}
