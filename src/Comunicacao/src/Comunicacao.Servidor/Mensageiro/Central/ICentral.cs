using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Snebur.Dominio;

namespace Snebur.Comunicacao.Mensageiro
{
    public interface ICentral : IIdentificador
    {
        ConexaoMensageiro[] TodasConexoes { get; }

        void AdicionarNovaConexao(ConexaoMensageiro conexao);

        void RemoverConexao(ConexaoMensageiro conexao);
    }

    public interface ICentral<TConexao> : ICentral where TConexao : ConexaoMensageiro
    {
        new TConexao[] TodasConexoes { get; }

        void AdicionarNovaConexao(TConexao conexao);

        void RemoverConexao(TConexao conexao);
    }
}