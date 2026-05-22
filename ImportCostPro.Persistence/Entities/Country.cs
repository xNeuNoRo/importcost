
using ImportCostPro.Persistence.Common;

namespace ImportCostPro.Persistence.Entities
{
    public class Country : BaseEntity<int>
    {
        public required string IsoCode { get; set; }
    }
}