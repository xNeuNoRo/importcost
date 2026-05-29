using FluentValidation;
using ImportCostPro.Application.DTOs.TariffCategory.Requests;

namespace ImportCostPro.Application.Validators.TariffCategory
{
    public class CreateTariffCategoryRequestValidator : AbstractValidator<CreateTariffCategoryRequest>
    {
        public CreateTariffCategoryRequestValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código arancelario es requerido.")
                .MaximumLength(20).WithMessage("El código arancelario debe tener un máximo de 20 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("El nombre o descripción es requerido.")
                .MaximumLength(150).WithMessage("El nombre o descripción debe tener un máximo de 150 caracteres.");

            RuleFor(x => x.CustomsDutyRate)
                .InclusiveBetween(0, 100).WithMessage("El porcentaje de arancel debe estar entre 0 y 100.");

            RuleFor(x => x.ExciseTaxRate)
                .Must((request, rate) =>
                {
                    if (request.AppliesExciseTax)
                        return rate > 0 && rate <= 100;
                    return true;
                }).WithMessage("Si aplica impuesto selectivo, el porcentaje debe ser mayor que 0 y no mayor que 100.")
                .Must((request, rate) =>
                {
                    if (!request.AppliesExciseTax)
                        return rate == 0;
                    return true;
                }).WithMessage("Si no aplica impuesto selectivo, el porcentaje de impuesto selectivo debe ser 0.");
        }
    }
}
