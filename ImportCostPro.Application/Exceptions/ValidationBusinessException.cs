namespace ImportCostPro.Application.Exceptions
{
    /// <summary>
    /// Excepción específica para fallos de reglas de negocio asociados a un campo o propiedad del formulario.
    /// </summary>
    public class ValidationBusinessException : BusinessException
    {
        public string PropertyName { get; }

        public ValidationBusinessException(string propertyName, string message)
            : base(message)
        {
            PropertyName = propertyName;
        }
    }
}
