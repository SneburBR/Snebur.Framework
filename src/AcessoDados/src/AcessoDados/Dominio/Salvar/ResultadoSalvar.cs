namespace Snebur.AcessoDados;

public class ResultadoSalvar : Resultado
{
    #region Campos Privados

    #endregion

    public List<EntidadeSalva> EntidadesSalvas { get; set; } = new List<EntidadeSalva>();

    public List<ErroValidacao> ErrosValidacao { get; set; } = new List<ErroValidacao>();

    public ResultadoSalvar()
    {

    }
}