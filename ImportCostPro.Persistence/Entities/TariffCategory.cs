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

        /// <summary>
        /// Indica si la categoría arancelaria está sujeta al pago de ITBIS (Generalmente 18% en RD)
        /// </summary>
        public bool AppliesItbis { get; private set; }

        /// <summary>
        /// Indica si la categoría arancelaria está sujeta al pago del Impuesto Selectivo al Consumo (ISC)
        /// </summary>
        public bool AppliesExciseTax { get; private set; }

        public TariffCategory() { }

        private TariffCategory(
            string code,
            string description,
            decimal customsDutyRate,
            decimal exciseTaxRate,
            bool appliesItbis,
            bool appliesExciseTax
        )
        {
            Code = code.Trim().ToUpperInvariant();
            Description = description.Trim();
            CustomsDutyRate = customsDutyRate;
            ExciseTaxRate = exciseTaxRate;
            AppliesItbis = appliesItbis;
            AppliesExciseTax = appliesExciseTax;
            IsActive = true;
        }

        public static TariffCategory Create(
            string code,
            string description,
            decimal customsDutyRate,
            decimal exciseTaxRate,
            bool appliesItbis,
            bool appliesExciseTax
        )
        {
            return new TariffCategory(
                code,
                description,
                customsDutyRate,
                exciseTaxRate,
                appliesItbis,
                appliesExciseTax
            );
        }

        public void UpdateDetails(
            string code,
            string description,
            decimal customsDutyRate,
            decimal exciseTaxRate,
            bool appliesItbis,
            bool appliesExciseTax
        )
        {
            Code = code.Trim().ToUpperInvariant();
            Description = description.Trim();
            CustomsDutyRate = customsDutyRate;
            ExciseTaxRate = exciseTaxRate;
            AppliesItbis = appliesItbis;
            AppliesExciseTax = appliesExciseTax;
        }
    }
}
