using FluentValidation;
using ImportCostPro.Application.DTOs.Currency.Requests;

namespace ImportCostPro.Application.Validators.Currency
{
    public class CreateCurrencyRequestValidator : AbstractValidator<CreateCurrencyRequest>
    {
        public CreateCurrencyRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre de la moneda es requerido.")
                .MaximumLength(100).WithMessage("El nombre de la moneda no debe exceder los 100 caracteres.");

            RuleFor(x => x.IsoCode)
                .NotEmpty().WithMessage("El código ISO de la moneda es requerido.")
                .Length(3).WithMessage("El código ISO de la moneda debe tener exactamente 3 caracteres.");

            RuleFor(x => x.Symbol)
                .NotEmpty().WithMessage("El símbolo de la moneda es requerido.")
                .MaximumLength(10).WithMessage("El símbolo de la moneda no debe exceder los 10 caracteres.");
        }
    }
}
