using ImportCostPro.Persistence.Common;

namespace ImportCostPro.Persistence.Entities
{
    public class TariffCategory : ActivatableBaseEntity
    {
        /// <summary>
        /// Código oficial del arancel
        /// </summary>
        public string Code { get; private set; } = null!;

        /// <summary>
        /// Descripción oficial de la mercancía
        /// </summary>
        public string Description { get; private set; } = null!;

        /// <summary>
        /// Tasa de arancel aduanero aplicable a esta categoría, expresada como un porcentaje (por ejemplo, 5.5 para 5.5%).
        /// </summary>
        public decimal CustomsDutyRate { get; private set; }

        /// <summary>
        /// Tasa de impuesto especial aplicable a esta categoría,
        /// expresada como un porcentaje (por ejemplo, 10.0 para 10%).
        /// </summary>
        public decimal ExciseTaxRate { get; private set; }

        public TariffCategory() { }

        private TariffCategory(
            string code,
            string description,
            decimal customsDutyRate,
            decimal exciseTaxRate = 0m
        )
        {
            Code = code.Trim().ToUpperInvariant();
            Description = description.Trim();
            CustomsDutyRate = customsDutyRate;
            ExciseTaxRate = exciseTaxRate;
            IsActive = true;
        }

        public static TariffCategory Create(
            string code,
            string description,
            decimal customsDutyRate,
            decimal exciseTaxRate = 0m
        )
        {
            return new TariffCategory(code, description, customsDutyRate, exciseTaxRate);
        }

        public void UpdateDetails(
            string code,
            string description,
            decimal customsDutyRate,
            decimal exciseTaxRate
        )
        {
            Code = code.Trim().ToUpperInvariant();
            Description = description.Trim();
            CustomsDutyRate = customsDutyRate;
            ExciseTaxRate = exciseTaxRate;
        }
    }
}
