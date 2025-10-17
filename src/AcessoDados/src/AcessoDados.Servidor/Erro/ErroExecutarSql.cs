using Snebur.AcessoDados.Mapeamento;
using Snebur.Linq;
using System.Runtime.CompilerServices;

namespace Snebur.AcessoDados;

[Serializable]
public class ErroExecutarSql : ErroAcessoDados
{
    public List<IParametroInfo>? ParametroInfos { get; }
    public string? Filtro { get; }
    public override string Message
        => base.Message + this.GetParametersDescription() + this.GetFilterDescription();

    public ErroExecutarSql(
        string mensagem ,
        IEnumerable<IParametroInfo>? parametroInfos,
        string? filtro,
        Exception? erroInterno = null,
        [CallerMemberName] string nomeMetodo = "",
        [CallerFilePath] string caminhoArquivo = "",
        [CallerLineNumber] int linhaDoErro = 0) :
        base(mensagem, erroInterno, nomeMetodo, caminhoArquivo, linhaDoErro)
    {
        this.ParametroInfos = parametroInfos?.ToList();
        this.Filtro = filtro;
    }
     
    private string GetParametersDescription()
    {
        if (this.ParametroInfos is null || 
            this.ParametroInfos.Count == 0)
        {
            return string.Empty;
        }
        return $" Parâmetros: {string.Join(", ", this.ParametroInfos)}";
    }

    private string GetFilterDescription()
    {
        if (string.IsNullOrWhiteSpace(this.Filtro))
        {
            return string.Empty;
        }
        return $" Filtro: {this.Filtro}";
    }
}