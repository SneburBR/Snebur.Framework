using Microsoft.Web.WebSockets;
using System;
using System.Reflection;
using Snebur.Dominio;
using Snebur.Utilidade;

namespace Snebur.Comunicacao.Mensageiro
{
    public abstract partial class ConexaoMensageiro : WebSocketHandler, IBaseConexaoMensageiro, IIdentificador
    {
        public IUsuario Usuario { get; protected set; }

        public ISessaoUsuario SessaoUsuario { get; protected set; }

        public Guid IdentificadorSessaoUsuario { get; }

        public Guid identificadorUnicoConexao { get; }

        public Guid IdentificadorUsuario => this.Usuario.Identificador;

        public event EventoMensagemHandler EventoNovaConectado;

        public event EventoMensagemHandler EventoConexaoDesconectada;

        #region Construtor

        public ConexaoMensageiro(Guid identificadorSessaoUsuario,
                                 Guid identificadorUnicoConexao)
        {
            this.IdentificadorSessaoUsuario = identificadorSessaoUsuario;
            this.identificadorUnicoConexao = identificadorUnicoConexao;
        }
      

        
        #endregion

        protected void Notificar(MethodInfo methodBase, IUsuario destinario, BaseDominio valorParametro)
        {
            throw new NotImplementedException();
            //var mensagemBytes = this.RetornarMensagemBytes(methodBase, usario, valorParametro);
            //var sessoes = this.RetornarSessoes(usario);
            //foreach (var sessao in sessoes)
            //{
            //    sessao.Send(mensagemBytes);
            //}
        }

        private byte[] RetornarMensagemBytes(MethodInfo methodInfo,
                                             IUsuario usuario,
                                             BaseDominio valorParametro)
        {
            var contratoMensagem = this.RetornarContrato(methodInfo, usuario, valorParametro);
            var contratoSerializado = JsonUtil.Serializar(contratoMensagem);
            return PacoteUtil.CompactarPacote(contratoSerializado);
        }

        private ContratoMensageiro RetornarContrato(MethodInfo methodInfo, IUsuario usuario, BaseDominio valorParametro)
        {
            throw new NotImplementedException();
        }
        //private ConexaoMensageiro[] RetornarSessoes(IUsuario usuario)
        //{
        //    var contextosUsuario = this.Central.Mensageiros;
        //    if (contextosUsuario.ContainsKey(usuario.Identificador))
        //    {
        //        if (contextosUsuario.TryGetValue(usuario.Identificador, out DicionarioConexaoUsuario contextoUsuario))

        //            return contextoUsuario.TodosConexoes;
        //    }
        //    return Array.Empty<ConexaoMensageiro>();
        //}

        Guid IIdentificador.Identificador => this.identificadorUnicoConexao;
    }
}