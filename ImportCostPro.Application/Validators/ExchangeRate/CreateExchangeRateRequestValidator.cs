using FluentValidation;
using ImportCostPro.Application.DTOs.ExchangeRate.Requests;
using ImportCostPro.Persistence.Interfaces.Providers;

namespace ImportCostPro.Application.Validators.ExchangeRate
{
    public class CreateExchangeRateRequestValidator : AbstractValidator<CreateExchangeRateRequest>
    {
        public CreateExchangeRateRequestValidator(IDateTimeProvider dateTimeProvider)
        {
            RuleFor(x => x.FromCurrencyId)
                .GreaterThan(0).WithMessage("La moneda origen seleccionada no es válida.")
                .NotEqual(x => x.ToCurrencyId).WithMessage("La moneda origen no puede ser igual a la moneda destino.");

            RuleFor(x => x.ToCurrencyId)
                .GreaterThan(0).WithMessage("La moneda destino seleccionada no es válida.");

            RuleFor(x => x.RateValue)
                .GreaterThan(0).WithMessage("El valor de la tasa debe ser mayor que 0.");

            RuleFor(x => x.EffectiveDate)
                .Must(date => date.Date >= dateTimeProvider.UtcNow.Date)
                .WithMessage("La fecha de vigencia de la tasa de cambio no puede ser una fecha pasada.");
        }
    }
}
