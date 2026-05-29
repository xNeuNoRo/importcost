using ImportCostPro.Persistence.Common;

namespace ImportCostPro.Persistence.Entities
{
    public class TaxConfiguration : BaseEntity
    {
        /// <summary>
        /// Tasa general del ITBIS que se aplicará a los productos importados.
        /// </summary>
        public decimal GeneralItbisRate { get; private set; }

        /// <summary>
        /// Tasa del servicio aduanero que se aplicará a los productos importados.
        /// </summary>
        public decimal CustomsServiceRate { get; private set; }

        protected TaxConfiguration() { }

        private TaxConfiguration(decimal generalItbisRate, decimal customsServiceRate)
        {
            GeneralItbisRate = generalItbisRate;
            CustomsServiceRate = customsServiceRate;
        }

        public static TaxConfiguration Create(decimal generalItbisRate, decimal customsServiceRate)
        {
            return new TaxConfiguration(generalItbisRate, customsServiceRate);
        }

        public void UpdateRates(decimal newItbisRate, decimal newCustomsRate)
        {
            GeneralItbisRate = newItbisRate;
            CustomsServiceRate = newCustomsRate;
        }
    }
}
