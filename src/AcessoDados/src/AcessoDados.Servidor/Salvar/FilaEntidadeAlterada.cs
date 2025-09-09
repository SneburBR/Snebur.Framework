namespace Snebur.AcessoDados.Servidor.Salvar;

internal partial class FilaEntidadeAlterada : IDisposable
{

    private List<EntidadeAlterada> Restantes { get; set; }
    private Dictionary<string, EntidadeAlterada> EntidadesNaFila { get; set; }
    private Queue<EntidadeAlterada> Fila { get; set; }

    private FilaEntidadeAlterada(List<EntidadeAlterada> entidadesAlterada)
    {
        this.Fila = new Queue<EntidadeAlterada>();
        this.EntidadesNaFila = new Dictionary<string, EntidadeAlterada>();
        this.Restantes = entidadesAlterada.OrderBy(x => x.TipoAlteracao).ToList();
    }

    private Queue<EntidadeAlterada> RetornarFila()
    {
        while (this.Restantes.Count > 0)
        {
            var proximo = this.RetornarProximaEntidade();
            this.Fila.Enqueue(proximo);
        }
        return this.Fila;
    }

    private EntidadeAlterada RetornarProximaEntidade()
    {
        var proximaEntidade = this.Restantes.FirstOrDefault(x =>
         {
             return (/*(x.TipoAlteracao == EnumTipoAlteracao.Deletar && x.IsImplementaIDeletado) ||*/
                      x.EntidadesRelacaoChaveEstrangeiraDepedente == null ||
                      x.EntidadesRelacaoChaveEstrangeiraDepedente.Count == 0 ||
                      x.EntidadesRelacaoChaveEstrangeiraDepedente.All(r =>

                           this.RelacaoDependente(x, r.Value) ||
                           this.EntidadesNaFila.ContainsKey(r.Value.EntidadeRelacao.__IdentificadorEntidade))

                     );

         });

        if (proximaEntidade is null)
        {
            var detalhesRestante = String.Join(Environment.NewLine, this.Restantes.Select(x =>
            {
                return $"{x.Entidade.__NomeTipoEntidade} - {x.Entidade.Id} - {x.Entidade.GetHashCode()}";
            }));

            throw new Erro(String.Format("Não foi encontrada a próxima entidade alterada da relação dependente  Restante: {0}", detalhesRestante));
        }
        this.Restantes.Remove(proximaEntidade);
        this.Restantes.Remove(proximaEntidade);

        this.EntidadesNaFila.Add(proximaEntidade.IdentificadorEntidade, proximaEntidade);
        return proximaEntidade;
    }

    private bool RelacaoDependente(EntidadeAlterada entidadeAlterada, RelacaoChaveEstrageniraDependente relacaoDepedente)
    {
        if (relacaoDepedente.EntidadeRelacao is null)
        {
            var estruturaRelacao = relacaoDepedente.EstruturaRelacaoChaveEstrangeira;
            var idChaveEstrangeira = ConverterUtil.ParaInt64(estruturaRelacao.EstruturaCampoChaveEstrangeira.Propriedade.GetValue(entidadeAlterada.Entidade));
            return idChaveEstrangeira > 0;
        }
        return false;
    }

    #region IDisposable 

    public void Dispose()
    {
        this.EntidadesNaFila?.Clear();
    }

    #endregion
}