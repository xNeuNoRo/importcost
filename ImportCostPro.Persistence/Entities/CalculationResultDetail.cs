using ImportCostPro.Persistence.Common;

namespace ImportCostPro.Persistence.Entities
{
    public class CalculationResultDetail : BaseEntity
    {
        public int CalculationResultId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal OriginalUnitPriceFob { get; set; }
        public decimal OriginalTotalFob { get; set; }
        public decimal LocalTotalFob { get; set; }
        public decimal AllocatedFreight { get; set; }
        public decimal AllocatedInsurance { get; set; }
        public decimal LocalTotalCif { get; set; }
        public decimal CustomsDutyAmount { get; set; }
        public decimal ExciseTaxAmount { get; set; }
        public decimal CustomsServiceAmount { get; set; }
        public decimal ItbisAmount { get; set; }
        public decimal AllocatedLocalExpenses { get; set; }
        public decimal LocalTotalLandedCost { get; set; }
        public decimal UnitLandedCost { get; set; }
        public decimal ProfitMarginRate { get; set; }
        public decimal SuggestedRetailPrice { get; set; }

        // => Navigation Properties
        public CalculationResult CalculationResult { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
