using System;

namespace Snebur.AcessoDados.Dominio
{
    public static class BaseRelacaoAbertaExtensao
    {
        public static Type RetornarTipoEntidade(this BaseRelacaoAberta baseRelacaoAberta)
        {
            return TipoEntidadeUtil.RetornarTipoEntidade(baseRelacaoAberta.TipoEntidadeAssemblyQualifiedName);

            //var nomeNamespace = baseRelacaoAberta.NamespaceTipoServidor;
            //var nomeTipoEntidade = baseRelacaoAberta.NomeTipoEntidade;
            //var chave = String.Format("{0}.{1}", nomeNamespace, nomeTipoEntidade);

            //Type tipoEntidade = null;

            //if (!Repositorio.TiposEntidade.TryGetValue(chave, out tipoEntidade))
            //{
            //    throw new Erro(String.Format("Não foi encontrado o {0} no assemblys ", chave));
            //}
            //return tipoEntidade;
        }

        public static Type RetornarPropriedade(this BaseRelacaoAberta baseRelacaoAberta)
        {
            return TipoEntidadeUtil.RetornarTipoEntidade(baseRelacaoAberta.TipoEntidadeAssemblyQualifiedName);
            //var nomeNamespace = baseRelacaoAberta.NamespaceTipoServidor;
            //var nomeTipoEntidade = baseRelacaoAberta.NomeTipoEntidade;
            //var chave = String.Format("{0}.{1}", nomeNamespace, nomeTipoEntidade);

            //Type tipoEntidade = null;

            //if (!Repositorio.TiposEntidade.TryGetValue(chave, out tipoEntidade))
            //{
            //    throw new Erro(String.Format("Não foi encontrado o {0} no assemblys ", chave));
            //}
            //return tipoEntidade;
        }
    }
}
