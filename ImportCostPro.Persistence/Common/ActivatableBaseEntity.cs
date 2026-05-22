namespace ImportCostPro.Persistence.Common
{
    public abstract class ActivatableBaseEntity : BaseEntity
    {
        public bool IsActive { get; set; } = true;
    }
}
