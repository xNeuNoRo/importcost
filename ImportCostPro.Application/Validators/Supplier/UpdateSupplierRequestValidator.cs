using FluentValidation;
using ImportCostPro.Application.DTOs.Supplier.Requests;

namespace ImportCostPro.Application.Validators.Supplier
{
    public class UpdateSupplierRequestValidator : AbstractValidator<UpdateSupplierRequest>
    {
        public UpdateSupplierRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID del proveedor no es válido.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del proveedor es requerido.")
                .MaximumLength(150).WithMessage("El nombre del proveedor debe tener un máximo de 150 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("El correo electrónico del proveedor es requerido.")
                .MaximumLength(100).WithMessage("El correo electrónico debe tener un máximo de 100 caracteres.")
                .EmailAddress().WithMessage("El correo electrónico debe tener un formato válido.");

            RuleFor(x => x.OriginCountryId)
                .GreaterThan(0).WithMessage("El país de origen seleccionado no es válido.");

            RuleFor(x => x.DefaultCurrencyId)
                .GreaterThan(0).WithMessage("La moneda predeterminada seleccionada no es válida.");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("El teléfono debe tener un máximo de 20 caracteres.");
        }
    }
}
