using FluentValidation;
using ImportCostPro.Application.DTOs.ImportExpense.Requests;

namespace ImportCostPro.Application.Validators.ImportExpense
{
    public class CreateImportExpenseRequestValidator : AbstractValidator<CreateImportExpenseRequest>
    {
        public CreateImportExpenseRequestValidator()
        {
            RuleFor(x => x.ImportOrderId)
                .GreaterThan(0).WithMessage("La orden de importación seleccionada no es válida.");

            RuleFor(x => x.CurrencyId)
                .GreaterThan(0).WithMessage("La moneda seleccionada no es válida.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción del gasto de importación es obligatoria.")
                .MaximumLength(250).WithMessage("La descripción del gasto no debe exceder los 250 caracteres.");

            RuleFor(x => x.ExpenseType)
                .IsInEnum().WithMessage("El tipo de gasto seleccionado no es válido.");

            RuleFor(x => x.DistributionBase)
                .IsInEnum().WithMessage("La base de distribución seleccionada no es válida.");

            RuleFor(x => x.OriginalAmount)
                .GreaterThan(0).WithMessage("El monto original del gasto logístico debe ser mayor a 0.");
                
            RuleFor(x => x.ExpenseDate)
                .NotEmpty().WithMessage("La fecha del gasto es obligatoria.");
        }
    }
}
