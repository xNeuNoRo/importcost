using FluentValidation;
using ImportCostPro.Application.DTOs.ImportOrder.Requests;

namespace ImportCostPro.Application.Validators.ImportOrder
{
    public class UpdateImportOrderRequestValidator : AbstractValidator<UpdateImportOrderRequest>
    {
        public UpdateImportOrderRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID de la orden no es válido.");

            RuleFor(x => x.OrderNumber)
                .NotEmpty().WithMessage("El número de orden es requerido.")
                .MaximumLength(30).WithMessage("El número de orden debe tener un máximo de 30 caracteres.");

            RuleFor(x => x.ImporterId)
                .GreaterThan(0).WithMessage("El importador seleccionado no es válido.");

            RuleFor(x => x.SupplierId)
                .GreaterThan(0).WithMessage("El proveedor seleccionado no es válido.");

            RuleFor(x => x.OriginCountryId)
                .GreaterThan(0).WithMessage("El país de origen seleccionado no es válido.");

            RuleFor(x => x.CurrencyId)
                .GreaterThan(0).WithMessage("La moneda seleccionada no es válida.");

            RuleFor(x => x.TransportMode)
                .IsInEnum().WithMessage("La modalidad de transporte seleccionada no es válida.");
        }
    }
}
