using FluentValidation;
using ImportCostPro.Application.DTOs.TaxConfiguration.Requests;

namespace ImportCostPro.Application.Validators.TaxConfiguration
{
    public class SaveTaxConfigurationRequestValidator : AbstractValidator<SaveTaxConfigurationRequest>
    {
        public SaveTaxConfigurationRequestValidator()
        {
            RuleFor(x => x.ItbisPercentage)
                .InclusiveBetween(0, 100).WithMessage("El porcentaje de ITBIS debe estar entre 0 y 100.");

            RuleFor(x => x.CustomsServiceRatePercentage)
                .InclusiveBetween(0, 100).WithMessage("El porcentaje de servicio aduanal debe estar entre 0 y 100.");
        }
    }
}
