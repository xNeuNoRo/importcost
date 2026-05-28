namespace ImportCostPro.Application.DTOs.TariffCategory.Requests
{
    public class CreateTariffCategoryRequest
    {
        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal CustomsDutyRate { get; set; }
        public decimal ExciseTaxRate { get; set; }
    }
}
