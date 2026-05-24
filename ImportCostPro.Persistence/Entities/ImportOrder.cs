using ImportCostPro.Persistence.Common;
using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Persistence.Entities
{
    public class ImportOrder : BaseEntity
    {
        public required string OrderNumber { get; set; }
        public required DateTime OrderDate { get; set; }
        public required TransportMode TransportMode { get; set; }
        public required OrderStatus Status { get; set; }

        public int ImporterId { get; set; }
        public int SupplierId { get; set; }
        public int OriginCountryId { get; set; }
        public int CurrencyId { get; set; }

        // => Navigation Properties
        public Importer Importer { get; set; } = null!;
        public Supplier Supplier { get; set; } = null!;
        public Country OriginCountry { get; set; } = null!;
        public Currency Currency { get; set; } = null!;
    }
}
