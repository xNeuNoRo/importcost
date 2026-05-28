using ImportCostPro.Persistence.Common;

namespace ImportCostPro.Persistence.Entities
{
    public class CalculationResult : BaseEntity
    {
        public int ImportOrderId { get; set; }
        public int LocalCurrencyUsedId { get; set; }
        public decimal ExchangeRateUsed { get; set; }
        public decimal TotalOriginalFob { get; set; }
        public decimal TotalLocalFob { get; set; }
        public decimal TotalFreight { get; set; }
        public decimal TotalInsurance { get; set; }
        public decimal TotalCif { get; set; }
        public decimal TotalTariff { get; set; }
        public decimal TotalExciseTax { get; set; }
        public decimal TotalCustomsService { get; set; }
        public decimal TotalItbis { get; set; }
        public decimal TotalLocalExpenses { get; set; }
        public decimal TotalImportCost { get; set; }
        public decimal TotalImportedQuantity { get; set; }
        public decimal HistoricalItbisRate { get; set; }
        public decimal HistoricalCustomsServiceRate { get; set; }
        public DateTime CalculationDate { get; set; }

        // => Navigation Properties
        public ImportOrder ImportOrder { get; set; } = null!;
        public Currency LocalCurrencyUsed { get; set; } = null!;
        public ICollection<CalculationResultDetail> Details { get; set; } =
            new List<CalculationResultDetail>();
    }
}
