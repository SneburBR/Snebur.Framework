using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snebur.Comunicacao.Mensageiro
{
    public abstract partial class ConexaoMensageiro<TCentral> : ConexaoMensageiro where TCentral : ICentral
    {
        public abstract Guid IdentificadorCentral { get; }

        public TCentral Central => Mensageiro.Centrais.RetornarCentral<TCentral>(this.IdentificadorCentral);


        public ConexaoMensageiro(Guid identificadorSessaoUsuario,
                                 Guid identificadorUnicoConexao) :
                                 base(identificadorSessaoUsuario, identificadorUnicoConexao)
        {

        }

        public override void OnOpen()
        {
            base.OnOpen();
            this.Central.AdicionarNovaConexao(this);
        }

        public override void OnMessage(byte[] mensagemBytes)
        {
            this.PrecoessarMensagem(mensagemBytes);
            base.OnMessage(mensagemBytes);
        }
        public override void OnClose()
        {
            this.Central.RemoverConexao(this);
        }


        private void PrecoessarMensagem(byte[] mensagemBytes)
        {
            var bytesNormalizado = mensagemBytes.Reverse().ToArray();

            throw new NotImplementedException();
        }
    }
}
