using ImportCostPro.Persistence.Entities;

namespace ImportCostPro.Persistence.Interfaces.Repositories
{
    public interface ITaxConfigurationRepository
    {
        /// <summary>
        /// Obtiene el registro unico de configuracion de impuestos en el sistema.
        /// </summary>
        Task<TaxConfiguration?> GetSingleConfigurationAsync();

        /// <summary>
        /// Actualiza la configuracion de impuestos existente.
        /// </summary>
        /// <param name="config">La nueva configuracion de impuestos</param>
        Task UpdateAsync(TaxConfiguration config);

        /// <summary>
        /// Agrega una nueva configuración de impuestos al sistema.
        /// SOLO PARA EL SEED INICIAL, NO DEBERÍA HABER MÁS DE UN REGISTRO EN LA TABLA DE CONFIGURACIÓN DE IMPUESTOS.
        /// </summary>
        /// <param name="config"></param>
        Task AddAsync(TaxConfiguration config);
    }
}
