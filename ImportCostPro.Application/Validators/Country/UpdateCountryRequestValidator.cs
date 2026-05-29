using FluentValidation;
using ImportCostPro.Application.DTOs.Country.Requests;

namespace ImportCostPro.Application.Validators.Country
{
    public class UpdateCountryRequestValidator : AbstractValidator<UpdateCountryRequest>
    {
        public UpdateCountryRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID del país no es válido.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del país es requerido.")
                .MaximumLength(150).WithMessage("El nombre del país no debe exceder los 150 caracteres.");

            RuleFor(x => x.IsoCode)
                .NotEmpty().WithMessage("El código ISO del país es requerido.")
                .Length(2, 3).WithMessage("El código ISO del país debe tener entre 2 y 3 caracteres.");
        }
    }
}
