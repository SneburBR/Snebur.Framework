namespace Snebur.Dominio;

public class EntityValidationEventArgs : EventArgs
{
    public IReadOnlyList<ErroValidacaoInfo> ValidationErrors { get; }

    // If true, indicates that the consumer handled the errors
    public bool Handled { get; set; }

    public object? DataContext { get; }

    public EntityValidationEventArgs(
        Entidade entidade,
        IReadOnlyList<ErroValidacaoInfo> validationErrors,
        object? dataContext)
    {
        ValidationErrors = validationErrors;
        this.DataContext = dataContext;
    }
}