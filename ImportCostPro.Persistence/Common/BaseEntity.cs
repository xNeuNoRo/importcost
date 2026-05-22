namespace ImportCostPro.Persistence.Common
{
    public class BaseEntity<TKey>
    {
        public required TKey ID { get; set; }
        public required string Name { get; set; }
        public required bool IsActive { get; set; }
    }
}