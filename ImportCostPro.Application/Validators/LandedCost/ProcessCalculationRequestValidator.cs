using FluentValidation;
using ImportCostPro.Application.DTOs.LandedCost.Requests;

namespace ImportCostPro.Application.Validators.LandedCost
{
    public class ProcessCalculationRequestValidator : AbstractValidator<ProcessCalculationRequest>
    {
        public ProcessCalculationRequestValidator()
        {
            RuleFor(x => x.ImportOrderId)
                .GreaterThan(0).WithMessage("El ID de la orden de importación no es válido.");
        }
    }
}
