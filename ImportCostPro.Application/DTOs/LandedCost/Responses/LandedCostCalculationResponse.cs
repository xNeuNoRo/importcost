namespace ImportCostPro.Application.DTOs.LandedCost.Responses
{
    public class LandedCostCalculationResponse
    {
        public int Id { get; set; }
        public int ImportOrderId { get; set; }
        public string OrderNumber { get; set; } = null!;
        public int LocalCurrencyUsedId { get; set; }
        public string LocalCurrencyIsoCode { get; set; } = null!;
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

        public ICollection<CalculationResultDetailResponse> Details { get; set; } =
            new List<CalculationResultDetailResponse>();
    }
}
