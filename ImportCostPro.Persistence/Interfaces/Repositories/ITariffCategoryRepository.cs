using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Persistence.Interfaces.Repositories
{
    public interface ITariffCategoryRepository : IGenericRepository<TariffCategory>
    {
        /// <summary>
        /// Obtiene una categoría arancelaria específica buscando por su código oficial de aduanas.
        /// </summary>
        Task<TariffCategory?> GetByCodeAsync(string code);

        /// <summary>
        /// Verifica si ya existe una partida arancelaria con el mismo código,
        /// permitiendo excluir el ID actual durante una edición.
        /// </summary>
        Task<bool> ExistsByCodeAsync(string code, int? excludeId = null);

        /// <summary>
        /// Verifica si la categoría arancelaria está siendo referenciada por algún producto en el catálogo.
        /// </summary>
        Task<bool> IsTariffCategoryReferencedAsync(int tariffCategoryId);
    }
}
