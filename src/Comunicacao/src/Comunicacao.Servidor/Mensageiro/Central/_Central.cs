using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Snebur.Dominio;

namespace Snebur.Comunicacao.Mensageiro
{


    public abstract partial class Central<TConexao> : BaseDicionarioConexao<DicionarioConexaoUsuario<TConexao>, TConexao>, ICentral, ICentral<TConexao>
                                                      where TConexao : ConexaoMensageiro
    {

        public Dictionary<Guid, DicionarioConexaoUsuario<TConexao>> DicionarioConexaoUsuario => this.Dicionario;

        public override TConexao[] TodasConexoes => this.DicionarioConexaoUsuario.SelectMany(x => x.Value.TodasConexoes).ToArray();

        protected Central(Guid identificador) : base(identificador)
        {

        }

        public void AdicionarNovaConexao(TConexao conexao)
        {
            var dicionarioConexaoUsuario = this.RetornarDicionarioConexoes(conexao.IdentificadorUsuario);
            var dicionarioConexaoSessaoUsuario = dicionarioConexaoUsuario.RetornarDicionarioConexoes(conexao.IdentificadorSessaoUsuario);
            dicionarioConexaoSessaoUsuario.AdicionaConexao(conexao);
        }

        public void RemoverConexao(TConexao conexao)
        {
            var dicionarioConexaoUsuario = this.RetornarDicionarioConexoes(conexao.IdentificadorUsuario);
            var dicionarioSessaoUsuario = dicionarioConexaoUsuario.RetornarDicionarioConexoes(conexao.IdentificadorSessaoUsuario);
            dicionarioSessaoUsuario.RemoverConexao(conexao);

        }
         
        #region ICentral 

        ConexaoMensageiro[] ICentral.TodasConexoes => this.TodasConexoes.Cast<ConexaoMensageiro>().ToArray();

        void ICentral.AdicionarNovaConexao(ConexaoMensageiro conexao)
        {
            this.AdicionarNovaConexao((TConexao)conexao);
        }

        void ICentral.RemoverConexao(ConexaoMensageiro conexao)
        {
            this.RemoverConexao((TConexao)conexao);
        }

        #endregion



    }
}