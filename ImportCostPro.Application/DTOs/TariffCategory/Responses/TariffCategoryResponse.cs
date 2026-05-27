namespace ImportCostPro.Application.DTOs.TariffCategory.Responses
{
    public class TariffCategoryResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal CustomsDutyRate { get; set; }
        public bool IsActive { get; set; }
    }
}
