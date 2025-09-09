using Snebur.AcessoDados.Ajudantes;

namespace Snebur.AcessoDados;

public partial class ConsultaEntidade<TEntidade> where TEntidade : IEntidade
{

    public int Count()
    {
        return this.RetornarValorFuncao<int>(EnumTipoFuncao.Contar, null);
    }

    public bool Any()
    {
        return this.RetornarValorFuncao<bool>(EnumTipoFuncao.Existe, null);
    }

    private T? RetornarValorFuncao<T>(EnumTipoFuncao tipoFuncaoEnum, Expression? expressao)
    {
        var estruturaConsultaEscalar = new EstruturaConsulta
        {
            IsIncluirDeletados = this.EstruturaConsulta.IsIncluirDeletados,
            IsIncluirInativos = this.EstruturaConsulta.IsIncluirInativos,
            TipoEntidadeConsulta = this.EstruturaConsulta.TipoEntidadeConsulta,
            TipoFuncaoEnum = tipoFuncaoEnum,
            NomeTipoEntidade = this.EstruturaConsulta.NomeTipoEntidade,
            TipoEntidadeAssemblyQualifiedName = this.EstruturaConsulta.TipoEntidadeAssemblyQualifiedName,
            FiltroGrupoE = this.EstruturaConsulta.FiltroGrupoE,
            RelacoesAbertaFiltro = this.EstruturaConsulta.RelacoesAbertaFiltro,
        };

        if (estruturaConsultaEscalar.Take > 0 || estruturaConsultaEscalar.Skip > 0)
        {
            throw new ErroNaoSuportado("Take ou Skip não é suportado na funções");
        }
        if (expressao != null)
        {
            var caminhoPropriedade = this.RetornarCaminhoPropriedade(expressao);
            estruturaConsultaEscalar.CaminhoPropriedadeFuncao = caminhoPropriedade;
            this.AbrirRelacaoFiltro(estruturaConsultaEscalar, caminhoPropriedade);
        }
        return this.ContextoDados.RetornarValorScalar<T>(estruturaConsultaEscalar);
    }

    private string RetornarCaminhoPropriedade(Expression expressao)
    {
        if (expressao != null)
        {
            var caminhoPropriedade = AjudanteConsultaEntidade.RetornarCaminhoPropriedade(expressao);
            if (!caminhoPropriedade.Contains("."))
            {
                // caminhoPropriedade = String.Format("{0}.{1}", this.TipoEntidadeConsulta.Name, caminhoPropriedade);
            }
            return caminhoPropriedade;
        }
        return string.Empty;
    }
    //private void AbrirRelacaoFuncao(Expression expressao, ConsultaAcessoDados consultaAcessoDados)
    //{

    //    var propriedades = ExpressaoUtil.RetornarPropriedades(expressao);
    //    var propriedadesCaminho = new List<PropertyInfo>();

    //    ConsultaAcessoDados consultaAcessoDadosAtual = consultaAcessoDados;

    //    foreach (var propriedade in propriedades)
    //    {
    //        propriedadesCaminho.Add(propriedade);

    //        var caminhoPropriedadeParcial = AjudanteConsultaEntidade.RetornarCaminhoPropriedade(propriedadesCaminho);

    //        ErroUtil.ValidarStringVazia(caminhoPropriedadeParcial, nameof(caminhoPropriedadeParcial));

    //        if (!consultaAcessoDadosAtual.RelacoesAbertaFiltro.ContainsKey(caminhoPropriedadeParcial))
    //        {

    //            if (propriedade.PropertyType.IsSubclassOf(typeof(BaseEntidade)))
    //            {

    //                var relacaoAbertaEntidade = new RelacaoAbertaEntidade();
    //                relacaoAbertaEntidade.CaminhoPropriedade = caminhoPropriedadeParcial;

    //                relacaoAbertaEntidade.NomeTipoEntidade = propriedade.PropertyType.Name;
    //                relacaoAbertaEntidade.TipoEntidadeAssemblyQualifiedName = propriedade.PropertyType.AssemblyQualifiedName; 

    //                relacaoAbertaEntidade.NomeTipoDeclarado = propriedade.DeclaringType?.Name;
    //                relacaoAbertaEntidade.TipoDeclaradoAssemblyQualifiedName = propriedade.DeclaringType?.AssemblyQualifiedName;

    //                consultaAcessoDadosAtual.RelacoesAbertaFiltro.Add(caminhoPropriedadeParcial, relacaoAbertaEntidade);

    //            }
    //            else if (AjudanteConsultaEntidade.PropriedadeRetornarListaEntidade(propriedade))
    //            {

    //                throw new ErroNaoSuportado(String.Format("Coleção não são suportado para cmainho da propriedade para um função {0} ", propriedade.Name));
    //            }
    //            else
    //            {
    //                continue;
    //            }
    //        }
    //    }
    //}
}