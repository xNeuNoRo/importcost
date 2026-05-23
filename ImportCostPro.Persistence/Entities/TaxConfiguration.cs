using ImportCostPro.Persistence.Common;

namespace ImportCostPro.Persistence.Entities
{
    public class TaxConfiguration : BaseEntity
    {
        /// <summary>
        /// Tasa general del ITBIS que se aplicará a los productos importados.
        /// </summary>
        public required decimal GeneralItbisRate { get; set; }

        /// <summary>
        /// Tasa del servicio aduanero que se aplicará a los productos importados.
        /// </summary>
        public required decimal CustomsServiceRate { get; set; }
    }
}
