namespace Snebur.Dominio;

[IgnorarInterfaceTS]
public interface ITipoUsuarioAdicionarGrupoUsuario : IEntidadeSeguranca, IMembrosDe
{
    string NomeTipoUsuario { get; set; }
}