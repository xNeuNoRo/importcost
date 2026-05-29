using System.ComponentModel.DataAnnotations;
using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Application.ViewModels.ImportOrderViewModels
{
    public class ImportOrderViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Número de Orden")]
        public string OrderNumber { get; set; } = null!;

        [Display(Name = "Importador")]
        public string ImporterName { get; set; } = null!;

        [Display(Name = "Proveedor")]
        public string SupplierName { get; set; } = null!;

        [Display(Name = "País de Origen")]
        public string OriginCountryName { get; set; } = null!;

        [Display(Name = "Moneda")]
        public string CurrencyIsoCode { get; set; } = null!;

        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Transporte")]
        public TransportMode TransportMode { get; set; }

        [Display(Name = "Estado")]
        public OrderStatus Status { get; set; }

        [Display(Name = "Total FOB")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal FobTotal { get; set; }

        [Display(Name = "Total Landed Cost")]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal TotalLandedCost { get; set; }
    }
}
