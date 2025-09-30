namespace Snebur.Dominio;

[IgnorarInterfaceTS]
public interface IRelacaoTipoUsuarioAdicionarGrupoUsuarioGrupoUsuario : IEntidadeSeguranca
{
    [RelacaoPai]
    ITipoUsuarioAdicionarGrupoUsuario? TipoUsuarioAdicionarGrupoUsuario { get; set; }

    [RelacaoPai]
    IGrupoUsuario? GrupoUsuario { get; set; }
}
