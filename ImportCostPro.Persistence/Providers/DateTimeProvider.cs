using ImportCostPro.Persistence.Interfaces.Providers;

namespace ImportCostPro.Persistence.Providers
{
    /// <summary>
    /// Implementación de IDateTimeProvider que devuelve la fecha y hora actual en formato UTC.
    /// </summary>
    public class DateTimeProvider : IDateTimeProvider
    {
        /// <summary>
        /// Obtiene la fecha y hora actual en formato UTC.
        /// </summary>
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
