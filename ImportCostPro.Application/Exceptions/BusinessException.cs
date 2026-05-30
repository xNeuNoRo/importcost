namespace ImportCostPro.Application.Exceptions
{
    /// <summary>
    /// Excepción base para todos los errores lógicos y de reglas de negocio.
    /// </summary>
    public class BusinessException : Exception
    {
        public BusinessException(string message)
            : base(message) { }
    }
}
