namespace ImportCostPro.Application.DTOs.TariffCategory.Requests
{
    public class UpdateTariffCategoryRequest
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal CustomsDutyRate { get; set; }
    }
}
