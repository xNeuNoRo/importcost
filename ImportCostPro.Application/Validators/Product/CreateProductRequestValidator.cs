using FluentValidation;
using ImportCostPro.Application.DTOs.Product.Requests;

namespace ImportCostPro.Application.Validators.Product
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del producto es requerido.")
                .MaximumLength(150).WithMessage("El nombre del producto debe tener un máximo de 150 caracteres.");

            RuleFor(x => x.ReferenceCode)
                .NotEmpty().WithMessage("El código o referencia es requerido.")
                .MaximumLength(50).WithMessage("El código o referencia debe tener un máximo de 50 caracteres.");

            RuleFor(x => x.UnitWeight)
                .GreaterThan(0).WithMessage("El peso unitario debe ser mayor que 0.");

            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("La descripción debe tener un máximo de 250 caracteres.");

            RuleFor(x => x.UnitOfMeasure)
                .IsInEnum().WithMessage("La unidad de medida seleccionada no es válida.");

            RuleFor(x => x.DefaultOriginCountryId)
                .GreaterThan(0).WithMessage("El país de origen seleccionado no es válido.");

            RuleFor(x => x.TariffCategoryId)
                .GreaterThan(0).WithMessage("La categoría arancelaria seleccionada no es válida.");

            RuleFor(x => x)
                .Must(x =>
                {
                    bool any = x.Length.HasValue || x.Width.HasValue || x.Height.HasValue;
                    if (!any) return true;
                    return (x.Length > 0 && x.Width > 0 && x.Height > 0);
                })
                .WithMessage("Si se decide colocar dimensiones, los tres campos (Largo, Ancho, Alto) deben tener valor y ser mayores a cero.");
        }
    }
}
