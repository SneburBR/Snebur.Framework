namespace Snebur.Dominio;

public interface IEntityLifecycle : IEntidade
{
    void Creating();
    void Saving();
    void Saved();
    void Deleting();
    void Deleted();
}