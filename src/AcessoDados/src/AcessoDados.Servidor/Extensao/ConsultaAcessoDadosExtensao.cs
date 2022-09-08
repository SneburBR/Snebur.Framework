using System;
using System.Collections.Generic;

namespace Snebur.AcessoDados
{
    public static class ConsultaAcessoDadosExtensao
    {

        public static Type RetornarTipoEntidade(this EstruturaConsulta estruturaConsulta)
        {
            return TipoEntidadeUtil.RetornarTipoEntidade(estruturaConsulta.TipoEntidadeAssemblyQualifiedName);

            //var nomeNamespace = consultaAcessoDados.NamespaceTipoServidor;
            //var nomeTipoEntidade = consultaAcessoDados.NomeTipoEntidade;

            //var chave = String.Format("{0}.{1}", nomeNamespace, nomeTipoEntidade);
            //Type tipoEntidade = null;

            //if (!Repositorio.TiposEntidade.TryGetValue(chave, out tipoEntidade))
            //{
            //    throw new Erro(String.Format("Não foi encontrado o {0} no assemblys ", chave));
            //}
            //return tipoEntidade;
        }

        public static List<BaseRelacaoAberta> RetornarTodasRelacoesAberta(this EstruturaConsulta estruturaConsulta)
        {
            var relacoes = new List<BaseRelacaoAberta>();
            relacoes.AddRange(estruturaConsulta.RelacoesAberta.Values);
            relacoes.AddRange(estruturaConsulta.ColecoesAberta.Values);
            return relacoes;
        }
    }
}
