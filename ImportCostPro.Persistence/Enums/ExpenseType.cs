using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Persistence.Enums
{
    public enum ExpenseType
    {
        [Display(Name = "Flete Internacional")]
        InternationalFreight = 1,

        [Display(Name = "Seguro Internacional")]
        InternationalInsurance = 2,

        [Display(Name = "Gastos Portuarios")]
        PortCharges = 3,

        [Display(Name = "Transporte Local")]
        LocalTransport = 4,

        [Display(Name = "Honorarios Aduanales")]
        CustomsFees = 5,

        [Display(Name = "Almacenaje y Estadía")]
        Storage = 6,

        [Display(Name = "Manejo de Carga")]
        CargoHandling = 7,

        [Display(Name = "Otros Gastos Logísticos")]
        OtherExpenses = 8,
    }
}
