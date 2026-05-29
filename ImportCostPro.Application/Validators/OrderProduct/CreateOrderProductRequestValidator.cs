using FluentValidation;
using ImportCostPro.Application.DTOs.OrderProduct.Requests;

namespace ImportCostPro.Application.Validators.OrderProduct
{
    public class CreateOrderProductRequestValidator : AbstractValidator<CreateOrderProductRequest>
    {
        public CreateOrderProductRequestValidator()
        {
            RuleFor(x => x.ImportOrderId)
                .GreaterThan(0).WithMessage("La orden de importación seleccionada no es válida.");

            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("El producto seleccionado no es válido.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("La cantidad del producto debe ser mayor a 0.");

            RuleFor(x => x.UnitFobPrice)
                .GreaterThan(0).WithMessage("El precio FOB unitario del producto debe ser mayor a 0.");

            RuleFor(x => x.TargetProfitMargin)
                .GreaterThanOrEqualTo(0).WithMessage("El margen de ganancia esperado debe ser mayor o igual que 0 y menor que 100%.")
                .LessThan(100).WithMessage("El margen de ganancia esperado debe ser mayor o igual que 0 y menor que 100%.");
        }
    }
}
