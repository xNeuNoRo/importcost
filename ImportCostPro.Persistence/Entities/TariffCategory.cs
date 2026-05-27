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

        public TariffCategory() { }

        private TariffCategory(string code, string description, decimal customsDutyRate)
        {
            Code = code.Trim().ToUpperInvariant();
            Description = description.Trim();
            CustomsDutyRate = customsDutyRate;
            IsActive = true;
        }

        public static TariffCategory Create(
            string code,
            string description,
            decimal customsDutyRate
        )
        {
            return new TariffCategory(code, description, customsDutyRate);
        }

        public void UpdateDetails(string code, string description, decimal customsDutyRate)
        {
            Code = code.Trim().ToUpperInvariant();
            Description = description.Trim();
            CustomsDutyRate = customsDutyRate;
        }
    }
}
