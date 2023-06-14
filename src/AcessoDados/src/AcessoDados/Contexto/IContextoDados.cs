using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using System;
using System.Collections.Generic;

namespace Snebur.AcessoDados
{
    [IgnorarInterfaceTS]
    public interface IContextoDados : IServicoDados/*, IDisposable*/
    {
        IUsuario UsuarioLogado { get; }

        T RetornarValorScalar<T>(EstruturaConsulta estruturaConsulta);

        IConsultaEntidade<TEntidade> RetornarConsulta<TEntidade>() where TEntidade : IEntidade;

        IConsultaEntidade<TEntidade> RetornarConsulta<TEntidade>(Type tipoConsulta) where TEntidade : IEntidade;

        EstruturaConsulta RetornarEstruturaConsulta<TEntidade>() where TEntidade : IEntidade;

        EstruturaConsulta RetornarEstruturaConsulta(Type tipoEntidade);

        ResultadoSalvar Salvar(IEntidade entidade);

        ResultadoDeletar Deletar(IEntidade entidade);

        ResultadoDeletar Deletar(IEntidade entidade, string relacoesEmCascata);

        ResultadoDeletar Deletar(IEnumerable<IEntidade> entidades);


        //ResultadoSalvar Salvar(params IEntidade[] entidades);

        //ResultadoSalvar Salvar(params Entidade[] entidades);

        //ResultadoSalvar Salvar(ListaEntidades<Entidade> entidades);

        //ResultadoSalvar Salvar(List<Entidade> entidades);

        //ResultadoExcluir Excluir(params Entidade[] entidades);

        //ResultadoExcluir Exclur(ListaEntidades<Entidade> entidades);

        //ResultadoExcluir Excluir(List<Entidade> entidades, string relacoesEmCascata);

        //DateTime RetornarDataHora(bool utc = true);
    }
}
