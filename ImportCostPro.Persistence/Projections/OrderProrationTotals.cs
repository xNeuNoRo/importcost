namespace ImportCostPro.Persistence.Projections
{
    public record OrderProrationTotalsProjection(
        decimal TotalOriginalFob,
        decimal TotalWeight,
        decimal TotalVolume,
        decimal TotalQuantity
    );
}
