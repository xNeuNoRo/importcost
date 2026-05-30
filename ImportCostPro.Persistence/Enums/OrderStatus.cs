using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Persistence.Enums
{
    public enum OrderStatus
    {
        [Display(Name = "Abierta")]
        Open = 1,

        [Display(Name = "Liquidada")]
        Calculated = 2,

        [Display(Name = "Cerrada")]
        Closed = 3,

        [Display(Name = "Anulada")]
        Canceled = 4,
    }
}
