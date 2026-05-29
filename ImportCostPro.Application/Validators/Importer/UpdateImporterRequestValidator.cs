using FluentValidation;
using ImportCostPro.Application.DTOs.Importer.Requests;

namespace ImportCostPro.Application.Validators.Importer
{
    public class UpdateImporterRequestValidator : AbstractValidator<UpdateImporterRequest>
    {
        public UpdateImporterRequestValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID del importador no es válido.");

            RuleFor(x => x.LegalName)
                .NotEmpty().WithMessage("El nombre o razón social es requerido.")
                .MaximumLength(150).WithMessage("El nombre o razón social debe tener un máximo de 150 caracteres.");

            RuleFor(x => x.TaxId)
                .NotEmpty().WithMessage("El RNC o identificación fiscal es requerido.")
                .MaximumLength(20).WithMessage("El RNC o identificación fiscal debe tener un máximo de 20 caracteres.");

            RuleFor(x => x.CountryId)
                .GreaterThan(0).WithMessage("El país seleccionado no es válido.");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("El teléfono debe tener un máximo de 20 caracteres.");

            RuleFor(x => x.Email)
                .MaximumLength(100).WithMessage("El correo electrónico debe tener un máximo de 100 caracteres.")
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
                .WithMessage("El correo electrónico debe tener un formato válido.");

            RuleFor(x => x.Address)
                .MaximumLength(250).WithMessage("La dirección debe tener un máximo de 250 caracteres.");
        }
    }
}
