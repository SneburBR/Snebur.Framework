namespace Snebur.Dominio;

public interface IContextoSessaoUsuario
{
    public bool IsSessaoAtiva { get; }
    public IUsuario? Usuario { get; }
    public ISessaoUsuario? SessaoUsuario { get; }
}