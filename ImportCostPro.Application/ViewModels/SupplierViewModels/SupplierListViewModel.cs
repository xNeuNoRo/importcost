using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.SupplierViewModels
{
    public class SupplierListViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        public string Name { get; set; } = null!;

        [Display(Name = "País")]
        public string CountryName { get; set; } = null!;

        [Display(Name = "Moneda")]
        public string CurrencyIsoCode { get; set; } = null!;

        [Display(Name = "Correo")]
        public string? Email { get; set; }

        [Display(Name = "Teléfono")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Estado")]
        public bool IsActive { get; set; }
    }
}