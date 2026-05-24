using ImportCostPro.Persistence.Common;

namespace ImportCostPro.Persistence.Entities
{
    public class TariffCategory : ActivatableBaseEntity
    {
        /// <summary>
        /// Código oficial del arancel
        /// </summary>
        public required string Code { get; set; }

        /// <summary>
        /// Descripción oficial de la mercancía
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Tasa de arancel aduanero aplicable a esta categoría, expresada como un porcentaje (por ejemplo, 5.5 para 5.5%).
        /// </summary>
        public required decimal CustomsDutyRate { get; set; }
    }
}
