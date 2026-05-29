using System.ComponentModel.DataAnnotations;
using ImportCostPro.Persistence.Enums;


namespace ImportCostPro.Application.ViewModels.ImportExpense
{
    public class ImportExpenseUpdateVM
    {
        public int Id { get; set; }

        [Display(Name = "ID Orden")]
        public int ImportOrderId { get; set; }

        public int CurrencyId { get; set; }

        [Display(Name = "Moneda")]
        public string CurrencyIsoCode { get; set; } = null!;

        [Display(Name = "Descripción")]
        public string Description { get; set; } = null!;

        [Display(Name = "Tipo de Gasto")]
        public ExpenseType ExpenseType { get; set; }

        [Display(Name = "Base Distribución")]
        public DistributionBase DistributionBase { get; set; }

        [Display(Name = "Monto Original")]
        public decimal OriginalAmount { get; set; }
    }



}