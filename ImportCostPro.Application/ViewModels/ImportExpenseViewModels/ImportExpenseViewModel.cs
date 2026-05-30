using System.ComponentModel.DataAnnotations;
using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Application.ViewModels.ImportExpenseViewModels
{
    public class ImportExpenseViewModel
    {
        public int Id { get; set; }
        public int ImportOrderId { get; set; }
        public int CurrencyId { get; set; }

        [Display(Name = "Moneda")]
        public string CurrencyIsoCode { get; set; } = null!;

        [Display(Name = "Descripción")]
        public string Description { get; set; } = null!;

        [Display(Name = "Tipo de Gasto")]
        public ExpenseType ExpenseType { get; set; }

        [Display(Name = "Base de Distribución")]
        public DistributionBase DistributionBase { get; set; }

        [Display(Name = "Monto Original")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal OriginalAmount { get; set; }

        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ExpenseDate { get; set; }
    }
}
