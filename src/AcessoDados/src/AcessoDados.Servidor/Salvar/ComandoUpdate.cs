using Snebur.AcessoDados.Estrutura;
using Snebur.Linq;
using Snebur.Servicos;
using System.Diagnostics;

namespace Snebur.AcessoDados.Servidor.Salvar;

internal class ComandoUpdate : Comando, IComandoUpdate
{
    internal override bool IsAdiconarParametrosChavePrimaria => true;
    public Dictionary<string, PropriedadeAlterada> PropriedadesAlterada { get; private set; }

    internal bool ExisteAtualizacao { get => this.EstruturasCampoParametro.Count > 0; }

    internal ComandoUpdate(EntidadeAlterada entidadeAlterada,
                           EstruturaEntidade estruturaEntidade) : base(entidadeAlterada, estruturaEntidade)
    {

        this.PropriedadesAlterada = entidadeAlterada.RetornarPropriedadesAlteradas();

        this.EstruturasCampoParametro.AddRange(this.RetornarEstrutasCamposAlterados());

        if (this.EstruturaEntidade.IsSomenteLeitura)
        {
            if (!this.EstruturasCampoParametro.All(x => x.IsAutorizarAlteracaoPropriedade))
            {
                throw new ErroSeguranca("Não é autorizado atualizar uma entidade somente leitura, somente os campos autorizados",
                                        EnumTipoLogSeguranca.AlterarandoEntidadeSomenteLeitura);
            }
        }
    }

    private List<EstruturaCampo> RetornarEstrutasCamposAlterados()
    {
        if (this.Entidade.Id == 0)
        {
            return this.EstruturaEntidade.EstruturasCampos.Values.ToList();
        }

        var estruturasCamposAlterados = this.EstruturaEntidade.EstruturasCampos.
                                                        Where(x => this.PropriedadesAlterada.Keys.Contains(x.Key)).
                                                        Select(x => x.Value).ToList();

        var estruturasCamposSomenteLeitura = estruturasCamposAlterados.Where(x => x.OpcoesSomenteLeitura.IsSomenteLeitura).ToList();

        if (estruturasCamposSomenteLeitura.Count > 0)
        {
            foreach (var estruturaCampoSomenteLeitura in estruturasCamposSomenteLeitura)
            {
                if (!this.EntidadeAlterada.Contexto.IsPodeSobreEscrever(estruturaCampoSomenteLeitura))
                {
                    estruturasCamposAlterados.Remove(estruturaCampoSomenteLeitura);
                   
                    if (estruturaCampoSomenteLeitura.OpcoesSomenteLeitura.IsNotificarSeguranca)
                    {
                        var mensagem = $"Não é autorizado alterar valores das propriedades somente leitura" +
                                   $" '{estruturaCampoSomenteLeitura.Propriedade.Name}' na entidade '{estruturaCampoSomenteLeitura.EstruturaEntidade.TipoEntidade.Name}'";

                        Debugger.Break();
                        Trace.TraceWarning(mensagem);
                        LogUtil.ErroAsync(new ErroSeguranca(mensagem, EnumTipoLogSeguranca.AlterarandoPropriedadeSomenteLeitura));
                    }
                }
            }
        }
        return estruturasCamposAlterados;
    }

    protected override string RetornarSqlComando()
    {
        if (this.EstruturasCampoParametro.Count > 0)
        {
            var camposAtualizar = this.EstruturasCampoParametro.Select(x => $" {x.NomeCampoSensivel} = {x.NomeParametroOuValorFuncaoServidor} ").ToList();
            var estrutraChavePrimaria = this.EstruturaEntidade.EstruturaCampoChavePrimaria;
            var sb = new StringBuilderSql();
            sb.Append($" UPDATE [{this.EstruturaEntidade.Schema}].[{this.EstruturaEntidade.NomeTabela}]  ");
            sb.Append($" SET {String.Join(",", camposAtualizar)}  ");
            sb.Append($" WHERE {estrutraChavePrimaria.NomeCampoSensivel} = {estrutraChavePrimaria.NomeParametro}");
            return sb.ToString();
        }
        return String.Empty;
    }

    //this.EstruturasCampoParametro.AddRange(this.EstruturaEntidade.EstruturasCampos.Values);
}