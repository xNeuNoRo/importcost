using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Persistence.Enums
{
    public enum UnitOfMeasure
    {
        [Display(Name = "Unidad")]
        Unit = 1,

        [Display(Name = "Caja")]
        Box = 2,

        [Display(Name = "Paquete")]
        Pack = 3,

        [Display(Name = "Docena")]
        Dozen = 4,

        [Display(Name = "Galón")]
        Gallon = 5,

        [Display(Name = "Metro")]
        Meter = 6,
    }
}
