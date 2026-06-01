using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Persistence.Enums
{
    public enum DistributionBase
    {
        [Display(Name = "Valor FOB")]
        FobValue = 1,

        [Display(Name = "Peso Bruto")]
        Weight = 2,

        [Display(Name = "Volumen")]
        Volume = 3,

        [Display(Name = "Cantidad")]
        Quantity = 4,
    }
}
