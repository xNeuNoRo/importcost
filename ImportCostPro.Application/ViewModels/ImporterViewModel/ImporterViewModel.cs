using System.ComponentModel.DataAnnotations;

namespace ImportCostPro.Application.ViewModels.Importer
{
    public class ImporterViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Razon Social")]
        public string LegalName { get; set; } = null!;

        [Display(Name = "RNC")]
        public string TaxId { get; set; } = null!;

        [Display(Name = "Pais")]
        public string CountryId { get; set; } = null!;

        [Display(Name = "Cód. ISO País")]
        public string CountryIsoCode { get; set; } = null!;

        [Display(Name = "Teléfono")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Correo Electrónico")]
        public string? Email { get; set; }

        [Display(Name = "Dirección")]
        public string? Address { get; set; }

        [Display(Name = "Estado")]
        public bool IsActive { get; set; }
    }
}

