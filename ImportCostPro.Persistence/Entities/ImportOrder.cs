using ImportCostPro.Persistence.Common;
using ImportCostPro.Persistence.Enums;

namespace ImportCostPro.Persistence.Entities
{
    public class ImportOrder : BaseEntity
    {
        public string OrderNumber { get; private set; } = null!;
        public DateTime OrderDate { get; private set; }
        public TransportMode TransportMode { get; private set; }
        public OrderStatus Status { get; private set; }

        public int ImporterId { get; private set; }
        public int SupplierId { get; private set; }
        public int OriginCountryId { get; private set; }
        public int CurrencyId { get; private set; }

        // => Navigation Properties
        public Importer Importer { get; private set; } = null!;
        public Supplier Supplier { get; private set; } = null!;
        public Country OriginCountry { get; private set; } = null!;
        public Currency Currency { get; private set; } = null!;

        public ICollection<OrderProduct> OrderProducts { get; private set; } =
            new List<OrderProduct>();
        public ICollection<ImportExpense> Expenses { get; private set; } =
            new List<ImportExpense>();
        public ICollection<CalculationResult> CalculationResults { get; private set; } =
            new List<CalculationResult>();

        public ImportOrder() { }

        private ImportOrder(
            string orderNumber,
            DateTime orderDate,
            TransportMode transportMode,
            int importerId,
            int supplierId,
            int originCountryId,
            int currencyId
        )
        {
            OrderNumber = orderNumber;
            OrderDate = orderDate.Date;
            TransportMode = transportMode;
            ImporterId = importerId;
            SupplierId = supplierId;
            OriginCountryId = originCountryId;
            CurrencyId = currencyId;
            Status = OrderStatus.Open;
        }

        public static ImportOrder Create(
            string orderNumber,
            DateTime orderDate,
            TransportMode transportMode,
            int importerId,
            int supplierId,
            int originCountryId,
            int currencyId
        )
        {
            return new ImportOrder(
                orderNumber,
                orderDate,
                transportMode,
                importerId,
                supplierId,
                originCountryId,
                currencyId
            );
        }

        public void UpdateDetails(
            string orderNumber,
            DateTime orderDate,
            TransportMode transportMode,
            int importerId,
            int supplierId,
            int originCountryId,
            int currencyId
        )
        {
            OrderNumber = orderNumber;
            OrderDate = orderDate.Date;
            TransportMode = transportMode;
            ImporterId = importerId;
            SupplierId = supplierId;
            OriginCountryId = originCountryId;
            CurrencyId = currencyId;
        }

        public void ChangeStatus(OrderStatus newStatus)
        {
            Status = newStatus;
        }
    }
}
