using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Persistence.Enums
{
    public enum TransportMode
    {
        [Display(Name = "Marítimo")]
        Maritime = 1,

        [Display(Name = "Aéreo")]
        Air = 2,

        [Display(Name = "Terrestre")]
        Land = 3,
    }
}
